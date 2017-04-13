using RestSharp.Deserializers;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    [DeserializeAs(Name = "Facility")]
    public class DPFacility : DPResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
