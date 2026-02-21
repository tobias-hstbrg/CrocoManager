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
    public partial class RegisterViewModel : BaseViewModel
    {

        [ObservableProperty] string? email;
        [ObservableProperty] string? password;

        public RegisterViewModel(
            INavigationService navigationService,
            INotificationService notificationService,
            IAuthService authService) 
            : base(navigationService, notificationService, authService)
        {
        }

        [RelayCommand]
        async Task RegisterUserAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await NotificationService.ShowErrorAsync("Validierungsfehler", "Bitte geben Sie eine gültige E-Mail Adresse und ein Passwort an.");
                return;
            }

            if (!IsValidPassword(Password))
            {
                await NotificationService.ShowWarningAsync("Passwort-Richtlinien",
                    "Das Passwort muss mindestens 8 Zeichen lang sein und Großbuchstaben, Kleinbuchstaben sowie Sonderzeichen oder Zahlen enthalten.");
                return;
            }

            try
            {
                var session = await AuthService.RegisterAsync(Email, Password);

                if (session != null)
                {
                    await NotificationService.ShowSuccessAsync("Erfolg", $"Registrierung für {session.User.Email} erfolgreich.");
                }
                else
                {
                    await NotificationService.ShowErrorAsync("Fehler", "Registrierung fehlgeschlagen. Bitte versuche es erneut.");
                }
            }
            catch (InvalidOperationException ex)
            {
                await NotificationService.ShowErrorAsync("Whitelist Fehler", ex.Message);
            }
            catch (Exception)
            {
                await NotificationService.ShowErrorAsync("Fehler", "Ein unerwarteter Fehler ist aufgetreten.");
            }
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8) return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigitOrSpecial = password.Any(c => char.IsDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c));

            return hasUpper && hasLower && hasDigitOrSpecial;
        }

        [RelayCommand]
        private void GoToLogin()
        {
            NavigationService.SetRoot("Login");
        }
    }
}
