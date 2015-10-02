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
        public NotificationStatus Post(string channel, PushPayload payload)
        {
            if (Object.Equals(null, payload))
                return new NotificationStatus { StatusCode = "0", StatusDescription = "Los parámetros son incorrectos" };
            else if(payload.isNotValidInstance())
            {
                payload.alert = "Alerta desde el servicio";
                payload.badge = "1";
                payload.icon = "appicon";
                payload.vibrate = true;
                //return new NotificationStatus { StatusCode = "0", StatusDescription = "Los parámetros del mensaje son incorrectos, 'alert' y 'title' son campos obligatorios" };
            }
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
                        subscriptions = new List<Subscription>();
                        GetSubscriptions(loginResponse.response.users[0].id.ToString());
                        SendPush(payload, channel);
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
                request.AddParameter("user_id", idUsuario);
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

        private void SendPush(PushPayload payload, string channel)
        {
            if (subscriptions.Count > 0)
            {
                try
                {
                    string payloadStr = JsonConvert.SerializeObject(payload);
                    string delimiter = ",";
                    var validSubscriptions = from s in subscriptions
                                             where s.channel.Contains(channel)
                                             select s.device_token;
                    string tokens = String.Join(delimiter, validSubscriptions);

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
    }
}
