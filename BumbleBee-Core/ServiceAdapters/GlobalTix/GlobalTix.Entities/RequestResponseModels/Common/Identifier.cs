using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class Identifier
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    }

	public class IdentifierWithClass : Identifier
	{
		[JsonProperty(PropertyName = "class")]
		public string Class { get; set; }
	}

	public class IdentifierWithName : Identifier
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

}
