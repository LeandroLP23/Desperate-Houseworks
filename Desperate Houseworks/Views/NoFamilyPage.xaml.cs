using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Desperate_Houseworks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    
    /* Pagina per utenti che non hanno una famiglia.*/
    public partial class NoFamilyPage : ContentPage
    {
        private ObservableCollection<FamilyRequestView> CollectionRichieste { get; set; }

        public NoFamilyPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ListViewNoFamily.BeginRefresh();
        }

        /* Refreshing della ListView, per cercare inviti in famiglia.*/
        private async void ListViewNoFamily_Refreshing(object sender, EventArgs e)
        {
            List<JoinFamilyRequest> lista = await DesperateDB.GetUserJoinFamilyRequestsAsync();
            CollectionRichieste = new ObservableCollection<FamilyRequestView>(lista.ConvertAll(JoinRequest2FamilyRequest));

            //Impostazione binding listview
            ListViewNoFamily.ItemsSource = CollectionRichieste;
        
            ListViewNoFamily.EndRefresh();
        }

        /* Gestione dell'invito in famiglia.*/
        private async void ListViewNoFamily_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            FamilyRequestView richiesta = (FamilyRequestView)e.Item;

            bool answer = await DisplayAlert("",
                "Vuoi accettare l'invito per entrare nella famiglia "
                + richiesta.FamilyName + "?", "Accetta", "Annulla");
            
            if (answer)
            {
                //Accettare invito
                Accept(richiesta.FamilyCode, richiesta.FamilyName);
            }
            else
            {
                //Annullare click
                bool rispostaRifiuto = await DisplayAlert("",
                    "Preferisci cancellare questo invito?", "Si", "No");
                if (rispostaRifiuto)
                {
                    //Rifiuto invito
                    Refuse(richiesta.FamilyCode);
                }
            }
        }

        /* Accettare invito.*/
        private async void Accept(int familyID, string familyName)
        {
            int response = await DesperateDB.JoinFamily(familyID);
            if (response == 200)
            {
                ClassGlobal.ActualUser.FamilyName = familyName;
                await DisplayAlert("Evviva!", "Sei entrato con successo nella famiglia", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Oh no!", "Qualcosa è andato storto", "OK");
            }
        }

        /* Rifiutare invito.*/
        private async void Refuse(int familyID)
        {
            int response = await DesperateDB.RefuseFamilyJoinRequest(familyID);
            if (response == 200)
            {
                EliminaRichiesta(familyID);
                await DisplayAlert("", "Hai rifiutato con successo la richiesta!", "OK");
            }
            else
            {
                await DisplayAlert("Oh no!", "Qualcosa è andato storto", "OK");
            }
        }

        /* Cancellazione invito.*/
        private void EliminaRichiesta(int familyID)
        {
            FamilyRequestView richiesta = new FamilyRequestView();
            foreach (FamilyRequestView item in CollectionRichieste)
            {
                if (item.FamilyCode.Equals(familyID))
                {
                    richiesta = item;
                }
            }
            CollectionRichieste.Remove(richiesta);
        }

        /* Creazione di una famiglia.*/
        private async void CreateNewFamilyClicked(object sender, EventArgs e)
        {
            string familyName = await DisplayPromptAsync("Inserisci il nome della famiglia che vuoi creare", "");
            
            //Inserimento anullato
            if (familyName != null)
            {
                if (familyName.Length < 3)
                {
                    await DisplayAlert("Errore!", "La lunghezza minima per il nome della famiglia è pari a 3", "Ok");
                    return;
                }

                //Gestione ActivityIndicator e bottone
                CreateFamilyButton.IsVisible = false;
                ActivityIndicator.IsRunning = true;
                ActivityIndicator.IsRunning = true;

                var response = await DesperateDB.CreateFamily(familyName);

                //Gestione ActivityIndicator e bottone
                CreateFamilyButton.IsVisible = true;
                ActivityIndicator.IsRunning = false;
                ActivityIndicator.IsRunning = false;

                if (response == 200)
                {
                    ClassGlobal.ActualUser.FamilyName = familyName;
                    await DisplayAlert("Complimenti!", "La famiglia '" + familyName + "' è stata creata con successo", "Ok");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Oh no!", "Qualcosa è andato storto", "OK");
                }
            }
        }

        /* Convertitore JoinRequest 2 FamilyRequest*/
        private FamilyRequestView JoinRequest2FamilyRequest(JoinFamilyRequest input)
        {
            FamilyRequestView tmp = new FamilyRequestView
            {
                FamilyCode = input.FamilyCode,
                FamilyName = input.FamilyName,
                Username = input.Username,
                Nickname = input.Nickname,
                Picture = input.Picture
            };
            return tmp;
        }

    }

    public class FamilyRequestView : JoinFamilyRequest
    {
        public string Icon { get => FamilyMethods.MemberInt2Pic(Picture); }
        public string UserAsking { get => Nickname; }
    }
}
