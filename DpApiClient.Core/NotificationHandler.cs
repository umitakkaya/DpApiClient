using DpApiClient.Data;
using DpApiClient.Data.Repositories;
using DpApiClient.REST.Client;
using DpApiClient.REST.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Core
{
    public class NotificationHandler
    {
        private HospitalContext _db;
        private VisitManager _visitManager;
        private ScheduleManager _scheduleManager;
        private VisitRepository _visitRepo;
        private ScheduleRepository _scheduleRepo;
        private DoctorMappingRepository _mappingRepo;
        private DpApi _client;

        public NotificationHandler(DpApi client, HospitalContext db = null)
        {
            _db = db ?? new HospitalContext();

            _visitRepo = new VisitRepository(_db);
            _mappingRepo = new DoctorMappingRepository(_db);
            _scheduleRepo = new ScheduleRepository(_db);

            _client = client;

            _scheduleManager = new ScheduleManager(_db, _client);
            _visitManager = new VisitManager(_db, _client, _scheduleManager);

        }

        public bool HandleNotification(Notification notification)
        {
            switch (notification.Name)
            {
                case "slot-booked":
                    return SlotBookedEvent(notification.Data.Facility, notification.Data.Doctor, notification.Data.Address, notification.Data.VisitBooking);
                case "slot-booking":
                    return SlotBookingEvent(notification.Data.Facility, notification.Data.Doctor, notification.Data.Address, notification.Data.VisitBookingRequest);
                case "booking-canceled":
                    return BookingCanceled(notification.Data.Facility, notification.Data.Doctor, notification.Data.Address, notification.Data.VisitBooking);
                case "booking-moved":
                    return BookingMoved(notification.Data.Facility, notification.Data.Doctor, notification.Data.Address, notification.Data.OldVisitBooking, notification.Data.NewVisitBooking);

                case "slot-overbooked":
                    return SlotOverbooked(notification.Data.Facility, notification.Data.Doctor, notification.Data.Address, notification.Data.VisitBooking);
                case "slot-changed":
                default:
                    DefaultEventHandler(notification.Data.Facility, notification.Data.Doctor, notification.Data.Address);
                    return false;
            }
        }

        /// <summary>
        /// Given the case when overbooking a slot is not permitted.
        /// </summary>
        /// <param name="facility"></param>
        /// <param name="doctor"></param>
        /// <param name="address"></param>
        /// <param name="visitBooking"></param>
        private bool SlotOverbooked(DPFacility facility, DPDoctor doctor, Address address, Booking visitBooking)
        {
            return _visitManager.CancelVisitDP(facility.Id, doctor.Id, address.Id, visitBooking.Id);
        }

        private bool BookingMoved(DPFacility facility, DPDoctor doctor, Address address, Booking oldVisitBooking, Booking newVisitBooking)
        {
            return _visitManager.MoveVisit(oldVisitBooking, newVisitBooking, facility, doctor, address);
        }

        private bool BookingCanceled(DPFacility facility, DPDoctor doctor, Address address, Booking visitBooking)
        {
            return _visitManager.CancelVisit(visitBooking.Id, false);
        }

        private bool SlotBookedEvent(DPFacility facility, DPDoctor doctor, Address address, Booking visitBooking)
        {
            bool result = _visitManager.RegisterDpVisit(facility, doctor, address, visitBooking);

            if (result == false)
            {
                _visitManager.CancelVisitDP(facility.Id, doctor.Id, address.Id, visitBooking.Id);
            }

            return result;
        }

        private bool SlotBookingEvent(DPFacility facility, DPDoctor doctor, Address address, RealtimeBooking visitBooking)
        {
            return _visitManager.CanSlotBeBooked(facility, doctor, address, visitBooking);
        }

        private bool DefaultEventHandler(DPFacility facility, DPDoctor doctor, Address address)
        {
            return true;
        }

    }
}
