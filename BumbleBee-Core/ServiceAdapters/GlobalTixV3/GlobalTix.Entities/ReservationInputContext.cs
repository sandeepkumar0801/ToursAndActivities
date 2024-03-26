using Isango.Entities;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using System.Collections.Generic;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Entities
{
    public class ReservationInputContext : InputContext
    {
        public List<SelectedProduct> SelectedProducts { get; set; }
        public string BookingReferenceNumber { get; set; } 
    }
}
