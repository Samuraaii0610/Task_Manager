using TaskManager.ViewModels;
using TaskManager.Models;
using Microsoft.Maui.Controls;
using TaskManager.Data;
using Microsoft.EntityFrameworkCore;
using TaskManager.Services;

namespace TaskManager.Views
{
    [QueryProperty(nameof(TaskId), "TaskId")]
    public partial class TaskDetailPage : ContentPage
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly AuthService _authService;
        private TaskDetailViewModel? _viewModel;
        private int _taskId;

        public int TaskId 
        { 
            get => _taskId; 
            set 
            {
                _taskId = value;
                LoadTaskAsync(_taskId);
            }
        }

        public TaskDetailPage(IDbContextFactory<ApplicationDbContext> contextFactory, AuthService authService)
        {
            InitializeComponent();
            _contextFactory = contextFactory;
            _authService = authService;
        }

        private async void LoadTaskAsync(int taskId)
        {
            try
            {
                Console.WriteLine($"Chargement de la tâche avec l'ID : {taskId}");
                
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    var taskFromDb = await context.Tasks
                        .Include(t => t.Author)
                        .Include(t => t.Assignee)
                        .FirstOrDefaultAsync(t => t.Id == taskId);

                    if (taskFromDb != null)
                    {
                        Console.WriteLine($"Tâche trouvée : {taskFromDb.Title}");
                        _viewModel = new TaskDetailViewModel(taskFromDb, _contextFactory, _authService);
                        BindingContext = _viewModel;
                    }
                    else
                    {
                        Console.WriteLine("Tâche non trouvée dans la base de données");
                        await Shell.Current.DisplayAlert("Erreur", "Tâche non trouvée", "OK");
                        await Shell.Current.GoToAsync("..");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
                Console.WriteLine($"Stack trace : {ex.StackTrace}");
                await Shell.Current.DisplayAlert("Erreur", $"Erreur lors du chargement de la tâche : {ex.Message}", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
    }
} 