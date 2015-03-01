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
        public string UserId { get; set; }
        public string PlayerName { get; set; }
        public string PhNumber { get; set; }
        public byte[] Image { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<Team> Teams { get; set; }
    }
}