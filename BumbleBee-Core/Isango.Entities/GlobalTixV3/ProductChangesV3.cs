using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.GlobalTixV3
{
    public class ProductChangesV3
    {
        public string name { get; set; }
        public DateTime lastUpdated { get; set; }
        public int id { get; set; }
        public int cityId { get; set; }
        public int countryId { get; set; }
    }
}
