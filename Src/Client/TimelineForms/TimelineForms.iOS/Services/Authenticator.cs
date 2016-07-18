using TimelineForms.iOS.Services;
using TimelineForms.Services;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(Authenticator))]
namespace TimelineForms.iOS.Services
{
    public class Authenticator : IAuthenticator
    {
        private readonly IMobileServiceClient client;

        public Authenticator()
        {
            client = ServiceLocator.Current.GetInstance<IMobileServiceClient>();
        }

        public Task<MobileServiceUser> LoginAsync()
            => client.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController, MobileServiceAuthenticationProvider.Facebook);

        public Task LogoutAsync() => client.LogoutAsync();
    }
}
