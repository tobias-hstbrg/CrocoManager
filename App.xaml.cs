using CrocoManager.Services;
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
            Window window = new Window(new AppShell());
#if WINDOWS
            window.MinimumWidth = 400;
            window.MinimumHeight = 1000;
#endif
            return window;
        }
    }
}