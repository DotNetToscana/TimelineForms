using TimelineForms.Common.Models;
using GalaSoft.MvvmLight;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TimelineForms.Services
{
    public class UserService : ObservableObject, IUserService
    {
        public bool IsAuthenticated => CurrentUser != null;

        private User currentUser;
        public User CurrentUser
        {
            get { return currentUser; }
            set
            {
                if (this.Set(ref currentUser, value))
                    RaisePropertyChanged(() => IsAuthenticated);
            }
        }

        private readonly IMobileServiceClient client;
        private readonly IAuthenticator authenticator;
        private readonly INotificationManager notificationManager;

        public UserService(IMobileServiceClient client)
        {
            this.client = client;
            this.authenticator = DependencyService.Get<IAuthenticator>();
            this.notificationManager = DependencyService.Get<INotificationManager>();
        }

        private async Task LoadUserAsync()
        {
            CurrentUser = null;
            CurrentUser = await client.InvokeApiAsync<User>("me", HttpMethod.Get, null);

            try
            {
                await notificationManager?.RegisterAsync();
            }
            catch { }
        }

        public async Task<bool> TryAutoLoginAsync()
        {
            if (Settings.IsLogged && client.CurrentUser == null)
            {
                // The user is already logged.
                var user = new MobileServiceUser(Settings.UserName);
                user.MobileServiceAuthenticationToken = Settings.Password;
                client.CurrentUser = user;

                try
                {
                    await this.LoadUserAsync();
                }
                catch
                {
                    // Loading user error: authentication needs to be reissued.
                    client.CurrentUser = null;
                    Settings.ClearCredential();
                }
            }

            return IsAuthenticated;
        }

        public async Task<bool> LoginAsync()
        {
            var user = await authenticator.LoginAsync();

            if (user != null)
            {
                Settings.SaveCredential(user.UserId, user.MobileServiceAuthenticationToken);
                await this.LoadUserAsync();
            }

            return IsAuthenticated;
        }

        public async Task LogoutAsync()
        {
            await notificationManager?.UnregisterAsync();
            await authenticator.LogoutAsync();

            Settings.ClearCredential();
            client.CurrentUser = null;
            CurrentUser = null;
        }
    }
}
