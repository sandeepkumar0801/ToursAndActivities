namespace ServiceAdapters.PrioHub.PrioHub.Entities
{
    public sealed class ConstantPrioHub
    {
        public const string BOOKINGPROCESSINGCONFIRMATION = "BOOKING_PROCESSING_CONFIRMATION";//webhook
        public const string BOOKINGCONFIRMED = "BOOKING_CONFIRMED";//webhook

        public const string BOOKINGPROCESSINGCANCELLATION = "BOOKING_PROCESSING_CANCELLATION";//webhook
        public const string BOOKINGCANCELLED = "BOOKING_CANCELLED";//webhook
   
        public const string ORDERPENDING = "ORDER_PENDING";
        public const string ORDERCANCELLED = "ORDER_CANCELLED";
        public const string ORDERCONFIRMED = "ORDER_CONFIRMED";

        public const string BOOKINGRESERVED = "BOOKING_RESERVED";
        public const string BOOKINGRESERVATIONCANCELLED = "BOOKING_RESERVATION_CANCELLED";
        

        public const string CANCEL = "CANCEL";
        public const string CANCELWEBHOOK = "CANCELWEBHOOK";
        public const string BOOKING = "BOOKING";

        public const string INVALIDCANCELLATON = "INVALID_CANCELLATON";
        public const string CancellationPolicy = "cancellation policy";

        public const string CancelBooking = "Cancel Booking";
        public const string CancelReservation = "Cancel Reservation";
        
    }
}