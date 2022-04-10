using Android.App;
using Android.OS;
using Android.Runtime;
using Firebase;
using Plugin.FirebasePushNotification;
using Plugin.LocalNotification;
using System;

namespace MainApplication
{

    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            //Set the default notification channel for your app when running Android Oreo
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                //Change for your default notification channel id here
                FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";

                //Change for your default notification channel name here
                FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
            }
            
            //If debug you should reset the token each time.
#if DEBUG
            FirebasePushNotificationManager.Initialize(this, true);
#else
            FirebasePushNotificationManager.Initialize(this,false);
#endif

            //Handle notification when app is closed here
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                CrossFirebasePushNotification.Current.ClearAllNotifications();
                p.Data.TryGetValue("scheduledTime", out object Date);
                p.Data.TryGetValue("name", out object Name);
                if (Date == null || Name == null)
                {
                    return;
                }
                DateTime data = DateTime.Parse(Date.ToString());
                string name = Name.ToString();

                NotificationRequest notification = new NotificationRequest
                {
                    BadgeNumber = 1,
                    Title = "Desperate HouseWorks",
                    Image = new NotificationImage()
                    {
                        FilePath = "icon_notifiche.png"
                    },
                    Schedule =
                    {
                        NotifyTime = data
                    },
                    Description = "E' il momento di fare " + name + "!",
                    NotificationId = 1337,
                };
                NotificationCenter.Current.Show(notification).Wait();
            };

            //Handles the tap on the Notification.
            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Opened");
                foreach (var data in p.Data)
                {
                    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                }
            };
        }
    }
}