using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<User> Users => Set<User>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(
            "server=localhost;port=3306;database=TaskMaster;user=root;password=;",
            ServerVersion.Create(8, 0, 36, ServerType.MySql) // Spécifiez votre version de MySQL
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuration des relations
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Auteur)
            .WithMany(u => u.TasksCrees)
            .HasForeignKey(t => t.AuteurId);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Realisateur)
            .WithMany(u => u.TasksAssignees)
            .HasForeignKey(t => t.RealisateurId)
            .IsRequired(false);
    }
} 