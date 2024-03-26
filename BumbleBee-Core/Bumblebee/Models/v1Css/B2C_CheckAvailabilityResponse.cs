using Isango.Entities;
using System.Collections.Generic;
using WebAPI.Models.ResponseModels.CheckAvailability;

namespace WebAPI.Models.ResponseModels
{
    public class B2C_CheckAvailabilityResponse
    {
        public int ActivityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TokenId { get; set; }
        public List<B2C_Option> Options { get; set; }

        /// <summary>
        /// If its true and passenger count is more than 1 then additional passenger details is required for booking, else only lead passenger details is required to create booking.
        /// </summary>
        public bool IsPaxDetailRequired { get; set; }


        /// <summary>
        /// If its then passenger details is required for during Booking Reservation call.
        /// </summary>
        public bool IsPaxDetailRequiredDuringReservation { get; set; }
        public List<Error> Errors { get; set; }
    }
}