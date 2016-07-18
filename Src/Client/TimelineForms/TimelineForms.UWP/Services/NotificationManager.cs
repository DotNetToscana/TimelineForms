using TimelineForms.Services;
using TimelineForms.UWP.Services;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotificationManager))]

namespace TimelineForms.UWP.Services
{
    public class NotificationManager : INotificationManager
    {
        private IMobileServiceClient client;

        public NotificationManager()
        {
            client = ServiceLocator.Current.GetInstance<IMobileServiceClient>();
        }

        private const string templateBodyWNS = @"<toast launch=""action=$(action)"">
                                                  <visual>
                                                    <binding template=""ToastGeneric"">
                                                      <text>$(title)</text>
                                                      <text>$(message)</text>
                                                       <image placement=""appLogoOverride"" hint-crop=""circle"" src=""$(image)"" />
                                                    </binding>
                                                  </visual>
                                                </toast>";

        public async Task RegisterAsync()
        {
            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            var headers = new JObject
            {
                ["X-WNS-Type"] = "wns/toast"
            };

            var templates = new JObject
            {
                ["genericMessage"] = new JObject
                {
                    ["body"] = templateBodyWNS,
                    ["headers"] = headers // Only needed for WNS & MPNS
                }
            };

            await client.GetPush().RegisterAsync(channel.Uri, templates);
        }

        public Task UnregisterAsync() => client.GetPush().UnregisterAsync();
    }
}
