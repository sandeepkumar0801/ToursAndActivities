using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.ReservationResponse
{
    public class ReservationResponse
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
        [JsonProperty("reservation")]
        public Reservation Reservation { get; set; }
    }

    public class Reservation
    {
        [JsonProperty("reservation_distributor_id")]
        public string ReservationDistributorId { get; set; }
        [JsonProperty("reservation_distributor_name")]
        public string ReservationDistributorName { get; set; }
        [JsonProperty("reservation_reference")]
        public string ReservationReference { get; set; }
        [JsonProperty("reservation_external_reference")]
        public string ReservationExternalReference { get; set; }
        [JsonProperty("reservation_valid_until")]
        public DateTime ReservationValidUntil { get; set; }
        [JsonProperty("reservation_details")]
        public List<ReservationDetailsResponse> ReservationDetails { get; set; }
        [JsonProperty("reservation_pricing")]
        public ReservationPricing ReservationPricing { get; set; }
        [JsonProperty("reservation_payments")]
        public List<ReservationPayments> ReservationPayments { get; set; }
    }

    public class ReservationPricing
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

    public class ReservationDetailsResponse
    {
        [JsonProperty("booking_external_reference")]
        public string BookingExternalReference { get; set; }
        [JsonProperty("booking_status")]
        public string BookingStatus { get; set; }
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
        [JsonProperty("product_id")]
        public string ProductId { get; set; }
        [JsonProperty("product_availability_id")]
        public string ProductAvailabilityId { get; set; }
        [JsonProperty("product_availability_from_date_time")]
        public DateTime ProductAvailabilityFromDateTime { get; set; }
        [JsonProperty("product_availability_to_date_time")]
        public DateTime ProductAvailabilityToDateTime { get; set; }
        [JsonProperty("product_availability_capacity_id")]
        public string ProductAvailabilityCapacityid { get; set; }
        [JsonProperty("product_title")]
        public string ProductTitle { get; set; }
        [JsonProperty("product_supplier_id")]
        public string ProductSupplierId { get; set; }
        [JsonProperty("product_supplier_name")]
        public string ProductSupplierName { get; set; }
        [JsonProperty("product_entry_notes")]
        public string ProductentryNotes { get; set; }
        [JsonProperty("product_cancellation_allowed")]
        public bool ProductCancellationAllowed { get; set; }
        [JsonProperty("product_admission_type")]
        public string ProductAdmissionType { get; set; }
        [JsonProperty("product_currency_code")]
        public string ProductCurrencyCode { get; set; }
        [JsonProperty("product_type_details")]
        public List<ProductTypeDetails> ProductTypeDetails { get; set; }
        [JsonProperty("booking_reservation_reference")]
        public string BookingReservationReference { get; set; }
        [JsonProperty("booking_reservation_valid_until")]
        public DateTime BookingReservationValidUntil { get; set; }
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

    public class ReservationPayments
    {
        [JsonProperty("payment_status")]
        public string PaymentStatus { get; set; }
    }
}