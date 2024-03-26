using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.ProductChanges
{
    public class ProductChangesList
    {

        [JsonProperty(PropertyName = "data")]
        public List<Datum> Data { get; set; }

        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }

        [JsonProperty(PropertyName = "size")]
        public int Size { get; set; }

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
        

        public class Datum
        {
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "lastUpdated")]
            public DateTime LastUpdated { get; set; }

            [JsonProperty(PropertyName = "id")]
            public int Id { get; set; }

            [JsonProperty(PropertyName = "cityId")]
            public int CityId { get; set; }

            [JsonProperty(PropertyName = "countryId")]
            public int CountryId { get; set; }
        }

        


    }
}
