using TimelineForms.Common;
using TimelineForms.Common.Models;
using TimelineForms.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using TimelineForms.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TimelineForms.ViewModels
{
    public partial class WritePostViewModel : ViewModelBase
    {
        private readonly IMobileServiceClient client;
        private readonly IUserService userService;
        private readonly ITimelineService timelineService;

        private string message;
        public string Message
        {
            get { return message; }
            set { this.Set(ref message, value, broadcast: true); }
        }

        public bool PostOnFacebook
        {
            get { return Settings.PostOnFacebook; }
            set
            {
                Settings.PostOnFacebook = value;
                RaisePropertyChanged();
            }
        }

        public User User => userService.CurrentUser;

        public AutoRelayCommand SendCommand { get; set; }

        public WritePostViewModel(IMobileServiceClient client, IUserService userService, ITimelineService timelineService)
        {
            this.client = client;
            this.userService = userService;
            this.timelineService = timelineService;

            this.CreateCommands();
        }

        private void CreateCommands()
        {
            SendCommand = new AutoRelayCommand(async () => await SendAsync(),
                () => !string.IsNullOrWhiteSpace(message) && !IsBusy).DependsOn(() => Message).DependsOn(() => IsBusy);
        }

        private async Task SendAsync()
        {
            IsBusy = true;

            try
            {
                // Sends the post to the mobile app.
                await timelineService.SendPostAsync(message, PostOnFacebook);
                Message = null;

                NavigationService.GoBack();
            }
            catch
            {
                await DialogService.AlertAsync("An error occurred while sending the post.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override async void Activate(object parameter)
        {
            IsBusy = true;

            if (!await userService.EnsureLoggedInAsync())
                return;

            // Ensures user's information is notificated to the UI.
            RaisePropertyChanged(() => User);

            if (!timelineService.IsLoaded)
                await timelineService.LoadAsync();

            IsBusy = false;
            base.Activate(parameter);
        }
    }
}
