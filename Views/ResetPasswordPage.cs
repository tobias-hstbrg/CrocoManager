using CrocoManager.ViewModel;

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
    

