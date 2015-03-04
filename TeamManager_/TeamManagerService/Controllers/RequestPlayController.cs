using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using TeamManagerService.DataObjects;
using TeamManagerService.Models;
using System.Collections.Generic;

namespace TeamManagerService.Controllers
{
    public class RequestPlayController : TableController<RequestPlay>
    {
        private TeamManagerContext context;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            context = new TeamManagerContext();
            DomainManager = new EntityDomainManager<RequestPlay>(context, Request, Services);
        }

        // GET tables/RequestPlay
        public IQueryable<RequestPlay> GetAllRequestPlay()
        {
            return Query(); 
        }

        // GET tables/RequestPlay/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<RequestPlay> GetRequestPlay(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/RequestPlay/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<RequestPlay> PatchRequestPlay(string id, Delta<RequestPlay> patch)
        {
            RequestPlay current = context.RequestPlays.Where(a => a.Id == id).FirstOrDefault();
            Team team = context.Teams.Where(a => a.Id == current.TeamId).FirstOrDefault();
            WindowsPushMessage message = new WindowsPushMessage();  //+ team.TeamName

            message.XmlPayload = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
                                @"<toast><visual><binding template=""ToastText01"">" +
                                @"<text id=""3"">" + current.Id + @"</text>" +
                                @"</binding></visual></toast>";
            message.Headers.Add("X-WNS-Cache-Policy", "cache");
            message.Headers.Add("X-WNS-Type", "wns/raw");
            var tags = new List<string> { "0876493789", "t1" };
            try
            {
                var result = await Services.Push.SendAsync(message, tags);
                Services.Log.Info(result.State.ToString());
            }
            catch (System.Exception ex)
            {
                Services.Log.Error(ex.Message, null, "Push.SendAsync Error");
            }
            return await UpdateAsync(id, patch);
        }

        // POST tables/RequestPlay
        public async Task<IHttpActionResult> PostRequestPlay(RequestPlay item)
        {
            //Team team = context.Teams.Where(a => a.Id == item.TeamId).FirstOrDefault();
            RequestPlay current = await InsertAsync(item);
            WindowsPushMessage message = new WindowsPushMessage();  //+ team.TeamName

            message.XmlPayload = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
                                 @"<toast><visual><binding template=""ToastText01"">" +
                                 @"<text id=""2"">" + current.Id + @"</text>" +
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

        // DELETE tables/RequestPlay/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteRequestPlay(string id)
        {
             return DeleteAsync(id);
        }

    }
}