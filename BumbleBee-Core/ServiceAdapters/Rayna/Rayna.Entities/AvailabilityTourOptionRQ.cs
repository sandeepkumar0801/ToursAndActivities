using Newtonsoft.Json;

namespace ServiceAdapters.Rayna.Rayna.Entities
{
    public class AvailabilityTourOptionRQ
    {
        [JsonProperty(PropertyName = "tourId")]
        public int TourId { get; set; }
        [JsonProperty(PropertyName = "contractId")]
        public int ContractId { get; set; }
        [JsonProperty(PropertyName = "travelDate")]
        public string TravelDate { get; set; }
        [JsonProperty(PropertyName = "noOfAdult")]
        public int NoOfAdult { get; set; }
        [JsonProperty(PropertyName = "noOfChild")]
        public int NoOfChild { get; set; }
        [JsonProperty(PropertyName = "noOfInfant")]
        public int NoOfInfant { get; set; }
    }



}
