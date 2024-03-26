using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
	public class Package : IdentifierWithName
	{
		[JsonProperty(PropertyName = "variation")]
		public EnumValue Variation { get; set; }
		[JsonProperty(PropertyName = "linkId")]
		public int LinkId { get; set; }
		[JsonProperty(PropertyName = "currency")]
		public string CurrencyCode { get; set; }
		[JsonProperty(PropertyName = "price")]
		public decimal? Price { get; set; }
	}
}
