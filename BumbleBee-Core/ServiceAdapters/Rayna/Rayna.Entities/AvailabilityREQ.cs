using Newtonsoft.Json;


namespace ServiceAdapters.Rayna.Rayna.Entities
{
    public class AvailabilityREQ
    {
        [JsonProperty(PropertyName = "tourId")]
        public int TourId { get; set; }
        [JsonProperty(PropertyName = "tourOptionId")]
        public int TourOptionId { get; set; }
        [JsonProperty(PropertyName = "transferId")]
        public int TransferId { get; set; }
        [JsonProperty(PropertyName = "travelDate")]
        public string TravelDate { get; set; }
        [JsonProperty(PropertyName = "adult")]
        public int Adult { get; set; }
        [JsonProperty(PropertyName = "child")]
        public int Child { get; set; }
        [JsonProperty(PropertyName = "infant")]
        public int Infant { get; set; }
        [JsonProperty(PropertyName = "contractId")]
        public int ContractId { get; set; }
    }
}
