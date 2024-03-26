using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Isango.Entities
{
    public class IsangoBookingData
    {
        public string AffiliateId { get; set; }
        public string TokenId { get; set; }
        public string BookingReferenceNumber { get; set; }
        public string CurrencyIsoCode { get; set; }
        public string LanguageCode { get; set; }
        public string CustomerEmailId { get; set; }
        public string AgentEmail { get; set; }
        public string AgentID { get; set; }
        public string PaymentMethodType { get; set; }
        public string UTMParameter { get; set; }
        public string BookingAgent { get; set; }
        public bool IsGuestUser { get; set; }
        public CustomerLocation CustomerLocation { get; set; }
        public List<TransactionDetail> TransactionDetail { get; set; }
        public CustomerAddress RegisteredCustomerDetail { get; set; }
        public List<BookedProduct> BookedProducts { get; set; }
        public string ExternalReferenceNumber { get; set; }
        public string VistaraMemberNumber { get; set; }
        public decimal? CVPoints { get; set; }
    }

    public class CustomerLocation
    {
        public string IPAddress { get; set; }
        public string CustomerOriginDestination { get; set; }
        public string CustomerOriginCountry { get; set; }
    }

    public class TransactionDetail
    {
        public bool Is3DSecure { get; set; }
        public string CardHolderName { get; set; }
        public string CardType { get; set; }
        public string PaymentGatewayId { get; set; }
        public string PaymentGatewayTransactionId { get; set; }
        public string AuthorizationCode { get; set; }
        public string TransactionFlowName { get; set; }
        public decimal TransactionAmount { get; set; }
        public string PaymentGateway { get; set; }
        public List<int> OptionIds { get; set; }

        public string AdyenMerchantAccout { get; set; }
    }

    public class CustomerAddress
    {
        public string CustomerPhoneNumber { get; set; }
        public bool IsGuestUser { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountryIsoCode { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
    }

    public class BookedProduct : ErrorList
    {
        public string AvailabilityReferenceId { get; set; }
        public int ServiceId { get; set; }
        public bool IsPaxDetailRequired { get; set; }
        public int OptionId { get; set; }
        public int BundleOptionId { get; set; }
        public string OptionName { get; set; }
        public DateTime CheckinDate { get; set; }
        public DateTime CheckoutDate { get; set; }
        public string OptionStatus { get; set; }
        public string PickUpLocation { get; set; }
        public string SpecialRequest { get; set; }
        public string UnitType { get; set; }
        public string ContractComment { get; set; }
        public OptionPrice OptionPrice { get; set; }
        public List<Discount> DiscountList { get; set; }
        public List<Sale> AppliedSales { get; set; }
        public List<PassengerDetail> PassengerDetails { get; set; }
        public List<CancellationPrice> CancellationPolicy { get; set; }
        public ApiExtraDetail APIExtraDetail { get; set; }
        public string Time { get; set; }

        [JsonIgnore]
        public string CountryCode { get; set; }
        public bool IsShowSupplierVoucher { get; set; }

        public string InvoicingCompany { get; set; }
    }

    public class OptionPrice
    {
        public string CostCurrency { get; set; }
        public decimal CostPrice { get; set; }
        public decimal GatePrice { get; set; }
        public decimal BasePrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal SellROE { get; set; }
        public decimal CostROE { get; set; }
        public int Quantity { get; set; }
        public decimal MultiSave { get; set; }
        public int PersonCharged { get; set; }
    }

    public class Discount
    {
        public decimal DiscountSellAmount { get; set; }
        public string DiscountCode { get; set; }
    }

    public class Sale
    {
        public decimal SaleAmount { get; set; }
        public string SaleType { get; set; }
        public int AppliedRuleId { get; set; }

        public decimal CostAmount { get; set; }

    }

    public class CancellationPolicyDetail
    {
        public DateTime CancellationDate { get; set; }
        public decimal CancellationAmount { get; set; }
        public bool IsPercent { get; set; }
    }

    public class PassengerDetail
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PassengerTypeId { get; set; }
        public bool IsLeadPassenger { get; set; }
        public PerPaxPrice PaxPrice { get; set; }
        public bool IndependablePax { get; set; }

        public int? Age { get; set; }
    }

    public class PerPaxPrice
    {
        public decimal CostPrice { get; set; }
        public decimal GatePrice { get; set; }
        public decimal BasePrice { get; set; }
        public decimal SellPrice { get; set; }
    }

    public class ApiExtraDetail
    {
        public string SupplieReferenceNumber { get; set; }
        public string SupplierLineNumber { get; set; }
        public string VATNo { get; set; }
        public string OfficeCode { get; set; }
        public string APIOptionName { get; set; }
        public string ProductCode { get; set; }
        public string PickUpId { get; set; }
        public string QRCode { get; set; }
        public string QRCodeType { get; set; }
        public string APIProductStatus { get; set; }
        public string ETicketGUIDs { get; set; }

        /// <summary>
        /// This is api cancellation policy in serialized string
        /// </summary>
        public string SupplierCancellationPolicy { get; set; }

        public string SupplierName { get; set; }
        public string ProviderInformation { get; set; }
        public bool IsQRCodePerPax { get; set; }
        public List<ApiTicketDetail> APITicketDetails { get; set; }

        [JsonIgnore]
        public List<Attachment> ConfirmedTicketAttachments { get; set; }
    }

    public class ApiTicketDetail
    {
        public string APIOrderId { get; set; }

        public string BarCode { get; set; }

        public string FiscalNumber { get; set; }

        public string SeatId { get; set; }

        public string CodeType { get; set; }
        public string CodeValue { get; set; }
        public string ResourceType { get; set; }
        public string ResouceRemoteUrl { get; set; }
        public string ResourceLocal { get; set; }
        public string PassengerType { get; set; }
        public bool IsResourceApply { get; set; }
        public string QRCodeType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string APIAgeGroupCode { get; set; }
    }
}