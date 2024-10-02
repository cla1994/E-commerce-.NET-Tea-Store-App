
using TeaStoreApp.Models;
using TeaStoreApp.Services;

namespace TeaStoreApp.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
        // nome della label in file xaml (stessi param. di set)
        LblUserName.Text = "Hi " + Preferences.Get("username", string.Empty);
		// mostrare categorie (da api)
		GetCategories();   // questo getcategories è = al metodo qui sotto ma =/= da apiservice.getcategories
		GetTrendingProducts();
		GetBestSellingProducts();
	}

    private async void GetBestSellingProducts()
    {
        var products = await ApiService.GetProducts("bestselling", string.Empty);   // lista di prodotti, ordinati per + venduti
        CvBestSelling.ItemsSource = products; // vedi sotto
    }

    private async void GetTrendingProducts()
    {
        var products = await ApiService.GetProducts("trending", string.Empty);   // lista di prodotti, ordinati per tendenza
		CvTrending.ItemsSource = products; // vedi sotto
    }

    private async void GetCategories()
    {
		var categories = await ApiService.GetCategories();  
		CvCategories.ItemsSource = categories;  // cvccategories è come viene chiamata collection view delle categorie in xaml
	}

    private void CvCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Category;
        if (currentSelection == null) return;
        // se non nulla, user viene indirizzato verso lista prodotti
        Navigation.PushAsync(new ProductListPage(currentSelection.Id));   // id della categoria corrente
        // clear la collection view
        ((CollectionView)sender).SelectedItem = null;
    }

    private void CvBestSelling_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // stesso metodo che in productlistpage per rimandare a dettagli prodotto
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Product;   
        if (currentSelection == null) return;
        Navigation.PushAsync(new ProductDetailPage(currentSelection.Id));
        ((CollectionView)sender).SelectedItem = null;
    }

    private void CvTrending_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // vedi sopra
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Product;
        if (currentSelection == null) return;
        Navigation.PushAsync(new ProductDetailPage(currentSelection.Id));
        ((CollectionView)sender).SelectedItem = null;
    }
}