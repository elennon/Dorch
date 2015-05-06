using Dorch.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorch.ViewModel
{
    public class InfoViewModel : ViewModelBase, INavigable
    {
        private INavigationService _navigationService;
        private string info;
        public string Info
        {
            get { return info; }
            set { info = value; }
        }

        private string moreInfo;
        public string MoreInfo
        {
            get { return moreInfo; }
            set { moreInfo = value; }
        }
        
        public InfoViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Info = "Dorch is an app for any group of people who meet up regularly to organize who is going to turn up. A message is sent to members of the group and once they reply, the rest of the members can see if they are in or out. The app can also be used for members to text each other as a group.";
            MoreInfo = "You can create a team and start inviting people to join. Once they accept they are added to the team. When you're due to meet up, check team members and send a request. Scroll right to see who's in/out and everyone can get involved in group texts!";
        }

        public void Activate(object parameter)
        {
            //throw new NotImplementedException();
        }

        public void Deactivate(object parameter)
        {
            //throw new NotImplementedException();
        }
    }
}
