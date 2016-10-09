using Acr.UserDialogs;
using TimelineForms.Common;
using TimelineForms.Services;
using TimelineForms.Views;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Globalization;

namespace TimelineForms.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var navigationService = new NavigationService();
            navigationService.Configure(Constants.LoginPage, typeof(LoginPage));
            navigationService.Configure(Constants.HomePage, typeof(HomePage));
            navigationService.Configure(Constants.WritePostPage, typeof(WritePostPage));
            navigationService.Configure(Constants.PostDetailsPage, typeof(PostDetailsPage));
            SimpleIoc.Default.Register<NavigationService>(() => navigationService);

            SimpleIoc.Default.Register<IMobileServiceClient>(() =>
            {
                var client = new MobileServiceClient(MobileAppConstants.ServiceUrl);
                return client;
            });

            SimpleIoc.Default.Register<IUserService, Services.UserService>();
            SimpleIoc.Default.Register<ITimelineService, Services.TimelineService>();

            SimpleIoc.Default.Register<IUserDialogs>(() => UserDialogs.Instance);

            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<HomeViewModel>();
            SimpleIoc.Default.Register<WritePostViewModel>();
            SimpleIoc.Default.Register<PostDetailsViewModel>();
        }

        public LoginViewModel LoginViewModel => ServiceLocator.Current.GetInstance<LoginViewModel>();

        public HomeViewModel HomeViewModel => ServiceLocator.Current.GetInstance<HomeViewModel>();

        public WritePostViewModel WritePostViewModel => ServiceLocator.Current.GetInstance<WritePostViewModel>();

        public PostDetailsViewModel PostDetailsViewModel => ServiceLocator.Current.GetInstance<PostDetailsViewModel>();
    }
}
