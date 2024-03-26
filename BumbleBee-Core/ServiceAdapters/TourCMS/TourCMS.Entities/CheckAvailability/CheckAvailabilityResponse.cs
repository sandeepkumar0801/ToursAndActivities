using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.CheckAvailabilityResponse
{
    [XmlRoot(ElementName = "response")]
    public class CheckAvailabilityResponse
    {
        [XmlElement(ElementName = "request")]
        public string Request { get; set; }

        [XmlElement(ElementName = "error")]
        public string Error { get; set; }

        [XmlElement(ElementName = "channel_id")]
        public int ChannelId { get; set; }

        [XmlElement(ElementName = "account_id")]
        public int AccountId { get; set; }

        [XmlElement(ElementName = "tour_id")]
        public int TourId { get; set; }

        [XmlElement(ElementName = "tour_name")]
        public string TourName { get; set; }

        [XmlElement(ElementName = "tour_name_long")]
        public string TourNameLong { get; set; }

        [XmlElement(ElementName = "component_key_valid_for")]
        public string ComponentKeyValidFor { get; set; }

        [XmlElement(ElementName = "sale_quantity_rule")]
        public string SaleQuantityRule { get; set; }

       [XmlElement(ElementName = "available_components")]
       public AvailableComponents AvailableComponents { get; set; }

    }

    public class AvailableComponents
    {
        [XmlElement(ElementName = "component")]
        public List<Component> Component { get; set; }
    }

    public class Component
    {
        [XmlElement(ElementName = "start_date")]
        public DateTime StartDate { get; set; }

        [XmlElement(ElementName = "end_date")]
        public DateTime EndDate { get; set; }

        [XmlElement(ElementName = "start_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "end_time")]
        public string EndTime { get; set; }


        [XmlElement(ElementName = "start_time_utcseconds")]
        public string StartTimeUtcseconds { get; set; }

        [XmlElement(ElementName = "end_time_utcseconds")]
        public string EndTimeUtcseconds { get; set; }

        [XmlElement(ElementName = "public_bookable")]
        public bool PublicBookable { get; set; }

        [XmlElement(ElementName = "date_code")]
        public string DateCode { get; set; }

        [XmlElement(ElementName = "sale_currency")]
        public string SaleCurrency { get; set; }

        [XmlElement(ElementName = "min_booking_size")]
        public int MinBookingSize { get; set; }

        [XmlElement(ElementName = "spaces_remaining")]
        public int SpacesRemaining { get; set; }

        [XmlElement(ElementName = "total_price")]
        public decimal TotalPrice { get; set; }

        [XmlElement(ElementName = "total_price_display")]
        public string TotalPriceDisplay { get; set; }

        [XmlElement(ElementName = "net_price")]
        public decimal NetPrice { get; set; }

        [XmlElement(ElementName = "note")]
        public string Note { get; set; }

        [XmlElement(ElementName = "special_offer_note")]
        public object SpecialOfferNote { get; set; }


        [XmlElement(ElementName = "component_key")]
        public string ComponentKey { get; set; }

        [XmlElement(ElementName = "supplier_note")]
        public string SupplierNote { get; set; }

        [XmlElement(ElementName = "date_id")]
        public int DateId { get; set; }

        [XmlElement(ElementName = "date_type")]
        public string DateType { get; set; }

        [XmlElement(ElementName = "options")]
        public ComponentOptions Options { get; set; }

        [XmlElement(ElementName = "price_breakdown")]
        public PriceBreakdown PriceBreakdown { get; set; }

        [XmlElement(ElementName = "pickup_on_request_key")]
        public string PickupOnRequestKey { get; set; }

        [XmlElement(ElementName = "pickup_points")]
        public PickupPoints PickupPoints { get; set; }

        [XmlElement(ElementName = "questions")]
        public Questions Questions { get; set; }
    }
    public class Questions
    {
        [XmlElement(ElementName = "q")]
        public List<Quest> Quest { get; set; }

    }
    public class PickupPoints
    {
        [XmlElement(ElementName = "pickup")]
        public List<Pickup> Pickup { get; set; }
        
    }
        public class PriceBreakdown
    {
        [XmlElement(ElementName = "price_row")]
        public List<PriceBreakdownPrice> PriceBreakdownPrice { get; set; }
    }

    public class ComponentOptions
    {
        [XmlElement(ElementName = "option")]
        public List<ComponentOptionsOption> ComponentOptionsOption { get; set; }
    }
    public class Quest
    {
        [XmlElement(ElementName = "question_key")]
        public string QuestionKey { get; set; }

        [XmlElement(ElementName = "question")]
        public string Question { get; set; }

        [XmlElement(ElementName = "explanation")]
        public string Explanation { get; set; }

        [XmlElement(ElementName = "placeholder")]
        public string Placeholder { get; set; }

        [XmlElement(ElementName = "question_internal")]
        public string QuestionInternal { get; set; }

        [XmlElement(ElementName = "repeat")]
        public string Repeat { get; set; }

        [XmlElement(ElementName = "repeat_type")]
        public string RepeatType { get; set; }

        [XmlElement(ElementName = "answer_type")]
        public string AnswerType { get; set; }

        [XmlElement(ElementName = "answer_mandatory")]
        public string AnswerMandatory { get; set; }
    }
    public class Pickup
    {
        [XmlElement(ElementName = "pickup_key")]
        public string PickupKey { get; set; }

        [XmlElement(ElementName = "time")]
        public string TimePickup { get; set; }

        [XmlElement(ElementName = "pickup_name")]
        public string PickUpName { get; set; }

        [XmlElement(ElementName = "pickup_id")]
        public string PickupId { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "address1")]
        public string Address1 { get; set; }

        [XmlElement(ElementName = "address2")]
        public string Address2 { get; set; }

        [XmlElement(ElementName = "postcode")]
        public string PostCode { get; set; }

        [XmlElement(ElementName = "geocode")]
        public string GeoCode { get; set; }
    }
    public class ComponentOptionsOption
    {
        [XmlElement(ElementName = "option_id")]
        public string OptionId { get; set; }

        [XmlElement(ElementName = "group")]
        public byte Group { get; set; }

        [XmlElement(ElementName = "group_title")]
        public string GroupTitle { get; set; }

        [XmlElement(ElementName = "local_payment")]
        public byte LocalPayment { get; set; }

        [XmlElement(ElementName = "sale_quantity_rule")]
        public string SaleQantityRule { get; set; }

        [XmlElement(ElementName = "duration_rule")]
        public string DurationRule { get; set; }

        [XmlElement(ElementName = "option_group_radio")]
        public string OptionGroupRadio { get; set; }

        [XmlElement(ElementName = "option_name")]
        public string OptionName { get; set; }

        [XmlElement(ElementName = "extension")]
        public object Extension { get; set; }

        [XmlElement(ElementName = "per_text")]
        public string PerText { get; set; }

        [XmlElement(ElementName = "quantities_and_prices")]
        public QuantitiesAndPrices QuantitiesAndPrices;
    }

    public class QuantitiesAndPrices
    {
        [XmlElement(ElementName = "selection")]
        public List<QuantitiesAndPricesSelection> Selection;
    }
    public class QuantitiesAndPricesSelection
    {
        [XmlElement(ElementName = "quantity")]
        public byte Quantity;

        [XmlElement(ElementName = "price")]
        public decimal Price;

        [XmlElement(ElementName = "price_display")]
        public string PriceDisplay;

        [XmlElement(ElementName = "option_sale_currency")]
        public string OptionSaleCurrency;

        [XmlElement(ElementName = "component_key")]
        public string ComponentKey;
    }
    public class PriceBreakdownPrice
    {

        [XmlAttribute("rate-id")]
        public string RateId;

        [XmlAttribute("label")]
        public string Label;

        [XmlAttribute("sale-currency")]
        public string SaleCurrency;

        [XmlAttribute("price")]
        public decimal Price;

        [XmlAttribute("price-display")]
        public string PriceDisplay;

        [XmlText()]
        public string Value;
    }
}