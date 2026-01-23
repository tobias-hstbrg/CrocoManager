using CrocoManager.Views;

namespace CrocoManager
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // LoginPage as ShellContent WITHOUT a custom route
            //Items.Add(new ShellContent
            //{
            //    ContentTemplate = new DataTemplate(() => MauiProgram.ServiceProvider.GetRequiredService<LoginPage>())
            //});


            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(AdminPage), typeof(AdminPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(ResetPasswordPage), typeof(ResetPasswordPage));
        }
    }
}