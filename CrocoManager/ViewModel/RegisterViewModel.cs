using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
using CrocoManager.Core.Services;
using CrocoManager.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.ViewModel
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

        [RelayCommand]
        private void GoToLogin()
        {
            NavigationService.SetRoot(typeof(LoginPage));
        }
    }
}
