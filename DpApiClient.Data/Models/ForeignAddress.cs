using System.ComponentModel.DataAnnotations;

namespace DpApiClient.Data
{
    public class ForeignAddress
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string ForeignDoctorId { get; set; }
        public string ForeignFacilityId { get; set; }

        public virtual ForeignDoctor ForeignDoctor { get; set; }
        public virtual ForeignFacility ForeignFacility { get; set; }
        public virtual DoctorMapping DoctorMapping { get; set; }

        public virtual bool IsMapped { get { return DoctorMapping != null; } }

        /// <summary>
        /// Shows which extra fields are visible on the website.
        /// </summary>
        public virtual BookingExtraFields BookingExtraFields { get; set; }
    }
}
