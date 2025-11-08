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
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(role))
            {
                TempData["Error"] = "Session expired. Please log in again.";
                return RedirectToAction("Index", "Login");
            }

            // ✅ Admin can view dashboard also
            if (role == "Admin")
            {
                ViewBag.UserName = "Administrator";
                return View();
            }

            // ✅ For normal user
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Login");

            var user = _context.Users
                .Include(u => u.InverseSponsor)
                    .ThenInclude(l1 => l1.InverseSponsor)
                        .ThenInclude(l2 => l2.InverseSponsor)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserCode = HttpContext.Session.GetString("UserCode");

            return View(user);
        }
    }
}
