using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DpApiClient.Data
{
    public class BookingExtraFields
    {
        public string ForeignAddressId { get; set; }
        public ForeignAddress ForeignAddress { get; set; }

        public bool IsBirthDate { get; set; }
        public bool IsGender { get; set; }
        public bool IsNin { get; set; }
    }
}
