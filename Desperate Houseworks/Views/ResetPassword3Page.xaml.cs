using Desperate_Houseworks.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Desperate_Houseworks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /* Terza pagina recupero password, dove va sostituita la password.*/
    public partial class ResetPassword3Page : ContentPage
    {
        public ResetPassword3Page()
        {
            InitializeComponent();
        }

        /* Verifica che le due password siano uguali.*/
        private bool CheckPasswords()
        {
            return EntryPass1.Text.Equals(EntryPass2.Text);
        }

        /* Quando il testo delle entry delle password viene cambiato, 
         * verifica se sia necessario abilitare il bottone.*/
        private void EntryPass_TextChanged(object sender, TextChangedEventArgs e)
        {
            NewPasswordButton.IsEnabled = CheckPasswords();
        }

        /* Invia richiesta nuova password.*/
        private async void NewPasswordButton_Clicked(object sender, EventArgs e)
        {
            if (CheckPasswords())
            {
                //Gestione ActivityIndicator
                ActivityIndicator.IsRunning = true;

                var code = await DesperateDB.ResetPasswordOfficial(ResetPassword1Page.UserEmail, EntryPass2.Text);

                //Gestione ActivityIndicator
                ActivityIndicator.IsRunning = false;

                //Richiesta al DBonline
                if (code == 200)
                {
                    await DisplayAlert("Successo!", "Sostituzione della password effettuata con successo!\n" +
                        "Verrai reindirizzato alla pagina del login.", "Ok");

                    //Pulizia caselle di testo
                    EntryPass1.Text = string.Empty;
                    EntryPass2.Text = string.Empty;

                    await Shell.Current.GoToAsync("//first/login");
                }
                else
                {
                    //Errore generico
                    await DisplayAlert("Errore", "Qualcosa è andato storto, riprova.", "Ok");
                }
            }
        }
    }
}