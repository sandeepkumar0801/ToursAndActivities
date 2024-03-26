using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Rayna.Rayna.Entities
{

    public class TourStaticData
    {
        [JsonProperty(PropertyName = "statuscode")]
        public int StatusCode { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        [JsonProperty(PropertyName = "result")]
        public List<ResultTour> ResultTour { get; set; }
    }

    public class ResultTour
    {
        [JsonProperty(PropertyName = "tourId")]
        public int TourId { get; set; }
        [JsonProperty(PropertyName = "countryId")]
        public int CountryId { get; set; }
        [JsonProperty(PropertyName = "countryName")]
        public string CountryName { get; set; }
        [JsonProperty(PropertyName = "cityId")]
        public int CityId { get; set; }
        [JsonProperty(PropertyName = "cityName")]
        public string CityName { get; set; }
        [JsonProperty(PropertyName = "tourName")]
        public string TourName { get; set; }
        [JsonProperty(PropertyName = "reviewCount")]
        public int ReviewCount { get; set; }
        [JsonProperty(PropertyName = "rating")]
        public float Rating { get; set; }
        [JsonProperty(PropertyName = "duration")]
        public string Duration { get; set; }
        [JsonProperty(PropertyName = "imagePath")]
        public string ImagePath { get; set; }
        [JsonProperty(PropertyName = "imageCaptionName")]
        public string ImageCaptionName { get; set; }
        [JsonProperty(PropertyName = "cityTourTypeId")]
        public string CityTourTypeId { get; set; }
        [JsonProperty(PropertyName = "cityTourType")]
        public string CityTourType { get; set; }
        [JsonProperty(PropertyName = "tourShortDescription")]
        public string TourShortDescription { get; set; }
        [JsonProperty(PropertyName = "cancellationPolicyName")]
        public string CancellationPolicyName { get; set; }
        [JsonProperty(PropertyName = "isSlot")]
        public bool IsSlot { get; set; }
        [JsonProperty(PropertyName = "onlyChild")]
        public bool OnlyChild { get; set; }
        [JsonProperty(PropertyName = "contractId")]
        public int ContractId { get; set; }
        [JsonProperty(PropertyName = "recommended")]
        public bool Recommended { get; set; }
        [JsonProperty(PropertyName = "isPrivate")]
        public bool IsPrivate { get; set; }
    }
}
