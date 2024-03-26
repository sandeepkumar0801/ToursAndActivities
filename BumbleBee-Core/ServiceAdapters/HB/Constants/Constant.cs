namespace ServiceAdapters.HB.Constants
{
    public sealed class Constant
    {
        public const string ApiDumpPath = "ApiDumpPath";
        public const string ApiResponses = "api-responses";
        public const string Rq = "-RQ_";
        public const string Underscore = "_";
        public const string Rs = "-RS_";
        public const string Json = ".json";
        public const string DateInddMMyyyyhhmmssfff = "ddMMyyyyhhmmssfff";
        public const string EndpointBooking = "EndpointBooking";
        public const string Slash = "/";
        public const string CancellationFlagValue = "CANCELLATION";
        public const string SimulationCancellationFlagValue = "SIMULATION";
        public const string EndpointActivitySimple = "EndpointActivitySimple";
        public const string EndpointActivityFull = "EndpointActivityFull";
        public const string EndpointSearch = "EndpointSearch";
        public const string Child = "CHILD";
        public const string Adult = "ADULT";
        public const char Dash = '-';
        public const string RegionName = "Region Name not returned by api";
        public const string Ch = "CH";
        public const string Isango = "ISANGO";

        /// <summary>
        /// Date in yyyy-MM-dd format
        /// </summary>
        public const string DateInyyyyMMdd = "yyyy-MM-dd";

        public const string Localhost = "127.0.0.1";
        public const string Icanhazip = "http://icanhazip.com";
        public const string HBApitudeServiceURL = "HBApitudeServiceURL";
        public const string HBApitudeContentServiceURL = "HBApitudeContentServiceURL";
        public const string HBApitudeApiKey = "HBApitudeApiKey";
        public const string HBApitudeApi = "HBApitudeAPI";
        public const string HBApitudeApiSecret = "HBApitudeApiSecret";
        public const string ActivitiesDetailsURL = "activities/details?content=false";
        public const string CalendarURL = "activities/availability";
        public const string ContentMultiURL = "activities";
        public const string ActivitiesDetailsFullURL = "activities/details/full";
        public const string BookingConfirm = "bookings";
        public const string BookingQueryStringCancellation = "CANCELLATION";
        public const string BookingQueryStringCancellationPreview = "SIMULATION";

        /// <summary>
        /// 0 = es
        /// 1 = 102-12345678
        /// 2 = Value for CancellationFlag (SIMULATION means preview and CANCELLATION means full cancel)
        /// </summary>
        public const string BookingCancellation = "bookings/{0}/{1}?cancellationFlag={2}";

        public const string HeaderXSignature = "X-Signature";
        public const string HeaderApiKey = "Api-Key";
        public const string HeaderAccept = "Accept";
        public const string HeaderContentType = "Content-Type";
        public const string HeaderApplicationJson = "application/json";
        public const string RequestPrepare = "Request Prepare, it should change";

        public const string QuestionPaxName = "PAX NAME";
        public const string QuestionPhoneNumber = "PHONENUMBER";
        public const string QuestionPassport = "PASAPORTE";

        public const string QuestionGender = "GENDER";
        public const string AnswerGenderDefault = "MEN";

        public const string QuestionPaxTitle = "TITLE";
        public const string AnswerPaxTitleDefault = "Mr";
        public const string AnswerNotAvailable = "Not Available";
    }
}