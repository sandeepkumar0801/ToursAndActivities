using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace Isango.Entities.Booking
{
    public class ReservationResponse : ErrorList
    {
        /// <summary>
        /// On successful reservation this field will have Booking Reference Number.
        /// </summary>
        public string BookingReferenceNumber { get; set; }

        /// <summary>
        /// Success status of reservation.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Expiration Time In Minutes , If booking reservation is not confirmed in given time it will be automatically canceled after this time.
        /// </summary>
        public int ExpirationTimeInMinutes { get; set; }

        /// <summary>
        /// Selected products to be reserved.
        /// </summary>
        public List<BookedProductStatus> Products { get; set; }

        [JsonIgnore]
        public override List<Error> Errors { get; set; }


        public override void UpdateErrors(CommonErrorCodes code, HttpStatusCode httpStatusCode, string errorMessage)
        {
            if (Errors == null)
            {
                Errors = new System.Collections.Generic.List<Error>();
            }

            var error = new Error
            {
                Code = code.ToString(),
                HttpStatus = httpStatusCode,
                Message = errorMessage
            };
            Errors.Add(error);
        }

        public override void UpdateDBLogFlag()
        {
            if (Errors == null || Errors?.Count == 0)
            {
                Errors = new System.Collections.Generic.List<Error>();
                var error = new Error
                {
                    Code = string.Empty,
                    HttpStatus = HttpStatusCode.BadRequest,
                    Message = string.Empty,
                    IsLoggedInDB = true
                };
                Errors.Add(error);
            }
            else
            {
                Errors.FirstOrDefault().IsLoggedInDB = true;
            }
        }
    }

    public class BookedProductStatus
    {
        public string AvailabilityReferenceID { get; set; }

        [JsonIgnore]
        public string Status { get; set; }
    }

    public class CancelReservationRequest
    {
        [Required]
        public string BookingReferenceNumber { get; set; }
    }

    public class ReservationDBDetails
    {
        public string Token { get; set; }
        public string AvailabilityRefNo { get; set; }
    }
}