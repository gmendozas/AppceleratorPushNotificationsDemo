using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PushNotificationsDemo.Models
{
    public class User
    {
        public string id { get; set; }
        public string sessionId { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }
}