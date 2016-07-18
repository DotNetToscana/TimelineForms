using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Gcm;
using Android.Util;
using Android.Media;
using Android.Support.V4.App;

namespace TimelineForms.Droid.Services
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class MyGcmListenerService : GcmListenerService
    {
        public override void OnMessageReceived(string from, Bundle data)
        {
            var action = data.GetString("action");
            var title = data.GetString("title");
            var message = data.GetString("message");
            var image = data.GetString("image");

            this.SendNotification(action, title, message, image);
        }

        private void SendNotification(string action, string title, string message, string image)
        {
            //Create notification
            var notificationManager = GetSystemService(Context.NotificationService) as Android.App.NotificationManager;

            //Create an intent to show UI
            var uiIntent = new Intent(this, typeof(SplashActivity));
            uiIntent.PutExtra("action", action);
            var pendingIntent = PendingIntent.GetActivity(this, 0, uiIntent, PendingIntentFlags.OneShot);

            //Use Notification Builder
            var builder = new NotificationCompat.Builder(this);

            //Create the notification
            //we use the pending intent, passing our UI intent over which will get called
            //when the notification is tapped.
            var notification = builder.SetContentIntent(pendingIntent)
                    .SetSmallIcon(Resource.Drawable.icon)
                    .SetTicker(title)
                    .SetContentTitle(title)
                    .SetContentText(message)

                    //Set the notification sound
                    .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))

                    //Auto cancel will remove the notification once the user touches it
                    .SetAutoCancel(true).Build();

            //Show the notification
            notificationManager.Notify(1, notification);
        }
    }
}