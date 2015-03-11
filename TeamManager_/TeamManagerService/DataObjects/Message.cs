using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamManagerService.DataObjects
{
    public class Message : EntityData
    {
        public string Content { get; set; }
        public string Sender { get; set; }
        public string SenderId { get; set; }
        public string TeamId { get; set; }
        public string SendingDate { get; set; }
    }
}