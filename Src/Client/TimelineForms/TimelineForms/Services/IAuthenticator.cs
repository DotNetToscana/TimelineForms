using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelineForms.Services
{
    /// <summary>
    /// https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-xamarin-forms-get-started-users
    /// </summary>
    public interface IAuthenticator
    {
        Task<MobileServiceUser> LoginAsync();

        Task LogoutAsync();
    }
}
