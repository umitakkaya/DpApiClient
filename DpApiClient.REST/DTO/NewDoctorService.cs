using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    public class NewDoctorService
    {
        public string ServiceId { get; set; }
        public double PriceMin { get; set; }
        public double PriceMax { get; set; }
    }
}
