using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.CommitBooking
{
    [XmlRoot(ElementName = "response")]
    public class CommitBookingResponse
    {
        [XmlElement(ElementName = "request")]
        public string Request { get; set; }
        [XmlElement(ElementName = "error")]
        public string Error { get; set; }
        [XmlElement(ElementName = "booking")]
        public ResponseBooking booking { get; set; }
    }


    public class ResponseBooking
    {
        [XmlElement(ElementName = "booking_id")]
        public Int64 BookingId { get; set; }
        [XmlElement(ElementName = "channel_id")]
        public Int64 ChannelId { get; set; }
        [XmlElement(ElementName = "account_id")]
        public Int64 AccountId { get; set; }
        [XmlElement(ElementName = "status")]
        public Int64 Status { get; set; }
        [XmlElement(ElementName = "status_text")]
        public string StatusText { get; set; }
        [XmlElement(ElementName = "voucher_url")]
        public string VoucherUrl { get; set; }
        [XmlElement(ElementName = "barcode_data")]
        public string BarcodeData { get; set; }

        [XmlElement(ElementName = "components")]
        public ResponseBookingComponent components { get; set; }
    }
    public class ResponseBookingComponent
    {
        [XmlElement(ElementName = "component")]
        public List<ResponseBookingComponentItem> component { get; set; }
    }

    public class ResponseBookingComponentItem
    {
        [XmlElement(ElementName = "component_id")]
        public Int64 ComponentId { get; set; }

        [XmlElement(ElementName = "barcode_symbology")]
        public string BarcodeSymbology { get; set; }

        [XmlElement(ElementName = "operator_reference")]
        public string OperatorReference { get; set; }

        [XmlElement(ElementName = "linked_component_id")]
        public Int64 LinkedComponentId { get; set; }

        [XmlElement(ElementName = "product_id")]
        public Int64 ProductId { get; set; }

        [XmlElement(ElementName = "product_code")]
        public string ProductCode { get; set; }

        [XmlElement(ElementName = "date_id")]
        public Int64 DateId { get; set; }

        [XmlElement(ElementName = "date_code")]
        public object DateCode { get; set; }

        [XmlElement(ElementName = "start_date")]
        public System.DateTime StartDate { get; set; }

        [XmlElement(ElementName = "end_date")]
        public System.DateTime EndDate { get; set; }

        [XmlElement(ElementName = "local_payment")]
        public Int64 LocalPayment { get; set; }

        [XmlElement(ElementName = "customer_payment")]
        public Int64 CustomerPayment { get; set; }

        [XmlElement(ElementName = "rate_breakdown")]
        public string RateBreakdown { get; set; }

        [XmlElement(ElementName = "rate_description")]
        public string RateDescription { get; set; }

        [XmlElement(ElementName = "start_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "end_time")]
        public string EndTime { get; set; }

        [XmlElement(ElementName = "date_type")]
        public string DateType { get; set; }

        [XmlElement(ElementName = "component_name")]
        public string ComponentName { get; set; }

        [XmlElement(ElementName = "sale_quantity_rule")]
        public string SaleQuantityRule { get; set; }

        [XmlElement(ElementName = "sale_quantity")]
        public Int64 SaleQuantity { get; set; }

        [XmlElement(ElementName = "tickets")]
        public TicketsTicket Tickets { get; set; }
    }
    public class TicketsTicket
    {
        [XmlElement(ElementName = "ticket")]
        public List<Ticket> Ticket { get; set; }
    }
    public class Ticket
    {
        [XmlElement(ElementName = "label")]
        public object Label;

        [XmlElement(ElementName = "value")]
        public string Value;
    }
}


