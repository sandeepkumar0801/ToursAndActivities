using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.GoCity.GoCity.Entities.Booking
{

    public class CancelBookingRequest
    {
        [JsonProperty("orderCancelRequests")]
        public List<Ordercancelrequest> OrderCancelRequests { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
        [JsonProperty("internalUserEmail")]
        public string InternalUserEmail { get; set; }
    }

    public class Ordercancelrequest
    {
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }
    }
 }