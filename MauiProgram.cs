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
            builder.Services.AddTransient<AdminViewModel>();
            builder.Services.AddTransient<HomeViewModel>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<AdminPage>();
            builder.Services.AddTransient<HomePage>();

            // Shell
            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddSingleton<SupabaseClientService>();
            builder.Services.AddSingleton<IAuthService, SupabaseAuthService>();
            builder.Services.AddSingleton<IWhitelistService, WhitelistService>();

            var app = builder.Build();
            ServiceProvider = app.Services;

            return app;
        }
    }
}
