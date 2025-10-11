using CrocoManager.Interfaces;
using CrocoManager.Services;
using CrocoManager.ViewModel;
using CrocoManager.Views;
using Microsoft.Extensions.Logging;

namespace CrocoManager
{
    public static class MauiProgram
    {
        public static IServiceProvider ServiceProvider { get; private set; }
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

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Pages & ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();

            // Shell
            builder.Services.AddSingleton<AppShell>();

            // Auth service via async factory (blocking)
            builder.Services.AddSingleton<IAuthService>(sp =>
                SupabaseAuthService.CreateAsync().GetAwaiter().GetResult()
            );

            var app = builder.Build();
            ServiceProvider = app.Services;

            return app;
        }
    }
}
