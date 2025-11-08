using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MLM_Web_App.Models;
using System.Collections.Generic;
using System.Linq;

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
            var role = HttpContext.Session.GetString("UserRole");

            // Only allow Admin
            if (string.IsNullOrEmpty(role))
            {
                TempData["Error"] = "Session expired. Please log in again.";
                return RedirectToAction("Index", "Login");
            }

            // Load users including Sponsor & downline
            var users = _context.Users
                .Include(u => u.Sponsor)
                .Include(u => u.InverseSponsor)
                .ToList();

            ViewBag.AdminName = HttpContext.Session.GetString("UserName") ?? "Admin";

            return View(users);
        }

        // Toggle active/inactive user
        public IActionResult ToggleStatus(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(role))
            {
                TempData["Error"] = "Session expired. Please log in again.";
                return RedirectToAction("Index", "Login");
            }

            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                _context.SaveChanges();
                TempData["Success"] = $"User {(user.IsActive ? "activated" : "deactivated")} successfully.";
            }
            else
            {
                TempData["Error"] = "User not found.";
            }

            return RedirectToAction("Users");
        }
    }
}
