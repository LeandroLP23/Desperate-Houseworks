using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using Desperate_Houseworks.Views;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;

namespace Desperate_Houseworks.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private FamilyMember Utente { get; set; }

        private static Dictionary<string, int> MedalsToUpdate = new Dictionary<string, int>();

        private ObservableCollection<MedalClass> obsCollectionMedaglie = new ObservableCollection<MedalClass>();
        public ObservableCollection<MedalClass> ObsCollectionMedaglie
        {
            get => obsCollectionMedaglie;
            set
            {
                if (obsCollectionMedaglie != value)
                {
                    obsCollectionMedaglie = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Icon
        {
            get
            {
                if (Utente.Username.Equals(ClassGlobal.ActualUser.Username))
                {
                    return FamilyMethods.MemberInt2Pic(ClassGlobal.ActualUser.Picture);
                }
                else
                {
                    return FamilyMethods.MemberInt2Pic(Utente.Picture);
                }
            }
            set => OnPropertyChanged();
        }

        public string LabelNickname
        {
            get => Utente.Nickname;
            set => OnPropertyChanged();
        }

        public AsyncCommand OpenSettingPageCommand => new AsyncCommand(() => OpenSettingPageAsync());
        public AsyncCommand OpenFamilyIconCommand => new AsyncCommand(() => OpenFamilyPageAsync());

        public AsyncCommand ChangeNameCommand { get; set; }

        private Command userIconCommand;
        public Command UserIconCommand
        {
            get => userIconCommand;
            set
            {
                if (userIconCommand != value)
                {
                    userIconCommand = value;
                    OnPropertyChanged();
                }
            }
        }

        public ProfileViewModel(FamilyMember user)
        {
            Utente = user;

            //Apertura del profilo personale
            if (user.Equals(ClassGlobal.ActualUser))
            {
                //Posso cambiare immagine del profilo
                UserIconCommand = new Command(UserIconButtonAsync);
                ChangeNameCommand = new AsyncCommand(ChangeName);
            }
            //Apertura del profilo di un membro della famiglia
            else
            {
                //Non posso cambiare immagine del profilo
                UserIconCommand = new Command(UserIconButtonReadOnlyAsync);
            }

            //BruteForce in quanto l'isEnabled non funziona come dovrebbe.
            if (!ClassGlobal.isLogged)
            {
                UserIconCommand = new Command(UserIconButtonReadOnlyAsync);
            }

            GeneraMedaglie(user.Username);
        }

        /* PopUp per cambiare icona del profilo. */
        public async void UserIconButtonAsync()
        {
            Popup popup = new Popup();
            popup.Content = new IconPickerPopup(popup);
            Icon = (string)await Shell.Current.Navigation.ShowPopupAsync(popup);
        }

        /* Pressione immagine del profilo di un familiare.*/
        private void UserIconButtonReadOnlyAsync()
        {
            System.Diagnostics.Debug.WriteLine(";)");
        }

        /* Apertura FamilyPage se utente ha una famiglia.
         * Apertura NoFamilyPage se utente non ha famiglia.*/
        private async Task OpenFamilyPageAsync()
        {
            if (!ClassGlobal.isLogged)
            {
                await Shell.Current.DisplayAlert("Oh no!", "Effettua l'accesso per vedere la schermata della famiglia", "OK");
                return;
            }

            if (ClassGlobal.ActualUser.FamilyName == null)
            {
                await Shell.Current.GoToAsync("//main/nofamily");
            }
            else
            {
                await Shell.Current.GoToAsync("//main/family");
            }
        }

        /* Cambiare nickname dell'utente. */
        private async Task ChangeName()
        {
            string nick = ClassGlobal.ActualUser.Nickname;

            string result = await Shell.Current.DisplayPromptAsync("Modifica il nickname!",
                "Il nickname è il nome con cui vieni visualizzato nella tua famiglia.", placeholder: nick);

            //Inserimento annullato
            if (result != null)
            {
                if (result.Length < 3)
                {
                    await Shell.Current.DisplayAlert("Nickname non valido!", "Inserire almeno 3 caratteri", "Ok");
                }
                else
                {
                    //Se il nickname inserito è uguale a quello già presente sul dispositivo
                    if (result.Equals(ClassGlobal.ActualUser.Nickname))
                    {
                        //Non effetuo modifiche
                        return;
                    }

                    int code = await ProfilePage.ChangeNameFromDBs(result);
                    if (code == 200)
                    {
                        LabelNickname = result;
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Errore inaspettato",
                            "Riprova a modificare il nickname più tardi.", "Ok");
                    }
                    return;
                }
            }
        }

        /* 1) Generazione del medagliere vuoto;
         * 2) Richiesta delle medaglie Online 
         * 3) Sostituione delle medaglie. */
        public async void GeneraMedaglie(string username)
        {
            GenerazioneMedagliereVuoto(username);

            //Controllo per sicurezza
            if (username != null && ClassGlobal.isLogged)
            {
                //Richiesta delle medaglie dal DBonline
                List<MedalClass> lista = await DesperateDB.GetUserMedals(username);
                if (lista != null)
                {
                    foreach (MedalClass item in lista)
                    {
                        if (ObsCollectionMedaglie.Remove(item))
                        {
                            ObsCollectionMedaglie.Add(item);
                        }
                    }
                    var app = ObsCollectionMedaglie.OrderByDescending(x => x.Quantity);

                    ObsCollectionMedaglie = new ObservableCollection<MedalClass>(app);
                }
            }
        }

        /* Creazione e visualizzazione del medagliere vuoto.*/
        private void GenerazioneMedagliereVuoto(string username)
        {
            ObsCollectionMedaglie.Clear();
            List<string> catList = GestioneEnum.Cat2IList();

            foreach (string cat in catList)
            {
                ObsCollectionMedaglie.Add(new MedalClass(username, cat, 0));
            }
        }

        /* Ogni qual volta che una task viene verificata, aggiungo 1 alla rispettiva categoria.
         * Questo avviene in maniera tale da aggiornare il profilo senza eseguire ogni volta 
         * una chiamata al DB online.*/
        public static void UpdateMedals(string category)
        {
            if (MedalsToUpdate.TryGetValue(category, out int verifica))
            {
                MedalsToUpdate.Remove(category);
                MedalsToUpdate.Add(category, verifica + 1);
            }
            else
            {
                MedalsToUpdate.Add(category, 1);
            }
        }

        /* Metodo per aggiornare i valori alle rispettive medaglie appartenenti all'Observable
         * Una volta aggiornati, ripulisco il dizionario utilizzato in precedenza.*/
        internal void UpdateUserMedals()
        {
            foreach (var categoria in MedalsToUpdate.Keys)
            {
                MedalsToUpdate.TryGetValue(categoria, out int quantity);
                foreach (var medaglia in ObsCollectionMedaglie)
                {
                    if (medaglia.Name.Equals(categoria))
                    {
                        medaglia.Quantity += quantity;
                    }
                }
            }
            MedalsToUpdate.Clear();
        }

        /* Navigazione verso SettingsPage.*/
        private async Task OpenSettingPageAsync()
        {
            Routing.RegisterRoute(route: "settings", typeof(SettingsPage));
            await Shell.Current.GoToAsync("//main/settings");
        }

    }
}
