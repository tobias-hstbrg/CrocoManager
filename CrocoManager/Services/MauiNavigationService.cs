using CrocoManager.Core.Interfaces;
using CrocoManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Services
{
    public class MauiNavigationService : INavigationService
    {
        private readonly IServiceProvider _services;

        public MauiNavigationService(IServiceProvider services) => _services = services;

        public Task GoToAsync(string route) => Shell.Current.GoToAsync(route);

        public void SetRoot(string pageKey)
        {
            if (Application.Current?.Windows.FirstOrDefault() is Window window)
            {
                Type? pageType = pageKey switch
                {
                    "Login" => typeof(LoginPage),
                    "Admin" => typeof(AdminPage),
                    "AppShell" => typeof(AppShell),
                    "Register" => typeof(RegisterPage),
                    "ResetPassword" => typeof(ResetPasswordPage),
                    _ => null
                };

                if (pageType != null)
                {
                    window.Page = (Page)_services.GetRequiredService(pageType);
                }
            }
        }
    }
}
