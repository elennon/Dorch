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
            RequestPending current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/RequestPending/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteRequestPending(string id)
        {
             return DeleteAsync(id);
        }

    }
}