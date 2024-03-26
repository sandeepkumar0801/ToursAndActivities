using ServiceAdapters.Ventrata.Ventrata.Entities;

namespace ServiceAdapters.Ventrata.Constants
{
    public sealed class Constants
    {
        public const string AuthorizationHeaderKey = "Authorization";
        public const string OctoEnvKey = "Octo-Env";
        public const string OctoTestKey = "test";
        public const string OctoLiveKey = "live";
        public const string OctoPricingKey = "octo/pricing";
        public const string OctoContentKey = "octo/content";
        public const string OctoPickupsKey = "octo/pickups";
        public const string OctoOffersKey = "octo/offers";
        public const string OctoQuestionsKey = "octo/questions";
        public const string OctoPackagesKey = "octo/packages";
        public const string TestModeKey = "X-TestMode";
        public const string DateFormatyyyyMMdd = "yyyyMMdd";
        public const string Availability = "availability";
        public const string Products = "products";
        public const string BookingReservation = "bookings";
        public const string Confirm = "confirm";
        public const string HttpDeleteMethodType = "DELETE";
        public const string StatusFailed = "Failed";
        public const string StatusSuccess = "Success";
        public const string StatusReservation = "Reservation";
        public const string Bearer = "Bearer";
        public const string VentrataAPIUrl = "VentrataAPIUrl";
        public const string VentrataAPIBearerToken = "VentrataApiBearerToken";
        public const string IsRequestInTestMode = "IsRequestInTestMode";
        public const string VentrataAdapterClassName = "Ventrata.VentrataAdapter";
        public const string MaxParallelThreadCount = "MaxParallelThreadCount";

        public const string VoucherDeliveryMethod = "VOUCHER";
        public const string TicketDeliveryMethod = "TICKET";
        public const string QRCodeDeliveryFormat = "QRCODE";
        public const string PDFDeliveryFormat = "PDF_URL";
        public const string LinkTypeForPDF = "Link";
        public const string StringTypeForQRCode = "String";

        public const string Child = "child";
        public const string Infant = "infant";
        public const string Senior = "senior";
        public const string Adult = "adult";
        public const string Family = "family";
        public const string Student = "student";
        public const string Military = "military";
        public const string Youth = "youth";

        public const string APIName = "Ventrata";
        public const string DateFormat = "yyyy-MM-dd";
        public const string DateTimeWithSecondFormat = "yyyy-MM-dd'T'HH:mm:sszzz";
        public static IDictionary<string, PassengerType> Mapper_PassengerType = new Dictionary<string, PassengerType>
        {
            {"adult", PassengerType.adult},
            {"child", PassengerType.child},
            {"youth", PassengerType.youth},
            {"infant", PassengerType.infant},
            {"senior", PassengerType.senior},
            {"family", PassengerType.family},
            {"student", PassengerType.student},
            {"military", PassengerType.military}
        };

        public static IDictionary<string, Isango.Entities.Enums.PassengerType> CrossMapper_PassengerType_Isango_VentrataString = new Dictionary<string, Isango.Entities.Enums.PassengerType>
        {
            {"adult", Isango.Entities.Enums.PassengerType.Adult},
            {"child", Isango.Entities.Enums.PassengerType.Child},
            {"youth", Isango.Entities.Enums.PassengerType.Youth},
            {"infant", Isango.Entities.Enums.PassengerType.Infant},
            {"senior", Isango.Entities.Enums.PassengerType.Senior},
            {"family", Isango.Entities.Enums.PassengerType.Family},
            {"student", Isango.Entities.Enums.PassengerType.Student},
            {"military", Isango.Entities.Enums.PassengerType.Military}
        };

        public const string OctoCapabilities = "Octo-Capabilities";
    }
}
