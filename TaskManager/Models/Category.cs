namespace TaskManager.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        
        public virtual ICollection<TodoTask> Tasks { get; set; } = new List<TodoTask>();
    }
} 