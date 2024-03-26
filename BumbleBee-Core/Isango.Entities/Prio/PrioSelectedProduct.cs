namespace Isango.Entities.Prio
{
    public class PrioSelectedProduct : SelectedProduct
    {
        public PrioApi PrioApiConfirmedBooking { get; set; }

        public string PrioReservationReference { get; set; }

        public string PrioDistributorReference { get; set; }

        public string PrioBookingStatus { get; set; }

        public int PrioTicketClass { get; set; }

        public string PickupPoints { get; set; }

        public PickupPointDetails[] PickupPointDetails { get; set; }

        public string ReservationExpiry { get; set; }

        public string Code { get; set; }

        public string Destination { get; set; }

        public int FactsheetId { get; set; }
    }
}