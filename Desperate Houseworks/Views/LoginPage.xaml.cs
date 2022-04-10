using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Desperate_Houseworks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /* Pagina del Login dell'utene.*/
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        /* Richiesta di login */
        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            if (EntryUsername.Text.Equals(string.Empty))
            {
                ClassGlobal.Shaking_Animation((Button)sender);
                await DisplayAlert("Errore", "Inserisci un username!", "Ok");
                return;
            }

            if (EntryPassword.Text.Equals(string.Empty))
            {
                ClassGlobal.Shaking_Animation((Button)sender);
                await DisplayAlert("Errore", "Inserisci una password", "Ok");
                return;
            }

            //Gestione activityIndicator e bottone
            LoginButton.IsVisible = false;
            ActivityIndicator.IsRunning = true;
            ActivityIndicator.IsVisible = true;

            int statusCode = await DesperateDB.SignIn(EntryUsername.Text, EntryPassword.Text);

            //Gestione activityIndicator e bottone
            ActivityIndicator.IsRunning = false;
            ActivityIndicator.IsVisible = false;
            LoginButton.IsVisible = true;

            ClassGlobal.isLogged = false;

            switch (statusCode)
            {
                case 200:
                    ClassGlobal.isLogged = true;
                    await Shell.Current.GoToAsync("//main/MainTaskPage");
                    TextCleanUp();
                    return;

                case 399:
                    await DisplayAlert("Errore", "Problemi di connessione, sei offline. ", "Riprova");
                    return;

                case 400:
                    await DisplayAlert("Errore", "Hai inserito una username o una password errati.", "Riprova");
                    return;

                case 402:
                    await DisplayAlert("Errore", "Hai inserito una username o una password errati.", "Riprova");
                    TextCleanUp();
                    return;

                case 401:
                    await DisplayAlert("Errore", "Il tuo account non è stato ancora verificato," +
                        " riprova ad accedere dopo aver verificato il tuo account.", "Riprova");
                    TextCleanUp();
                    return;

                default:
                    await DisplayAlert("Errore", "Errore imprevisto, riprova più tardi.", "Ok");
                    TextCleanUp();
                    return;
            }
        }


        /* Pulizia testo delle Username e Password entry. */
        private void TextCleanUp()
        {
            EntryUsername.Text = "";
            EntryPassword.Text = "";
        }

        /* Invio automatico alla pressione del tasto invio. */
        private void OnEnterClicked(object sender, EventArgs e)
        {
            LoginButton_Clicked(sender, e);
        }

        /* Apertura pagina per registrarsi. */
        private async void NavigationRegisterButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//first/FirstPage/login/registration");
        }

        /* Apertura pagina per resettare la password. */
        private async void PasswordResetLabel_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//first/FirstPage/login/password1");
        }

    }
}