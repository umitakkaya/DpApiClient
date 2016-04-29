using DpApiClient.REST.Client;
using DpApiClient.REST.DTO;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Core.Worker
{
    public class PullNotificationJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            DpApi _client = new DpApi(AppSettings.ClientId, AppSettings.ClientSecret, (Locale)AppSettings.Locale);
            NotificationHandler handler = new NotificationHandler(_client);

            while (true)
            {
                Notification notification = _client.GetNotification();

                if (notification != null && notification.Message == null)
                {
                    try
                    {   
                        handler.HandleNotification(notification);
                    }
                    catch (Exception ex)
                    {
                        var dpEx = new DpBookingException("Couldn't process the notifiation data", ex);
                        dpEx.Data.Add("notificationData", notification.Data);
                        throw dpEx;
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
