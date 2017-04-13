using System;
using System.ComponentModel.DataAnnotations;

namespace DpApiClient.Data
{
    public class Visit
    {
        public int Id { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public VisitStatus VisitStatus { get; set; }

        public int DoctorId { get; set; }
        public int FacilityId { get; set; }
        public int DoctorScheduleId { get; set; }

        public VisitPatient VisitPatient { get; set; }
        public virtual DoctorFacility DoctorFacility { get; set; }
        public virtual DoctorSchedule DoctorSchedule { get; set; }


        public string ForeignVisitId { get; set; }
    }
}
