using BookieDookie.Data;
using BookieDookie.Models;
using BookieDookie.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookieDookie.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public List<User> GetAllUsers()
        {
            return _context.Users
                .Where(u => !u.IsDeleted)
                .ToList();
        }

        public User? GetUserById(Guid id)
        {
            return _context.Users
                .FirstOrDefault(u =>
                    u.Id == id &&
                    !u.IsDeleted);
        }

        public User? GetUserByUsername(string username)
        {
            return _context.Users
                .FirstOrDefault(u =>
                    u.Username == username &&
                    !u.IsDeleted);
        }

        public User? GetUserByEmail(string email)
        {
            return _context.Users
                .FirstOrDefault(u =>
                    u.Email == email &&
                    !u.IsDeleted);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User updatedUser)
        {
            var user = _context.Users
                .FirstOrDefault(u =>
                    u.Id == updatedUser.Id &&
                    !u.IsDeleted);

            if (user == null)
                return;

            user.Email = updatedUser.Email;
            user.Username = updatedUser.Username;
            user.Role = updatedUser.Role;

            if (!string.IsNullOrWhiteSpace(updatedUser.PasswordHash))
            {
                user.PasswordHash = updatedUser.PasswordHash;
            }

            _context.SaveChanges();
        }

        public void SetPassword(User user, string password)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            if (_context.Entry(user).State == EntityState.Detached)
            {
                _context.Users.Add(user);
            }

            _context.SaveChanges();
        }


        public bool VerifyPassword(User user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);

            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }

        public void DeleteUser(Guid id)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return;

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.Status = UserStatus.Inactive;

            _context.SaveChanges();
        }

        public void ToggleStatus(Guid id)
        {
            var user = _context.Users
                .FirstOrDefault(u =>
                    u.Id == id &&
                    !u.IsDeleted);

            if (user == null)
                return;

            user.Status = user.Status == UserStatus.Active
                ? UserStatus.Inactive
                : UserStatus.Active;

            _context.SaveChanges();
        }

        public List<User> GetActiveUsers()
        {
            return _context.Users
                .Where(u =>
                    u.Status == UserStatus.Active &&
                    !u.IsDeleted)
                .ToList();
        }

        public List<User> GetInactiveUsers()
        {
            return _context.Users
                .Where(u =>
                    u.Status == UserStatus.Inactive &&
                    !u.IsDeleted)
                .ToList();
        }
    }
}