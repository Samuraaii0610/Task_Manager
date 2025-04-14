using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.Views;

namespace TaskManager.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string usernameOrEmail = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private bool rememberMe = false;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isError = false;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            Title = "Connexion";
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(UsernameOrEmail) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Veuillez remplir tous les champs";
                IsError = true;
                return;
            }

            IsBusy = true;
            IsError = false;

            try
            {
                var request = new Models.LoginRequest
                {
                    UsernameOrEmail = UsernameOrEmail,
                    Password = Password,
                    RememberMe = RememberMe
                };

                var response = await _authService.Login(request);

                if (response.Success)
                {
                    // Rediriger vers la page principale
                    await Shell.Current.GoToAsync("//tasks");
                }
                else
                {
                    ErrorMessage = response.Message ?? "Erreur de connexion";
                    IsError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Une erreur est survenue: {ex.Message}";
                IsError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }
    }
} 