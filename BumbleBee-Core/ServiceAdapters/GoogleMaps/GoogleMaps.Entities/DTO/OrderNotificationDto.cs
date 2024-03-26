namespace ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO
{
    public class OrderNotificationDto
    {
        public string PartnerId { get; set; }
        public string OrderId { get; set; }
        public string UpdateMask { get; set; }
        public OrderNotificationRequest OrderNotificationRequest { get; set; }
    }
}
