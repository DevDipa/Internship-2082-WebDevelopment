using Microsoft.AspNetCore.Mvc;
using BookieDookie.Models;

namespace BookieDookie.Controllers
{

    public class BooksController : Controller
    {
        // Simulated database (in-memory)
        private static List<Book> books = new List<Book>();

        public IActionResult Index()
        {
            return View(books); // pass the list of books to view
        }

        [HttpGet]
        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBook(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newBook = new Book
                {
                    Id = books.Count + 1,
                    Title = model.Title,
                    Author = model.Author,
                    Genre = model.Genre,
                    Summary = model.Summary
                };

                // handle image upload
                if (model.Image != null && model.Image.Length > 0)
                {
                    var fileName = Path.GetFileName(model.Image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.Image.CopyTo(stream);
                    }

                    newBook.ImageUrl = "/images/" + fileName;
                }

                books.Add(newBook);
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }

}