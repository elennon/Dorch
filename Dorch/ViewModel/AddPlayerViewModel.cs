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
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace Dorch.ViewModel
{
    public class AddPlayerViewModel : ViewModelBase, INavigable, INotifyPropertyChanged
    {

        private IRepository repo = new Repository();
        private INavigationService _navigationService;
        private byte[] ImageBytes = null;

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
        public GalaSoft.MvvmLight.Command.RelayCommand ShowAllPlayersCommand { get; set; }
        public GalaSoft.MvvmLight.Command.RelayCommand ContactsListCommand { get; set; }

        public AddPlayerViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            PlayerName = "";
            Phone = "";
            Image = "";
            this.AddPlayerCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnAddPlayerCommand);
            this.FilePickerCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnFilePickerCommand);
            this.ShowAllPlayersCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnShowAllPlayersCommand);
            this.ContactsListCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnContactsListCommand);
        }

        private async void OnContactsListCommand()        
        {
            var contactPicker = new Windows.ApplicationModel.Contacts.ContactPicker();
            contactPicker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.PhoneNumber);
            Contact contact = await contactPicker.PickContactAsync();

            var pic = await GetThumbNail(contact);
            var ph = contact.Phones.FirstOrDefault();
            string id = contact.DisplayName + "," + ph.Number;
            this.PlayerName = contact.DisplayName;
            this.Phone = ph.Number;
            ImageBytes = pic;
            SaveNewPlayer(id);

        }

        private void OnAddPlayerCommand()
        {
            if (PlayerName == "") { Toast("Fill in name please."); return; }
            else if (Phone == "") { Toast("Fill in Phone No. please."); return; }
            else if (Image == "") { Image = "person-icon.png"; }

            string id = PlayerName + "," + Phone;

            SaveNewPlayer(id);
        }

        private async void SaveNewPlayer(string id)
        {
            var u = thisTeam.Players.Where(a => a.Id == id).Count();    // check if player already in this team
            if (u < 1)
            {
                Player pl = new Player
                {
                    Id = id,
                    PlayerName = this.PlayerName,
                    PhNumber = this.Phone,                    
                    Image = ImageBytes,                    
                };

                bool isNew = await repo.AddNewPlayerAsync(pl, thisTeam);          // isNew true if player is new to db and not in any other teams
                
                if (isNew)
                {
                    Windows.ApplicationModel.Chat.ChatMessage msg = new Windows.ApplicationModel.Chat.ChatMessage();
                    msg.Body = "Hi " + PlayerName + ". " + ((App)Application.Current).UserName + " sent this request to join "
                        + thisTeam.TeamName + ". You can download the app here to get started. http://itsligo.ie/ ";
                    msg.Recipients.Add("0892392943");

                    await Windows.ApplicationModel.Chat.ChatMessageManager.ShowComposeSmsMessageAsync(msg);
                }
                else
                {
                    // todo -- send push request to join
                }
                PlayerName = "";
                Phone = "";
                Image = "";
                _navigationService.NavigateTo("ViewTeam", thisTeam);
            }
            else
            {
                MessageDialog msgbox = new MessageDialog(PlayerName + " is already in this team!!");
                msgbox.Commands.Clear();
                msgbox.Commands.Add(new UICommand { Label = "OK", Id = 0 });
                var res = await msgbox.ShowAsync();
            }            
        }

       
        public async Task<byte[]> GetThumbNail(Contact contact)
        {
            byte[] buffer = null;
            if (contact.Thumbnail != null)
            {
                IRandomAccessStreamWithContentType stream = await contact.Thumbnail.OpenReadAsync();
                if (stream != null && stream.Size > 0)
                {
                    buffer = new byte[stream.Size];
                    int bytRead = 0;
                    bytRead = await (stream.AsStreamForRead()).ReadAsync(buffer, 0, buffer.Length);
                }               
            }
            return buffer;
        }

        private void OnShowAllPlayersCommand()      // choose from another team
        {
            _navigationService.NavigateTo("ShowAllPlayers", thisTeam);
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
                Image = args.Files[0].Name;
                ImageBytes = await ReadImage.GetImageBytes(args.Files[0]);
                    //= await TransferToStorage(args.Files[0]);
            }
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
