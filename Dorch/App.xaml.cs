using Dorch.Common;
using Dorch.DAL;
using Dorch.Model;
using Dorch.Utilities;
using Dorch.View;
using Dorch.ViewModel;
using Microsoft.WindowsAzure.Messaging;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Dorch
{
   
    public sealed partial class App : Application
    {
        private IRepository repo = new Repository();
        public static string DBPath = string.Empty;
        private TransitionCollection transitions;
        public ViewTeamViewModel vtvm { get; set; }
       
        private string _userName;
        public string UserName
        {
            get
            {
                object value = AppSettings.ReadResetSettingsValue(Constants.UserName);
                if (value == null)
                {
                    return _userName = null;
                }
                else
                {
                    _userName = (string)value;  
                    return _userName;
                }
            }
            set
            {
                _userName = value;
            }
        }

        private string _userId;
        public string UserId
        {
            get
            {
                object value = AppSettings.ReadResetSettingsValue(Constants.UserId);
                if (value == null)
                {
                    return _userId = null;
                }
                else
                {
                    _userId = (string)value;
                    return _userId;
                }
            }
            set
            {
                _userId = value;
            }
        }

        public static NotificationHub Hub = new NotificationHub("teammanagerhub", "Endpoint=sb://teammanagerhub5-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=G+jow8KKiawBOFp3cRmmhvlYqAk7Bn56JN5dW5a0xbM=");        

        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://teammanager.azure-mobile.net/",
            "dFKsDxvkgKoEMauxHLOHAsuxJxAlgz48"
        );

        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;            
        }

        private async void InitNotificationsAsync()
        {
            System.Exception exception = null;
            try
            {
                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                channel.PushNotificationReceived += OnPushNotification;     // registers a method to recieve push messages when app is running
                var tags = new List<string> { "0876493789" };
                await Hub.RegisterNativeAsync(channel.Uri, tags);                
            }
            catch (System.Exception ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                var dialog = new MessageDialog(exception.Message, "Registering Channel URI");
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
        }
        string content = null;
        private async void OnPushNotification(PushNotificationChannel sender, PushNotificationReceivedEventArgs e)
        {
            string notificationContent = String.Empty;
            switch (e.NotificationType)
            {
                case PushNotificationType.Badge:
                    notificationContent = e.BadgeNotification.Content.GetXml();
                    break;

                case PushNotificationType.Tile:
                    notificationContent = e.TileNotification.Content.GetXml();
                    break;

                case PushNotificationType.Toast:
                    notificationContent = e.ToastNotification.Content.GetXml();
                    break;

                case PushNotificationType.Raw:
                    notificationContent = e.RawNotification.Content;
                    break;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(toast));
            byte requestType;
            string mContent = "";
            using (TextReader reader = new StringReader(notificationContent))
            {
                var t = (toast)serializer.Deserialize(reader);
                requestType = t.visual.binding.text.id;
                mContent = t.visual.binding.text.Value;               
            }
            e.Cancel = true;
            if (requestType == 2)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    RequestPlay rp = await repo.GetPlayRequestrAsync(mContent);
                    await repo.ConfirmPlay(rp);
                    Player p = await repo.GetThisPlayerAsync(rp.PlayerId);
                    if(vtvm != null)
                    vtvm.StatusPlayers.Add(p);
                });            
                
            }
        }

        private void ChangeTile2()
        {
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText01);

            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = "otification";

            XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/Malaga.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "red graphic");

            XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text04);
            XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("text");
            squareTileTextAttributes[0].AppendChild(squareTileXml.CreateTextNode("Hello World! My very own tile notification"));
            IXmlNode node = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
            tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(tileXml);

            tileNotification.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(10);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);

        }

        private void ChangeTile()
        {
            //XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text01);   //TileWide310x150ImageAndText01

            var tileTemplate = TileTemplateType.TileSquare150x150Image;
            var tileXml = TileUpdateManager.GetTemplateContent(tileTemplate);

            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = "Hello World! My very own tile notification";

            XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/logo.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "red graphic");

            XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Image);
            XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("image");
            ((XmlElement)squareTileTextAttributes[0]).SetAttribute("src", "ms-appx:///Assets/logo2.png");
            IXmlNode node = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
            tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            //XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text04);
            //XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("text");
            //squareTileTextAttributes[0].AppendChild(squareTileXml.CreateTextNode("notification recieved !!!"));
            //IXmlNode node = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
            //tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(tileXml);

            tileNotification.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(10);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            //_navigationService.NavigateTo("AddTeam");         TileWideText02/TileWide310x150Text02
        }

        private void ResetStartTile()
        {
            var tileTemplate = TileTemplateType.TileWide310x150Text02;
            var tileXml = TileUpdateManager.GetTemplateContent(tileTemplate);
            TileNotification tileNotification = new TileNotification(tileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
         
            ResetStartTile();
            InitNotificationsAsync();
            await TaskHelper.RegisterTask();
            CheckMessages();
            var idd = this.UserId;

            if(string.IsNullOrEmpty(UserName))
            {
                GoToSingIn(e);
            }
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }
                
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private async void CheckMessages()      // todo replace hard coded values
        {
            object value = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.JoinTeamRequest);
            if (value != null)
            {
                var joinTeammessage = (bool)value;      // if a join team request was reccieved and save in storage 
                if (joinTeammessage)                     //-- get the request id and get user to confirm they want to join, then add to team
                {
                    object mContent = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.JoinTeamRequestContent);
                    if (mContent != null)
                    {
                        string rjtId = ((string)mContent).Split(',')[1];
                        //RequestJoinTeam rjt = await repo.GetJoinTeamRequestrAsync(((App)Application.Current).UserId);         // commented for testing
                        RequestJoinTeam rjt = await repo.GetJoinTeamRequestrAsync(rjtId);
                        Player thisPlayer = await repo.GetThisPlayerAsync("0876493789");
                        await repo.ConfirmJoinTeam(rjt, thisPlayer);
                    }
                }
            }
            object playR = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.PlayRequest);
            if (playR != null)
            {
                var prMessage = (bool)playR;      // if a join team request was reccieved and save in storage 
                if (prMessage)                     //-- get the request id and get user to confirm they want to join, then add to team
                {
                    object mContent = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.PlayRequestContent);
                    if (mContent != null)
                    {
                        string messageId = ((string)mContent).Split(',')[1];
                        //RequestJoinTeam rjt = await repo.GetJoinTeamRequestrAsync(((App)Application.Current).UserId);         // commented for testing
                        RequestPlay rp = await repo.GetPlayRequestrAsync(messageId);
                        await repo.ConfirmPlay(rp);
                    }
                }
            }
        }

        private void GoToSingIn(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;
               
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }
                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

      
                if (!rootFrame.Navigate(typeof(SignUp), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            Window.Current.Activate();
        }

        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }

    public static class TaskHelper
    {
        private static string TASK_NAME = "UpdateTile";
        private static string TASK_ENTRY = "BackgroundUpdateTile.UpdateTile";

        public static async Task RegisterTask()
        {
            var result = await BackgroundExecutionManager.RequestAccessAsync();
            if (result == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                result == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                foreach (var t in BackgroundTaskRegistration.AllTasks)
                {
                    //if (t.Value.Name == TASK_NAME)
                        t.Value.Unregister(true);
                }
                var builder = new BackgroundTaskBuilder { Name = TASK_NAME, TaskEntryPoint = TASK_ENTRY };
                var trigger = new PushNotificationTrigger();
                builder.SetTrigger(trigger);
                try
                {
                    IBackgroundTaskRegistration task = builder.Register();
                }
                catch (Exception)
                {

                    throw;
                }
                //IBackgroundTaskRegistration task = builder.Register();
              //  task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
            }

        }

        private static void OnCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            throw new NotImplementedException();
        }

        //private void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        //{
        //    var settings = ApplicationData.Current.LocalSettings;
        //    var key = task.TaskId.ToString();
        //    var message = settings.Values[key].ToString();
        //    //UpdateUIExampleMethod(message);
        //}

    }
}