using TaskManager.ViewModels;
using TaskManager.Services;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;

namespace TaskManager.Views
{
    public partial class ProfilePage : ContentPage
    {
        private readonly ProfileViewModel _viewModel;

        public ProfilePage(IDbContextFactory<ApplicationDbContext> contextFactory, AuthService authService)
        {
            InitializeComponent();
            _viewModel = new ProfileViewModel(contextFactory, authService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadProfileAsync();
        }
    }
} 