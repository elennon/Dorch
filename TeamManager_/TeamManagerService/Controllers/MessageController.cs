 using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using TeamManagerService.DataObjects;
using TeamManagerService.Models;

namespace TeamManagerService.Controllers
{
    public class MessageController : TableController<Message>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            TeamManagerContext context = new TeamManagerContext();
            DomainManager = new EntityDomainManager<Message>(context, Request, Services);
        }

        // GET tables/Message
        public IQueryable<Message> GetAllMessage()
        {
            return Query(); 
        }

        // GET tables/Message/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Message> GetMessage(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Message/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Message> PatchMessage(string id, Delta<Message> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Message
        public async Task<IHttpActionResult> PostMessage(Message item)
        {
            Message current = await InsertAsync(item);
            WindowsPushMessage message = new WindowsPushMessage();  //+ team.TeamName
           
            message.XmlPayload = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
                                 @"<toast><visual><binding template=""ToastText01"">" +
                                 @"<text id=""4"">" + current.Id + @"</text>" +
                                 @"</binding></visual></toast>";
            message.Headers.Add("X-WNS-Cache-Policy", "cache");
            message.Headers.Add("X-WNS-Type", "wns/raw");
            try
            {
                var result = await Services.Push.SendAsync(message, item.TeamId);    //  , item.TeamId
                Services.Log.Info(result.State.ToString());
            }
            catch (System.Exception ex)
            {
                Services.Log.Error(ex.Message, null, "Push.SendAsync Error");
            }
    
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Message/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteMessage(string id)
        {
             return DeleteAsync(id);
        }

    }
}