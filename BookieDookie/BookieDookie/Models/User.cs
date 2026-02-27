using System.ComponentModel.DataAnnotations;

namespace BookieDookie.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Active;

        public List<Book> Books { get; set; }
    }
}