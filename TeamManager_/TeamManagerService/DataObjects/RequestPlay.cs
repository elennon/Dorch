using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamManagerService.DataObjects
{
    public class RequestPlay : EntityData
    {
        public string PlayerId { get; set; }
        public string TeamId { get; set; }
        public bool Confirmed { get; set; }
    }
}