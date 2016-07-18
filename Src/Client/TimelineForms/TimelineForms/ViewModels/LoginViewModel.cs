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
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IMobileServiceClient client;
        private readonly IUserService userService;

        public AutoRelayCommand LoginCommand { get; set; }

        public bool CanLogin => !IsBusy && !userService.IsAuthenticated;

        public LoginViewModel(IMobileServiceClient client, IUserService userService)
        {
            this.client = client;
            this.userService = userService;

            this.CreateCommands();
        }

        private void CreateCommands()
        {
            LoginCommand = new AutoRelayCommand(async () => await LoginAsync(), () => CanLogin)
                .DependsOn(()=>IsBusy).DependsOn(() => CanLogin);
        }

        private async Task LoginAsync()
        {
            IsBusy = true;

            try
            {
                var logged = await userService.LoginAsync();
                if (logged)
                    NavigationService.NavigateTo(Constants.HomePage, HistoryBehavior.ClearHistory);
            }
            catch (InvalidOperationException)
            {
                /* Authentication was canceled by the user. */
                IsBusy = false;
            }
            catch
            {
                await DialogService.AlertAsync("An error occurred while logging in.");

                IsBusy = false;
            }
        }

        public override async void Activate(object parameter)
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

            base.Activate(parameter);
        }

        protected override void OnIsBusyChanged()
        {
            RaisePropertyChanged(() => CanLogin);
            base.OnIsBusyChanged();
        }
    }
}
