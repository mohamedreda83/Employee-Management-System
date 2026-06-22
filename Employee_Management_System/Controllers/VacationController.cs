using Employee_Management_System.Data;
using Employee_Management_System.Models;
using Employee_Management_System.ViewModels.Vacation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Controllers
{
    [Authorize]
    public class VacationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VacationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> RequestForm()
        {
            var employeeId = await GetCurrentEmployeeIdAsync();
            var balances = await _context.VacationBalances
                .Include(vb => vb.VacationType)
                .Where(vb => vb.EmployeeId == employeeId)
                .ToListAsync();

            var requests = await _context.Vacations
                .Include(v => v.VacationType)
                .Where(v => v.EmployeeId == employeeId)
                .OrderByDescending(v => v.StartDate)
                .Select(v => new VacationRequestItemViewModel
                {
                    Id = v.Id,
                    VacationType = v.VacationType!.Name,
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    DaysRequested = v.DaysRequested,
                    Status = v.Status,
                    Reason = v.Reason
                })
                .ToListAsync();

            var model = new VacationRequestViewModel
            {
                BalanceCards = balances.Select(vb => new VacationBalanceCardViewModel
                {
                    VacationType = vb.VacationType!.Name,
                    EntitledDays = vb.EntitledDays,
                    UsedDays = vb.UsedDays,
                    RemainingDays = vb.EntitledDays - vb.UsedDays
                }).ToList(),
                VacationTypes = await _context.VacationTypes.Select(vt => new VacationTypeSelectItem
                {
                    Id = vt.Id,
                    Name = vt.Name
                }).ToListAsync(),
                Requests = requests,
                NewRequest = new VacationRequestFormModel
                {
                    EmployeeId = employeeId,
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow)
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRequest([Bind(Prefix = "NewRequest")] VacationRequestFormModel form)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(RequestForm));
            }

            var employeeId = await GetCurrentEmployeeIdAsync();
            var vacation = new Vacation
            {
                EmployeeId = employeeId,
                VacationTypeId = form.VacationTypeId,
                StartDate = form.StartDate,
                EndDate = form.EndDate,
                DaysRequested = form.DaysRequested,
                Reason = form.Reason,
                Status = "Pending"
            };

            _context.Vacations.Add(vacation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(RequestForm));
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