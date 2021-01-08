using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace hass_workstation_service.Domain.Notify
{
    public class Notifier
    {
        public void GenerateToast(NotificationModel notificationModel)
        {
            var template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);

            var textNodes = template.GetElementsByTagName("text");

            textNodes[0].AppendChild(template.CreateTextNode(notificationModel.Title));
            textNodes[2].AppendChild(template.CreateTextNode(notificationModel.Content));

            if (File.Exists(notificationModel.ImagePath))
            {
                XmlNodeList toastImageElements = template.GetElementsByTagName("image");
                ((XmlElement)toastImageElements[0]).SetAttribute("src", notificationModel.ImagePath);
            }
            IXmlNode toastNode = template.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "long");

            var notifier = ToastNotificationManager.CreateToastNotifier("Home Assistant");
            var notification = new ToastNotification(template);

            notifier.Show(notification);
        }

        public class NotificationModel
        {
            public string Title { get; private set; }
            public string Content { get; private set; }
            public string ImagePath { get; private set; }

            [JsonConstructor]
            public NotificationModel(string title, string content)
            {
                Title = title;
                Content = content;
                this.ImagePath = Path.Combine(Environment.CurrentDirectory, @"hass-workstation-logo.ico");
            }

            public NotificationModel(string title, string content, string imagePath)
            {
                Title = title;
                Content = content;
                ImagePath = imagePath;
            }
        }
    }
}
