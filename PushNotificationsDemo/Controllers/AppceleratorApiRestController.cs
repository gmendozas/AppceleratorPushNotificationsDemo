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

        //[Route("api/AppeceleratorApiRest/{payload}/{channel}")]
        public void Post([FromBody]PushPayload payload)
        {
            if (Object.Equals(null, payload))
                return;
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
            SendPush(payload, "news_alerts");
        }

        private void SendPush(PushPayload payload, string channel)
        {
            try
            {
                string payloadStr = JsonConvert.SerializeObject(payload);
                client.BaseUrl = new Uri("https://api.cloud.appcelerator.com");
                request.Resource = "/v1/push_notification/notify.json?key={appkey}";
                request.Method = Method.POST;
                request.AddUrlSegment("appkey", APP_KEY);

                request.AddParameter("channel", channel);
                request.AddParameter("payload", payloadStr);
                var response = client.Execute(request);
            }
            catch (Exception ex)
            {
                string errorMessage = "Message " + ex.Message + " \n Inner Exception " + ex.InnerException + " \n Stack Trace" + ex.StackTrace;
            }
        }
    }
}
