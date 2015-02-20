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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Dorch.ViewModel
{
    public class ViewTeamViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {

        private IRepository repo = new Repository();
        private INavigationService _navigationService;

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

        public Team thisTeam { get; set; }

        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<Object> ItemSelectedCommand { get; set; }
        public RelayCommand<Player> PlayerSelectedCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand StartCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand AddPlayerCommand { get; set; }

        public ViewTeamViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.ItemSelectedCommand = new RelayCommand<object>(OnItemSelectedCommand);
            this.PlayerSelectedCommand = new RelayCommand<Player>(OnPlayerSelectedCommand);
            this.StartCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnStartCommand);
            this.AddPlayerCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnAddPlayerCommand);
            ChosenPlayers = new ObservableCollection<Player>();
            StatusPlayers = new ObservableCollection<Player>();
        }

        private void OnAddPlayerCommand()
        {
            _navigationService.NavigateTo("AddPlayer", thisTeam);
        }

        private async void OnStartCommand()
        {
            Windows.ApplicationModel.Chat.ChatMessage msg = new Windows.ApplicationModel.Chat.ChatMessage();
            msg.Body = "This is body of demo message.";
            msg.Recipients.Add("0876493789");
            
            await Windows.ApplicationModel.Chat.ChatMessageManager.ShowComposeSmsMessageAsync(msg);
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
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            var dataFolder = await local.CreateFolderAsync("Assets", CreationCollisionOption.OpenIfExists);
            foreach (var item in Players)
            {
                item.ImageSource = await MakeImage(item.Image, dataFolder);
            }
        }

        async Task<ImageSource> MakeImage(string fileName, StorageFolder folder)
        {
            BitmapImage bitmapImage = null;
            try
            {
                StorageFile file = await folder.GetFileAsync(fileName);
                if (file != null)
                {
                    bitmapImage = new BitmapImage();

                    using (var stream = await file.OpenReadAsync())
                    {
                        await bitmapImage.SetSourceAsync(stream);
                    }
                }
                return (bitmapImage);
            }
            catch (Exception ex)
            {
                return null;
            }
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
            
        }

    }
}
