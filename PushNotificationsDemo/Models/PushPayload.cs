﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PushNotificationsDemo.Models
{
    public class PushPayload
    {
        public PushPayload() { }

        #region Propiedades
        public string alert { get; set; }

        public string badge { get; set; }

        public string icon { get; set; }

        public string sound { get; set; }

        public string title { get; set; }

        public bool vibrate { get; set; }
        #endregion

        #region Métodos
        public bool isNotValidInstance()
        {
            return string.IsNullOrEmpty(alert) && string.IsNullOrEmpty(title);
        }
        #endregion
    }   
}