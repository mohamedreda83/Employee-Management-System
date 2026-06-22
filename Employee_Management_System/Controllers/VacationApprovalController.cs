using Employee_Management_System.Data;
using Employee_Management_System.Models;
using Employee_Management_System.ViewModels.Vacation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Controllers
{
    [Authorize(Policy = Permissions.Vacations.Approve)]
    public class VacationApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VacationApprovalController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string tab = "Pending")
        {
            var query = _context.Vacations
                .Include(v => v.Employee)
                .Include(v => v.VacationType)
                .Include(v => v.ApprovedBy)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tab) && tab != "All")
            {
                query = query.Where(v => v.Status == tab);
            }

            var items = await query
                .OrderByDescending(v => v.StartDate)
                .Select(v => new VacationApprovalItemViewModel
                {
                    Id = v.Id,
                    EmployeeId = v.EmployeeId,
                    EmployeeName = v.Employee!.FullName,
                    VacationType = v.VacationType!.Name,
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    DaysRequested = v.DaysRequested,
                    Status = v.Status,
                    Reason = v.Reason,
                    ApprovedBy = v.ApprovedBy != null ? v.ApprovedBy.FullName : null,
                    DecisionComment = v.DecisionComment
                })
                .ToListAsync();

            var model = new VacationApprovalViewModel
            {
                CurrentTab = tab,
                Tabs = new[] { "Pending", "Approved", "Rejected" },
                Requests = items
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string? comment)
        {
            var request = await _context.Vacations.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Approved";
            request.ApprovedById = await GetCurrentEmployeeIdAsync();
            request.DecisionComment = comment;
            _context.Vacations.Update(request);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { tab = "Approved" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? comment)
        {
            var request = await _context.Vacations.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Rejected";
            request.ApprovedById = await GetCurrentEmployeeIdAsync();
            request.DecisionComment = comment;
            _context.Vacations.Update(request);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { tab = "Rejected" });
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