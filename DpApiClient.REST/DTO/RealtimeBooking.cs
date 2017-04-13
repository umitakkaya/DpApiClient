using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    public class RealtimeBooking : DPResponse
    {
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public int Duration { get; set; }
        public string BookingBy { get; set; }
        public string BookingAt { get; set; }
        public string Comment { get; set; }
        public Patient Patient { get; set; }
        public DoctorService Service { get; set; }
    }
}
