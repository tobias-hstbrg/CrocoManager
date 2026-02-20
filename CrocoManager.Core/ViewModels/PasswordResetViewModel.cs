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
            IAuthService authService) 
            : base(navigationService, notificationService, authService)
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

            bool result =  await AuthService.ResetPasswordAsync(Email, Password);

            if(!result)
            {
                await  NotificationService.ShowErrorAsync("Fehler", "Passwort konnte nicht zurückgesetzt werden. Bitte versuchen Sie es erneut.");
                return;
            }

            await NotificationService.ShowWarningAsync("Erfolg", "Passwort erfolgreich geändert!");

            GoToLogin();
        }

        [RelayCommand]
        private void GoToLogin()
        {
            NavigationService.SetRoot("Login");
        }
    }
}
