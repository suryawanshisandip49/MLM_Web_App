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
                {
                    builder.Append(b.ToString("x2")); // convert to hex string
                }
                return builder.ToString();
            }
        }

        // GET: Login
        public IActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter Email and Password.";
                return View();
            }
          
           // email = email.Trim().ToLower();
            string hashedPassword = HashPassword(password);
         

            var user = _context.Users.FirstOrDefault(u =>
    u.Email.ToLower().Trim() == email.ToLower().Trim() &&
    u.PasswordHash == hashedPassword &&
        u.IsActive);

            // Check user in database
            //var user = _context.Users
            //    .FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid Email or Password.";
                return View();
            }

            // Store user info in session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserCode", user.UserCode);
            HttpContext.Session.SetString("UserName", user.FullName);

            // Redirect to Dashboard
            return RedirectToAction("Index", "Dashboard");
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Remove all session data
            TempData["Success"] = "You have been logged out successfully!";
            return RedirectToAction("Index", "Login");
        }
    }
}
