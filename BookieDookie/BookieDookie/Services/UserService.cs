using BookieDookie.Models;
using BookieDookie.Services.Interface;
using BookieDookie.Data;
using Microsoft.EntityFrameworkCore;

namespace BookieDookie.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User GetUserById(Guid id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public User GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User updatedUser)
        {
            _context.Users.Update(updatedUser);
            _context.SaveChanges();
        }
        
        public void DeleteUser(Guid id)
        {
            var user = _context.Users
                .Include(u => u.Books)
                .FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                _context.Books.RemoveRange(user.Books); // delete related books
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public void ToggleStatus(Guid id)
        {
            var user = GetUserById(id);
            if (user != null)
            {
                user.Status = user.Status == UserStatus.Active
                    ? UserStatus.Inactive
                    : UserStatus.Active;

                _context.SaveChanges();
            }
        }

        public List<User> GetActiveUsers()
        {
            return _context.Users
                .Where(u => u.Status == UserStatus.Active)
                .ToList();
        }

        public List<User> GetInactiveUsers()
        {
            return _context.Users
                .Where(u => u.Status == UserStatus.Inactive)
                .ToList();
        }
    }
}