using BookieDookie.Data;
using BookieDookie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookieDookie.Controllers
{
    [Authorize]
    public class ReadingStatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReadingStatsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================
        // READING STATS PAGE
        // =========================

        public IActionResult Index()
        {
            var userIdString = User.FindFirst("UserId")?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            var stats = _context.ReadingStats
                .FirstOrDefault(s => s.UserId == userId);

            if (stats == null)
            {
                stats = new ReadingStats
                {
                    Id = Guid.NewGuid(),

                    UserId = userId,

                    PagesReadToday = 0,

                    TotalPagesRead = 0,

                    BooksRead = 0,

                    ReadingStreak = 0,

                    LastUpdated = DateTime.UtcNow,

                    LastReadingDate = null
                };

                _context.ReadingStats.Add(stats);

                _context.SaveChanges();
            }

            return View(stats);
        }


        // =========================
        // UPDATE PAGES
        // =========================

        [HttpPost]
        public IActionResult UpdatePages(int pages)
        {
            var userIdString = User.FindFirst("UserId")?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            var stats = _context.ReadingStats
                .FirstOrDefault(s => s.UserId == userId);

            if (stats == null)
                return BadRequest("Reading stats not found.");


            // =========================
            // DATE INFORMATION
            // =========================

            var today = DateTime.UtcNow.Date;


            // =========================
            // PREVENT NEGATIVE VALUES
            // =========================

            if (pages < 0)
                pages = 0;


            // =========================
            // FIRST READING ACTIVITY
            // =========================

            if (stats.LastReadingDate == null)
            {
                if (pages > 0)
                {
                    stats.ReadingStreak = 1;
                    stats.LastReadingDate = today;
                }
            }


            // =========================
            // READING ACTIVITY EXISTS
            // =========================

            else
            {
                var lastReadingDate =
                    stats.LastReadingDate.Value.Date;

                var daysSinceLastReading =
                    (today - lastReadingDate).Days;


                // ---------------------------------
                // SAME DAY
                // ---------------------------------

                if (daysSinceLastReading == 0)
                {
                    // Same-day updates do NOT increase streak.

                    int difference =
                        pages - stats.PagesReadToday;

                    stats.TotalPagesRead += difference;

                    stats.PagesReadToday = pages;
                }


                // ---------------------------------
                // EXACTLY ONE DAY LATER
                // ---------------------------------

                else if (daysSinceLastReading == 1)
                {
                    // A new reading day.
                    // Increase streak only if pages > 0.

                    if (pages > 0)
                    {
                        stats.ReadingStreak += 1;
                        stats.LastReadingDate = today;
                    }

                    stats.TotalPagesRead += pages;
                    stats.PagesReadToday = pages;
                }


                // ---------------------------------
                // MISSED ONE OR MORE DAYS
                // ---------------------------------

                else
                {
                    // User missed at least one complete day.
                    // Streak starts again from 1.

                    if (pages > 0)
                    {
                        stats.ReadingStreak = 1;
                        stats.LastReadingDate = today;
                    }
                    else
                    {
                        stats.ReadingStreak = 0;
                    }

                    stats.TotalPagesRead += pages;
                    stats.PagesReadToday = pages;
                }
            }


            // =========================
            // GENERAL UPDATE TIME
            // =========================

            stats.LastUpdated = DateTime.UtcNow;


            _context.SaveChanges();


            return RedirectToAction("Index");
        }


        // =========================
        // UPDATE BOOKMARK
        // =========================

        [HttpPost]
        public IActionResult UpdateBookmark(string book, int page)
        {
            var userIdString = User.FindFirst("UserId")?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            var stats = _context.ReadingStats
                .FirstOrDefault(s => s.UserId == userId);

            if (stats == null)
                return BadRequest("Reading stats not found.");

            stats.BookmarkBook = book;
            stats.BookmarkPage = page;

            stats.LastUpdated = DateTime.UtcNow;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        // =========================
        // UPDATE FEELING
        // =========================

        [HttpPost]
        public IActionResult UpdateFeeling(string feeling)
        {
            var userIdString = User.FindFirst("UserId")?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            var stats = _context.ReadingStats
                .FirstOrDefault(s => s.UserId == userId);

            if (stats == null)
                return BadRequest("Reading stats not found.");

            stats.Feeling = feeling;

            stats.LastUpdated = DateTime.UtcNow;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}