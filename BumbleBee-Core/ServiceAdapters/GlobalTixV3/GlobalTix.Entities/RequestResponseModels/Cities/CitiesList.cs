using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.Cities
{
    public class CitiesList
    {
            
            [JsonProperty(PropertyName = "data")]
            public Datum[] Data { get; set; }

            [JsonProperty(PropertyName = "error")]
            public object Error { get; set; }

            [JsonProperty(PropertyName = "size")]
            public object Size { get; set; }

            [JsonProperty(PropertyName = "success")]
            public bool Success { get; set; }
        

        public class Datum
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("countryId")]
            public int CountryId { get; set; }

            [JsonProperty("timezoneOffset")]
            public int? TimezoneOffset { get; set; }
        }

    }
}
