using CrocoManager.ViewModel;

namespace CrocoManager.Views;

public partial class AdminPage : ContentPage
{
    public AdminPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}