using Dorch.Common;
using Dorch.DAL;
using Dorch.Model;
using Dorch.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.WindowsAzure.MobileServices;
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
using Windows.UI.Popups;
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

        private string _imageNme;
        public string ImageNme
        {
            get { return _imageNme; }
            set { _imageNme = value; NotifyPropertyChanged("ImageNme"); }
        }

        private byte[] _image;
        public byte[] Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged("Image"); }
        }

        public GalaSoft.MvvmLight.Command.RelayCommand<RoutedEventArgs> LoadCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand SetUpCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand FilePickerCommand { get; set; }

        
        public SignUpViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            UserName = "";
            PhNumber = "";
            this.LoadCommand = new GalaSoft.MvvmLight.Command.RelayCommand<RoutedEventArgs>(OnLoadCommand);
            this.SetUpCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnSetUpCommand);
            this.FilePickerCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnFilePickerCommand);            
        }

        private async void OnLoadCommand(RoutedEventArgs obj)
        {
            //await AuthenticateAsync();
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
            
            Player pl = new Player { Id = this.PhNumber, PlayerName = this.UserName, PhNumber = this.PhNumber, Image = Image };
            await repo.AddNewPlayerOnSignUpAsync(pl);   

            ((App)Application.Current).UserName = UserName;
            AppSettings.SaveSettingsValue(Constants.UserName, UserName);

            ((App)Application.Current).UserId = this.PhNumber;
            AppSettings.SaveSettingsValue(Constants.UserId, this.PhNumber);
            UserName = "";
            PhNumber = "";
            ImageNme = "";            
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
                Image = await ReadImage.GetImageBytes(args.Files[0]);
                ImageNme = args.Files[0].DisplayName;
            }
        }

        //private async Task<string> TransferToStorage(object file)
        //{            
        //    StorageFile stfile = (StorageFile)file;
        //    StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;          
        //    var dataFolder = await local.CreateFolderAsync("Assets", CreationCollisionOption.OpenIfExists);
        //    try
        //    {
        //        StorageFile t = await dataFolder.GetFileAsync(stfile.Name);
        //        return stfile.Name;
        //    }
        //    catch (FileNotFoundException ex)
        //    {
        //        CopyFile(stfile,dataFolder);
        //        return stfile.Name;
        //    }            
        //}

        //public async void CopyFile(StorageFile stfile, StorageFolder dest)
        //{
        //    await stfile.CopyAsync(dest);
        //}

        //private void AddDefaultImages()
        //{
        //    List<string> samplePics = new List<string> { "Charlton.png", "fca.png", "Carpi.png", "Malaga.png", "person-icon.png" };
        //    foreach (var item in samplePics)
        //    {
        //        CopyToStorage(item);
        //    }
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

    }
}
