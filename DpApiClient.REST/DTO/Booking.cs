using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    public class Booking : DPResponse
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public int Duration { get; set; }
        public string BookedBy { get; set; }
        public string CanceledBy { get; set; }
        public string BookedAt { get; set; }
        public string CanceledAt { get; set; }
        public string Comment { get; set; }
        public Patient Patient { get; set; }
        public DoctorService Service { get; set; }
    }
}
