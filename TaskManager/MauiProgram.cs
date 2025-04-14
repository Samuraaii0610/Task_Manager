using Microsoft.Extensions.Logging;
using TaskManager.Data;
using Microsoft.EntityFrameworkCore;
using TaskManager.ViewModels;
using TaskManager.Views;
using TaskManager.Services;

namespace TaskManager;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Configuration de la base de données
		var connectionString = "Server=localhost;Database=taskmanager;User=root;Password=;";
		builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
			options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

		// Enregistrement des Services
		builder.Services.AddSingleton<AuthService>();

		// Enregistrement des ViewModels
		builder.Services.AddTransient<TasksViewModel>();
		builder.Services.AddTransient<TaskDetailViewModel>();
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<RegisterViewModel>();
		builder.Services.AddTransient<ProfileViewModel>();

		// Enregistrement des Pages
		builder.Services.AddTransient<TaskDetailPage>();
		builder.Services.AddTransient<TasksPage>();
		builder.Services.AddTransient<AddTaskPage>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RegisterPage>();
		builder.Services.AddTransient<ProfilePage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();

		// Initialiser la base de données avec des données de test
		using (var scope = app.Services.CreateScope())
		{
			var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
			using (var context = contextFactory.CreateDbContext())
			{
				DbInitializer.Initialize(context);
			}
		}

		return app;
	}
}
