using CrocoManager.Core.ViewModels;

namespace CrocoManager.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel viewmodel)
	{
		InitializeComponent();
		BindingContext = viewmodel;
    }
}
