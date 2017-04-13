using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    public class Notification : DPResponse
    {
        public string Name { get; set; }
        public NotificationData Data { get; set; }
        public string CreatedAt { get; set; }
    }
}
