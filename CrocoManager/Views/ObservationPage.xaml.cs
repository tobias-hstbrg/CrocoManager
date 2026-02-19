using CrocoManager.ViewModel;

namespace CrocoManager.Views;

public partial class ObservationPage : ContentPage
{
	private readonly ObservationViewModel _viewModel;
    public ObservationPage(ObservationViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}