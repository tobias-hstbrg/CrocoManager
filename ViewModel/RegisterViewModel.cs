using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Interfaces;
using CrocoManager.Models;
using CrocoManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.ViewModel
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty] string? email;
        [ObservableProperty] string? password;
        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        async Task RegisterUserAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Error", "Enter credentials", "OK");
                return;
            }

            var session = await _authService.RegisterAsync(Email, Password);

            if (session != null)
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Success", $"Registered {session.User.Email}", "OK");
            }
            else
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Error", "Registration failed", "OK");
            }
        }
    }
}
