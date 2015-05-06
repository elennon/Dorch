using Dorch.Common;
using Dorch.DAL;
using Dorch.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Dorch.ViewModel
{
    
    public class ViewTeamViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {
        private IRepository repo = new Repository();
        private INavigationService _navigationService;
        
        private ObservableCollection<Message> _messages; //= new List<Message> 
        //{
        //    new Message{Content= "papion for a peeon "}           
        //};

        public ObservableCollection<Message> Messages
        {
            get { return _messages; }
            set { _messages = value; NotifyPropertyChanged("Messages"); }
        }
        
        private ObservableCollection<Player> _chosenPlayers;
        public ObservableCollection<Player> ChosenPlayers
        {
            get
            {
                return _chosenPlayers;
            }
            set
            {
                if (_chosenPlayers != value)
                {
                    _chosenPlayers = value;
                    NotifyPropertyChanged("ChosenPlayers");
                }
            }
        }

        private ObservableCollection<Player> _players;
        public ObservableCollection<Player> Players
        {
            get
            {
                return _players;
            }
            set
            {
                if (_players != value)
                {
                    _players = value;
                    NotifyPropertyChanged("Players");
                }
            }
        }

        private ObservableCollection<Player> _sPlayers;
        public ObservableCollection<Player> StatusPlayers
        {
            get
            {
                return _sPlayers;
            }
            set
            {
                if (_sPlayers != value)
                {
                    _sPlayers = value;
                    NotifyPropertyChanged("StatusPlayers");
                }
            }
        }

        private string _messageToSend;
        public string MessageToSend
        {
            get { return _messageToSend; }
            set { _messageToSend = value; NotifyPropertyChanged("MessageToSend"); }
        }

        private int _bMessIndex;
        public int BottomMessageIndex
        {
            get { return _bMessIndex; }
            set { _bMessIndex = value; NotifyPropertyChanged("BottomMessageIndex"); }
        }
        
        public Team thisTeam { get; set; }
        public TimeSpan SetTime { get; set; }

        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<Object> ItemSelectedCommand { get; set; }
        public RelayCommand<Player> PlayerSelectedCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand StartCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand AddPlayerCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand TimeChangedCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand SendMessageCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand SetTimerCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand MessageChangeCommand { get; set; }

        public ViewTeamViewModel(INavigationService navigationService)
        {
            ((App)Application.Current).vtvm = this;
            _navigationService = navigationService;
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.ItemSelectedCommand = new RelayCommand<object>(OnItemSelectedCommand);
            this.PlayerSelectedCommand = new RelayCommand<Player>(OnPlayerSelectedCommand);
            this.StartCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnStartCommand);
            this.AddPlayerCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnAddPlayerCommand);
            this.TimeChangedCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnTimeChangedCommand);
            this.SendMessageCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnSendMessageCommand);
            this.SetTimerCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnSetTimerCommand);
            this.MessageChangeCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnMessageChangeCommand);
            ChosenPlayers = new ObservableCollection<Player>();
            StatusPlayers = new ObservableCollection<Player>();
            Messages = new ObservableCollection<Message>();
            SetTime = new TimeSpan(18, 0, 0);
        }

        private void OnMessageChangeCommand()
        {
            BottomMessageIndex = Messages.Count - 1;
            //var selectedIndex = Messages.Count - 1;
            //if (selectedIndex < 0)
            //    return;
            //var ft = ((App)Application.Current).vtvm.

            //MyListView.SelectedIndex = selectedIndex;
            //MyListView.UpdateLayout();

            //MyListView.ScrollIntoView(MyListView.SelectedItem); 
        }

        private void OnSetTimerCommand()
        {
            _navigationService.NavigateTo("SetTimer", thisTeam);
        }

        private async void OnSendMessageCommand()
        {
            var df = DateTime.Now.ToString("ddd").Substring(0, 3);

            string date = df + ", " + (DateTime.Now).ToString("HH:mm");
            Message ms = new Message{ Content = MessageToSend, TeamId = thisTeam.Id, Sender = ((App)Application.Current).UserName, 
                            SendingDate = date , SenderId = ((App)Application.Current).UserId};
            await repo.SendMessage(ms);
           // Messages.Add(ms);
            MessageToSend = "";
        }

        private void OnTimeChangedCommand()
        {
            var time = SetTime;
        }

        private void OnAddPlayerCommand()
        {
            _navigationService.NavigateTo("AddPlayer", thisTeam);
        }

        private async void OnStartCommand()
        {
            StatusPlayers.Clear();
            var time = SetTime;
            foreach (var item in ChosenPlayers)
            {
                RequestPlay rp = new RequestPlay{ PlayerId = item.Id, TeamId = thisTeam.Id, Confirmed = false};
                await repo.SendPlayRequest(rp);
            }
        }

        private void OnPlayerSelectedCommand(Player obj)
        {           
            if (!obj.IsPicked)       // if already chosen, then unselect
            {
                obj.IsPicked = true;
                ChosenPlayers.Add(obj);
            }
            else
            {
                obj.IsPicked = false;
                ChosenPlayers.Remove(obj);
            }         
        }

        private void OnItemSelectedCommand(object obj)
        {
            //throw new NotImplementedException();
        }

        private async void OnLoadCommand(RoutedEventArgs obj)
        {
            Messages = new ObservableCollection<Message>(await repo.GetMessages(thisTeam.Id));
            BottomMessageIndex = Messages.Count() -1;
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
                var plyrs = ((Team)parameter).Players;
                Players = new ObservableCollection<Player>(plyrs);
                thisTeam = ((Team)parameter);

            }
        }

        public void Deactivate(object parameter)
        {
            ChosenPlayers.Clear();
        }

        
    }
}
