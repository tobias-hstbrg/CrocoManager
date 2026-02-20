using CrocoManager.Core.ViewModels;

namespace CrocoManager.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel viewmodel)
	{
		InitializeComponent();
		BindingContext = viewmodel;
    }
}
