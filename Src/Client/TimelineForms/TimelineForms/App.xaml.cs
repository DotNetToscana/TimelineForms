using TimelineForms.Services;
using TimelineForms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TimelineForms
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // The root page of your application
            var mainPage = new LoginPage();
            MainPage = new NavigationPage(mainPage);
        }

        protected override async void OnStart()
        {
            // If the app is launched one hour after its last stop, its navigation history
            // will be restored.
            await this.MainPage.Navigation.RestoreAsync(TimeSpan.FromHours(1));
        }

        protected override void OnSleep()
        {
            this.MainPage.Navigation.Store();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
