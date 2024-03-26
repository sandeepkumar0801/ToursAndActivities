using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Rayna.Rayna.Entities
{
    public class CityByCountry
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
        public List<ResultCityByCountry> ResultCityByCountry { get; set; }
    }

    public class ResultCityByCountry
    {
        [JsonProperty(PropertyName = "cityId")]
        public int CityId { get; set; }
        [JsonProperty(PropertyName = "cityName")]
        public string CityName { get; set; }
    }
}