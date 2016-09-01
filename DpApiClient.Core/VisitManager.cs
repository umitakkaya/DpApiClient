using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DpApiClient.Data;
using DpApiClient.REST.Client;
using DpApiClient.Data.Repositories;
using DpApiClient.REST.DTO;
using DpApiClient.REST.Extensions;

namespace DpApiClient.Core
{
    public class VisitManager
    {
        private HospitalContext _db;
        private DpApi _client;
        private VisitRepository _visitRepository;
        private ScheduleRepository _scheduleRepo;
        private DoctorMappingRepository _mappingRepo;
        private ScheduleManager _scheduleManager;

        private string timeZone
        {
            get
            {
                return ((TimeZones)AppSettings.Locale).ToString();
            }
        }


        public VisitManager(HospitalContext db, DpApi client, ScheduleManager scheduleManager)
        {
            _db = db;
            _client = client;

            _mappingRepo = new DoctorMappingRepository(_db);
            _scheduleRepo = new ScheduleRepository(_db);
            _visitRepository = new VisitRepository(_db);

            _scheduleManager = new ScheduleManager(_db, _client);
        }

        /// <summary>
        /// Handles the visit-moved event from Docplanner
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public bool MoveVisit(Booking oldVisit, Booking newVisit, DPFacility facility, DPDoctor doctor, Address address)
        {

            var visit = _visitRepository.GetByForeignId(oldVisit.Id);
            var newStart = newVisit.StartAt.LocalDateTime.ChangeTimeZone(timeZone);
            var newEnd = newVisit.EndAt.LocalDateTime.ChangeTimeZone(timeZone);

            if (_scheduleManager.IsSlotExist(newStart, newEnd, visit.DoctorFacility))
            {
                visit.ForeignVisitId = newVisit.Id;
                visit.StartAt = newStart;
                visit.EndAt = newEnd;

                var oldSchedule = visit.DoctorSchedule;
                var newSchedule = _scheduleManager.FindDoctorSchedule(visit.DoctorFacility, visit.StartAt, visit.EndAt, newVisit.Service.Id);

                visit.DoctorSchedule = newSchedule;
                visit.DoctorScheduleId = newSchedule.Id;

                _scheduleManager.RestoreSchedule(oldSchedule);
                _scheduleManager.ArrangeSchedule(visit);

                return true;
            }
            else
            {
                CancelVisit(visit, false);
                CancelVisitDP(visit);
                CancelVisitDP(facility.Id, doctor.Id, address.Id, newVisit.Id);

                return false;
            }
        }

        public bool CancelVisit(Visit visit, bool notifyDp = true)
        {
            if(visit.StartAt <= DateTime.Now)
            {
                throw new ArgumentException("You can't cancel a visit which is in the past");
            }

            visit.VisitStatus = VisitStatus.Cancelled;
            _visitRepository.Save();

            var mapping = visit.DoctorFacility.DoctorMapping;

            _scheduleManager.RestoreSchedule(visit.DoctorSchedule);

            if (notifyDp && visit.ForeignVisitId != null)
            {
                CancelVisitDP(visit);

            }

            return true;
        }

        public bool CancelVisit(string foreignVisitId, bool notifyDp = true)
        {
            var visit = _visitRepository.GetByForeignId(foreignVisitId);

            if (visit == null)
            {
                return false;
            }

            return CancelVisit(visit, notifyDp);
        }

        public bool CancelVisitDP(Visit visit)
        {
            var mapping = visit.DoctorFacility.DoctorMapping;
            var address = mapping.ForeignAddress;

            return CancelVisitDP(address.ForeignFacilityId, address.ForeignDoctorId, address.Id, visit.ForeignVisitId);
        }

        public bool CancelVisitDP(string facilityId, string doctorId, string addressId, string foreignVisitId)
        {
            bool result = _client.CancelBooking(facilityId, doctorId, addressId, foreignVisitId);

            if (result == false)
            {
                throw new DPCancelException("Visit is not cancelled");
            }

            return true;
        }

        public void BookVisit(Visit visit, bool isDpVisit = false)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var mapping = visit.DoctorFacility.DoctorMapping;

                    if (visit.StartAt <= DateTime.Now)
                    {
                        throw new ArgumentException("You can't book a visit which is in the past");
                    }


                    _visitRepository.Insert(visit);
                    _visitRepository.Save();

                    _scheduleManager.ArrangeSchedule(visit);


                    if (mapping != null && isDpVisit == false)
                    {
                        var booking = BookVisitDP(visit, mapping);

                        visit.ForeignVisitId = booking.Id;
                        _visitRepository.Save();
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public Booking BookVisitDP(Visit visit, DoctorMapping mapping)
        {
            var foreignAddress = mapping.ForeignAddress;
            var defaultDoctorService = mapping.ForeignDoctorService;
            var visitDoctorService = visit.DoctorSchedule.ForeignDoctorService;
            var patient = visit.VisitPatient;

            var bookRequest = new BookSlotRequest()
            {
                DoctorServiceId = (visit.DoctorSchedule.ForeignDoctorService ?? defaultDoctorService).Id,
                IsReturning = false,
                Patient = new Patient()
                {
                    Name = patient.Name,
                    Surname = patient.Surname,
                    BirthDate = patient.Birthdate,
                    Email = patient.Email,
                    Gender = patient.StrGender,
                    Nin = patient.NIN,
                    Phone = patient.Phone
                }
            };

            var booking = _client.BookSlot(foreignAddress.ForeignFacilityId, foreignAddress.ForeignDoctorId, foreignAddress.Id, visit.StartAt, bookRequest);

            if (booking.Message == null)
            {
                return booking;
            }
            else
            {
                throw new DpBookingException(booking.Message);
            }
        }

        public bool CanSlotBeBooked(DPFacility facility, DPDoctor doctor, Address address, RealtimeBooking visitBooking)
        {
            var doctorMapping = _mappingRepo.GetByForeignAddress(address.Id);
            var startAt = visitBooking.StartAt.LocalDateTime.ChangeTimeZone(timeZone);
            var endAt = visitBooking.EndAt.LocalDateTime.ChangeTimeZone(timeZone);

            if (doctorMapping == null)
            {
                return false;
            }

            return _scheduleManager.IsSlotExist(startAt, endAt, doctorMapping.DoctorFacility);
        }

        public bool RegisterDpVisit(DPFacility facility, DPDoctor doctor, Address address, Booking visitBooking)
        {
            var doctorMapping = _mappingRepo.GetByForeignAddress(address.Id);
            var existingVisit = _visitRepository.GetByForeignId(visitBooking.Id);
            var startAt = visitBooking.StartAt.LocalDateTime.ChangeTimeZone(timeZone);
            var endAt = visitBooking.EndAt.LocalDateTime.ChangeTimeZone(timeZone);

            if (existingVisit != null)
            {
                return true;
            }
            else if (false == _scheduleManager.IsSlotExist(startAt, endAt, doctorMapping.DoctorFacility))
            {
                return false;
            }
            else if (doctorMapping == null)
            {
                return false;
            }

            var patient = visitBooking.Patient;
            var visit = new Visit()
            {
                DoctorFacility = doctorMapping.DoctorFacility,
                DoctorId = doctorMapping.DoctorId,
                FacilityId = doctorMapping.FacilityId,
                StartAt = visitBooking.StartAt.LocalDateTime.ChangeTimeZone(timeZone),
                EndAt = visitBooking.EndAt.LocalDateTime.ChangeTimeZone(timeZone),
                ForeignVisitId = visitBooking.Id,
                VisitStatus = VisitStatus.Booked,
                VisitPatient = new VisitPatient()
                {

                    Name = patient.Name,
                    Surname = patient.Surname,
                    Phone = patient.Phone,
                    Email = patient.Email,
                    Gender = patient.Gender == null ? Gender.NotSpecified : (patient.Gender == "m" ? Gender.Male : Gender.Female),
                    NIN = patient.Nin,
                    Birthdate = patient.BirthDate
                }
            };


            var schedule = _scheduleManager.FindDoctorSchedule(doctorMapping.DoctorFacility, visit.StartAt, visit.EndAt, visitBooking.Service.Id);
            if (schedule == null || schedule.IsFullfilled)
            {
                return false;
            }
            else
            {
                visit.DoctorSchedule = schedule;
                visit.DoctorScheduleId = schedule.Id;

                BookVisit(visit, true);
                return true;
            }
        }
    }
}
