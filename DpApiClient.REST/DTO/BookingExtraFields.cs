using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    public class BookingExtraFields
    {

        [DeserializeAs(Name = "birth_date")]
        public bool IsBirthDateEnabled { get; set; }

        [DeserializeAs(Name = "gender")]
        public bool IsGenderEnabled { get; set; }

        [DeserializeAs(Name = "nin")]
        public bool IsNinEnabled { get; set; }
    }
}
