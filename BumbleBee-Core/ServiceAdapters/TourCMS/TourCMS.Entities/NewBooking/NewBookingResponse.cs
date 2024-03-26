using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.NewBooking
{
    [XmlRoot(ElementName = "response")]
    public class NewBookingResponse
    {
        [XmlElement(ElementName = "request")]
        public string Request { get; set; }

        [XmlElement(ElementName = "error")]
        public string Error { get; set; }

        [XmlElement(ElementName = "booking")]
        public ResponseBooking Booking { get; set; }
    }

    public class ResponseBooking
    {
        [XmlElement(ElementName ="booking_id")]
        public Int64 BookingId { get; set; }
        [XmlElement(ElementName ="channel_id")]
        public Int64 ChannelId { get; set; }
        [XmlElement(ElementName ="account_id")]
        public Int64 AccountId { get; set; }
        [XmlElement(ElementName ="booking_engine_url")]
        public string BookingEngineUrl { get; set; }
        [XmlElement(ElementName ="available_component_count")]
        public Int64 AvailableComponentCount { get; set; }
        [XmlElement(ElementName ="unavailable_component_count")]
        public Int64 UnavailableComponentCount { get; set; }
        [XmlElement(ElementName ="balance_owed_by")]
        public string BalanceOwedBy { get; set; }
        [XmlElement(ElementName ="sale_currency")]
        public string SaleCurrency { get; set; }
        [XmlElement(ElementName ="sales_revenue")]
        public decimal SalesRevenue { get; set; }
        [XmlElement(ElementName ="sales_revenue_display")]
        public string SalesRevenueDisplay { get; set; }
        [XmlElement(ElementName ="sales_revenue_due_now")]
        public decimal SalesRevenueDueNow { get; set; }
        [XmlElement(ElementName ="sales_revenue_due_now_display")]
        public string SalesRevenueDueNowDisplay { get; set; }
        [XmlElement(ElementName ="sales_price_due_ever")]
        public decimal SalesPriceDueEver { get; set; }
        [XmlElement(ElementName ="sales_price_due_ever_display")]
        public string SalesPriceDueEverDisplay { get; set; } 
        [XmlElement(ElementName ="commission")]
        public decimal Commission { get; set; }
        [XmlElement(ElementName ="commission_tax")]
        public decimal CommissionTax { get; set; }
        [XmlElement(ElementName ="commission_currency")]
        public string CommissionCurrency { get; set; }
        [XmlElement(ElementName ="commission_display")]
        public string CommissionDisplay { get; set; }
        [XmlElement(ElementName ="commission_tax_display")]
        public string CommissionTaxDisplay { get; set; }
        [XmlElement(ElementName ="lead_customer_id")]
        public Int64 LeadCustomerId { get; set; }
        [XmlElement(ElementName = "customers")]
        public ResponseBookingCustomers Customers { get; set; }
        [XmlElement(ElementName ="hold_time_seconds")]
        public decimal HoldTimeSeconds { get; set; }
      }

    public  class ResponseBookingCustomers
    {
        [XmlElement(ElementName ="customer")]
        public  List<ResponseBookingCustomer> Customer { get; set; }
    }

    public class ResponseBookingCustomer
    {
        [XmlElement(ElementName = "customer_id")]
        public Int64 CustomerId { get; set; }
    }
}