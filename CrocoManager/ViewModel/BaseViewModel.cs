using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
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
        protected INotificationService NotificationService { get; }
        protected IAuthService AuthService { get; }
        protected INavigationService NavigationService { get; }

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

        protected BaseViewModel(INavigationService navigationService, INotificationService notificationService, IAuthService authService)
        {
            NavigationService = navigationService;
            NotificationService = notificationService;
            AuthService = authService;
        }

        /// <summary>
        /// Initializes the viewmodel andf loads the UserRole
        /// </summary>
        public async Task InitializeAsync()
        {
            IsBusy = true;
            try
            {
                CurrentUserRole = await AuthService.GetUserRoleAsync();
                SetPermissions();
            }
            finally
            {
                IsBusy = false;
            }
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
                    NavigationService.SetRoot(typeof(LoginPage));
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
