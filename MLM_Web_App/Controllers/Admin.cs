using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MLM_Web_App.Models;

namespace MLM_Web_App.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Users
        public IActionResult Users()
        {
            // Include parent (Sponsor) and direct referrals (InverseSponsor)
            var users = _context.Users
                .Include(u => u.Sponsor)
                .Include(u => u.InverseSponsor)
                .ToList();

            return View(users);
        }

        // Toggle active/inactive status
        public IActionResult ToggleStatus(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                _context.SaveChanges();
            }
            return RedirectToAction("Users");
        }
    }
}
