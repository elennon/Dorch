using Dorch.Common;
using Dorch.DAL;
using Dorch.Model;
using Dorch.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Phone.PersonalInformation;
using Windows.Storage;
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

        private void OnTextCommand()
        {           
            _navigationService.NavigateTo("ShowAllPlayers");
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
            repo.fillDb();
        }

        private void OnEditCommand(Team obj)
        {
            editMode = !editMode;               // switch edit mode
            IsVisible = !IsVisible;
            //if (IsVisible) _isVisible = false;
            //else IsVisible = true;
        }

        private void OnDeleteSelectedCommand(Team obj)
        {
            Teams.Remove(obj);
            repo.DeleteTeam(obj);
        }

        private async void OnLoadCommand(RoutedEventArgs obj)
        {
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
