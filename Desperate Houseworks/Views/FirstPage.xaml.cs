using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Desperate_Houseworks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FirstPage : ContentPage
    {

        public string Presentazione = "Organizza i tuoi compiti e quelli della tua famiglia.\n" +
            "Fallo con una singola applicazione che memorizza e riorganizza le faccende domestiche, " +
            "costantemente a portata di click.";

        public FirstPage()
        {
            InitializeComponent();
            LabelPresentazione.Text = Presentazione;
        }

        protected override async void OnAppearing()
        {
            if (!ClassGlobal.ActualUser.Username.Equals(string.Empty))
            {
                ClassGlobal.isLogged = false;
                await Shell.Current.GoToAsync("//main/MainTaskPage");
                VerifyTokenValidity(ClassGlobal.ActualUser);
            }
            base.OnAppearing();
        }

        /* Verifica validità token ingresso automatico nell'applicazione.*/
        private async void VerifyTokenValidity(User u)
        {
            while (App.notificationToken.Equals(""))
            {
                await Task.Delay(200);
            }

            if (await DesperateDB.SignIn(u.Token) == 200)
            {
                ClassGlobal.isLogged = true;

                Page pagina = Shell.Current.CurrentPage;

                //Aggiornamento della pagina aperta dall'utente
                if (pagina.GetType().Equals(typeof(MainTaskPage)))
                {
                    ((MainTaskPage)pagina).OnLogin();
                }
                else if (pagina.GetType().Equals(typeof(ProfilePage)))
                {
                    ((ProfilePage)pagina).OnLogin();
                }
                else if (pagina.GetType().Equals(typeof(NewTaskPage)))
                {
                    ((NewTaskPage)pagina).OnLogin();
                }
            }
            else
            {
                bool answer = await DisplayAlert(
                    "Attenzione!", "A causa di un'errore la tua applicazione è stata avviata in modalità offline.\n" +
                    "", "Ritenta il Login", "Continua in modalità offline");
                if (answer)
                {
                    await Shell.Current.GoToAsync("//first/FirstPage");
                }
            }
        }

        /*Apertura pagina del login*/
        public async void LogIn(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//first/FirstPage/login");
        }

        /*Apertura pagina del signin*/
        public async void SignIn(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//first/FirstPage/registration");
        }

    }
}