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
        private MobileServiceCollection<RequestPending, RequestPending> requests;

        private IMobileServiceTable<Team> teamTable = App.MobileService.GetTable<Team>();
        private IMobileServiceTable<Player> playerTable = App.MobileService.GetTable<Player>();
        private IMobileServiceTable<RequestPending> requestTable = App.MobileService.GetTable<RequestPending>();

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
                teams = await teamTable.ToCollectionAsync();
                foreach (var item in teams)
                {                    
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

        //public async Task<Team> GetATeamAsync(Team tm)
        //{            
        //    ErrorMessage = null;
        //    try
        //    {
        //        var team = await teamTable.LookupAsync(tm);
        //        foreach (var item in teams)
        //        {
        //            if (item.Image == null) { item.Image = "Charlton.png"; }
        //            tms.Add(item);
        //        }
        //    }
        //    catch (MobileServiceInvalidOperationException ex)
        //    {
        //        ErrorMessage = ex.Message;
        //    }
        //    catch (HttpRequestException ex2)
        //    {
        //        ErrorMessage = ex2.Message;
        //    }
        //    return tms;
        //}

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

        public async Task<bool> AddNewPlayerAsync(Player pl, Team tm)   // called from Add Player page. 
        {                                                               // either be a request to someone new to db or already in db 
            bool isNew = false;
            ErrorMessage = null;
            try
            {
                var players = await playerTable.ToCollectionAsync();
                var plCheck = players.Where(a => a.Id == pl.Id).FirstOrDefault();   // first thing is chek if new to db.            
                if(plCheck == null)
                {
                    await playerTable.InsertAsync(pl);      // if its a new user -- add to player table
                    isNew = true;
                    RequestPending rp = new RequestPending { TeamId = tm.Id, PlayerId = pl.Id, Confirmed = false };
                    await requestTable.InsertAsync(rp);
                }
                else           
                {
                    RequestPending rp = new RequestPending { TeamId = tm.Id, PlayerId = pl.Id, Confirmed = false };
                    await requestTable.InsertAsync(rp);
                }
                return isNew;
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
            catch (HttpRequestException ex2)
            {
                ErrorMessage = ex2.Message;
                return false;
            }
        }

        public async Task AddNewPlayerOnSignUpAsync(Player pl)    // this method to add a new user regestering for the first time (called from signIn page)
        {                                                       // they're either responding to a request or just new
            ErrorMessage = null;
            try
            {
                var players = await playerTable.ToCollectionAsync();
                var plCheck = players.Where(a => a.Id == pl.Id).FirstOrDefault();
                if (plCheck == null)                // if null they'r new  
                {
                    await playerTable.InsertAsync(pl);                        
                } 
                else                                // else they recieved a request and need to be confirmed and added to requested team
                {
                    requests = await requestTable.ToCollectionAsync();
                    var checkRequest = requests.Where(a => a.PlayerId == pl.Id && a.Confirmed == false).FirstOrDefault();
                    if (checkRequest != null)
                    {
                        checkRequest.Confirmed = true;
                        await requestTable.UpdateAsync(checkRequest);

                        var tms = await GetTeamsAsync();
                        Team tm2Update = tms.Where(a => a.Id == checkRequest.TeamId).FirstOrDefault();
                        tm2Update.Players.Add(plCheck);
                        await UpdateTeamAsync(tm2Update);
                    }
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

        public async Task SendRequest(RequestPending rp)
        {
            try
            {
                await requestTable.InsertAsync(rp);
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
                tms.Add(new Team { TeamName = "Friday Astro", Location = "Amenities  Centre" });
                tms.Add(new Team { TeamName = "Sunday Morning 5 Aside", Location = "Amenities  Centre" });
                tms.Add(new Team { TeamName = "Astro Training", Location = "Amenities  Centre"});
                tms.Add(new Team { TeamName = "Wednesday Celbridge", Location = "Celbridge Astro" });
                foreach (var item in tms)
                {
                    db.Insert(item);
                }
                cnt = db.Table<Team>().Count();

                List<Player> plys = new List<Player>();
                plys.Add(new Player { PlayerName = "Mick Keane", PhNumber = "0876493789" });
                plys.Add(new Player { PlayerName = "Paddy Whelan", PhNumber = "0876493789" });
                plys.Add(new Player { PlayerName = "Liam Dempsey", PhNumber = "0876493789" });
                plys.Add(new Player { PlayerName = "Mark Brennan", PhNumber = "0876493789" });
                plys.Add(new Player { PlayerName = "John Dowling", PhNumber = "0876493789" });
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
