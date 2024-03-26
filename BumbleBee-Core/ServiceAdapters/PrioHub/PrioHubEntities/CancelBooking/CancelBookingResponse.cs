using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.CancelBookingResponse
{
    public class CancelBookingResponse
    {
        [JsonProperty("api_version")]
        public string Api_version { get; set; }
        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("error_reference")]
        public string ErrorReference { get; set; }
        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }
        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
        [JsonProperty("error_uri")]
        public string ErrorUri { get; set; }
        [JsonProperty("errors")]
        public List<string> Errors { get; set; }
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
        public string Order_platform_id { get; set; }
        [JsonProperty("order_platform_name")]
        public string Order_platform_name { get; set; }
        [JsonProperty("order_reseller_id")]
        public string Order_reseller_id { get; set; }
        [JsonProperty("order_reseller_name")]
        public string Order_reseller_name { get; set; }
        [JsonProperty("order_distributor_id")]
        public string Order_distributor_id { get; set; }
        [JsonProperty("order_distributor_name")]
        public string Order_distributor_name { get; set; }
        [JsonProperty("order_partner_id")]
        public string Order_partner_id { get; set; }
        [JsonProperty("order_partner_name")]
        public string Order_partner_name { get; set; }
        [JsonProperty("order_reference")]
        public string OrderReference { get; set; }
        [JsonProperty("order_external_reference")]
        public string OrderExternalReference { get; set; }
        [JsonProperty("order_status")]
        public string OrderStatus { get; set; }
        [JsonProperty("order_settlement_type")]
        public string Order_settlement_type { get; set; }
        [JsonProperty("order_channel")]
        public string Order_channel { get; set; }
        [JsonProperty("order_language")]
        public string Order_language { get; set; }
        [JsonProperty("order_version")]
        public int Order_version { get; set; }
        [JsonProperty("order_archived")]
        public bool Order_archived { get; set; }
        [JsonProperty("order_redacted")]
        public bool Order_redacted { get; set; }
        [JsonProperty("order_contacts")]
        public OrderContacts[] Order_contacts { get; set; }
        [JsonProperty("order_promocodes")]
        public OrderPromocodes[] Order_promocodes { get; set; }
        [JsonProperty("order_payments")]
        public OrderPayments[] Order_payments { get; set; }
        [JsonProperty("order_pricing")]
        public OrderPricing Order_pricing { get; set; }
        [JsonProperty("order_credit")]
        public OrderCredit Order_credit { get; set; }
        [JsonProperty("order_invoices")]
        public OrderInvoices[] Order_invoices { get; set; }
        [JsonProperty("order_options")]
        public OrderOptions Order_options { get; set; }
        [JsonProperty("order_flags")]
        public OrderFlags[] Order_flags { get; set; }
        [JsonProperty("order_custom_fields")]
        public OrderCustomFields[] Order_custom_fields { get; set; }
        [JsonProperty("order_notes")]
        public OrderNotes[] Order_notes { get; set; }
        [JsonProperty("order_activity_url")]
        public string Order_activity_url { get; set; }
        [JsonProperty("order_customer_url")]
        public string Order_customer_url { get; set; }
        [JsonProperty("order_created")]
        public DateTime Order_created { get; set; }
        [JsonProperty("order_modified")]
        public DateTime Order_modified { get; set; }
        [JsonProperty("order_confirmed")]
        public DateTime Order_confirmed { get; set; }
        [JsonProperty("order_invoiced")]
        public DateTime Order_invoiced { get; set; }
        [JsonProperty("order_cancellation_date_time")]
        public DateTime OrderCancellationDateTime { get; set; }
        [JsonProperty("order_created_name")]
        public string Order_created_name { get; set; }
        [JsonProperty("order_created_email")]
        public string Order_created_email { get; set; }
        [JsonProperty("order_view_type")]
        public string Order_view_type { get; set; }
        [JsonProperty("order_bookings")]
        public OrderBookings[] Order_bookings { get; set; }
    }

    public class OrderPricing
    {
        [JsonProperty("price_type")]
        public string Price_type { get; set; }
        [JsonProperty("price_currency_code")]
        public string Price_currency_code { get; set; }
        [JsonProperty("price_currency_rate")]
        public string Price_currency_rate { get; set; }
        [JsonProperty("price_subtotal")]
        public string Price_subtotal { get; set; }
        [JsonProperty("price_variations")]
        public PriceVariations[] Price_variations { get; set; }
        [JsonProperty("price_promocodes")]
        public PricePromocodes[] Price_promocodes { get; set; }
        [JsonProperty("price_taxes")]
        public PriceTaxes[] Price_taxes { get; set; }
        [JsonProperty("price_fees")]
        public PriceFees[] Price_fees { get; set; }
        [JsonProperty("price_margins")]
        public PriceMargins[] Price_margins { get; set; }
        [JsonProperty("price_total")]
        public string Price_total { get; set; }
    }

    public class PriceVariations
    {
        [JsonProperty("variation_label")]
        public string Variation_label { get; set; }
        [JsonProperty("variation_amount")]
        public string Variation_amount { get; set; }
        [JsonProperty("variation_type")]
        public string Variation_type { get; set; }
    }

    public class PricePromocodes
    {
        [JsonProperty("promo_code")]
        public string Promo_code { get; set; }
        [JsonProperty("promo_amount")]
        public string Promo_amount { get; set; }
    }

    public class PriceTaxes
    {
        [JsonProperty("tax_id")]
        public string Tax_id { get; set; }
        [JsonProperty("tax_name")]
        public string Tax_name { get; set; }
        [JsonProperty("tax_amount")]
        public string Tax_amount { get; set; }
    }

    public class PriceFees
    {
        [JsonProperty("fee_label")]
        public string Fee_label { get; set; }
        [JsonProperty("fee_type")]
        public string Fee_type { get; set; }
        [JsonProperty("fee_amount")]
        public string Fee_amount { get; set; }
        [JsonProperty("fee_percentage")]
        public string Fee_percentage { get; set; }
        [JsonProperty("fee_tax_id")]
        public string Fee_tax_id { get; set; }
        [JsonProperty("fee_tax_amount")]
        public string Fee_tax_amount { get; set; }
        [JsonProperty("fee_included")]
        public bool Fee_included { get; set; }
        [JsonProperty("fee_refundable")]
        public bool Fee_refundable { get; set; }
    }

    public class PriceMargins
    {
        [JsonProperty("margin_type")]
        public string Margin_type { get; set; }
        [JsonProperty("margin_amount")]
        public string Margin_amount { get; set; }
        [JsonProperty("margin_tax_id")]
        public string Margin_tax_id { get; set; }
        [JsonProperty("margin_tax_amount")]
        public string Margin_tax_amount { get; set; }
    }

    public class OrderCredit
    {
        [JsonProperty("credit_status")]
        public string Credit_status { get; set; }
        [JsonProperty("credit_total")]
        public string Credit_total { get; set; }
        [JsonProperty("credit_deposit")]
        public string Credit_deposit { get; set; }
        [JsonProperty("credit_blocked")]
        public string Credit_blocked { get; set; }
        [JsonProperty("credit_used")]
        public string Credit_used { get; set; }
        [JsonProperty("credit_remaining")]
        public string Credit_remaining { get; set; }
        [JsonProperty("credit_invoice_interval")]
        public string Credit_invoice_interval { get; set; }
        [JsonProperty("credit_invoice_settlement")]
        public string Credit_invoice_settlement { get; set; }
        [JsonProperty("credit_reset")]
        public DateTime Credit_reset { get; set; }
    }

    public class OrderOptions
    {
        [JsonProperty("email_options")]
        public EmailOptions Email_options { get; set; }
        [JsonProperty("price_on_voucher")]
        public bool Price_on_voucher { get; set; }
    }

    public class EmailOptions
    {
        [JsonProperty("email_types")]
        public EmailTypes Email_types { get; set; }
    }

    public class EmailTypes
    {
        [JsonProperty("send_tickets")]
        public bool Send_tickets { get; set; }
        [JsonProperty("send_receipt")]
        public bool Send_receipt { get; set; }
        [JsonProperty("send_marketing")]
        public bool Send_marketing { get; set; }
        [JsonProperty("send_offers")]
        public bool Send_offers { get; set; }
        [JsonProperty("send_notification")]
        public bool Send_notification { get; set; }
    }

    public class OrderContacts
    {
        [JsonProperty("contact_uid")]
        public string Contact_uid { get; set; }
        [JsonProperty("contact_external_uid")]
        public string Contact_external_uid { get; set; }
        [JsonProperty("contact_version")]
        public int Contact_version { get; set; }
        [JsonProperty("contact_number")]
        public string Contact_number { get; set; }
        [JsonProperty("contact_type")]
        public string Contact_type { get; set; }
        [JsonProperty("contact_title")]
        public string Contact_title { get; set; }
        [JsonProperty("contact_name_first")]
        public string Contact_name_first { get; set; }
        [JsonProperty("contact_name_last")]
        public string Contact_name_last { get; set; }
        [JsonProperty("contact_email")]
        public string Contact_email { get; set; }
        [JsonProperty("contact_phone")]
        public string Contact_phone { get; set; }
        [JsonProperty("contact_mobile")]
        public string Contact_mobile { get; set; }
        [JsonProperty("contact_language")]
        public string Contact_language { get; set; }
        [JsonProperty("contact_nationality")]
        public string Contact_nationality { get; set; }
        [JsonProperty("contact_flight_number")]
        public string Contact_flight_number { get; set; }
        [JsonProperty("contact_loyalty_number")]
        public string Contact_loyalty_number { get; set; }
        [JsonProperty("contact_birth_place")]
        public string Contact_birth_place { get; set; }
        [JsonProperty("contact_birth_date")]
        public string Contact_birth_date { get; set; }
        [JsonProperty("contact_passport")]
        public string Contact_passport { get; set; }
        [JsonProperty("contact_gender")]
        public string Contact_gender { get; set; }
        [JsonProperty("contact_age")]
        public int Contact_age { get; set; }
        [JsonProperty("contact_room_number")]
        public string Contact_room_number { get; set; }
        [JsonProperty("contact_website")]
        public string Contact_website { get; set; }
        [JsonProperty("contact_classification")]
        public string Contact_classification { get; set; }
        [JsonProperty("contact_address")]
        public ContactAddress Contact_address { get; set; }
        [JsonProperty("contact_notes")]
        public ContactNotes[] Contact_notes { get; set; }
        [JsonProperty("contact_custom_fields")]
        public ContactCustomFields[] Contact_custom_fields { get; set; }
        [JsonProperty("contact_created")]
        public DateTime Contact_created { get; set; }
        [JsonProperty("contact_modified")]
        public DateTime Contact_modified { get; set; }
    }

    public class ContactAddress
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("addition")]
        public string Addition { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("postal_code")]
        public string Postal_code { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("country_code")]
        public string Country_code { get; set; }
        [JsonProperty("place_id")]
        public string Place_id { get; set; }
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
        [JsonProperty("notes")]
        public string Notes { get; set; }
    }

    public class ContactNotes
    {
        [JsonProperty("note_value")]
        public string Note_value { get; set; }
        [JsonProperty("note_type")]
        public string Note_type { get; set; }
        [JsonProperty("note_date")]
        public DateTime Note_date { get; set; }
        [JsonProperty("note_creator")]
        public string Note_creator { get; set; }
    }

    public class ContactCustomFields
    {
        [JsonProperty("custom_field_name")]
        public string Custom_field_name { get; set; }
        [JsonProperty("custom_field_value")]
        public string Custom_field_value { get; set; }
        [JsonProperty("custom_field_type")]
        public string Custom_field_type { get; set; }
    }

    public class OrderPromocodes
    {
        [JsonProperty("promo_title")]
        public string Promo_title { get; set; }
        [JsonProperty("promo_description")]
        public string Promo_description { get; set; }
        [JsonProperty("promo_code")]
        public string Promo_code { get; set; }
    }

    public class OrderPayments
    {
        [JsonProperty("payment_id")]
        public string Payment_id { get; set; }
        [JsonProperty("payment_original_id")]
        public string Payment_original_id { get; set; }
        [JsonProperty("payment_partner_id")]
        public string Payment_partner_id { get; set; }
        [JsonProperty("payment_shift_id")]
        public string Payment_shift_id { get; set; }
        [JsonProperty("payment_merchant_reference")]
        public string Payment_merchant_reference { get; set; }
        [JsonProperty("payment_external_reference")]
        public string Payment_external_reference { get; set; }
        [JsonProperty("payment_order_reference")]
        public string Payment_order_reference { get; set; }
        [JsonProperty("payment_booking_references")]
        public string[] Payment_booking_references { get; set; }
        [JsonProperty("payment_order_version")]
        public int Payment_order_version { get; set; }
        [JsonProperty("payment_status")]
        public string Payment_status { get; set; }
        [JsonProperty("payment_method")]
        public string Payment_method { get; set; }
        [JsonProperty("payment_scheme")]
        public string Payment_scheme { get; set; }
        [JsonProperty("payment_type")]
        public string Payment_type { get; set; }
        [JsonProperty("payment_link")]
        public string Payment_link { get; set; }
        [JsonProperty("payment_link_expires_at")]
        public DateTime Payment_link_expires_at { get; set; }
        [JsonProperty("payment_authorization_expires_at")]
        public DateTime Payment_authorization_expires_at { get; set; }
        [JsonProperty("payment_recurring")]
        public bool Payment_recurring { get; set; }
        [JsonProperty("payment_recurring_type")]
        public string Payment_recurring_type { get; set; }
        [JsonProperty("payment_class")]
        public string Payment_class { get; set; }
        [JsonProperty("payment_refund_type")]
        public string Payment_refund_type { get; set; }
        [JsonProperty("payment_refund_reason")]
        public string Payment_refund_reason { get; set; }
        [JsonProperty("payment_currency_code")]
        public string Payment_currency_code { get; set; }
        [JsonProperty("payment_currency_rate")]
        public string Payment_currency_rate { get; set; }
        [JsonProperty("payment_currency_amount")]
        public string Payment_currency_amount { get; set; }
        [JsonProperty("payment_amount")]
        public string Payment_amount { get; set; }
        [JsonProperty("payment_total")]
        public string Payment_total { get; set; }
        [JsonProperty("payment_gateway_details")]
        public PaymentGatewayDetails Payment_gateway_details { get; set; }
        [JsonProperty("payment_contact")]
        public PaymentContact Payment_contact { get; set; }
        [JsonProperty("payment_fees")]
        public PaymentFees[] Payment_fees { get; set; }
        [JsonProperty("payment_notes")]
        public PaymentNotes[] Payment_notes { get; set; }
        [JsonProperty("payment_created")]
        public DateTime Payment_created { get; set; }
        [JsonProperty("payment_created_name")]
        public string Payment_created_name { get; set; }
        [JsonProperty("payment_created_email")]
        public string Payment_created_email { get; set; }
    }

    public class PaymentGatewayDetails
    {
        [JsonProperty("payment_merchant_account_name")]
        public string Payment_merchant_account_name { get; set; }
        [JsonProperty("payment_service_provider_reference")]
        public string Payment_service_provider_reference { get; set; }
        [JsonProperty("payment_service_provider_original_reference")]
        public string Payment_service_provider_original_reference { get; set; }
        [JsonProperty("payment_gateway_type")]
        public string Payment_gateway_type { get; set; }
        [JsonProperty("payment_gateway_additional_values")]
        public PaymentGatewayAdditionalValues Payment_gateway_additional_values { get; set; }
    }

    public class PaymentGatewayAdditionalValues
    {
        [JsonProperty("psp_shopper_name")]
        public string Psp_shopper_name { get; set; }
        [JsonProperty("psp_shopper_cardnumber")]
        public string Psp_shopper_cardnumber { get; set; }
        [JsonProperty("psp_issuer_country")]
        public string Psp_issuer_country { get; set; }
        [JsonProperty("psp_fraudscore")]
        public string Psp_fraudscore { get; set; }
        [JsonProperty("psp_payment_status")]
        public string Psp_payment_status { get; set; }
    }

    public class PaymentContact
    {
        [JsonProperty("contact_uid")]
        public string Contact_uid { get; set; }
        [JsonProperty("contact_external_uid")]
        public string Contact_external_uid { get; set; }
        [JsonProperty("contact_version")]
        public int Contact_version { get; set; }
        [JsonProperty("contact_number")]
        public string Contact_number { get; set; }
        [JsonProperty("contact_type")]
        public string Contact_type { get; set; }
        [JsonProperty("contact_title")]
        public string Contact_title { get; set; }
        [JsonProperty("contact_name_first")]
        public string Contact_name_first { get; set; }
        [JsonProperty("contact_name_last")]
        public string Contact_name_last { get; set; }
        [JsonProperty("contact_email")]
        public string Contact_email { get; set; }
        [JsonProperty("contact_phone")]
        public string Contact_phone { get; set; }
        [JsonProperty("contact_mobile")]
        public string Contact_mobile { get; set; }
        [JsonProperty("contact_language")]
        public string Contact_language { get; set; }
        [JsonProperty("contact_nationality")]
        public string Contact_nationality { get; set; }
        [JsonProperty("contact_flight_number")]
        public string Contact_flight_number { get; set; }
        [JsonProperty("contact_loyalty_number")]
        public string Contact_loyalty_number { get; set; }
        [JsonProperty("contact_birth_place")]
        public string Contact_birth_place { get; set; }
        [JsonProperty("contact_birth_date")]
        public string Contact_birth_date { get; set; }
        [JsonProperty("contact_passport")]
        public string Contact_passport { get; set; }
        [JsonProperty("contact_gender")]
        public string Contact_gender { get; set; }
        [JsonProperty("contact_age")]
        public int Contact_age { get; set; }
        [JsonProperty("contact_room_number")]
        public string Contact_room_number { get; set; }
        [JsonProperty("contact_website")]
        public string Contact_website { get; set; }
        [JsonProperty("contact_classification")]
        public string Contact_classification { get; set; }
        [JsonProperty("contact_address")]
        public ContactAddress1 Contact_address { get; set; }
        [JsonProperty("contact_notes")]
        public ContactNotes1[] Contact_notes { get; set; }
        [JsonProperty("contact_custom_fields")]
        public ContactCustomFields1[] Contact_custom_fields { get; set; }
        [JsonProperty("contact_created")]
        public DateTime Contact_created { get; set; }
        [JsonProperty("contact_modified")]
        public DateTime Contact_modified { get; set; }
    }

    public class ContactAddress1
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("addition")]
        public string Addition { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("postal_code")]
        public string Postal_code { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("country_code")]
        public string Country_code { get; set; }
        [JsonProperty("place_id")]
        public string Place_id { get; set; }
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
        [JsonProperty("notes")]
        public string Notes { get; set; }
    }

    public class ContactNotes1
    {
        [JsonProperty("note_value")]
        public string Note_value { get; set; }
        [JsonProperty("note_type")]
        public string Note_type { get; set; }
        [JsonProperty("note_date")]
        public DateTime Note_date { get; set; }
        [JsonProperty("note_creator")]
        public string Note_creator { get; set; }
    }

    public class ContactCustomFields1
    {
        [JsonProperty("custom_field_name")]
        public string Custom_field_name { get; set; }
        [JsonProperty("custom_field_value")]
        public string Custom_field_value { get; set; }
        [JsonProperty("custom_field_type")]
        public string Custom_field_type { get; set; }
    }

    public class PaymentFees
    {
        [JsonProperty("fee_label")]
        public string Fee_label { get; set; }
        [JsonProperty("fee_amount")]
        public string Fee_amount { get; set; }
        [JsonProperty("fee_type")]
        public string Fee_type { get; set; }
    }

    public class PaymentNotes
    {
        [JsonProperty("note_value")]
        public string Note_value { get; set; }
        [JsonProperty("note_type")]
        public string Note_type { get; set; }
        [JsonProperty("note_date")]
        public DateTime Note_date { get; set; }
        [JsonProperty("note_creator")]
        public string Note_creator { get; set; }
    }

    public class OrderInvoices
    {
        [JsonProperty("invoice_id")]
        public string Invoice_id { get; set; }
        [JsonProperty("invoice_product_id")]
        public string Invoice_product_id { get; set; }
        [JsonProperty("invoice_product_quantity")]
        public int Invoice_product_quantity { get; set; }
        [JsonProperty("invoice_date")]
        public DateTime Invoice_date { get; set; }
        [JsonProperty("invoice_status")]
        public string Invoice_status { get; set; }
        [JsonProperty("invoice_type")]
        public string Invoice_type { get; set; }
    }

    public class OrderFlags
    {
        [JsonProperty("flag_id")]
        public string Flag_id { get; set; }
        [JsonProperty("flag_name")]
        public string Flag_name { get; set; }
        [JsonProperty("flag_type")]
        public string Flag_type { get; set; }
        [JsonProperty("flag_value_id")]
        public string Flag_value_id { get; set; }
        [JsonProperty("flag_value")]
        public string Flag_value { get; set; }
        [JsonProperty("flag_visible")]
        public bool Flag_visible { get; set; }
    }

    public class OrderCustomFields
    {
        [JsonProperty("custom_field_name")]
        public string Custom_field_name { get; set; }
        [JsonProperty("custom_field_value")]
        public string Custom_field_value { get; set; }
        [JsonProperty("custom_field_type")]
        public string Custom_field_type { get; set; }
    }

    public class OrderNotes
    {
        [JsonProperty("note_value")]
        public string Note_value { get; set; }
        [JsonProperty("note_type")]
        public string Note_type { get; set; }
        [JsonProperty("note_date")]
        public DateTime Note_date { get; set; }
        [JsonProperty("note_creator")]
        public string Note_creator { get; set; }
    }

    public class OrderBookings
    {
        [JsonProperty("booking_external_reference")]
        public string Booking_external_reference { get; set; }
        [JsonProperty("booking_status")]
        public string Booking_status { get; set; }
        [JsonProperty("booking_version")]
        public int Booking_version { get; set; }
        [JsonProperty("booking_voucher_released")]
        public bool Booking_voucher_released { get; set; }
        [JsonProperty("booking_travel_date")]
        public DateTime booking_travel_date { get; set; }
        [JsonProperty("booking_valid_until")]
        public DateTime Booking_valid_until { get; set; }
        [JsonProperty("booking_invoice_status")]
        public string Booking_invoice_status { get; set; }
        [JsonProperty("booking_language")]
        public string Booking_language { get; set; }
        [JsonProperty("booking_pricing")]
        public BookingPricing Booking_pricing { get; set; }
        [JsonProperty("booking_notes")]
        public BookingNotes[] Booking_notes { get; set; }
        [JsonProperty("booking_created")]
        public DateTime Booking_created { get; set; }
        [JsonProperty("booking_modified")]
        public DateTime Booking_modified { get; set; }
        [JsonProperty("booking_confirmed")]
        public DateTime Booking_confirmed { get; set; }
        [JsonProperty("booking_cancelled")]
        public DateTime Booking_cancelled { get; set; }
        [JsonProperty("product_id")]
        public string Product_id { get; set; }
        [JsonProperty("product_relation_id")]
        public string Product_relation_id { get; set; }
        [JsonProperty("product_pickup_point_id")]
        public string Product_pickup_point_id { get; set; }
        [JsonProperty("product_pickup_point")]
        public ProductPickupPoint Product_pickup_point { get; set; }
        [JsonProperty("product_availability_id")]
        public string Product_availability_id { get; set; }
        [JsonProperty("product_availability_from_date_time")]
        public DateTime Product_availability_from_date_time { get; set; }
        [JsonProperty("product_availability_to_date_time")]
        public DateTime Product_availability_to_date_time { get; set; }
        [JsonProperty("product_availability_capacity_id")]
        public string Product_availability_capacity_id { get; set; }
        [JsonProperty("product_availability_capacity_shared_id")]
        public string Product_availability_capacity_shared_id { get; set; }
        [JsonProperty("product_title")]
        public string Product_title { get; set; }
        [JsonProperty("product_supplier_id")]
        public string Product_supplier_id { get; set; }
        [JsonProperty("product_supplier_name")]
        public string Product_supplier_name { get; set; }
        [JsonProperty("product_supplier_admin_id")]
        public string Product_supplier_admin_id { get; set; }
        [JsonProperty("product_supplier_admin_name")]
        public string Product_supplier_admin_name { get; set; }
        [JsonProperty("product_market_admin_id")]
        public string Product_market_admin_id { get; set; }
        [JsonProperty("product_market_admin_name")]
        public string Product_market_admin_name { get; set; }
        [JsonProperty("product_source_id")]
        public string Product_source_id { get; set; }
        [JsonProperty("product_source_name")]
        public string Product_source_name { get; set; }
        [JsonProperty("product_entry_notes")]
        public string Product_entry_notes { get; set; }
        [JsonProperty("product_admission_type")]
        public string Product_admission_type { get; set; }
        [JsonProperty("product_currency_code")]
        public string Product_currency_code { get; set; }
        [JsonProperty("product_cancellation_allowed")]
        public bool Product_cancellation_allowed { get; set; }
        [JsonProperty("product_options")]
        public ProductOptions[] product_options { get; set; }
        [JsonProperty("product_combi_details")]
        public ProductCombiDetails[] Product_combi_details { get; set; }
        [JsonProperty("product_code")]
        public string Product_code { get; set; }
        [JsonProperty("product_code_settings")]
        public ProductCodeSettings Product_code_settings { get; set; }
        [JsonProperty("product_type_details")]
        public ProductTypeDetails1[] Product_type_details { get; set; }
        [JsonProperty("product_cancellation_policies")]
        public ProductCancellationPolicies[] Product_cancellation_policies { get; set; }
        [JsonProperty("booking_reference")]
        public string Booking_reference { get; set; }
        [JsonProperty("booking_supplier_reference")]
        public string Booking_supplier_reference { get; set; }
    }

    public class BookingPricing
    {
        [JsonProperty("price_type")]
        public string Price_type { get; set; }
        [JsonProperty("price_currency_code")]
        public string Price_currency_code { get; set; }
        [JsonProperty("price_currency_rate")]
        public string Price_currency_rate { get; set; }
        [JsonProperty("price_subtotal")]
        public string Price_subtotal { get; set; }
        [JsonProperty("price_variations")]
        public PriceVariations1[] Price_variations { get; set; }
        [JsonProperty("price_promocodes")]
        public PricePromocodes1[] Price_promocodes { get; set; }
        [JsonProperty("price_taxes")]
        public PriceTaxes1[] Price_taxes { get; set; }
        [JsonProperty("price_fees")]
        public PriceFees1[] Price_fees { get; set; }
        [JsonProperty("price_margins")]
        public PriceMargins1[] Price_margins { get; set; }
        [JsonProperty("price_total")]
        public string Price_total { get; set; }
    }

    public class PriceVariations1
    {
        [JsonProperty("variation_label")]
        public string Variation_label { get; set; }
        [JsonProperty("variation_amount")]
        public string Variation_amount { get; set; }
        [JsonProperty("variation_type")]
        public string Variation_type { get; set; }
    }

    public class PricePromocodes1
    {
        [JsonProperty("promo_code")]
        public string Promo_code { get; set; }
        [JsonProperty("promo_amount")]
        public string Promo_amount { get; set; }
    }

    public class PriceTaxes1
    {
        [JsonProperty("tax_id")]
        public string Tax_id { get; set; }
        [JsonProperty("tax_name")]
        public string Tax_name { get; set; }
        [JsonProperty("tax_amount")]
        public string Tax_amount { get; set; }
    }

    public class PriceFees1
    {
        [JsonProperty("fee_label")]
        public string Fee_label { get; set; }
        [JsonProperty("fee_type")]
        public string Fee_type { get; set; }
        [JsonProperty("fee_amount")]
        public string Fee_amount { get; set; }
        [JsonProperty("fee_percentage")]
        public string Fee_percentage { get; set; }
        [JsonProperty("fee_tax_id")]
        public string Fee_tax_id { get; set; }
        [JsonProperty("fee_tax_amount")]
        public string Fee_tax_amount { get; set; }
        [JsonProperty("fee_included")]
        public bool Fee_included { get; set; }
        [JsonProperty("fee_refundable")]
        public bool Fee_refundable { get; set; }
    }

    public class PriceMargins1
    {
        [JsonProperty("margin_type")]
        public string Margin_type { get; set; }
        [JsonProperty("margin_amount")]
        public string Margin_amount { get; set; }
        [JsonProperty("margin_tax_id")]
        public string Margin_tax_id { get; set; }
        [JsonProperty("margin_tax_amount")]
        public string Margin_tax_amount { get; set; }
    }

    public class ProductPickupPoint
    {
        [JsonProperty("pickup_point_id")]
        public string Pickup_point_id { get; set; }
        [JsonProperty("pickup_point_name")]
        public string Pickup_point_name { get; set; }
        [JsonProperty("pickup_point_description")]
        public string Pickup_point_description { get; set; }
        [JsonProperty("pickup_point_location")]
        public string Pickup_point_location { get; set; }
        [JsonProperty("pickup_point_time")]
        public string Pickup_point_time { get; set; }
        [JsonProperty("pickup_point_times")]
        public string[] Pickup_point_times { get; set; }
        [JsonProperty("pickup_point_duration")]
        public int Pickup_point_duration { get; set; }
        [JsonProperty("pickup_point_availability_dependency")]
        public bool Pickup_point_availability_dependency { get; set; }
    }

    public class ProductCodeSettings
    {
        [JsonProperty("product_code_format")]
        public string Product_code_format { get; set; }
        [JsonProperty("product_code_source")]
        public string Product_code_source { get; set; }
        [JsonProperty("product_group_code")]
        public bool Product_group_code { get; set; }
        [JsonProperty("product_combi_code")]
        public bool Product_combi_code { get; set; }
        [JsonProperty("product_voucher_settings")]
        public string Product_voucher_settings { get; set; }
        [JsonProperty("product_code_release_date")]
        public DateTime Product_code_release_date { get; set; }
        [JsonProperty("product_code_release_details")]
        public string[] Product_code_release_details { get; set; }
    }

    public class BookingNotes
    {
        [JsonProperty("note_value")]
        public string Note_value { get; set; }
        [JsonProperty("note_type")]
        public string Note_type { get; set; }
        [JsonProperty("note_date")]
        public DateTime Note_date { get; set; }
        [JsonProperty("note_creator")]
        public string Note_creator { get; set; }
    }

    public class ProductOptions
    {
        [JsonProperty("option_id")]
        public string Option_id { get; set; }
        [JsonProperty("option_value_text")]
        public string Option_value_text { get; set; }
        [JsonProperty("option_values")]
        public OptionValues[] Option_values { get; set; }
    }

    public class OptionValues
    {
        [JsonProperty("value_id")]
        public string Value_id { get; set; }
        [JsonProperty("value_name")]
        public string Value_name { get; set; }
        [JsonProperty("value_price")]
        public string Value_price { get; set; }
        [JsonProperty("value_price_tax_id")]
        public string Value_price_tax_id { get; set; }
        [JsonProperty("value_price_tax_amount")]
        public string Value_price_tax_amount { get; set; }
        [JsonProperty("value_product_type_id")]
        public string Value_product_type_id { get; set; }
        [JsonProperty("value_icon")]
        public string Value_icon { get; set; }
        [JsonProperty("value_count")]
        public int Value_count { get; set; }
    }

    public class ProductCombiDetails
    {
        [JsonProperty("product_parent_id")]
        public string Product_parent_id { get; set; }
        [JsonProperty("product_id")]
        public string Product_id { get; set; }
        [JsonProperty("product_title")]
        public string Product_title { get; set; }
        [JsonProperty("product_supplier_id")]
        public string Product_supplier_id { get; set; }
        [JsonProperty("product_supplier_name")]
        public string Product_supplier_name { get; set; }
        [JsonProperty("product_admission_type")]
        public string Product_admission_type { get; set; }
        [JsonProperty("product_currency_code")]
        public string Product_currency_code { get; set; }
        [JsonProperty("product_availability_id")]
        public string Product_availability_id { get; set; }
        [JsonProperty("product_availability_from_date_time")]
        public DateTime Product_availability_from_date_time { get; set; }
        [JsonProperty("product_availability_to_date_time")]
        public DateTime Product_availability_to_date_time { get; set; }
        [JsonProperty("product_availability_capacity_id")]
        public string Product_availability_capacity_id { get; set; }
        [JsonProperty("product_availability_capacity_shared_id")]
        public string Product_availability_capacity_shared_id { get; set; }
        [JsonProperty("booking_travel_date")]
        public DateTime Booking_travel_date { get; set; }
        [JsonProperty("booking_external_reference")]
        public string Booking_external_reference { get; set; }
        [JsonProperty("product_code")]
        public string Product_code { get; set; }
        [JsonProperty("product_code_settings")]
        public ProductCodeSettings1 Product_code_settings { get; set; }
        [JsonProperty("product_type_details")]
        public ProductTypeDetails[] Product_type_details { get; set; }
        [JsonProperty("booking_supplier_reference")]
        public string Booking_supplier_reference { get; set; }
    }

    public class ProductCodeSettings1
    {
        [JsonProperty("product_code_format")]
        public string Product_code_format { get; set; }
        [JsonProperty("product_code_source")]
        public string Product_code_source { get; set; }
        [JsonProperty("product_group_code")]
        public bool Product_group_code { get; set; }
        [JsonProperty("product_combi_code")]
        public bool Product_combi_code { get; set; }
        [JsonProperty("product_voucher_settings")]
        public string Product_voucher_settings { get; set; }
        [JsonProperty("product_code_release_date")]
        public DateTime Product_code_release_date { get; set; }
        [JsonProperty("product_code_release_details")]
        public string[] Product_code_release_details { get; set; }
    }

    public class ProductTypeDetails
    {
        [JsonProperty("product_type")]
        public string Product_type { get; set; }
        [JsonProperty("product_type_class")]
        public string Product_type_class { get; set; }
        [JsonProperty("product_type_id")]
        public string Product_type_id { get; set; }
        [JsonProperty("product_type_label")]
        public string Product_type_label { get; set; }
        [JsonProperty("product_type_age_from")]
        public int Product_type_age_from { get; set; }
        [JsonProperty("product_type_age_to")]
        public int Product_type_age_to { get; set; }
        [JsonProperty("product_type_count")]
        public int Product_type_count { get; set; }
        [JsonProperty("product_type_pax")]
        public int Product_type_pax { get; set; }
        [JsonProperty("product_type_capacity")]
        public int Product_type_capacity { get; set; }
        [JsonProperty("product_type_spots")]
        public object[] Product_type_spots { get; set; }
        [JsonProperty("product_type_code")]
        public string Product_type_code { get; set; }
        [JsonProperty("product_type_transaction_id")]
        public string Product_type_transaction_id { get; set; }
        [JsonProperty("product_type_status")]
        public string Product_type_status { get; set; }
        [JsonProperty("product_type_redemption_status")]
        public string Product_type_redemption_status { get; set; }
        [JsonProperty("product_type_redemption_date_time")]
        public DateTime Product_type_redemption_date_time { get; set; }
        [JsonProperty("product_type_pass")]
        public ProductTypePass Product_type_pass { get; set; }
    }

    public class ProductTypePass
    {
    }

    public class ProductTypeDetails1
    {
        [JsonProperty("product_type")]
        public string Product_type { get; set; }
        [JsonProperty("product_type_class")]
        public string Product_type_class { get; set; }
        [JsonProperty("product_type_id")]
        public string Product_type_id { get; set; }
        [JsonProperty("product_type_label")]
        public string Product_type_label { get; set; }
        [JsonProperty("product_type_age_from")]
        public int Product_type_age_from { get; set; }
        [JsonProperty("product_type_age_to")]
        public int Product_type_age_to { get; set; }
        [JsonProperty("product_type_count")]
        public int Product_type_count { get; set; }
        [JsonProperty("product_type_pax")]
        public int Product_type_pax { get; set; }
        [JsonProperty("product_type_capacity")]
        public int Product_type_capacity { get; set; }
        [JsonProperty("product_type_spots")]
        public ProductTypeSpots[] Product_type_spots { get; set; }
        [JsonProperty("product_type_code")]
        public string Product_type_code { get; set; }
        [JsonProperty("product_type_transaction_id")]
        public string Product_type_transaction_id { get; set; }
        [JsonProperty("product_type_status")]
        public string Product_type_status { get; set; }
        [JsonProperty("product_type_redemption_status")]
        public string Product_type_redemption_status { get; set; }
        [JsonProperty("product_type_redemption_date_time")]
        public DateTime Product_type_redemption_date_time { get; set; }
        [JsonProperty("product_type_pass")]
        public ProductTypePass1 Product_type_pass { get; set; }
        [JsonProperty("product_type_pricing")]
        public ProductTypePricing Product_type_pricing { get; set; }
    }

    public class ProductTypePass1
    {
        [JsonProperty("pass_duration")]
        public PassDuration Pass_duration { get; set; }
    }

    public class PassDuration
    {
        [JsonProperty("pass_duration_start")]
        public DateTime Pass_duration_start { get; set; }
        [JsonProperty("pass_duration_end")]
        public DateTime Pass_duration_end { get; set; }
        [JsonProperty("pass_duration_total")]
        public int Pass_duration_total { get; set; }
        [JsonProperty("pass_duration_remaining")]
        public int Pass_duration_remaining { get; set; }
    }

    public class ProductTypePricing
    {
        [JsonProperty("price_type")]
        public string Price_type { get; set; }
        [JsonProperty("price_currency_code")]
        public string Price_currency_code { get; set; }
        [JsonProperty("price_currency_rate")]
        public string Price_currency_rate { get; set; }
        [JsonProperty("price_subtotal")]
        public string Price_subtotal { get; set; }
        [JsonProperty("price_variations")]
        public PriceVariations2[] Price_variations { get; set; }
        [JsonProperty("price_promocodes")]
        public PricePromocodes2[] Price_promocodes { get; set; }
        [JsonProperty("price_taxes")]
        public PriceTaxes2[] Price_taxes { get; set; }
        [JsonProperty("price_fees")]
        public PriceFees2[] Price_fees { get; set; }
        [JsonProperty("price_margins")]
        public PriceMargins2[] Price_margins { get; set; }
        [JsonProperty("price_total")]
        public string Price_total { get; set; }
    }

    public class PriceVariations2
    {
        [JsonProperty("variation_label")]
        public string Variation_label { get; set; }
        [JsonProperty("variation_amount")]
        public string Variation_amount { get; set; }
        [JsonProperty("variation_type")]
        public string Variation_type { get; set; }
    }

    public class PricePromocodes2
    {
    }

    public class PriceTaxes2
    {
    }

    public class PriceFees2
    {
    }

    public class PriceMargins2
    {
    }

    public class ProductTypeSpots
    {
        [JsonProperty("spot_id")]
        public string Spot_id { get; set; }
        [JsonProperty("spot_name")]
        public string Spot_name { get; set; }
        [JsonProperty("spot_section")]
        public string Spot_section { get; set; }
        [JsonProperty("spot_row")]
        public string Spot_row { get; set; }
        [JsonProperty("spot_number")]
        public string Spot_number { get; set; }
        [JsonProperty("spot_type")]
        public string Spot_type { get; set; }
        [JsonProperty("spot_state")]
        public string Spot_state { get; set; }
    }

    public class ProductCancellationPolicies
    {
        [JsonProperty("cancellation_description")]
        public string Cancellation_description { get; set; }
        [JsonProperty("cancellation_type")]
        public string Cancellation_type { get; set; }
        [JsonProperty("cancellation_fee_threshold")]
        public int Cancellation_fee_threshold { get; set; }
        [JsonProperty("cancellation_fee_percentage")]
        public float Cancellation_fee_percentage { get; set; }
        [JsonProperty("cancellation_fee_amount")]
        public string Cancellation_fee_amount { get; set; }
    }
}