using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
	public class PackagesListRS
	{
		[JsonProperty(PropertyName = "data")]
		public List<Package> Packages { get; set; }
		[JsonProperty(PropertyName = "size")]
		public int TotalActivities { get; set; }
		[JsonProperty(PropertyName = "success")]
		public bool IsSuccess { get; set; }
	}
}
