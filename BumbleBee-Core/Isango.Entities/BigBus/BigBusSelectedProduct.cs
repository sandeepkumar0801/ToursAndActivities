using System.Collections.Generic;

namespace Isango.Entities.BigBus
{
    public class BigBusSelectedProduct : SelectedProduct
    {
        public string BookingStatus { get; set; }
        public string ReservationReference { get; set; }
        public bool TicketPerPassenger { get; set; }
        public string BookingReference { get; set; }
        public string ShortReference { get; set; }
        public string SupplierReferenceNumber { get; set; }
        public List<BigBusTicket> BigBusTickets { get; set; }
    }

    public class BigBusTicket
    {
        public string TicketType { get; set; }

        public string TicketBarCode { get; set; }
        public string PassengerType { get; set; }
        public string Quantity { get; set; }
    }
}