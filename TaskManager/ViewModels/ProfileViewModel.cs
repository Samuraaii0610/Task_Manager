using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly AuthService _authService;
        private Models.UserDto? _currentUser;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string firstName = string.Empty;

        [ObservableProperty]
        private string lastName = string.Empty;

        [ObservableProperty]
        private string currentPassword = string.Empty;

        [ObservableProperty]
        private string newPassword = string.Empty;

        [ObservableProperty]
        private string confirmNewPassword = string.Empty;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        [ObservableProperty]
        private bool isSuccess = false;

        [ObservableProperty]
        private bool isError = false;

        [ObservableProperty]
        private int totalTasks = 0;

        [ObservableProperty]
        private int completedTasks = 0;

        [ObservableProperty]
        private int pendingTasks = 0;

        public ProfileViewModel(IDbContextFactory<ApplicationDbContext> contextFactory, AuthService authService)
        {
            _contextFactory = contextFactory;
            _authService = authService;
            Title = "Mon Profil";
        }

        public async Task LoadProfileAsync()
        {
            IsBusy = true;
            
            try
            {
                var authStatus = await _authService.CheckAuthStatus();
                if (authStatus == null || !authStatus.Success || authStatus.User == null)
                {
                    // Non connecté, rediriger vers la page de connexion
                    await Shell.Current.GoToAsync("//login");
                    return;
                }

                _currentUser = authStatus.User;
                
                // Charger les informations de l'utilisateur
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    var userAccount = await context.UserAccounts
                        .Include(ua => ua.User)
                        .FirstOrDefaultAsync(ua => ua.Id == _currentUser.Id);
                    
                    if (userAccount?.User != null)
                    {
                        Username = userAccount.Username;
                        Email = userAccount.Email;
                        FirstName = userAccount.User.FirstName;
                        LastName = userAccount.User.LastName;
                        
                        // Charger les statistiques des tâches
                        var tasks = await context.Tasks
                            .Where(t => t.AuthorId == userAccount.User.Id || t.AssigneeId == userAccount.User.Id)
                            .ToListAsync();
                        
                        TotalTasks = tasks.Count;
                        CompletedTasks = tasks.Count(t => t.Status == Status.Done);
                        PendingTasks = TotalTasks - CompletedTasks;
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors du chargement du profil: {ex.Message}";
                IsError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task UpdateProfileAsync()
        {
            if (_currentUser == null)
                return;
                
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                StatusMessage = "Veuillez remplir le prénom et le nom";
                IsError = true;
                IsSuccess = false;
                return;
            }

            IsBusy = true;
            IsError = false;
            IsSuccess = false;

            try
            {
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    var user = await context.Users
                        .FirstOrDefaultAsync(u => u.Id == _currentUser.Id);
                    
                    if (user != null)
                    {
                        user.FirstName = FirstName;
                        user.LastName = LastName;
                        
                        await context.SaveChangesAsync();
                        
                        StatusMessage = "Profil mis à jour avec succès";
                        IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la mise à jour du profil: {ex.Message}";
                IsError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            if (_currentUser == null)
                return;
                
            if (string.IsNullOrWhiteSpace(CurrentPassword) || 
                string.IsNullOrWhiteSpace(NewPassword) || 
                string.IsNullOrWhiteSpace(ConfirmNewPassword))
            {
                StatusMessage = "Veuillez remplir tous les champs de mot de passe";
                IsError = true;
                IsSuccess = false;
                return;
            }

            if (NewPassword != ConfirmNewPassword)
            {
                StatusMessage = "Les nouveaux mots de passe ne correspondent pas";
                IsError = true;
                IsSuccess = false;
                return;
            }

            if (NewPassword.Length < 6)
            {
                StatusMessage = "Le nouveau mot de passe doit contenir au moins 6 caractères";
                IsError = true;
                IsSuccess = false;
                return;
            }

            IsBusy = true;
            IsError = false;
            IsSuccess = false;

            // Pour cet exemple, la vérification et mise à jour du mot de passe seraient intégrées dans AuthService
            // Cette fonction serait à ajouter à AuthService
            StatusMessage = "Changement de mot de passe non implémenté pour cet exemple";
            IsError = true;

            IsBusy = false;
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            await _authService.Logout();
            await Shell.Current.GoToAsync("//login");
        }
    }
} 