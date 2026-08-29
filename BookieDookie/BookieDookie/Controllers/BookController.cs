using System.Security.Claims;
using BookieDookie.Models;
using BookieDookie.Data;
using BookieDookie.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BookieDookie.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ApplicationDbContext _context;

        public BookController(
            IBookService bookService,
            ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }


        // ==========================================
        // CURRENT LOGGED-IN USER
        // ==========================================

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
                return userId;

            return null;
        }


        // ==========================================
        // BOOKSHELF
        // ==========================================

        public IActionResult Index()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var books = _bookService.GetBooksByUser(userId.Value);

            return View(books);
        }


        // ==========================================
        // CREATE BOOK
        // ==========================================

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(
            Book book,
            IFormFile ImageFile)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            if (ImageFile != null)
            {
                var fileName =
                    Guid.NewGuid() +
                    Path.GetExtension(ImageFile.FileName);

                var uploadFolder =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads");

                Directory.CreateDirectory(uploadFolder);

                var path =
                    Path.Combine(
                        uploadFolder,
                        fileName);

                using (var stream =
                       new FileStream(
                           path,
                           FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                book.ImageUrl =
                    "/uploads/" + fileName;
            }

            _bookService.AddBook(
                book,
                userId.Value);

            return RedirectToAction("Index");
        }


        // ==========================================
        // EDIT BOOK
        // ==========================================

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var book =
                _bookService.GetBookById(id);

            if (book == null)
                return NotFound();

            // Make sure this book belongs
            // to the currently logged-in user.
            if (book.UserId != userId.Value)
                return Forbid();

            return View(book);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(
            Book book,
            IFormFile ImageFile)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            // Retrieve the real book from the database.
            var existingBook =
                _bookService.GetBookById(book.Id);

            if (existingBook == null)
                return NotFound();

            // Prevent one user from editing
            // another user's book.
            if (existingBook.UserId != userId.Value)
                return Forbid();


            // Update book information.
            existingBook.Title =
                book.Title;

            existingBook.Author =
                book.Author;

            existingBook.Genre =
                book.Genre;

            existingBook.Description =
                book.Description;


            // Replace cover only if a new image
            // has actually been uploaded.
            if (ImageFile != null)
            {
                var fileName =
                    Guid.NewGuid() +
                    Path.GetExtension(ImageFile.FileName);

                var uploadFolder =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads");

                Directory.CreateDirectory(uploadFolder);

                var path =
                    Path.Combine(
                        uploadFolder,
                        fileName);

                using (var stream =
                       new FileStream(
                           path,
                           FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                existingBook.ImageUrl =
                    "/uploads/" + fileName;
            }


            _bookService.UpdateBook(
                existingBook);

            return RedirectToAction("Index");
        }


        // ==========================================
        // DELETE BOOK
        // ==========================================

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var book =
                _bookService.GetBookById(id);

            if (book == null)
                return NotFound();

            // Normal users can only delete
            // their own books.
            if (book.UserId != userId.Value)
            {
                var role =
                    User.FindFirst(
                        ClaimTypes.Role)?.Value;

                if (role != "Admin")
                    return Forbid();
            }

            _bookService.DeleteBook(id);

            return RedirectToAction("Index");
        }
    }
}