namespace ServiceAdapters.HB.HB.Entities.Booking
{
    /// <summary>
    /// Used for getting booking detail and cancelling the booking
    /// GetBookingDetail using following
    ///  1) https://api.test.hotelbeds.com/activity-api/3.0/bookings/language/booking_refrerence
    ///  2) https://api.test.hotelbeds.com/activity-api/3.0/bookings/language/customer_refrerence/holder_name/holder_surname/start_date/end_date
    ///
    /// CancelBooinkingRequset in 2 step
    ///  1) https://api.test.hotelbeds.com/activity-api/3.0/bookings/en/ISANGOTEST0001?cancellationFlag=SIMULATION
    ///  2) https://api.test.hotelbeds.com/activity-api/3.0/bookings/en/ISANGOTEST0001?cancellationFlag=CANCELLATION
    /// </summary>
    public class BookingRq
    {
        public string Language { get; set; }

        public string BookingReference { get; set; }
        public string CustomerRefrerence { get; set; }

        public string HolderName { get; set; }

        public string HolderSurname { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}