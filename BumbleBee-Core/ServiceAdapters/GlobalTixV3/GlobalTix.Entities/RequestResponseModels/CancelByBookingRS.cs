using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels
{
    public class CancelByBookingRS
    {
        //[JsonProperty(PropertyName = "error")]
        //public Error errorData { get; set; }

        //[JsonProperty(PropertyName = "data")]
        //public CancelByBookingRSData Data { get; set; }
        
        [JsonProperty(PropertyName = "success")]
        public bool IsSuccess { get; set; }
        
        [JsonProperty(PropertyName = "size")]
        public object SizeOf { get; set; }
    }

    //public class CancelByBookingRSData
    //{
    //    [JsonProperty(PropertyName = "tickets")]
    //    public List<BookTicket> Tickets;
    //}

    //public class Error
    //{
    //    public string code { get; set; }
    //    public object errorDetails { get; set; }
    //    public string message { get; set; }
    //}
}
