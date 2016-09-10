using TimelineForms.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using TimelineForms.Extensions;
using GalaSoft.MvvmLight.Messaging;

namespace TimelineForms.Views
{
    public partial class HomePage : ContentPageBase
    {
        public HomePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            Messenger.Default.Register<NotificationMessage>(this, msg =>
            {
                switch (msg.Notification)
                {
                    case Constants.RefreshCompleted:
                        postsListView.EndRefresh();
                        break;

                    default:
                        break;
                }
            });

            base.OnAppearing();
        }
    }
}
