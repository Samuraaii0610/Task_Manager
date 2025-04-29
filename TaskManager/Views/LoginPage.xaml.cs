using TaskManager.Services;
using TaskManager.ViewModels;

namespace TaskManager.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(AuthService authService)
        {
            InitializeComponent();
            BindingContext = new LoginViewModel(authService);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }
    }
} 