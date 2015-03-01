using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;

namespace TeamManagerService.Utilities
{
    public class MyNotificationHandler : INotificationHandler
    {
        public Task Register(ApiServices services, HttpRequestContext context, NotificationRegistration registration)
        {
            registration.Tags.Add("Henrik");
            return Task.FromResult(true);
        }

        public Task Unregister(ApiServices services, HttpRequestContext context, string deviceId)
        {
            return Task.FromResult(true);
        }
    }
}

