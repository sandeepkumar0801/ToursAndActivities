using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.Redemption
{
    [XmlRoot("response")]
    public class RedemptionResponse
    {
        [XmlElement("request")]
        public string Request { get; set; }

        [XmlElement("error")]
        public string Error { get; set; }

        [XmlElement("total_bookings_count")]
        public int TotalBookingsCount { get; set; }

        [XmlElement("bookings")]
        public BookingList Bookings { get; set; }
    }

    public class BookingList
    {
        [XmlElement("booking")]
        public List<RedemptionBooking> Bookings { get; set; }
    }

    public class RedemptionBooking
    {
        [XmlElement("booking_id")]
        public int BookingId { get; set; }

        [XmlElement("channel_id")]
        public int ChannelId { get; set; }

        [XmlElement("account_id")]
        public int AccountId { get; set; }

        [XmlElement("made_date_time")]
        public string MadeDateTime { get; set; }

        [XmlElement("made_username")]
        public string MadeUsername { get; set; }

        [XmlElement("made_type")]
        public string MadeType { get; set; }

        [XmlElement("made_name")]
        public string MadeName { get; set; }

        [XmlElement("start_date")]
        public string StartDate { get; set; }

        [XmlElement("end_date")]
        public string EndDate { get; set; }

        [XmlElement("booking_name")]
        public string BookingName { get; set; }

        [XmlElement("booking_name_custom")]
        public string BookingNameCustom { get; set; }

        [XmlElement("status")]
        public int Status { get; set; }

        [XmlElement("status_text")]
        public string StatusText { get; set; }

        [XmlElement("voucher_url")]
        public string VoucherUrl { get; set; }

        [XmlElement("barcode_data")]
        public string BarcodeData { get; set; }

        [XmlElement("cancel_reason")]
        public int CancelReason { get; set; }

        [XmlElement("cancel_text")]
        public string CancelText { get; set; }

        [XmlElement("cancel_date_time")]
        public string CancelDateTime { get; set; }

        [XmlElement("final_check")]
        public int FinalCheck { get; set; }

        [XmlElement("lead_customer_id")]
        public int LeadCustomerId { get; set; }

        [XmlElement("lead_customer_name")]
        public string LeadCustomerName { get; set; }

        [XmlElement("lead_customer_firstname")]
        public string LeadCustomerFirstName { get; set; }

        [XmlElement("lead_customer_surname")]
        public string LeadCustomerSurname { get; set; }

        [XmlElement("lead_customer_country")]
        public string LeadCustomerCountry { get; set; }

        [XmlElement("lead_customer_city")]
        public string LeadCustomerCity { get; set; }

        [XmlElement("lead_customer_postcode")]
        public string LeadCustomerPostcode { get; set; }

        [XmlElement("lead_customer_address")]
        public string LeadCustomerAddress { get; set; }

        [XmlElement("lead_customer_email")]
        public string LeadCustomerEmail { get; set; }

        [XmlElement("lead_customer_tel_home")]
        public string LeadCustomerTelHome { get; set; }

        [XmlElement("lead_customer_tel_mobile")]
        public string LeadCustomerTelMobile { get; set; }

        [XmlElement("lead_customer_contact_note")]
        public string LeadCustomerContactNote { get; set; }

        [XmlElement("lead_customer_agent_ref")]
        public string LeadCustomerAgentRef { get; set; }

        [XmlElement("lead_customer_travelling")]
        public int LeadCustomerTravelling { get; set; }

        [XmlElement("customer_count")]
        public int CustomerCount { get; set; }

        [XmlElement("sale_currency")]
        public string SaleCurrency { get; set; }

        [XmlElement("sales_revenue")]
        public decimal SalesRevenue { get; set; }

        [XmlElement("sales_revenue_display")]
        public string SalesRevenueDisplay { get; set; }

        [XmlElement("deposit")]
        public decimal Deposit { get; set; }

        [XmlElement("deposit_display")]
        public string DepositDisplay { get; set; }

        [XmlElement("agent_type")]
        public string AgentType { get; set; }

        [XmlElement("agent_ref")]
        public string AgentRef { get; set; }

        [XmlElement("agent_ref_components")]
        public string AgentRefComponents { get; set; }

        [XmlElement("tracking_miscid")]
        public int TrackingMiscId { get; set; }

        [XmlElement("commission")]
        public decimal Commission { get; set; }

        [XmlElement("commission_tax")]
        public decimal CommissionTax { get; set; }

        [XmlElement("commission_currency")]
        public string CommissionCurrency { get; set; }

        [XmlElement("commission_display")]
        public string CommissionDisplay { get; set; }

        [XmlElement("commission_tax_display")]
        public string CommissionTaxDisplay { get; set; }

        [XmlElement("booking_has_net_price")]
        public int BookingHasNetPrice { get; set; }

        [XmlElement("payment_status")]
        public int PaymentStatus { get; set; }

        [XmlElement("payment_status_text")]
        public string PaymentStatusText { get; set; }

        [XmlElement("balance_owed_by")]
        public string BalanceOwedBy { get; set; }

        [XmlElement("balance")]
        public decimal Balance { get; set; }

        [XmlElement("balance_display")]
        public string BalanceDisplay { get; set; }

        [XmlElement("balance_due")]
        public string BalanceDue { get; set; }

        [XmlElement("customer_special_request")]
        public string CustomerSpecialRequest { get; set; }

        [XmlElement("customers_agecat_breakdown")]
        public string CustomersAgecatBreakdown { get; set; }

        [XmlElement("components")]
        public Components BookingComponents { get; set; }
    }

    public class Components
    {
        [XmlElement("component")]
        public List<Component> ComponentList { get; set; }
    }

    public class Component
    {
        [XmlElement("component_id")]
        public int ComponentId { get; set; }

        [XmlElement("product_id")]
        public int ProductId { get; set; }

        [XmlElement("linked_component_id")]
        public int LinkedComponentId { get; set; }

        [XmlElement("date_id")]
        public int DateId { get; set; }

        [XmlElement("date_type")]
        public string DateType { get; set; }

        [XmlElement("product_code")]
        public string ProductCode { get; set; }

        [XmlElement("date_code")]
        public string DateCode { get; set; }

        [XmlElement("start_date")]
        public string StartDate { get; set; }

        [XmlElement("end_date")]
        public string EndDate { get; set; }

        [XmlElement("local_payment")]
        public decimal LocalPayment { get; set; }

        [XmlElement("customer_payment")]
        public decimal CustomerPayment { get; set; }

        [XmlElement("start_time")]
        public string StartTime { get; set; }

        [XmlElement("end_time")]
        public string EndTime { get; set; }

        [XmlElement("start_time_utcseconds")]
        public string StartTimeUtcSeconds { get; set; }

        [XmlElement("end_time_utcseconds")]
        public string EndTimeUtcSeconds { get; set; }

        [XmlElement("component_name")]
        public string ComponentName { get; set; }

        [XmlElement("product_note")]
        public string ProductNote { get; set; }

        [XmlElement("component_note")]
        public string ComponentNote { get; set; }

        [XmlElement("rate_breakdown")]
        public string RateBreakdown { get; set; }

        [XmlElement("rate_description")]
        public string RateDescription { get; set; }

        [XmlElement("supplier_note")]
        public string SupplierNote { get; set; }

        [XmlElement("sale_quantity")]
        public int SaleQuantity { get; set; }

        [XmlElement("sale_quantity_rule")]
        public string SaleQuantityRule { get; set; }

        [XmlElement("sale_tax_percentage")]
        public decimal SaleTaxPercentage { get; set; }

        [XmlElement("sale_tax_inclusive")]
        public int SaleTaxInclusive { get; set; }

        [XmlElement("sale_currency")]
        public string SaleCurrency { get; set; }

        [XmlElement("sale_price")]
        public decimal SalePrice { get; set; }

        [XmlElement("tax_total")]
        public decimal TaxTotal { get; set; }

        [XmlElement("sale_price_inc_tax_total")]
        public decimal SalePriceIncTaxTotal { get; set; }

        [XmlElement("sale_exchange_rate")]
        public decimal SaleExchangeRate { get; set; }

        [XmlElement("currency_base")]
        public string CurrencyBase { get; set; }

        [XmlElement("sale_price_base")]
        public decimal SalePriceBase { get; set; }

        [XmlElement("tax_total_base")]
        public decimal TaxTotalBase { get; set; }

        [XmlElement("sale_price_inc_tax_total_base")]
        public decimal SalePriceIncTaxTotalBase { get; set; }

        [XmlElement("voucher_redemption_status")]
        public int VoucherRedemptionStatus { get; set; }

        [XmlElement("voucher_redemption_username")]
        public string VoucherRedemptionUsername { get; set; }
    }


}
