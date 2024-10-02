using TeaStoreApp.Services;

namespace TeaStoreApp.Pages;

public partial class ProfilePage : ContentPage
{
	private byte[] imageArray;  // array di byte in cui verrà convertito memorystream sotto
	public ProfilePage()
	{
		InitializeComponent();
		LblUserName.Text = Preferences.Get("username", string.Empty);
	}

    private async void ImgUserProfileBtn_Clicked(object sender, EventArgs e)
    {
		// classe per prendere foto e video dal device, cliccando su immagine profilo la aggiungo
		var file = await MediaPicker.PickPhotoAsync();
		if(file != null)
		{
			using (var stream = await file.OpenReadAsync())
			{
				// using garantisce che istanza venga eliminata correttamente
				// altro stream di memoria con using
				using (var memoryStream = new MemoryStream())
				{
					await stream.CopyToAsync(memoryStream);
					imageArray = memoryStream.ToArray();  // dati binari grezzi (raw data) dell'immagine
					ImgUserProfileBtn.Source = ImageSource.FromStream(() => new MemoryStream(imageArray));
					// nuovo memorystream da imagearray, mostra immagine profilo
				}
			}
		}

		// come caricare immagine di utente
		var response = await ApiService.UploadUserImage(imageArray);
		if(response)
		{
            await DisplayAlert("", "Image Uploaded Successfully", "Alright");
        }
		else
		{
            await DisplayAlert("", "Error occurred", "Cancel");
        }
    }

    // c'è problema immagine caricata non rimane se spengo e riaccendo app -> faccio override. Occhio a essere in scope giusto!
    protected override async void OnAppearing()
    {
		base.OnAppearing();
		var response = await ApiService.GetUserProfileImage();
		if (response.ImageUrl != null)
		{
			ImgUserProfileBtn.Source = response.FullImageUrl;  // se c'è immagine la prende
		}
    }

    private void TapOrders_Tapped(object sender, TappedEventArgs e)
    {
		// mando a pagina ordini passati
		Navigation.PushAsync(new OrdersPage());
    }

    private void BtnLogout_Clicked(object sender, EventArgs e)
    {
        // ripulisco prima access token (perché faccio logout)
        Preferences.Set("accesstoken", string.Empty);
		Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}