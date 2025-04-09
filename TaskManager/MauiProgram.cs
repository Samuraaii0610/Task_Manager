using Microsoft.Extensions.Logging;
using TaskManager.Data;
using Microsoft.EntityFrameworkCore;
using TaskManager.ViewModels;
using TaskManager.Views;

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
		builder.Services.AddDbContext<ApplicationDbContext>(options =>
			options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

		// Enregistrement des ViewModels
		builder.Services.AddTransient<TasksViewModel>();
		builder.Services.AddTransient<TaskDetailViewModel>();

		// Enregistrement des Pages
		builder.Services.AddTransient<TaskDetailPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
