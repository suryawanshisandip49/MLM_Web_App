using Microsoft.AspNetCore.Mvc;
using MLM_Web_App.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MLM_Web_App.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        // ✅ GET: Login
        public IActionResult Index()
        {
            return View();
        }

        // ✅ POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter Email and Password.";
                return View();
            }

            string normalizedEmail = email.Trim().ToLower();
            string trimmedPassword = password.Trim();

            // ✅ Admin Login
            if (normalizedEmail == "admin@mlm.com" && trimmedPassword == "admin123")
            {
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("UserName", "Administrator");

                return RedirectToAction("Users", "Admin");
            }

            // ✅ Regular User Login
            string hashedPassword = HashPassword(trimmedPassword);

            var user = _context.Users.FirstOrDefault(u =>
                u.Email.ToLower().Trim() == normalizedEmail &&
                u.PasswordHash == hashedPassword &&
                u.IsActive);

            if (user == null)
            {
                ViewBag.Error = "Invalid Email or Password.";
                return View();
            }

            HttpContext.Session.Clear();
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserCode", user.UserCode);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", "User");

            return RedirectToAction("Index", "Dashboard");
        }

        // ✅ Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been logged out successfully!";
            return RedirectToAction("Index");
        }
    }
}
