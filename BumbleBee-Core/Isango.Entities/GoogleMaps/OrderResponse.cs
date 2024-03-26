namespace Isango.Entities.GoogleMaps
{
    public class OrderResponse : CustomTableEntity
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public string Order { get; set; }
        public string BookingStatus { get; set; }
        public bool IsNotifiedToGoogle { get; set; }
    }

    public enum BookingStatus
    {
        Confirmed,
        Cancelled
    }
}