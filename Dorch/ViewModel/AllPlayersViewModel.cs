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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Dorch.ViewModel
{
    public class AllPlayersViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {

        private IRepository repo = new Repository();
        private INavigationService _navigationService;
        private ObservableCollection<Team> allTeams = new ObservableCollection<Team>();

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

        private ObservableCollection<ContactGroup> _cGroups;
        public ObservableCollection<ContactGroup> CGroups
        {
            get { return _cGroups; }
            set { _cGroups = value; }  //NotifyPropertyChanged("CGroups");
        }

        private CollectionViewSource _playersViewSource;
        public CollectionViewSource PlayersViewSource
        {
            get { return _playersViewSource; }
            set { _playersViewSource = value; NotifyPropertyChanged("PlayersViewSource"); }
        }

        public Team thisTeam { get; set; }

        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<Object> ItemSelectedCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand RefreshCommand { get; set; }
        
        public AllPlayersViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.ItemSelectedCommand = new RelayCommand<object>(OnItemSelectedCommand);
            this.RefreshCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnRefreshCommand);            
        }

        private async void OnLoadCommand(RoutedEventArgs obj)
        {
            allTeams = new ObservableCollection<Team>(await repo.GetTeamsAsync());
            //foreach (Team tm in allTeams)
            //{
            //    foreach (Player item in tm.Players)
            //    {
            //        item.PlayerImage = ReadImage.GetImage(item.Image);                   
            //    }
            //}

            var tmms = GetGroups(allTeams);
            PlayersViewSource = new CollectionViewSource();
            PlayersViewSource.IsSourceGrouped = true;
            PlayersViewSource.Source = tmms;
            PlayersViewSource.ItemsPath = new PropertyPath("Playerses");
        }

        private async void OnRefreshCommand()
        {
            await LoadTeams();
        }

        private async Task LoadTeams()
        {
            allTeams = new ObservableCollection<Team>(await repo.GetTeamsAsync());
            //foreach (Team tm in allTeams)
            //{
            //    foreach (Player item in tm.Players)
            //    {
            //        item.PlayerImage = ReadImage.GetImage(item.Image); 
            //    }
            //}
            
            var tmms = GetGroups(allTeams);
            PlayersViewSource = new CollectionViewSource();
            PlayersViewSource.IsSourceGrouped = true;
            PlayersViewSource.Source = tmms;
            PlayersViewSource.ItemsPath = new PropertyPath("Playerses");
        }
        
        private ObservableCollection<ContactGroup> GetGroups(ObservableCollection<Team> tms)    // method to group all tracks alphabetically
        {
            List<ContactGroup> teamGroups = new List<ContactGroup>();
            ContactGroup tGroup = new ContactGroup();
            foreach (var item in tms)
            {
                tGroup = new ContactGroup() { Title = item.TeamName, BackgroundColour = "Gray", Playerses = new ObservableCollection<Player>(item.Players) };
                teamGroups.Add(tGroup);
            }
            return new ObservableCollection<ContactGroup>(teamGroups);
        }

        private async void OnItemSelectedCommand(object obj)
        {
            var player = obj as Player;
            var u = thisTeam.Players.Where(a => a.Id == player.Id).Count();
            if (u < 1)
            {
                RequestPending rp = new RequestPending { TeamId = thisTeam.Id, PlayerId = player.Id, Confirmed = false };
                await repo.SendRequest(rp);               
                _navigationService.NavigateTo("ViewTeam", thisTeam);
            }
            else
            {
                MessageDialog msgbox = new MessageDialog(player.PlayerName + " is already in this team!!");
                msgbox.Commands.Clear();
                msgbox.Commands.Add(new UICommand { Label = "OK", Id = 0 });                
                var res = await msgbox.ShowAsync();
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
                thisTeam = ((Team)parameter);
            }
        }

        public void Deactivate(object parameter)
        {

        }

    }
}



//CGroups = GetGroups(new ObservableCollection<Team>(tms));  // adding CGroups as observable collection as the data source to be able to bind adding or deleting from list i.e. bin tracks
            //var ftgy = GetGroups(tms);
            //PlayersViewSource = new CollectionViewSource();
            //PlayersViewSource.IsSourceGrouped = true;
            //PlayersViewSource.Source = ftgy;
            //PlayersViewSource.ItemsPath = new PropertyPath("Playerses");

//List<Player> plys = new List<Player>();
//plys.Add(new Player { Id = "p1", PlayerName = "Mick Keane", PhNumber = "0876493789" });
//plys.Add(new Player { Id = "p2", PlayerName = "Paddy Whelan", PhNumber = "0876493789" });
//plys.Add(new Player { Id = "p3", PlayerName = "Liam Dempsey", PhNumber = "0876493789"});
//plys.Add(new Player { Id = "p4", PlayerName = "Mark Brennan", PhNumber = "0876493789" });
//plys.Add(new Player { Id = "p5", PlayerName = "John Dowling", PhNumber = "0876493789"});

//List<Team> tms = new List<Team>();
//tms.Add(new Team { Id = "t1", Players = plys, TeamName = "Friday Astro", Location = "Amenities  Centre"});
//tms.Add(new Team { Id = "t2", Players = plys, TeamName = "Sunday Morning 5 Aside", Location = "Amenities  Centre" });
//tms.Add(new Team { Id = "t3", Players = plys, TeamName = "Astro Training", Location = "Amenities  Centre" });
//tms.Add(new Team { Id = "t4", Players = plys, TeamName = "Wednesday Celbridge", Location = "Celbridge Astro" });

//async Task<ImageSource> MakeImage(string fileName, StorageFolder folder)
//        {
//            BitmapImage bitmapImage = null;
//            try
//            {
//                StorageFile file = await folder.GetFileAsync(fileName);
//                if (file != null)
//                {
//                    bitmapImage = new BitmapImage();

//                    using (var stream = await file.OpenReadAsync())
//                    {
//                        await bitmapImage.SetSourceAsync(stream);
//                    }
//                }
//                return (bitmapImage);
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }


//private async Task<ObservableCollection<ContactGroup>> GetGroups(ObservableCollection<Player> plays)    // method to group all tracks alphabetically
//{
//    List<ContactGroup> teamGroups = new List<ContactGroup>();
//    ContactGroup tGroup = new ContactGroup();
//    var grps = allTeams.Select(a => a.TeamName).Distinct().ToList();

//    foreach (var playr in plays)
//    {
//        var picc = await ReadImage.GetImage(playr.Image);
//        playr.ImageSource = picc;
//    }
//    var ftg = new ObservableCollection<Player>(item.PlayerColl);
//    tGroup = new ContactGroup() { Title = item.TeamName, BackgroundColour = "Gray", Playerses = plays };
//    teamGroups.Add(tGroup);

//    //return teamGroups;
//     return new ObservableCollection<ContactGroup>(teamGroups);
//}
