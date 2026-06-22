using Employee_Management_System.Data;
using Employee_Management_System.Models;
using Employee_Management_System.ViewModels.UserManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Controllers
{
    [Authorize(Policy = Permissions.UserManagement.View)]
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Roles available in this system
        private static readonly string[] SystemRoles = { "Admin", "HR", "Employee" };

        public UserManagementController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ─── INDEX ────────────────────────────────────────────────────────────────
        public async Task<IActionResult> Index(string? search)
        {
            // Ensure all system roles exist
            foreach (var role in SystemRoles)
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.ApplicationUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e =>
                    e.FullName.Contains(search) ||
                    e.EmployeeNumber.Contains(search) ||
                    (e.Email != null && e.Email.Contains(search)));

            var employees = await query.OrderBy(e => e.FullName).ToListAsync();

            var rows = new List<UserRowViewModel>();
            foreach (var emp in employees)
            {
                var row = new UserRowViewModel
                {
                    EmployeeId = emp.Id,
                    EmployeeNumber = emp.EmployeeNumber,
                    FullName = emp.FullName,
                    Email = emp.Email,
                    DepartmentName = emp.Department?.Name,
                    EmploymentStatus = emp.EmploymentStatus
                };

                if (emp.ApplicationUser != null)
                {
                    row.UserId = emp.ApplicationUser.Id;
                    row.Roles = await _userManager.GetRolesAsync(emp.ApplicationUser);
                    row.IsLockedOut = emp.ApplicationUser.LockoutEnd.HasValue &&
                                     emp.ApplicationUser.LockoutEnd > DateTimeOffset.UtcNow;
                }

                rows.Add(row);
            }

            return View(new UserManagementListViewModel { Rows = rows, Search = search });
        }

        // ─── CREATE ACCOUNT ───────────────────────────────────────────────────────
        public async Task<IActionResult> Create(int employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null) return NotFound();
            if (employee.ApplicationUser != null)
            {
                TempData["Error"] = $"{employee.FullName} already has an account.";
                return RedirectToAction(nameof(Index));
            }

            return View(new CreateUserViewModel
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.FullName,
                EmployeeNumber = employee.EmployeeNumber,
                Email = employee.Email ?? string.Empty,
                AvailableRoles = SystemRoles,
                SelectedRoles = new List<string> { "Employee" }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = SystemRoles;
                return View(model);
            }

            var employee = await _context.Employees.FindAsync(model.EmployeeId);
            if (employee == null) return NotFound();

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                EmployeeId = model.EmployeeId,
                Roles = string.Join(",", model.SelectedRoles)
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);
                model.AvailableRoles = SystemRoles;
                return View(model);
            }

            if (model.SelectedRoles.Any())
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);

            TempData["Success"] = $"Account created for {employee.FullName}.";
            return RedirectToAction(nameof(Index));
        }

        // ─── EDIT ROLES / LOCK ────────────────────────────────────────────────────
        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var employee = await _context.Employees.FindAsync(user.EmployeeId);
            var roles = await _userManager.GetRolesAsync(user);

            return View(new EditUserViewModel
            {
                UserId = user.Id,
                Email = user.Email!,
                EmployeeName = employee?.FullName ?? "—",
                EmployeeNumber = employee?.EmployeeNumber ?? "—",
                IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow,
                SelectedRoles = roles.ToList(),
                AvailableRoles = SystemRoles
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            // Update roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (model.SelectedRoles.Any())
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);

            // Update lock-out
            if (model.IsLockedOut)
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            else
                await _userManager.SetLockoutEndDateAsync(user, null);

            // Sync Roles string on ApplicationUser
            user.Roles = string.Join(",", model.SelectedRoles);
            await _userManager.UpdateAsync(user);

            TempData["Success"] = $"Account for {model.EmployeeName} updated.";
            return RedirectToAction(nameof(Index));
        }

        // ─── RESET PASSWORD ───────────────────────────────────────────────────────
        public async Task<IActionResult> ResetPassword(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var employee = await _context.Employees.FindAsync(user.EmployeeId);

            return View(new ResetPasswordAdminViewModel
            {
                UserId = user.Id,
                Email = user.Email!,
                EmployeeName = employee?.FullName ?? "—"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordAdminViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);
                return View(model);
            }

            TempData["Success"] = $"Password reset for {model.EmployeeName}.";
            return RedirectToAction(nameof(Index));
        }

        // ─── DELETE ACCOUNT ───────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var employee = await _context.Employees.FindAsync(user.EmployeeId);
            var name = employee?.FullName ?? user.Email;

            await _userManager.DeleteAsync(user);

            TempData["Success"] = $"Account for {name} has been removed.";
            return RedirectToAction(nameof(Index));
        }
    }
}
