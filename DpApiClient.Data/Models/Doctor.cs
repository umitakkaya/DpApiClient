using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DpApiClient.Data
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private List<Specialization> _specializations;

        [ScaffoldColumn(true)]
        public virtual List<Specialization> Specializations
        {
            get { return _specializations ?? (_specializations = new List<Specialization>()); }
            set { _specializations = value; }
        }


        private List<DoctorFacility> _doctorFacilities;

        public virtual List<DoctorFacility> DoctorFacilities
        {
            get { return _doctorFacilities ?? (_doctorFacilities = new List<DoctorFacility>()); }
            set { _doctorFacilities = value; }
        }
    }
}
