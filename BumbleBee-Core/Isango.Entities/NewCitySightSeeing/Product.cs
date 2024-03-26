using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.NewCitySightSeeing
{
	public class Product
    {
		public int Id { get; set; }
		public string? sku { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
		public string? content { get; set; }

        public string? notes { get; set; }
        public string? duration { get; set; }
        public string? availableDays { get; set; }
        public string? cancellationPolicy { get; set; }
        public string? DestinationTitle { get; set; }
        public string? DestinationSubTitle { get; set; }
        public string? ShortDescription { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public float zoom { get; set; }
        public string? address { get; set; }
    }
}
