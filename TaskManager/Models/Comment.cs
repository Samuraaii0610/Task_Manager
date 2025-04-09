using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public int AuthorId { get; set; }
        public User Author { get; set; } = null!;

        public int TodoTaskId { get; set; }
        public TodoTask TodoTask { get; set; } = null!;
    }
} 