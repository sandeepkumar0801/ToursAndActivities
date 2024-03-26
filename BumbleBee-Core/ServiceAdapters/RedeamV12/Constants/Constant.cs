namespace ServiceAdapters.RedeamV12.Constants
{
#pragma warning disable S1118 // Utility classes should not have public constructors

    public class Constant
#pragma warning restore S1118 // Utility classes should not have public constructors
    {
        public const string BaseAddressV12 = "RedeamBaseAddressV12";
        public const string ApiKeyHeader = "X-Api-Key";
        public const string ApiSecretKeyHeader = "X-Api-Secret";
        public const string ApiKeyV12 = "RedeamApiKeyV12";
        public const string ApiSecretKeyV12 = "RedeamApiSecretKeyV12";
        public const string ApplicationMediaType = "application/json";
        public const string DateTimeStringFormat = "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'";
        public const string DateTimeStringFormatSingle = "yyyy-MM-dd";
        public const string TimeFormat = "hh:mm tt";
        public const string ErrorWhileAPIHit = "ErrorWhileAPIHit";
        public const string ReleasedStatus = "RELEASED";
        public const string CancelledStatus = "CANCELLED";
        public const string RedeamTimeout = "RedeamTimeout";

        public const string Adult = "ADULT";
        public const string Child = "CHILD";
        public const string Any = "ANY";
        public const string Infant = "INFANT";
        public const string Senior = "SENIOR";
        public const string Student = "STUDENT";
        public const string Unknown = "UNKNOWN";
        public const string Youth = "YOUTH";

        public const string ReservedType = "RESERVED";
        public const string FreeSaleType = "FREESALE";
        public const string PassType = "PASS";

        public const string CancellationPolicyBeforeTourStart = "Free cancellation up to 24 hours before the tour/activity. ";
        public const string CancellationPolicyNonRefundable = "Non-Refundable";

    }
}