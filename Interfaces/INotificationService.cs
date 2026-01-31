using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Interfaces
{
    public interface INotificationService
    {
        Task ShowSuccessAsync(string title, string message);
        Task ShowErroryAsync(string title, string message);
        Task ShowWarningAsync(string title, string message);
        Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Ja", string cancel = "Nein");
        Task ShowInfoAsync(string title, string message);
    }
}
