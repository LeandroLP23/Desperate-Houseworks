using Plugin.FirebasePushNotification;
using Desperate_Houseworks.Services;
using System;
using System.IO;
using Xamarin.Forms;
using System.Threading;

namespace Desperate_Houseworks
{
    public partial class App : Application
    {
        public static string notificationToken { get => CrossFirebasePushNotification.Current.Token; }

        private static TaskDatabase dbListaTasks;
        private static UserDatabase dbListaUsers;

        public static TaskDatabase Database
        {
            get
            {
                //Verifico se il database è gia stato recuperato
                if (dbListaTasks == null)
                {
                    //recupero il database dalla memoria
                    dbListaTasks = new TaskDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tasks.db3"));
                }
                return dbListaTasks;
            }
        }

        public static UserDatabase DatabaseUser
        {
            get
            {
                //Verifico se il database è gia stato recuperato
                if (dbListaUsers == null)
                {
                    //recupero il database dalla memoria
                    dbListaUsers = new UserDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tasks.db3"));
                }
                return dbListaUsers;
            }
        }

        public App()
        {
            //CrossFirebasePushNotification.Current.OnTokenRefresh += Current_OnTokenRefresh;
            InitializeComponent();
            Database.InitGenericTaskDb();

            new Thread(async () => ClassGlobal.ActualUser = await DatabaseUser.GetUser()).Start();

            MainPage = new AppShell();
        }

        /*private void Current_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Token: {e.Token}");
        } 
        */

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
