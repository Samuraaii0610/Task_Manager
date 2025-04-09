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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des relations
            modelBuilder.Entity<TodoTask>()
                .HasOne(t => t.Author)
                .WithMany()
                .HasForeignKey("AuthorId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TodoTask>()
                .HasOne(t => t.Assignee)
                .WithMany()
                .HasForeignKey("AssigneeId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubTask>()
                .HasOne<TodoTask>()
                .WithMany(t => t.SubTasks)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey("AuthorId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne<TodoTask>()
                .WithMany(t => t.Comments)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 