using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace timelineformsService.Extensions
{
    public static class UserExtensions
    {
        public static string GetUserId(this IPrincipal user)
            => (user.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
    }
}