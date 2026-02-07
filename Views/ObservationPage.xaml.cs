using CrocoManager.ViewModel;

namespace CrocoManager.Views;

public partial class ObservationPage : ContentPage
{
	public ObservationPage(ObservationViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}