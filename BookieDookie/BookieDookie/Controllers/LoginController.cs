using Microsoft.AspNetCore.Mvc;
using BookieDookie.Models;
using BookieDookie.Services.Interface;
using BookieDookie.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OtpNet;
using System.Security.Cryptography;

namespace BookieDookie.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;
        
        public LoginController(IUserService userService, ApplicationDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Username and password required.";
                return View("Index");
            }

            var user = _userService.GetUserByUsername(username);

            // If user doesn't exist → redirect to signup
            if (user == null)
            {
                return RedirectToAction("Index"); // or show signup section
            }

            // If password wrong
            if (user.Password != password)
            {
                ViewBag.Error = "Invalid password.";
                return View("Index");
            }

            // AUTHENTICATION COOKIE
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal
            );

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult SignUp(string email, string username, UserRole role, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match!";
                return View("Index");
            }

            if (_userService.GetUserByUsername(username) != null)
            {
                ViewBag.Error = "Username already exists!";
                return View("Index");
            }

            var newUser = new User
            {
                Email = email,
                Username = username,
                Role = role,
                Password = password
            };

            _userService.AddUser(newUser);

            return RedirectToAction("Index");
        }

        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Login");
        }
        
        public IActionResult ForgotPassword()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult SendResetCode(string email)
        {
            var user = _userService.GetUserByEmail(email);

            if (user == null)
                return Content("User not found");

            byte[] secretKey = KeyGeneration.GenerateRandomKey(20);

            var base32Secret = Base32Encoding.ToString(secretKey);

            user.TotpSecret = base32Secret;
            user.TotpGeneratedAt = DateTime.UtcNow;

            _userService.UpdateUser(user);

            var totp = new Totp(secretKey);

            var code = totp.ComputeTotp();

            TempData["ResetCode"] = code;
            TempData["ResetUserId"] = user.Id;   // ⭐ IMPORTANT

            return RedirectToAction("VerifyResetCode");
        }
        
        public IActionResult VerifyResetCode()
        {
            TempData.Keep("ResetUserId");
            TempData.Keep("ResetCode");

            return View();
        }
        
        [HttpPost]
        public IActionResult VerifyResetCode(string code)
        {
            var userIdObj = TempData["ResetUserId"];

            if (userIdObj == null)
                return Content("Reset session expired.");

            var userId = Guid.Parse(userIdObj.ToString());

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null || user.TotpSecret == null)
                return Content("Reset request not found.");

            var secretKey = Base32Encoding.ToBytes(user.TotpSecret);

            var totp = new Totp(secretKey);

            bool valid = totp.VerifyTotp(code, out long timeWindowUsed);

            if (!valid)
                return Content("Invalid or expired code");

            TempData["ResetUserId"] = user.Id;

            return RedirectToAction("ResetPassword");
        }
        
        public IActionResult ResetPassword()
        {
            TempData.Keep("ResetUserId");
            return View();
        }
        
        [HttpPost]
        public IActionResult ResetPassword(string password, string confirmPassword)
        {
            if (password != confirmPassword)
                return Content("Passwords do not match");

            var userIdObj = TempData["ResetUserId"];

            if (userIdObj == null)
                return Content("Reset session expired.");

            var userId = Guid.Parse(userIdObj.ToString());

            var user = _userService.GetUserById(userId);

            if (user == null)
                return Content("User not found.");

            user.Password = password;

            user.TotpSecret = null;
            user.TotpGeneratedAt = null;

            _userService.UpdateUser(user);

            return RedirectToAction("Index", "Login");
        }
    }
}