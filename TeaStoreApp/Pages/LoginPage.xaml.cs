using TeaStoreApp.Services;

namespace TeaStoreApp.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void BtnSignIn_Clicked(object sender, EventArgs e)
    {
        var response = await ApiService.Login(EntEmail.Text, EntPassword.Text);
        if (response)
        {
            // rimanda a 1 delle varie pagine della tab bar, inizialm. home page
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("", "Oops something went wrong", "Cancel");
        }
    }

    private async void TapRegister_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new SignupPage());
    }
}