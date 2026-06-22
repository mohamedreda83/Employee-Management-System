using Employee_Management_System.Data;
using Employee_Management_System.Models;
using Employee_Management_System.ViewModels.Attendance;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttendanceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> EmployeeAttendance()
        {
            var employeeId = await GetCurrentEmployeeIdAsync();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var attendance = await _context.Attendances
                .Where(a => a.EmployeeId == employeeId && a.Date == today)
                .FirstOrDefaultAsync();

            var monthlyRecords = await _context.Attendances
                .Where(a => a.EmployeeId == employeeId && a.Date.Year == today.Year && a.Date.Month == today.Month)
                .OrderBy(a => a.Date)
                .Select(a => new AttendanceMonthRecordViewModel
                {
                    Date = a.Date,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    Status = a.Status,
                    LateMinutes = a.LateMinutes,
                    EarlyLeaveMinutes = a.EarlyLeaveMinutes,
                    OvertimeMinutes = a.OvertimeMinutes
                })
                .ToListAsync();

            var model = new EmployeeAttendanceViewModel
            {
                CurrentDate = today,
                CurrentTime = DateTime.UtcNow.ToLocalTime().TimeOfDay,
                IsCheckedIn = attendance?.CheckInTime != null && attendance?.CheckOutTime == null,
                IsCheckedOut = attendance?.CheckOutTime != null,
                TodayRecord = attendance != null ? new AttendanceMonthRecordViewModel
                {
                    Date = attendance.Date,
                    CheckInTime = attendance.CheckInTime,
                    CheckOutTime = attendance.CheckOutTime,
                    Status = attendance.Status,
                    LateMinutes = attendance.LateMinutes,
                    EarlyLeaveMinutes = attendance.EarlyLeaveMinutes,
                    OvertimeMinutes = attendance.OvertimeMinutes
                } : null,
                MonthlyRecords = monthlyRecords
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn()
        {
            var employeeId = await GetCurrentEmployeeIdAsync();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == today);

            if (attendance == null)
            {
                attendance = new Attendance
                {
                    EmployeeId = employeeId,
                    Date = today,
                    CheckInTime = TimeOnly.FromDateTime(DateTime.UtcNow),
                    Status = "Present"
                };
                _context.Attendances.Add(attendance);
            }
            else
            {
                attendance.CheckInTime = TimeOnly.FromDateTime(DateTime.UtcNow);
                attendance.Status = "Present";
                _context.Attendances.Update(attendance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EmployeeAttendance));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut()
        {
            var employeeId = await GetCurrentEmployeeIdAsync();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == today);

            if (attendance == null)
            {
                return BadRequest("Check-in record not found for today.");
            }

            attendance.CheckOutTime = TimeOnly.FromDateTime(DateTime.UtcNow);
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EmployeeAttendance));
        }

        [Authorize(Policy = Permissions.Attendance.Review)]
        public async Task<IActionResult> Review()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .ToListAsync();

            var model = new AttendanceReviewViewModel
            {
                Employees = employees.Select(e => new AttendanceEmployeeItemViewModel
                {
                    EmployeeId = e.Id,
                    EmployeeNumber = e.EmployeeNumber,
                    FullName = e.FullName,
                    Department = e.Department?.Name,
                    Position = e.Position?.Title
                }).ToList(),
                StatusFilter = "All",
                Date = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.Attendance.Review)]
        public async Task<IActionResult> ReviewFilter(DateOnly date, string status)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                .ThenInclude(e => e.Department)
                .Include(a => a.Employee)
                .ThenInclude(e => e.Position)
                .Where(a => a.Date == date)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = query.Where(a => a.Status == status);
            }

            var records = await query
                .Select(a => new AttendanceReviewRecordViewModel
                {
                    AttendanceId = a.Id,
                    EmployeeId = a.EmployeeId,
                    EmployeeNumber = a.Employee != null ? a.Employee.EmployeeNumber : string.Empty,
                    FullName = a.Employee != null ? a.Employee.FullName : string.Empty,
                    Department = a.Employee != null && a.Employee.Department != null ? a.Employee.Department.Name : null,
                    Position = a.Employee != null && a.Employee.Position != null ? a.Employee.Position.Title : null,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    Status = a.Status,
                    LateMinutes = a.LateMinutes,
                    EarlyLeaveMinutes = a.EarlyLeaveMinutes,
                    OvertimeMinutes = a.OvertimeMinutes,
                    ManualReason = string.Empty
                })
                .ToListAsync();

            var model = new AttendanceReviewViewModel
            {
                Date = date,
                StatusFilter = status,
                AttendanceRecords = records,
                StatusOptions = new[] { "All", "Present", "Absent", "Late", "On Leave" }
            };

            return View("Review", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.Attendance.Review)]
        public async Task<IActionResult> UpdateAttendance(int id, string status, string? manualReason)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            attendance.Status = status;
            attendance.Reason = manualReason;
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Review));
        }

        private async Task<int> GetCurrentEmployeeIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new InvalidOperationException("Authenticated user is required.");
            }
            return user.EmployeeId;
        }
    }
}