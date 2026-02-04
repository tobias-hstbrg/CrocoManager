using CrocoManager.Interfaces;
using CrocoManager.Services;
using CrocoManager.ViewModel;
using CrocoManager.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace CrocoManager
{
    public static class MauiProgram
    {
        private static IServiceProvider? _serviceProvider;

        public static IServiceProvider ServiceProvider
        {
            get => _serviceProvider ?? throw new InvalidOperationException("ServiceProvider has not been initialized.");
            private set => _serviceProvider = value;
        }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    //fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    //fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Inter-VariableFont_opsz,wght.ttf", "Inter");
                    fonts.AddFont("Inter-Italic-VariableFont_opsz,wght.ttf", "InterItalic");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Pages & ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<AdminViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<PasswordResetViewModel>();
            builder.Services.AddTransient<AnimalViewModel>();
            builder.Services.AddTransient<FeedingPlanViewModel>();
            builder.Services.AddTransient<FeedingViewModel>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<AdminPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<ResetPasswordPage>();
            builder.Services.AddTransient<AnimalPage>();
            builder.Services.AddTransient<FeedingPlanPage>();
            builder.Services.AddTransient<FeedingPage>();

            // Shell
            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddSingleton<SupabaseClientService>();
            builder.Services.AddSingleton<IAuthService, SupabaseAuthService>();
            builder.Services.AddSingleton<IWhitelistService, WhitelistService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();
            builder.Services.AddSingleton<AnimalService>();
            builder.Services.AddSingleton<FeedingPlanService>();
            builder.Services.AddHttpClient<ObservationService>();
            builder.Services.AddSingleton<FeedingService>();

            var app = builder.Build();
            _serviceProvider = app.Services;

            return app;
        }
    }
}
