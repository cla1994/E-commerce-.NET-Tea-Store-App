
using TeaStoreApp.Models;
using TeaStoreApp.Services;

namespace TeaStoreApp.Pages;

public partial class ProductDetailPage : ContentPage
{
    private int productId;  // metto fuori da constructor perché voglio usarla fuori da esso (per addtocart in basso)
    private string imageUrl;  // una delle proprietà per salvare favorito in sqlite
    private BookmarkItemService bookmarkItemService = new BookmarkItemService();   // x operazioni prodotti favoriti (salvo su db sqlite)

    public ProductDetailPage(int productId)
	{
		InitializeComponent();
		GetProductDetail(productId);
        this.productId = productId;  // assegno a productid che è sopra, fuori da costruttore
	}

    private async void GetProductDetail(int productId)
    {
        var product = await ApiService.GetProductDetail(productId);
        // serve immagine, aggiunta proprietà a classe product detail
        LblProductName.Text = product.Name;
        LblProductDescription.Text = product.Detail;
        ImgProduct.Source = product.FullImageUrl;
        LblProductPrice.Text = product.Price.ToString();  // tostring perché è int sennò
        LblTotalPrice.Text = product.Price.ToString();
        // prezzo relativo a quantità (es. 35$ e ne ordino 3 -> 105)
        imageUrl = product.FullImageUrl;
        // questo serve x immagine oggetto bookmarked, usato in basso
    }

    private void BtnAdd_Clicked(object sender, EventArgs e)
    {
        var i = Convert.ToInt32(LblQty.Text);  // contatore per modificare quantità prodotto. Per renderlo numero, toint32
        i++;
        LblQty.Text = i.ToString();
        var totalPrice = i * Convert.ToInt32(LblProductPrice.Text);  // calcolo totale
        LblTotalPrice.Text = totalPrice.ToString();  // update del prezzo mostrato
    }

    private void BtnRemove_Clicked(object sender, EventArgs e)
    {
        var i = Convert.ToInt32(LblQty.Text);  
        i--;  // qui si decrementa, per il resto = a sopra eccetto per condizione non andare nei negativi

        if (i < 1) return;

        LblQty.Text = i.ToString();
        var totalPrice = i * Convert.ToInt32(LblProductPrice.Text); 
        LblTotalPrice.Text = totalPrice.ToString();  
    }

    private async void BtnAddToCart_Clicked(object sender, EventArgs e)
    {
        var shoppingCart = new ShoppingCart()
        {
            Qty = Convert.ToInt32(LblQty.Text),
            Price = Convert.ToInt32(LblProductPrice.Text),
            TotalAmount = Convert.ToInt32(LblTotalPrice.Text),
            ProductId = productId,
            CustomerId = Preferences.Get("userid", 0)
        };   // productid con la p minuscola definito in cima a pagina
             // userid salvato in preferences
        var response = await ApiService.AddItemsInCart(shoppingCart);
        if(response)
        {
            await DisplayAlert("", "Your item has been added to the cart", "Alright");
        }
        else
        {
            await DisplayAlert("Oops", "Something went wrong", "Cancel");
        }
    }

    private void ImgBtnFavorite_Clicked(object sender, EventArgs e)
    {
        // uso metodi sql scritti in precedenza per lavorare su db
        var existingBookmark = bookmarkItemService.Read(productId);
        if (existingBookmark != null)
        {
            // se quel prodotto è già favorito, ripremere su pulsante cuoricino lo rimuove da preferiti
            bookmarkItemService.Delete(existingBookmark);
        }
        else
        {
            // sennò diventa preferito e lo metto in db
            var bookmarkedProduct = new BookmarkedProduct()
            {
                ProductId = productId,
                IsBookmarked = true,
                Detail = LblProductDescription.Text,
                Name = LblProductName.Text,
                Price = Convert.ToInt32(LblProductPrice.Text),
                ImageUrl = imageUrl
            };

            bookmarkItemService.Create(bookmarkedProduct);
        }

        UpdateFavoriteButton();   // in ogni caso faccio update colore del cuore ogni volta che cliccato
    }

    private void UpdateFavoriteButton()
    {
        var existingBookmark = bookmarkItemService.Read(productId);
        if (existingBookmark != null)
        {
            ImgBtnFavorite.Source = "heartfill";  // se già favorito e clicco, cuore da rosso diventa vuoto (non + favorito)
        }
        else
        {
            ImgBtnFavorite.Source = "heart";  // il contrario
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateFavoriteButton();  // fatto override per aggiornare lo stato + recente possibile di cuoricino / favorito
                                 // ogni volta che si switcha su questa pagina
    }
}