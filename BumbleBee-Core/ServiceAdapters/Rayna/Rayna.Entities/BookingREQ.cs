using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.Rayna.Rayna.Entities
{

    public class BookingREQ
    {
        [JsonProperty(PropertyName = "uniqueNo")]
        public string UniqueNo { get; set; }
        [JsonProperty(PropertyName = "TourDetails")]
        public List<Tourdetail> TourDetails { get; set; }
        [JsonProperty(PropertyName = "passengers")]
        public List<Passenger> Passengers { get; set; }
    }

    public class Tourdetail
    {
        [JsonProperty(PropertyName = "serviceUniqueId")]
        public string ServiceUniqueId { get; set; }
        [JsonProperty(PropertyName = "tourId")]
        public int TourId { get; set; }
        [JsonProperty(PropertyName = "optionId")]
        public int OptionId { get; set; }
        [JsonProperty(PropertyName = "adult")]
        public int Adult { get; set; }
        [JsonProperty(PropertyName = "child")]
        public int Child { get; set; }
        [JsonProperty(PropertyName = "infant")]
        public int Infant { get; set; }
        [JsonProperty(PropertyName = "tourDate")]
        public string TourDate { get; set; }
        [JsonProperty(PropertyName = "timeSlotId")]
        public int TimeSlotId { get; set; }
        [JsonProperty(PropertyName = "startTime")]
        public string StartTime { get; set; }
        [JsonProperty(PropertyName = "transferId")]
        public int TransferId { get; set; }
        [JsonProperty(PropertyName = "pickup")]
        public string Pickup { get; set; }
        [JsonProperty(PropertyName = "adultRate")]
        public decimal AdultRate { get; set; }
        [JsonProperty(PropertyName = "childRate")]
        public decimal ChildRate { get; set; }
        [JsonProperty(PropertyName = "serviceTotal")]
        public string ServiceTotal { get; set; }
    }

    public class Passenger
    {
        [JsonProperty(PropertyName = "serviceType")]
        public string ServiceType { get; set; }
        [JsonProperty(PropertyName = "prefix")]
        public string Prefix { get; set; }
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "mobile")]
        public string Mobile { get; set; }
        [JsonProperty(PropertyName = "nationality")]
        public string Nationality { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "leadPassenger")]
        public int LeadPassenger { get; set; }
        [JsonProperty(PropertyName = "paxType")]
        public string PaxType { get; set; }
        [JsonProperty(PropertyName = "clientReferenceNo")]
        public string ClientReferenceNo { get; set; }
    }

}
