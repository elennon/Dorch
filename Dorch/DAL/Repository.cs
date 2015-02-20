using Dorch.Model;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Dorch.DAL
{
    public class Repository : IRepository, INotifyPropertyChanged
    {
        MobileServiceClient _client = App.MobileService;

        private MobileServiceCollection<Player, Player> _Players;
        public MobileServiceCollection<Player, Player> Players
        {
            get { return _Players; }
            set
            {
                _Players = value;
                NotifyPropertyChanged("Players");
            }
        }

        private string _ErrorMessage = null;
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                _ErrorMessage = value;
                NotifyPropertyChanged("ErrorMessage");
            }
        }

        private MobileServiceCollection<Team, Team> teams;
        private MobileServiceCollection<Player, Player> players; 
        private IMobileServiceTable<Team> teamTable = App.MobileService.GetTable<Team>();
        private IMobileServiceTable<Player> playerTable = App.MobileService.GetTable<Player>();

        public async Task addNewTeamAsync(Team tm)
        {
            await teamTable.InsertAsync(tm);
        }

        public async Task<List<Team>> GetTeamsAsync()
        {
            var tms = new List<Team>();
            ErrorMessage = null;
            try
            {
                //var tm = await teamTable.LookupAsync("t1");
                //var dr = new Player { Id = "b77", PlayerName = "jefryy" };
                //        //List<Team> uu = new List<Team>();
                //        //uu.Add(tm);
                //        //dr.Teams = uu;
                ////await playerTable.InsertAsync(dr);

                //var ft = await playerTable.LookupAsync("b77");

                
                //tm.Players.Add(ft);

                //await teamTable.UpdateAsync(tm);

                //var fit = await playerTable.LookupAsync("b77");

                teams = await teamTable.ToCollectionAsync();
                foreach (var item in teams)
                {
                    if (item.Image == null) { item.Image = "Charlton.png"; }
                    tms.Add(item);
                }
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (HttpRequestException ex2)
            {
                ErrorMessage = ex2.Message;
            }           
            return tms;
        }

        public async Task UpdateTeamAsync(Team tm)
        {
            await teamTable.UpdateAsync(tm);
        }

        public async void DeleteTeam(Team tm)
        {
            await teamTable.DeleteAsync(tm);
        }


        public async Task<List<Player>> GetPlayersAsync()
        {
            var pls = new List<Player>();
            ErrorMessage = null;
            try
            {
                players = await playerTable.ToCollectionAsync();
                foreach (var item in players)
                {
                    if (item.Image == null) { item.Image = "music.jpg"; }
                    pls.Add(item);
                }
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (HttpRequestException ex2)
            {
                ErrorMessage = ex2.Message;
            }           
            return pls;
        }

        public async Task AddNewPlayerAsync(Player pl)
        {
            ErrorMessage = null;
            try
            {
                var players = await playerTable.ToCollectionAsync();     // if its not a new player, just update to add this team to player's team list
                if(!players.Contains(pl))
                {
                    await playerTable.InsertAsync(pl);
                }                
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (HttpRequestException ex2)
            {
                ErrorMessage = ex2.Message;
            }
        }

        public void fillDb()
        {
            
            DropDB();
            List<string> samplePics = new List<string> { "Charlton.png", "fca.png", "Carpi.png", "Malaga.png", "music2.jpg", "music3.jpg" };
            foreach (var item in samplePics)
            {
                CopyToStorage(item);
            }
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var cnt = db.Table<Team>().Count();
                List<Team> tms = new List<Team>();
                tms.Add(new Team { TeamName = "Friday Astro", Location = "Amenities  Centre", Image = "Charlton.png" });
                tms.Add(new Team { TeamName = "Sunday Morning 5 Aside", Location = "Amenities  Centre", Image = "fca.png" });
                tms.Add(new Team { TeamName = "Astro Training", Location = "Amenities  Centre", Image = "Carpi.png" });
                tms.Add(new Team { TeamName = "Wednesday Celbridge", Location = "Celbridge Astro", Image = "Malaga.png" });
                foreach (var item in tms)
                {
                    db.Insert(item);
                }
                cnt = db.Table<Team>().Count();

                List<Player> plys = new List<Player>();
                plys.Add(new Player { PlayerName = "Mick Keane", PhNumber = "0876493789", Image = "music3.jpg" });
                plys.Add(new Player { PlayerName = "Paddy Whelan", PhNumber = "0876493789", Image = "music2.jpg" });
                plys.Add(new Player { PlayerName = "Liam Dempsey", PhNumber = "0876493789", Image = "music3.jpg" });
                plys.Add(new Player { PlayerName = "Mark Brennan", PhNumber = "0876493789", Image = "music2.jpg" });
                plys.Add(new Player { PlayerName = "John Dowling", PhNumber = "0876493789", Image = "music3.jpg" });
                foreach (var item in plys)
                {
                    db.Insert(item);
                }
            }
        }

        private async void CopyToStorage(string item)
        {
            try
            {
                string uri = "ms-appx:///Assets/" + item;
                var urii = new System.Uri(uri);
                StorageFile file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(urii);
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                var dataFolder = await local.CreateFolderAsync("Assets", CreationCollisionOption.OpenIfExists);
                try
                {
                    StorageFile t = await dataFolder.GetFileAsync(item);                    
                }
                catch (FileNotFoundException ex)
                {
                    CopyFile(file, dataFolder);
                }    
            }             
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public async void CopyFile(StorageFile stfile, StorageFolder dest)
        {
            await stfile.CopyAsync(dest);
        }

        private void DropDB()
        {
            try
            {                
                using (var db = new SQLite.SQLiteConnection(App.DBPath))
                {
                    db.DeleteAll<Team>();
                    
                }
            }
            catch (Exception ex)
            {
                string g = ex.InnerException.Message;
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
    }
}
