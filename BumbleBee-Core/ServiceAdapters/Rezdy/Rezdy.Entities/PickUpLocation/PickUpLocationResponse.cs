

using System.Collections.Generic;
using Newtonsoft.Json;

namespace ServiceAdapters.Rezdy.Rezdy.Entities.PickUpLocation
{
    public class PickUpLocationResponse
    {
        [JsonProperty("pickupList")]
        public PickUpList PickUps{ get; set; }

        [JsonProperty("requestStatus")]
        public PickupRequestStatus RequestStatus { get; set; }
    }

    public class PickupRequestStatus
    {
        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }


    public class PickUpList
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")] 
        public string Name { get; set; }

        [JsonProperty("additionalNotes")]
        public string AdditionalNotes { get; set; }

        [JsonProperty("otherLocationInstructions")]
        public string OtherLocationInstructions { get; set; }

        [JsonProperty("pickupLocations")]
        public List<PickUpLocation> PickUpLocations { get; set; }
    }

    public class PickUpLocation
    {
        [JsonProperty("locationName")]
        public string LocationName { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("minutesPrior")]
        public int MinutesPrior{ get; set; }

        [JsonProperty("additionalInstructions")]
        public string AdditionalInstructions { get; set; }
    }

}
