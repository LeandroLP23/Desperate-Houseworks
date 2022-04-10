using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Desperate_Houseworks.Views;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.CommunityToolkit.Extensions;

namespace Desperate_Houseworks.ViewModels
{
    internal class NewTaskViewModel : BaseViewModel
    {
        /* Presenza activityIndicator task Generiche.*/
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

        /* Presenza activityIndicator task Personalizzate.*/
        private bool customActivityRunning = false;
        public bool CustomActivityRunning
        {
            get => customActivityRunning;
            set
            {
                if (customActivityRunning != value)
                {
                    customActivityRunning = value;
                    OnPropertyChanged();
                }
            }
        }

        /* Rotazione della freccia del frame 'Compito personalizzato' */
        private bool showImageRotated;
        public bool ShowImageRotated
        {
            get => showImageRotated;
            set => SetProperty(ref showImageRotated, value);
        }

        /* Presenza schermo nero */
        private bool schermoOpaco_Visible = false;
        public bool SchermoOpaco_Visible
        {
            get => schermoOpaco_Visible;
            set
            {
                if (schermoOpaco_Visible != value)
                {
                    schermoOpaco_Visible = value;
                    OnPropertyChanged();
                }
            }
        }

        /* Lista membri della famiglia + utente stesso */
        private List<FamilyMember> usersList = ClassGlobal.UserList4Picker;
        public List<FamilyMember> UsersList
        {
            get => usersList;
            set
            {
                if (usersList != value)
                {
                    usersList = value;
                    OnPropertyChanged();
                }
            }
        }

        /* Lista delle categorie delle task */
        private List<string> categsList = GestioneEnum.Cat2IList();
        public List<string> CategsList
        {
            get => categsList;
            set
            {
                if (categsList != value)
                {
                    categsList = value;
                    OnPropertyChanged();
                }
            }
        }

        /* Posizione della categoria 'Altro' */
        private int index_CategCustom = 5;
        public int Index_CategCustom
        {
            get => index_CategCustom;
            set
            {
                if (index_CategCustom != value)
                {
                    index_CategCustom = value;
                    OnPropertyChanged();
                }
            }
        }

        /* Posizione dell'utente stesso */
        private int index_UsersCustom = 0;
        public int Index_UsersCustom
        {
            get => index_UsersCustom;
            set
            {
                if (index_UsersCustom != value)
                {
                    index_UsersCustom = value;
                    OnPropertyChanged();
                }
            }
        }

        public AsyncCommand OnClosePageCommand => new AsyncCommand(() => OnClosePage());
        public Command ExpandingCommand => new Command(() => BlackFrameChanging());
        public AsyncCommand RefreshCommand => new AsyncCommand(() => Refresh());
        public AsyncCommand<object[]> OnSavingCommand { get; }
        public Command<object[]> OnCancelButtonCommand { get; }
        public Command<object[]> OnCancelCustomCommand { get; }
        public Command<object[]> OnSavingCustomCommand { get; }

        public NewTaskViewModel()
        {
            OnSavingCommand = new AsyncCommand<object[]>(OnSavingAsync);
            OnCancelButtonCommand = new Command<object[]>(OnCancelButton);
            OnCancelCustomCommand = new Command<object[]>(OnCancelCustom);
            OnSavingCustomCommand = new Command<object[]>(OnSavingCustom);
        }

        /* Aggiornamento lista familiari.*/
        public async Task Refresh()
        {
            IsBusy = true;

            await ClassGlobal.GettingUsers();

            IsBusy = false;
        }

        /* Chiusura schermata di inserimento nuova task. */
        private async Task OnClosePage()
        {
            await Shell.Current.GoToAsync("//main/MainTaskPage");
        }

        /* Rende visibile o invisibile lo schermo nero. */
        public void BlackFrameChanging()
        {
            SchermoOpaco_Visible = !SchermoOpaco_Visible;
        }

        /* Annulla l'inserimento della TaskPersonalizzata.
         * Resetta i valori introdotti dall'utente.
         * Disattiva schermo nero.*/
        private void OnCancelCustom(object[] args)
        {
            Entry e1 = (Entry)args[0];
            e1.Text = string.Empty;

            //Brute froce con il colore impostato in App.xaml
            //Altrimenti può restare rosso doopo invio di compito senza nome
            e1.PlaceholderColor = Color.Gray;

            Entry e2 = (Entry)args[1];
            e2.Text = string.Empty;

            DatePicker d = (DatePicker)args[2];
            d.Date = DateTime.Now.Date;

            TimePicker t = (TimePicker)args[3];
            t.Time = new TimeSpan(12, 0, 0);

            //Categoria in automatico ad altro
            Picker pickCat = (Picker)args[4];
            pickCat.SelectedIndex = 5;

            //Utente stesso
            Picker pickUsr = (Picker)args[5];
            pickUsr.SelectedIndex = 0;

        }

        /* Annulla inserimento della task gia presente nel db 
         * con informazioni aggiuntive*/
        private void OnCancelButton(object[] args)
        {
            Entry entry = (Entry)args[0];
            DatePicker datepicker = (DatePicker)args[1];
            TimePicker timepicker = (TimePicker)args[2];
            entry.Text = "";
            datepicker.Date = DateTime.Now.Date;
            timepicker.Time = new TimeSpan(12, 0, 0);

            //Utente stesso
            Picker pickUsr = (Picker)args[4];
            pickUsr.SelectedIndex = 0;

            //Gestione della chiusura della task
            Expander expander = (Expander)args[3];
            expander.IsExpanded = false;
        }

        /* Salvataggio della task personalizzata. 
          Resetta i valori introdotti dall'utente e chiude la tendina.*/
        private async void OnSavingCustom(object[] args)
        {

            UserTaskClass task = new UserTaskClass
            {
                Name = (args[0] as Entry).Text,
                Category = GestioneEnum.Cat2IList()[(args[4] as Picker).SelectedIndex],
                Verified = false,
                Description = (args[1] as Entry).Text,
            };

            //Gestione del nome
            if ((args[0] as Entry).Text.Equals(string.Empty))
            {
                ClassGlobal.Shaking_Animation(args[0] as Entry);
                (args[0] as Entry).PlaceholderColor = Color.Red;
                return;
            }

            //Gestione della data
            string d = DateTime.Parse((args[2] as DatePicker).Date.ToString()).ToString("dd/MM/yyyy");
            string h = DateTime.Parse((args[3] as TimePicker).Time.ToString()).ToString("HH:mm");
            string orario = d + " " + h;
            task.Date = DateTime.ParseExact(orario, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

            //Gestione dello user
            if ((args[5] as Picker).SelectedIndex == -1)
            {
                task.User = ClassGlobal.ActualUser.Username;
            }
            else
            {
                task.User = ClassGlobal.UserList4Picker[(args[5] as Picker).SelectedIndex].Username;
            }

            //Gestione ActivityIndicator
            CustomActivityRunning = true;

            //Chiamata ai DBs
            int response = await NewTaskPage.AddTaskDBs(task);

            //Gestione ActivityIndicator
            CustomActivityRunning = false;

            if (response == 200)
            {
                //Utente vuole tornare sulla Home
                var Action = new SnackBarActionOptions
                {
                    Action = async () =>
                    {
                        //Pulizia delle caselle
                        var tmp = new object[6] { args[0], args[1], args[2], args[3], args[4], args[5] };

                        OnCancelCustom(tmp);

                        await Shell.Current.GoToAsync("//main/MainTaskPage");
                    },
                    Text = "Home",
                    ForegroundColor = Color.White,
                };

                //Snackbar da mostrare in fondo alla pagina
                var SnackBar = new SnackBarOptions
                {
                    MessageOptions = new MessageOptions
                    {
                        Foreground = Color.White,
                        Message = "Compito Inviato!",
                    },
                    BackgroundColor = Color.FromHex("#0047AB"),
                    Duration = TimeSpan.FromSeconds(2),
                    Actions = new[] { Action },
                    CornerRadius = 10,
                };

                //Display SnackBar
                var ans = await Application.Current.MainPage.DisplaySnackBarAsync(SnackBar);

                //Pulizia delle caselle
                var appoggio = new object[6] { args[0], args[1], args[2], args[3], args[4], args[5] };
                OnCancelCustom(appoggio);
            }
            else
            {
                //Gestione errori
                await ErrorsCheck(response);
            }
        }

        /* Aggiunta della task sul DBonline e DBlocale.
         * Può contenere campi modificati dall'utente.
         * Dopo salvataggio effettuato con successo, 
         * chiusura expander contenente la task 
         * e resettati tutti i campi con l'invocazione di OnCancelButton. */
        private async Task OnSavingAsync(object[] args)
        {
            //Ricreo la UserTask
            UserTaskClass task = new UserTaskClass
            {
                Name = (string)args[0],
                Category = (string)args[1],
                Description = (args[2] as Entry).Text,
                Verified = false,
            };

            //Gestione della data
            string d = DateTime.Parse((args[3] as DatePicker).Date.ToString()).ToString("dd/MM/yyyy");
            string h = DateTime.Parse((args[4] as TimePicker).Time.ToString()).ToString("HH:mm");
            string orario = d + " " + h;
            task.Date = DateTime.ParseExact(orario, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

            //Gestione dello user
            if ((args[6] as Picker).SelectedIndex == -1)
            {
                task.User = ClassGlobal.ActualUser.Username;
            }
            else
            {
                task.User = ClassGlobal.UserList4Picker[(args[6] as Picker).SelectedIndex].Username;
            }

            //Gestione ActivityIndicator
            ActivityRunning = true;

            //Provo a salvare su DBs
            int response = await NewTaskPage.AddTaskDBs(task);

            //Gestione ActivityIndicator
            ActivityRunning = false;

            //Verifica operazione salvataggio su DBs
            if (response == 200)
            {
                //Utente vuole tornare sulla Home
                var Action = new SnackBarActionOptions
                {
                    Action = async () =>
                    {
                        //Pulizia delle caselle
                        var tmp = new object[5] { args[2], args[3], args[4], args[5], args[6] };
                        OnCancelButton(tmp);

                        await Shell.Current.GoToAsync("//main/MainTaskPage");
                    },
                    Text = "Home",
                    ForegroundColor = Color.White,
                };

                //Snackbar da mostrare in fondo alla pagina
                var SnackBar = new SnackBarOptions
                {
                    MessageOptions = new MessageOptions
                    {
                        Foreground = Color.White,
                        Message = "Compito Inviato!",
                    },
                    BackgroundColor = Color.FromHex("#0047AB"),
                    Duration = TimeSpan.FromSeconds(2),
                    Actions = new[] { Action },
                    CornerRadius = 10,
                };

                //Display SnackBar
                var ans = await Application.Current.MainPage.DisplaySnackBarAsync(SnackBar);

                //Pulizia delle caselle
                var appoggio = new object[5] { args[2], args[3], args[4], args[5], args[6] };
                OnCancelButton(appoggio);
            }
            else
            {
                //Gestione errori
                await ErrorsCheck(response);
            }
        }

        /* Gestione degli errori invio task su DBs.
         * Mostrano SnackBar adeguata.*/
        private static async Task ErrorsCheck(int code)
        {
            // Compito già presente
            if (code == 422)
            {
                //Snackbar da mostrare in fondo alla pagina
                var SnackBar = new SnackBarOptions
                {
                    MessageOptions = new MessageOptions
                    {
                        Foreground = Color.White,
                        Message = "Compito già presente!",
                    },
                    BackgroundColor = Color.Red,
                    Duration = TimeSpan.FromSeconds(3),
                    CornerRadius = 10,
                };

                //Display SnackBar
                await Application.Current.MainPage.DisplaySnackBarAsync(SnackBar);
            }
            // Errore generico
            else
            {
                //Snackbar da mostrare in fondo alla pagina
                var SnackBar = new SnackBarOptions
                {
                    MessageOptions = new MessageOptions
                    {
                        Foreground = Color.White,
                        Message = "Problemi con l'invio del compito!",
                    },
                    BackgroundColor = Color.Red,
                    Duration = TimeSpan.FromSeconds(3),
                    CornerRadius = 10,
                };

                //Display SnackBar
                await Application.Current.MainPage.DisplaySnackBarAsync(SnackBar);
            }
        }
    }
}
