using DpApiClient.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DpApiClient.ViewModels
{
    public class MappingViewModel
    {
        public IEnumerable<ForeignAddress> ForeignAddresses;
        public IEnumerable<DoctorFacility> DoctorFacilities;
        public IEnumerable<ForeignDoctorService> ForeignDoctorServices;
    }
}