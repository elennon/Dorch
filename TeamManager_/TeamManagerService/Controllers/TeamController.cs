using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using TeamManagerService.DataObjects;
using TeamManagerService.Models;
using TeamManagerService.Utilities;
using System.Collections.Generic;

namespace TeamManagerService.Controllers
{
    public class TeamController : TableController<Team>
    {
        private TeamManagerContext context;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            context = new TeamManagerContext();
            DomainManager = new EntityDomainManager<Team>(context, Request, Services);
        }

        // GET tables/Team
        [QueryableExpand("Players")]
        public IQueryable<Team> GetAllTeam()
        {
            return Query(); 
        }

        // GET tables/Team/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [QueryableExpand("Players")]
        public SingleResult<Team> GetTeam(string id)
        {
            return Lookup(id);
        }


        //public async Task<Team> PatchTeam(string id, Delta<Team> patch)
        //{
        //    //Read TodoItem to update from database so that EntityFramework updates
        //    //existing entry
        //    Team currentTeam = this.context.Teams.Include("Players").First(j => (j.Id == id));

        //    Team updatedpatchEntity = patch.GetEntity();
        //    ICollection<Player> updatedItems;

        //    //Check if incoming request contains Items
        //    bool requestContainsRelatedEntities = patch.GetChangedPropertyNames().Contains("Players");

        //    if (requestContainsRelatedEntities)
        //    {
        //        //Remove related entities from the database. Comment following for loop if you do not
        //        //want to delete related entities from the database
        //        for (int i = 0; i < currentTeam.Players.Count && updatedpatchEntity.Players != null; i++)
        //        {
        //            Player ply = updatedpatchEntity.Players.FirstOrDefault(j => (j.Id == currentTeam.Players.ElementAt(i).Id));
        //            if (ply == null)
        //            {
        //                this.context.Players.Remove(currentTeam.Players.ElementAt(i));
        //            }
        //        }

        //        //If request contains Items get the updated list from the patch
        //        //Mapper.Map<TodoItemDTO, TodoItem>(updatedpatchEntity, currentTeam);
        //        updatedItems = updatedpatchEntity.Players;
        //    }
        //    else
        //    {
        //        //If request doest not have Items, then retain the original association

        //        patch.Patch(currentTeam);

        //        updatedItems = currentTeam.Players;
        //    }

        //    if (updatedItems != null)
        //    {
        //        //Update related Items
        //        currentTeam.Players = new List<Player>();
        //        foreach (Player currentItemDTO in updatedItems)
        //        {
        //            //Look up existing entry in database
        //            Player existingPlayer = this.context.Players.Include("Teams").FirstOrDefault(j => (j.Id == currentItemDTO.Id));

        //            existingPlayer.Teams.Add(currentTeam);

        //            currentTeam.Players.Add(existingPlayer);
        //        }
        //    }
        //    await this.context.SaveChangesAsync();

        //    //var result = await base.UpdateAsync(id, patch);
        //    //return result;// currentTeam;

        //    return currentTeam;
        //}


        //PATCH tables/Team/48D68C86-6EA6-4C25-AA33-223FC9A27959
       // [QueryableExpand("Players")]
        public Team PatchTeam(string id, Delta<Team> patch)     //  Task<Team>
        {
            Team currentTeam = this.context.Teams.Include("Players").First(j => (j.Id == id));
            Team updatedpatchEntity = patch.GetEntity();
            ICollection<Player> updatedItems;
            bool requestContainsRelatedEntities = patch.GetChangedPropertyNames().Contains("Players");

            if (requestContainsRelatedEntities)
            {
                updatedItems = updatedpatchEntity.Players;
            }
            else
            {
                //If request doest not have Items, then retain the original association
                patch.Patch(currentTeam);
                updatedItems = currentTeam.Players;
            }

            if (updatedItems != null)
            {
                //Update related Items
                currentTeam.Players = new List<Player>();
                foreach (Player currentItemDTO in updatedItems)
                {
                    //Look up existing entry in database
                    Player existingPlayer = this.context.Players.Include("Teams").FirstOrDefault(j => (j.Id == currentItemDTO.Id));

                    existingPlayer.Teams.Add(currentTeam);

                    currentTeam.Players.Add(existingPlayer);
                }
            }
            context.SaveChanges();
            return currentTeam;
            //return UpdateAsync(id, patch);
        }

        // POST tables/Team
        public async Task<IHttpActionResult> PostTeam(Team item)
        {
            Team current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Team/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteTeam(string id)
        {
             return DeleteAsync(id);
        }

    }
}