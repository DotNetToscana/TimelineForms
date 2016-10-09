using timelineformsService.Extensions;
using timelineformsService.Models;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TimelineForms.Common.Models;

namespace timelineformsService.Controllers
{
    [MobileAppController]
    [Authorize]
    public class MeController : ApiController
    {
        public async Task<IHttpActionResult> GetMe()
        {
            //MobileAppSettingsDictionary settings = this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();
            //ITraceWriter traceWriter = this.Configuration.Services.GetTraceWriter();

            var userId = this.User.GetUserId();
            var creds = await this.User.GetAppServiceIdentityAsync<FacebookCredentials>(this.Request);

            string facebookUserId = null;
            string firstName = null;
            string lastName = null;
            string eMail = null;
            string imageUrl = null;
            string profileUrl = null;

            using (var db = new timelineformsContext())
            {
                var userDb = db.Users.FirstOrDefault(u => u.Id == userId);
                if (userDb == null)
                {
                    // Inserts the user into the database.
                    facebookUserId = creds.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                    firstName = creds.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName).Value;
                    lastName = creds.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Surname).Value;
                    eMail = creds.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                    imageUrl = $"https://graph.facebook.com/{facebookUserId}/picture?width=150";
                    profileUrl = $"https://www.facebook.com/app_scoped_user_id/{facebookUserId}/";

                    userDb = new DataObjects.User
                    {
                        Id = userId,
                        FacebookUserId = facebookUserId,
                        FirstName = firstName,
                        LastName = lastName,
                        EMail = eMail,
                        ImageUrl = imageUrl,
                        ProfileUrl = profileUrl,
                        Deleted = false
                    };

                    db.Users.Add(userDb);
                    await db.SaveChangesAsync();
                }
                else
                {
                    // The user already exists. Retrieves the information.
                    facebookUserId = userDb.FacebookUserId;
                    firstName = userDb.FirstName;
                    lastName = userDb.LastName;
                    eMail = userDb.EMail;
                    imageUrl = userDb.ImageUrl;
                    profileUrl = userDb.ProfileUrl;
                }
            }

            var user = new User
            {
                UserId = userId,
                FacebookUserId = facebookUserId,
                FirstName = firstName,
                LastName = lastName,
                EMail = eMail,
                ImageUrl = imageUrl,
                ProfileUrl = profileUrl
            };

            return Ok(user);
        }
    }
}
