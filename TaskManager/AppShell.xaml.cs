using TaskManager.Views;
using TaskManager.Services;
using Microsoft.Maui.Controls;
using System.Windows.Input;
using TaskManager.ViewModels;

namespace TaskManager;

public partial class AppShell : Shell
{
	private readonly AuthService _authService;
	
	public ICommand LogoutCommand { get; }
	
	public AppShell(AuthService authService)
	{
		InitializeComponent();
		
		_authService = authService;
		LogoutCommand = new Command(async () => await LogoutAsync());
		
		BindingContext = this;
		
		// Enregistrement des routes pour la navigation
		Routing.RegisterRoute(nameof(TaskDetailPage), typeof(TaskDetailPage));
		Routing.RegisterRoute(nameof(AddTaskPage), typeof(AddTaskPage));
		Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
	}
	
	private async Task LogoutAsync()
	{
	    bool confirm = await DisplayAlert("Confirmation", "Voulez-vous vraiment vous déconnecter ?", "Oui", "Non");
	    
	    if (confirm)
	    {
	        await _authService.Logout();
	        await GoToAsync("//login");
	    }
	}
	
	protected override async void OnAppearing()
	{
	    base.OnAppearing();
	    
	    var authStatus = await _authService.CheckAuthStatus();
	    if (authStatus == null || !authStatus.Success)
	    {
	        await GoToAsync("//login");
	    }
	}
}
