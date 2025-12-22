using CrocoManager.Services;
using CrocoManager.Views;

namespace CrocoManager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}