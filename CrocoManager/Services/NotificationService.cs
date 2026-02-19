using CrocoManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Services
{
    class NotificationService : INotificationService
    {
        public async Task ShowSuccessAsync(string title, string message)
        {
            if(Application.Current?.Windows?.FirstOrDefault()?.Page != null)
            {
                await Application.Current.Windows[0].Page.DisplayAlert(title, message, "OK");
            }
        }

        public async Task ShowErrorAsync(string title, string message)
        {
            if (Application.Current?.Windows?.FirstOrDefault()?.Page != null)
            {
                await Application.Current.Windows[0].Page.DisplayAlert(title, message, "OK");
            }
        }

        public async Task ShowWarningAsync(string title, string message)
        {
            if (Application.Current?.Windows?.FirstOrDefault()?.Page != null)
            {
                await Application.Current.Windows[0].Page.DisplayAlert(title, message, "OK");
            }
        }

        public async Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Ja", string cancel = "Nein")
        {
            if (Application.Current?.Windows?.FirstOrDefault()?.Page != null)
            {
                return await Application.Current.Windows[0].Page.DisplayAlert(title, message, accept, cancel);
            }
            return false;
        }

        public async Task ShowInfoAsync(string title, string message)
        {
            if (Application.Current?.Windows?.FirstOrDefault()?.Page != null)
            {
                await Application.Current.Windows[0].Page.DisplayAlert(title, message, "OK");
            }
        }
    }
}
