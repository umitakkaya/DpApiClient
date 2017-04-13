using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Data
{
    public class ForeignSpecialization
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string ForeignDoctorId { get; set; }
        public ForeignDoctor ForeignDoctor { get; set; }
    }
}
