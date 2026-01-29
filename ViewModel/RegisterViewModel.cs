using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Interfaces;
using CrocoManager.Models;
using CrocoManager.Services;
using CrocoManager.Views;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty] string? email;
        [ObservableProperty] string? password;
        public RegisterViewModel(IAuthService authService, IServiceProvider serviceProvider)
        {
            _authService = authService;
            _serviceProvider = serviceProvider;
        }

        [RelayCommand]
        async Task RegisterUserAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await ShowAlert("Error", "Enter credentials");
                return;
            }

            var session = await _authService.RegisterAsync(Email, Password);

            if (session != null)
            {
                await ShowAlert("Success", $"Registred {session.User.Email}");
                await Shell.Current.DisplayAlert("Success", $"Registered {session.User.Email}", "OK");
            }
            else
            {
                await ShowAlert("Error", "Registration failed");
            }
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
