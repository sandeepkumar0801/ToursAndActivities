using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities
{
    public class ActivityPickupLocations
    {
        public int? ServiceID { get; set; }
        public string Pickuplocation { get; set; }
        public string Languagecode { get; set; }
        public int? Serviceoptionid { get; set; }
        public int? ID { get; set; }
    }
}
