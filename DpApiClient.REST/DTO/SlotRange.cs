using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    public class SlotRange : Slot
    {
        public DateTimeOffset End { get; set; }

        public List<SlotDoctorService> DoctorServices { get; set; }
    }
}
