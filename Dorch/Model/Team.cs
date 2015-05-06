using Dorch.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Dorch.Model
{
    public class Team : INotifyPropertyChanged
    {
        public string Id { get; set; }
        public string TeamName { get; set; }
        public string Location { get; set; }
        public byte[] TeamImage { get; set; }
        public string PlayerTargetNumber { get; set; }
        public ICollection<Player> Players { get; set; }

        private ImageSource _imgSource;
        public ImageSource ImgSource        // returns TeamImage byte[] as image source or default if null
        {
            get { return _imgSource; }
            //set { _imgSource = value; }    
            set { _imgSource = value ?? ReadImage.GetTeamImage(this.TeamImage); }                             
        }
             
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }



   
}
