using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    public class DPCollection<T> : DPResponse
    {
        [DeserializeAs(Name = "_items")]
        public List<T> Items { get; set; }
    }
}
