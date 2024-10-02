namespace TeaStoreApp.Pages;

public partial class AddressPage : ContentPage
{
	public AddressPage()
	{
		InitializeComponent();
	}

    private void BtnSave_Clicked(object sender, EventArgs e)
    {
		// salva nome cellu e indirizzo dati per la consegna
		Preferences.Set("address", EntAddress.Text + "," + EntPhone.Text + "," + EntName.Text);
		Navigation.PopAsync();  // redirect a pagina precedente una volta fatto salvataggio info
    }
}