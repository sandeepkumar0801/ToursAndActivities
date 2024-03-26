using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.CreateBookingResponse
{
    public class CreateBookingResponse
    {
        [JsonProperty("api_version")]
        public string ApiVersion { get; set; }
        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }
        [JsonProperty("order")]
        public Order Order { get; set; }
    }

    public class Order
    {
        [JsonProperty("order_platform_id")]
        public string OrderPlatformId { get; set; }
        [JsonProperty("order_platform_name")]
        public string OrderPlatformName { get; set; }
        [JsonProperty("order_reseller_id")]
        public string OrderResellerId { get; set; }
        [JsonProperty("order_reseller_name")]
        public string OrderResellerName { get; set; }
        [JsonProperty("order_distributor_id")]
        public string OrderDistributorId { get; set; }
        [JsonProperty("order_distributor_name")]
        public string OrderDistributorName { get; set; }
        [JsonProperty("order_reference")]
        public string OrderReference { get; set; }
        [JsonProperty("order_external_reference")]
        public string OrderExternalReference { get; set; }
        [JsonProperty("order_status")]
        public string OrderStatus { get; set; }
        [JsonProperty("order_settlement_type")]
        public string OrderSettlementType { get; set; }
        [JsonProperty("order_language")]
        public string OrderLanguage { get; set; }
        [JsonProperty("order_version")]
        public int OrderVersion { get; set; }
        [JsonProperty("order_channel")]
        public string OrderChannel { get; set; }
        [JsonProperty("order_bookings")]
        public List<OrderBookings> OrderBookings { get; set; }
        [JsonProperty("order_pricing")]
        public OrderPricing OrderPricing { get; set; }
        [JsonProperty("order_payments")]
        public List<OrderPayments> OrderPayments { get; set; }
        [JsonProperty("order_contacts")]
        public List<OrderContacts> OrderContacts { get; set; }
        [JsonProperty("order_options")]
        public OrderOptions OrderOptions { get; set; }
        [JsonProperty("order_flags")]
        public List<OrderFlags> Orderflags { get; set; }
        [JsonProperty("order_custom_fields")]
        public List<OrderCustomFields> OrderCustomFields { get; set; }
        [JsonProperty("order_created")]
        public DateTime OrderCreated { get; set; }
        [JsonProperty("order_modified")]
        public DateTime OrderModified { get; set; }
        [JsonProperty("order_created_name")]
        public string OrdeCreatedName { get; set; }
        [JsonProperty("order_created_email")]
        public string OrderCreatedEmail { get; set; }
    }

    public class OrderPricing
    {
        [JsonProperty("price_subtotal")]
        public string PriceSubTotal { get; set; }
        [JsonProperty("price_taxes")]
        public List<PriceTaxes> PriceTaxes { get; set; }
        [JsonProperty("price_fees")]
        public List<PriceFees> PriceFees { get; set; }
        [JsonProperty("price_total")]
        public string PriceTotal { get; set; }
    }

    public class PriceTaxes
    {
        [JsonProperty("tax_id")]
        public string TaxId { get; set; }
        [JsonProperty("tax_amount")]
        public string TaxAmount { get; set; }
    }

    public class PriceFees
    {
        [JsonProperty("fee_type")]
        public string FeeType { get; set; }
        [JsonProperty("fee_amount")]
        public string FeeAmount { get; set; }
        [JsonProperty("fee_tax_amount")]
        public string FeeTaxAmount { get; set; }
    }

    public class OrderOptions
    {
        [JsonProperty("email_options")]
        public EmailOptions EmailOptions { get; set; }
    }

    public class EmailOptions
    {
        [JsonProperty("email_types")]
        public EmailTypes EmailTypes { get; set; }
        [JsonProperty("email_recipients")]
        public List<EmailRecipients> EmailRecipients { get; set; }
    }

    public class EmailTypes
    {
        [JsonProperty("send_tickets")]
        public bool SendTickets { get; set; }
        [JsonProperty("send_receipt")]
        public bool SendReceipt { get; set; }
    }

    public class EmailRecipients
    {
        [JsonProperty("recipients_name")]
        public string RecipientsName { get; set; }
        [JsonProperty("recipients_address")]
        public string RecipientsAddress { get; set; }
    }

    public class OrderBookings
    {
        [JsonProperty("booking_external_reference")]
        public string BookingExternalReference { get; set; }
        [JsonProperty("booking_status")]
        public string BookingStatus { get; set; }
        [JsonProperty("booking_version")]
        public int BookingVersion { get; set; }
        [JsonProperty("booking_travel_date")]
        public DateTime BookingTravelDate { get; set; }
        [JsonProperty("booking_language")]
        public string BookingLanguage { get; set; }
        [JsonProperty("booking_pricing")]
        public BookingPricing BookingPricing { get; set; }
        [JsonProperty("booking_created")]
        public DateTime BookingCreated { get; set; }
        [JsonProperty("booking_modified")]
        public DateTime BookingModified { get; set; }
        [JsonProperty("booking_confirmed")]
        public DateTime BookingConfirmed { get; set; }
        [JsonProperty("product_id")]
        public string ProductId { get; set; }
        [JsonProperty("product_availability_id")]
        public string ProductAvailabilityId { get; set; }
        [JsonProperty("product_availability_from_date_time")]
        public DateTime ProductAvailabilityFromDateTime { get; set; }
        [JsonProperty("product_availability_to_date_time")]
        public DateTime ProductAvailabilityToDateTime { get; set; }
        [JsonProperty("product_availability_capacity_id")]
        public string ProductAvailabilityCapacityId { get; set; }
        [JsonProperty("product_title")]
        public string ProductTitle { get; set; }
        [JsonProperty("product_supplier_id")]
        public string ProductSupplierId { get; set; }
        [JsonProperty("product_supplier_name")]
        public string ProductSupplierName { get; set; }
        [JsonProperty("product_entry_notes")]
        public string ProductEntryNotes { get; set; }
        [JsonProperty("product_cancellation_allowed")]
        public bool ProductCancellationAllowed { get; set; }
        [JsonProperty("product_admission_type")]
        public string ProductAdmissionType { get; set; }
        [JsonProperty("product_currency_code")]
        public string ProductCurrencyCode { get; set; }
        [JsonProperty("product_code")]
        public string ProductCode { get; set; }
        [JsonProperty("product_code_settings")]
        public ProductCodeSettings ProductCodeSettings { get; set; }
        [JsonProperty("product_type_details")]
        public List<ProductTypeDetails> ProductTypeDetails { get; set; }
        [JsonProperty("product_cancellation_policies")]
        public List<ProductCancellationPolicies> ProductCancellationPolicies { get; set; }
        [JsonProperty("booking_group_code")]
        public string BookingGroupCode { get; set; }
        [JsonProperty("booking_reference")]
        public string BookingReference { get; set; }
        [JsonProperty("booking_supplier_reference")]
        public string BookingSupplierReference { get; set; }
    }

    public class BookingPricing
    {
        [JsonProperty("price_subtotal")]
        public string PriceSubtotal { get; set; }
        [JsonProperty("price_taxes")]
        public List<PriceTaxes1> PriceTaxes { get; set; }
        [JsonProperty("price_fees")]
        public List<PriceFees1> PriceFees { get; set; }
        [JsonProperty("price_total")]
        public string PriceTotal { get; set; }
    }

    public class PriceTaxes1
    {
        [JsonProperty("tax_id")]
        public string TaxId { get; set; }
        [JsonProperty("tax_amount")]
        public string TaxAmount { get; set; }
    }

    public class PriceFees1
    {
        [JsonProperty("fee_type")]
        public string FeeType { get; set; }
        [JsonProperty("fee_amount")]
        public string FeeAmount { get; set; }
        [JsonProperty("fee_tax_amount")]
        public string FeeTaxAmount { get; set; }
    }

    public class ProductCodeSettings
    {
        [JsonProperty("product_code_format")]
        public string ProductCodeFormat { get; set; }
        [JsonProperty("product_code_source")]
        public string ProductCodeSource { get; set; }
        [JsonProperty("product_group_code")]
        public bool ProductGroupCode { get; set; }
        [JsonProperty("product_combi_code")]
        public bool ProductCombiCode { get; set; }
        [JsonProperty("product_voucher_settings")]
        public string ProductVoucherSettings { get; set; }
    }

    public class ProductTypeDetails
    {
        [JsonProperty("product_type")]
        public string ProductType { get; set; }
        [JsonProperty("product_type_id")]
        public string ProductTypeId { get; set; }
        [JsonProperty("product_type_label")]
        public string ProductTypeLabel { get; set; }
        [JsonProperty("product_type_class")]
        public string ProductTypeClass { get; set; }
        [JsonProperty("product_type_age_from")]
        public int ProductTypeAgeFrom { get; set; }
        [JsonProperty("product_type_age_to")]
        public int ProductTypeAgeTo { get; set; }
        [JsonProperty("product_type_count")]
        public int ProductTypeCount { get; set; }
        [JsonProperty("product_type_pax")]
        public int ProductTypePax { get; set; }
        [JsonProperty("product_type_capacity")]
        public int ProductTypeCapacity { get; set; }
        [JsonProperty("product_type_code")]
        public string ProductTypeCode { get; set; }
        [JsonProperty("product_type_transaction_id")]
        public string ProductTypeTransactionId { get; set; }
        [JsonProperty("product_type_status")]
        public string ProductTypeStatus { get; set; }
        [JsonProperty("product_type_redemption_status")]
        public string ProductTypeRedemptionStatus { get; set; }
        [JsonProperty("product_type_pricing")]
        public ProductTypePricing ProductTypePricing { get; set; }
    }

    public class ProductTypePricing
    {
        [JsonProperty("price_subtotal")]
        public string PriceSubtotal { get; set; }
        [JsonProperty("price_taxes")]
        public List<PriceTaxes2> PriceTaxes { get; set; }
        [JsonProperty("price_fees")]
        public List<PriceFees2> PriceFees { get; set; }
        [JsonProperty("price_total")]
        public string PriceTotal { get; set; }
    }

    public class PriceTaxes2
    {
        [JsonProperty("tax_id")]
        public string TaxId { get; set; }
        [JsonProperty("tax_amount")]
        public string TaxAmount { get; set; }
    }

    public class PriceFees2
    {
        [JsonProperty("fee_type")]
        public string FeeType { get; set; }
        [JsonProperty("fee_amount")]
        public string FeeAmount { get; set; }
        [JsonProperty("fee_tax_amount")]
        public string FeeTaxAmount { get; set; }
    }

    public class ProductCancellationPolicies
    {
        [JsonProperty("cancellation_type")]
        public string CancellationType { get; set; }
    }

    public class OrderPayments
    {
        [JsonProperty("payment_id")]
        public string PaymentId { get; set; }
        [JsonProperty("payment_original_id")]
        public string PaymentOriginalId { get; set; }
        [JsonProperty("payment_order_reference")]
        public string PaymentOrderReference { get; set; }
        [JsonProperty("payment_booking_references")]
        public List<string> PaymentBookingReferences { get; set; }
        [JsonProperty("payment_status")]
        public string PaymentStatus { get; set; }
        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }
        [JsonProperty("payment_scheme")]
        public string PaymentScheme { get; set; }
        [JsonProperty("payment_currency_code")]
        public string PaymentCurrencyCode { get; set; }
        [JsonProperty("payment_currency_rate")]
        public string PaymentCurrencyRate { get; set; }
        [JsonProperty("payment_currency_amount")]
        public string PaymentCurrencyAmount { get; set; }
        [JsonProperty("payment_amount")]
        public string PaymentAmount { get; set; }
        [JsonProperty("payment_total")]
        public string PaymentTotal { get; set; }
        [JsonProperty("payment_type")]
        public string PaymentType { get; set; }
        [JsonProperty("payment_recurring")]
        public bool PaymentRecurring { get; set; }
        [JsonProperty("payment_created")]
        public DateTime PaymentCreated { get; set; }
        [JsonProperty("payment_created_name")]
        public string PaymentCreatedName { get; set; }
        [JsonProperty("payment_created_email")]
        public string PaymentCreatedEmail { get; set; }
    }

    public class OrderContacts
    {
        [JsonProperty("contact_uid")]
        public string ContactUId { get; set; }
        [JsonProperty("contact_version")]
        public int ContactVersion { get; set; }
        [JsonProperty("contact_type")]
        public string ContactType { get; set; }
        [JsonProperty("contact_name")]
        public string ContactName { get; set; }
        [JsonProperty("contact_name_first")]
        public string ContactNameFirst { get; set; }
        [JsonProperty("contact_name_last")]
        public string ContactNameLast { get; set; }
        [JsonProperty("contact_email")]
        public string ContactEmail { get; set; }
        [JsonProperty("contact_created")]
        public DateTime ContactCreated { get; set; }
        [JsonProperty("contact_modified")]
        public DateTime ContactModified { get; set; }
    }

    public class OrderFlags
    {
        [JsonProperty("flag_id")]
        public string FlagId { get; set; }
        [JsonProperty("flag_type")]
        public string FlagType { get; set; }
        [JsonProperty("flag_name")]
        public string FlagName { get; set; }
        [JsonProperty("flag_value")]
        public string FlagValue { get; set; }
    }

    public class OrderCustomFields
    {
        [JsonProperty("custom_field_name")]
        public string CustomFieldName { get; set; }
        [JsonProperty("custom_field_value")]
        public string CustomFieldValue { get; set; }
    }
}