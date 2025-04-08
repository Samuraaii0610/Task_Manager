using Microsoft.Extensions.Logging;
using TaskManager.Data;
using Microsoft.EntityFrameworkCore;

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

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
