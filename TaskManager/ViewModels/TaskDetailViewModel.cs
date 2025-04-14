using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskManager.Models;
using System.Threading.Tasks;
using TaskManager.Data;
using Microsoft.EntityFrameworkCore;
using TaskManager.Services;
using System.Collections.ObjectModel;

namespace TaskManager.ViewModels
{
    public partial class TaskDetailViewModel : BaseViewModel
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly AuthService _authService;
        private TodoTask? _task;
        private bool _isNewTask;
        private int? _currentUserId;

        [ObservableProperty]
        private string taskTitle = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private DateTime dueDate = DateTime.Now.AddDays(1);

        [ObservableProperty]
        private Priority priority = Priority.Medium;

        [ObservableProperty]
        private Status status = Status.ToDo;

        [ObservableProperty]
        private string assigneeEmail = string.Empty;

        [ObservableProperty]
        private ObservableCollection<User> availableUsers = new ObservableCollection<User>();

        public TaskDetailViewModel(IDbContextFactory<ApplicationDbContext> contextFactory, AuthService authService)
        {
            _contextFactory = contextFactory;
            _authService = authService;
            _isNewTask = true;
            Title = "Ajouter une Tâche";
            
            // Définir l'utilisateur connecté comme auteur et charger les données
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                // Vérifier l'utilisateur connecté
                var authStatus = await _authService.CheckAuthStatus();
                if (authStatus?.Success == true && authStatus.User != null)
                {
                    _currentUserId = authStatus.User.Id;
                    Console.WriteLine($"Utilisateur connecté avec ID: {_currentUserId}");
                }
                else
                {
                    Console.WriteLine("Aucun utilisateur connecté");
                    await Shell.Current.GoToAsync("//login");
                    return;
                }
                
                // Charger les utilisateurs disponibles pour l'assignation
                await LoadAvailableUsersAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'initialisation : {ex.Message}");
            }
        }
        
        private async Task LoadAvailableUsersAsync()
        {
            try
            {
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    var users = await context.Users.ToListAsync();
                    
                    AvailableUsers.Clear();
                    foreach (var user in users)
                    {
                        AvailableUsers.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erreur", $"Erreur lors du chargement des utilisateurs : {ex.Message}", "OK");
            }
        }

        public TaskDetailViewModel(TodoTask task, IDbContextFactory<ApplicationDbContext> contextFactory, AuthService authService)
        {
            _contextFactory = contextFactory;
            _authService = authService;
            _task = task;
            _isNewTask = false;
            Title = "Modifier la Tâche";

            // Copier les valeurs de la tâche dans les propriétés
            taskTitle = task.Title;
            description = task.Description;
            dueDate = task.DueDate;
            priority = task.Priority;
            status = task.Status;
            
            // Charger l'email de l'assigné s'il existe
            if (task.Assignee != null)
            {
                assigneeEmail = task.Assignee.Email;
            }

            // Notifier les changements
            OnPropertyChanged(nameof(TaskTitle));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(DueDate));
            OnPropertyChanged(nameof(Priority));
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(AssigneeEmail));
            
            // Charger les utilisateurs disponibles et vérifier l'utilisateur connecté
            InitializeAsync();
        }

        [RelayCommand]
        private async Task SaveTaskAsync()
        {
            if (string.IsNullOrWhiteSpace(TaskTitle))
            {
                await Shell.Current.DisplayAlert("Erreur", "Le titre est obligatoire", "OK");
                return;
            }

            // Vérifier que l'utilisateur est connecté
            if (_currentUserId == null)
            {
                await Shell.Current.DisplayAlert("Erreur", "Vous devez être connecté pour créer ou modifier une tâche", "OK");
                await Shell.Current.GoToAsync("//login");
                return;
            }

            try
            {
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    // Rechercher l'assigné par email
                    int? assigneeId = null;
                    if (!string.IsNullOrWhiteSpace(AssigneeEmail))
                    {
                        var assignee = await context.Users
                            .AsNoTracking() // Pour éviter les problèmes de tracking
                            .FirstOrDefaultAsync(u => u.Email.ToLower() == AssigneeEmail.ToLower());
                            
                        if (assignee == null)
                        {
                            await Shell.Current.DisplayAlert("Erreur", $"Aucun utilisateur trouvé avec l'email {AssigneeEmail}", "OK");
                            return;
                        }
                        
                        assigneeId = assignee.Id;
                    }

                    if (_isNewTask)
                    {
                        // Création d'une nouvelle tâche
                        var newTask = new TodoTask
                        {
                            Title = TaskTitle,
                            Description = Description,
                            DueDate = DueDate,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            Priority = Priority,
                            Status = Status,
                            AuthorId = _currentUserId.Value, // Utiliser l'ID de l'utilisateur connecté
                            AssigneeId = assigneeId
                        };

                        context.Tasks.Add(newTask);
                        await context.SaveChangesAsync();
                        Console.WriteLine($"Tâche créée avec l'auteur ID: {_currentUserId.Value}");
                    }
                    else
                    {
                        // Mise à jour de la tâche existante
                        if (_task == null)
                        {
                            await Shell.Current.DisplayAlert("Erreur", "La tâche n'a pas été correctement chargée", "OK");
                            return;
                        }

                        _task.Title = TaskTitle;
                        _task.Description = Description;
                        _task.DueDate = DueDate;
                        _task.Priority = Priority;
                        _task.Status = Status;
                        _task.AssigneeId = assigneeId;
                        _task.UpdatedAt = DateTime.Now;

                        context.Tasks.Update(_task);
                        await context.SaveChangesAsync();
                    }
                }
                
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erreur", $"Erreur lors de la sauvegarde : {ex.Message}", "OK");
                Console.WriteLine($"Exception lors de la sauvegarde : {ex}");
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
} 