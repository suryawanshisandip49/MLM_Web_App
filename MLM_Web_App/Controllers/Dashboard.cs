using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MLM_Web_App.Models;

namespace MLM_Web_App.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // ✅ Example: Fetch logged-in user
            // (Later you’ll get user ID from session/login)
           // int userId = 1; // Temporary demo value

            // ✅ Check if user logged in
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Include all 3 levels
            var user = _context.Users
                .Include(u => u.InverseSponsor)
                    .ThenInclude(l1 => l1.InverseSponsor)
                        .ThenInclude(l2 => l2.InverseSponsor)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }
            // ✅ Pass username and usercode to view
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserCode = HttpContext.Session.GetString("UserCode");

            return View(user);
        }
    }
}
