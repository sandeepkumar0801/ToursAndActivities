namespace Isango.Entities
{
    #region Prio Ticket

    public class PrioApi
    {
        public string DistributorReference { get; set; }
        public string BookingReference { get; set; }
        public string BookingStatus { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public BookingDetails[] BookingDetails { get; set; }
    }

    public class BookingDetails
    {
        public string VenueName { get; set; }
        public string CodeType { get; set; }
        public string GroupCode { get; set; }
        public TicketDetails[] TicketDetails { get; set; }
    }

    public class TicketDetails
    {
        public string TicketName { get; set; }
        public string TicketType { get; set; }
        public string TicketCode { get; set; }
    }

    public class PickupPointDetails
    {
        public string PickupPointId { get; set; }
    }

    #endregion Prio Ticket
}