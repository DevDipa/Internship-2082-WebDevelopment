using Microsoft.AspNetCore.Mvc;
using BookieDookie.Models;
using BookieDookie.Services.Interface;
using BookieDookie.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OtpNet;
using System.Text.RegularExpressions;

namespace BookieDookie.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public LoginController(
            IUserService userService,
            ApplicationDbContext context)
        {
            _userService = userService;
            _context = context;
        }


      //Login Page

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Error = null;

            return View();
        }

        //Login Action
        [HttpPost]
        public async Task<IActionResult> Login(
            string username,
            string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Username and password are required.";

                return View("Index");
            }

            var user = _userService.GetUserByUsername(username);

            if (user == null ||
                !_userService.VerifyPassword(user, password))
            {
                ViewBag.Error = "Invalid username or password.";

                return View("Index");
            }

            if (user.Status != UserStatus.Active)
            {
                ViewBag.Error = "Your account is inactive.";

                return View("Index");
            }

//Authentication Cookie

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                new Claim(
                    "UserId",
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Role,
                    user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var claimsPrincipal =
                new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal);


            if (user.Role == UserRole.Admin)
            {
                return RedirectToAction(
                    "Index",
                    "Admin");
            }

            return RedirectToAction(
                "Index",
                "Home");
        }

//Signup form

        [HttpPost]
        public IActionResult SignUp(
            string email,
            string username,
            string password,
            string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ViewBag.Error = "All fields are required.";

                return View("Index");
            }


            //Password validation

            if (!IsValidPassword(password))
            {
                ViewBag.Error =
                    "Password must be at least 5 characters and include an uppercase letter, a number and a special character.";

                return View("Index");
            }


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


            if (_userService.GetUserByEmail(email) != null)
            {
                ViewBag.Error =
                    "An account with this email already exists!";

                return View("Index");
            }


            //New user creation

            var newUser = new User
            {
                Email = email,
                Username = username,

                // as Users, ofc
                Role = UserRole.User,

                Status = UserStatus.Active
            };


            // Hash the password through UserService
            _userService.SetPassword(
                newUser,
                password);


            return RedirectToAction("Index");
        }

//Logout

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction(
                "Index",
                "Login");
        }


        //Forgot Password

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            ViewBag.Error = null;

            return View();
        }


        [HttpPost]
        public IActionResult SendResetCode(string email)
        {
            var user = _userService.GetUserByEmail(email);

            if (user == null)
            {
                ViewBag.Error =
                    "No account was found with that email.";

                return View("ForgotPassword");
            }


            byte[] secretKey =
                KeyGeneration.GenerateRandomKey(20);

            var base32Secret =
                Base32Encoding.ToString(secretKey);

            user.TotpSecret = base32Secret;

            user.TotpGeneratedAt =
                DateTime.UtcNow;

            _userService.UpdateUser(user);


            var totp = new Totp(secretKey);

            var code = totp.ComputeTotp();


            TempData["ResetCode"] = code;
            TempData["ResetUserId"] = user.Id;


            return RedirectToAction(
                "VerifyResetCode");
        }


        //Verify reset code

        [HttpGet]
        public IActionResult VerifyResetCode()
        {
            TempData.Keep("ResetUserId");
            TempData.Keep("ResetCode");

            return View();
        }


        [HttpPost]
        public IActionResult VerifyResetCode(
            string code)
        {
            var userIdObj =
                TempData["ResetUserId"];

            if (userIdObj == null)
                return Content("Reset session expired.");


            var userId =
                Guid.Parse(userIdObj.ToString()!);


            var user =
                _context.Users.FirstOrDefault(u => u.Id == userId);


            if (user == null ||
                user.TotpSecret == null)
            {
                return Content(
                    "Reset request not found.");
            }


            var secretKey =
                Base32Encoding.ToBytes(
                    user.TotpSecret);

            var totp =
                new Totp(secretKey);


            bool valid =
                totp.VerifyTotp(
                    code,
                    out long timeWindowUsed);


            if (!valid)
            {
                return Content(
                    "Invalid or expired code.");
            }


            TempData["ResetUserId"] =
                user.Id;


            return RedirectToAction(
                "ResetPassword");
        }


        //Reset password

        [HttpGet]
        public IActionResult ResetPassword()
        {
            TempData.Keep("ResetUserId");

            return View();
        }


        [HttpPost]
        public IActionResult ResetPassword(
            string password,
            string confirmPassword)
        {
            if (!IsValidPassword(password))
            {
                return Content(
                    "Password must be at least 5 characters and include an uppercase letter, a number and a special character.");
            }


            if (password != confirmPassword)
            {
                return Content(
                    "Passwords do not match.");
            }


            var userIdObj =
                TempData["ResetUserId"];

            if (userIdObj == null)
            {
                return Content(
                    "Reset session expired.");
            }


            var userId =
                Guid.Parse(userIdObj.ToString()!);


            var user =
                _userService.GetUserById(userId);


            if (user == null)
            {
                return Content(
                    "User not found.");
            }


            _userService.SetPassword(
                user,
                password);


            user.TotpSecret = null;
            user.TotpGeneratedAt = null;

            _userService.UpdateUser(user);


            return RedirectToAction(
                "Index",
                "Login");
        }

//Password validation

        private bool IsValidPassword(string password)
        {
            return password.Length >= 5 &&
                   Regex.IsMatch(password, "[A-Z]") &&
                   Regex.IsMatch(password, "[0-9]") &&
                   Regex.IsMatch(password, @"[^a-zA-Z0-9]");
        }
    }

}