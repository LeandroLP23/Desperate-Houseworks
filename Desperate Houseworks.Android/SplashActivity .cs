using Android.App;
using Android.Content;
using Android.OS;
using System.Threading.Tasks;

namespace Desperate_Houseworks.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        // Avvia la task di StartUp
        protected override void OnResume()
        {
            base.OnResume();
            Task.Run(() => { StartActivity(new Intent(Application.Context, typeof(MainActivity))); });
        }

        public override void OnBackPressed() { }
    }
}