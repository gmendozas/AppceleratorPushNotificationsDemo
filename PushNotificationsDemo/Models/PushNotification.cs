using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PushNotificationsDemo.Models
{
    public class PushNotification
    {
        public PushPayload Payload { get; set; }
        public Location Location { get; set; }
        public List<string> Devices { get; set; }
    }
}