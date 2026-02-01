using CrocoManager.ViewModel;

namespace CrocoManager.Views;

public partial class FeedingPage : ContentPage
{
    private readonly FeedingViewModel _viewModel;
    public FeedingPage(FeedingViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}