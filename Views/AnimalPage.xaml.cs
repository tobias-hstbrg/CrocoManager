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

    private async void OnContentScrolled(object sender, ScrolledEventArgs e)
    {
        if (isScrolling) return;

        isScrolling = true;
        await headerScroll.ScrollToAsync(e.ScrollX, 0, false);
        isScrolling = false;
    }

    private void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var animal = button?.BindingContext as Animal;
        // Handle edit
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var animal = button?.BindingContext as Animal;
        // Handle delete
    }
}