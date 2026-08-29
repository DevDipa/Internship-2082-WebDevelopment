using BookieDookie.Models;
using BookieDookie.Data;
using BookieDookie.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookieDookie.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // GET ALL BOOKS
        // ==========================================

        public List<Book> GetAllBooks()
        {
            return _context.Books
                .Where(b => !b.IsDeleted)
                .ToList();
        }


        // ==========================================
        // GET BOOK BY ID
        // ==========================================

        public Book? GetBookById(Guid id)
        {
            return _context.Books
                .FirstOrDefault(b =>
                    b.Id == id &&
                    !b.IsDeleted);
        }


        // ==========================================
        // GET BOOKS BELONGING TO ONE USER
        // ==========================================

        public List<Book> GetBooksByUser(Guid userId)
        {
            return _context.Books
                .Where(b =>
                    b.UserId == userId &&
                    !b.IsDeleted)
                .OrderBy(b => b.Title)
                .ToList();
        }


        // ==========================================
        // ADD BOOK
        // ==========================================

        public void AddBook(
            Book book,
            Guid userId)
        {
            book.UserId = userId;

            _context.Books.Add(book);

            _context.SaveChanges();
        }


        // ==========================================
        // UPDATE BOOK
        // ==========================================

        public void UpdateBook(Book book)
        {
            _context.Books.Update(book);

            _context.SaveChanges();
        }


        // ==========================================
        // DELETE BOOK
        // ==========================================

        public void DeleteBook(Guid id)
        {
            var book =
                _context.Books.FirstOrDefault(
                    b => b.Id == id);

            if (book == null)
                return;

            _context.Books.Remove(book);

            _context.SaveChanges();
        }
    }
}