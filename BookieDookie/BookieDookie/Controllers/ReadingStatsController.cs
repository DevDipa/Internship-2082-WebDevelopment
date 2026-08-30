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

//Reading stats

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

//Update pages

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


         //Date

            var today = DateTime.UtcNow.Date;


           //No negative values

            if (pages < 0)
                pages = 0;
            
            if (stats.LastReadingDate == null)
            {
                if (pages > 0)
                {
                    stats.ReadingStreak = 1;
                    stats.LastReadingDate = today;
                }
            }


            else
            {
                var lastReadingDate =
                    stats.LastReadingDate.Value.Date;

                var daysSinceLastReading =
                    (today - lastReadingDate).Days;

//same day logic

                if (daysSinceLastReading == 0)
                {

                    int difference =
                        pages - stats.PagesReadToday;

                    stats.TotalPagesRead += difference;

                    stats.PagesReadToday = pages;
                }

                else if (daysSinceLastReading == 1)
                {
                    // A new reading day.

                    if (pages > 0)
                    {
                        stats.ReadingStreak += 1;
                        stats.LastReadingDate = today;
                    }

                    stats.TotalPagesRead += pages;
                    stats.PagesReadToday = pages;
                }

//streak break

                else
                {

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
            

            stats.LastUpdated = DateTime.UtcNow;


            _context.SaveChanges();


            return RedirectToAction("Index");
        }

//Bookmark

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


        //Feeling update

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