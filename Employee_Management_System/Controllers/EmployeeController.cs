using Employee_Management_System.Data;
using Employee_Management_System.Models;
using Employee_Management_System.ViewModels.Employee;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Controllers
{
    [Authorize(Policy = Permissions.Employees.View)]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int? departmentId, int? positionId, string? status)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e => e.FullName.Contains(search) || e.EmployeeNumber.Contains(search) || (e.Email != null && e.Email.Contains(search)));
            }

            if (departmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == departmentId.Value);
            }

            if (positionId.HasValue)
            {
                query = query.Where(e => e.PositionId == positionId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(e => e.EmploymentStatus == status);
            }

            var employees = await query
                .Select(e => new EmployeeListItemViewModel
                {
                    Id = e.Id,
                    EmployeeNumber = e.EmployeeNumber,
                    FullName = e.FullName,
                    ProfilePhoto = e.ProfilePhoto,
                    Department = e.Department != null ? e.Department.Name : string.Empty,
                    Position = e.Position != null ? e.Position.Title : string.Empty,
                    EmploymentStatus = e.EmploymentStatus ?? string.Empty,
                    Email = e.Email,
                    Phone = e.Phone
                })
                .ToListAsync();

            var model = new EmployeeListViewModel
            {
                Search = search,
                DepartmentId = departmentId,
                PositionId = positionId,
                Status = status,
                Employees = employees,
                Departments = await _context.Departments.Select(d => new SelectItem { Id = d.Id, Name = d.Name }).ToListAsync(),
                Positions = await _context.Positions.Select(p => new SelectItem { Id = p.Id, Name = p.Title }).ToListAsync(),
                Statuses = new[] { "Active", "Inactive", "On Leave" }
            };

            return View(model);
        }

        [Authorize(Policy = Permissions.Employees.Create)]
        public async Task<IActionResult> Create()
        {
            var model = new EmployeeEditViewModel
            {
                Departments = await _context.Departments.Select(d => new SelectItem { Id = d.Id, Name = d.Name }).ToListAsync(),
                Positions = await _context.Positions.Select(p => new SelectItem { Id = p.Id, Name = p.Title }).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.Employees.Create)]
        public async Task<IActionResult> Create(EmployeeEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Departments = await _context.Departments.Select(d => new SelectItem { Id = d.Id, Name = d.Name }).ToListAsync();
                model.Positions = await _context.Positions.Select(p => new SelectItem { Id = p.Id, Name = p.Title }).ToListAsync();
                return View(model);
            }

            var employee = new Employee
            {
                EmployeeNumber = model.EmployeeNumber,
                FullName = model.FullName,
                ProfilePhoto = model.ProfilePhoto,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                NationalId = model.NationalId,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                DepartmentId = model.DepartmentId,
                PositionId = model.PositionId,
                Salary = model.Salary,
                HireDate = model.HireDate,
                EmploymentStatus = model.EmploymentStatus,
                Notes = model.Notes
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = Permissions.Employees.Edit)]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var model = new EmployeeEditViewModel
            {
                Id = employee.Id,
                EmployeeNumber = employee.EmployeeNumber,
                FullName = employee.FullName,
                ProfilePhoto = employee.ProfilePhoto,
                Gender = employee.Gender,
                DateOfBirth = employee.DateOfBirth,
                NationalId = employee.NationalId,
                Email = employee.Email,
                Phone = employee.Phone,
                Address = employee.Address,
                DepartmentId = employee.DepartmentId,
                PositionId = employee.PositionId,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                EmploymentStatus = employee.EmploymentStatus,
                Notes = employee.Notes,
                Departments = await _context.Departments.Select(d => new SelectItem { Id = d.Id, Name = d.Name }).ToListAsync(),
                Positions = await _context.Positions.Select(p => new SelectItem { Id = p.Id, Name = p.Title }).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.Employees.Edit)]
        public async Task<IActionResult> Edit(EmployeeEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Departments = await _context.Departments.Select(d => new SelectItem { Id = d.Id, Name = d.Name }).ToListAsync();
                model.Positions = await _context.Positions.Select(p => new SelectItem { Id = p.Id, Name = p.Title }).ToListAsync();
                return View(model);
            }

            var employee = await _context.Employees.FindAsync(model.Id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.EmployeeNumber = model.EmployeeNumber;
            employee.FullName = model.FullName;
            employee.ProfilePhoto = model.ProfilePhoto;
            employee.Gender = model.Gender;
            employee.DateOfBirth = model.DateOfBirth;
            employee.NationalId = model.NationalId;
            employee.Email = model.Email;
            employee.Phone = model.Phone;
            employee.Address = model.Address;
            employee.DepartmentId = model.DepartmentId;
            employee.PositionId = model.PositionId;
            employee.Salary = model.Salary;
            employee.HireDate = model.HireDate;
            employee.EmploymentStatus = model.EmploymentStatus;
            employee.Notes = model.Notes;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Attendances)
                .Include(e => e.Vacations)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            var model = new EmployeeDetailsViewModel
            {
                Id = employee.Id,
                EmployeeNumber = employee.EmployeeNumber,
                FullName = employee.FullName,
                ProfilePhoto = employee.ProfilePhoto,
                Gender = employee.Gender,
                DateOfBirth = employee.DateOfBirth,
                NationalId = employee.NationalId,
                Email = employee.Email,
                Phone = employee.Phone,
                Address = employee.Address,
                Department = employee.Department?.Name,
                Position = employee.Position?.Title,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                EmploymentStatus = employee.EmploymentStatus,
                Notes = employee.Notes,
                AttendanceHistory = employee.Attendances.Select(a => new AttendanceHistoryItem
                {
                    Date = a.Date,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    Status = a.Status
                }).OrderByDescending(a => a.Date).ToList(),
                VacationHistory = employee.Vacations.Select(v => new VacationHistoryItem
                {
                    VacationType = v.VacationType?.Name,
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    DaysRequested = v.DaysRequested,
                    Status = v.Status
                }).OrderByDescending(v => v.StartDate).ToList()
            };

            return View(model);
        }
    }
}