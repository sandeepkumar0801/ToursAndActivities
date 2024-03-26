using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.ChannelListResponse
{
    [XmlRoot(ElementName = "response")]
    public class TourShowResponse
    {
        [XmlElement(ElementName = "request")]
        public string Request { get; set; }

        [XmlElement(ElementName = "error")]
        public string Error { get; set; }

        [XmlElement(ElementName = "tour")]
        public List<TourListShow> TourListShow { get; set; }
    }

    public class TourListShow
    {
        [XmlElement(ElementName = "channel_id")]
        public string ChannelId { get; set; }

        [XmlElement(ElementName = "account_id")]
        public string AccountId { get; set; }

        [XmlElement(ElementName = "tour_id")]
        public string TourId { get; set; }

        [XmlElement(ElementName = "tour_name")]
        public string TourName { get; set; }

        [XmlElement(ElementName = "tour_name_long")]
        public string TourNameLong { get; set; }

        [XmlElement(ElementName = "time_type")]
        public string TimeType { get; set; }

        [XmlElement(ElementName = "start_timezone")]
        public string StartTimeZone { get; set; }

        [XmlElement(ElementName = "end_timezone")]
        public string EndTimeZone { get; set; }

        [XmlElement(ElementName = "quantity_rule")]
        public string QuantityRule { get; set; }

        [XmlElement(ElementName = "from_price")]
        public string FromPrice { get; set; }

        [XmlElement(ElementName = "from_price_display")]
        public string FromPriceDisplay { get; set; }

        [XmlElement(ElementName = "from_price_unit")]
        public string FromPriceUnit { get; set; }

        [XmlElement(ElementName = "sale_currency")]
        public string SaleCurrency { get; set; }

        [XmlElement(ElementName = "has_sale")]
        public string HasSale { get; set; }

        [XmlElement(ElementName = "has_d")]
        public string HasD { get; set; }

        [XmlElement(ElementName = "has_f")]
        public string HasF { get; set; }

        [XmlElement(ElementName = "has_h")]
        public string HasH { get; set; }

        [XmlElement(ElementName = "created_date")]
        public string CreatedDate { get; set; }

        [XmlElement(ElementName = "descriptions_last_updated")]
        public string DescriptionsLastUpdated { get; set; }

        [XmlElement(ElementName = "volume_pricing")]
        public string VolumePricing { get; set; }

        [XmlElement(ElementName = "min_booking_size")]
        public string MinBookingSize { get; set; }

        [XmlElement(ElementName = "max_booking_size")]
        public string MaxBookingSize { get; set; }

        [XmlElement(ElementName = "next_bookable_date")]
        public string NextBookableDate { get; set; }

        [XmlElement(ElementName = "last_bookable_date")]
        public string LastBookableDate { get; set; }

        [XmlElement(ElementName = "has_sale_jan")]
        public string HasSaleJan { get; set; }

        [XmlElement(ElementName = "has_sale_feb")]
        public string HasSaleFeb { get; set; }

        [XmlElement(ElementName = "has_sale_mar")]
        public string HasSaleMar { get; set; }

        [XmlElement(ElementName = "has_sale_apr")]
        public string HasSaleApr { get; set; }

        [XmlElement(ElementName = "has_sale_may")]
        public string HasSaleMay { get; set; }

        [XmlElement(ElementName = "has_sale_jun")]
        public string HasSaleJun { get; set; }

        [XmlElement(ElementName = "has_sale_jul")]
        public string HasSaleJul { get; set; }

        [XmlElement(ElementName = "has_sale_aug")]
        public string HasSaleAug { get; set; }

        [XmlElement(ElementName = "has_sale_sep")]
        public string HasSaleSep { get; set; }

        [XmlElement(ElementName = "has_sale_oct")]
        public string HasSaleOct { get; set; }

        [XmlElement(ElementName = "has_sale_nov")]
        public string HasSaleNov { get; set; }

        [XmlElement(ElementName = "has_sale_dec")]
        public string HasSaleDec { get; set; }

        [XmlElement(ElementName = "from_price_jan")]
        public string FromPriceJan { get; set; }

        [XmlElement(ElementName = "from_price_jan_display")]
        public string FromPriceJanDisplay { get; set; }

        [XmlElement(ElementName = "from_price_feb")]
        public string FromPriceFeb { get; set; }

        [XmlElement(ElementName = "from_price_feb_display")]
        public string FromPriceFebDisplay { get; set; }

        [XmlElement(ElementName = "from_price_mar")]
        public string FromPriceMar { get; set; }

        [XmlElement(ElementName = "from_price_mar_display")]
        public string FromPriceMarDisplay { get; set; }

        [XmlElement(ElementName = "from_price_apr")]
        public string FromPriceApr { get; set; }

        [XmlElement(ElementName = "from_price_apr_display")]
        public string FromPriceAprDisplay { get; set; }

        [XmlElement(ElementName = "from_price_may")]
        public string FromPriceMay { get; set; }

        [XmlElement(ElementName = "from_price_may_display")]
        public string FromPriceMayDisplay { get; set; }

        [XmlElement(ElementName = "from_price_jun")]
        public string FromPriceJun { get; set; }

        [XmlElement(ElementName = "from_price_jun_display")]
        public string FromPriceJunDisplay { get; set; }

        [XmlElement(ElementName = "from_price_jul")]
        public string FromPriceJul { get; set; }

        [XmlElement(ElementName = "from_price_jul_display")]
        public string FromPriceJulDisplay { get; set; }

        [XmlElement(ElementName = "from_price_aug")]
        public string FromPriceAug { get; set; }

        [XmlElement(ElementName = "from_price_aug_display")]
        public string FromPriceAugDisplay { get; set; }

        [XmlElement(ElementName = "from_price_sep")]
        public string FromPriceSep { get; set; }

        [XmlElement(ElementName = "from_price_sep_display")]
        public string FromPriceSepDisplay { get; set; }

        [XmlElement(ElementName = "from_price_oct")]
        public string FromPriceOct { get; set; }

        [XmlElement(ElementName = "from_price_oct_display")]
        public string FromPriceOctDisplay { get; set; }

        [XmlElement(ElementName = "from_price_nov")]
        public string FromPriceNov { get; set; }

        [XmlElement(ElementName = "from_price_nov_display")]
        public string FromPriceNovDisplay { get; set; }

        [XmlElement(ElementName = "from_price_dec")]
        public string FromPriceDec { get; set; }

        [XmlElement(ElementName = "from_price_dec_display")]
        public string FromPriceDecDisplay { get; set; }

        [XmlElement(ElementName = "priority")]
        public string Priority { get; set; }

        [XmlElement(ElementName = "country")]
        public string Country { get; set; }

        [XmlElement(ElementName = "languages_spoken")]
        public string LanguagesSpoken { get; set; }


        [XmlElement(ElementName = "geocode_start")]
        public string GeocodeStart { get; set; }

        [XmlElement(ElementName = "geocode_end")]
        public string GeocodeEnd { get; set; }


        [XmlElement(ElementName = "tourleader_type")]
        public string TourleaderType { get; set; }


        [XmlElement(ElementName = "grade")]
        public string Grade { get; set; }

        [XmlElement(ElementName = "accomrating")]
        public string Accomrating { get; set; }

        [XmlElement(ElementName = "location")]
        public string Location { get; set; }

        [XmlElement(ElementName = "summary")]
        public string Summary { get; set; }

        [XmlElement(ElementName = "shortdesc")]
        public string Shortdesc { get; set; }

        [XmlElement(ElementName = "exp")]
        public string Exp { get; set; }

        [XmlElement(ElementName = "duration_desc")]
        public string DurationDesc { get; set; }

        [XmlElement(ElementName = "duration")]
        public string Duration { get; set; }

        [XmlElement(ElementName = "available")]
        public string Available { get; set; }

        [XmlElement(ElementName = "inc_ex")]
        public string IncEx { get; set; }

        [XmlElement(ElementName = "inc")]
        public string Inc { get; set; }

        [XmlElement(ElementName = "ex")]
        public string Ex { get; set; }

        [XmlElement(ElementName = "tour_url")]
        public string TourUrl { get; set; }


        [XmlElement(ElementName = "tour_url_tracked")]
        public string TourUrlTracked { get; set; }

        [XmlElement(ElementName = "book_url")]
        public string BookUrl { get; set; }

        [XmlElement(ElementName = "suitable_for_solo")]
        public string SuitableForSolo { get; set; }

        [XmlElement(ElementName = "suitable_for_couples")]
        public string SuitableForCouples { get; set; }

        [XmlElement(ElementName = "suitable_for_children")]
        public string SuitableForChildren { get; set; }

        [XmlElement(ElementName = "suitable_for_groups")]
        public string SuitableForGroups { get; set; }

        [XmlElement(ElementName = "suitable_for_students")]
        public string SuitableForStudents { get; set; }

        [XmlElement(ElementName = "suitable_for_business")]
        public string SuitableForBusiness { get; set; }

        [XmlElement(ElementName = "suitable_for_wheelchairs")]
        public string SuitableForWheelchairs { get; set; }

        [XmlElement(ElementName = "pickup_on_request")]
        public string PickupOnRequest { get; set; }

        [XmlElement(ElementName = "pickup_on_request_key")]
        public string PickupOnRequestKey { get; set; }

        [XmlElement(ElementName = "distribution_identifier")]
        public string DistributionIdentifier { get; set; }

        [XmlElement(ElementName = "pickup_points")]
        public List<ResponseTourPickup> PickupPoints { get; set; }

        [XmlElement(ElementName = "images")]
        public ResponseTourImages Images { get; set; }

        [XmlElement(ElementName = "new_booking")]
        public ResponseTourNew_booking NewBooking { get; set; }

        [XmlElement(ElementName = "health_and_safety")]
        public List<ResponseTourItem> HealthAndSafety { get; set; }

        [XmlElement(ElementName = "product_type")]
        public ResponseTourProduct_type ProductType { get; set; }

        [XmlElement(ElementName = "cutoff")]
        public ResponseTourCutoff CutOff { get; set; }
    }

    public class ResponseTourPickup
    {
        [XmlElement(ElementName = "pickup_name")]
        public string PickupName { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "address1")]
        public string Address1 { get; set; }

        [XmlElement(ElementName = "address2")]
        public string Address2 { get; set; }

        [XmlElement(ElementName = "postcode")]
        public string Postcode { get; set; }
        [XmlElement(ElementName = "geocode")]
        public string Geocode { get; set; }

        [XmlElement(ElementName = "pickup_id")]
        public string PickupId { get; set; }
    }

    public class ResponseTourImages
    {
        [XmlElement(ElementName = "url_thumbnail")]
        public string UrlThumbnail { get; set; }

        [XmlElement(ElementName = "url")]
        public string Url { get; set; }

        [XmlElement(ElementName = "url_large")]
        public string UrlLarge { get; set; }

        [XmlElement(ElementName = "url_xlarge")]
        public string UrlXlarge { get; set; }

        [XmlElement(ElementName = "image_desc")]
        public string ImageDesc { get; set; }
        [XmlElement(ElementName = "thumbnail")]
        public string Thumbnail { get; set; }
    }

    public class ResponseTourNew_booking
    {

        [XmlElement(ElementName = "people_selection")]
        public PeopleSelection PeopleSelection { get; set; }

        [XmlElement(ElementName = "date_selection")]
        public DateSelection DateSelection { get; set; }

     }

    public class PeopleSelection
    {
        [XmlElement(ElementName = "rate")]
        public List<Rate> BookingRate { get; set; }
    }
    public class DateSelection
    {
        [XmlElement(ElementName = "date_type")]
        public string Datetype { get; set; }
    }

    
    public class Rate
    {

        [XmlElement(ElementName = "label_1")]
        public string Label1 { get; set; }

        [XmlElement(ElementName = "label_2")]
        public string Label2 { get; set; }

        [XmlElement(ElementName = "minimum")]
        public string Minimum { get; set; }

        [XmlElement(ElementName = "maximum")]
        public string Maximum { get; set; }

        [XmlElement(ElementName = "rate_id")]
        public string RateId { get; set; }

        [XmlElement(ElementName = "agecat")]
        public string AgeCat { get; set; }

        [XmlElement(ElementName = "agerange_min")]
        public string AgerangeMin { get; set; }

        [XmlElement(ElementName = "agerange_max")]
        public string AgerangeMax { get; set; }

        [XmlElement(ElementName = "from_price")]
        public string FromPrice { get; set; }

        [XmlElement(ElementName = "from_price_display")]
        public string FromPriceDisplay { get; set; }
    }
    public class ResponseTourItem
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "display_name")]
        public string DisplayName { get; set; }

        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
      }

    public class ResponseTourProduct_type
    {
        [XmlElement(ElementName = "direction")]
        public string Direction { get; set; }

        [XmlElement(ElementName = "airport_code")]
        public string AirportCode { get; set; }

        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    public class ResponseTourCutoff
    {
        [XmlElement(ElementName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
   }
}