using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities
{
    public class AvailabilityInputContext : InputContext
    {
        public string CountryId { get; set; }
        public string CityId { get; set; }
        public int PageNumber { get; set; }
    }
}
