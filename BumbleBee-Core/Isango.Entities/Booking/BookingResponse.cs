using System;
using System.Collections.Generic;
using System.Linq;

namespace Isango.Entities.Booking
{
    public class BookingResponse : ErrorList
    {
        public string Status { get; set; }
        public string ReferenceId { get; set; }
        public string Message { get; set; }
        public GatewayDetail GatewayDetail { get; set; }
        public BookingDetailResponse BookingDetail { get; set; }
        public bool IsWebhookReceived { get; set; }
        
    }
}