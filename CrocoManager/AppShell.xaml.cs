using CrocoManager.Core.Interfaces;
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

            //Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            //Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(AdminPage), typeof(AdminPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            //Routing.RegisterRoute(nameof(ResetPasswordPage), typeof(ResetPasswordPage));

            if (!string.IsNullOrEmpty(startRoute))
            {
                Dispatcher.Dispatch(async () =>
                {
                    await GoToAsync(startRoute);
                });
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            if(Expand.IsVisible)
            {
                var animation = new Animation((current) =>
                {
                    FlyoutWidth = current;

                }, 65, 250, null);

                animation.Commit(this, "expand", finished: (value, cancelled) =>
                {
                    Expand.IsVisible = false;
                    Minimize.IsVisible = true;
                });
            }
            else
            {
                var animation = new Animation((current) =>
                {
                    FlyoutWidth = current;

                }, 250, 65, null);

                animation.Commit(this, "minimize", finished: (value, cancelled) =>
                {
                    Expand.IsVisible = true;
                    Minimize.IsVisible = false;
                });
            }
        }
    }
}