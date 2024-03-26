namespace Isango.Entities.Booking
{
    public class BookingDetail
    {
        public int BookingDetailId { get; set; }

        public string ServiceName { get; set; }

        public int ServiceId { get; set; }

        public string BookingCurrency { get; set; }

        public string SupplierCurrency { get; set; }

        public int ServiceOptionId { get; set; }

        public int PassengerId { get; set; }

        public bool IsHotelbed { get; set; }

        public string HbReferenceNo { get; set; }

        public string OfficeCode { get; set; }

        public string LanguageCode { get; set; }

        public int TrakerStatusId { get; set; }
    }
}