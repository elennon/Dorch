using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorch.Model
{
    public class ContactGroup
    {
        public string Title { get; set; }
        public string BackgroundColour { get; set; }
        public ObservableCollection<Player> Playerses { get; set; }
    }
}
