using TimelineForms.Common;
using TimelineForms.Services;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelineForms.Extensions
{
    public static class UserServiceExtensions
    {
        public static async Task<bool> EnsureLoggedInAsync(this IUserService userService)
        {
            if (!await userService.TryAutoLoginAsync())
            {
                await Task.Delay(10);

                var navigationService = ServiceLocator.Current.GetInstance<NavigationService>();
                navigationService.NavigateTo(Constants.LoginPage, HistoryBehavior.ClearHistory);

                return false;
            }

            return true;
        }
    }
}
