using Dorch.Common;
using Dorch.Model;
using Dorch.View;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
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
        public static string DBPath = string.Empty;
        private TransitionCollection transitions;
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
                channel.PushNotificationReceived += OnPushNotification;

                await MobileService.GetPush().RegisterNativeAsync(channel.Uri);
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
        private void OnPushNotification(PushNotificationChannel sender, PushNotificationReceivedEventArgs e)
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
            using (TextReader reader = new StringReader(notificationContent))
            {
                var t = (toast)serializer.Deserialize(reader);
                var g = t.visual.binding.text.id;
            }
            //using (XmlReader reader = XmlReader.Create(notificationContent))
            //{
            //    var t = (toast)serializer.Deserialize(reader);
            //    var g = t.visual.binding.text.id;
            //}
            e.Cancel = true;
            ChangeTile();
        }
        private void ChangeTile()
        {
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150ImageAndText01);

            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = "Hello World! My very own tile notification";

            XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/redWide.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "red graphic");

            XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text04);
            XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("text");
            squareTileTextAttributes[0].AppendChild(squareTileXml.CreateTextNode("notification recieved !!!"));
            IXmlNode node = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
            tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(tileXml);

            tileNotification.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(10);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            // _navigationService.NavigateTo("AddTeam");
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            //SecondaryTile tileData = new SecondaryTile()
            //{
            //    TileId = "MyTileID",
            //    DisplayName = "Dorch", 
            //    Arguments = "Some arguments"
            //};
            //tileData.VisualElements.ForegroundText = ForegroundText.Light;
            //tileData.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/Logo.scale-100.png");
            //tileData.VisualElements.ShowNameOnSquare150x150Logo = true;
            //await tileData.RequestCreateAsync();

            //InitNotificationsAsync();
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
}