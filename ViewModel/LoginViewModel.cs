using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Interfaces;
using CrocoManager.Services;
using CrocoManager.Views;
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
        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        async Task LoginUserAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter both email and password", "OK");
                return;
            }

            var session = await _authService.LoginAsync(Email, Password);

            if (session == null)
            {
                await Shell.Current.DisplayAlert("Anmeldung fehlgeschlagen", "Email oder Passwort falsch. Bitte versuche es erneut.", "OK");
                return;
            }

            // Session exists but user data is missing (shouldn't happen, but you never know)
            if (session.User?.UserMetadata == null)
            {
                await Shell.Current.DisplayAlert("Error", "Nutzerdaten konnten nicht abgerufen werden. Bitte versuchen sie es später erneut.", "OK");
                return;
            }

            // User hasn't been assigned a role yet
            if (session.User.UserMetadata.Role == Models.UserRole.NotAssigned)
            {
                await Shell.Current.DisplayAlert("Account in Bearbeitung", "Ihr Account ist noch keiner Rolle zugewiesen worden. Bitte kontaktieren Sie ihren Administrator.", "OK");
                return;
            }

            // Valid login - navigate based on role
            if (session.User.UserMetadata.Role == Models.UserRole.Admin)
            {
                await Shell.Current.GoToAsync(nameof(AdminPage));
            }
            else
            {
                await Shell.Current.GoToAsync(nameof(HomePage));
            }

        }

        [RelayCommand]
        async Task TestConnectionAsync()
        {
            //bool ok = await SupabaseService.Instance.TestConnectionAsync();
            
            var ok = await _authService.TestConnectionAsync();
            await Shell.Current.DisplayAlert("Connection Test", ok ? "Connected" : "Failed", "OK");
        }

        [RelayCommand]
        async Task GetTextMessageAsync()
        {
            var msg = await _authService.GetTextMessageAsync();
            await Shell.Current.DisplayAlert(
                "Supabase Response",
                msg ?? "No message found",
                "OK");
        }

        [RelayCommand]
        private async Task GoToRegister()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }
    }
}
