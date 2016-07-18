using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace timelineformsService.Extensions
{
    public static class NotificationHubExtensions
    {
        public static NotificationHubClient GetHubNotificationClient(this ApiController controller)
        {
            // Get the settings for the server project.
            var settings = controller.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();
            //var traceWriter = controller.Configuration.Services.GetTraceWriter();

            // Get the Notification Hubs credentials for the Mobile App.
            var notificationHubName = settings.NotificationHubName;
            var notificationHubConnection = settings.Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

            // Create a new Notification Hub client.
            var hub = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            return hub;
        }

        public static string GetNotificationTag(this IPrincipal user)
            => NotificationHubExtensions.GetNotificationTagFor(user.GetUserId());

        public static string GetNotificationTagFor(string userId) => $"_UserId:{userId}";
    }
}
