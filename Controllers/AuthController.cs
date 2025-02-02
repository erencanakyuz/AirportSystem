using Microsoft.AspNetCore.Mvc;
using AirportDemo.Data;
using AirportDemo.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace AirportDemo.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string RECAPTCHA_SECRET = "6LcQzsoqAAAAAAsEOms0BJpn3W75dHg1ZS2sl4Dv"; // Google reCAPTCHA Secret Key

        public AuthController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Auth/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        public async Task<IActionResult> Register(User user, [FromForm(Name = "g-recaptcha-response")] string gRecaptchaResponse)
        {
            if (!await VerifyRecaptcha(gRecaptchaResponse))
            {
                ModelState.AddModelError("", "reCAPTCHA doğrulaması başarısız oldu. Lütfen tekrar deneyin.");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("", "Bu e-posta zaten kayıtlı.");
                    return View(user);
                }

                // Şifreyi hashleyerek kaydediyoruz
                user.PasswordHash = HashPassword(user.PasswordHash);
                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(user);
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Home"); // Giriş yapmışsa anasayfaya yönlendir
            }
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, [FromForm(Name = "g-recaptcha-response")] string gRecaptchaResponse)
        {
            // Debug amaçlı reCAPTCHA yanıtını loglayın
            Console.WriteLine($"reCAPTCHA Response: {gRecaptchaResponse}");

            if (string.IsNullOrEmpty(gRecaptchaResponse))
            {
                ModelState.AddModelError("", "reCAPTCHA doğrulaması başarısız oldu. Lütfen tekrar deneyin.");
                return View();
            }

            if (!await VerifyRecaptcha(gRecaptchaResponse))
            {
                ModelState.AddModelError("", "reCAPTCHA doğrulaması başarısız oldu. Lütfen tekrar deneyin.");
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || user.PasswordHash != HashPassword(password))
            {
                ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
                return View();
            }

            // Session’a kullanıcı bilgilerini ekleyelim
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Google reCAPTCHA servisine istek atarak doğrulamayı gerçekleştirir.
        /// </summary>
        private async Task<bool> VerifyRecaptcha(string recaptchaResponse)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={RECAPTCHA_SECRET}&response={recaptchaResponse}",
                null);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(jsonResponse))
            {
                return false;
            }

            try
            {
                var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                if (result == null || result["success"] == null)
                {
                    return false;
                }
                return result["success"].ToObject<bool>();
            }
            catch (JsonException)
            {
                return false;
            }
        }

        /// <summary>
        /// Girilen şifreyi SHA256 algoritması ile hashler.
        /// </summary>
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}
