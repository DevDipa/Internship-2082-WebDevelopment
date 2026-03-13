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

        public UserRole Role { get; set; }
        
        public string? TotpSecret { get; set; }

        public DateTime? TotpGeneratedAt { get; set; }

        public List<Book> Books { get; set; }
    }
}