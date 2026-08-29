using System.Diagnostics;
using System.Security.Claims;
using BookieDookie.Data;
using Microsoft.AspNetCore.Mvc;
using BookieDookie.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookieDookie.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Get the currently logged-in user's ID
            var userIdClaim = User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userIdClaim) ||
                !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return RedirectToAction("Index", "Login");
            }

            // Make sure the logged-in user actually exists
            var user = _context.Users
                .FirstOrDefault(u =>
                    u.Id == userId &&
                    !u.IsDeleted);

            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // =========================
            // USER'S BOOKS
            // =========================

            var booksRead = _context.Books
                .Count(b =>
                    b.UserId == userId &&
                    !b.IsDeleted);

            // =========================
            // USER'S READING STATS
            // =========================

            var stats = _context.ReadingStats
                .FirstOrDefault(s =>
                    s.UserId == userId);

            int totalPages = 0;
            int streak = 0;

            if (stats != null)
            {
                totalPages = stats.TotalPagesRead;
                streak = stats.ReadingStreak;

                ViewBag.BookmarkBook = stats.BookmarkBook;
                ViewBag.BookmarkPage = stats.BookmarkPage;
            }

            ViewBag.TotalBooks = booksRead;
            ViewBag.TotalPages = totalPages;
            ViewBag.Streak = streak;

            // =========================
            // GREETING
            // =========================

            int hour = DateTime.Now.Hour;
            string greeting;

            if (hour >= 5 && hour <= 7)
                greeting = "Goodie morning! Ready to grow today?";
            else if (hour >= 8 && hour <= 11)
                greeting = "Hope your morning is unfolding gently.";
            else if (hour >= 12 && hour <= 15)
                greeting = "Ummhmm...a little afternoon-escape for a few pages, huh?";
            else if (hour >= 16 && hour <= 19)
                greeting = "A quiet evening for reflection.";
            else if (hour >= 20 && hour <= 22)
                greeting = "Want a gentle read before bed?";
            else
                greeting = "Still awake? No worries. BookieDookie's here to befriend you ^.^";

            ViewBag.GreetingLine = greeting;

            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}