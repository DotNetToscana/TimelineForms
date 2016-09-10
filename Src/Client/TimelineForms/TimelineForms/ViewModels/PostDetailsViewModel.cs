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

namespace TimelineForms.ViewModels
{
    public class PostDetailsViewModel : ViewModelBase
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

        private Post post;
        public Post Post
        {
            get { return post; }
            set { this.Set(ref post, value); }
        }

        public User User => userService.CurrentUser;

        public AutoRelayCommand RefreshCommand { get; set; }

        public AutoRelayCommand SendCommand { get; set; }

        public PostDetailsViewModel(IMobileServiceClient client, IUserService userService, ITimelineService timelineService)
        {
            this.client = client;
            this.userService = userService;
            this.timelineService = timelineService;

            this.CreateCommands();
        }

        private void CreateCommands()
        {
            RefreshCommand = new AutoRelayCommand(async () => await RefreshAsync(),
                () => !IsBusy).DependsOn(() => IsBusy);

            SendCommand = new AutoRelayCommand(async () => await SendAsync(),
                () => !string.IsNullOrWhiteSpace(message) && !IsBusy).DependsOn(() => Message).DependsOn(() => IsBusy);
        }

        private async Task RefreshAsync()
        {
            IsBusy = true;

            try
            {
                await timelineService.LoadCommentsOfAsync(Post);
                RaisePropertyChanged(() => Post);
            }
            catch
            {
                await DialogService.AlertAsync("An error occurred while refreshing comments.");
            }
            finally
            {
                MessengerInstance.Send(new NotificationMessage(Constants.RefreshCompleted));
                IsBusy = false;
            }
        }

        private async Task SendAsync()
        {
            IsBusy = true;

            try
            {
                // Sends the comment to the mobile app.
                await timelineService.SendCommentAsync(post, message);
                Message = null;
            }
            catch
            {
                await DialogService.AlertAsync("An error occurred while sending the comment.");
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

            try
            {
                var postId = parameter.ToString();
                Post = await timelineService.GetPostAsync(postId);

                if (Post == null)
                {
                    NavigationService.GoBack();
                    return;
                }
            }
            catch
            {
                await DialogService.AlertAsync("An error occurred while getting post details.");
                NavigationService.GoBack();
            }
            finally
            {
                IsBusy = false;
            }

            base.Activate(parameter);
        }

        public override void Deactivate()
        {
            Post = null;
            base.Deactivate();
        }
    }
}
