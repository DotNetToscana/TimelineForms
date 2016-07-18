using TimelineForms.Common;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TimelineForms.Services
{
    public enum HistoryBehavior
    {
        Default,
        ClearHistory
    }

    public class NavigationService : INavigationService
    {
        private Dictionary<string, Type> pages { get; } = new Dictionary<string, Type>();

        public string CurrentPageKey { get; private set; }

        public Page MainPage => Application.Current.MainPage;

        public void Configure(string key, Type pageType) => pages.Add(key, pageType);

        #region INavigationService implementation

        public void GoBack()
        {
            if (MainPage.Navigation.ModalStack.Count > 0)
                MainPage.Navigation.PopModalAsync();
            else
                MainPage.Navigation.PopAsync();
        }

        public void NavigateTo(string pageKey) => NavigateTo(pageKey, null);

        public void NavigateTo(string pageKey, object parameter) => NavigateTo(pageKey, parameter, HistoryBehavior.Default);

        public void NavigateTo(string pageKey, HistoryBehavior historyBehavior) => NavigateTo(pageKey, null, historyBehavior);

        public void NavigateTo(string pageKey, object parameter, HistoryBehavior historyBehavior)
        {
            Type pageType;
            if (pages.TryGetValue(pageKey, out pageType))
            {
                var displayPage = (Page)Activator.CreateInstance(pageType);
                CurrentPageKey = pageKey;

                if (historyBehavior == HistoryBehavior.ClearHistory)
                {
                    displayPage.SetNavigationArgs(parameter);
                    MainPage.Navigation.InsertPageBefore(displayPage, MainPage.Navigation.NavigationStack[0]);

                    // Since we want to clear history, removes all the other pages from the navigation stack.
                    var existingPages = MainPage.Navigation.NavigationStack.ToList();
                    for (int i = 1; i < existingPages.Count; i++)
                        MainPage.Navigation.RemovePage(existingPages[i]);
                }
                else
                {
                    MainPage.Navigation.PushAsync(displayPage, parameter, animated: true);
                }
            }
            else
            {
                throw new ArgumentException(
                          $"No such page: {pageKey}. Did you forget to call NavigationService.Configure?",
                          nameof(pageKey));
            }
        }

        #endregion
    }
}
