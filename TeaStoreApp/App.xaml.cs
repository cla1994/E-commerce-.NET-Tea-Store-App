using TeaStoreApp.Pages;

namespace TeaStoreApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var accesstoken = Preferences.Get("accesstoken", string.Empty); // prendo token salvato in apiservice con la set
            if (string.IsNullOrEmpty(accesstoken))
            {
                MainPage = new NavigationPage(new SignupPage());
                // prima era appshell la 1° pagina, messa quella di sign up invece. Serve using sopra. Poi aggiunto navigation
                // ora se ho fatto login in precedenza, quando riavvio app mi tiene loggato
                // e mi manda direttamente alla homepage col mio account
                // se cancello l'app mi rimanda a pagina signup
            }
            else         
            {
                MainPage = new AppShell();
            }
        }
    }
}
