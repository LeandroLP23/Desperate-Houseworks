using Desperate_Houseworks.Views;
using Xamarin.Forms;

namespace Desperate_Houseworks
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(route: "login", typeof(LoginPage));
            Routing.RegisterRoute(route: "registration", typeof(RegisterPage));
            Routing.RegisterRoute(route: "family", typeof(FamilyPage));
            Routing.RegisterRoute(route: "nofamily", typeof(NoFamilyPage));
            Routing.RegisterRoute(route: "password1", typeof(ResetPassword1Page));
            Routing.RegisterRoute(route: "password2", typeof(ResetPassword2Page));
            Routing.RegisterRoute(route: "password3", typeof(ResetPassword3Page));
        }
    }
}
