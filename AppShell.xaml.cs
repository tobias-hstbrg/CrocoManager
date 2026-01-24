using CrocoManager.Interfaces;
using CrocoManager.Views;

namespace CrocoManager
{
    public partial class AppShell : Shell
    {
        private readonly IAuthService _authService;

        public AppShell(IAuthService authService, string? startRoute = null)
        {
            InitializeComponent();
            _authService = authService;

            SetAdaptiveFlyout();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(AdminPage), typeof(AdminPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(ResetPasswordPage), typeof(ResetPasswordPage));

            if (!string.IsNullOrEmpty(startRoute))
            {
                Dispatcher.Dispatch(async () =>
                {
                    await GoToAsync(startRoute);
                });
            }
        }

        private void SetAdaptiveFlyout()
        {
            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            var width = displayInfo.Width / displayInfo.Density;

            if (width >= 1024)
            {
                FlyoutBehavior = FlyoutBehavior.Locked;
                FlyoutWidth = 350;
            }
            else if (width >= 768)
            {
                FlyoutBehavior = FlyoutBehavior.Flyout;
                FlyoutWidth = 320;
            }
            else
            {
                FlyoutBehavior = FlyoutBehavior.Disabled;
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Abmelden",
                "Möchten Sie sich wirklich abmelden?",
                "Ja",
                "Nein"
            );

            if (confirm)
            {
                await _authService.SignOutAsync();

                var services = Application.Current!.Handler.MauiContext!.Services;
                var loginPage = services.GetRequiredService<LoginPage>();

                Application.Current.MainPage = new NavigationPage(loginPage);
            }
        }
    }
}