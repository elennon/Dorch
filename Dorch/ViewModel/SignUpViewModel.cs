using Dorch.Common;
using Dorch.DAL;
using Dorch.Model;
using Dorch.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace Dorch.ViewModel
{
   
    public class SignUpViewModel : ViewModelBase, INotifyPropertyChanged
    {

        private IRepository repo = new Repository();
        private INavigationService _navigationService;
        private CoreApplicationView view = CoreApplication.GetCurrentView();
        public SignUp sUp;
        
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; NotifyPropertyChanged("UserName"); }
        }
        private string _phNumber;
        public string PhNumber
        {
            get { return _phNumber; }
            set { _phNumber = value; NotifyPropertyChanged("PhNumber"); }
        }
        private string _image;
        public string Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged("Image"); }
        }

        public GalaSoft.MvvmLight.Command.RelayCommand SetUpCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand FilePickerCommand { get; set; }

        public SignUpViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            UserName = "";
            PhNumber = "";
            Image = "";
            this.SetUpCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnSetUpCommand);
            this.FilePickerCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnFilePickerCommand);
            AddDefaultImages();
        }

        private void AddDefaultImages()
        {
            List<string> samplePics = new List<string> { "Charlton.png", "fca.png", "Carpi.png", "Malaga.png", "music2.jpg", "music3.jpg" };
            foreach (var item in samplePics)
            {
                CopyToStorage(item);
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

        private void OnFilePickerCommand()
        {
            string ImagePath = string.Empty;
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.ViewMode = PickerViewMode.Thumbnail;

            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".jpg");

            filePicker.PickSingleFileAndContinue();
            view.Activated += viewActivated; 
        }

        private async void OnSetUpCommand()
        {
            if (UserName == "") { ShowPop(); return; }
            else if (PhNumber == "") { ShowPop(); return; }
            else if (Image == "") { Image = "Carpi.png"; }
            string id = UserName + "," + PhNumber;
            Player pl = new Player { Id = id, PlayerName = this.UserName, PhNumber = this.PhNumber, Image = this.Image };
            await repo.AddNewPlayerAsync(pl);

            ((App)Application.Current).UserName = UserName;
            AppSettings.SaveSettingsValue(Constants.UserName, UserName);

            ((App)Application.Current).UserId = id;
            AppSettings.SaveSettingsValue(Constants.UserId, id);
            UserName = "";
            PhNumber = "";
            Image = "";            
            _navigationService.NavigateTo("MainPage");
        }

        private async void ShowPop()
        {
            sUp = SignUp.signUpPage;
            if (!sUp.popup.IsOpen) { sUp.popup.IsOpen = true; }
            await Task.Delay(TimeSpan.FromSeconds(2));
            if (sUp.popup.IsOpen) { sUp.popup.IsOpen = false; }
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

        private async void viewActivated(CoreApplicationView sender, IActivatedEventArgs args1)
        {
            FileOpenPickerContinuationEventArgs args = args1 as FileOpenPickerContinuationEventArgs;

            if (args != null)
            {
                if (args.Files.Count == 0) return;

                view.Activated -= viewActivated;
                //Image = args.Files[0].Name;
                Image =await TransferToStorage(args.Files[0]);
            }
        }

        private async Task<string> TransferToStorage(object file)
        {            
            StorageFile stfile = (StorageFile)file;
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;          
            var dataFolder = await local.CreateFolderAsync("Assets", CreationCollisionOption.OpenIfExists);
            try
            {
                StorageFile t = await dataFolder.GetFileAsync(stfile.Name);
                return stfile.Name;
            }
            catch (FileNotFoundException ex)
            {
                CopyFile(stfile,dataFolder);
                return stfile.Name;
            }            
        }

        public async void CopyFile(StorageFile stfile, StorageFolder dest)
        {
            await stfile.CopyAsync(dest);
        }

    }
}
