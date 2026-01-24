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
        private readonly IServiceProvider _serviceProvider;
        public PasswordResetViewModel(IAuthService authService, IServiceProvider serviceProvider)
        {
            _authService = authService;
            _serviceProvider = serviceProvider;
        }

        [RelayCommand]
        private async Task ResetPassword()
        {
            var currentPage = Application.Current?.Windows?.FirstOrDefault()?.Page;

            if (string.IsNullOrWhiteSpace(Email))
            {
                await ShowAlert("Fehler", "Bitte geben Sie Ihre E-Mail-Adresse ein.");
                return;
            }

            if(string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(PasswordCheck))
            {
                await ShowAlert("Fehler", "Bitte geben Sie ein Passwort ein.");

                return;
            }

            if(Password.Trim() != PasswordCheck.Trim())
            {
                await ShowAlert("Fehler", "Das Passwort stimmt nicht überein");
                return;
            }

            await _authService.ResetPasswordAsync(Email, Password);

            await ShowAlert("Erfolg", "Passwort erfolgreich geändert!");

            GoToLogin();
        }

        private async Task ShowAlert(string title, string message)
        {
            var page = Application.Current?.Windows?.FirstOrDefault()?.Page;
            if (page != null)
            {
                await page.DisplayAlert(title, message, "OK");
            }
        }

        [RelayCommand]
        private void GoToLogin()
        {
            var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
            if (Application.Current?.Windows?.FirstOrDefault() is Window window)
            {
                window.Page = loginPage;
            }
        }
    }
}