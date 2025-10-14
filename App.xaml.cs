using CrocoManager.Services;
using CrocoManager.Views;

namespace CrocoManager
{
    public partial class App : Application
    {
        private readonly AppShell _appShell;

        public App(AppShell appShell)
        {
            InitializeComponent();
            _appShell = appShell;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_appShell);
        }

        protected override async void OnStart()
        {
            var clientService = MauiProgram.ServiceProvider.GetRequiredService<SupabaseClientService>();
            await clientService.InitializeAsync();
        }
    }
}