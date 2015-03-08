using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using TeamManagerService.DataObjects;

namespace TeamManagerService.Models
{
    public class TeamManagerContext : DbContext
    {
        
        private const string connectionStringName = "Name=MS_TableConnectionString";

        public TeamManagerContext() : base(connectionStringName)
        {
        } 

        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<RequestJoinTeam> RequestPendings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            string schema = ServiceSettingsDictionary.GetSchemaName();
            if (!string.IsNullOrEmpty(schema))
            {
                modelBuilder.HasDefaultSchema(schema);
            }

            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));

            modelBuilder.Entity<Team>().HasMany(t => t.Players).WithMany(t => t.Teams);
                                                           
        }

        public System.Data.Entity.DbSet<TeamManagerService.DataObjects.RequestPlay> RequestPlays { get; set; }

        public System.Data.Entity.DbSet<TeamManagerService.DataObjects.Message> Messages { get; set; }        
    }

}
