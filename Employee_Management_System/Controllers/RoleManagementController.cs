using Employee_Management_System.Models;
using Employee_Management_System.ViewModels.RoleManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Employee_Management_System.Controllers
{
    [Authorize(Policy = Permissions.RoleManagement.View)]
    public class RoleManagementController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleManagementController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var model = new RoleListViewModel();
            var roleItems = new List<RoleItemViewModel>();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                roleItems.Add(new RoleItemViewModel
                {
                    Id = role.Id,
                    Name = role.Name!,
                    UsersCount = usersInRole.Count
                });
            }

            model.Roles = roleItems.OrderBy(r => r.Name);
            return View(model);
        }

        [Authorize(Policy = Permissions.RoleManagement.Manage)]
        public IActionResult Create()
        {
            return View(new RoleFormViewModel
            {
                AvailablePermissions = GetAvailablePermissionsGrouped()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.RoleManagement.Manage)]
        public async Task<IActionResult> Create(RoleFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailablePermissions = GetAvailablePermissionsGrouped();
                return View(model);
            }

            if (await _roleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError(string.Empty, "Role already exists.");
                model.AvailablePermissions = GetAvailablePermissionsGrouped();
                return View(model);
            }

            var role = new IdentityRole(model.Name);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                foreach (var permission in model.SelectedPermissions)
                {
                    await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                }
                TempData["Success"] = $"Role '{role.Name}' created successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var err in result.Errors) ModelState.AddModelError(string.Empty, err.Description);
            model.AvailablePermissions = GetAvailablePermissionsGrouped();
            return View(model);
        }

        [Authorize(Policy = Permissions.RoleManagement.Manage)]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            var claims = await _roleManager.GetClaimsAsync(role);
            var selectedPerms = claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();

            var model = new RoleFormViewModel
            {
                Id = role.Id,
                Name = role.Name!,
                SelectedPermissions = selectedPerms,
                AvailablePermissions = GetAvailablePermissionsGrouped()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.RoleManagement.Manage)]
        public async Task<IActionResult> Edit(string id, RoleFormViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                model.AvailablePermissions = GetAvailablePermissionsGrouped();
                return View(model);
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            if (role.Name != model.Name && await _roleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError(string.Empty, "Another role with that name already exists.");
                model.AvailablePermissions = GetAvailablePermissionsGrouped();
                return View(model);
            }

            // Keep 'Admin' named Admin.
            if (role.Name == "Admin" && model.Name != "Admin")
            {
                ModelState.AddModelError(string.Empty, "The Admin role name cannot be changed.");
                model.AvailablePermissions = GetAvailablePermissionsGrouped();
                return View(model);
            }

            role.Name = model.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                var existingClaims = await _roleManager.GetClaimsAsync(role);
                var permissionClaims = existingClaims.Where(c => c.Type == "Permission").ToList();
                
                foreach (var claim in permissionClaims)
                    await _roleManager.RemoveClaimAsync(role, claim);

                foreach (var permission in model.SelectedPermissions)
                    await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));

                TempData["Success"] = $"Role '{role.Name}' updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var err in result.Errors) ModelState.AddModelError(string.Empty, err.Description);
            model.AvailablePermissions = GetAvailablePermissionsGrouped();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.RoleManagement.Manage)]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            if (usersInRole.Any())
            {
                TempData["Error"] = $"Cannot delete role '{role.Name}' because {usersInRole.Count} user(s) are assigned to it.";
                return RedirectToAction(nameof(Index));
            }

            if (role.Name == "Admin")
            {
                TempData["Error"] = "The Admin role cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            await _roleManager.DeleteAsync(role);
            TempData["Success"] = $"Role '{role.Name}' was deleted.";
            return RedirectToAction(nameof(Index));
        }

        private Dictionary<string, List<string>> GetAvailablePermissionsGrouped()
        {
            var grouped = new Dictionary<string, List<string>>();
            foreach (var perm in Permissions.All)
            {
                var groupName = perm.Split('.')[0];
                if (!grouped.ContainsKey(groupName))
                    grouped[groupName] = new List<string>();
                grouped[groupName].Add(perm);
            }
            return grouped;
        }
    }
}
