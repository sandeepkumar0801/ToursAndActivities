using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.ChannelListResponse
{

    public class TourResponse
    {
        public string Request { get; set; }
        public string Error { get; set; }
        public List<Tour> ResponseTourList { get; set; }
    }


    public class Tour
    {
        public string ChannelId { get; set; }
        public string Accountid { get; set; }
        public string TourId { get; set; }
        public string TourCode { get; set; }
        public string HasSale { get; set; }
        public string HasD { get; set; }
        public string HasF { get; set; }
        public string HasH { get; set; }
        public string DescriptionsLastUpdated { get; set; }
        public string TourName { get; set; }
        public string TourNameLong { get; set; }
        public string TimeType { get; set; }
        public string StartTimeZone { get; set; }
        public string EndTimeZone { get; set; }
        public string QuantityRule { get; set; }
        public string FromPrice { get; set; }
        public string FromPriceDisplay { get; set; }
        public string FromPriceUnit { get; set; }
        public string SaleCurrency { get; set; }
        public string CreatedDate { get; set; }
        public string VolumePricing { get; set; }
        public string MinBookingSize { get; set; }
        public string MaxBookingSize { get; set; }
        public string NextBookableDate { get; set; }
        public string LastBookableDate { get; set; }
        public string HasSaleJan { get; set; }
        public string HasSaleFeb { get; set; }
        public string HasSaleMar { get; set; }
        public string HasSaleApr { get; set; }
        public string HasSaleMay { get; set; }
        public string HasSaleJun { get; set; }
        public string HasSaleJul { get; set; }
        public string HasSaleAug { get; set; }
        public string HasSaleSep { get; set; }
        public string HasSaleOct { get; set; }
        public string HasSaleNov { get; set; }
        public string HasSaleDec { get; set; }
        public string FromPriceJan { get; set; }
        public string FromPriceJanDisplay { get; set; }
        public string FromPriceFeb { get; set; }
        public string FromPriceFebDisplay { get; set; }
        public string FromPriceMar { get; set; }
        public string FromPriceMarDisplay { get; set; }
        public string FromPriceApr { get; set; }
        public string FromPriceAprDisplay { get; set; }
        public string FromPriceMay { get; set; }
        public string FromPriceMayDisplay { get; set; }
        public string FromPriceJun { get; set; }
        public string FromPriceJunDisplay { get; set; }
        public string FromPriceJul { get; set; }
        public string FromPriceJulDisplay { get; set; }
        public string FromPriceAug { get; set; }
        public string FromPriceAugDisplay { get; set; }
        public string FromPriceSep { get; set; }
        public string FromPriceSepDisplay { get; set; }
        public string FromPriceOct { get; set; }
        public string FromPriceOctDisplay { get; set; }
        public string FromPriceNov { get; set; }
        public string FromPriceNovDisplay { get; set; }
        public string FromPriceDec { get; set; }
        public string FromPriceDecDisplay { get; set; }
        public string Priority { get; set; }
        public string Country { get; set; }
        public string LanguagesSpoken { get; set; }
        public string GeocodeStart { get; set; }
        public string GeocodeEnd { get; set; }
        public string TourleaderType { get; set; }
        public string Grade { get; set; }
        public string Accomrating { get; set; }
        public string Location { get; set; }
        public string Summary { get; set; }
        public string Shortdesc { get; set; }
        public string Exp { get; set; }
        public string DurationDesc { get; set; }
        public string Duration { get; set; }
        public string Available { get; set; }
        public string IncEx { get; set; }
        public string Inc { get; set; }
        public string Ex { get; set; }
        public string TourUrl { get; set; }
        public string TourUrlTracked { get; set; }
        public string BookUrl { get; set; }
        public string SuitableForSolo { get; set; }
        public string SuitableForCouples { get; set; }
        public string SuitableForChildren { get; set; }
        public string SuitableForGroups { get; set; }
        public string SuitableForStudents { get; set; }
        public string SuitableForBusiness { get; set; }
        public string SuitableForWheelchairs { get; set; }
        public string PickupOnRequest { get; set; }
        public string PickupOnRequestKey { get; set; }
        public string DistributionIdentifier { get; set; }

        //ResponseTourPickup
        public string PickupName { get; set; }
        public string Description { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Postcode { get; set; }
        public string Geocode { get; set; }
        public string PickupId { get; set; }
        //ResponseTourPickup

        //Reposonse Tour Images
        public string UrlThumbnail { get; set; }
        public string Url { get; set; }
        public string UrlLarge { get; set; }
        public string UrlXlarge { get; set; }
        public string ImageDesc { get; set; }
        public string Thumbnail { get; set; }
        //Response Tour Images

        //ResponseTourNew_booking
        public string Datetype { get; set; }
        //ResponseTourNew_booking

        //ResponseTourNew_bookingRate
        public string Label1 { get; set; }
        public string Label2 { get; set; }
        public string Minimum { get; set; }
        public string Maximum { get; set; }
        public string RateId { get; set; }
        public string AgeCat { get; set; }
        public string AgerangeMin { get; set; }
        public string AgerangeMax { get; set; }
        public string BookingRateFromPrice { get; set; }
        public string BookingRateFromPriceDisplay { get; set; }
        //ResponseTourNew_bookingRate

        //ResponseTourItem
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }
        //ResponseTourItem

        //ResponseTourProduct_type
        public string Direction { get; set; }
        public string AirportCode { get; set; }
        public string ProductTypeValue { get; set; }
        //ResponseTourProduct_type

        //ResponseTourCutoff
        public string Type { get; set; }
        public string TourCutoffValue { get; set; }
        //ResponseTourCutoff
    }
}