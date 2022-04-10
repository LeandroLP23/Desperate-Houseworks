using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using Desperate_Houseworks.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Desperate_Houseworks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /* Schermata per permettere all'utente di decidere quali task svolgere.*/
    public partial class NewTaskPage : ContentPage
    {

        private bool AlreadyLoadedFamily = false;

        private bool UpsideDown = false;
        public ObservableCollection<GenericTaskClass> GenericTasksCollection { get; set; }


        public NewTaskPage()
        {
            InitializeComponent();
            BindingContext = new NewTaskViewModel();
            LoadTask();
        } 

        protected override void OnAppearing()
        {
            //Chisura expander 'compito personalizzato'
            if (ExpanderCustomTask.IsExpanded)
            {
                //Annullare l'animazione
                ExpanderCustomTask.AnimationLength = 0;

                //Chiusura Expander
                ExpanderCustomTask.IsExpanded = false;

                //Chiusura schermo nero
                ((NewTaskViewModel)BindingContext).SchermoOpaco_Visible = false;
            }

            //Reset orientamento della freccia
            ResetRotation();

            //Nel caso in cui l'utente fosse già loggato, e non avesse ancora recuperato la famiglia
            //Allora gli e la si fa recuperare da DB.
            if(ClassGlobal.isLogged && !AlreadyLoadedFamily)
            {
                OnLogin();
            }

            base.OnAppearing();
        }

        /* Caricamento delle Task dal DBlocale e salvataggio sulla collezione
         * osservabile di GenericTask. 
         * Binding della collezione alla List View. */
        private async void LoadTask()
        {
            //Recupero delle GenericTask dal DBlocale
            GenericTasksCollection = new ObservableCollection<GenericTaskClass>(
                await App.Database.GetGenericTasksOrderedByCategoryAsync());

            //Binding alla ListView
            ListViewGenericTasks.ItemsSource = GenericTasksCollection;
        }

        /* Premuto invio sulla tastiera durante l'inserimento
         * della categoria. Fa perdere il focus dalla entry. */
        public void OnEnterClicked(object s, EventArgs e)
        {
            ((Entry)s).Unfocus();
        }

        /* Gestione del tap sul frame nero. */
        private void BlackFrame_Tapped(object sender, EventArgs e)
        {
            //Chiusura o apertura dell'expander 'compito personalizzato'
            ExpanderCustomTask.IsExpanded = !ExpanderCustomTask.IsExpanded;

            //Gestione interazioni con expander
            // (Frame nero e freccia)
            ExpanderCustomTask_Tapped(sender, e);

        }

        /* Gestione interazioni espansione expander 'compito personalizzato'. */
        private async void ExpanderCustomTask_Tapped(object sender, EventArgs e)
        {
            //Gestione visibilità frame nero
            ((NewTaskViewModel)BindingContext).BlackFrameChanging();

            //Animazione freccia
            if (UpsideDown)
            {
                ResetRotation();
            }
            else
            {
                await FrecciaCustom.RotateTo(180, 100);
                UpsideDown = true;
            }
        }

        /* Orientamento originario della freccia. */
        private async void ResetRotation()
        {
            await FrecciaCustom.RotateTo(0, 100);
            UpsideDown = false;
        }

        /* Aggiunta ASYNCRONA di una UserTask sul DBonline */
        public static async Task<int> AddTaskDBs(AbTaskClass t)
        {
            int codice = await DesperateDB.AddUserTask(t);

            //Gestione del salvataggio in DBlocale, se task per utente stesso
            if (codice == 200 && t.User == ClassGlobal.ActualUser.Username)
            {
                //Gestione della descrizione
                if (t.Description == null)
                {
                    t.Description = GestioneEnum.Name2Desc(t.Name);
                }
                //Aggiunta al DBlocale
                await App.Database.AddUserTaskAsync((UserTaskClass)t);

            }

            //I vari codici sono gestiti nel ViewModel
            return codice;
        }

        /* Trattamento della pagina come se fosse una Navigation page,
         * cliccando indietro si torna alla schermata principale. */
        protected override bool OnBackButtonPressed()
        {
            Shell.Current.GoToAsync("//main/MainTaskPage");
            return true;
        }

        //Nel momento in cui l'utente esegue l'accesso, viene caricata la lista dei familiari
        //Per assegnare i compiti.
        public async void OnLogin()
        {
            AlreadyLoadedFamily = true;
            
            await Task.Run(async () => await ((NewTaskViewModel)BindingContext).Refresh());

            //Forzare il caricamento della lista di utenti
            //Per gli elementi nella listview

            BindingContext = new NewTaskViewModel()
            {
                UsersList = new List<FamilyMember>(ClassGlobal.UserList4Picker),
            };

            //Selezione automatica utente

            /* QUESTA IMPOSTAZIONE DEL VALORE MODIFICA IL VALORE DELLA 
             * PROPRIETA' IN COMUNE 'selectedItem' DEI PICKER DEGLI UTENTI
             * DELLA FAMIGLIA. Nonostante stiamo modificando solamente uno dei due.*/
            UserPickerCust.SelectedIndex = 0;
            CategoryPickerCust.SelectedIndex = 5;
        }
    }
}