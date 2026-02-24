using Microsoft.AspNetCore.Mvc;

namespace BookieDookie.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // TODO: Add authentication logic
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult SignUp(string username, string password)
        {
            // TODO: Add registration logic
            return RedirectToAction("Index");
        }
    }
}