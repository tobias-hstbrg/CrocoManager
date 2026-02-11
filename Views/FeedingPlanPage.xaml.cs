using CrocoManager.Models;
using CrocoManager.ViewModel;

namespace CrocoManager.Views;

public partial class FeedingPlanPage : ContentPage
{
    private readonly FeedingPlanViewModel _viewModel;
    public FeedingPlanPage(FeedingPlanViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.InitializeAsync();

        if(_viewModel.LoadPlansCommand.CanExecute(null))
        {
            await _viewModel.LoadPlansCommand.ExecuteAsync(null);
        }
    }

    private void OnContentScrolled(object sender, ScrolledEventArgs e)
    {
        headerScroll.ScrollToAsync(e.ScrollX, 0, false);
        actionsScroll.ScrollToAsync(0, e.ScrollY, false);
    }

    private void OnHeaderScrolled(object sender, ScrolledEventArgs e)
    {
        contentScroll.ScrollToAsync(e.ScrollX, contentScroll.ScrollY, false);
    }

    private void OnActionsScrolled(object sender, ScrolledEventArgs e)
    {
        contentScroll.ScrollToAsync(contentScroll.ScrollX, e.ScrollY, false);
    }
}