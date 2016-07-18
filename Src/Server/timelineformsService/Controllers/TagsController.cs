using timelineformsService.Extensions;
using timelineformsService.Models;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace timelineformsService.Controllers
{
    [MobileAppController]
    [Authorize]
    public class TagsController : ApiController
    {
        private NotificationHubClient hubClient;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            hubClient = this.GetHubNotificationClient();
        }

        // GET api/tags/id
        [HttpGet]
        public async Task<IHttpActionResult> GetTagsByInstallationId(string id)
        {
            try
            {
                // Return the installation for the specific ID.
                var installation = await hubClient.GetInstallationAsync(id);
                return Ok(installation.Tags);
            }
            catch
            {
                return BadRequest();
            }
        }

        // POST api/tags/id
        [HttpPost]
        public async Task<IHttpActionResult> AddTagsToInstallation(string id)
        {
            // Get the tags to update from the body of the request.
            var message = await this.Request.Content.ReadAsStringAsync();

            // Validate the submitted tags.
            if (string.IsNullOrEmpty(message) || message.Contains("sid:"))
            {
                // We can't trust users to submit their own user IDs.
                return BadRequest();
            }

            // Verify that the tags are a valid JSON array.
            var tags = JArray.Parse(message);

            // Define a collection of PartialUpdateOperations. Note that
            // only one '/tags' path is permitted in a given collection.
            var updates = new List<PartialUpdateOperation>();

            // Add a update operation for the tag.
            updates.Add(new PartialUpdateOperation
            {
                Operation = UpdateOperationType.Add,
                Path = "/tags",
                Value = tags.ToString()
            });

            try
            {
                // Add the requested tag to the installation.
                await hubClient.PatchInstallationAsync(id, updates);

                // Return success status.
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (MessagingException)
            {
                // When an error occurs, return a failure status.
                return InternalServerError();
            }
        }
    }
}
