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
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Notifications;

namespace Dorch.ViewModel
{
    public class AddPlayerViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {

        private IRepository repo = new Repository();
        private INavigationService _navigationService;
        public Team thisTeam { get; set; }

        private string _playerName;
        public string PlayerName
        {
            get { return _playerName; }
            set { _playerName = value; NotifyPropertyChanged("PlayerName"); }
        }
        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; NotifyPropertyChanged("Phone"); }
        }
        private string _image;
        public string Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged("Image"); }
        }
        private CoreApplicationView view = CoreApplication.GetCurrentView();
        public AddPlayer ap;

        public GalaSoft.MvvmLight.Command.RelayCommand AddPlayerCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand FilePickerCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand ToastCommand { get; set; }

        public AddPlayerViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            PlayerName = "";
            Phone = "";
            Image = "";
            this.AddPlayerCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnAddPlayerCommand);
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

        private async void OnAddPlayerCommand()
        {
            if (PlayerName == "") { Toast("Fill in name please."); return; }
            else if (Phone == "") { Toast("Fill in Phone No. please."); return; }
            else if (Image == "") { Image = "music2.jpg"; }

            List<Team> tms = new List<Team>();
            tms.Add(thisTeam);
            string id = PlayerName + "," + Phone;
            Player pl = new Player { Id = id, PlayerName = this.PlayerName, PhNumber = this.Phone, Image = this.Image };//, Teams = tms
            
            await repo.AddNewPlayerAsync(pl);
            thisTeam.Players.Add(pl);
            await repo.UpdateTeamAsync(thisTeam);
            
            PlayerName = "";
            Phone = "";
            Image = "";
            _navigationService.NavigateTo("ViewTeam", thisTeam);
        }

        private async void ShowPop()
        {
            ap = AddPlayer.addPlayerPage;
            if (!ap.popup.IsOpen) { ap.popup.IsOpen = true; }
            await Task.Delay(TimeSpan.FromSeconds(2));
            if (ap.popup.IsOpen) { ap.popup.IsOpen = false; }
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
                CopyFile(stfile, dataFolder);
                return stfile.Name;
            }
        }
        public async void CopyFile(StorageFile stfile, StorageFolder dest)
        {
            await stfile.CopyAsync(dest);
        }

        public void Activate(object parameter)
        {
            if (parameter is Team)
            {
                thisTeam = ((Team)parameter);
            }
        }

        public void Deactivate(object parameter)
        {
            //throw new NotImplementedException();
        }
    }
}
