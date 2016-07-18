using System;
using TimelineForms.Common;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Views;
using TimelineForms.Services;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace TimelineForms.ViewModels
{
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase, INavigable
    {
        public NavigationService NavigationService { get; }

        public IUserDialogs DialogService { get; }

        public ViewModelBase()
        {
            NavigationService = ServiceLocator.Current.GetInstance<NavigationService>();
            DialogService = ServiceLocator.Current.GetInstance<IUserDialogs>();

            IsConnected = Plugin.Connectivity.CrossConnectivity.Current.IsConnected;
            Plugin.Connectivity.CrossConnectivity.Current.ConnectivityChanged += (s, e) =>
            {
                Device.BeginInvokeOnMainThread(() => IsConnected = e.IsConnected);
            };
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (this.SetBusy(value) && !isBusy)
                    BusyMessage = null;
            }
        }

        private string busyMessage;
        public string BusyMessage
        {
            get { return busyMessage; }
            set { this.Set(ref busyMessage, value, broadcast: true); }
        }

        public bool SetBusy(bool value, string message = null)
        {
            BusyMessage = message;

            var isSet = this.Set(() => IsBusy, ref isBusy, value, broadcast: true);
            if (isSet)
                OnIsBusyChanged();

            return isSet;
        }

        private bool isConnected;
        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                if (this.Set(ref isConnected, value, broadcast: true))
                    OnNetworkAvailabilityChanged();
            }
        }

        protected virtual void OnIsBusyChanged() { }

        protected virtual async void OnNetworkAvailabilityChanged()
        {
            if (!IsConnected)
                await DialogService.AlertAsync("The device seems to be offline. Check your Internet connection.", "Network");
        }

        public virtual void Activate(object parameter) { }

        public virtual void Deactivate() { }
    }
}