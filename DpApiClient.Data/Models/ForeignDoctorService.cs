using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DpApiClient.Data
{
    public class ForeignDoctorService
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? Duration { get; set; }
        public double? PriceMin { get; set; }
        public double? PriceMax { get; set; }
        public string ServiceId { get; set; }


        public string ForeignDoctorId { get; set; }
        public ForeignDoctor ForeignDoctor { get; set; }

        private List<DoctorMapping> _doctorMappings;
        public virtual List<DoctorMapping> DoctorMappings
        {
            get { return _doctorMappings ?? (_doctorMappings = new List<DoctorMapping>()); }
            set { _doctorMappings = value; }
        }

        private List<Visit> _visits;
        public virtual List<Visit> Visits
        {
            get { return _visits ?? (_visits = new List<Visit>()); }
            set { _visits = value; }
        }

        private List<DoctorSchedule> _doctorSchedules;
        public virtual List<DoctorSchedule> DoctorSchedules
        {
            get { return _doctorSchedules ?? (_doctorSchedules = new List<DoctorSchedule>()); }
            set { _doctorSchedules = value; }
        }

        public virtual bool IsBelongsTo(Doctor doctor)
        {
            var mappings = ForeignDoctor.ForeignAddresses.Where(fa => fa.DoctorMapping != null).Select(fa => fa.DoctorMapping);
            return mappings.Any(dm => dm.DoctorId == doctor.Id);
        }

        public virtual int? MappedDoctorId
        {
            get
            {
                var mapping = ForeignDoctor.ForeignAddresses
                    .Where(fa => fa.DoctorMapping != null)
                    .Select(fa => fa.DoctorMapping)
                    .FirstOrDefault();
                if (mapping == null)
                {
                    return null;
                }
                return mapping.DoctorId;
            }
        }
    }
}
