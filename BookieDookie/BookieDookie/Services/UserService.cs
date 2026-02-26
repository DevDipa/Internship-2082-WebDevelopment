using BookieDookie.Models;
using BookieDookie.Services.Interface;

namespace BookieDookie.Services
{
    public class UserService : IUserService
    {
        private static List<User> _users = new List<User>();

        public List<User> GetAllUsers()
        {
            return _users;
        }

        public User GetUserById(Guid id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public User GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public void UpdateUser(User updatedUser)
        {
            var user = GetUserById(updatedUser.Id);

            if (user != null)
            {
                user.Email = updatedUser.Email;
                user.Username = updatedUser.Username;
                user.Password = updatedUser.Password;
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
            }
        }

        public List<User> GetActiveUsers()
        {
            return _users.Where(u => u.Status == UserStatus.Active).ToList();
        }

        public List<User> GetInactiveUsers()
        {
            return _users.Where(u => u.Status == UserStatus.Inactive).ToList();
        }
    }
}