using System.Collections.Generic;

namespace DpApiClient.Data
{
    public class ForeignDoctor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public virtual string Fullname
        {
            get { return $"{Name} {Surname}"; }
        }

        private List<ForeignSpecialization> _foreignSpecializations;
        public virtual List<ForeignSpecialization> ForeignSpecializations
        {
            get { return _foreignSpecializations ?? (_foreignSpecializations = new List<ForeignSpecialization>()); }
            set { _foreignSpecializations = value; }
        }



        private List<ForeignAddress> _foreignAddresses;
        public virtual List<ForeignAddress> ForeignAddresses
        {
            get { return _foreignAddresses ?? (_foreignAddresses = new List<ForeignAddress>()); }
            set { _foreignAddresses = value; }
        }


        private List<ForeignDoctorService> _foreignDoctorServices;
        public virtual List<ForeignDoctorService> ForeignDoctorServices
        {
            get { return _foreignDoctorServices ?? (_foreignDoctorServices = new List<ForeignDoctorService>()); }
            set { _foreignDoctorServices = value; }
        }
    }
}
