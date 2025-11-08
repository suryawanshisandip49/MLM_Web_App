using Microsoft.AspNetCore.Mvc;
using MLM_Web_App.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MLM_Web_App.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegistrationController(ApplicationDbContext context)
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
        // GET: /Registration/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Registration/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User model)
        {
            //if (!ModelState.IsValid)
            //    return View(model);

            ModelState.Remove("UserCode");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Error = x.Value.Errors.First().ErrorMessage })
                    .ToList();

                // Log or inspect errors
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(errors));

                ViewBag.Error = "Model validation failed.";
                return View(model);
            }

            // ✅ Validate sponsor (optional)
          //  int? sponsorId = null;
            //if (!string.IsNullOrEmpty(model.SponsorUserCode))
            //{
            //    var sponsor = _context.Users.FirstOrDefault(u => u.UserCode == model.SponsorUserCode);
            //    if (sponsor == null)
            //    {
            //        ModelState.AddModelError("SponsorUserCode", "Invalid Sponsor Code");
            //        return View(model);
            //    }
            //    sponsorId = sponsor.Id;
            //}

            var existingUser = _context.Users
          .FirstOrDefault(u => u.Email == model.Email || u.Mobile == model.Mobile);

            if (existingUser != null)
            {
                // ❌ User already exists
                ModelState.AddModelError("Email", "User with this Email or Mobile already exists.");
                //ModelState.AddModelError("Mobile", "User with this Mobile already exists.");
                return View(model);
            }


            int? sponsorId = null;

            if (!string.IsNullOrEmpty(model.SponsorUserCode))
            {
                var sponsor = _context.Users.FirstOrDefault(u => u.UserCode == model.SponsorUserCode);
                if (sponsor == null)
                {
                    ModelState.AddModelError("SponsorUserCode", "Invalid Sponsor Code.");
                    return View(model);
                }
                sponsorId = sponsor.Id;
            }

            // ✅ Generate unique UserCode
            int lastId = _context.Users.OrderByDescending(u => u.Id).FirstOrDefault()?.Id ?? 0;
            string newUserCode = "REG" + (1000 + lastId + 1);

            // ✅ Hash password (simple hash for demo)
            string passwordHash = ComputeSha256Hash(model.PasswordHash);

        
            // ✅ Create user entity
            var newUser = new User
            {
                UserCode = newUserCode,
                FullName = model.FullName,
                Email = model.Email,
                Mobile = model.Mobile,

                PasswordHash = passwordHash,
                SponsorUserCode = model.SponsorUserCode,
                SponsorId = sponsorId,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            TempData["Success"] = "Registration successful! Please log in.";
            return RedirectToAction("Index", "Login");
        }

        // Helper: hash password
        private string ComputeSha256Hash(string rawData)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
