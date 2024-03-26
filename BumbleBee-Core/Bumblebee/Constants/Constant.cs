namespace WebAPI.Constants
{
    public class Constant
    {
        public const string Subscribed = "subscribed";
        public const string error = "error";
        public const string invalidemail = "invalidemail";
        public const string AlreadyExist = "alreadyexist";

        public const string SubscribedMessage = "Newsletter Subscribed successfully";
        public const string AlreadyExistMessage = "Newsletter already exists";
        public const string ErrorMessage = "Failed to subscribe the newsletter";
        public const string DefaultCurrency = "GBP";
        public const string DefaultLanguage = "en";

        public const string dd_MMM_yyyy = "dd_MMM_yyyy";
        public const string Availabilties = "Availabilties";
        public const string StorageConnectionString = "StorageConnectionString";
        public const string RowKey = "RowKey";
        public const string BookingGuid = "BookingGuid";
        public const string InvalidCastErrorMessage = "Not able to cast ProductOption in ActivityOption";
        public const string DecimalPrefix = "D_";
        public const char PipeSeparator = '|';
        public const string EN = "EN";
        public const string AppliedPriceOffersTable = "AppliedPriceOffers";
        public const string BookingRequest = "BookingRequest";

        public const string IP2LocationCoordinatesDB = "IP2LocationCoordinatesDB";
        public const string IP2LocationCoordinatesLicense = "IP2LocationCoordinatesLicense";
        public const string FirstName = "firstName";
        public const string LastName = "lastName";
        public const string Email = "email";
        public const string Phonenumber = "phonenumber";
        public const string QuestionType = "MAIN_CONTACT_DETAILS";
        public const string AdultFirstName = "AdultFirstName";
        public const string AdultLastName = "AdultLastName";
        public const string ChildFirstName = "ChildFirstName";
        public const string ChildLastName = "ChildLastName";
        public const int AdultAge = 30;
        public const string WebAPIBaseUrl = "WebAPIBaseUrl";
        
        //Xml related constants
        public const string AvailabilitiesEntityConfig = "AvailabilitiesEntity.config";

        public const string AvailabilityEntities = "AvailabilityEntities";
        public const string Name = "name";

        // TransferOptions related
        public const string AirlineNameId = "airlineName";

        public const string FlightNumberId = "flightNumber";
        public const string TransferTimeId = "transferTime";
        public const string HotelNameId = "hotelName";
        public const string HotelAddressId = "hotelAddress";
        public const string PostCodeId = "postCode";
        public const string MobileId = "mobile";
        public const string OriginId = "origin";
        public const string DestinationId = "destination";
        public const string GreetingNameId = "greetingName";

        public const string PostCode = "Post/Zip Code";
        public const string Destination = "Destination";
        public const string AirlineName = "Airline/Train Name";
        public const string FlightNumber = "Flight Number";
        public const string GreetingName = "Greeting Board Details";
        public const string HotelAddress = "Hotel Address";
        public const string HotelName = "Hotel Name";
        public const string Mobile = "Cell/Mobile Number";
        public const string Origin = "Origin";
        public const string TransferTime = "Arrival Time";

        // Google Maps Start
        public const string GoogleMapsOrder = "GoogleMapsOrder";

        public const string UserId = "UserId";
        public const string OrderId = "OrderId";
        public const long MicroUnit = 1000000;

        public const string PriceAndAvailabilityDumping = "PriceAndAvailabilityDumping";
        // Google Maps End

        //Cancellation Constants Start
        public const string RefundAmountRemark = "Entered refund amount is greater than selling price !";

        public const string CancellationSucceed = "Cancellation Succeed";
        public const string CancellationFailed = "Cancellation Failed";
        public const string BookingDetailNotPresentRemark = "Booked option details are not present!";
        public const string UserDataNotPresentRemark = "User data is not present!";
        public const string CancellationPolicyNotPresentRemark = "Cancellation Policy details are not present!";
        public const string NonCancellableProductRemark = "Product is non-cancellable";
        public const string ProductAlreadyCancelledRemark = "This product has already been cancelled !";
        //Cancellation Constants End
        public const string BookingCallBackRequest = "BookingCallBackRequest";
        public const string BookingCallBackChallenge = "Adyen Challenge Initiated";
        public const string BookingCallBackStart = "start";
        public const string BookingCallBackCompleted = "completed";
        public const string BookingCallBackError = "error";

        public const string DuplicateCallWaitTime = "DuplicateCall_WaitTime";
        public const string DuplicateCallWaitTimeLoop = "DuplicateCall_WaitTimeLoop";
        public const string ReceiveLinkExpired = "This link is expired, Please contact customer support.";
        public const string ReceiveNotValidLink = "This is not a valid link, please check and retry.";
        public const string ReceiveNotValidNow = "We have already received amount against this link, it isn't valid now.";
        public const string ReceiveSomeThingWentWrong = "Something went wrong, please try later!";
        public const string ReceiveAmendmentExpire = "amendmentid expired";
        public const string ReceiveInvalidAmendmentId = "invalid amendmentid";
        public const string ReceiveAlreadyReceived = "already received";
        public const string ReceiveNotValidLinkRetry = "This is not a valid link, please check and retry.";
        public const string UserEnteredValueAndPolicy = "User entered refund amount not match with the cancellation policy amount.";
        public const string success = "success";
        public const string MoulinRougeNotSpported = "MoulinRouge Products are not supported.";

        //Error Messages Start
        public const string DataReferenceId = "Data for AvailabilityReferenceId ";
        public const string Token = "Token ";
        public const string IsNotAvailable = "is not available. ";
        public const string HasExpired = "has expired.Please re-do the check availability to get new AvailabilityReferenceId.";
        public const string NoAffiliateData = "No Affiliate Data.";
        public const string ActivityNoData = "No Data return from Cosmos and Database";
        public const string NoBookingeData = "Booking Data is Null";
        //Error Messages End

    }
}