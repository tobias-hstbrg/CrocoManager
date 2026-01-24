using CrocoManager.Interfaces;
using CrocoManager.Services;
using CrocoManager.ViewModel;
using CrocoManager.Views;

namespace CrocoManager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
           
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var loginPage = Handler.MauiContext!.Services.GetRequiredService<LoginPage>();
            MainPage = new NavigationPage(loginPage);
            Window window = base.CreateWindow(activationState);
#if WINDOWS
            window.MinimumWidth = 400;
            window.MinimumHeight = 1000;
#endif
            return window;
        }
    }
}