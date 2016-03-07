using DpApiClient.Data;
using System;

namespace DpApiClient.Core
{
    public class VisitSlot
    {
        public DateTime Start { get; set; }
        public TimeSpan StartTime
        {
            get
            {
                return Start.TimeOfDay;
            }
        }

        public DateTime End { get; set; }
        public TimeSpan EndTime
        {
            get
            {
                return End.TimeOfDay;
            }
        }

        public int Duration
        {
            get
            {
                return (int)(EndTime - StartTime).TotalMinutes;
            }
        }

        public ForeignDoctorService DoctorService
        {
            get
            {
                return DoctorSchedule.ForeignDoctorService;
            }
        }
        public DoctorSchedule DoctorSchedule { get; set; }
        public DoctorFacility DoctorFacility { get; set; }
    }
}