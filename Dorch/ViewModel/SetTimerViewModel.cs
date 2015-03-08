using Dorch.Common;
using Dorch.DAL;
using Dorch.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Dorch.ViewModel
{
    public class Days
    {
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    public class SetTimerViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {

        private IRepository repo = new Repository();
        private INavigationService _navigationService;
        public Team thisTeam { get; set; }
        private List<Days> _days = new List<Days> 
        {
            new Days{Name= "Monday"}, 
            new Days{Name="Tuesday"}, 
            new Days{Name="Wednesday"},        
            new Days{Name="Thursday"},
            new Days{Name="Friday"},
            new Days{Name="Saturday"},
            new Days{Name="Sunday"}
        };

        public List<Days> Days
        {
            get { return _days; }
            set { _days = value; }
        }
        private string _Name;
        public string UserName
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged("UserName"); }
        }

        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<RoutedEventArgs> DoneCommand { get; set; }

        public SetTimerViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.DoneCommand = new RelayCommand<RoutedEventArgs>(OnDoneCommand);
        }

        private void OnLoadCommand(RoutedEventArgs obj)
        {

        }

        private void OnDoneCommand(RoutedEventArgs obj)
        {
            _navigationService.NavigateTo("ViewTeam", thisTeam);
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

        public void Activate(object parameter)
        {
            if (parameter is Team)
            {
                thisTeam = ((Team)parameter);
            }
        }

        public void Deactivate(object parameter)
        {

        }


    }
}
