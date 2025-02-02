using Microsoft.AspNetCore.Mvc;
using AirportDemo.Data;
using AirportDemo.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AirportDemo.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("", "This email is already registered.");
                    return View();
                }

                user.PasswordHash = HashPassword(user.PasswordHash);
                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || user.PasswordHash != HashPassword(password))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            return RedirectToAction("Index", "Flights");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
