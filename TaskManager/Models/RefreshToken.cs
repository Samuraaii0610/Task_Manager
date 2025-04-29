using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        
        [Required]
        public required string Token { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime ExpiresAt { get; set; }
        
        public int UserAccountId { get; set; }
        public UserAccount UserAccount { get; set; } = null!;
    }
} 