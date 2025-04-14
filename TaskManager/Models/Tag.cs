using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        public string Color { get; set; } = "#000000";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public List<TodoTask> Tasks { get; set; } = new();
    }
} 