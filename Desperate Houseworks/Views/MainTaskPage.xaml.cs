using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using Desperate_Houseworks.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Desperate_Houseworks.Views
{
    /* Pagina principale, contenete le task dell'utente.*/
    public partial class MainTaskPage : ContentPage
    {
        private bool alreadyRecovered = false;

        public static string filtroTask = "Giorni";

        public MainTaskPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            (BindingContext as MainTaskViewModel).RefreshNickname();

            /* Utente non loggato / non ha la connessione. */
            if (!ClassGlobal.isLogged)
            {
                //Se non ha gia recuperato le task dal DBlocale le recupera
                if (!alreadyRecovered && !IsBusy)
                {
                    //Caricamento delle sole task presenti nel DBlocale
                    Task.Run(() => ((MainTaskViewModel)BindingContext).Refresh());

                }

                //Utente ha recuperato le task
                alreadyRecovered = true;

                //Non può più aggiornare
                ListViewUserTask.IsPullToRefreshEnabled = false;
            }
            /* Utente loggato && connesso. */
            else
            {
                //Può aggiornare la pagina
                ListViewUserTask.IsPullToRefreshEnabled = true;

                //Non ci interessa se ha gia recuperato le task
                alreadyRecovered = false;

                //Aggiorniamo la pagina all'avvio se non sta aggiornando
                if (!IsBusy)
                {
                    Task.Run(() => ((MainTaskViewModel)BindingContext).Refresh());
                }
            }

        }

        /* Aggiunta task recuperate dal DBlocale alla collezione. */
        public async static Task<List<UserTaskClass>> RecoverLocalDB()
        {
            //Recupera le task presenti nel DBlocale
            var tmp = new List<UserTaskClass>();

            foreach (UserTaskClass task in await App.Database.RecoveryFromUserTaskDb())
            {
                //Gestione colore categoria
                task.Color = GestioneEnum.Cat2Color(task.Category);

                //Gestione icona categoria
                task.Icon = GestioneEnum.Cat2Icon(task.Category);

                //Gestione della descrizione
                if (task.Description == "") /* Task senza descrizione*/
                {
                    //Aggiunta descrizione predefinita
                    task.Description = GestioneEnum.Name2Desc(task.Name);
                }

                tmp.Add(task);
            }
            return tmp;
        }

        /* Rimozione task da DBlocale e DBonline. */
        public async static Task<bool> CancelTaskFromDBs(UserTaskClass task)
        {
            //Rimozione dal DBonline
            if (await DesperateDB.RemoveUserTask(task) == 400)
            {
                //Rimozione non effettuata
                return false;
            };

            //Rimozione dal DBlocale
            await App.Database.DeleteUserTaskAsync(task);
            return true;
        }

        /* Rimozione task dal DBlocale e aggiornamento 
         * 'verified' sul DBonline. */
        public async static Task<bool> TaskDoneFromDBs(UserTaskClass task)
        {

            //Aggiornamento sul DBonline
            if (await DesperateDB.UserTaskVerifier(task) == 400)
            {
                //Aggiornamento non riuscito
                return false;
            }

            //Rimoziono dal DBlocale
            await App.Database.DeleteUserTaskAsync(task);
            return true;

        }

        /* Una volta che l'utente è stato autenticato viene refreshata la schermata
         * dei logs (se il refresh non è stato gia inizializzato) e viene abilitato il refresh.*/
        public async void OnLogin()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await ((MainTaskViewModel)BindingContext).Refresh();
                IsBusy = false;
            }
            ListViewUserTask.IsPullToRefreshEnabled = true;
        }

        /* Item cliccato, si espande il frame.*/
        public void ListViewUserTask_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((UserTaskClass)e.Item).IsTapped = !((UserTaskClass)e.Item).IsTapped;
        }
    }
}