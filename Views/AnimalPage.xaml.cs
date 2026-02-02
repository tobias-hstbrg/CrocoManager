using CrocoManager.Models;
using CrocoManager.ViewModel;

namespace CrocoManager.Views;

public partial class AnimalPage : ContentPage
{

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