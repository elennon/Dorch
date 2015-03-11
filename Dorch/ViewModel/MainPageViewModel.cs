using Dorch.Common;
using Dorch.DAL;
using Dorch.Model;
using Dorch.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Contacts;
using Windows.Phone.PersonalInformation;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Dorch.ViewModel
{
    public class MainPageViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {

        private IRepository repo = new Repository();
        private INavigationService _navigationService;
        private bool editMode = false;

        private ObservableCollection<Team> _teams;
        public ObservableCollection<Team> Teams
        {
            get
            {
                return _teams;
            }
            set
            {
                if (_teams != value)
                {
                    _teams = value;
                    NotifyPropertyChanged("Teams");
                }
            }
        }
        private bool _isVisible = false;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; NotifyPropertyChanged("IsVisible"); }
        }

        public RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public RelayCommand<Team> ItemSelectedCommand { get; set; }
        public RelayCommand<Team> EditCommand { get; set; }
        public RelayCommand<Team> DeleteSelectedCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand FillDbCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand AddTeamCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand TextCommand { get; set; }

        //private MobileServiceUser user;
        //private async System.Threading.Tasks.Task AuthenticateAsync()
        //{
        //    while (user == null)
        //    {
        //        string message;
        //        try
        //        {
        //            // Change 'MobileService' to the name of your MobileServiceClient instance.
        //            // Sign-in using Facebook authentication.
        //            user = await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Google);

        //            message =
        //                string.Format("You are now signed in - {0}", user.UserId);
        //        }
        //        catch (InvalidOperationException)
        //        {
        //            message = "You must log in. Login Required";
        //        }

        //        var dialog = new MessageDialog(message);
        //        dialog.Commands.Add(new UICommand("OK"));
        //        await dialog.ShowAsync();
        //    }
        //}

        //private async System.Threading.Tasks.Task AuthenticateAsync()
        //{
        //    string message;
        //     This sample uses the Facebook provider.
        //    var provider = "Google";

        //     Use the PasswordVault to securely store and access credentials.
        //    PasswordVault vault = new PasswordVault();
        //    PasswordCredential credential = null;

        //    while (credential == null)
        //    {
        //        try
        //        {
        //             Try to get an existing credential from the vault.
        //            credential = vault.FindAllByResource(provider).FirstOrDefault();
        //        }
        //        catch (Exception)
        //        {
        //             When there is no matching resource an error occurs, which we ignore.
        //        }

        //        if (credential != null)
        //        {
        //             Create a user from the stored credentials.
        //            user = new MobileServiceUser(credential.UserName);
        //            credential.RetrievePassword();
        //            user.MobileServiceAuthenticationToken = credential.Password;

        //             Set the user from the stored credentials.
        //            App.MobileService.CurrentUser = user;

        //            try
        //            {
        //                 Try to return an item now to determine if the cached credential has expired.
        //                await App.MobileService.GetTable<RequestPending>().Take(1).ToListAsync();
        //            }
        //            catch (MobileServiceInvalidOperationException ex)
        //            {
        //                if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //                {
        //                     Remove the credential with the expired token.
        //                    vault.Remove(credential);
        //                    credential = null;
        //                    continue;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            try
        //            {
        //                 Login with the identity provider.
        //                user = await App.MobileService.LoginAsync(provider);


        //                 Create and store the user credentials.
        //                credential = new PasswordCredential(provider,
        //                    user.UserId, user.MobileServiceAuthenticationToken);
        //                vault.Add(credential);
        //            }
        //            catch (MobileServiceInvalidOperationException ex)
        //            {
        //                message = "You must log in. Login Required";
        //            }
        //        }
        //        message = string.Format("You are now logged in - {0}", user.UserId);
        //        var dialog = new MessageDialog(message);
        //        dialog.Commands.Add(new UICommand("OK"));
        //        await dialog.ShowAsync();
        //    }
        //}

        public MainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            //Teams = new ObservableCollection<Team>(repo.GetTeams());

            this.LoadCommand = new RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.DeleteSelectedCommand = new RelayCommand<Team>(OnDeleteSelectedCommand);
            this.EditCommand = new RelayCommand<Team>(OnEditCommand);
            this.FillDbCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnFillDbCommand);
            this.AddTeamCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnAddTeamCommand);
            this.ItemSelectedCommand = new RelayCommand<Team>(OnItemSelectedCommand);
            this.TextCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnTextCommand);
        }
        private async void regClick()
        {
            Debug.WriteLine("Registering task");
            var taskRegistered = false;
            var exampleTaskName = "UpdateTile";

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == exampleTaskName)
                {
                    //taskRegistered = true;
                    task.Value.Unregister(true);
                    break;
                }
            }

            await BackgroundExecutionManager.RequestAccessAsync();
            if (!taskRegistered)
            {
                Debug.WriteLine("Registering task inside");
                var builder = new BackgroundTaskBuilder();
                builder.Name = exampleTaskName;
                builder.TaskEntryPoint = "BackgroundUpdateTile.UpdateTile";
                builder.SetTrigger(new PushNotificationTrigger());
                //builder.SetTrigger(new SystemTrigger(SystemTriggerType.TimeZoneChange, false));
                //builder.SetTrigger(new SystemTrigger(SystemTriggerType.NetworkStateChange, false));
                BackgroundTaskRegistration task = builder.Register();
                await new MessageDialog("Task registered!").ShowAsync();
            }
        }
        private async void OnLoadCommand(RoutedEventArgs obj)
        {
            //regClick();
            //await AuthenticateAsync();
            //App.InitNotificationsAsync();
            List<Team> lst = await repo.GetTeamsAsync();
            Teams = new ObservableCollection<Team>(lst);
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            var dataFolder = await local.CreateFolderAsync("Assets", CreationCollisionOption.OpenIfExists);
            foreach (var item in Teams)
            {
                //     item.ImageSource = await ReadImage.MakeImage(item.Image, dataFolder);
                //item.ImageSource = await MakeImage(item.Image, dataFolder);
            }
        }

        private void OnTextCommand()
        {
            //RequestJoinTeam rp = new RequestJoinTeam { PlayerId = "0876493789", TeamId = "t3" };
            //await repo.SendRequest(rp);
            _navigationService.NavigateTo("BlankPage1");
        }

        private void OnItemSelectedCommand(Team obj)
        {
            if (!editMode)
            { _navigationService.NavigateTo("ViewTeam", obj); }
        }

        private void OnAddTeamCommand()
        {
            _navigationService.NavigateTo("AddTeam");
        }

        private void OnFillDbCommand()
        {
            //repo.fillDb();
        }

        private void OnEditCommand(Team obj)
        {
            editMode = !editMode;               // switch edit mode
            IsVisible = !IsVisible;
        }

        private void OnDeleteSelectedCommand(Team obj)
        {
            Teams.Remove(obj);
            repo.DeleteTeam(obj);
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

        public void Activate(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Deactivate(object parameter)
        {
            throw new NotImplementedException();
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
