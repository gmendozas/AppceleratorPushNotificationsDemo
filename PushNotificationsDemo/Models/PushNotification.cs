using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PushNotificationsDemo.Models
{
    public class PushNotification
    {
        public PushPayload payload { get; set; }
        public Location location { get; set; }
    }
}