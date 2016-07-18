using Android.Content;
using Android.Gms.Common;
using Android.Util;
using TimelineForms.Droid.Services;
using TimelineForms.Services;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotificationManager))]
namespace TimelineForms.Droid.Services
{
    public class NotificationManager : INotificationManager
    {
        private readonly IMobileServiceClient client;

        public NotificationManager()
        {
            client = ServiceLocator.Current.GetInstance<IMobileServiceClient>();
        }

        public Task RegisterAsync()
        {
            try
            {
                var intent = new Intent(MainActivity.Current, typeof(RegistrationIntentService));
                MainActivity.Current.StartService(intent);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(NotificationManager), $"Unable to start Intent for push notifications registration: {ex.Message}");
            }

            return Task.FromResult<object>(null);
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(MainActivity.Current);
            return resultCode == ConnectionResult.Success;
        }

        public Task UnregisterAsync() => client.GetPush().UnregisterAsync();
    }
}