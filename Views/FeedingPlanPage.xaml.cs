using CrocoManager.Models;
using CrocoManager.ViewModel;

namespace CrocoManager.Views;

public partial class FeedingPlanPage : ContentPage
{
    private bool isScrolling = false;

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

    private void OnAnyScrolled(object sender, ScrolledEventArgs e)
    {
        if (isScrolling) return;
        isScrolling = true;

        try
        {
            var scrollView = sender as ScrollView;
            if (scrollView == headerScroll)
            {
                // Header scrolled horizontal -> sync content horizontal only
                contentScroll.ScrollToAsync(e.ScrollX, contentScroll.ScrollY, false);
            }
            else if (scrollView == contentScroll)
            {
                // Content scrolled -> sync header horizontal AND move actions vertical
                headerScroll.ScrollToAsync(e.ScrollX, 0, false);
                // Move the actions stack vertically
                actionsStack.TranslationY = -e.ScrollY;
            }
        }
        finally
        {
            isScrolling = false;
        }
    }
}