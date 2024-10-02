
using System.Collections.ObjectModel;
using TeaStoreApp.Models;
using TeaStoreApp.Services;

namespace TeaStoreApp.Pages;

public partial class CartPage : ContentPage
{
    private ObservableCollection<ShoppingCartItem> ShoppingCartItems = new ObservableCollection<ShoppingCartItem>();
    // collezione observable per poter usare l'helper method remove in funzione di cancellazione oggetto, in basso
	public CartPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        // faccio overriding perché voglio cambiare funzionalità base di apparizione della schermata -> in base a carrello
        base.OnAppearing();
        GetShoppingCartItems();
        // user ha aggiunto indirizzo di spedizione di già? Se sì lo scrivo subito in label
        bool address = Preferences.ContainsKey("address");
        if (address)
        {
            LblAddress.Text = Preferences.Get("address", string.Empty);
        }
        else
        {
            LblAddress.Text = "Provide your address";
        }
    }

    private async void GetShoppingCartItems()
    {
        ShoppingCartItems.Clear();  // così elementi restano eliminati (sennò quelli cancellati li rimette in lista)

        var shoppingcartitems = await ApiService.GetShoppingCartItems(Preferences.Get("userid", 0));

        foreach (var shoppingCart in shoppingcartitems)
        {
            ShoppingCartItems.Add(shoppingCart);   // attenzione, aggiungi alla collezione observable!
        }

        CvCart.ItemsSource = ShoppingCartItems;
        UpdateTotalPrice(); // vedi sotto
    }

    private void UpdateTotalPrice()
    {
        // questo metodo si chiama quando si preme pulsante x aggiungere togliere o cancellare, e in getshoppingcartitems
        // x calcolare e aggiornare prezzo totale 
        var totalPrice = ShoppingCartItems.Sum(item => item.Price * item.Qty);  
        // lambda expression x far prima, somma tutti i (prezzi * quantità)
        LblTotalPrice.Text = totalPrice.ToString();
    }

    // creo funzione usata sia da btndecrease che btnincrease, per risparmiare righe di codice
    private async void UpdateCartQuantity(int productId, string action)
    {
        var response = await ApiService.UpdateCartQuantity(productId, action);
        if (response)
        {
            return;
        }
        else
        {
            await DisplayAlert("Oops", "Something went wrong", "Cancel");
        }
    }

    private async void BtnDecrease_Clicked(object sender, EventArgs e)
    {
        // vedi sotto, in più c'è controllo quantità non vada sotto 1
        if (sender is Button button && button.BindingContext is ShoppingCartItem cartItem)
        {
            if (cartItem.Qty == 1) return;
            else if (cartItem.Qty > 1)
            {
                cartItem.Qty--;
                UpdateTotalPrice();
                UpdateCartQuantity(cartItem.ProductId, "decrease");  // passa l'action decrease all'api
            }
        }
    }

    private async void BtnIncrease_Clicked(object sender, EventArgs e)
    {
        // if controlla se è stato un pulsante a generare evento, e se pulsante è collegato a shoppingcartitem
        if (sender is Button button && button.BindingContext is ShoppingCartItem cartItem)
        {
            cartItem.Qty++;
            UpdateTotalPrice();
            UpdateCartQuantity(cartItem.ProductId, "increase");
        }
    }

    private void BtnDelete_Clicked(object sender, EventArgs e)
    {
        // imagebutton perché è pulsante con immagine (cestino x cancellare)
        if (sender is ImageButton button && button.BindingContext is ShoppingCartItem cartItem)
        {
            ShoppingCartItems.Remove(cartItem);  // rimuove l'item da front end
            UpdateTotalPrice();
            UpdateCartQuantity(cartItem.ProductId, "delete");  // da back end (server)
        }
    }

    private void BtnEditAddress_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AddressPage());
    }

    private async void TapPlaceOrder_Tapped(object sender, TappedEventArgs e)
    {
        var order = new Order()
        {
            Address = LblAddress.Text,
            UserId = Preferences.Get("userid", 0),
            OrderTotal = Convert.ToInt32(LblTotalPrice.Text)
        };

        var response = await ApiService.PlaceOrder(order);
        if (response)
        {
            await DisplayAlert("", "Your order has been placed", "Alright");
            ShoppingCartItems.Clear();  // ripulisco l'ordine che ha avuto successo
        }
        else
        {
            await DisplayAlert("Oops", "Something went wrong", "Cancel");
        }
    }
}