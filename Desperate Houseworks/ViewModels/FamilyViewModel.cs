using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using Desperate_Houseworks.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Desperate_Houseworks.ViewModels
{
    internal class FamilyViewModel : BaseViewModel
    {
        private bool activityVisible = false;
        public bool ActivityVisible
        {
            get => activityVisible;
            set
            {
                if (activityVisible != value)
                {
                    activityVisible = value;
                    OnPropertyChanged();
                }

            }
        }

        private bool activityRunning = false;
        public bool ActivityRunning
        {
            get => activityRunning;
            set
            {
                if (activityRunning != value)
                {
                    activityRunning = value;
                    OnPropertyChanged();
                }

            }
        }

        private bool labelVisible = true;
        public bool LabelVisible
        {
            get => labelVisible;
            set
            {
                if (labelVisible != value)
                {
                    labelVisible = value;
                    OnPropertyChanged();
                }

            }
        }

        private string familyName = ClassGlobal.ActualUser.FamilyName;
        public string FamilyName
        {
            get => familyName;
            set
            {
                if (familyName != value)
                {
                    familyName = value;
                    OnPropertyChanged();
                }

            }
        }

        public ObservableCollection<FamilyMemberView> ListFamilyMembers
        {
            get => listFamilyMembers;
            set
            {
                if (listFamilyMembers != value)
                {
                    listFamilyMembers = value;
                    OnPropertyChanged();
                }

            }
        }
        private ObservableCollection<FamilyMemberView> listFamilyMembers = new ObservableCollection<FamilyMemberView>();

        public AsyncCommand RefreshCommand => new AsyncCommand(() => Refresh());
        public AsyncCommand ChangeFamilyNameCommand => new AsyncCommand(() => ChangeFamilyName());

        public FamilyViewModel()
        {
        }

        /* Aggiornamento ObservableCollection dei membri della famiglia.*/
        public async Task Refresh()
        {
            IsBusy = true;
            var tmp = (await FamilyPage.InitializeFamily()).ConvertAll(FamilyMember2FamilyMemberView);
            ListFamilyMembers = new ObservableCollection<FamilyMemberView>(tmp);
            IsBusy = false;
        }

        /* Cambio del nome della famiglia.*/
        public async Task ChangeFamilyName()
        {
            string result = await Shell.Current.DisplayPromptAsync("Modifica",
                            "Modifica il nome della famiglia", placeholder: familyName);

            //Inserimento non effettuato
            if (result != null)
            {
                //Nome troppo corto
                if (result.Length < 3)
                {
                    await Shell.Current.DisplayAlert("Nome famiglia non valido!", "Inserire almeno 3 caratteri", "Ok");
                }
                else
                {
                    //Se il nome della famiglia inserito è uguale a quello già presente sul DB
                    if (result.Equals(ClassGlobal.ActualUser.Nickname))
                    {
                        //Non effettuo modifiche
                        return;
                    }

                    //Gestione ActivityIndicator
                    LabelVisible = false;
                    ActivityRunning = true;
                    ActivityVisible = true;

                    if (await FamilyPage.ChangeFamilyNameFromDBs(result))
                    {
                        //Aggiornamento nome sulla pagina
                        FamilyName = result;
                    }

                    //Gestione ActivityIndicator
                    LabelVisible = true;
                    ActivityRunning = false;
                    ActivityVisible = false;

                }
            }
        }

        /* Convertitore FamilyMember 2 FamilyMemberView*/
        private FamilyMemberView FamilyMember2FamilyMemberView(FamilyMember f)
        {
            return new FamilyMemberView
            {
                Username = f.Username,
                Picture = f.Picture,
                Nickname = f.Nickname,
            };
        }
    }
}
