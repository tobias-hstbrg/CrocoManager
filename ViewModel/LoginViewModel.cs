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
        async Task TestConnectionAsync()
        {
            //bool ok = await SupabaseService.Instance.TestConnectionAsync();
            
            bool ok = _authService.TestConnectionAsync().Result;
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

        //[RelayCommand]
        //async Task LoginAsync()
        //{
        //    if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        //    {
        //        await Application.Current.Windows[0].Page.DisplayAlert("Error", "Enter credentials", "OK");
        //        return;
        //    }

        //    var user = await SupabaseService.Instance.LoginAsync(Email, Password);

        //    if (user != null)
        //    {
        //        await Application.Current.Windows[0].Page.DisplayAlert("Success", $"Welcome {user.Email}", "OK");
        //        // Navigate to home page if needed
        //    }
        //    else
        //    {
        //        await Application.Current.Windows[0].Page.DisplayAlert("Error", "Invalid credentials", "OK");
        //    }
        //}
    }
}
