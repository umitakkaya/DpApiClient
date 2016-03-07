using Newtonsoft.Json;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    public class Patient : DPResponse
    {
        /// <summary>
        /// National Identity Number:
        /// For Turkey: T.C. Kimlik No,
        /// For Poland: PESEL
        /// </summary>
        public string Nin { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// "m" stands for male, "f" stands for female
        /// </summary>
        /// <value>"m" stands for male, "f" stands for female</value>
        public string Gender { get; set; }
        
    }
}
