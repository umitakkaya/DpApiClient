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
using System.Data.Entity;
using System.Globalization;

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

        private IEnumerable<DoctorSchedule> GetSchedules(DoctorFacility doctorFacility, DateTime maxScheduleDate)
        {
            var schedules = _scheduleRepository.GetByDoctorFacility(doctorFacility)
                .Where(s =>
                    s.IsFullfilled == false &&
                    s.Start < s.End &&
                    s.Date.Add(s.Start) > DateTime.Now &&
                    s.Date.Add(s.End) < maxScheduleDate
                );

            return schedules;
        }

        public bool PushSlots(DoctorFacility doctorFacility)
        {
            var mapping = doctorFacility.DoctorMapping;

            if (mapping == null)
            {
                return false;
            }

            var maxSlotDate = CultureInfo.InvariantCulture.Calendar.AddWeeks(DateTime.Now, 12);
            var schedules = GetSchedules(doctorFacility, maxSlotDate);

            if (schedules.Count() == 0)
            {
                return false;
            }

            var calendarBlocks = CalendarGenerator.MergeSchedules(schedules);

            var address = mapping.ForeignAddress;
            var groupedSchedules = schedules.GroupBy(s => s.Date.Date);
            var defaultDoctorService = mapping.ForeignDoctorService;

            var slots = calendarBlocks.Select(b => new SlotRange()
            {
                Start = b.Start,
                End = b.End,
                DoctorServices = ConvertToSlotDoctorServiceList(b.DoctorService ?? defaultDoctorService, b.Duration)
            });

            var putSlotsRequest = new PutSlotsRequest() { Slots = slots.ToList() };

            return _client.PutSlots(address.ForeignFacilityId, address.ForeignDoctorId, address.Id, putSlotsRequest);
        }

        /// <summary>
        /// Delete all slots on Docplanner side
        /// </summary>
        /// <param name="doctorFacility"></param>
        /// <returns></returns>
        public bool ClearDPCalendar(DoctorFacility doctorFacility)
        {
            var mapping = doctorFacility.DoctorMapping;

            if (mapping == null)
            {
                return false;
            }

            var address = mapping.ForeignAddress;

            var startDate = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now);
            var maxSlotDate = CultureInfo.InvariantCulture.Calendar.AddWeeks(DateTime.Now, 12);
            var schedules = GetSchedules(doctorFacility, maxSlotDate)
                .GroupBy(s => s.Date.Date);

            var slots = _client.GetSlots(address.ForeignFacilityId, address.ForeignDoctorId, address.Id, startDate, maxSlotDate);

            //If there aren't any slots there is nothing to do.
            if (slots == null || slots.Any() == false)
            {
                return true;
            }

            //Clear slots for the dates which has slots.
            //No need to send request for the days which don't have any slots.
            foreach (var item in slots.GroupBy(s => s.Start.Date))
            {
                //If we have valid schedules for a day, do not clear it, it will be replaced anyway. 
                if (schedules.Any(s => s.Key == item.Key))
                {
                    continue;
                }

                _client.DeleteSlots(address.ForeignFacilityId, address.ForeignDoctorId, address.Id, item.Key);
            }

            return true;
        }


        public List<VisitSlot> GetSlots(DateTime date, DoctorFacility doctorFacility)
        {
            var doctorSchedules = _scheduleRepository.GetByDateAndDoctorFacilityAsNoTracking(date, doctorFacility.DoctorId, doctorFacility.FacilityId)
                .Where(ds => !ds.IsFullfilled);

            var slots = new List<VisitSlot>();

            foreach (var schedule in doctorSchedules)
            {
                TimeSpan start = schedule.Start;
                TimeSpan end = schedule.End;

                DateTime dtStart = schedule.Date.Add(start);
                DateTime dtEnd = dtStart;

                int duration = RoundToNearestFive(schedule.Duration);

                do
                {
                    dtEnd = dtStart.AddMinutes(duration);

                    slots.Add(new VisitSlot()
                    {
                        Start = dtStart,
                        End = dtEnd,
                        DoctorSchedule = schedule,
                        DoctorFacility = schedule.DoctorFacility
                    });

                    dtStart = dtStart.AddMinutes(duration);

                } while (dtEnd.AddMinutes(duration).TimeOfDay <= end);
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

        public void ArrangeSchedule(Visit visit)
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
            duration = RoundToNearestFive(duration);

            return new List<SlotDoctorService>()
            {
                new SlotDoctorService()
                {
                    DoctorServiceId = doctorService.Id,
                    Duration = duration
                }
            };
        }

        private int RoundToNearestFive(int number)
        {
            return 5 * (int)Math.Round(number / 5.0);
        }
    }
}
