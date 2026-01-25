using CrocoManager.Models;

namespace CrocoManager.Views;

public partial class AnimalPage : ContentPage
{
    private bool isScrolling = false;
    public List<Animal> Animals { get; set; }

    public AnimalPage()
	{
		InitializeComponent();

        Animals = new List<Animal>
        {
            new Animal { Name = "Charlie", Gender = "Männlich", Age = 8, Species = "Amerikanischer Alligator" },
            new Animal { Name = "Rex", Gender = "Männlich", Age = 12, Species = "Spitzkrokodil" },
            new Animal { Name = "Bella", Gender = "Weiblich", Age = 5, Species = "Nilkrokodil" },
            new Animal { Name = "Max", Gender = "Männlich", Age = 15, Species = "Leistenkrokodil" },
            new Animal { Name = "Luna", Gender = "Weiblich", Age = 3, Species = "Sumpfkrokodil" }
        };

        BindingContext = this;
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