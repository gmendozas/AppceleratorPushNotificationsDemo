using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PushNotificationsDemo.Models
{
    public class Subscription
    {
        public string type { get; set; }
        public int badge_number { get; set; }
        public string app_id { get; set; }
        public string device_token { get; set; }
        public string user_id { get; set; }
        public List<string> channel { get; set; }
        public string android_type { get; set; }
    }
}