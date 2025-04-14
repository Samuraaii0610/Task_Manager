using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string confirmPassword = string.Empty;

        [ObservableProperty]
        private string firstName = string.Empty;

        [ObservableProperty]
        private string lastName = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isError = false;

        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
            Title = "Inscription";
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            // Validation des champs
            if (string.IsNullOrWhiteSpace(Username) || 
                string.IsNullOrWhiteSpace(Email) || 
                string.IsNullOrWhiteSpace(Password) || 
                string.IsNullOrWhiteSpace(ConfirmPassword) ||
                string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName))
            {
                ErrorMessage = "Veuillez remplir tous les champs";
                IsError = true;
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Les mots de passe ne correspondent pas";
                IsError = true;
                return;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères";
                IsError = true;
                return;
            }

            IsBusy = true;
            IsError = false;

            try
            {
                var request = new Models.RegisterRequest
                {
                    Username = Username,
                    Email = Email,
                    Password = Password,
                    ConfirmPassword = ConfirmPassword,
                    FirstName = FirstName,
                    LastName = LastName
                };

                var response = await _authService.Register(request);

                if (response.Success)
                {
                    // Rediriger vers la page principale
                    await Shell.Current.GoToAsync("//tasks");
                }
                else
                {
                    ErrorMessage = response.Message ?? "Erreur d'inscription";
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
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
} 