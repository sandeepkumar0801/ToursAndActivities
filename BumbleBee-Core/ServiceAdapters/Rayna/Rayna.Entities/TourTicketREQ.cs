using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Rayna.Rayna.Entities
{

    public class TourTicketREQ
    {
        [JsonProperty(PropertyName = "uniqNO")]
        public string UniqNO { get; set; }
        [JsonProperty(PropertyName = "referenceNo")]
        public string ReferenceNo { get; set; }
        [JsonProperty(PropertyName = "bookedOption")]
        public List<Bookedoption> BookedOption { get; set; }
    }

    public class Bookedoption
    {
        [JsonProperty(PropertyName = "serviceUniqueId")]
        public string ServiceUniqueId { get; set; }
        [JsonProperty(PropertyName = "bookingId")]
        public int BookingId { get; set; }
    }


}
