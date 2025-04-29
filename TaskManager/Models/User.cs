using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public List<TodoTask> CreatedTasks { get; set; } = new();
        public List<TodoTask> AssignedTasks { get; set; } = new();
        
        public override string ToString()
        {
            return FullName;
        }
    }
} 