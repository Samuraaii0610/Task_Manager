using TaskManager.ViewModels;
using TaskManager.Data;
using Microsoft.EntityFrameworkCore;
using TaskManager.Services;

namespace TaskManager.Views
{
    public partial class AddTaskPage : ContentPage
    {
        public AddTaskPage(IDbContextFactory<ApplicationDbContext> contextFactory, AuthService authService)
        {
            InitializeComponent();
            BindingContext = new TaskDetailViewModel(contextFactory, authService);
        }
    }
} 