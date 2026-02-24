using Microsoft.AspNetCore.Mvc;
using BookieDookie.Models;

namespace BookieDookie.Controllers
{
    public class LoginController : Controller
    {
        //IN-MEMORY STORAGE
        public static List<User> _users = new List<User>();
        
        // GET: Login
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _users.FirstOrDefault(u => 
                u.Username == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid credentials!";
            return View("Index");
        }

        [HttpPost]
        public IActionResult SignUp(string email, string username, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match!";
                return View("Index");
            }

            if (_users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Username already exists!";
                return View("Index");
            }

            var newUser = new User
            {
                Email = email,
                Username = username,
                Password = password
            };

            _users.Add(newUser);

            return RedirectToAction("Index");
        }
    }
}