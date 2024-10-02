using TeaStoreApp.Models;
using TeaStoreApp.Services;

namespace TeaStoreApp.Pages;

public partial class OrdersPage : ContentPage
{
	public OrdersPage()
	{
		InitializeComponent();
		// popolare pagina con ordini passati
		GetOrdersList();
	}

    private async void GetOrdersList()
    {
		var orders = await ApiService.GetOrdersByUser(Preferences.Get("userid", 0));
		CvOrders.ItemsSource = orders;  
		// ora binding in pagina xaml, per mostrare contenuti nella schermata
    }

    private void CvOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
		var selectedItem = e.CurrentSelection.FirstOrDefault() as OrderByUser;
		if (selectedItem == null) return;
		Navigation.PushAsync(new OrderDetailPage(selectedItem.Id, selectedItem.OrderTotal));
		((CollectionView)sender).SelectedItem = null;
    }
}