using BookieDookie.Models;

namespace BookieDookie.Services.Interface
{
    public interface IUserService
    {
        List<User> GetAllUsers();
        User GetUserById(Guid id);
        User GetUserByUsername(string username);
        void AddUser(User user);
        void DeleteUser(Guid id);
        void UpdateUser(User updatedUser);
        void ToggleStatus(Guid id);
        List<User> GetActiveUsers();
        List<User> GetInactiveUsers();
    }
}



