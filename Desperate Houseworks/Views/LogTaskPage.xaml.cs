using Desperate_Houseworks.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Desperate_Houseworks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /* Pagina contenente i log della famiglia per intero.*/
    public partial class LogTaskPage : ContentPage
    {
        public static string filtroLog = "Giornate";
        public LogTaskPage()
        {
            InitializeComponent();
            BindingContext = new LogTaskViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Task.Run(() => ((LogTaskViewModel)BindingContext).RefreshLogs());
        }

        /* Pressione su frame per aprire il datepicker della data iniziale.*/
        private void FrameDateBegins_Tapped(object sender, EventArgs e)
        {
            PickerBeginsDate.Focus();
        }

        /* Pressione su frame per aprire il datepicker della data finale.*/
        private void FrameDateEnds_Tapped(object sender, EventArgs e)
        {
            PickerEndsDate.Focus();
        }

        /* Perdita focus dal calendario della data iniziale.*/
        private void PickerBeginsDate_DateSelected(object sender, DateChangedEventArgs e)
        {
            PickerBeginsDate.Unfocus();
        }

        /* Perdita focus dal calendario della data finale.*/
        private void PickerEndsDate_DateSelected(object sender, DateChangedEventArgs e)
        {
            PickerEndsDate.Unfocus();
        }

    }
}
