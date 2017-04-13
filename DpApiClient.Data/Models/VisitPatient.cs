using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DpApiClient.Data
{
    public class VisitPatient
    {
        public int VisitId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        /// <summary>
        /// National Identity Number
        /// </summary>
        public string NIN { get; set; }
        public Gender? Gender { get; set; }
        public virtual string StrGender
        {
            get
            {
                if (Gender.HasValue)
                {
                    return Gender.Value == Data.Gender.Male ? "m" : "f";
                }
                else
                {
                    return null;
                }
            }
        }


        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Birthdate { get; set; }

        public Visit Visit { get; set; }
    }
}
