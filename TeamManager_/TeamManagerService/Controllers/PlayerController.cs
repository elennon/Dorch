using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using TeamManagerService.DataObjects;
using TeamManagerService.Models;
using TeamManagerService.Utilities;
using System;

namespace TeamManagerService.Controllers
{
    public class PlayerController : TableController<Player>
    {
        private TeamManagerContext context;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            context = new TeamManagerContext();
            DomainManager = new EntityDomainManager<Player>(context, Request, Services);
        }

        // GET tables/Player
        [QueryableExpand("Teams")]
        public IQueryable<Player> GetAllPlayer()
        {
            return Query(); 
        }

        // GET tables/Player/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [QueryableExpand("Teams")]
        public SingleResult<Player> GetPlayer(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Player/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Player> PatchPlayer(string id, Delta<Player> patch)
        {
             return UpdateAsync(id, patch);
        }

        //public async Task<IHttpActionResult> PostPlayer(Player item)
        //{
        //    if (string.IsNullOrEmpty(item.Id))
        //    {
        //        item.Id = Guid.NewGuid().ToString();
        //    }
        //    Player newPlayer = (item);

        //    if (item.Teams != null)
        //    {
        //        foreach (Team equipmentDTO in item.Teams)
        //        {
        //            Team tm = this.context.Teams.Include("Players").FirstOrDefault(eq => eq.Id.Equals(equipmentDTO.Id));

        //            tm.Players.Add(newPlayer);
        //            newPlayer.Teams.Add(tm);
        //        }
        //    }

        //    await this.context.SaveChangesAsync();
            
        //    return this.CreatedAtRoute("Tables", new { id = item.Id }, item);
        //}

        // POST tables/Player

        public async Task<IHttpActionResult> PostPlayer(Player item)
        {
            Player current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Player/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePlayer(string id)
        {
             return DeleteAsync(id);
        }

    }
}