using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.ViewModels
{
    public partial class PasswordResetViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _passwordCheck = string.Empty;

        public PasswordResetViewModel(
            INavigationService navigationService,
            INotificationService notificationService,
            IAuthService authService,
            IConnectivityService connectivityService) 
            : base(navigationService, notificationService, authService, connectivityService)
        {
        }

        [RelayCommand]
        private async Task ResetPassword()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await NotificationService.ShowWarningAsync("Fehler", "Bitte geben Sie Ihre E-Mail-Adresse ein.");
                return;
            }

            if(string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(PasswordCheck))
            {
                await NotificationService.ShowWarningAsync("Fehler", "Bitte geben Sie ein Passwort ein.");

                return;
            }

            if(Password.Trim() != PasswordCheck.Trim())
            {
                await NotificationService.ShowWarningAsync("Fehler", "Das Passwort stimmt nicht überein");
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
                IsBusy = true;
                bool result = await AuthService.ResetPasswordAsync(Email, Password);

                if (!result)
                {
                    await NotificationService.ShowErrorAsync("Fehler", "Passwort konnte nicht zurückgesetzt werden. Bitte versuchen Sie es erneut.");
                    return;
                }

                await NotificationService.ShowWarningAsync("Erfolg", "Passwort erfolgreich geändert!");
                GoToLogin();
            }
            catch (Exception ex)
            {
                await DisplayError("Fehler beim Zurücksetzen", ex);
            }
            finally
            {
                IsBusy = false;
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
