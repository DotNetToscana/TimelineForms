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
using System.Reflection;

namespace TimelineForms.ViewModels
{
    public partial class HomeViewModel : ViewModelBase
    {
        private readonly IMobileServiceClient client;
        private readonly IUserService userService;
        private readonly ITimelineService timelineService;

        public AutoRelayCommand<Post> ItemTappedCommand { get; set; }

        public AutoRelayCommand RefreshCommand { get; set; }

        public AutoRelayCommand WritePostCommand { get; set; }

        public AutoRelayCommand LogoutCommand { get; set; }

        public ObservableCollection<TimelinePost> Posts => timelineService.Posts;

        public HomeViewModel(IMobileServiceClient client, IUserService userService, ITimelineService timelineService)
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

            ItemTappedCommand = new AutoRelayCommand<Post>((post) => NavigationService.NavigateTo(Constants.PostDetailsPage, post.PostId));

            WritePostCommand = new AutoRelayCommand(() => NavigationService.NavigateTo(Constants.WritePostPage),
                () => !IsBusy).DependsOn(() => IsBusy);

            LogoutCommand = new AutoRelayCommand(async () => await LogoutAsync());
        }

        private async Task RefreshAsync()
        {
            IsBusy = true;

            try
            {
                await timelineService.LoadAsync();
            }
            catch
            {
                await DialogService.AlertAsync("An error occurred while refreshing the timeline.");
            }
            finally
            {
                MessengerInstance.Send(new NotificationMessage(Constants.RefreshCompleted));
                IsBusy = false;
            }
        }

        private async Task LogoutAsync()
        {
            await userService.LogoutAsync();
            timelineService.Posts.Clear();
            NavigationService.NavigateTo(Constants.LoginPage, HistoryBehavior.ClearHistory);
        }

        public override async void Activate(object parameter)
        {
            IsBusy = true;

            if (!await userService.EnsureLoggedInAsync())
                return;

            if (!timelineService.IsLoaded)
                await this.RefreshAsync();

            IsBusy = false;
            base.Activate(parameter);
        }
    }
}
