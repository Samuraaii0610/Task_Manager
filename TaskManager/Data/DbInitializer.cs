using TaskManager.Models;

namespace TaskManager.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // S'assurer que la base de données existe et est à jour avec le modèle
            // mais ne pas la supprimer et la recréer
            context.Database.EnsureCreated();

            // Si des utilisateurs existent déjà, ne pas ajouter de données de test
            if (context.Users.Any())
            {
                return; // La base de données a déjà été initialisée
            }

            // Créer des utilisateurs de test
            var users = new User[]
            {
                new User
                {
                    FirstName = "Jean",
                    LastName = "Dupont",
                    Email = "jean.dupont@example.com"
                },
                new User
                {
                    FirstName = "Marie",
                    LastName = "Martin",
                    Email = "marie.martin@example.com"
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            // Créer des comptes utilisateurs
            var salt1 = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(16));
            var salt2 = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(16));
            
            // Mot de passe haché pour "password123"
            var passwordHash1 = HashPassword("password123", salt1);
            var passwordHash2 = HashPassword("password123", salt2);
            
            var userAccounts = new UserAccount[]
            {
                new UserAccount
                {
                    Username = "jean",
                    Email = "jean.dupont@example.com",
                    PasswordHash = passwordHash1,
                    PasswordSalt = salt1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UserId = users[0].Id
                },
                new UserAccount
                {
                    Username = "marie",
                    Email = "marie.martin@example.com",
                    PasswordHash = passwordHash2,
                    PasswordSalt = salt2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UserId = users[1].Id
                }
            };
            
            context.UserAccounts.AddRange(userAccounts);
            context.SaveChanges();

            // Créer des catégories
            var categories = new Category[]
            {
                new Category
                {
                    Name = "Travail",
                    Description = "Tâches professionnelles",
                    Color = "#3498db"
                },
                new Category
                {
                    Name = "Personnel",
                    Description = "Tâches personnelles",
                    Color = "#2ecc71"
                },
                new Category
                {
                    Name = "Projet",
                    Description = "Tâches liées aux projets",
                    Color = "#9b59b6"
                }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();

            // Créer des tags
            var tags = new Tag[]
            {
                new Tag
                {
                    Name = "Urgent",
                    Description = "Tâches urgentes",
                    Color = "#FF0000",
                    CreatedById = users[0].Id
                },
                new Tag
                {
                    Name = "En attente",
                    Description = "Tâches en attente d'une action externe",
                    Color = "#FFA500",
                    CreatedById = users[0].Id
                },
                new Tag
                {
                    Name = "Documentation",
                    Description = "Tâches liées à la documentation",
                    Color = "#0000FF",
                    CreatedById = users[1].Id
                }
            };

            context.Tags.AddRange(tags);
            context.SaveChanges();

            // Créer des tâches
            var tasks = new TodoTask[]
            {
                new TodoTask
                {
                    Title = "Préparer la présentation",
                    Description = "Préparer la présentation pour la réunion de lundi",
                    DueDate = DateTime.Now.AddDays(5),
                    Priority = Priority.High,
                    Status = Status.InProgress,
                    AuthorId = users[0].Id,
                    AssigneeId = users[1].Id,
                    CategoryId = categories[0].Id,
                    Tags = new List<Tag> { tags[0], tags[2] }
                },
                new TodoTask
                {
                    Title = "Mettre à jour la documentation",
                    Description = "Mettre à jour la documentation technique du projet",
                    DueDate = DateTime.Now.AddDays(10),
                    Priority = Priority.Medium,
                    Status = Status.ToDo,
                    AuthorId = users[1].Id,
                    CategoryId = categories[2].Id,
                    Tags = new List<Tag> { tags[2] }
                },
                new TodoTask
                {
                    Title = "Faire les courses",
                    Description = "Acheter les provisions pour la semaine",
                    DueDate = DateTime.Now.AddDays(2),
                    Priority = Priority.Low,
                    Status = Status.ToDo,
                    AuthorId = users[0].Id,
                    AssigneeId = users[0].Id,
                    CategoryId = categories[1].Id,
                    Tags = new List<Tag>()
                }
            };

            context.Tasks.AddRange(tasks);
            context.SaveChanges();

            // Créer des sous-tâches
            var subtasks = new SubTask[]
            {
                new SubTask
                {
                    Title = "Créer les diapositives",
                    Description = "Créer les diapositives PowerPoint",
                    Status = Status.InProgress,
                    DueDate = DateTime.Now.AddDays(3),
                    TodoTaskId = tasks[0].Id
                },
                new SubTask
                {
                    Title = "Préparer les démos",
                    Description = "Préparer les démonstrations pour la présentation",
                    Status = Status.ToDo,
                    DueDate = DateTime.Now.AddDays(4),
                    TodoTaskId = tasks[0].Id
                },
                new SubTask
                {
                    Title = "Acheter du pain",
                    Description = "Aller à la boulangerie",
                    Status = Status.ToDo,
                    TodoTaskId = tasks[2].Id
                }
            };

            context.SubTasks.AddRange(subtasks);
            context.SaveChanges();

            // Créer des commentaires
            var comments = new Comment[]
            {
                new Comment
                {
                    Content = "N'oublie pas d'inclure les graphiques de performance",
                    AuthorId = users[1].Id,
                    TodoTaskId = tasks[0].Id
                },
                new Comment
                {
                    Content = "J'ai commencé à travailler sur la première partie",
                    AuthorId = users[0].Id,
                    TodoTaskId = tasks[0].Id
                },
                new Comment
                {
                    Content = "N'oublie pas les produits bio",
                    AuthorId = users[1].Id,
                    TodoTaskId = tasks[2].Id
                }
            };

            context.Comments.AddRange(comments);
            context.SaveChanges();
        }
        
        private static string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using (var rfc2898 = new System.Security.Cryptography.Rfc2898DeriveBytes(password, saltBytes, 10000))
            {
                var hashBytes = rfc2898.GetBytes(32);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
} 