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
    public class RequestJoinTeamController : TableController<RequestJoinTeam>
    {
        private TeamManagerContext context;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            context = new TeamManagerContext();
            DomainManager = new EntityDomainManager<RequestJoinTeam>(context, Request, Services);
        }

        // GET tables/RequestPending
        public IQueryable<RequestJoinTeam> GetAllRequestPending()
        {
            return Query(); 
        }

        // GET tables/RequestPending/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<RequestJoinTeam> GetRequestPending(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/RequestPending/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<RequestJoinTeam> PatchRequestPending(string id, Delta<RequestJoinTeam> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/RequestPending
        public async Task<IHttpActionResult> PostRequestPending(RequestJoinTeam item)
        {
            var team = context.Teams.Where(a => a.Id == item.TeamId).FirstOrDefault();
            RequestJoinTeam current = await InsertAsync(item);   
            WindowsPushMessage message = new WindowsPushMessage();  //+ team.TeamName
    
            message.XmlPayload = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
                                 @"<toast><visual><binding template=""ToastText01"">" +
                                 @"<text id=""1"">" + current.Id + @"</text>" +
                                 @"</binding></visual></toast>";
            message.Headers.Add("X-WNS-Cache-Policy", "cache");
            message.Headers.Add("X-WNS-Type", "wns/raw");            
            try
            {
                var result = await Services.Push.SendAsync(message, item.PlayerId);                    
                Services.Log.Info(result.State.ToString());
            }
            catch (System.Exception ex)
            {
                Services.Log.Error(ex.Message, null, "Push.SendAsync Error");
            }
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }
       
        // DELETE tables/RequestPending/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteRequestPending(string id)
        {
             return DeleteAsync(id);
        }

    }
}