using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    public class DoctorService : DPResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double? PriceMin { get; set; }
        public double? PriceMax { get; set; }

        /// <see cref="Service"/>
        public string ServiceId { get; set; }
    }
}
