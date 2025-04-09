using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class SubTask
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? CompletedAt { get; set; }

        public int TodoTaskId { get; set; }
        public TodoTask TodoTask { get; set; } = null!;
    }
} 