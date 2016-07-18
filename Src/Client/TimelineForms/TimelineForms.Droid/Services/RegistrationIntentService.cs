using System;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TimelineForms.Droid.Services
{
    [Service(Exported = false)]
    public class RegistrationIntentService : IntentService
    {
        static object locker = new object();

        private const string SENDER_ID = "950777784010";

        private const string templateBodyGCM = @"{""data"": {
                                                                ""action"": ""$(action)"",
                                                                ""title"": ""$(title)"",
                                                                ""message"": ""$(message)"",
                                                                ""image"": ""$(image)"",
                                                            }
                                                 }";

        public RegistrationIntentService() : base(nameof(RegistrationIntentService)) { }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                lock (locker)
                {
                    var instanceID = InstanceID.GetInstance(this);
                    instanceID.DeleteInstanceID();

                    instanceID = InstanceID.GetInstance(this);
                    var token = instanceID.GetToken(SENDER_ID, GoogleCloudMessaging.InstanceIdScope, null);

                    this.SendRegistrationToAppServer(token);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(nameof(RegistrationIntentService), $"Failed to get a registration token:{ex.Message}");
            }
        }

        private async void SendRegistrationToAppServer(string token)
        {
            var client = ServiceLocator.Current.GetInstance<IMobileServiceClient>();
            var push = client.GetPush();

            try
            {
                var templates = new JObject
                {
                    ["genericMessage"] = new JObject
                    {
                        ["body"] = templateBodyGCM
                    }
                };

                await push.RegisterAsync(token, templates);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(RegistrationIntentService), $"Failed to register for push notifications:{ex.Message}");
            }
        }
    }
}