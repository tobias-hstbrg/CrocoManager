using CrocoManager.Interfaces;
using CrocoManager.Services;
using CrocoManager.ViewModel;
using CrocoManager.Views;

namespace CrocoManager
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        public App(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
           
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var loginPage = Handler.MauiContext!.Services.GetRequiredService<LoginPage>();

            var window = new Window(loginPage);

#if WINDOWS
            window.MinimumWidth = 400;
            window.MinimumHeight = 1000;
#endif
            return window;
        }
    }
}