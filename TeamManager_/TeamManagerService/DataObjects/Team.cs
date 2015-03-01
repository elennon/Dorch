using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamManagerService.DataObjects
{
    public class Team : EntityData
    {
        public string TeamName { get; set; }
        public string Location { get; set; }
        public byte[] Image { get; set; }

        //[JsonIgnore]
        public virtual ICollection<Player> Players { get; set; }
    }
}