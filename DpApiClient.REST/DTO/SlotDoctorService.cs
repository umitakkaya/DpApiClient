using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    public class SlotDoctorService
    {
        public string DoctorServiceId { get; set; }
        public int Duration { get; set; }
    }
}
