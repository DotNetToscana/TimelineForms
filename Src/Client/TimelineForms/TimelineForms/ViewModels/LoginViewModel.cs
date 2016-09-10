using TimelineForms.Common;
using TimelineForms.Common.Models;
using TimelineForms.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TimelineForms.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IMobileServiceClient client;
        private readonly IUserService userService;

        public AutoRelayCommand LoginCommand { get; set; }

        public bool CanLogin => !IsBusy && !IsLoggingIn && !userService.IsAuthenticated;

        private bool isLoggingIn;
        public bool IsLoggingIn
        {
            get { return isLoggingIn; }
            set
            {
                if (this.Set(ref isLoggingIn, value, broadcast: true))
                    RaisePropertyChanged(() => CanLogin);
            }
        }

        public LoginViewModel(IMobileServiceClient client, IUserService userService)
        {
            this.client = client;
            this.userService = userService;

            this.CreateCommands();
        }

        private void CreateCommands()
        {
            LoginCommand = new AutoRelayCommand(async () => await LoginAsync(), () => CanLogin)
                .DependsOn(() => CanLogin);
        }

        private async Task LoginAsync()
        {
            IsBusy = true;
            IsLoggingIn = true;

            try
            {
                var logged = await userService.LoginAsync();
                if (logged)
                    NavigationService.NavigateTo(Constants.HomePage, HistoryBehavior.ClearHistory);
            }
            catch (InvalidOperationException)
            {
                /* Authentication was canceled by the user. */
            }
            catch
            {
                await DialogService.AlertAsync("An error occurred while logging in.");
            }
            finally
            {
                IsBusy = false;
                IsLoggingIn = false;
            }
        }

        public override async void Activate(object parameter)
        {
            // Checks if a logging-in operation isn't already in progress.
            if (!isLoggingIn)
            {
                IsBusy = true;

                var logged = await userService.TryAutoLoginAsync();
                if (logged)
                {
                    NavigationService.NavigateTo(Constants.HomePage, HistoryBehavior.ClearHistory);
                }
                else
                {
                    IsBusy = false;
                }
            }

            base.Activate(parameter);
        }

        protected override void OnIsBusyChanged()
        {
            RaisePropertyChanged(() => CanLogin);
            base.OnIsBusyChanged();
        }
    }
}
