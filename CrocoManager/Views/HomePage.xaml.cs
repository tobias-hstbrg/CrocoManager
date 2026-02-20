using CrocoManager.Core.ViewModels;

namespace CrocoManager.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;
    public HomePage(HomeViewModel viewmodel)
	{
		InitializeComponent();
		BindingContext = viewmodel;
    }
}
