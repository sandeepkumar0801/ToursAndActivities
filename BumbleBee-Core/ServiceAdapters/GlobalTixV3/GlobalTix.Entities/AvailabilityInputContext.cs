using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities
{
    public class AvailabilityInputContext
    {
        public string CountryId { get; set; }
        public string Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
		public int PageNumber { get; set; }
	}
}
