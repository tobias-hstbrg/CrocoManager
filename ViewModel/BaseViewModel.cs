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
    public abstract partial class BaseViewModel : ObservableObject
    {
        protected readonly INotificationService _notificationService;
        protected readonly IAuthService _authService;
        protected readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private bool isBusy;

        protected BaseViewModel(INotificationService notificationService, IAuthService authService, IServiceProvider serviceProvider)
        {
            _notificationService = notificationService;
            _authService = authService;
            _serviceProvider = serviceProvider;
        }

        [RelayCommand]
        protected virtual async Task SignOutAsync()
        {
            try
            {
                bool confirm = await _notificationService.ShowConfirmationAsync("Abmelden", "Möchten Sie sich wirklich abmelden?", "Ja", "Nein");

                if (!confirm) return;

                bool successful = await _authService.SignOutAsync();

                if(successful)
                {
                    var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
                    if(Application.Current?.Windows.FirstOrDefault() is Window window)
                    {
                        window.Page = loginPage;
                    }
                }
                else
                {
                    await _notificationService.ShowErrorAsync("Fehler", "Abmeldung fehlgeschlagen. Bitte versuchen Sie es erneut.");
                }
            }
            catch (Exception ex)
            {
                await _notificationService.ShowErrorAsync("Fehler", $"Ein fehler ist aufgetreten: {ex.Message}");
            }
        }
    }
}
