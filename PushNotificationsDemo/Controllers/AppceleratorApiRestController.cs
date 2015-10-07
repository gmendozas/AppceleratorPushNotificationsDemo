using Newtonsoft.Json;
using PushNotificationsDemo.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PushNotificationsDemo.Controllers
{
    public class AppceleratorApiRestController : ApiController
    {
        private const string APP_KEY = "H2tnZbCkaf8YMJcyN0ICKuVLKhgOFzDz";
        private RestClient client;
        private RestRequest request;
        private List<Subscription> subscriptions;
        private NotificationStatus status;

        [Route("api/AppceleratorApiRest/{channel}")]
        public NotificationStatus Post(string channel, PushNotification notification)
        {
            if (Object.Equals(null, notification.Payload))
                return new NotificationStatus { StatusCode = "0", StatusDescription = "Los parámetros son incorrectos" };
            
            try
            {
                client = new RestClient("https://api.cloud.appcelerator.com");
                client.CookieContainer = new System.Net.CookieContainer();
                request = new RestRequest("/v1/users/login.json?key={appkey}", Method.POST)
                {
                    RequestFormat = DataFormat.Json,
                };
                request.AddUrlSegment("appkey", APP_KEY);
                request.AddBody(new
                {
                    login = "developer",
                    password = "developer"
                });
                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    dynamic loginResponse = JsonConvert.DeserializeObject(response.Content);
                    if (loginResponse.response.users.Count > 0)
                    {
                        if (notification.Devices.Count == 0)
                        {
                            subscriptions = new List<Subscription>();
                            GetSubscriptions(loginResponse.response.users[0].id.ToString());
                        }                        
                        SendPush(notification, channel);
                    }
                }
                else
                    status = new NotificationStatus { StatusCode = response.StatusCode.ToString(), StatusDescription = response.StatusDescription };
                return status;
            }
            catch (Exception ex)
            {
                string errorMessage = "Message " + ex.Message + " \n Inner Exception " + ex.InnerException + " \n Stack Trace" + ex.StackTrace;
                status = new NotificationStatus { StatusCode = "-1", StatusDescription = errorMessage };
            }
            return status;
        }

        private void GetSubscriptions(string idUsuario)
        {
            try
            {
                client.BaseUrl = new Uri("https://api.cloud.appcelerator.com");
                request.Resource = "/v1/push_notification/query.json?key={appkey}";
                request.Method = Method.GET;
                request.AddUrlSegment("appkey", APP_KEY);

                request.AddParameter("pretty_json", "true");
                request.AddParameter("type", "android");
                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    dynamic responseSubscriptions = JsonConvert.DeserializeObject(response.Content);
                    if (responseSubscriptions.response.subscriptions.Count > 0)
                        subscriptions = responseSubscriptions.response.subscriptions.ToObject<List<Subscription>>();
                }
                status = new NotificationStatus { StatusCode = response.StatusCode.ToString(), StatusDescription = response.StatusDescription };
            }
            catch (Exception ex)
            {
                string errorMessage = "Message " + ex.Message + " \n Inner Exception " + ex.InnerException + " \n Stack Trace" + ex.StackTrace;
                status = new NotificationStatus { StatusCode = "-1", StatusDescription = errorMessage };
            }
        }

        private void SendPush(PushNotification notification, string channel)
        {
            if (notification.Devices.Count > 0 || IsSubscriptionsValid())
            {
                try
                {
                    if (string.IsNullOrEmpty(notification.Payload.sound))
                        notification.Payload.sound = "default";
                    string payloadStr = JsonConvert.SerializeObject(notification.Payload);
                    string delimiter = ",";
                    string tokens = string.Empty;
                    if(notification.Devices.Count > 0)
                    {
                        tokens = String.Join(delimiter, notification.Devices);
                    } else {
                        var validSubscriptions = from s in subscriptions
                                                 where s.channel.Contains(channel)
                                                 select s.device_token;
                        tokens = String.Join(delimiter, validSubscriptions);
                    }
                    

                    client.BaseUrl = new Uri("https://api.cloud.appcelerator.com");
                    request.Resource = "/v1/push_notification/notify_tokens.json?key={appkey}";
                    // where={"loc": { "$nearSphere" : { "$geometry" : { "type" : "Point" , "coordinates" : [-122.2708,37.8044] } , "$maxDistance" : 2000 }}}
                    request.Method = Method.POST;
                    request.AddUrlSegment("appkey", APP_KEY);

                    request.AddParameter("channel", channel);
                    request.AddParameter("to_tokens", tokens);
                    request.AddParameter("payload", payloadStr);
                    var response = client.Execute(request);
                    status = new NotificationStatus { StatusCode = response.StatusCode.ToString(), StatusDescription = response.StatusDescription };
                }
                catch (Exception ex)
                {
                    string errorMessage = "Message " + ex.Message + " \n Inner Exception " + ex.InnerException + " \n Stack Trace" + ex.StackTrace;
                    status = new NotificationStatus { StatusCode = "-1", StatusDescription = errorMessage };
                }
                finally
                {
                    Logout();
                }
            }
        }

        private void Logout()
        {
            try
            {
                client.BaseUrl = new Uri("https://api.cloud.appcelerator.com");
                request.Resource = "/v1/users/logout.json?key={appkey}";
                request.Method = Method.GET;
                request.AddUrlSegment("appkey", APP_KEY);

                request.AddParameter("pretty_json", "true");
                var response = client.Execute(request);
                status = new NotificationStatus { StatusCode = response.StatusCode.ToString(), StatusDescription = response.StatusDescription };
            }
            catch (Exception ex)
            {
                string errorMessage = "Message " + ex.Message + " \n Inner Exception " + ex.InnerException + " \n Stack Trace" + ex.StackTrace;
                status = new NotificationStatus { StatusCode = "-1", StatusDescription = errorMessage };
            }
        }

        private bool IsSubscriptionsValid()
        {
            if (Object.Equals(null, subscriptions))
                return false;
            else if (subscriptions.Count == 0)
                return false;
            return true;
        }
    }
}
