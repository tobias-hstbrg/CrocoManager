using CrocoManager.Models;
using CrocoManager.ViewModel;
using System.Threading.Tasks;

namespace CrocoManager.Views;

public partial class AnimalPage : ContentPage
{
    private readonly AnimalViewModel _viewModel;

    public AnimalPage(AnimalViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.InitializeAsync();

        if (_viewModel.LoadAnimalsCommand.CanExecute(null))
        {
            await _viewModel.LoadAnimalsCommand.ExecuteAsync(null);
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