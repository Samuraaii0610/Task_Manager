using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TodoTask> Tasks { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des relations TodoTask
            modelBuilder.Entity<TodoTask>()
                .HasOne(t => t.Author)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TodoTask>()
                .HasOne(t => t.Assignee)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration des relations SubTask
            modelBuilder.Entity<SubTask>()
                .HasOne(st => st.TodoTask)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(st => st.TodoTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration des relations Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.TodoTask)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TodoTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration des relations Tag
            modelBuilder.Entity<Tag>()
                .HasOne(t => t.CreatedBy)
                .WithMany()
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration de la relation many-to-many entre TodoTask et Tag
            modelBuilder.Entity<TodoTask>()
                .HasMany(t => t.Tags)
                .WithMany(t => t.Tasks)
                .UsingEntity(j => j.ToTable("TaskTags"));
        }
    }
} 