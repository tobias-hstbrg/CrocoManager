using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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


        [RelayCommand]
        async Task TestConnectionAsync()
        {
            bool ok = await SupabaseService.Instance.TestConnectionAsync();
            await Application.Current.Windows[0].Page.DisplayAlert("Connection Test", ok ? "Connected" : "Failed", "OK");
        }

        [RelayCommand]
        async Task GetTextMessageAsync()
        {
            var msg = await SupabaseService.Instance.GetTextMessageAsync();
            await Application.Current.Windows[0].Page.DisplayAlert(
                "Supabase Response",
                msg ?? "No message found",
                "OK");
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
