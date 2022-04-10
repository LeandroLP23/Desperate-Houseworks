using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Desperate_Houseworks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /* Pagina di registrazione dell'utente.*/
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        /* L'utente ha premuto il bottone per effettuare la registrazione*/
        private async void RegisterButton_Clicked(object sender, EventArgs e)
        {
            int statusCode;

            /* Verifica correttezza password inserita*/
            if (Username.Text == null || Password.Text == null ||
                PasswordRepeat.Text == null || Email.Text == null)
            {
                statusCode = 415;
            }
            else if (!MailVerificata(Email.Text))
            {
                statusCode = 413;
            }
            else if (!Password.Text.Equals(PasswordRepeat.Text))
            {
                statusCode = 414;
            }
            else //password inserita corretta, inviamo la richesta di registrazione
            {
                ActivityIndicator.IsRunning = true;
                ActivityIndicator.IsVisible = true;
                RegisterButton.IsVisible = false;
                statusCode = await DesperateDB.SignUp(Username.Text, Password.Text, Email.Text);
                ActivityIndicator.IsRunning = false;
                ActivityIndicator.IsVisible = false;
                RegisterButton.IsVisible = true;
            }

            // Registrazione effettuata con successo
            if (statusCode == 200)
            {
                await DisplayAlert("", "Registrazione avvenuta con successo!" +
                    " Verifica la tua e-mail per continuare.", "OK");
                return;
            }

            // Errore con registrazione
            ClassGlobal.Shaking_Animation(RegisterButton);

            switch (statusCode)
            {
                case 411:
                    await DisplayAlert("", "L'Username utilizzato è già presente.", "Ok"); ;
                    return;
                case 412:
                    await DisplayAlert("", "Indirizzo e-mail già esistente.", "Ok");
                    return;
                case 413:
                    await DisplayAlert("", "Indirizzo e-mail non valido.", "Ok");
                    return;
                case 414:
                    await DisplayAlert("", "Password non corrispondenti.", "Ok");
                    return;
                case 415:
                    await DisplayAlert("", "Il campo non puo' essere vuoto!", "Ok");
                    return;
                default:
                    await DisplayAlert("", "Errore generico. " +
                        "Contatta il supporto per maggiori informazioni.", "Ok");
                    return;
            }
        }

        /* Apertura pagina del login. */
        private async void NavigationLoginButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//first/FirstPage/login");
        }

        /* Verifica che le due password coincidano.*/
        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Se entrambe le password sono vuote la scritta è invisibile
            if (Password.Text.Equals(string.Empty) && PasswordRepeat.Text.Equals(string.Empty))
            {
                LabelPasswords.IsVisible = false;
            }
            else
            {
                LabelPasswords.IsVisible = !Password.Text.Equals(PasswordRepeat.Text);
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