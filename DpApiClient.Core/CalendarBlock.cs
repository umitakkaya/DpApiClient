using DpApiClient.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Core
{
    public class CalendarBlock
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Duration { get; set; }
        public ForeignDoctorService DoctorService { get; set; }
    }
}
