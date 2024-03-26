using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Rayna.Rayna.Entities
{
    public class Countries
    {
        [JsonProperty(PropertyName = "statuscode")]
        public int Statuscode { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        [JsonProperty(PropertyName = "result")]
        public List<ResultCountry> ResultCountry { get; set; }
    }

    public class ResultCountry
    {
        [JsonProperty(PropertyName = "countryId")]
        public int CountryId { get; set; }
        [JsonProperty(PropertyName = "countryName")]
        public string CountryName { get; set; }
    }
}