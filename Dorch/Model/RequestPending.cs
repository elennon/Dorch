using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorch.Model
{
    public class RequestPending 
    {
        public string id { get; set; }
        public string PlayerId { get; set; }
        public string TeamId { get; set; }
        public bool Confirmed { get; set; }
    }
}
