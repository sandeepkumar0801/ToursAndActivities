using TableStorageOperations.Models.AdditionalPropertiesModels;

namespace TableStorageOperations.Models.Booking
{
    public class AsyncBookingLog : CustomTableEntity
    {
        public string Id { get; set; }
        public string AsyncBookingId { get; set; }
        public string Status { get; set; }
        public string ProcessedDateTime { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string RequestType { get; set; }
    }
}