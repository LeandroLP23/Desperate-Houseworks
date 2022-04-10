using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using Desperate_Houseworks.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Desperate_Houseworks.ViewModels
{
    public class LogTaskViewModel : BaseViewModel
    {

        public List<LogView> LogAppoggio;
        public ObservableCollection<Grouping<string, LogView>> listaLog = new ObservableCollection<Grouping<string, LogView>>();
        public ObservableCollection<Grouping<string, LogView>> ListaLog
        {
            get => listaLog;
            set
            {
                if (listaLog != value)
                {
                    listaLog = value;
                    OnPropertyChanged();
                }
            }
        }

        public string testoFiltro = "Giornate";
        public string TestoFiltro
        {
            get => testoFiltro;
            set
            {
                if (testoFiltro != value)
                {
                    testoFiltro = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime periodBegins;
        public DateTime PeriodBegins
        {
            get => periodBegins;
            set
            {
                if (periodBegins != value)
                {

                    periodBegins = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime periodEnds;
        public DateTime PeriodEnds
        {
            get => periodEnds;
            set
            {
                if (periodEnds != value)
                {

                    periodEnds = value;
                    OnPropertyChanged();
                }
            }
        }
        public Command ChangeFilterCommand => new Command(() => ChangeFilter());
        public AsyncCommand RefreshLogsCommand => new AsyncCommand(() => RefreshLogs());

        public LogTaskViewModel()
        {
            PeriodBegins = DateTime.Today;
            PeriodEnds = DateTime.Today;
        }

        public LogTaskViewModel(DateTime periodBegins, DateTime periodEnds)
        {
            PeriodBegins = periodBegins;
            PeriodEnds = periodEnds;
        }

        /* Modifica del filtro del grouping della CollectionView.*/
        private void ChangeFilter()
        {
            if (LogTaskPage.filtroLog.Equals("Giornate"))
            {
                LogTaskPage.filtroLog = "Svolte";
                TestoFiltro = "Svolte";
                FiltroVerifica();
            }
            else
            {
                LogTaskPage.filtroLog = "Giornate";
                TestoFiltro = "Giornate";
                FiltroGiornate();
            }
        }

        /* Richiesta log dal DBonline, il periodo di tempo è quello salvato nelle proprietà.
         * In caso non l'utente non fosse loggato, non vengono mostrati i log.
         * La lista di Log viene raggruppata a seconda della richiesta.*/
        public async Task RefreshLogs()
        {
            IsBusy = true;

            //Se non sei loggato non puoi vedere Log
            if (!ClassGlobal.isLogged)
            {
                IsBusy = false;
                return;
            }

            //Richiesta dei log dal DBonline
            LogAppoggio = await DesperateDB.GetLog(PeriodBegins, PeriodEnds);
            
            IsBusy = false;

            //Conto il numero di elementi presenti nella lista
            if (LogAppoggio.Count == 0)
            {
                //Lista vuota
                ListaLog = new ObservableCollection<Grouping<string, LogView>>();
                return;
            }

            //Gestione raggruppamenti della lista
            if (LogTaskPage.filtroLog.Equals("Giornate"))
            {
                FiltroGiornate();
            }
            else
            {
                FiltroVerifica();
            }

        }

        /* Raggruppamento della lista LogAppoggio secondo la data.*/
        private void FiltroGiornate()
        {
            //Raggruppamento per Data
            var sorted = from item in LogAppoggio
                         group item by item.Date.ToString("dd/MM/yyyy") into itemGroup
                         select new Grouping<string, LogView>(itemGroup.Key, itemGroup);

            //Aggiornamento dei gruppi
            ListaLog = new ObservableCollection<Grouping<string, LogView>>(sorted);
        }

        /* Raggruppamento della lista LogAppoggio secondo la verifica.*/
        private void FiltroVerifica()
        {
            //Raggruppamento per Verificato
            var sorted = from item in LogAppoggio
                         orderby (item.Date.ToString("dd/MM/yyyy")) descending, item.Verified
                         group item by item.Date.ToString("dd/MM/yyyy") into itemGroup
                         select new Grouping<string, LogView>(itemGroup.Key, itemGroup);

            //Aggiornamento dei gruppi
            ListaLog = new ObservableCollection<Grouping<string, LogView>>(sorted);
        }
    }
}
