using CrocoManager.Core.ViewModels;

namespace CrocoManager.Views
{

    public partial class ResetPasswordPage : ContentPage
    {
        public ResetPasswordPage(PasswordResetViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
    

