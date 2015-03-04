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
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;

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
        private MobileServiceCollection<RequestJoinTeam, RequestJoinTeam> requests;
        private MobileServiceCollection<RequestPlay, RequestPlay> playRequests;

        private IMobileServiceTable<Team> teamTable = App.MobileService.GetTable<Team>();
        private IMobileServiceTable<Player> playerTable = App.MobileService.GetTable<Player>();
        private IMobileServiceTable<RequestJoinTeam> RequestJoinTeamTable = App.MobileService.GetTable<RequestJoinTeam>();
        private IMobileServiceTable<RequestPlay> RequestPlayTable = App.MobileService.GetTable<RequestPlay>();

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

        public async Task<Player> GetThisPlayerAsync(string id)
        {
            Player ply = new Player();
            ErrorMessage = null;
            try
            {
                players = await playerTable.ToCollectionAsync();
                ply = players.Where(a => a.Id == id).FirstOrDefault();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (HttpRequestException ex2)
            {
                ErrorMessage = ex2.Message;
            }
            return ply;
        }
        
        public async Task<bool> AddNewPlayerAsync(Player pl, Team tm, string requestedBy)   // called from Add Player page. 
        {                                                               // either be a request to someone new to db or already in db 
            bool isNew = false;
            ErrorMessage = null;
            try
            {
                var players = await playerTable.ToCollectionAsync();
                var plCheck = players.Where(a => a.Id == pl.Id).FirstOrDefault();   // first thing is chek if new to db.            
                if(plCheck == null)
                {
                    await playerTable.InsertAsync(pl);      // if its a new user -- add to player table, then add as unconfirmed team request
                    isNew = true;
                    RequestJoinTeam rp = new RequestJoinTeam { TeamId = tm.Id, PlayerId = pl.Id, Confirmed = false, RequestedBy = requestedBy };
                    await RequestJoinTeamTable.InsertAsync(rp);
                }
                else           
                {
                    RequestJoinTeam rp = new RequestJoinTeam { TeamId = tm.Id, PlayerId = pl.Id, Confirmed = false, RequestedBy = requestedBy };
                    await RequestJoinTeamTable.InsertAsync(rp);
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
                else                       // else they recieved a request and need to be confirmed and added to requested team
                {
                    RequestJoinTeam rp = new RequestJoinTeam { TeamId = "t1", PlayerId = "0876493789", RequestedBy = "Hitler", Confirmed = false };
                    await RequestJoinTeamTable.InsertAsync(rp);

                    requests = await RequestJoinTeamTable.ToCollectionAsync();
                    var checkRequest = requests.Where(a => a.PlayerId == pl.Id && a.Confirmed == false).FirstOrDefault();
                    if (checkRequest != null)
                    {
                        await ConfirmJoinTeam(checkRequest,  plCheck);
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

        public async Task ConfirmJoinTeam(RequestJoinTeam checkRequest, Player playerToAdd)
        {
            Team team = await teamTable.LookupAsync(checkRequest.TeamId);
            string content = checkRequest.RequestedBy + " has requested you be a member of " + team.TeamName + "?";
            bool ok = await CheckWithUser(content);
            if (ok)
            {
                checkRequest.Confirmed = true;
                await RequestJoinTeamTable.UpdateAsync(checkRequest);

                var tms = await GetTeamsAsync();
                Team tm2Update = tms.Where(a => a.Id == checkRequest.TeamId).FirstOrDefault();
                tm2Update.Players.Add(playerToAdd);
                await UpdateTeamAsync(tm2Update);
            }
        }

        private async Task<bool> CheckWithUser(string content)
        {
            bool result = false;
            MessageDialog msgbox = new MessageDialog(content);
            msgbox.Commands.Clear();
            msgbox.Commands.Add(new UICommand { Label = "Yes", Id = 0 });
            msgbox.Commands.Add(new UICommand { Label = "No", Id = 1 });

            var res = await msgbox.ShowAsync();

            if ((int)res.Id == 0)
            {
                result = true;
            }
            if ((int)res.Id == 1)
            {
                result = false;
            }       
            return result;
        }

        public async Task SendRequest(RequestJoinTeam rp)
        {
            rp.RequestedBy = "the pope";
            rp.PlayerId = "0876493789";
            try
            {
                await RequestJoinTeamTable.InsertAsync(rp);
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

        public async Task<RequestJoinTeam> GetJoinTeamRequestrAsync(string id)   // gets the join request for this row id
        {
            RequestJoinTeam rjt = new RequestJoinTeam();
            ErrorMessage = null;
            try
            {
                requests = await RequestJoinTeamTable.ToCollectionAsync();
                rjt = requests.Where(a => a.Id == id).FirstOrDefault();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (HttpRequestException ex2)
            {
                ErrorMessage = ex2.Message;
            }
            return rjt;
        }

        public async Task SendPlayRequest(RequestPlay rp)
        {
            try
            {
                await RequestPlayTable.InsertAsync(rp);
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

        public async Task<RequestPlay> GetPlayRequestrAsync(string id)
        {
            RequestPlay rp = new RequestPlay();
            ErrorMessage = null;
            try
            {
                playRequests = await RequestPlayTable.ToCollectionAsync();
                rp = playRequests.Where(a => a.Id == id).FirstOrDefault();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (HttpRequestException ex2)
            {
                ErrorMessage = ex2.Message;
            }
            return rp;
        }

        public async Task ConfirmPlay(RequestPlay playRequest)
        {
            Team team = await teamTable.LookupAsync(playRequest.TeamId);
            string content = " Are you up for playing with " + team.TeamName + "?";
            bool ok = await CheckWithUser(content);
            if (ok)
            {
                playRequest.Confirmed = true;
                await RequestPlayTable.UpdateAsync(playRequest);
            }
        }

        private async void PaymentCheckCompleted()
        {
            MessageDialog dlg = null;
            
            dlg = new MessageDialog("Status is UNKNOWN", "SMS Provider");
        
            dlg.Commands.Add(new UICommand("OK"));
            dlg.DefaultCommandIndex = 0;
            dlg.CancelCommandIndex = 0;
            dlg.Options = MessageDialogOptions.None;

            CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { });

//            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => dlg.ShowAsync());

        }


        //public void fillDb()
        //{
            
        //    //DropDB();
        //    //List<string> samplePics = new List<string> { "Charlton.png", "fca.png", "Carpi.png", "Malaga.png", "music2.jpg", "music3.jpg" };
        //    //foreach (var item in samplePics)
        //    //{
        //    //    CopyToStorage(item);
        //    //}
        //    //using (var db = new SQLite.SQLiteConnection(App.DBPath))
        //    //{
        //    //    var cnt = db.Table<Team>().Count();
        //    //    List<Team> tms = new List<Team>();
        //    //    tms.Add(new Team { TeamName = "Friday Astro", Location = "Amenities  Centre" });
        //    //    tms.Add(new Team { TeamName = "Sunday Morning 5 Aside", Location = "Amenities  Centre" });
        //    //    tms.Add(new Team { TeamName = "Astro Training", Location = "Amenities  Centre"});
        //    //    tms.Add(new Team { TeamName = "Wednesday Celbridge", Location = "Celbridge Astro" });
        //    //    foreach (var item in tms)
        //    //    {
        //    //        db.Insert(item);
        //    //    }
        //    //    cnt = db.Table<Team>().Count();

        //    //    List<Player> plys = new List<Player>();
        //    //    plys.Add(new Player { PlayerName = "Mick Keane", PhNumber = "0876493789" });
        //    //    plys.Add(new Player { PlayerName = "Paddy Whelan", PhNumber = "0876493789" });
        //    //    plys.Add(new Player { PlayerName = "Liam Dempsey", PhNumber = "0876493789" });
        //    //    plys.Add(new Player { PlayerName = "Mark Brennan", PhNumber = "0876493789" });
        //    //    plys.Add(new Player { PlayerName = "John Dowling", PhNumber = "0876493789" });
        //    //    foreach (var item in plys)
        //    //    {
        //    //        db.Insert(item);
        //    //    }
        //    //}
        //}

        //private async void CopyToStorage(string item)
        //{
        //    try
        //    {
        //        string uri = "ms-appx:///Assets/" + item;
        //        var urii = new System.Uri(uri);
        //        StorageFile file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(urii);
        //        StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
        //        var dataFolder = await local.CreateFolderAsync("Assets", CreationCollisionOption.OpenIfExists);
        //        try
        //        {
        //            StorageFile t = await dataFolder.GetFileAsync(item);                    
        //        }
        //        catch (FileNotFoundException ex)
        //        {
        //            CopyFile(file, dataFolder);
        //        }    
        //    }             
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }            
        //}

        //public async void CopyFile(StorageFile stfile, StorageFolder dest)
        //{
        //    await stfile.CopyAsync(dest);
        //}

        //private void DropDB()
        //{
        //    //try
        //    //{
        //    //    using (var db = new SQLite.SQLiteConnection(App.DBPath))
        //    //    {
        //    //        db.DeleteAll<Team>();

        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    string g = ex.InnerException.Message;
        //    //}
        //}

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
