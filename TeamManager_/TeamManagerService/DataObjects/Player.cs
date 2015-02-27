using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamManagerService.DataObjects
{
    public class Player : EntityData
    {
        //public string Id { get; set; }
        public string PlayerName { get; set; }
        public string PhNumber { get; set; }
        public byte[] Image { get; set; }
        public bool Pending { get; set; }
        public string RequestedTeam { get; set; }

        [JsonIgnore]
        public virtual ICollection<Team> Teams { get; set; }
    }
}