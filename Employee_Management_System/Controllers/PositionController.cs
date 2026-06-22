using Employee_Management_System.Data;
using Employee_Management_System.Models;
using Employee_Management_System.ViewModels.Position;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Controllers
{
    [Authorize(Policy = Permissions.Positions.View)]
    public class PositionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PositionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Position
        public async Task<IActionResult> Index(string? search, int? departmentId)
        {
            var query = _context.Positions
                .Include(p => p.Department)
                .Include(p => p.Employees)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Title.Contains(search));

            if (departmentId.HasValue)
                query = query.Where(p => p.DepartmentId == departmentId.Value);

            var positions = await query
                .Select(p => new PositionListItemViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    DepartmentName = p.Department != null ? p.Department.Name : null,
                    EmployeeCount = p.Employees.Count
                })
                .OrderBy(p => p.Title)
                .ToListAsync();

            return View(new PositionListViewModel
            {
                Positions = positions,
                Search = search,
                FilterDepartmentId = departmentId,
                Departments = await GetDepartmentsAsync()
            });
        }

        // GET: /Position/Create
        [Authorize(Policy = Permissions.Positions.Create)]
        public async Task<IActionResult> Create()
        {
            return View(new PositionFormViewModel
            {
                Departments = await GetDepartmentsAsync()
            });
        }

        // POST: /Position/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.Positions.Create)]
        public async Task<IActionResult> Create(PositionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Departments = await GetDepartmentsAsync();
                return View(model);
            }

            var position = new Position
            {
                Title = model.Title,
                DepartmentId = model.DepartmentId == 0 ? null : model.DepartmentId
            };

            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Position \"{position.Title}\" created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Position/Edit/5
        [Authorize(Policy = Permissions.Positions.Edit)]
        public async Task<IActionResult> Edit(int id)
        {
            var position = await _context.Positions.FindAsync(id);
            if (position == null) return NotFound();

            return View(new PositionFormViewModel
            {
                Id = position.Id,
                Title = position.Title,
                DepartmentId = position.DepartmentId,
                Departments = await GetDepartmentsAsync()
            });
        }

        // POST: /Position/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.Positions.Edit)]
        public async Task<IActionResult> Edit(int id, PositionFormViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                model.Departments = await GetDepartmentsAsync();
                return View(model);
            }

            var position = await _context.Positions.FindAsync(id);
            if (position == null) return NotFound();

            position.Title = model.Title;
            position.DepartmentId = model.DepartmentId == 0 ? null : model.DepartmentId;

            _context.Positions.Update(position);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Position \"{position.Title}\" updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Position/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.Positions.Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            var position = await _context.Positions
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (position == null) return NotFound();

            if (position.Employees.Any())
            {
                TempData["Error"] = $"Cannot delete \"{position.Title}\" — it has {position.Employees.Count} employee(s) assigned.";
                return RedirectToAction(nameof(Index));
            }

            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Position \"{position.Title}\" deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<DepartmentSelectItem>> GetDepartmentsAsync()
        {
            return await _context.Departments
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentSelectItem { Id = d.Id, Name = d.Name })
                .ToListAsync();
        }
    }
}
