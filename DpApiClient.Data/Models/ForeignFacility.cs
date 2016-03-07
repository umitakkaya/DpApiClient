using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DpApiClient.Data
{
    public class ForeignFacility
    {
        public string Id { get; set; }
        public string Name { get; set; }

        private List<ForeignAddress> _foreignAddresses;
        public virtual List<ForeignAddress> ForeignAddresses
        {
            get { return _foreignAddresses ?? (_foreignAddresses = new List<ForeignAddress>()); }
            set { _foreignAddresses = value; }
        }
    }
}
