using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace m.transport.Android.alpha.Broadcast
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Log.Info("Alarm", "alarm triggered");
            runLocationCheckAsync();

            var intentForRepeat = new Intent(context, typeof(AlarmReceiver));
            var source = PendingIntent.GetBroadcast(context, 0, intentForRepeat, 0);
            var am = (AlarmManager) global::Android.App.Application.Context.GetSystemService(Context.AlarmService);
            am.SetExactAndAllowWhileIdle(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime() + (1000 * 60 * 5), source);
        }

        private async System.Threading.Tasks.Task runLocationCheckAsync()
        {
            //await transport.LocationManager.Instance.CheckLocation();
        }
    }
}
