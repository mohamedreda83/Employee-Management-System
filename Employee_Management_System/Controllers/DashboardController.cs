using Employee_Management_System.Data;
using Employee_Management_System.Models;
using Employee_Management_System.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Policy = Permissions.Dashboard.View)]
        public async Task<IActionResult> Index()
        {
            var totalEmployees = await _context.Employees.CountAsync();
            var presentCount = await _context.Attendances
                .CountAsync(a => a.Date == DateOnly.FromDateTime(DateTime.UtcNow) && a.CheckInTime != null && a.CheckOutTime == null);
            var absentCount = await _context.Attendances
                .CountAsync(a => a.Date == DateOnly.FromDateTime(DateTime.UtcNow) && a.CheckInTime == null);
            var onLeaveCount = await _context.Vacations
                .CountAsync(v => v.StartDate <= DateOnly.FromDateTime(DateTime.UtcNow) && v.EndDate >= DateOnly.FromDateTime(DateTime.UtcNow) && v.Status == "Approved");

            var departmentStats = await _context.Departments
                .Select(d => new DepartmentChartItem
                {
                    DepartmentName = d.Name,
                    EmployeeCount = d.Employees.Count
                })
                .ToListAsync();

            var recentActivities = await _context.Attendances
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.CheckInTime)
                .Take(10)
                .Select(a => new ActivityItem
                {
                    EmployeeName = a.Employee!.FullName,
                    ActivityType = a.CheckInTime != null ? "Checked in" : "Absent",
                    ActivityTime = a.CheckInTime.HasValue ? a.CheckInTime.Value.ToString() : string.Empty,
                    Date = a.Date
                })
                .ToListAsync();

            var notifications = await _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .Select(n => new NotificationItem
                {
                    Title = n.Title,
                    Body = n.Body,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            var model = new DashboardViewModel
            {
                TotalEmployees = totalEmployees,
                PresentCount = presentCount,
                AbsentCount = absentCount,
                OnLeaveCount = onLeaveCount,
                DepartmentChart = departmentStats,
                RecentActivities = recentActivities,
                Notifications = notifications,
                QuickActions = new[] { "Add Employee", "Add Vacation", "View Reports" }
            };

            return View(model);
        }

        [Authorize(Policy = Permissions.Dashboard.ViewEmployee)]
        public async Task<IActionResult> EmployeeIndex()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.EmployeeId == null)
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var employeeId = user.EmployeeId;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var firstDayOfMonth = new DateOnly(today.Year, today.Month, 1);

            var presentDays = await _context.Attendances
                .CountAsync(a => a.EmployeeId == employeeId && a.Date >= firstDayOfMonth && a.Date <= today && a.CheckInTime != null);

            var absentDays = await _context.Attendances
                .CountAsync(a => a.EmployeeId == employeeId && a.Date >= firstDayOfMonth && a.Date <= today && a.CheckInTime == null);

            var leaveDaysList = await _context.Vacations
                .Include(v => v.VacationType)
                .Where(v => v.EmployeeId == employeeId && v.Status == "Approved" && v.StartDate <= today && v.EndDate >= firstDayOfMonth)
                .ToListAsync();

            int leaveDays = 0;
            var vacationDetails = new List<EmployeeVacationDetail>();
            foreach (var leave in leaveDaysList)
            {
                var start = leave.StartDate < firstDayOfMonth ? firstDayOfMonth : leave.StartDate;
                var end = leave.EndDate > today ? today : leave.EndDate;
                var days = end.DayNumber - start.DayNumber + 1;
                leaveDays += days;

                vacationDetails.Add(new EmployeeVacationDetail
                {
                    VacationType = leave.VacationType?.Name ?? "Leave",
                    StartDate = leave.StartDate,
                    EndDate = leave.EndDate,
                    Days = days,
                    Status = leave.Status,
                    Reason = leave.Reason
                });
            }

            var model = new EmployeeDashboardViewModel
            {
                PresentDays = presentDays,
                AbsentDays = absentDays,
                LeaveDays = leaveDays,
                TotalExpectedDays = today.Day - firstDayOfMonth.Day + 1,
                VacationDetails = vacationDetails
            };

            return View(model);
        }
    }
}