using CrocoManager.Views;

namespace CrocoManager
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            Items.Add(new ShellContent
            {
                Title = "Login",
                ContentTemplate = new DataTemplate(() => MauiProgram.ServiceProvider.GetRequiredService<LoginPage>())
            });

            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("AdminPage", typeof(AdminPage));
            Routing.RegisterRoute("HomePage", typeof(HomePage));
        }
    }
}
