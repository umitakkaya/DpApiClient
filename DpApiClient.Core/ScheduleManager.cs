using DpApiClient.Data;
using DpApiClient.Data.Repositories;
using DpApiClient.REST.Client;
using DpApiClient.REST.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpApiClient.REST.Extensions;

namespace DpApiClient.Core
{
    public class ScheduleManager
    {
        private HospitalContext _db;
        private DpApi _client;
        private VisitRepository _visitRepository;
        private ScheduleRepository _scheduleRepository;

        private string timeZone
        {
            get
            {
                return ((TimeZones)AppSettings.Locale).ToString();
            }
        }


        public ScheduleManager(HospitalContext db, DpApi client)
        {
            _db = db;
            _client = client;
            _visitRepository = new VisitRepository(_db);
            _scheduleRepository = new ScheduleRepository(_db);
        }

        public bool DeleteSlots(DoctorSchedule schedule, DoctorFacility doctorFacility)
        {
            if (doctorFacility.IsMapped)
            {
                var mapping = doctorFacility.DoctorMapping;
                var address = mapping.ForeignAddress;
                return _client.DeleteSlots(address.ForeignFacilityId, address.ForeignDoctorId, address.Id, schedule.Date);
            }
            else
            {
                return true;
            }
        }

        public bool PushSlots(DoctorFacility doctorFacility)
        {
            var mapping = doctorFacility.DoctorMapping;
            var defaultDoctorService = mapping.ForeignDoctorService;

            if (mapping == null)
            {
                return false;
            }

            var address = mapping.ForeignAddress;
            var schedules = _scheduleRepository.GetByDoctorFacility(doctorFacility).Where(s => s.IsFullfilled == false);

            if (schedules.Count() == 0)
            {
                return false;
            }

            
            var slotRangeList = schedules.Select(s => new SlotRange()
            {
                Start = s.Date.Add(s.Start).SetTimeZone(timeZone),
                End = s.Date.Add(s.End).SetTimeZone(timeZone),
                DoctorServices = ConvertToSlotDoctorServiceList(s.ForeignDoctorService ?? defaultDoctorService, s.Duration)
            }).ToList();

            var putSlotsRequest = new PutSlotsRequest()
            {
                Slots = slotRangeList
            };

            return _client.PutSlots(address.ForeignFacilityId, address.ForeignDoctorId, address.Id, putSlotsRequest);
        }


        public List<VisitSlot> GetSlots(DateTime date, DoctorFacility doctorFacility)
        {
            var doctorSchedules = _scheduleRepository.GetByDateAndDoctorFacility(date, doctorFacility.DoctorId, doctorFacility.FacilityId)
                .Where(ds => !ds.IsFullfilled);

            var slots = new List<VisitSlot>();

            foreach (var schedule in doctorSchedules)
            {
                TimeSpan start = schedule.Start;
                TimeSpan end = schedule.End;

                DateTime dtStart = schedule.Date.Add(start);
                DateTime dtEnd = dtStart;

                do
                {

                    dtEnd = dtStart.AddMinutes(schedule.Duration);

                    slots.Add(new VisitSlot()
                    {
                        Start = dtStart,
                        End = dtEnd,
                        DoctorSchedule = schedule,
                        DoctorFacility = schedule.DoctorFacility
                    });

                    dtStart = dtStart.AddMinutes(schedule.Duration);

                } while (dtEnd.AddMinutes(schedule.Duration).TimeOfDay <= end);
            }

            return slots;
        }

        public VisitSlot GetSlot(DateTime start, DateTime end, DoctorFacility doctorFacility)
        {
            if (start.Date != end.Date)
            {
                throw new ArgumentException("Start and end parameter should be at the same date");
            }

            var slots = GetSlots(start.Date, doctorFacility);

            return slots.SingleOrDefault(s => s.Start == start && s.End == end);
        }

        public bool IsSlotExist(DateTime start, DateTime end, DoctorFacility doctorFacility)
        {
            return GetSlot(start, end, doctorFacility) != null;
        }

        public DoctorSchedule FindDoctorSchedule(DoctorFacility doctorFacility, DateTime start, DateTime end, string foreignDoctorServiceId)
        {
            var doctorSchedules = _scheduleRepository.GetByDateAndDoctorFacility(start, doctorFacility.DoctorId, doctorFacility.FacilityId);

            DoctorSchedule schedule = doctorSchedules.FirstOrDefault(ds =>
                    ds.Start <= start.TimeOfDay && end.TimeOfDay <= ds.End
                    &&
                    ds.ForeignDoctorServiceId == foreignDoctorServiceId
                );

            if (schedule == null)
            {
                schedule = doctorSchedules.FirstOrDefault(ds => ds.Start <= start.TimeOfDay && end.TimeOfDay <= ds.End);
            }

            return schedule;
        }

        public DoctorSchedule RestoreSchedule(DoctorSchedule schedule)
        {
            schedule.IsFullfilled = false;
            _scheduleRepository.Update(schedule);
            _scheduleRepository.Save();

            return schedule;
        }

        public List<DoctorSchedule> ArrangeSchedule(Visit visit)
        {
            var doctorSchedule = visit.DoctorSchedule;
            TimeSpan slot = new TimeSpan(0, doctorSchedule.Duration, 0);
            TimeSpan visitStart = visit.StartAt.TimeOfDay;
            TimeSpan visitEnd = visit.EndAt.TimeOfDay;


            if (visitStart == doctorSchedule.Start && visitEnd == doctorSchedule.End)
            {
                doctorSchedule.IsFullfilled = true;
            }
            else
            {
                var visitSchedule = CreateVisitSchedule(doctorSchedule, visit, visitStart, visitEnd);
                _scheduleRepository.Insert(visitSchedule);

                if (visitStart == doctorSchedule.Start)
                {
                    doctorSchedule.Start = doctorSchedule.Start.Add(slot);
                }
                else if (visitEnd == doctorSchedule.End)
                {
                    doctorSchedule.End = doctorSchedule.End.Subtract(slot);
                }
                else if (visitStart > doctorSchedule.Start && visitEnd < doctorSchedule.End)
                {
                    var remainingSchedule = DivideSchedule(doctorSchedule, visitStart, visitEnd);

                    if (remainingSchedule != null)
                    {
                        _scheduleRepository.Insert(remainingSchedule);
                    }

                    _scheduleRepository.Update(doctorSchedule);
                }
                else
                {
                    throw new ArgumentException("Visit doesn't belong to this schedule");
                }
            }

            _scheduleRepository.Update(doctorSchedule);
            _scheduleRepository.Save();

            return new List<DoctorSchedule>();
        }

        private DoctorSchedule CreateVisitSchedule(DoctorSchedule schedule, Visit visit, TimeSpan visitStart, TimeSpan visitEnd)
        {
            var visitSchedule = _scheduleRepository.Clone(schedule);

            visitSchedule.Start = visitStart;
            visitSchedule.End = visitEnd;
            visitSchedule.IsFullfilled = true;
            visit.DoctorSchedule = visitSchedule;

            return visitSchedule;
        }

        private DoctorSchedule DivideSchedule(DoctorSchedule schedule, TimeSpan visitStart, TimeSpan visitEnd)
        {
            var remainingSchedule = _scheduleRepository.Clone(schedule);

            schedule.End = visitStart;
            remainingSchedule.Start = visitEnd;

            TimeSpan leftover = remainingSchedule.End - remainingSchedule.Start;

            if (leftover.TotalMinutes >= remainingSchedule.Duration)
            {
                return remainingSchedule;
            }

            return null;
        }

        private List<SlotDoctorService> ConvertToSlotDoctorServiceList(ForeignDoctorService doctorService, int duration)
        {
            return new List<SlotDoctorService>()
            {
                new SlotDoctorService()
                {
                    DoctorServiceId = doctorService.Id,
                    Duration = duration
                }
            };
        }

    }
}
