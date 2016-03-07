using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    [DeserializeAs(Name = "Doctor")]
    public class DPDoctor : DPResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DPCollection<Specialization> Specializations { get; set; }
    }
}
