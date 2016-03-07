using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DpApiClient.Data
{
    public class DoctorSchedule : IValidatableObject
    {
        public int Id { get; set; }
        public int Duration { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public bool IsFullfilled { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public int DoctorId { get; set; }
        public int FacilityId { get; set; }
        public string ForeignDoctorServiceId { get; set; }


        public DoctorFacility DoctorFacility { get; set; }
        public ForeignDoctorService ForeignDoctorService { get; set; }

        private List<Visit> _visits;
        public virtual List<Visit> Visits
        {
            get { return _visits ?? (_visits = new List<Visit>()); }
            set { _visits = value; }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Duration < 10)
            {
                yield return new ValidationResult("Duration should be at least 10", new[] { "Duration" });
            }


            TimeSpan difference = End - Start;
            if(difference.TotalMinutes < Duration)
            {
                yield return new ValidationResult($"Duration must be less than or equal to the time range", new[] { "Duration", "Start", "End" });
            }
        }
    }
}
