namespace Isango.Entities.Booking
{
    public class EnrollmentCheckResponse
    {
        public string EnrollmentErrorOrHTML { get; set; }
        public string EnrollmentErrorCode { get; set; }
        public string BookingReferenceId { get; set; }
        public bool IsBookingSuccessful { get; set; }
        public bool IsPartialBooking { get; set; }
        public bool Is2DBooking { get; set; }
    }
}