using TaskManager.Models;

namespace TaskManager.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // S'assurer que la base de données est créée
            context.Database.EnsureCreated();

            // Vérifier s'il y a déjà des utilisateurs
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
                    Tags = new List<Tag> { tags[2] }
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
                    IsCompleted = false,
                    TodoTaskId = tasks[0].Id
                },
                new SubTask
                {
                    Title = "Préparer les démos",
                    Description = "Préparer les démonstrations pour la présentation",
                    IsCompleted = false,
                    TodoTaskId = tasks[0].Id
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
                }
            };

            context.Comments.AddRange(comments);
            context.SaveChanges();
        }
    }
} 