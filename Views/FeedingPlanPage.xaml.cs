using CrocoManager.Models;
using CrocoManager.ViewModel;

namespace CrocoManager.Views;

public partial class FeedingPlanPage : ContentPage
{
    private bool isSyncing;

    public FeedingPlanPage(FeedingPlanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is FeedingPlanViewModel vm &&
            vm.LoadPlansCommand?.CanExecute(null) == true)
        {
            vm.LoadPlansCommand.Execute(null);
        }
    }

    private async void OnAnyScrolled(object sender, ScrolledEventArgs e)
    {
        if (isSyncing)
            return;

        isSyncing = true;

        try
        {
            if (sender == headerScroll)
            {
                await contentScroll.ScrollToAsync(e.ScrollX, contentScroll.ScrollY, false);
            }
            else if (sender == contentScroll)
            {
                await headerScroll.ScrollToAsync(e.ScrollX, 0, false);
                await actionsScroll.ScrollToAsync(0, e.ScrollY, false);
            }
            else if (sender == actionsScroll)
            {
                await contentScroll.ScrollToAsync(contentScroll.ScrollX, e.ScrollY, false);
            }
        }
        finally
        {
            isSyncing = false;
        }
    }
}