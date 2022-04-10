using Desperate_Houseworks.Models;
using Desperate_Houseworks.Services;
using Desperate_Houseworks.ViewModels;
using Desperate_Houseworks.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Desperate_Houseworks.ViewModels
{
    /* ViewModel del PopUp che permette di selezionare le icone del profilo.*/
    internal class IconPickerPopupViewModel : BaseViewModel
    {
        private readonly Popup Popup;
        public UserIcon Selected { get; set; }
        public ObservableCollection<UserIcon> IconList { get; set; }

        public Command SaveCommand => new Command(() => Save());

        public IconPickerPopupViewModel(Popup popup)
        {
            Popup = popup;
            InitializeObservableCollection();
        }

        /* Caricamento dell'ObservableCollection contenente le icone del profilo.*/
        private void InitializeObservableCollection()
        {
            IconList = new ObservableCollection<UserIcon>();
            List<string> lista = FamilyMethods.GetEveryUserPic();

            foreach (string item in lista)
            {
                UserIcon tmp = new UserIcon { Icon = item };
                IconList.Add(tmp);

                //Selezione dell'icona attiva dell'utente
                if (tmp.ToString().Equals(FamilyMethods.MemberInt2Pic(ClassGlobal.ActualUser.Picture)))
                {
                    Selected = tmp;
                }
            }
        }

        /* Salvataggio dell'icona selezionata dall'utente.*/
        private async void Save()
        {
            //Controllo per sicurezza
            if (Selected != null)
            {
                string icon = Selected.Icon;
                if (!icon.Equals(ClassGlobal.ActualUser.Picture))
                {
                    //Chiamata al DBonline
                    int code = await IconPickerPopup.ChangeUserPicFromDBS(icon);

                    if (code != 200)
                    {
                        await Shell.Current.DisplayAlert("Errore",
                            "L'icona non è stata aggiornata.\nRiprova più tardi", "OK");
                    }
                }
                Popup.Dismiss(icon);
            }
        }
    }

    /* Classe per utilizzare ObservableCollection.*/
    public class UserIcon
    {
        public string Icon { get; set; }
        public override string ToString()
        {
            return Icon;
        }
    }
}
