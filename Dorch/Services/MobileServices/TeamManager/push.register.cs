using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

// http://go.microsoft.com/fwlink/?LinkId=290986&clcid=0x409

namespace Dorch
{
    internal class TeamManagerPush
    {
        public async static void UploadChannel()
        {
            var channel = await Windows.Networking.PushNotifications.PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            try
            {
                IEnumerable<string> tags = new List<string> { "eel" };
                await App.TeamManagerClient.GetPush().RegisterNativeAsync(channel.Uri, tags);
                await App.TeamManagerClient.InvokeApiAsync("Notifications", new JObject(new JProperty("toast", "Sample pr Toast")));                    
            }
            catch (Exception exception)
            {
                HandleRegisterException(exception);
            }
        }

        private static void HandleRegisterException(Exception exception)
        {

        }
    }
}
