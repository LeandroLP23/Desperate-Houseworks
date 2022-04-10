using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using Desperate_Houseworks.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;

namespace Desperate_Houseworks.ViewModels
{
    public class MainTaskViewModel : BaseViewModel
    {
        //Per poter impostare la prima lettera maiusco delle parole
        private readonly TextInfo MyTI = new CultureInfo("it-IT", false).TextInfo;

        private List<UserTaskClass> TasksList = new List<UserTaskClass>();

        private UserTaskClass PreviousUserTaskDone;
        private UserTaskClass PreviousUserTaskRemove;

        private bool listaVuota = false;
        public bool ListaVuota
        {
            get => listaVuota;
            set
            {
                if (listaVuota != value)
                {
                    listaVuota = value;
                    OnPropertyChanged();
                }
            }
        }

        private string nickname;
        public string Nickname
        {
            get => nickname;
            set
            {
                if (nickname != value)
                {
                    nickname = value;
                    OnPropertyChanged();
                }
            }
        }

        private string dateToday;
        public string DateToday
        {
            get => dateToday;
            set
            {
                if (dateToday != value)
                {
                    dateToday = value;
                    OnPropertyChanged();
                }
            }
        }

        //Se true mostra tutte le task. Se false mostra solo quelle non verificate.
        private bool filtroVerifica = false;
        public bool FiltroVerifica
        {
            get => filtroVerifica;
            set
            {
                if (filtroVerifica != value)
                {
                    filtroVerifica = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<Grouping<string, UserTaskClass>> tasksGroups =
            new ObservableCollection<Grouping<string, UserTaskClass>>();
        public ObservableCollection<Grouping<string, UserTaskClass>> TasksGroups
        {
            get => tasksGroups;
            set
            {
                if (tasksGroups != value)
                {
                    tasksGroups = value;
                    OnPropertyChanged();
                }
            }
        }

        private string raggruppamentoName = "Giorni";
        public string RaggruppamentoName
        {
            get => raggruppamentoName;
            set
            {
                if (raggruppamentoName != value)
                {
                    raggruppamentoName = value;
                    OnPropertyChanged();
                }
            }
        }

        public AsyncCommand RefreshCommand => new AsyncCommand(() => Refresh());
        public AsyncCommand LogTaskPageOpenCommand => new AsyncCommand(() => LogTaskPageOpen());
        public Command ChangeGroupingCommand => new Command(() => ChangeGrouping());
        public Command ChangeVerifiedCommand => new Command(() => ChangeVerified());
        public Command GoToTodaysTaskButton => new Command<ListView>((list) => GoToTodaysTask(list));
        public AsyncCommand<UserTaskClass> CancelTaskCommand => new AsyncCommand<UserTaskClass>((task) => RemoveTask(task));
        public AsyncCommand<UserTaskClass> DoneTaskCommand => new AsyncCommand<UserTaskClass>((task) => DoneTask(task));

        public MainTaskViewModel()
        {
            //Gestione della data
            DateToday = MyTI.ToTitleCase(DateTime.Now.ToString("dddd dd MMMM", new CultureInfo("it-IT")));

        }

        /* Aggiornamento del Nickname dell'utente.*/
        public void RefreshNickname()
        {
            Nickname = ClassGlobal.ActualUser.Nickname;
        }

        /* Cambia il metodo di raggruppamento (Giornate - Categoria).*/
        private void ChangeGrouping()
        {
            if (MainTaskPage.filtroTask.Equals("Giorni"))
            {
                MainTaskPage.filtroTask = "Categorie";
                RaggruppamentoName = "Categorie";
            }
            else
            {
                MainTaskPage.filtroTask = "Giorni";
                RaggruppamentoName = "Giorni";
            }
            Raggruppamento();
        }

        /* Cambia il metodo di raggruppamento (Svolte&DaSvolgere - DaSvolgere).*/
        private void ChangeVerified()
        {
            FiltroVerifica = !FiltroVerifica;
            Raggruppamento();
        }


        /* Rimozione task dalla collezione se riesce ad essere rimossa dal 
         * DBonline e poi dal DBlocale. SnackBar per accertarsi della richiesta. */
        private async Task DoneTask(UserTaskClass task)
        {
            //Gestione della descrizione vuota
            if (task.Description.Equals(ClassGlobal.descrizioneVuota))
            {
                task.Description = string.Empty;
            }
            UserTaskClass TaskDaModificare = new UserTaskClass();

            foreach (var item in TasksList)
            {
                if (item.Equals(task))
                {
                    TaskDaModificare = item;
                    TaskDaModificare.Verified = true;
                    TaskDaModificare.IsTapped = false;
                }
            }

            //Eseguo l'operazione precedente istantaneamente
            ExecuteTaskFunction();

            //Metto in coda la task attuale
            PreviousUserTaskDone = task;

            //Azione da svolgere in caso l'utente prema il bottone
            var Action = new SnackBarActionOptions
            {
                Action = async () =>
                {
                    //A questo punto non ho più task precedenti da eseguire.
                    PreviousUserTaskDone = null;
                    //Riaggiungo nella lista dalla collezione
                    TaskDaModificare.Verified = false;
                    Raggruppamento();
                    await Refresh();
                },
                Text = "Annulla",
                ForegroundColor = Color.White,
            };

            //Snackbar da mostrare in fondo alla pagina
            var SnackBar = new SnackBarOptions
            {
                MessageOptions = new MessageOptions
                {
                    Foreground = Color.White,
                    Message = "Compito Svolto!",
                },
                BackgroundColor = Color.FromHex("#0047AB"),
                Duration = TimeSpan.FromSeconds(3),
                Actions = new[] { Action },
                CornerRadius = 10,
            };

            //Display SnackBar
            var ans = await Application.Current.MainPage.DisplaySnackBarAsync(SnackBar);

            //In caso l'utente non abbia annullato l'operazione, quest'ultima viene svolta
            if (!ans)
            {
                //Chiamata ai DBs
                if (!await MainTaskPage.TaskDoneFromDBs(task))
                {
                    ProfileViewModel.UpdateMedals(task.Category);

                    //Dato che la task precedente da verificare è questa, ed è già stata verificata
                    //Rimetto il riferimento a null.

                    PreviousUserTaskDone = null;

                    //Problemi nella verifica
                    bool ans1 = await Application.Current.MainPage.DisplayAlert(
                        "Errore inaspettato", "Svolgimento non salvato", "Riprova", "Annulla");
                    if (ans1)
                    {
                        //Riprova a svolgere
                        await DoneTask(task);
                    }
                }
            }
            //Il caso else è svolto all'interno dell'azione
        }

        /* Rimozione task dalla collezione se riesce ad essere rimossa dal DBonline e poi 
         * dal DBlocale. PopUp in caso di eliminazione non riuscita. */
        private async Task RemoveTask(UserTaskClass task)
        {
            // Rimozione dalla collezione
            TasksList.Remove(task);
            Raggruppamento();

            //Se è presente un operazione precedente verrà eseguita istantaneamente.
            ExecuteTaskFunction();

            //Imposto una nuova operazione.
            PreviousUserTaskRemove = task;

            // Azione da svolgere in caso l'utente prema il bottone per sbaglio
            var Action = new SnackBarActionOptions
            {
                Action = async () =>
                {
                    //Non ho più task da eliminare
                    PreviousUserTaskRemove = null;
                    //Riaggiungo nella lista dalla collezione
                    TasksList.Add(task);
                    Raggruppamento();
                    await Refresh();
                },
                Text = "Annulla",
                ForegroundColor = Color.White,
            };

            // Snackbar da mostrare in fondo alla pagina
            var SnackBar = new SnackBarOptions
            {
                MessageOptions = new MessageOptions
                {
                    Foreground = Color.White,
                    Message = "Compito Cancellato!",
                },
                BackgroundColor = Color.FromHex("#0047AB"),
                Duration = TimeSpan.FromSeconds(3),
                Actions = new[] { Action },
                CornerRadius = 10,
            };

            // In caso di rimozione casuale
            var ans = await Application.Current.MainPage.DisplaySnackBarAsync(SnackBar);

            // In caso l'utente non abbia annullato l'opeazione, quest'ultima viene svolta
            if (!ans)
            {
                //Dato che l'utente non ha annullato la richiesta
                //Rimetto il riferimento a null.
                PreviousUserTaskRemove = null;

                //Chiamata ai Dbs
                if (!await MainTaskPage.CancelTaskFromDBs(task))
                {
                    //Problemi nella rimozione
                    bool ans1 = await Application.Current.MainPage.DisplayAlert(
                        "Errore inaspettato", "Rimozione non effettuata", "Riprova", "Annulla");
                    if (ans1)
                    {
                        //Riprova a svolgere
                        await RemoveTask(task);
                    }
                }
                // Il caso else è svolto all'interno dell'azione
            }
        }

        /* Aggiornamento della schermata: richieste le task dal DBonline, 
         * salvate nel DBlocale. Se non ricevere le task dal DBonline, 
         * si mantiene la lista attuale di task.*/
        public async Task Refresh()
        {
            IsBusy = true;

            if (ClassGlobal.isLogged)
            {
                //Recupero dal DBlocale se ho sincronizzato del DBonline
                await App.Database.SyncUserTaskClass();
            }

            //Lista delle task presenti nel DBlocale
            TasksList = await MainTaskPage.RecoverLocalDB();
            Raggruppamento();

            IsBusy = false;
        }

        /* Gestione del raggruppamento della lista di task*/
        private void Raggruppamento()
        {
            switch (MainTaskPage.filtroTask)
            {
                case "Giorni":
                    GroupTasksByDay();
                    break;
                case "Categorie":
                    GroupTasksByCategory();
                    break;
            }
        }

        /* Raggruppamento secondo Giornata.*/
        private void GroupTasksByDay()
        {
            /* Sfrutto il MyTi per la gestione delle lettere maiuscole */
            
            //Raggruppamento per giorno
            IEnumerable<Grouping<string, UserTaskClass>> sorted;

            if (FiltroVerifica)
            {
                sorted = from item in TasksList
                         orderby item.Verified, item.Date descending
                         group item by MyTI.ToTitleCase(item.Date.ToString("dddd, dd MMMM yyyy", new CultureInfo("it-IT"))) into itemGroup
                         select new Grouping<string, UserTaskClass>(itemGroup.Key, itemGroup);
            }
            else
            {
                sorted = from item in TasksList
                         where !item.Verified
                         orderby item.Verified, item.Date descending
                         group item by MyTI.ToTitleCase(item.Date.ToString("dddd, dd MMMM yyyy", new CultureInfo("it-IT"))) into itemGroup
                         select new Grouping<string, UserTaskClass>(itemGroup.Key, itemGroup);
            }

            //Gestione label invisibile
            ListaVuota = sorted.ToArray().Length == 0;

            //Aggiornamento dei gruppi
            TasksGroups = new ObservableCollection<Grouping<string, UserTaskClass>>(sorted);
        }

        /* Raggruppamento secondo Categoria.*/
        private void GroupTasksByCategory()
        {
            //Raggruppamento per categoria
            IEnumerable<Grouping<string, UserTaskClass>> sorted;
            if (FiltroVerifica)
            {
                sorted = from item in TasksList
                         orderby item.Verified, item.Date descending
                         group item by item.Category into itemGroup
                         select new Grouping<string, UserTaskClass>(itemGroup.Key, itemGroup);
            }
            else
            {
                sorted = from item in TasksList
                         where !item.Verified
                         orderby item.Verified, item.Date descending
                         group item by item.Category into itemGroup
                         select new Grouping<string, UserTaskClass>(itemGroup.Key, itemGroup);
            }

            //Gestione label invisibile
            ListaVuota = sorted.ToArray().Length == 0;

            //Aggiornamento dei gruppi
            TasksGroups = new ObservableCollection<Grouping<string, UserTaskClass>>(sorted);
        }

        /* Scorrimento automatico alle task del giorno.*/
        private void GoToTodaysTask(ListView list)
        {
            foreach (Grouping<string, UserTaskClass> grouping in TasksGroups)
            {
                string todaysKey = new CultureInfo("it-IT", false).TextInfo.ToTitleCase(DateTime.Now.ToString("dddd, dd MMMM yyyy", new CultureInfo("it-IT")));
                if (grouping.Key.Equals(todaysKey))
                {
                    list.ScrollTo(grouping[0], ScrollToPosition.Start, true);
                    break;
                }
            }
        }

        /*Esegue istantaneamente l'operazione precedente.*/
        private void ExecuteTaskFunction()
        {
            if (PreviousUserTaskRemove != null)
            {
                DesperateDB.RemoveUserTask(PreviousUserTaskRemove);
                PreviousUserTaskRemove = null;
            }
            if (PreviousUserTaskDone != null)
            {
                ProfileViewModel.UpdateMedals(PreviousUserTaskDone.Category);
                DesperateDB.UserTaskVerifier(PreviousUserTaskDone);
                PreviousUserTaskDone = null;
            }
        }

        /* Apertura LogTaskPage (Pagina eventi).*/
        private async Task LogTaskPageOpen()
        {
            Routing.RegisterRoute(route: "logpage", typeof(LogTaskPage));
            await Shell.Current.GoToAsync("//main/logpage");
        }
    }
}
