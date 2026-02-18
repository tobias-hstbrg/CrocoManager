using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Core.Interfaces;
using CrocoManager.Services;
using CrocoManager.Core.Models;
using CrocoManager.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.ViewModel
{
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string? email;
        [ObservableProperty]
        private string? password;

        public LoginViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [RelayCommand]
        async Task LoginUserAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await NotificationService.ShowErrorAsync("Error", "Please enter both email and password");
                return;
            }

            var session = await AuthService.LoginAsync(Email, Password);

            if (session == null)
            {
                await NotificationService.ShowErrorAsync("Anmeldung fehlgeschlagen", "Email oder Passwort falsch. Bitte versuche es erneut.");
                return;
            }

            // Session exists but user data is missing (shouldn't happen, but you never know)
            if (session.User?.UserMetadata == null)
            {
                await NotificationService.ShowErrorAsync("Error", "Nutzerdaten konnten nicht abgerufen werden. Bitte versuchen sie es später erneut.");
                return;
            }

            // User hasn't been assigned a role yet
            if (session.User.UserMetadata.Role == Core.Models.UserRole.NotAssigned)
            {
                await NotificationService.ShowInfoAsync("Account in Bearbeitung", "Ihr Account ist noch keiner Rolle zugewiesen worden. Bitte kontaktieren Sie ihren Administrator.");
                return;
            }

            if (session.User.UserMetadata.Role == Core.Models.UserRole.Admin)
            {
                var adminPage = ServiceProvider.GetRequiredService<AdminPage>();
                if (Application.Current?.Windows?.FirstOrDefault() is Window window)
                {
                    window.Page = adminPage;
                }
            }
            else
            {
                var appShell = new AppShell(AuthService, startRoute: "//HomePage");
                if (Application.Current?.Windows?.FirstOrDefault() is Window window)
                {
                    window.Page = appShell;
                }
            }

        }

        [RelayCommand]
        async Task TestConnectionAsync()
        {
            var ok = await AuthService.TestConnectionAsync();
            await NotificationService.ShowInfoAsync("Connection Test", ok ? "Connected" : "Failed");
        }

        [RelayCommand]
        private void GoToRegister()
        {
            var page = ServiceProvider.GetRequiredService<RegisterPage>();
            if (Application.Current?.Windows?.FirstOrDefault() is Window window)
            {
                window.Page = page;
            }
        }

        [RelayCommand]
        private void GoToResetPassword()
        {
            var page = ServiceProvider.GetRequiredService<ResetPasswordPage>();
            if (Application.Current?.Windows?.FirstOrDefault() is Window window)
            {
                window.Page = page;
            }
        }
    }
}
