using TaskManager.ViewModels;

namespace TaskManager.Views
{
    public partial class TaskDetailPage : ContentPage
    {
        public TaskDetailPage(TaskDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
} 