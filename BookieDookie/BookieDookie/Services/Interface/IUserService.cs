using BookieDookie.Models;

namespace BookieDookie.Services.Interface
{
    public interface IUserService
    {
        List<User> GetAllUsers();

        User? GetUserById(Guid id);

        User? GetUserByUsername(string username);

        User? GetUserByEmail(string email);

        void AddUser(User user);

        void UpdateUser(User updatedUser);

        void DeleteUser(Guid id);

        void ToggleStatus(Guid id);

        List<User> GetActiveUsers();

        List<User> GetInactiveUsers();

        void SetPassword(User user, string password);

        bool VerifyPassword(User user, string password);
    }
}