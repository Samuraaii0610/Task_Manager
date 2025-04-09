using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TaskManager.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using TaskManager.Views;

namespace TaskManager.ViewModels
{
    public partial class TasksViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ObservableCollection<TodoTask> tasks;

        [ObservableProperty]
        private TodoTask selectedTask;

        public ICommand AddTaskCommand { get; }

        public TasksViewModel()
        {
            Title = "Mes Tâches";
            Tasks = new ObservableCollection<TodoTask>();

            // Données mockées pour le test
            Tasks = new ObservableCollection<TodoTask>
            {
                new TodoTask 
                { 
                    Title = "Tâche exemple 1",
                    Description = "Description de la tâche 1",
                    DueDate = DateTime.Now.AddDays(7),
                    Priority = Priority.Medium,
                    Status = Status.ToDo
                },
                new TodoTask 
                { 
                    Title = "Tâche exemple 2",
                    Description = "Description de la tâche 2",
                    DueDate = DateTime.Now.AddDays(3),
                    Priority = Priority.High,
                    Status = Status.InProgress
                }
            };

            AddTaskCommand = new Command(async () => await OnAddTask());
        }

        private async Task OnAddTask()
        {
            await Shell.Current.GoToAsync(nameof(TaskDetailPage));
        }

        [RelayCommand]
        private async Task EditTaskAsync(TodoTask task)
        {
            // TODO: Implémenter la modification de tâche
        }

        [RelayCommand]
        private async Task DeleteTaskAsync(TodoTask task)
        {
            // TODO: Implémenter la suppression de tâche
        }

        [RelayCommand]
        private async Task ViewTaskDetailsAsync(TodoTask task)
        {
            // TODO: Implémenter la vue détaillée
        }
    }
} 