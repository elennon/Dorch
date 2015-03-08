using Dorch.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Notifications;

namespace BackgroundUpdateTile
{
    public sealed class UpdateTile : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {            
            RawNotification notification = (RawNotification)taskInstance.TriggerDetails;
            string content = notification.Content;

            XmlSerializer serializer = new XmlSerializer(typeof(toast));
            using (TextReader reader = new StringReader(content))
            {
                var t = (toast)serializer.Deserialize(reader);
                var requestType = t.visual.binding.text.id;
                var messageContent = t.visual.binding.text.Value;
                if (requestType == 1)
                {
                    messageContent = requestType + "," + messageContent;
                    ApplicationSettingsHelper.SaveSettingsValue(Constants.JoinTeamRequest, true);
                    ApplicationSettingsHelper.SaveSettingsValue(Constants.JoinTeamRequestContent, messageContent);
                }
                else if (requestType == 2)
                {
                    messageContent = requestType + "," + messageContent;
                    ApplicationSettingsHelper.SaveSettingsValue(Constants.PlayRequest, true);
                    ApplicationSettingsHelper.SaveSettingsValue(Constants.PlayRequestContent, messageContent);
                }
            }                      
            ChangeTile();
        }

        private void ChangeTile()
        {
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText01);

            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = "otification";

            XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/messa150.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "message recieved");

            //XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text04);
            //XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("text");
            //squareTileTextAttributes[0].AppendChild(squareTileXml.CreateTextNode("Hello World! My very own tile notification"));
            //IXmlNode node = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
            //tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(tileXml);

          //  tileNotification.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(10);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);

        }
    }
}


//ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
//            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
//            XmlNodeList textElements = toastXml.GetElementsByTagName("text");
//            textElements[0].AppendChild(toastXml.CreateTextNode("My first Task - Yeah"));
//            textElements[1].AppendChild(toastXml.CreateTextNode("I'm a message from your background task!"));
//            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toastXml));