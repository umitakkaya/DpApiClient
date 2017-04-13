using DpApiClient.REST.Client;
using DpApiClient.REST.DTO;
using DpApiClient.REST.Utilities;
using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpApiClient.Data;

namespace DpApiClient.Core.Worker
{
    public class PullNotificationJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PullNotificationJob));

        public void Execute(IJobExecutionContext context)
        {
            DpApi _client = new DpApi(AppSettings.ClientId, AppSettings.ClientSecret, (Locale)AppSettings.Locale);
            var db = new HospitalContext();
            NotificationHandler handler = new NotificationHandler(_client, db);

            while (true)
            {
                Notification notification = _client.GetNotification();
                string json = JsonConvert.SerializeObject(notification, new JsonSerializerSettings
                {
                    ContractResolver = new SnakeCaseContractResolver(),
                    Formatting = Formatting.Indented
                });

                if (notification != null && notification.Message == null)
                {
                    try
                    {
                        log.Warn("Parsing notification:");
                        log.Warn(json);
                        handler.HandleNotification(notification);
                    }
                    catch (Exception ex)
                    {
                        var dpEx = new DPNotificationException("Couldn't process the notifiation data", ex);
                        dpEx.Data.Add("notificationData", json);
                        log.Error("Couldn't process the notifiation data", dpEx);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
