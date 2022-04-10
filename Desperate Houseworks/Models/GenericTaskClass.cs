using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.UI.Views;

namespace Desperate_Houseworks.Models
{
    /* Classe contenente la task generica, serve per gestire il DB locale
     * contenente l'elenco delle possibili task che un utente può decidere di voler svolgere.
     * (Si parla delle task presenti della schermata NewTaskPage)*/
    public class GenericTaskClass : AbTaskClass
    {
    }

    /* Convertitore per il passaggio dei parametri per creare 
     * o cancellare una task personalizzata. */
    public class ConvertitoreParametriCustom : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null && values[1] != null && values[2] != null && values[3] != null &&
                values[4] != null && values[5] != null && values.Length == 6)
            {
                Entry CustEntry1 = values[0] as Entry;
                Entry CustEntry2 = values[1] as Entry;
                DatePicker CustDate = values[2] as DatePicker;
                TimePicker CustTime = values[3] as TimePicker;
                Picker CustCategPicker = values[4] as Picker;
                Picker CustUserPicker = values[5] as Picker;
                return new object[6] { CustEntry1, CustEntry2, CustDate, CustTime, CustCategPicker, CustUserPicker };
            }
            return null;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /* Convertitore per il passaggio dei parametri per resettare descrizione, data e orario
     * della task da creare */
    public class ConvertitoreParametriCancelButton : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null && values[1] != null && values[2] != null && values[3] != null &&
                values[4] != null && values.Length == 5)
            {
                Entry MyEntry = values[0] as Entry;
                DatePicker MyDate = values[1] as DatePicker;
                TimePicker MyTime = values[2] as TimePicker;
                Expander MyExpander = values[3] as Expander;
                Picker MyUserPicker = values[4] as Picker;
                return new object[5] { MyEntry, MyDate, MyTime, MyExpander, MyUserPicker };
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /* Convertitore per il passaggio dei parametri per salvare la task 
     * con tutti i campi presenti. */
    public class ConvertitoreParametriSaving : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null && values[1] != null && values[2] != null && values[3] != null &&
                values[4] != null && values[5] != null && values[6] != null && values.Length == 7)
            {
                string Name = (values[0] as Label).Text;
                string Categ = (values[1] as Label).Text;
                Entry MyEntry = values[2] as Entry;
                DatePicker MyDate = values[3] as DatePicker;
                TimePicker MyTime = values[4] as TimePicker;
                Expander MyExpander = values[5] as Expander;
                Picker MyUserPicker = values[6] as Picker;
                return new object[7] { Name, Categ, MyEntry, MyDate, MyTime, MyExpander, MyUserPicker };
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
