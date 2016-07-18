using TimelineForms.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelineForms.Services
{
    public interface IUserService
    {
        bool IsAuthenticated { get; }

        User CurrentUser { get; }

        Task<bool> TryAutoLoginAsync();

        Task<bool> LoginAsync();

        Task LogoutAsync();
    }
}
