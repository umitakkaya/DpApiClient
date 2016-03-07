using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Threading.Tasks;

namespace DpApiClient.Data
{
    public class Facility 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentFacilityId { get; set; }

        [ScaffoldColumn(true)]
        public virtual Facility ParentFacility { get; set; }

        private List<Facility> _branches;
        public virtual List<Facility> Branches
        {
            get { return _branches ?? (_branches = new List<Facility>()); }
            set { _branches = value; }
        }

        private List<DoctorFacility> _doctorFacilities;
        public virtual List<DoctorFacility> DoctorFacilities
        {
            get
            {
                return _doctorFacilities ?? (_doctorFacilities = new List<DoctorFacility>());
            }
            set
            {
                _doctorFacilities = value;
            }
        }
    }
}
