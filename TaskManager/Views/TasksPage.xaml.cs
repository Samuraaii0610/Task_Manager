using TaskManager.ViewModels;

namespace TaskManager.Views;

public partial class TasksPage : ContentPage
{
    public TasksPage()
    {
        InitializeComponent();
        BindingContext = new TasksViewModel();
    }
} 