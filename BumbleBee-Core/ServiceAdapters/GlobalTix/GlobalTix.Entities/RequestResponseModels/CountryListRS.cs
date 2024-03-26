using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
	public class CountryListRS
	{
		[JsonProperty(PropertyName = "data")]
		public List<IdentifierWithName> Countries { get; set; }
		[JsonProperty(PropertyName = "success")]
		public bool IsSuccess { get; set; }
	}
}
