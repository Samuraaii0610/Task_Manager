using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskManager.Models;
using System.Threading.Tasks;

namespace TaskManager.ViewModels
{
    public partial class TaskDetailViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private DateTime dueDate = DateTime.Now.AddDays(1);

        [ObservableProperty]
        private Priority priority = Priority.Medium;

        [ObservableProperty]
        private Status status = Status.ToDo;

        private TodoTask? _task;
        private bool _isNewTask;

        public TaskDetailViewModel(TodoTask? task = null)
        {
            Title = task == null ? "Nouvelle Tâche" : "Modifier la Tâche";
            _task = task;
            _isNewTask = task == null;

            if (task != null)
            {
                Title = task.Title;
                Description = task.Description;
                DueDate = task.DueDate;
                Priority = task.Priority;
                Status = task.Status;
            }
            else
            {
                DueDate = DateTime.Now.AddDays(7);
                Priority = Priority.Medium;
                Status = Status.ToDo;
            }
        }

        [RelayCommand]
        private async Task SaveTaskAsync()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                await Shell.Current.DisplayAlert("Erreur", "Le titre est obligatoire", "OK");
                return;
            }

            if (_isNewTask)
            {
                _task = new TodoTask();
            }

            _task.Title = Title;
            _task.Description = Description;
            _task.DueDate = DueDate;
            _task.Priority = Priority;
            _task.Status = Status;

            // TODO: Sauvegarder dans la base de données

            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
} 