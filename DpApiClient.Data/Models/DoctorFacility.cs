using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DpApiClient.Data
{
    public class DoctorFacility
    {
        public int DoctorId { get; set; }
        public int FacilityId { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual DoctorMapping DoctorMapping { get; set; }

        public virtual bool IsMapped
        {
            get { return DoctorMapping != null; }
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
    }
}
