using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Rayna.Rayna.Entities
{


    public class TourTicketRES
    {
        [JsonProperty(PropertyName = "statuscode")]
        public int Statuscode { get; set; }
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
        [JsonProperty(PropertyName = "result")]
        public ResultTicket ResultTicket { get; set; }
    }

    public class ResultTicket
    {
        [JsonProperty(PropertyName = "bookingId")]
        public int BookingId { get; set; }
        [JsonProperty(PropertyName = "referenceNo")]
        public string ReferenceNo { get; set; }
        [JsonProperty(PropertyName = "BookingStatus")]
        public string BookingStatus { get; set; }
        [JsonProperty(PropertyName = "ticketURL")]
        public string TicketURL { get; set; }
        [JsonProperty(PropertyName = "optionName")]
        public string OptionName { get; set; }
        [JsonProperty(PropertyName = "validity")]
        public string Validity { get; set; }
        [JsonProperty(PropertyName = "validityExtraDetails")]
        public string ValidityExtraDetails { get; set; }
        [JsonProperty(PropertyName = "printType")]
        public string PrintType { get; set; }
        [JsonProperty(PropertyName = "slot")]
        public string Slot { get; set; }
        [JsonProperty(PropertyName = "pnrNumber")]
        public string PnrNumber { get; set; }
        [JsonProperty(PropertyName = "ticketDetails")]
        public List<TicketDetail> TicketDetails { get; set; }
    }

    public class TicketDetail
    {
        [JsonProperty(PropertyName = "barCode")]
        public string BarCode { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "noOfAdult")]
        public int NoOfAdult { get; set; }
        [JsonProperty(PropertyName = "noOfchild")]
        public int NoOfchild { get; set; }
    }

  
}
