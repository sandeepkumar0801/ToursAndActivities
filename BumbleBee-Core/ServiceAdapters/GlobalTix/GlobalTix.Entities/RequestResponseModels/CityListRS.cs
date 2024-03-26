using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
	public class CityListRS
	{
		[JsonProperty(PropertyName = "data")]
		public List<CityData> Cities { get; set; }
		[JsonProperty(PropertyName = "success")]
		public bool IsSuccess { get; set; }
	}

	public class CityData : IdentifierWithName
	{
		[JsonProperty(PropertyName = "countryId")]
		public int CountryId { get; set; }
	}
}
