using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Isango.Entities.Booking.RequestModels
{
    public class CreateBookingRequest
    {
        public string? UTMParameter { get; set; }

        [Required]
        public string CurrencyIsoCode { get; set; }

        [Required]
        public string AffiliateId { get; set; }

        [Required]
        public string UserEmail { get; set; }

        public List<string>? DiscountCoupons { get; set; }

        [Required]
        public string TokenId { get; set; }

        public string? SessionId { get; set; }

        public string? LanguageCode { get; set; }

        //[Required]
        public bool IsGuestUser { get; set; }

        public string? UserLoginSource { get; set; }

        [Required]
        public string UserPhoneNumber { get; set; }

        public string? AgentEmailID { get; set; }
        public string? AgentID { get; set; }
        public string? BookingAgent { get; set; }
        public DateTime BookingTime { get; set; }
        public string? IPAddress { get; set; }
        public string? ActualIP { get; set; }
        public string? OriginCountry { get; set; }
        public string? OriginCity { get; set; }

        // [Required]
        public CustomerAddress? CustomerAddress { get; set; }

        [Required]
        public PaymentDetail PaymentDetail { get; set; }

        [Required]
        public List<SelectedProduct>? SelectedProducts { get; set; }

        public ClientDetail? ClientDetail { get; set; }

        public BrowserInfo? BrowserInfo { get; set; }

        public bool? IsReservation { get; set; }

        public string? BookingReferenceNumber { get; set; }

        public string? ExternalReferenceNumber { get; set; }

        public decimal? CVPoints { get; set; }

        public string?   VistaraMemberNumber { get; set; }

        [JsonIgnore]
        public string? TiquetsLanguageCode { get; set; }


    }

    public class BrowserInfo
    {
        public string? UserAgent { get; set; }
        public string? AcceptHeader { get; set; }
        public string? Language { get; set; }
        public string    ScreenHeight { get; set; }
        public string? ScreenWidth { get; set; }
        public string? ColorDepth { get; set; }
        public string? TimeZoneOffset { get; set; }
        public bool? JavaEnabled { get; set; }
    }

    public class PaymentDetail
    {
        //[Required]
        public string? UserFullName { get; set; }

        public Card? CardDetails { get; set; }
        public string? PaymentOption { get; set; }

        //Undefined = 0, WireCard = 1, Alternative = 2
        //[Required]
        public string? PaymentGateway { get; set; }

        public string? PaymentMethodType { get; set; }
    }

    public class Card
    {
        //[Required]
        public string? Number { get; set; }

        //[Required]
        public string? SecurityCode { get; set; }

        //[Required]
        public string? ExpiryMonth { get; set; }

        //[Required]
        public string? ExpiryYear { get; set; }

        //[Required]
        public string? Type { get; set; }
    }

    public class CustomerAddress
    {
        //[Required]
        public string? Address { get; set; }

        //[Required]
        public string? Town { get; set; }

        //[Required]
        public string? PostCode { get; set; }

        //[Required]
        //[RegularExpression(@"^[a-zA-Z]{2}$", ErrorMessage = "Invalid Country Iso Code.")]
        public string? CountryIsoCode { get; set; }

        //[Required]
        public string? CountryName { get; set; }

        public string? StateOrProvince { get; set; }
    }

    public class SelectedProduct
    {
        [Required]
        public string AvailabilityReferenceId { get; set; }

        [Required]
        public string CheckinDate { get; set; }

        public string? CheckoutDate { get; set; }
        public string PickupLocation { get; set; }
        public string? PickupLocationId { get; set; }
        public string? SpecialRequest { get; set; }

        [Required]
        public List<PassengerDetail> PassengerDetails { get; set; }

        public List<Question>? Questions { get; set; }

        public string? DropOffLocation { get; set; }
        public string? DropOffLocationId { get; set; }

        public bool IsSameGateBase { get; set; }
    }

    public class Question
    {
        public string? Id { get; set; }
        public string? Label { get; set; }
        public string? Answer { get; set; }
        public bool IsRequired { get; set; }
        public string? QuestionType { get; set; }
    }

    public class PassengerDetail
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public bool IsLeadPassenger { get; set; }

        //[Required]
        [JsonIgnore]
        public int AgeGroupId { get; set; }

        [Required] //mendatory but commented as currently required to support any of AgeGroupId or PassengerTypeId
        public int PassengerTypeId { get; set; }

        public int? Ages { get; set; }

        public string? PassportNumber { get; set; }

        public string? PassportNationality { get; set; }

        public string? AgeSupplier { get; set; }
    }
}