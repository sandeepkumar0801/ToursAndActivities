using Newtonsoft.Json;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities
{
    public class AvailablityRs : EntityBase
    {
        [JsonProperty(PropertyName = "Response_type")]
        public string ResponseType { get; set; }

        public AvailablityRsData Data { get; set; }
    }

    public class AvailablityRsData
    {
        public Availability[] Availabilities { get; set; }
    }

    public class Availability
    {
        [JsonProperty(PropertyName = "From_date_time")]
        public string FromDateTime { get; set; }

        [JsonProperty(PropertyName = "To_date_time")]
        public string ToDateTime { get; set; }

        public string Vacancies { get; set; }
    }
}