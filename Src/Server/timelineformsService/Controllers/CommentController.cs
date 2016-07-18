using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using timelineformsService.DataObjects;
using timelineformsService.Models;
using System.Security.Claims;
using timelineformsService.Filters;
using timelineformsService.Extensions;
using Microsoft.Azure.NotificationHubs;
using System.Collections.Generic;

namespace timelineformsService.Controllers
{
    [Authorize]
    public class CommentController : TableController<Comment>
    {
        private NotificationHubClient hubClient;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            timelineformsContext context = new timelineformsContext();
            DomainManager = new EntityDomainManager<Comment>(context, Request);

            hubClient = this.GetHubNotificationClient();
        }

        // GET tables/Comment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [QueryableExpand("Sender")]
        [QueryableExpand("Post/Sender")]
        public SingleResult<Comment> GetComment(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Comment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Comment> PatchComment(string id, Delta<Comment> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Comment
        public async Task<IHttpActionResult> PostComment(Comment item)
        {
            // Adds the userId to the comment.
            item.SenderId = this.User.GetUserId();
            Comment current = await InsertAsync(item);

            await this.SendPushNotificationAsync(item);

            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Comment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteComment(string id)
        {
            return DeleteAsync(id);
        }

        private async Task SendPushNotificationAsync(Comment comment)
        {
            string senderName = null;
            string senderImageUrl = null;
            string postAuthorId = null;

            using (var db = new timelineformsContext())
            {
                postAuthorId = db.Posts.Find(comment.PostId).SenderId;

                // If the author of the comment is the same of the post, we don't need
                // to send notification.
                if (postAuthorId == comment.SenderId)
                    return;

                var sender = db.Users.Find(comment.SenderId);
                senderName = sender.FullName;
                senderImageUrl = sender.ImageUrl;
            }

            // Sending the message so that all template registrations that contain the following params
            // will receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
            var templateParams = new Dictionary<string, string>
            {
                ["action"] = $"comment:{comment.Id}|post:{comment.PostId}",
                ["title"] = $"{senderName} commented your post",
                ["message"] = comment.Text,
                ["image"] = senderImageUrl
            };

            try
            {
                // Sends the push notification.
                var receiver = NotificationHubExtensions.GetNotificationTagFor(postAuthorId);
                var result = await hubClient.SendTemplateNotificationAsync(templateParams, receiver);
            }
            catch
            { }
        }
    }
}