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
using TimelineForms.Services;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;
using TimelineForms.Droid.Services;
using Xamarin.Forms;
using Android.Webkit;

[assembly: Dependency(typeof(Authenticator))]
namespace TimelineForms.Droid.Services
{
    public class Authenticator : IAuthenticator
    {
        private readonly IMobileServiceClient client;

        public Authenticator()
        {
            client = ServiceLocator.Current.GetInstance<IMobileServiceClient>();
        }

        public Task<MobileServiceUser> LoginAsync()
            => client.LoginAsync(MainActivity.Current, MobileServiceAuthenticationProvider.Facebook);

        public async Task LogoutAsync()
        {
            await client.LogoutAsync();
            this.ClearCookies();
        }

        private void ClearCookies()
        {
            var web = new Android.Webkit.WebView(MainActivity.Current);
            web.ClearCache(true);
            web.ClearHistory();
            web.Dispose();

#pragma warning disable 0618

            if (Build.VERSION.SdkInt >= BuildVersionCodes.LollipopMr1)
            {
                CookieManager.Instance.RemoveAllCookies(null);
                CookieManager.Instance.Flush();
            }
            else
            {
                var cookieSyncMngr = CookieSyncManager.CreateInstance(MainActivity.Current);
                cookieSyncMngr.StartSync();
                var cookieManager = CookieManager.Instance;
                cookieManager.RemoveAllCookie();
                cookieManager.RemoveSessionCookie();
                cookieSyncMngr.StopSync();
                cookieSyncMngr.Sync();
            }

#pragma warning restore 0618

        }
    }
}