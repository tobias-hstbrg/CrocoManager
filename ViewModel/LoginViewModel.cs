using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Interfaces;
using CrocoManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.ViewModel
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty] string? email;
        [ObservableProperty] string? password;

        private readonly IAuthService _authService;
        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        async Task LoginUserAsync()
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Error", "Enter credentials", "OK");
                return;
            }

            var session = await _authService.LoginAsync(email, Password);

            if(session != null && session.User.UserMetadata.Role == Models.UserRole.Admin)
            {
                // Navigate to admin page
                await Shell.Current.GoToAsync("AdminPage");
            }
            else if(session != null && session.User.UserMetadata.Role != Models.UserRole.NotAssigned)
            {
                // Navigate to user page
                await Shell.Current.GoToAsync("HomePage");
            }
            else
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Error", "Invalid credentials", "OK");
                // Navigate to user page
                //await Shell.Current.GoToAsync("UserPage");
            }

        }

        [RelayCommand]
        async Task TestConnectionAsync()
        {
            //bool ok = await SupabaseService.Instance.TestConnectionAsync();
            
            var ok = await _authService.TestConnectionAsync();
            await Application.Current.Windows[0].Page.DisplayAlert("Connection Test", ok ? "Connected" : "Failed", "OK");
        }

        [RelayCommand]
        async Task GetTextMessageAsync()
        {
            var msg = await _authService.GetTextMessageAsync();
            await Application.Current.Windows[0].Page.DisplayAlert(
                "Supabase Response",
                msg ?? "No message found",
                "OK");
        }

        [RelayCommand]
        private async Task GoToRegister()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }
    }
}
