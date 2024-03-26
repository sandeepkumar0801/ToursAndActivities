using Isango.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Isango.Entities.Rayna
{
    public class RaynaSelectedProduct : SelectedProduct
    {
        public string OrderStatus { get; set; }
        public bool Success { get; set; }

        public string TicketPdfUrl { get; set; }
        public string OrderReferenceId { get; set; }

        public bool DownloadRequired { get; set; }
        public string BookingReferenceNumber { get; set; }

        public string BookingId { get; set; }
    }
}