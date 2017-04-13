using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DpApiClient.Data
{
    public class DoctorMapping
    {
        public int DoctorId { get; set; }
        public int FacilityId { get; set; }

        public virtual DoctorFacility DoctorFacility { get; set; }
        public virtual ForeignAddress ForeignAddress { get; set; }

        public string ForeignDoctorServiceId { get; set; }
        public virtual ForeignDoctorService ForeignDoctorService { get; set; }
    }
}
