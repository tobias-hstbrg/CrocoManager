using CrocoManager.Core.ViewModels;

namespace CrocoManager.Views;

public partial class AdminPage : ContentPage
{
    public AdminPage(AdminViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
