using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using Desperate_Houseworks.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Desperate_Houseworks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /* Pagina contenente i componenti della famiglia.*/
    public partial class FamilyPage : ContentPage
    {
        public FamilyPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(() => ((FamilyViewModel)BindingContext).Refresh());
        }

        /* Caricamento lista dei familiari.*/
        public static async Task<List<FamilyMember>> InitializeFamily()
        {
            return await DesperateDB.GetFamily();
        }

        /* Apertura profilo familiare.*/
        private async void GoToMemberProfile_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //Non permette di cliccare velocemente l'apertura del profilo
            IsEnabled = false;
            FamilyMember membro = (FamilyMember)e.Item;
            await Navigation.PushModalAsync(new ProfilePage(membro));
            IsEnabled = true;
        }

        /* Invio invito di unione in famiglia.*/
        private async void AddFamilyButton_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Invita in famiglia", "Inserirsci l'username della persona da invitare:", "Ok", "Annulla");

            //Inserimento non effettuato
            if (result != null)
            {
                //Richiesta al DBonline
                var statusCode = await DesperateDB.SendFamilyJoinRequest(result);
                switch (statusCode)
                {
                    case 200:
                        await DisplayAlert("", "Richiesta inviata con successo", "Ok");
                        return;

                    case 432:
                        await DisplayAlert("Errore", "L'utente invitato è già in una famiglia.", "Ok");
                        return;

                    case 436:
                        await DisplayAlert("Errore", "Hai già invitato questo utente in famiglia.", "Ok");
                        return;

                    case 402:
                        await DisplayAlert("Errore", "L'username inserito è inesistente.", "Ok");
                        return;

                    default: //435
                        await DisplayAlert("Oh, no!", "Qualcosa è andato storto con la tua richiesta", "Ok");
                        return;
                };
            }
        }

        /* Modifica del nome della famiglia dai DBs.*/
        public static async Task<bool> ChangeFamilyNameFromDBs(string result)
        {
            //Chiamata DBonline
            if (await DesperateDB.UpdateFamilyName(result) != 200)
            {
                await Shell.Current.DisplayAlert("Errore inaspettato",
                    "Riprova a modificare il nome della famiglia più tardi.", "Ok");
                
                //Modifica non riuscita
                return false;
            }
            else
            {
                //Aggiornamento nome sulla ClassGlobal
                ClassGlobal.ActualUser.FamilyName = result;

                //Aggiornamento nome sul DBlocale
                await App.DatabaseUser.SetUser(ClassGlobal.ActualUser);

                //Modifica effettuata
                return true;
            }
        }
    }

}