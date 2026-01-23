using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Interfaces;
using CrocoManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.ViewModel
{
    public partial class PasswordResetViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _passwordCheck = string.Empty;

        private readonly IAuthService _authService;
        public PasswordResetViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task ResetPassword()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await Shell.Current.DisplayAlert("Fehler", "Bitte geben Sie Ihre E-Mail-Adresse ein.", "OK");
                return;
            }

            if(string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(PasswordCheck))
            {
                await Shell.Current.DisplayAlert("Fehler", "Bitte geben Sie ein Passwort ein.", "OK");
                return;
            }

            if(Password.Trim() != PasswordCheck.Trim())
            {
                await Shell.Current.DisplayAlert("Fehler", "Das Passwort stimmt nicht überein", "OK");
                return;
            }

            await _authService.ResetPasswordAsync(Email, Password);
            await Shell.Current.DisplayAlert("Erfolg", "Passwort erfolgreich geändert!", "OK");
            
            // For now, just go back to the login page
            await GoToLogin();
        }

        [RelayCommand]
        private async Task GoToLogin()
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}