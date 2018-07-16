using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using FixTricks.Droid.Receivers;
using Xamarin.Forms;
using Android.Widget;

namespace FixTricks.Droid
{
    [Activity(Label = "ИЭФ.Учёба", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            MessagingCenter.Subscribe<object, string>(this, "SendNotify", (s, e) => {
                Notification.Builder builder = new Notification.Builder(this)
                    .SetContentTitle("ИЭФ.Учёба")
                    .SetContentText(e)
                    .SetSmallIcon(Resource.Drawable.rates);

                // Build the notification:
                Notification notification = builder.Build();

                // Get the notification manager:
                NotificationManager notificationManager =
                    GetSystemService(Context.NotificationService) as NotificationManager;

                // Publish the notification:
                const int notificationId = 0;
                notificationManager.Notify(notificationId, notification);
            });

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
            
            AlarmManager man = (AlarmManager)GetSystemService(Context.AlarmService);
            Intent myIntent = new Intent(this, typeof(MessagingReceiver));
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, 0, myIntent, 0);
            man.SetRepeating(AlarmType.Rtc, SystemClock.ElapsedRealtime()+3000,60*1000,pendingIntent);
            
        }
        
    }
}

