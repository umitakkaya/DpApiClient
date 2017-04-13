using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    public class PutSlotsRequest
    {
        public List<SlotRange> Slots { get; set; }
    }
}
