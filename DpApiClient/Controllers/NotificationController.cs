using DpApiClient.Core;
using DpApiClient.REST.Client;
using DpApiClient.REST.DTO;
using DpApiClient.REST.Utilities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace DpApiClient.Controllers
{
    public class NotificationController : Controller
    {
        private DpApi _client;
        public NotificationController()
        {
            _client = new DpApi(AppSettings.ClientId, AppSettings.ClientSecret, (Locale)AppSettings.Locale);
        }

        // GET: Notification/Pull
        public ActionResult Pull()
        {
            var notification = _client.GetNotification();

            HandleNotification(notification);

            return View(notification);
        }

        // Post: Notification/Handle
        [HttpPost]
        public ActionResult Handle()
        {
            var req = Request.InputStream;
            var json = new StreamReader(req).ReadToEnd();

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SnakeCaseContractResolver()
            };

            Notification notification = JsonConvert.DeserializeObject<Notification>(json, settings);

            bool result = HandleNotification(notification);

            if(result)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }

            if(notification.Name == "slot-booking")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict); 
            }

            return Json(new { status = result, echo = json, message = "Notification was either already processed or not necessarily needed" });
        }

        private bool HandleNotification(Notification notification)
        {
            if (notification != null && notification.Message == null)
            {
                NotificationHandler handler = new NotificationHandler(_client);
                return handler.HandleNotification(notification);
            }

            return false;
        }
    }
}