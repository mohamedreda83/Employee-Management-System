using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Controllers
{
    public class HomeController : Controller
    {
        // Program.cs registers app.UseExceptionHandler("/Home/Error") for production.
        // Without this controller, any unhandled exception would itself 404 instead
        // of showing the friendly error page in Views/Shared/Error.cshtml.
        [Route("/Home/Error")]
        public IActionResult Error()
        {
            var requestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View("Error", requestId);
        }
    }
}
