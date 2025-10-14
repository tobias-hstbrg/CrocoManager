using CrocoManager.ViewModel;

namespace CrocoManager.Views;

public partial class AdminPage : ContentPage
{
	public AdminPage(AdminViewModel viewmodel)
	{
		InitializeComponent();
		BindingContext = viewmodel;
    }
}