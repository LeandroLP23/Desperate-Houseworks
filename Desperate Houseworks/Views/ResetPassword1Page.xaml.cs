using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Desperate_Houseworks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /* Prima pagina del recupero della password, dove viene introdotta l'email.*/
    public partial class ResetPassword1Page : ContentPage
    {
        public static string UserEmail { get; private set; }
        public ResetPassword1Page()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            /* Soluzione rudimentale, il focus non può essere fatto 
             * direttamente sull'onappearing in quanto l'entry non è 
             * stata ancora renderizzata. */
            await Task.Run(async () =>
            {
                await Task.Delay(100);
                Device.BeginInvokeOnMainThread(() =>
                {
                    EntryEmail.Focus();
                });
            });
        }

        /* Invio richiesta cambio password.*/
        private async void RequestButton_Clicked(object sender, EventArgs e)
        {
            //Salvataggio statico dell'email
            UserEmail = EntryEmail.Text;

            //Verifica correttezza email
            if (UserEmail.Equals(string.Empty))
            {
                //Email vuota
                await DisplayAlert("Errore!", "Introduci un indirizzo email.", "Ok");
                ClassGlobal.Shaking_Animation(EntryEmail);
                return;
            }
            else if (!MailVerificata(UserEmail))
            {
                //Email non valida
                await DisplayAlert("Errore!", "Introduci un indirizzo email valido.", "Ok");
                ClassGlobal.Shaking_Animation(EntryEmail);
                return;
            }

            //Gestione ActivityIndicator
            ActivityIndicator.IsRunning = true;

            var code = await DesperateDB.ResetPasswordEmailSender(UserEmail);

            //Gestion ActivityIndicator
            ActivityIndicator.IsRunning = false;

            //Chiamata al DBonline
            if (code == 200)
            {
                //Pulizia casella di testo
                EntryEmail.Text = string.Empty;

                //Email corretta passiamo, deve inserire il codice
                await Shell.Current.GoToAsync("//first/login/password2");
            }
            else
            {
                //Chiamata non effettuata o email non corretta
                await DisplayAlert("Errore", "Qualcosa è andato storto, verificare la tua connessione " +
                    "ad internet e la correttezza dell'email inserita.", "Ok");
            }
        }

        /* Metodo verifica correttezza email introdotte.*/
        private bool MailVerificata(string email)
        {
            Regex regex = new Regex(@"^([\w.-]+)@([\w-]+)((.(\w){2,3})+)$");
            Match match = regex.Match(email);
            return match.Success;
        }
    }
}