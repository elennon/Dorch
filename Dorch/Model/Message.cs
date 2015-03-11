using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorch.Model
{
    public class Message
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Sender { get; set; }
        public string SenderId { get; set; }
        public string TeamId { get; set; }
        public string SendingDate { get; set; }
    }
}
