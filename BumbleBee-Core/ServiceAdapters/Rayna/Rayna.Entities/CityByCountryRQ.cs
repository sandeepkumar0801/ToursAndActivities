using Newtonsoft.Json;
namespace ServiceAdapters.Rayna.Rayna.Entities
{
    public class CityByCountryRQ
    {
        [JsonProperty(PropertyName = "countryId")]
        public int? CountryId { get; set; }

    }

    public class TourStaticDataRQ
    {
        [JsonProperty(PropertyName = "countryId")]
        public int CountryId { get; set; }

        [JsonProperty(PropertyName = "cityId")]
        public int CityId { get; set; }

    }


    public class TourStaticDataByIdRQ
    {
        [JsonProperty(PropertyName = "countryId")]
        public int CountryId { get; set; }

        [JsonProperty(PropertyName = "cityId")]
        public int CityId { get; set; }

        [JsonProperty(PropertyName = "tourId")]
        public int TourId { get; set; }

        [JsonProperty(PropertyName = "contractId")]
        public int ContractId { get; set; }


        [JsonProperty(PropertyName = "travelDate")]
        public string TravelDate { get; set; }


    }
    public class TourOptionsRQ
    {

        [JsonProperty(PropertyName = "tourId")]
        public int TourId { get; set; }

        [JsonProperty(PropertyName = "contractId")]
        public int ContractId { get; set; }
    }
}