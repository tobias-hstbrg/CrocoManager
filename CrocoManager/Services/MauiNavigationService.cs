using CrocoManager.Core.Interfaces;
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

        public void SetRoot(object pageOrShell)
        {
            // Switches between Login (Auth Page) and Pages of the main app
            if(Application.Current?.Windows.FirstOrDefault() is Window window)
            {
                window.Page = pageOrShell is Type t ? (Page)_services.GetRequiredService(t) : (Page)pageOrShell;
            }
        }
    }
}
