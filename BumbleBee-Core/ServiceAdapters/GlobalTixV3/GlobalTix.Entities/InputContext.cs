using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities
{
    public class InputContext
    {
  
        public string AuthToken { get; set; }
        public MethodType MethodType { get; set; }
        public string CountryId { get; set; }
        public int id { get; set; }
        public int countryid { get; set; }
        public int cityid { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public string APIReferenceNumber { get; set; }
		public int PageNumber { get; set; }
	}
}
