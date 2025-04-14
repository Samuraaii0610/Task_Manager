using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class UserAccount
    {
        public int Id { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        public required string PasswordSalt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        public bool IsActive { get; set; } = true;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
} 