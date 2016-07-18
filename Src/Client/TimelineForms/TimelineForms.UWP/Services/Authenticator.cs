using TimelineForms.Services;
using TimelineForms.UWP.Services;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Authenticator))]
namespace TimelineForms.UWP.Services
{
    public class Authenticator : IAuthenticator
    {
        private readonly IMobileServiceClient client;

        public Authenticator()
        {
            client = ServiceLocator.Current.GetInstance<IMobileServiceClient>();
        }

        public Task<MobileServiceUser> LoginAsync() =>
            client.LoginAsync(MobileServiceAuthenticationProvider.Facebook);

        public Task LogoutAsync() => client.LogoutAsync();
    }
}
