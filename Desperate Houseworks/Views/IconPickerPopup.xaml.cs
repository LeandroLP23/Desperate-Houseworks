using Desperate_Houseworks.Models;
using Desperate_Houseworks.ViewModels;
using Desperate_Houseworks.Services;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Desperate_Houseworks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IconPickerPopup : ContentView
    {
        public IconPickerPopup(Popup popup)
        {
            InitializeComponent();
            BindingContext = new IconPickerPopupViewModel(popup);
        }

        /* Modifica dell'immagine del profilo dell'utente.*/
        public static async Task<int> ChangeUserPicFromDBS (string icon){
            int code = await DesperateDB.UpdateUserIcon(FamilyMethods.MemberPic2Int(icon));
            if (code == 200)
            {
                ClassGlobal.ActualUser.Picture = FamilyMethods.MemberPic2Int(icon);
                await App.DatabaseUser.SetUser(ClassGlobal.ActualUser);
            }
            return code;
        }
    }
}