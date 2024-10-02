using TeaStoreApp.Services;

namespace TeaStoreApp.Pages;

public partial class SignupPage : ContentPage
{
	public SignupPage()
	{
		InitializeComponent();
	}

    private async void BtnSignup_Clicked(object sender, EventArgs e)
    {
        // con click chiamo nostro metodo registeruser. Devo prendere cosa utente scrive
        // e passare i 4 valori testuali come parametri alla registeruser. Vado in xaml per vedere i nomi (le entry)
        var response = await ApiService.RegisterUser(EntName.Text, EntEmail.Text, EntPhone.Text, EntPassword.Text); 
        if (response)
        {
            // se risposta è valida mostra display alert / pop-up, con titolo, messaggio e scritta pulsante da premere
            await DisplayAlert("","Your account has been created","Alright");
            // se autenticazione avvenuta con successo, c'è reindirizzamento a altra pagina creata, login page
            await Navigation.PushAsync(new LoginPage());
        }
        else
        {
            await DisplayAlert("", "Oops something went wrong", "Cancel");
        }
    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
    {
        // fatto collegamento con xaml. Se login già fatto tramite ad es. desktop, rimanda direttamente a login page
        await Navigation.PushAsync(new LoginPage());
    }
}