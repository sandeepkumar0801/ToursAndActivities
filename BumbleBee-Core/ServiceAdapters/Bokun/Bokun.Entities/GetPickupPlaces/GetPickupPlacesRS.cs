using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Bokun.Bokun.Entities.GetPickupPlaces
{
    public class GetPickupPlacesRS
    {
        [JsonProperty("dropoffPlaces")]
        public List<PlaceDetails> DropoffPlaces { get; set; }

        [JsonProperty("pickupPlaces")]
        public List<PlaceDetails> PickupPlaces { get; set; }
    }

    public class PlaceDetails
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        public string type { get; set; }
        public bool askForRoomNumber { get; set; }
        public Location location { get; set; }
        public object unLocode { get; set; }
        public string[] flags { get; set; }
    }

    public class Location
    {
        public string address { get; set; }
        public string city { get; set; }
        public string countryCode { get; set; }
        public string postCode { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string zoomLevel { get; set; }
        public object origin { get; set; }
        public object originId { get; set; }
        public string wholeAddress { get; set; }
    }
}