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

        public LoginViewModel(IUserService userService)
        {
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
                var isLogged = await userService.LoginAsync();
                this.CheckLoginResult(isLogged);
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
            IsBusy = true;

            // Checks if a logging-in operation is already in progress.
            if (!isLoggingIn)
            {
                try
                {
                    var isLogged = await userService.TryAutoLoginAsync();
                    this.CheckLoginResult(isLogged);
                }
                finally
                {
                    IsBusy = false;
                }
            }

            base.Activate(parameter);
        }

        private void CheckLoginResult(bool isLogged)
        {
            if (isLogged)
                NavigationService.NavigateTo(Constants.HomePage, HistoryBehavior.ClearHistory);
        }

        protected override void OnIsBusyChanged()
        {
            RaisePropertyChanged(() => CanLogin);
            base.OnIsBusyChanged();
        }
    }
}
