using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities.Request
{
    public class Contact
    {
        [JsonProperty(PropertyName = "address")]
        public Address Address { get; set; }

        [JsonProperty(PropertyName = "phonenumber")]
        public string Phonenumber { get; set; }
    }
}