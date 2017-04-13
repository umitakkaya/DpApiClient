using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    public class NotificationData: DPResponse
    {
        public DPFacility Facility { get; set; }
        public Address Address { get; set; }
        public DPDoctor Doctor { get; set; }
        public Booking VisitBooking { get; set; }
        public RealtimeBooking VisitBookingRequest { get; set; }
        public Booking OldVisitBooking { get; set; }
        public Booking NewVisitBooking { get; set; }
    }
}