using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using Desperate_Houseworks.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Desperate_Houseworks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /* Pagina del profilo dell'utente (o dei membri della famiglia).*/
    public partial class ProfilePage : ContentPage
    {
        //Primo caricamento del profilo attuale. Da includere nel momento in cui si effettua  il Log Out
        private readonly FamilyMember Membro;
        public static bool firstLoad;

        public ProfilePage()
        {
            InitializeComponent();

            Membro = ClassGlobal.ActualUser;
            firstLoad = true;
        }

        public ProfilePage(FamilyMember membro)
        {
            InitializeComponent();

            Membro = membro;
            firstLoad = false;
            InitializeProfile(membro);

            ChangeNameIcon.IsVisible = false;
            FamilyButton.IsVisible = false;
            SettingsButton.IsVisible = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (firstLoad)
            {
                InitializeProfile(ClassGlobal.ActualUser);
            }

            if (Membro.Username.Equals(ClassGlobal.ActualUser.Username))
            {
                ChangeNameIcon.IsVisible = ClassGlobal.isLogged;
            }
            else
            {
                ChangeNameIcon.IsVisible = false;
                ((ProfileViewModel)BindingContext).GeneraMedaglie(Membro.Username);
            }

            ((ProfileViewModel)BindingContext).UpdateUserMedals();

            firstLoad = false;
        }

        /* Caricamento del profilo degli altri membri della famiglia.*/
        private void InitializeProfile(FamilyMember f)
        {
            //Imposto il BindingContext che genererà il medagliere
            BindingContext = new ProfileViewModel(f);
        }

        /* Modifica il nickname dell'utente stesso dal DBonline e DBlocale.*/
        public static async Task<int> ChangeNameFromDBs(string nickname)
        {
            var code = await DesperateDB.UpdateUserNickname(nickname);
            if (code == 200)
            {
                ClassGlobal.ActualUser.Nickname = nickname;
                await App.DatabaseUser.SetUser(ClassGlobal.ActualUser);
            }
            return code;
        }

        /* Una volta che l'utente è stato autenticato, vengono visualizzate le icone
         * Per poter modificare il profilo. */
        public void OnLogin()
        {
            ChangeNameIcon.IsVisible = true;
            ((ProfileViewModel)BindingContext).GeneraMedaglie(Membro.Username);
            ((ProfileViewModel)BindingContext).UserIconCommand = new Command(((ProfileViewModel)BindingContext).UserIconButtonAsync);
        }

        /* Trattamento della pagina come se fosse una Navigation page,
         * cliccando indietro si torna alla schermata principale se sto 
         * navigando la pagina come actualuser.
         * Se sto vedendo pagina del membro della famiglia torno alla 
         * schermata precedente.*/
        protected override bool OnBackButtonPressed()
        {
            if (LabelUsername.Text.Equals(ClassGlobal.ActualUser.Nickname))
            {
                Shell.Current.GoToAsync("//main/MainTaskPage");
            }
            else
            {
                Navigation.PopModalAsync();
            }
            return true;
        }
    }
}