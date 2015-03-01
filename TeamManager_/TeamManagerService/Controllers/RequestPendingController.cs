using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using TeamManagerService.DataObjects;
using TeamManagerService.Models;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Microsoft.ServiceBus.Notifications;
using System.Collections.Generic;

namespace TeamManagerService.Controllers
{
    //[AuthorizeLevel(AuthorizationLevel.User)]
    public class RequestPendingController : TableController<RequestPending>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            TeamManagerContext context = new TeamManagerContext();
            DomainManager = new EntityDomainManager<RequestPending>(context, Request, Services);
        }

        // GET tables/RequestPending
        public IQueryable<RequestPending> GetAllRequestPending()
        {
            return Query(); 
        }

        // GET tables/RequestPending/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<RequestPending> GetRequestPending(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/RequestPending/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<RequestPending> PatchRequestPending(string id, Delta<RequestPending> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/RequestPending
        public async Task<IHttpActionResult> PostRequestPending(RequestPending item)
        {
            //NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(      
            //"<connection string with full access>", "<hub name>");

            RequestPending current = await InsertAsync(item);
            // Create a WNS native toast.
            WindowsPushMessage message = new WindowsPushMessage();

            // Define the XML paylod for a WNS native toast notification 
            // that contains the text of the inserted item.
            message.XmlPayload = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
                                 @"<toast><visual><binding template=""ToastText01"">" +
                                 @"<text id=""1"">" + item.PlayerId + " has sent a push nc" + @"</text>" +
                                 @"</binding></visual></toast>";
            try
            {
                IEnumerable<string> tags = new List<string> { "eel", "otter" };
                //  await (Services.Push).HubClient.SendWindowsNativeNotificationAsync(message.XmlPayload, tags);
                var result = await Services.Push.SendAsync(message, "eel");    //, "eel"
                //// Get the logged-in user.
                //var currentUser = this.User as ServiceUser;

                //// Use a tag to only send the notification to the logged-in user.
                //var resiult = await Services.Push.SendAsync(message, currentUser.Id);

                Services.Log.Info(result.State.ToString());
            }
            catch (System.Exception ex)
            {
                Services.Log.Error(ex.Message, null, "Push.SendAsync Error");
            }
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        //public async Task<IHttpActionResult> PostRequestPending(RequestPending item)
        //{
        //    // Define a native MPNS push notification.
        //    var payload = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
        //        @"<wp:Notification xmlns:wp=""WPNotification""><wp:Toast>" +
        //        @"<wp:Text1>New Item</wp:Text1><wp:Text2>" + item.PlayerId +
        //        @"</wp:Text2></wp:Toast></wp:Notification>";

        //    // Insert the new item.
        //    RequestPending current = await InsertAsync(item);

        //    // Get the notification hub name from the service settings.
        //    var hubName = Services.Settings.NotificationHubName;

        //    // Get the connection string for the notification hub
        //    // from the mobile service runtime settings.
        //    string hubConnectionString;
        //    if (Services.Settings.TryGetValue("MS_NotificationHubConnectionString",
        //        out hubConnectionString))
        //    {
        //        // Create a new hubs client. 
        //        NotificationHubClient hub =
        //            NotificationHubClient.CreateClientFromConnectionString(
        //            hubConnectionString, hubName);
        //        var result = await hub.SendMpnsNativeNotificationAsync(payload);
        //        //// Log the result of the push notification.
        //        //Services.Log.Info(string.Format("Notification:{0} -state: {1}", 
        //        // result.TrackingId.ToString(), result.State.ToString()));
        //    }

        //    //// You should be able to just call this line of code, but there's a bug.
        //    // await Services.Push.HubClient.SendMpnsNativeNotificationAsync(payload);

        //    return CreatedAtRoute("Tables", new { id = current.Id }, current);
        //}


        //private static async void SendNotificationAsync()
        //{
        //    NotificationHubClient hub = NotificationHubClient
        //        .CreateClientFromConnectionString("<connection string with full access>", "<hub name>");
        //    var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">Hello from a .NET App!</text></binding></visual></toast>";
        //    await hub.SendWindowsNativeNotificationAsync(toast);
        //}
        

        // DELETE tables/RequestPending/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteRequestPending(string id)
        {
             return DeleteAsync(id);
        }

    }
}