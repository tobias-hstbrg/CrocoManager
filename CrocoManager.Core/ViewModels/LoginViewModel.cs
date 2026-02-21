using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string? email;
        [ObservableProperty]
        private string? password;

        public LoginViewModel(
            INavigationService navigationService,
            INotificationService notificationService,
            IAuthService authService) 
            : base(navigationService, notificationService, authService)
        {
        }

        [RelayCommand]
        async Task LoginUserAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await NotificationService.ShowErrorAsync("Validierungsfehler", "Bitte geben sie eine gültige E-Mail Adresse und ein Passwort an.");
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
                await NotificationService.ShowErrorAsync("Fehler", "Nutzerdaten konnten nicht abgerufen werden. Bitte versuchen sie es später erneut.");
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
                NavigationService.SetRoot("Admin");
            }
            else
            {
                NavigationService.SetRoot("AppShell");
                await NavigationService.GoToAsync("//HomePage");
            }

        }

        [RelayCommand]
        async Task TestConnectionAsync()
        {
            var ok = await AuthService.TestConnectionAsync();
            await NotificationService.ShowInfoAsync("Verbindungstest", ok ? "Supabase Verbindung aktiv" : "Supabase Verbindung inaktiv");
        }

        [RelayCommand]
        private void GoToRegister()
        {
            NavigationService.SetRoot("Register");
        }

        [RelayCommand]
        private void GoToResetPassword()
        {
            NavigationService.SetRoot("ResetPassword");
        }
    }
}
