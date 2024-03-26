namespace Isango.Entities.Enums
{
    public enum BookingStatus
    {
        //OptionBookingStatus
        //On iSango db booking, need to send these enum numbers
        //	Failed = 0,
        //  Requested = 1,
        //  Confirmed = 2,
        //  Cancelled = 3,
        //	consumed = 4

        /// <summary>
        /// Defines the booking for the tour selected is confirmed
        /// </summary>
        UnKnown = 0,

        /// <summary>
        /// Defines the booking for the tour selected is confirmed, If all Available products are booked
        /// </summary>
        Confirmed = 1,

        /// <summary>
        /// Defines the booking for the tour selected is cancelled,  If all products are cancelled
        /// </summary>
        Cancelled = 2,

        /// <summary>
        /// Defines the booking for the tour selected is requested, If any single product is OnRequest
        /// </summary>
        Requested = 3,

        /// <summary>
        /// Defines the booking for the tour selected is consumed/exhausted
        /// </summary>
        Consumed = 4,

        /// <summary>
        /// Defines the booking for the tour is partially succeeded
        /// </summary>
        Partial = 5,

        /// <summary>
        /// Defines the booking for tour is failed.
        /// </summary>
        Failed = 6,

        Initiated = 7,
        EXPIRED = 8

    }
}