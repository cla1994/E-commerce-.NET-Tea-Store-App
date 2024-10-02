using TeaStoreApp.Models;
using TeaStoreApp.Services;

namespace TeaStoreApp.Pages;

public partial class ProductListPage : ContentPage
{
	public ProductListPage(int categoryId)
	{
		// costruttore sopra ha un parametro per la cvcategories_selectionchanged in home page
		InitializeComponent();

		GetProducts(categoryId);
	}

    private async void GetProducts(int categoryId)
    {
		var products = await ApiService.GetProducts("category", categoryId.ToString());

		CvProducts.ItemsSource = products;	
    }

    private void CvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
		var currentSelection =  e.CurrentSelection.FirstOrDefault() as Product;   // primo elemento selezionato dalla collezione
		if (currentSelection == null) return;
		Navigation.PushAsync(new ProductDetailPage(currentSelection.Id));   
		// passo il prodotto con la push async. Serve l'id del prodotto. Modifico costruttore in productdetail per mettere id
		((CollectionView)sender).SelectedItem = null;   
		// = null così utente può tornare a pag iniziale e selezionare stesso oggetto di nuovo
    }
}