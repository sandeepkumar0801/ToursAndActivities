using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
    public class B2C_BookingRequest
    {
        /// <summary>
        /// Currency 3 Character Iso Code.
        /// Example "GBP".
        /// </summary>
        [JsonIgnore]
        public string? CurrencyIsoCode { get; set; }

        /// <summary>
        /// Affiliate GUID, It will be set using Identity claims from auth token.
        /// Example "58C11104-34E6-47BA-926D-E89E4242B962"
        /// </summary>
        //[JsonIgnore]
        public string? AffiliateId { get; set; }

        /// <summary>
        /// Email id, Voucher will be sent to this email id.
        /// Example test@test.com
        /// </summary>
        [JsonIgnore]
        public string? UserEmail { get; set; }

        /// <summary>
        ///Session Token GUID, helps to  track respective availability and other details required for booking and link all call in case of any error tracing.
        /// Example "600d9995-a1d7-45f5-b870-f5d66e0b7d9a"
        /// </summary>
        //[JsonIgnore]
        public string? TokenId { get; set; }

        /// <summary>
        /// 2 Characters Language Iso Code.
        /// Example "EN".
        /// </summary>
        public string? LanguageCode { get; set; }

        /// <summary>
        /// User Phone Number.
        /// Example 1234567890
        /// </summary>
        [JsonIgnore]
        public string? UserPhoneNumber { get; set; }

        /// <summary>
        /// Customer Address,
        /// Example
        /// {
        ///     "Address": "Stobinian, 53 Waggon Road ",
        ///     "Town": "Brightons, Falkirk",
        ///     "PostCode": "FK2 0EL",
        ///     "CountryIsoCode": "GB",
        ///     "CountryName": "United Kingdom",
        ///     "StateOrProvince": "UK"
        ///}
        /// </summary>
        [JsonIgnore]
        public B2C_Customeraddress? CustomerAddress { get; set; }

        [Required]
        public List<B2C_Selectedproduct?> SelectedProducts { get; set; }

        public string? ExternalReferenceNumber { get; set; }
    }

    public class B2C_Customeraddress
    {
        /// <summary>
        /// Customer Address -> Address,
        /// Example
        /// {
        ///     "Address": "Stobinian, 53 Waggon Road ",
        /// }
        /// </summary>
        [Required]
        public string Address { get; set; }

        /// <summary>
        /// Customer Address Town,
        /// Example
        /// {
        ///     "Town": "Brightons, Falkirk",
        /// }
        /// </summary>
        [Required]
        public string Town { get; set; }

        /// <summary>
        /// Customer Address PostCode,
        /// Example
        /// {
        ///     "PostCode": "FK2 0EL",
        /// }
        /// </summary>
        [Required]
        public string PostCode { get; set; }

        /// <summary>
        /// Customer Address CountryIsoCode,
        /// Example
        /// {
        ///     "CountryIsoCode": "GB",
        /// }
        /// </summary>
        [Required]
        public string CountryIsoCode { get; set; }

        /// <summary>
        /// Customer Address CountryName,
        /// Example
        /// {
        ///     "CountryName": "United Kingdom",
        /// }
        /// </summary>
        [Required]
        public string CountryName { get; set; }

        /// <summary>
        /// Customer Address StateOrProvince,
        /// Example
        /// {
        ///     "StateOrProvince": "UK"
        /// }
        /// </summary>
        public string StateOrProvince { get; set; }
    }

    public class B2C_Selectedproduct
    {
        /// <summary>
        /// AvailabilityReferenceId GUID.
        /// This is unique GUID that identifies bookable option and all other info like its api type, price, date , no of selected passenger etc.
        /// Using this system fetches all related information related to that bookable option so it can pass that to various components of system.
        /// Example "28919428-edef-49c8-aa18-5f58fce68bd2"
        /// </summary>
        [Required]
        public string AvailabilityReferenceId { get; set; }

        /// <summary>
        /// Passenger Details required for booking.
        /// Example
        /// {
        ///            "FirstName": "Nichola",
        ///            "LastName": "Speirs",
        ///            "IsLeadPassenger": true,
        ///            "PassengerTypeId": 1
        /// }
        /// </summary>
        [Required]
        public List<B2C_Passengerdetail> PassengerDetails { get; set; }
        public string? PickupLocation { get; set; }
        public string? PickupLocationId { get; set; }
        public string? SpecialRequest { get; set; }

        public List<B2C_Question>? Questions { get; set; }

        public string? DropOffLocation { get; set; }
        public string? DropOffLocationId { get; set; }
    }

    public class B2C_Question
    {
        public string? Id { get; set; }
        public string? Label { get; set; }
        public string? Answer { get; set; }
        public bool? IsRequired { get; set; }
        public string? QuestionType { get; set; }
    }

    public class B2C_Passengerdetail
    {
        /// <summary>
        /// Customer First Name.
        /// Example
        /// {
        ///            "FirstName": "Nichola",
        /// }
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Customer Last Name.
        /// Example
        /// {
        ///            "LastName": "Nichola",
        /// }
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// IsLeadPassenger.
        /// At least 1 passenger details are required by all the supplier to book. So this is lead passenger.
        /// Some times only one passenger details is sufficient. in that case that passenger is lead passenger by default. But when to booking all passenger details are mandatory by supplier API then all passengers details is required. In such case this filed helps system to identify the lead passenger.
        /// Example
        /// {
        ///            "IsLeadPassenger": true,
        /// }
        /// </summary>
        [Required]
        public bool IsLeadPassenger { get; set; }

        /// <summary>
        /// Passenger type id can be one of following int.
        /// Example
        ///{
        ///    Undefined = 0,
        ///    Adult = 1,
        ///    Child = 2,
        ///    Youth = 8,
        ///    Infant = 9,
        ///    Senior = 10,
        ///    Student = 11,
        ///    Family = 12,
        ///    TwoAndUnder = 13,
        ///    Butterbeer = 14,
        ///    Concession = 15,
        ///    Single = 16,
        ///    Twin = 17,
        ///    Under30 = 18,
        ///    Family2 = 19,
        ///    Pax1To2 = 20,
        ///    Pax3To4 = 21,
        ///    Pax5To6 = 22,
        ///    Pax7To8 = 23,
        ///    Pax1 = 24,
        ///    Pax2 = 25,
        ///    Pax3 = 26,
        ///    Military = 27,
        ///    Family3 = 28
        ///}
        /// </summary>
        [Required]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Invalid PassengerTypeId")]
        public int PassengerTypeId { get; set; }
        public string? PassportNumber { get; set; }

        public string? PassportNationality { get; set; }

    }
}