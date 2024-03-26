namespace Util
{
    public sealed class CommonUtilConstant
    {
        public const string ResourceManagerBaseName = "Util.Resources.Common";
        public const string DE = "de";
        public const string ES = "es";
        public const string FR = "fr";
        public const string Germany = "de-De";
        public const string French = "fr-Fr";
        public const string Spanish = "es-Es";

        public const string DefaultMargin = "DefaultMargin";
        public const string DefaultLanguage = "en";
        public const string DefaultCurrency = "gbp";
        
    }

    public sealed class CommonUtilConstantCancellation
    {
        #region Cancellation Policy Text

        public const string CancellationPolicyDefaultFree24Hours = "CancellationPolicyDefaultFree24Hours";
        //public const string CancellationPolicyDefaultFree24Hours_en = "No cancellation charges up to 24 hours before the activity.";
        //public const string CancellationPolicyDefaultFree24Hours_de = "Keine Stornogebühr bis zu 24 Stunden vor der Tourbeginn.";
        //public const string CancellationPolicyDefaultFree24Hours_es = "Sin gastos de cancelación hasta 24 horas antes de la actividad.";

        /// <summary>
        /// 0 Percentage, 1 Hours
        /// </summary>
        public const string CancellationPolicyChargable = "CancellationPolicyChargable";

        /// <summary>
        /// 0 Hours
        /// </summary>
        public const string CancellationPolicyChargableWithoutPercentage = "CancellationPolicyChargableWithoutPercentage";

        //public const string CancellationPolicyChargable_en = "{0}% Cancellation charges will be applied if the booking is canceled {1} hours before the travel date.";
        //public const string CancellationPolicyChargable_de = "{0}% des Preises als Stornogebühr - {1} Stunden vor Tourbeginn.";
        //public const string CancellationPolicyChargable_es = "Se aplicará un {0}% de gastos de cancelación si la reserva se cancela {1} horas antes de la fecha de viaje.";

        public const string CancellationPolicyNonRefundable = "CancellationPolicyNonRefundable";
        //public const string CancellationPolicyNonRefundable_en = "Non- Refundable";
        //public const string CancellationPolicyNonRefundable_de = "Nicht erstattungsfähig";
        //public const string CancellationPolicyNonRefundable_es = "No es reembolsable";

        /// <summary>
        /// Free cancellation up to 24 hours before the tour/activity. Less than or equal to 24 hours - 100% charges.
        /// </summary>
        public const string CancellationPolicy100ChargableBeforeNhours = "CancellationPolicy100ChargableBeforeNhours";

        //public const string CancellationPolicy100ChargableBeforeNhours_en = "No cancellation charges up to {0} hours before the activity. Full charges applicable if canceled within {1} hours.";
        //public const string CancellationPolicy100ChargableBeforeNhours_de = "Mehr als {0} Stunden vor Tourbeginn: keine Stornogebühr.Weniger als {1} Stunden vor Tourbeginn: 100% Stornogebühr..";
        //public const string CancellationPolicy100ChargableBeforeNhours_es = "Sin gastos de cancelación hasta {0} horas antes de la actividad. Si se cancela dentro de las {1} horas, los gastos son totales";

        public const string CancellationPolicyLast = "CancellationPolicyLast";
        //public const string CancellationPolicyLast_en = "No cancellation charges up to {0} hours before the activity.";
        //public const string CancellationPolicyLast_de = "Keine Stornogebühr bis zu {0} Stunden vor der Tourbeginn.";
        //public const string CancellationPolicyLast_es = "Sin gastos de cancelación hasta {0} horas antes de la actividad.";

        public const string CancellationPolicyFreeBeforeTravelDate = "CancellationPolicyFreeBeforeTravelDate";
        //public const string CancellationPolicyFreeBeforeTravelDate_en = "No cancellation charges before the activity date.";
        //public const string CancellationPolicyFreeBeforeTravelDate_de = "Keine Stornogebühr vor dem Reisedatum.";
        //public const string CancellationPolicyFreeBeforeTravelDate_es = "Sin gastos de cancelación antes de la fecha de la actividad";

        public const string FreeCancellation = "FreeCancellation";

        #endregion Cancellation Policy Text
    }

    public sealed class ErrorCodes
    {
        public const string AVAILABILITY_ERROR = "AVAILABILITY_ERROR";
        public const string BUNDLE_AVAILABILITY_ERROR = "BUNDLE_AVAILABILITY_ERROR";
        public const string BUNDLE_OPTION_NOT_FOUND = "BUNDLE_OPTION_NOT_FOUND";
        public const string ACTIVITY_NOT_FOUND = "ACTIVITY_NOT_FOUND";
        public const string ACTIVITY_OPTION_NOT_FOUND = "ACTIVITY_OPTION_NOT_FOUND";
        public const string PASSENGER_PRICES_NOT_FOUND_IN_DB = "PASSENGER_PRICES_NOT_FOUND_IN_DB";
    }

    public sealed class ErrorMessages
    {
        public const string BUNDLE_OPTION_NOT_FOUND = "Bundle options not available as per criteria. Please change search criteria & try again.";
        public const string PASSENGER_PRICES_NOT_FOUND_IN_DB = "Passenger prices not found in database.";
        public const string ACTIVITY_NOT_FOUND = "Activity found in database/cosmos. Or Not configured for the given AffilateId.";
        public const string ACTIVITY_OPTION_NOT_FOUND = "Activity option not loaded from database/cosmos.";
        public const string ACTIVITY_OPTION_NOT_FOUND_FROM_API = "Activity option not available as per criteria from API.";
    }

    public sealed class HttpStatusCodes
    {
        public const string OK = "200";
        public const string CREATED = "201";
        public const string BAD_REQUEST = "400";
        public const string NOT_FOUND = "404";
        public const string CONFLICT = "409";
        public const string INTERNAL_SERVER_ERROR = "500";
        public const string UNAUTHORIZED = "401";
        public const string BADGATEWAY = "502";
    }

    //public enum HttpStatusCode
    //{
    //    Continue = 100,
    //    SwitchingProtocols = 101,
    //    OK = 200,
    //    Created = 201,
    //    Accepted = 202,
    //    NonAuthoritativeInformation = 203,
    //    NoContent = 204,
    //    ResetContent = 205,
    //    PartialContent = 206,
    //    MultipleChoices = 300,
    //    Ambiguous = 300,
    //    MovedPermanently = 301,
    //    Moved = 301,
    //    Found = 302,
    //    Redirect = 302,
    //    SeeOther = 303,
    //    RedirectMethod = 303,
    //    NotModified = 304,
    //    UseProxy = 305,
    //    Unused = 306,
    //    TemporaryRedirect = 307,
    //    RedirectKeepVerb = 307,
    //    BadRequest = 400,
    //    Unauthorized = 401,
    //    PaymentRequired = 402,
    //    Forbidden = 403,
    //    NotFound = 404,
    //    MethodNotAllowed = 405,
    //    NotAcceptable = 406,
    //    ProxyAuthenticationRequired = 407,
    //    RequestTimeout = 408,
    //    Conflict = 409,
    //    Gone = 410,
    //    LengthRequired = 411,
    //    PreconditionFailed = 412,
    //    RequestEntityTooLarge = 413,
    //    RequestUriTooLong = 414,
    //    UnsupportedMediaType = 415,
    //    RequestedRangeNotSatisfiable = 416,
    //    ExpectationFailed = 417,
    //    UpgradeRequired = 426,
    //    InternalServerError = 500,
    //    NotImplemented = 501,
    //    BadGateway = 502,
    //    ServiceUnavailable = 503,
    //    GatewayTimeout = 504,
    //    HttpVersionNotSupported = 505
    //}
}