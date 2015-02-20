using Dorch.Common;
using Dorch.DAL;
using Dorch.Model;
using Dorch.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
using Windows.UI.Popups;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;


namespace Dorch.ViewModel
{
   
    public class AddTeamViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {

        private IRepository repo = new Repository();
        private INavigationService _navigationService;
        
        private string _teamName;
        public string TeamName
        {
            get { return _teamName; }
            set { _teamName = value; NotifyPropertyChanged("TeamName"); }
        }
        private string _location;
        public string Location
        {
            get { return _location; }
            set { _location = value; NotifyPropertyChanged("Location"); }
        }
        private string _image;
        public string Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged("Image"); }
        }
        private CoreApplicationView view = CoreApplication.GetCurrentView();
        public AddTeam at;

        public GalaSoft.MvvmLight.Command.RelayCommand AddTeamCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand FilePickerCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand ToastCommand { get; set; }

        public AddTeamViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            TeamName = "";
            Location = "";
            Image = "";
            this.AddTeamCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnAddTeamCommand);
            this.FilePickerCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnFilePickerCommand);
            this.ToastCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnToastCommand);
        }

        private void OnToastCommand()
        {
            
        }

        private void Toast(string txt)
        {
            ToastTemplateType toastType = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastType);

            XmlNodeList toastTextElement = toastXml.GetElementsByTagName("text");
            toastTextElement[0].AppendChild(toastXml.CreateTextNode(txt));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "short");

            XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.Reminder");
            toastNode.AppendChild(audio);

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
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

        private async void OnAddTeamCommand()
        {
            if (TeamName == "") { Toast("Fill in name please."); return; }
            else if (Location == "") { Toast("Fill in location please."); return; }
            else if (Image == "") { Image = "Carpi.png"; }
            Team tm = new Team { TeamName = this.TeamName, Location = this.Location, Image = this.Image };
            await repo.addNewTeamAsync(tm);
            TeamName = "";
            Location = "";
            Image = "";
            _navigationService.NavigateTo("MainPage");
        }

        private async void ShowPop()
        {
            at = AddTeam.addTeamPage;
            if (!at.popup.IsOpen) { at.popup.IsOpen = true; }
            await Task.Delay(TimeSpan.FromSeconds(2));
            if (at.popup.IsOpen) { at.popup.IsOpen = false; }
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
                Image = await TransferToStorage(args.Files[0]);
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

        public void Activate(object parameter)
        {
            throw new NotImplementedException();
            
        }

        public void Deactivate(object parameter)
        {
            throw new NotImplementedException();
        }
    }

}
