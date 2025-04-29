using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TaskManager.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using TaskManager.Views;
using System.Collections.Generic;
using TaskManager.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TaskManager.Services;

namespace TaskManager.ViewModels
{
    public partial class TasksViewModel : BaseViewModel
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly AuthService _authService;
        private List<TodoTask> _allTasks = new List<TodoTask>();
        private List<Category> _categories = new List<Category>();

        [ObservableProperty]
        private ObservableCollection<TodoTask> tasks;

        [ObservableProperty]
        private TodoTask? selectedTask;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private Status? filterStatus;

        [ObservableProperty]
        private Priority? filterPriority;

        [ObservableProperty]
        private Category? filterCategory;

        [ObservableProperty]
        private bool showCompletedTasks = true;

        [ObservableProperty]
        private string sortBy = "DueDate";

        public ICommand AddTaskCommand { get; }

        public List<Status> StatusList => Enum.GetValues<Status>().ToList();
        public List<Priority> PriorityList => Enum.GetValues<Priority>().ToList();
        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();
        public List<string> SortOptions => new List<string> { "DueDate", "Titre", "Priorité", "Statut", "Date de création" };

        public TasksViewModel(IDbContextFactory<ApplicationDbContext> contextFactory, AuthService authService)
        {
            _contextFactory = contextFactory;
            _authService = authService;
            Title = "Mes Tâches";
            Tasks = new ObservableCollection<TodoTask>();
            // Ne pas charger les tâches ici, cela sera fait dans OnAppearing()
            
            AddTaskCommand = new Command(async () => await OnAddTask());
        }

        public async Task CheckAuthenticationAsync()
        {
            var authStatus = await _authService.CheckAuthStatus();
            if (authStatus == null || !authStatus.Success)
            {
                await Shell.Current.GoToAsync("//login");
            }
        }

        public async Task LoadTasksAsync()
        {
            try
            {
                IsBusy = true;
                
                // Vérifier l'utilisateur connecté
                var authStatus = await _authService.CheckAuthStatus();
                if (authStatus == null || !authStatus.Success || authStatus.User == null)
                {
                    await Shell.Current.GoToAsync("//login");
                    return;
                }
                
                int currentUserId = authStatus.User.Id;
                
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    // Charger les catégories
                    var categoriesFromDb = await context.Categories.ToListAsync();
                    _categories = categoriesFromDb;
                    
                    Categories.Clear();
                    foreach (var category in _categories)
                    {
                        Categories.Add(category);
                    }

                    // Charger les tâches de l'utilisateur (créées par lui ou qui lui sont assignées)
                    var tasksFromDb = await context.Tasks
                        .Include(t => t.Author)
                        .Include(t => t.Assignee)
                        .Include(t => t.Category)
                        .Include(t => t.SubTasks)
                        .Include(t => t.Comments)
                        .Include(t => t.Tags)
                        .Where(t => t.AuthorId == currentUserId || t.AssigneeId == currentUserId)
                        .ToListAsync();

                    _allTasks = tasksFromDb;
                    ApplyFiltersAndSort();
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erreur", $"Erreur lors du chargement des tâches : {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ApplyFiltersAndSort()
        {
            // Commencer avec toutes les tâches
            var filteredTasks = _allTasks.AsQueryable();

            // Filtrer par texte de recherche
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLower();
                filteredTasks = filteredTasks.Where(t => 
                    t.Title.ToLower().Contains(searchLower) || 
                    t.Description.ToLower().Contains(searchLower));
            }

            // Filtrer par statut
            if (FilterStatus != null)
            {
                filteredTasks = filteredTasks.Where(t => t.Status == FilterStatus);
            }

            // Filtrer par priorité
            if (FilterPriority != null)
            {
                filteredTasks = filteredTasks.Where(t => t.Priority == FilterPriority);
            }

            // Filtrer par catégorie
            if (FilterCategory != null)
            {
                filteredTasks = filteredTasks.Where(t => t.CategoryId == FilterCategory.Id);
            }

            // Filtrer les tâches terminées
            if (!ShowCompletedTasks)
            {
                filteredTasks = filteredTasks.Where(t => t.Status != Status.Done);
            }

            // Appliquer le tri
            filteredTasks = SortBy switch
            {
                "Titre" => filteredTasks.OrderBy(t => t.Title),
                "Priorité" => filteredTasks.OrderByDescending(t => t.Priority),
                "Statut" => filteredTasks.OrderBy(t => t.Status),
                "Date de création" => filteredTasks.OrderByDescending(t => t.CreatedAt),
                _ => filteredTasks.OrderBy(t => t.DueDate) // Par défaut, trier par date d'échéance
            };

            // Mettre à jour la collection observable
            Tasks.Clear();
            foreach (var task in filteredTasks)
            {
                Tasks.Add(task);
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFiltersAndSort();
        }

        partial void OnFilterStatusChanged(Status? value)
        {
            ApplyFiltersAndSort();
        }

        partial void OnFilterPriorityChanged(Priority? value)
        {
            ApplyFiltersAndSort();
        }

        partial void OnFilterCategoryChanged(Category? value)
        {
            ApplyFiltersAndSort();
        }

        partial void OnShowCompletedTasksChanged(bool value)
        {
            ApplyFiltersAndSort();
        }

        partial void OnSortByChanged(string value)
        {
            ApplyFiltersAndSort();
        }

        [RelayCommand]
        private void ClearFilters()
        {
            SearchText = string.Empty;
            FilterStatus = null;
            FilterPriority = null;
            FilterCategory = null;
            ShowCompletedTasks = true;
            SortBy = "DueDate";
            ApplyFiltersAndSort();
        }

        private async void LoadTasks()
        {
            await LoadTasksAsync();
        }

        private async Task OnAddTask()
        {
            await Shell.Current.GoToAsync(nameof(AddTaskPage));
        }

        [RelayCommand]
        private async Task EditTaskAsync(TodoTask task)
        {
            if (task == null)
            {
                Console.WriteLine("La tâche est null");
                return;
            }

            Console.WriteLine($"Tentative de modification de la tâche avec l'ID : {task.Id}");
            var navigationParameter = new Dictionary<string, object>
            {
                { "TaskId", task.Id }
            };
            
            await Shell.Current.GoToAsync(nameof(TaskDetailPage), navigationParameter);
        }

        [RelayCommand]
        private async Task DeleteTaskAsync(TodoTask task)
        {
            if (task == null)
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Confirmation",
                "Êtes-vous sûr de vouloir supprimer cette tâche ?",
                "Oui",
                "Non");

            if (confirm)
            {
                try
                {
                    using (var context = await _contextFactory.CreateDbContextAsync())
                    {
                        // On doit attacher l'entité au nouveau contexte
                        context.Tasks.Attach(task);
                        context.Tasks.Remove(task);
                        await context.SaveChangesAsync();
                        _allTasks.Remove(task);
                        ApplyFiltersAndSort();
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Erreur", $"Erreur lors de la suppression : {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        private Task ViewTaskDetailsAsync(TodoTask task)
        {
            // TODO: Implémenter la vue détaillée
            return Task.CompletedTask;
        }
    }
} 