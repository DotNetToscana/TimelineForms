using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using timelineformsService.DataObjects;
using timelineformsService.Models;
using timelineformsService.Filters;
using System.Security.Principal;
using Facebook;
using Microsoft.Azure.Mobile.Server.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using timelineformsService.Extensions;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;

namespace timelineformsService.Controllers
{
    [Authorize]
    public class PostController : TableController<Post>
    {
        private NotificationHubClient hubClient;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            timelineformsContext context = new timelineformsContext();
            DomainManager = new EntityDomainManager<Post>(context, Request);

            hubClient = this.GetHubNotificationClient();
        }

        // GET tables/Post/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [QueryableExpand("Sender")]
        [QueryableExpand("Comments/Sender")]
        public SingleResult<Post> GetPost(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Post/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Post> PatchPost(string id, Delta<Post> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Post
        public async Task<IHttpActionResult> PostPost(Post item)
        {
            // Adds the userId to the post.
            item.SenderId = this.User.GetUserId();
            Post current = await InsertAsync(item);

            if (item.PublishOnFacebook)
                await this.PostToFacebookAsync(item);

            await this.SendPushNotificationAsync(item);

            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Post/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePost(string id)
        {
            return DeleteAsync(id);
        }

        private async Task PostToFacebookAsync(Post item)
        {
            var creds = await this.User.GetAppServiceIdentityAsync<FacebookCredentials>(this.Request);

            // Posts the item to Facebook.
            var fb = new FacebookClient(creds.AccessToken);

            var parameters = new Dictionary<string, object>();
            parameters["message"] = item.Text;
            await fb.PostTaskAsync("me/feed", parameters);
        }

        private async Task SendPushNotificationAsync(Post post)
        {
            string senderName = null;
            string senderImageUrl = null;

            using (var db = new timelineformsContext())
            {
                var sender = db.Users.Find(post.SenderId);
                senderName = sender.FullName;
                senderImageUrl = sender.ImageUrl;
            }

            // Sending the message so that all template registrations that contain the following params
            // will receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
            var templateParams = new Dictionary<string, string>
            {
                ["action"] = $"post:{post.Id}",
                ["title"] = $"New post from {senderName}",
                ["message"] = post.Text,
                ["image"] = senderImageUrl
            };

            try
            {
                // Sends the push notification.
                var receivers = $"!{User.GetNotificationTag()}";
                var result = await hubClient.SendTemplateNotificationAsync(templateParams, receivers);
            }
            catch
            { }
        }
    }
}