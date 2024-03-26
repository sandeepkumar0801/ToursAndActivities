using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Isango.Entities
{
    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public HttpStatusCode HttpStatus { get; set; }
        public bool IsLoggedInDB { get; set; }
    }

    public enum CommonErrorCodes
    {
        Default = 0,
        RuntimeException = 999,
        SupplierBookingError = 1000,
        BookingError = 1001,
        AvailabilityError = 1002,
        PaymentGatewayError = 1004,
        DBException = 1005,
        NoWebhookError = 1006
    }

    public sealed class CommonErrorConstants
    {
        public const string ActivityDuplicatePassengersCODE = "DUPLICATE_PASSENGERS_TYPES";
        public const string ActivityNotFoundCODE = "ACTIVITY_NOT_FOUND";
        public const string AffiliateNotCODE = "AFFILIATE_NOT_FOUND.";
        public const string ActivityOptionNotFoundCODE = "ACTIVITY_OPTIONS_NOT_FOUND.";
        public const string DateRangeExceedsCODE = "DATE_RANGE_EXCEEDS";

        public const string BundleComponentyAvailabilityNotFoundCODE = "BUNDLE_COMPONENT_AVAILABILITY_NOT_FOUND";
        public const string BundleComponentyAvailabilityNotFound = "Availability not found for any all component of bundle.";

        public const string ActivityNotFound = "Activity not found.";
        public const string ActivityOptionNotFound = "Activity Options not found.";
        public const string ActivityDuplicatePassengers = "Duplicate passengers types in request.";
        public const string AffiliateNotFound = "Affiliate not found.";

        public const string ActivityOptionNotFound_CODE = "OPTIONS_NOT_FOUND_FOR_ALL_ACTIVITIES";
        public const string ActivityOptionNotFound_Message = "Options not found for all activities of the bundle.";

        public const string OPTION_NOT_FOUND = "OPTION_NOT_FOUND";
        public const string INVALID_DATE_RANGE = "INVALID_DATE_RANGE";
        public const string INVALID_DATE = "INVALID_DATE";
        public const string INVALID_TIME = "INVALID_TIME";
        public const string INVALID_PERSON_TYPE = "INVALID_PERSON_TYPE";
        public const string NOT_BOOKABLE_INDEPENDENTLY = "NOT_BOOKABLE_INDEPENDENTLY";
        public const string INVALID_QUANTITY = "INVALID_QUANTITY";
        public const string AGENT_REQUIRED = "AGENT_REQUIRED";
        public const string INVALID_AGENT = "INVALID_AGENT";
        public const string NOT_AVAILABLE = "NOT_AVAILABLE";
        public const string RESERVATION_NOT_FOUND = "RESERVATION_NOT_FOUND";
        public const string INVALID_BOOKING_BODY = "INVALID_BOOKING_BODY";
        public const string BOOKING_NOT_FOUND = "BOOKING_NOT_FOUND";
        public const string UNEXPECTED_ERROR = "UNEXPECTED_ERROR";

    }

    public class ErrorList
    {
        public ErrorList()
        {
            Errors = new List<Error>();
        }

        public virtual List<Error> Errors { get; set; }

        public virtual void UpdateErrors(CommonErrorCodes code, HttpStatusCode httpStatusCode, string errorMessage)
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

        public virtual void UpdateDBLogFlag()
        {
            if (Errors == null)
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
}