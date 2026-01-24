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
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        public HomeViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task SignOut()
        {
            try
            {
                bool succesfull = await _authService.SignOutAsync();
                if (succesfull)
                {
                    var loginPage = App.Current?.Handler.MauiContext!.Services.GetRequiredService<LoginPage>();
                    if (Application.Current?.Windows?.FirstOrDefault() is Window window)
                    {
                        window.Page = loginPage;
                    }
                }
                else
                {
                    if (Application.Current?.Windows?.FirstOrDefault()?.Page is Page page)
                    {
                        await page.DisplayAlert("Fehler", "Abmeldung fehlgeschlagen. Bitte versuche es erneut.", "OK");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sign out failed: {ex.Message}");
            }
        }
    }
}
