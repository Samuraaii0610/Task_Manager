using TaskManager.Services;
using TaskManager.ViewModels;

namespace TaskManager.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage(AuthService authService)
        {
            InitializeComponent();
            BindingContext = new RegisterViewModel(authService);
        }
    }
} 