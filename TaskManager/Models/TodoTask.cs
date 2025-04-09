using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class TodoTask
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public Priority Priority { get; set; }

        public Status Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public int AuthorId { get; set; }
        public User Author { get; set; } = null!;

        public int? AssigneeId { get; set; }
        public User? Assignee { get; set; }

        public List<SubTask> SubTasks { get; set; } = new();

        public List<Comment> Comments { get; set; } = new();
    }
} 