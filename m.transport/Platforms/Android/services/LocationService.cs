
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Util;
using Android.Support.V4.App;
using static Android.OS.PowerManager;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport.Android.alpha.services
{
    [Service(Label = "Service")]
    [IntentFilter(new String[] { "com.yourname.Service" })]
    public class LocationService : Service
    {
        IBinder binder;
        public const int FORSERVICE_NOTIFICATION_ID = 10000;
        public const string MAIN_ACTIVITY_ACTION = "Main_activity";
        public const string PUT_EXTRA = "has_service_been_started";
        private PowerManager.WakeLock wakeLoc;


        public override StartCommandResult OnStartCommand(global::Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            registerForService();

            StartServiceInForeground();
            return StartCommandResult.Sticky;
        }

        private void registerForService()
        {
            if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
            {
                var channel = new NotificationChannel("my channel", "my service", global::Android.App.NotificationImportance.Default);
                NotificationManager manager = (NotificationManager)GetSystemService(Context.NotificationService);
                manager.CreateNotificationChannel(channel);
            }

            var notification = new NotificationCompat.Builder(this)
                .SetContentTitle("etc")
                .SetContentText("etc")
                .SetContentIntent(BuildIntentToShowMainActivity())
                .SetChannelId("my channel")
                .SetOngoing(true)
                .Build();

            // Enlist this instance of the service as a foreground service, MUST CALL IN < 5 SECONDS ON RUNTIME
            StartForeground(FORSERVICE_NOTIFICATION_ID, notification);
        }

        PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction(MAIN_ACTIVITY_ACTION);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(PUT_EXTRA, true);

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        private void StartServiceInForeground()
        {
            PowerManager pManager = ((PowerManager)(Forms.Context).GetSystemService(Context.PowerService));
            wakeLoc = pManager.NewWakeLock(WakeLockFlags.Partial, "GPS::Check");
            wakeLoc.Acquire();

            System.Timers.Timer Timer1 = new System.Timers.Timer();
            Timer1.Start();

            Timer1.Interval = GetCheckInterval();


            Timer1.Enabled = true;
            //Timer1.Elapsed += OnTimedEvent;
            Timer1.Elapsed += async (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                if (transport.LocationManager.CheckTime != DateTime.MinValue)
                {
                    long duration = (transport.LocationManager.CheckTime - DateTime.Now).Ticks / TimeSpan.TicksPerMillisecond;
                    Console.WriteLine("Next Location check: " + duration);
                    transport.LocationManager.CheckTime = DateTime.MinValue;
                    Timer1.Interval = duration;
                } else
                {
                    await transport.LocationManager.Instance.ReportLocation();
                    Timer1.Interval = GetCheckInterval();
                }
            };
            Timer1.Start();
        }

        private long GetCheckInterval()
        {
#if DEBUG
            // 1 min
            Console.WriteLine("Next check: 5 min");
            return 300000L;
#else
            // 5 min
            return 300000L;
#endif
        }
    }

    public class ServiceBinder : Binder
    {
        readonly Service service;

        public ServiceBinder(Service service)
        {
            this.service = service;
        }

        public Service GetService()
        {
            return service;
        }
    }
}
