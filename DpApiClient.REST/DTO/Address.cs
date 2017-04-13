using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    public class Address : DPResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PostCode { get; set; }
        public string Street { get; set; }
        public BookingExtraFields BookingExtraFields { get; set; }
    }
}
