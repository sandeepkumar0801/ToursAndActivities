using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.Rayna.Rayna.Entities
{
    public class AvailabilityTimeSlotRQ
    {
        [JsonProperty(PropertyName = "tourId")]
        public int TourId { get; set; }
        [JsonProperty(PropertyName = "tourOptionId")]
        public int TourOptionId { get; set; }
        [JsonProperty(PropertyName = "travelDate")]
        public string TravelDate { get; set; }
        [JsonProperty(PropertyName = "transferId")]
        public int TransferId { get; set; }
        [JsonProperty(PropertyName = "contractId")]
        public int ContractId { get; set; }
    }

}
