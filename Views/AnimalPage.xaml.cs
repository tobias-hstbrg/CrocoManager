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

        _viewModel.CanEdit = true;
        _viewModel.CanDelete = true;
        _viewModel.CanCreate = true;

        System.Diagnostics.Debug.WriteLine("=== OnAppearing START ===");

        await _viewModel.InitializeAsync();

        // Debug: Ausgabe nach InitializeAsync
        System.Diagnostics.Debug.WriteLine($"CurrentUserRole: {_viewModel.CurrentUserRole}");
        System.Diagnostics.Debug.WriteLine($"CanEdit: {_viewModel.CanEdit}");
        System.Diagnostics.Debug.WriteLine($"CanDelete: {_viewModel.CanDelete}");
        System.Diagnostics.Debug.WriteLine($"CanCreate: {_viewModel.CanCreate}");
        System.Diagnostics.Debug.WriteLine("=== OnAppearing END ===");

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