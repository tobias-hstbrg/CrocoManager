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
                await NotificationService.ShowErrorAsync("Error", "Enter credentials");
                return;
            }

            if (!IsValidPassword(Password))
            {
                await NotificationService.ShowWarningAsync("Passwort-Richtlinien",
                    "Das Passwort muss mindestens 8 Zeichen lang sein und Großbuchstaben, Kleinbuchstaben sowie Sonderzeichen oder Zahlen enthalten.");
                return;
            }

            var session = await AuthService.RegisterAsync(Email, Password);

            if (session != null)
            {
                await NotificationService.ShowSuccessAsync("Success", $"Registred {session.User.Email}");
            }
            else
            {
                await NotificationService.ShowErrorAsync("Error", "Registration failed");
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
