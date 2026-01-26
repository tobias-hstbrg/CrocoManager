using CrocoManager.Models;
using CrocoManager.ViewModel;

namespace CrocoManager.Views;

public partial class AnimalPage : ContentPage
{
    private bool isScrolling = false;
    public List<Animal> Animals { get; set; }

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

                // Move the actions stack vertically - SOFORT ohne Animation
                actionsStack.TranslationY = -e.ScrollY;

                // Debug output
                System.Diagnostics.Debug.WriteLine($"Scroll Y: {e.ScrollY}, Translation: {actionsStack.TranslationY}");
            }
        }
        finally
        {
            isScrolling = false;
        }
    }
}