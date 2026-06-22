using Employee_Management_System.Data;
using Employee_Management_System.Models;
using Employee_Management_System.ViewModels.Department;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Controllers
{
    [Authorize(Policy = Permissions.Departments.View)]
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Department
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Employees)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(d => d.Name.Contains(search));
            }

            var departments = await query
                .Select(d => new DepartmentListItemViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    ManagerName = d.Manager != null ? d.Manager.FullName : null,
                    EmployeeCount = d.Employees.Count
                })
                .OrderBy(d => d.Name)
                .ToListAsync();

            return View(new DepartmentListViewModel
            {
                Departments = departments,
                Search = search
            });
        }

        // GET: /Department/Create
        [Authorize(Policy = Permissions.Departments.Create)]
        public async Task<IActionResult> Create()
        {
            var model = new DepartmentFormViewModel
            {
                Managers = await GetManagersAsync()
            };
            return View(model);
        }

        // POST: /Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.Departments.Create)]
        public async Task<IActionResult> Create(DepartmentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Managers = await GetManagersAsync();
                return View(model);
            }

            var department = new Department
            {
                Name = model.Name,
                ManagerId = model.ManagerId == 0 ? null : model.ManagerId
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Department \"{department.Name}\" created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Department/Edit/5
        [Authorize(Policy = Permissions.Departments.Edit)]
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();

            var model = new DepartmentFormViewModel
            {
                Id = department.Id,
                Name = department.Name,
                ManagerId = department.ManagerId,
                Managers = await GetManagersAsync()
            };

            return View(model);
        }

        // POST: /Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.Departments.Edit)]
        public async Task<IActionResult> Edit(int id, DepartmentFormViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                model.Managers = await GetManagersAsync();
                return View(model);
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();

            department.Name = model.Name;
            department.ManagerId = model.ManagerId == 0 ? null : model.ManagerId;

            _context.Departments.Update(department);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Department \"{department.Name}\" updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Department/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.Departments.Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null) return NotFound();

            if (department.Employees.Any())
            {
                TempData["Error"] = $"Cannot delete \"{department.Name}\" — it has {department.Employees.Count} employee(s) assigned.";
                return RedirectToAction(nameof(Index));
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Department \"{department.Name}\" deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<ManagerSelectItem>> GetManagersAsync()
        {
            return await _context.Employees
                .OrderBy(e => e.FullName)
                .Select(e => new ManagerSelectItem { Id = e.Id, FullName = e.FullName })
                .ToListAsync();
        }
    }
}
