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
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

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

            modelBuilder.Entity<TodoTask>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

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
                
            // Configuration des relations UserAccount
            modelBuilder.Entity<UserAccount>()
                .HasOne(ua => ua.User)
                .WithOne()
                .HasForeignKey<UserAccount>(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<UserAccount>()
                .HasIndex(ua => ua.Username)
                .IsUnique();
                
            modelBuilder.Entity<UserAccount>()
                .HasIndex(ua => ua.Email)
                .IsUnique();
                
            // Configuration des relations RefreshToken
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.UserAccount)
                .WithMany(ua => ua.RefreshTokens)
                .HasForeignKey(rt => rt.UserAccountId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();
        }
    }
} 