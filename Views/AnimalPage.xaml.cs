using CrocoManager.Models;
using CrocoManager.ViewModel;

namespace CrocoManager.Views;

public partial class AnimalPage : ContentPage
{
    private bool isSyncing;

    public AnimalPage(AnimalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AnimalViewModel vm &&
            vm.LoadAnimalsCommand.CanExecute(null))
        {
            vm.LoadAnimalsCommand.Execute(null);
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