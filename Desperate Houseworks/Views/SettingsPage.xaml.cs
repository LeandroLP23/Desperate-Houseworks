using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Desperate_Houseworks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /* Pagina contenente le impostazioni generiche dell'app.*/
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        /* Uscita dalla famiglia*/
        private async void FamilyQuitButton_Clicked(object sender, EventArgs e)
        {
            if (ClassGlobal.ActualUser.FamilyName == null)
            {
                await DisplayAlert("",
                "ERRORE. Attualmente non appartieni ad una famiglia", "Ok");
                return;
            }
            //Aggiungere informazioni su log e task
            bool answer = await DisplayAlert("Vuoi abbandonare la famiglia?",
                "L'unico modo per poterci tornare è solo mediante un invito", "Si", "No");
            if (answer)
            {
                if (await DesperateDB.QuitFamily(ClassGlobal.ActualUser.Username) == 200)
                {
                    ClassGlobal.ActualUser.FamilyName = null;
                    await DisplayAlert("", "Hai abbandonato la famiglia con successo", "Ok");
                }
                else
                {
                    await DisplayAlert("", "Si è verificato un errore imprevisto. Riprovare più tardi", "Ok");
                }
            }
        }

        /* Logout dall'account*/
        private async void LogOutButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Logout",
               "Sei sicuro di voler effettuare il Logout?", "Si", "No");
            if (answer)
            {
                ProfilePage.firstLoad = true;
                await App.DatabaseUser.ClearUsers();
                Application.Current.MainPage = new AppShell();

                await Shell.Current.GoToAsync("//first/FirstPage");
            }
        }

        /* Eventi da implementare nella versione da commercializzare.*/
        private async void TBAButton_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("To Be Added","Questo pulsante verra' implementato nella versione commercializzata.","OK");
        }
    }
}
