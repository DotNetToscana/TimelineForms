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

namespace TimelineForms.Services.Demo
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

        private Task LoadUserAsync()
        {
            CurrentUser = null;
            CurrentUser = new User
            {
                UserId = "sid:0123456789",
                FacebookUserId = "0123456789",
                FirstName = "Marco",
                LastName = "Minerva",
                EMail = "mail@mail.com",
                ImageUrl = "https://graph.facebook.com/10209897682705340/picture?width=150"
            };

            return Task.FromResult<object>(null);
        }

        public async Task<bool> TryAutoLoginAsync()
        {
            if (Settings.IsLogged)
            {
                // The user is already logged.
                await this.LoadUserAsync();
            }

            return IsAuthenticated;
        }

        public async Task<bool> LoginAsync()
        {
            Settings.SaveCredential("UserName", "AccessToken");
            await this.LoadUserAsync();

            return IsAuthenticated;
        }

        public Task LogoutAsync()
        {
            Settings.ClearCredential();
            CurrentUser = null;

            return Task.FromResult<object>(null);
        }
    }
}
