using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        protected INotificationService NotificationService { get; }
        protected IAuthService AuthService { get; }
        protected INavigationService NavigationService { get; }
        protected IConnectivityService ConnectivityService { get; }

        [ObservableProperty]
        private bool isBusy;

        public UserRole CurrentUserRole { get; protected set; }

        // Permission Properties
        [ObservableProperty]
        private bool canEdit;

        [ObservableProperty]
        private bool canCreate;

        [ObservableProperty]
        private bool canDelete;

        [ObservableProperty]
        private bool isReadOnly;

        [ObservableProperty]
        private bool canViewItem;

        protected BaseViewModel(
            INavigationService navigationService, 
            INotificationService notificationService, 
            IAuthService authService,
            IConnectivityService connectivityService)
        {
            NavigationService = navigationService;
            NotificationService = notificationService;
            AuthService = authService;
            ConnectivityService = connectivityService;
        }

        /// <summary>
        /// Initializes the viewmodel andf loads the UserRole
        /// </summary>
        public async Task InitializeAsync()
        {
            if (!ConnectivityService.IsConnected)
            {
                await DisplayError("Keine Verbindung", new Exception("Es besteht keine Internetverbindung."));
                return;
            }

            IsBusy = true;
            try
            {
                CurrentUserRole = await AuthService.GetUserRoleAsync();
                SetPermissions();
            }
            catch (Exception ex)
            {
                await DisplayError("Initialisierungsfehler", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private static bool _isShowingNetworkError = false;
        private static readonly object _errorLock = new object();

        protected async Task DisplayError(string title, Exception ex)
        {
            var isNetwork = IsNetworkError(ex) || !ConnectivityService.IsConnected;
            
            var message = isNetwork 
                ? "Keine Internetverbindung. Bitte prüfen Sie Ihre Netzwerkverbindung." 
                : ex.Message;

            if (isNetwork)
            {
                // Double-check lock pattern for the flag
                lock (_errorLock)
                {
                    if (_isShowingNetworkError) return;
                    _isShowingNetworkError = true;
                }

                try
                {
                    await NotificationService.ShowErrorAsync("Verbindungsproblem", message);
                }
                finally
                {
                    // Give the user 2 seconds to breathe before showing the next network error
                    // and allow time for the alert to actually disappear
                    await Task.Delay(2000);
                    _isShowingNetworkError = false;
                }
            }
            else
            {
                await NotificationService.ShowErrorAsync(title, message);
            }
        }

        private bool IsNetworkError(Exception? ex)
        {
            if (ex == null) return false;

            if (ex is HttpRequestException ||
                ex is System.Net.Sockets.SocketException ||
                ex.Message.Contains("Host is not reachable") ||
                ex.Message.Contains("Der gegebene Host ist nicht erreichbar") ||
                ex.Message.Contains("Failed to connect") ||
                ex.Message.Contains("Name or service not known"))
            {
                return true;
            }

            if (ex is AggregateException agg)
            {
                return agg.InnerExceptions.Any(IsNetworkError);
            }

            return IsNetworkError(ex.InnerException);
        }

        /// <summary>
        /// ViewModel has to implement this to set permissions based on what a specific UserRole is allowed to do on the Page of the ViewModel
        /// </summary>
        protected virtual void SetPermissions()
        {

        }


        [RelayCommand]
        protected virtual async Task SignOutAsync()
        {
            try
            {
                bool confirm = await NotificationService.ShowConfirmationAsync("Abmelden", "Möchten Sie sich wirklich abmelden?", "Ja", "Nein");

                if (!confirm) return;

                bool successful = await AuthService.SignOutAsync();

                if(successful)
                {
                    NavigationService.SetRoot("Login");
                }
                else
                {
                    await NotificationService.ShowErrorAsync("Fehler", "Abmeldung fehlgeschlagen. Bitte versuchen Sie es erneut.");
                }
            }
            catch (Exception ex)
            {
                await NotificationService.ShowErrorAsync("Fehler", $"Ein fehler ist aufgetreten: {ex.Message}");
            }
        }
    }
}
