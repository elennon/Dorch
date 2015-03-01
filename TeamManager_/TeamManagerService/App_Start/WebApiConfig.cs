using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using TeamManagerService.DataObjects;
using TeamManagerService.Models;
using System.Linq;

namespace TeamManagerService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();
            //{
            //    PushAuthorization = Microsoft.WindowsAzure.Mobile.Service.Security.AuthorizationLevel.User
            //};

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));
            
            Database.SetInitializer(new TeamManagerInitializer());  // DropCreateDatabaseAlways
        }
    }

    public class TeamManagerInitializer : ClearDatabaseSchemaIfModelChanges<TeamManagerContext>  //  ClearDatabaseSchemaIfModelChanges
    {
        protected override void Seed(TeamManagerContext context)
        {
            List<Player> plys = new List<Player>();
            plys.Add(new Player { Id = "p1", PlayerName = "Mick Keane", PhNumber = "0876493789" });
            plys.Add(new Player { Id = "p2", PlayerName = "Paddy Whelan", PhNumber = "0876493789" });
            plys.Add(new Player { Id = "p3", PlayerName = "Liam Dempsey", PhNumber = "0876493789" });
            plys.Add(new Player { Id = "p4", PlayerName = "Mark Brennan", PhNumber = "0876493789" });
            plys.Add(new Player { Id = "p5", PlayerName = "John Dowling", PhNumber = "0876493789" });
            foreach (var item in plys)
            {
                context.Set<Player>().Add(item);
            }

            List<Team> tms = new List<Team>();
            tms.Add(new Team { Id = "t1", Players = plys, TeamName = "Friday Astro", Location = "Amenities  Centre" });
            tms.Add(new Team { Id = "t2", Players = plys, TeamName = "Sunday Morning 5 Aside", Location = "Amenities  Centre" });
            tms.Add(new Team { Id = "t3", Players = plys, TeamName = "Astro Training", Location = "Amenities  Centre" });
            tms.Add(new Team { Id = "t4", Players = plys, TeamName = "Wednesday Celbridge", Location = "Celbridge Astro" });
            foreach (var item in tms)
            {
                context.Set<Team>().Add(item);
            }

            RequestPending rp = new RequestPending { Id = "rp1", TeamId = "t1", PlayerId = "p1" };
          
            base.Seed(context);
        }

    //    private static void MergeTable(TeamManagerContext context)
    //    {
    //        AddMergeEntry(context, "t1", "p1");
    //        AddMergeEntry(context, "t1", "p2");
    //        AddMergeEntry(context, "t1", "p3");
    //        AddMergeEntry(context, "t1", "p4");
    //        AddMergeEntry(context, "t1", "p5");
    //        AddMergeEntry(context, "t2", "p1");
    //        AddMergeEntry(context, "t2", "p2");
    //        AddMergeEntry(context, "t2", "p3");
    //        AddMergeEntry(context, "t2", "p4");
    //        AddMergeEntry(context, "t2", "p5");
    //        AddMergeEntry(context, "t3", "p1");
    //        AddMergeEntry(context, "t3", "p2");
    //        AddMergeEntry(context, "t3", "p3");
    //        AddMergeEntry(context, "t3", "p4");
    //        AddMergeEntry(context, "t3", "p5");
    //        AddMergeEntry(context, "t4", "p1");
    //        AddMergeEntry(context, "t4", "p2");
    //        AddMergeEntry(context, "t4", "p3");
    //        AddMergeEntry(context, "t4", "p4");
    //        AddMergeEntry(context, "t4", "p5");
    //        AddMergeEntry(context, "t5", "p1");
    //        AddMergeEntry(context, "t5", "p2");
    //        AddMergeEntry(context, "t5", "p3");
    //        AddMergeEntry(context, "t5", "p4");
    //        AddMergeEntry(context, "t5", "p5");
    //    }

    //    private static void AddMergeEntry(TeamManagerContext context, string teamId, string playerId)
    //    {
    //        var team = context.Set<Team>().Single(j => j.Id == teamId);
    //        var player = context.Set<Player>().Single(e => e.Id == playerId);
    //        team.Players.Add(player);
    //    }
    }
}

