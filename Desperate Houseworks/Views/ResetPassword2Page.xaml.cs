using Desperate_Houseworks.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Desperate_Houseworks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /* Seconda pagina del recupero della password, dove vengono introdotti i codici.*/
    public partial class ResetPassword2Page : ContentPage
    {
        public ResetPassword2Page()
        {
            InitializeComponent();
        }

        /* Click del frame attorne all'entry.*/
        private void Frame_Tapped(object sender, EventArgs e)
        {
            Entry tmp = (Entry)((Frame)sender).Content;
            tmp.Focus();
        }

        /* Testo aggiornato.
         * Sposta il focus sulla entry successive, se presente.
         * Se necessario, abilita il bottone di invio del codice.*/
        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry entry = (Entry)sender;
            ConfirmButton.IsEnabled = ButtonEnable();
            switch (((Entry)sender).ClassId)
            {
                case "1":
                    if (Entry1.Text.Equals(string.Empty))
                    {
                        Entry1.Unfocus();
                    }
                    else
                    {
                        Entry2.Focus();
                    }
                    return;
                case "2":
                    if (Entry2.Text.Equals(string.Empty))
                    {
                        Entry1.Focus();
                    }
                    else
                    {
                        Entry3.Focus();
                    }
                    return;
                case "3":
                    if (Entry3.Text.Equals(string.Empty))
                    {
                        Entry2.Focus();
                    }
                    else
                    {
                        Entry4.Focus();
                    }
                    return;
                case "4":
                    if (Entry4.Text.Equals(string.Empty))
                    {
                        Entry3.Focus();
                    }
                    else
                    {
                        Entry5.Focus();
                    }
                    return;
                case "5":
                    if (Entry5.Text.Equals(string.Empty))
                    {
                        Entry4.Focus();
                    }
                    else
                    {
                        Entry6.Focus();
                    }
                    return;
                case "6":
                    if (Entry6.Text.Equals(string.Empty))
                    {
                        Entry5.Focus();
                    }
                    else
                    {
                        Entry6.Unfocus();
                    }
                    return;
            }
        }

        /* Invio del codice di sicurezza inserito.*/
        private async void ConfirmButton_Clicked(object sender, EventArgs e)
        {
            if (ButtonEnable())
            {
                //Composizione del codice
                string codice = string.Concat(Entry1.Text, Entry2.Text, Entry3.Text, Entry4.Text, Entry5.Text, Entry6.Text);
                
                //Gestione ActivityIndicator
                ActivityIndicator.IsRunning = true;

                var code = await DesperateDB.ResetPasswordTokenVerify(ResetPassword1Page.UserEmail, codice);

                //Gestione ActivityIndicator
                ActivityIndicator.IsRunning = false;
                
                //Richiesta al DBonline
                if (code == 200)
                {
                    //Successo, passiamo alla schermata per inserire la password
                    await Shell.Current.GoToAsync("//first/login/password3");
                    EntrysTextClear();
                }
                else
                {
                    //Qualcosa non ha funzionato
                    await DisplayAlert("", "Qualcosa è andato storto, riprovare.", "Ok");
                }

            }

        }

        /* Nuova richiesta del codice per email.*/
        private async void AskAgainButton_Clicked(object sender, EventArgs e)
        {
            //Richiesta al DBonline
            if (await DesperateDB.ResetPasswordEmailSender(ResetPassword1Page.UserEmail) == 200)
            {
                //Successo
                await DisplayAlert("", "Richiesta effettuata con successo, verifica nuovamente la tua email!", "Ok");
            }
            else
            {
                //Qualcosa non ha funzionato
                await DisplayAlert("Errore", "Qualcosa è andato storto, verificare la tua " +
                    "connessione ad internet", "Ok");
            }
        }

        /* True se tutti i numeri sono inseriti, False altrimenti.*/
        private bool ButtonEnable()
        {
            return !Entry1.Text.Equals(string.Empty) &&
                   !Entry2.Text.Equals(string.Empty) &&
                   !Entry3.Text.Equals(string.Empty) &&
                   !Entry4.Text.Equals(string.Empty) &&
                   !Entry5.Text.Equals(string.Empty) &&
                   !Entry6.Text.Equals(string.Empty);
        }

        /* Pulizia per intero delle caselle di testo.*/
        private void EntrysTextClear()
        {
            Entry1.Text = string.Empty;
            Entry2.Text = string.Empty;
            Entry3.Text = string.Empty;
            Entry4.Text = string.Empty;
            Entry5.Text = string.Empty;
            Entry6.Text = string.Empty;
        }
    }
}