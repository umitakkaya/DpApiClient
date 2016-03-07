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

            HandleNotification(notification);
            return Json(new { status = true, echo = json });
        }

        private void HandleNotification(Notification notification)
        {
            if (notification != null && notification.Message == null)
            {
                NotificationHandler handler = new NotificationHandler(_client);
                handler.HandleNotification(notification);
            }
        }
    }
}