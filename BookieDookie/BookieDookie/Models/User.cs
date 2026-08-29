using System.ComponentModel.DataAnnotations;

namespace BookieDookie.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public UserStatus Status { get; set; } = UserStatus.Active;

        public UserRole Role { get; set; } = UserRole.User;

        public string? TotpSecret { get; set; }

        public DateTime? TotpGeneratedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public List<Book> Books { get; set; } = new();
    }
}