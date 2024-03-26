using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.CancelReservationResponse
{
    public class CancelReservationResponse
    {
        [JsonProperty(PropertyName = "api_version")]
        public string ApiVersion { get; set; }
        [JsonProperty(PropertyName = "data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }
        [JsonProperty(PropertyName = "reservation")]
        public Reservation Reservation { get; set; }
    }

    public class Reservation
    {
        [JsonProperty(PropertyName = "reservation_distributor_id")]
        public string ReservationDistributorId { get; set; }
        [JsonProperty(PropertyName = "reservation_distributor_name")]
        public string Reservation_distributor_name { get; set; }
        [JsonProperty(PropertyName = "reservation_reference")]
        public string ReservationReference { get; set; }
        [JsonProperty(PropertyName = "reservation_external_reference")]
        public string ReservationExternalReference { get; set; }
        [JsonProperty(PropertyName = "reservation_valid_until")]
        public DateTime Reservation_valid_until { get; set; }
        [JsonProperty(PropertyName = "reservation_details")]
        public List<ReservationDetails> ReservationDetails { get; set; }
        [JsonProperty(PropertyName = "reservation_pricing")]
        public ReservationPricing Reservation_pricing { get; set; }
        [JsonProperty(PropertyName = "reservation_payments")]
        public List<ReservationPayments> Reservation_payments { get; set; }
    }

    public class ReservationPricing
    {
        [JsonProperty(PropertyName = "price_subtotal")]
        public string Price_subtotal { get; set; }
        [JsonProperty(PropertyName = "price_total")]
        public string Price_total { get; set; }
    }

    public class ReservationDetails
    {
        [JsonProperty(PropertyName = "booking_external_reference")]
        public string Booking_external_reference { get; set; }
        [JsonProperty(PropertyName = "booking_status")]
        public string Booking_status { get; set; }
        [JsonProperty(PropertyName = "booking_travel_date")]
        public DateTime Booking_travel_date { get; set; }
        [JsonProperty(PropertyName = "booking_language")]
        public string Booking_language { get; set; }
        [JsonProperty(PropertyName = "booking_pricing")]
        public BookingPricing Booking_pricing { get; set; }
        [JsonProperty(PropertyName = "booking_created")]
        public DateTime Booking_created { get; set; }
        [JsonProperty(PropertyName = "booking_modified")]
        public DateTime Booking_modified { get; set; }
        [JsonProperty(PropertyName = "booking_cancelled")]
        public DateTime Booking_cancelled { get; set; }
        [JsonProperty(PropertyName = "product_id")]
        public string Product_id { get; set; }
        [JsonProperty(PropertyName = "product_availability_id")]
        public string Product_availability_id { get; set; }
        [JsonProperty(PropertyName = "product_availability_from_date_time")]
        public DateTime Product_availability_from_date_time { get; set; }
        [JsonProperty(PropertyName = "product_availability_to_date_time")]
        public DateTime Product_availability_to_date_time { get; set; }
        [JsonProperty(PropertyName = "product_availability_capacity_id")]
        public string Product_availability_capacity_id { get; set; }
        [JsonProperty(PropertyName = "product_title")]
        public string Product_title { get; set; }
        [JsonProperty(PropertyName = "product_supplier_id")]
        public string Product_supplier_id { get; set; }
        [JsonProperty(PropertyName = "product_supplier_name")]
        public string Product_supplier_name { get; set; }
        [JsonProperty(PropertyName = "product_entry_notes")]
        public string Product_entry_notes { get; set; }
        [JsonProperty(PropertyName = "product_cancellation_allowed")]
        public bool Product_cancellation_allowed { get; set; }
        [JsonProperty(PropertyName = "product_admission_type")]
        public string Product_admission_type { get; set; }
        [JsonProperty(PropertyName = "product_currency_code")]
        public string Product_currency_code { get; set; }
        [JsonProperty(PropertyName = "product_type_details")]
        public List<ProductTypeDetails> Product_type_details { get; set; }
        [JsonProperty(PropertyName = "booking_reservation_reference")]
        public string BookingReservationReference { get; set; }
        [JsonProperty(PropertyName = "booking_reservation_valid_until")]
        public DateTime Booking_reservation_valid_until { get; set; }
    }

    public class BookingPricing
    {
        [JsonProperty(PropertyName = "price_subtotal")]
        public string Price_subtotal { get; set; }
        [JsonProperty(PropertyName = "price_taxes")]
        public List<PriceTaxes> Price_taxes { get; set; }
        [JsonProperty(PropertyName = "price_fees")]
        public List<PriceFees> Price_fees { get; set; }
        [JsonProperty(PropertyName = "price_total")]
        public string Price_total { get; set; }
    }

    public class PriceTaxes
    {
        [JsonProperty(PropertyName = "tax_id")]
        public string Tax_id { get; set; }
        [JsonProperty(PropertyName = "tax_amount")]
        public string Tax_amount { get; set; }
    }

    public class PriceFees
    {
        [JsonProperty(PropertyName = "fee_type")]
        public string Fee_type { get; set; }
        [JsonProperty(PropertyName = "fee_amount")]
        public string Fee_amount { get; set; }
        [JsonProperty(PropertyName = "fee_tax_amount")]
        public string Fee_tax_amount { get; set; }
    }

    public class ProductTypeDetails
    {
        [JsonProperty(PropertyName = "product_type")]
        public string Product_type { get; set; }
        [JsonProperty(PropertyName = "product_type_id")]
        public string Product_type_id { get; set; }
        [JsonProperty(PropertyName = "product_type_label")]
        public string Product_type_label { get; set; }
        [JsonProperty(PropertyName = "product_type_class")]
        public string Product_type_class { get; set; }
        [JsonProperty(PropertyName = "product_type_age_from")]
        public int Product_type_age_from { get; set; }
        [JsonProperty(PropertyName = "product_type_age_to")]
        public int Product_type_age_to { get; set; }
        [JsonProperty(PropertyName = "product_type_count")]
        public int Product_type_count { get; set; }
        [JsonProperty(PropertyName = "product_type_pax")]
        public int Product_type_pax { get; set; }
        [JsonProperty(PropertyName = "product_type_capacity")]
        public int Product_type_capacity { get; set; }
        [JsonProperty(PropertyName = "product_type_pricing")]
        public ProductTypePricing Product_type_pricing { get; set; }
    }

    public class ProductTypePricing
    {
        [JsonProperty(PropertyName = "price_subtotal")]
        public string Price_subtotal { get; set; }
        [JsonProperty(PropertyName = "price_taxes")]
        public List<PriceTaxes1> Price_taxes { get; set; }
        [JsonProperty(PropertyName = "price_fees")]
        public List<PriceFees1> Price_fees { get; set; }
        [JsonProperty(PropertyName = "price_total")]
        public string Price_total { get; set; }
    }

    public class PriceTaxes1
    {
        [JsonProperty(PropertyName = "tax_id")]
        public string Tax_id { get; set; }
        [JsonProperty(PropertyName = "tax_amount")]
        public string Tax_amount { get; set; }
    }

    public class PriceFees1
    {
        [JsonProperty(PropertyName = "fee_type")]
        public string Fee_type { get; set; }
        [JsonProperty(PropertyName = "fee_amount")]
        public string Fee_amount { get; set; }
        [JsonProperty(PropertyName = "fee_tax_amount")]
        public string Fee_tax_amount { get; set; }
        [JsonProperty(PropertyName = "fee_percentage")]
        public string Fee_percentage { get; set; }
    }

    public class ReservationPayments
    {
        [JsonProperty(PropertyName = "payment_status")]
        public string Payment_status { get; set; }
    }

}
