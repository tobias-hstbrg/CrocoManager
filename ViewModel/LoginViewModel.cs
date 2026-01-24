using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Interfaces;
using CrocoManager.Services;
using CrocoManager.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.ViewModel
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? email;
        [ObservableProperty]
        private string? password;

        private readonly IAuthService _authService;
        private readonly IServiceProvider _serviceProvider;
        public LoginViewModel(IAuthService authService, IServiceProvider serviceProvider)
        {
            _authService = authService;
            _serviceProvider = serviceProvider;
        }

        [RelayCommand]
        async Task LoginUserAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                if (Application.Current?.Windows?.FirstOrDefault()?.Page is Page page)
                {
                    await page.DisplayAlert("Error", "Please enter both email and password", "OK");
                }
                return;
            }

            var session = await _authService.LoginAsync(Email, Password);

            if (session == null)
            {
                if(Application.Current?.Windows?.FirstOrDefault()?.Page is Page page)
                {
                    await page.DisplayAlert("Anmeldung fehlgeschlagen", "Email oder Passwort falsch. Bitte versuche es erneut.", "OK");
                }
                return;
            }

            // Session exists but user data is missing (shouldn't happen, but you never know)
            if (session.User?.UserMetadata == null)
            {
                if(Application.Current?.Windows?.FirstOrDefault()?.Page is Page page)
                {
                    await page.DisplayAlert("Error", "Nutzerdaten konnten nicht abgerufen werden. Bitte versuchen sie es später erneut.", "OK");
                }
                return;
            }

            // User hasn't been assigned a role yet
            if (session.User.UserMetadata.Role == Models.UserRole.NotAssigned)
            {
                if (Application.Current?.Windows?.FirstOrDefault()?.Page is Page page )
                {
                    await page.DisplayAlert("Account in Bearbeitung", "Ihr Account ist noch keiner Rolle zugewiesen worden. Bitte kontaktieren Sie ihren Administrator.", "OK");
                }
                return;
            }

            if (session.User.UserMetadata.Role == Models.UserRole.Admin)
            {
                var adminPage = _serviceProvider.GetRequiredService<AdminPage>();
                if (Application.Current?.Windows?.FirstOrDefault() is Window window)
                {
                    window.Page = adminPage;
                }
            }
            else
            {
                var appShell = new AppShell(_authService, startRoute: "//HomePage");
                if (Application.Current?.Windows?.FirstOrDefault() is Window window)
                {
                    window.Page = appShell;
                }
            }

        }

        [RelayCommand]
        async Task TestConnectionAsync()
        {
            //bool ok = await SupabaseService.Instance.TestConnectionAsync();
            
            var ok = await _authService.TestConnectionAsync();
            if (Application.Current?.Windows?.FirstOrDefault()?.Page is Page page)
            {
                await page.DisplayAlert("Connection Test", ok ? "Connected" : "Failed", "OK");
            }
        }

        [RelayCommand]
        private void GoToRegister()
        {
            var page = _serviceProvider.GetRequiredService<RegisterPage>();
            if (Application.Current?.Windows?.FirstOrDefault() is Window window)
            {
                window.Page = page;
            }
        }

        [RelayCommand]
        private void GoToResetPassword()
        {
            var page = _serviceProvider.GetRequiredService<ResetPasswordPage>();
            if (Application.Current?.Windows?.FirstOrDefault() is Window window)
            {
                window.Page = page;
            }
        }
    }
}
