namespace TableStorageOperations.Models.AdditionalPropertiesModels.Booking
{
    public class BookingRequest : CustomTableEntity
    {
        public string CreateBookingRequest { get; set; }
        public string BookingReferenceNumber { get; set; }

        public string CreateBookingResponse { get; set; }
        public string Status { get; set; }
        public string StatusCode { get; set; }
        public string Token { get; set; }
        public bool IsCssBookingRequired { get; set; }


    }
}