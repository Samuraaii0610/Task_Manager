using Microsoft.EntityFrameworkCore;

namespace TaskManager.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<TestModel> TestModels { get; set; }

    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Configuration pour XAMPP (mot de passe vide par défaut)
            var connectionString = "Server=localhost;Database=taskmanager;User=root;Password=;";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Les configurations des modèles seront ajoutées ici
    }
} 