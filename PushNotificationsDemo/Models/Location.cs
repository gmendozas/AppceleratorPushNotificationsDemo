using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PushNotificationsDemo.Models
{
    public class Location
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string MaxDistance { get; set; }
    }
}