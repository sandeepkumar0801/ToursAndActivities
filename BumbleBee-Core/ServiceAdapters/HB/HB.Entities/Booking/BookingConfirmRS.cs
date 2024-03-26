using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using DetailFull = ServiceAdapters.HB.HB.Entities.ActivityDetailFull;

namespace ServiceAdapters.HB.HB.Entities.Booking.BookingDetail
{
    public class BookingDetailRs : EntityBase
    {
        [JsonProperty("booking")]
        public Booking Booking { get; set; }
    }

    public class Booking
    {
        public string reference { get; set; }
        public string status { get; set; }
        public string currency { get; set; }
        public float pendingAmount { get; set; }
        public Agency agency { get; set; }
        public DateTime creationDate { get; set; }
        public Paymentdata paymentData { get; set; }

        /// <summary>
        /// Isango Booking Reference Number i.e SGI831543
        /// </summary>
        public string clientReference { get; set; }

        public Holder holder { get; set; }
        public float total { get; set; }
        public float totalNet { get; set; }
        public List<Activity> activities { get; set; }
    }

    public class Agency
    {
        public int code { get; set; }
        public int branch { get; set; }
        public string comments { get; set; }
        public Sucursal sucursal { get; set; }
    }

    public class Sucursal
    {
        public string name { get; set; }
        public string street { get; set; }
        public string zip { get; set; }
        public string city { get; set; }
        public string email { get; set; }
        public string region { get; set; }
    }

    public class Paymentdata
    {
        public Paymenttype paymentType { get; set; }
        public Invoicingcompany invoicingCompany { get; set; }
        public string description { get; set; }
    }

    public class Paymenttype
    {
        public string code { get; set; }
    }

    public class Invoicingcompany
    {
        public string code { get; set; }
        public string name { get; set; }
        public string registrationNumber { get; set; }
    }

    public partial class Activity
    {
        public string status { get; set; }
        public Supplier supplier { get; set; }
        public List<Comment> comments { get; set; }
        public string type { get; set; }
        public List<Voucher> vouchers { get; set; }

        /// <summary>
        /// Hotel bed booking reference number
        /// "activityReference": "254-2247158", It required for any post operation on booking.
        /// </summary>
        public string activityReference { get; set; }

        public string code { get; set; }
        public string name { get; set; }
        public Modality modality { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public List<Cancellationpolicy> cancellationPolicies { get; set; }
        public List<Pax> paxes { get; set; }
        public List<Answer> questions { get; set; }
        public string id { get; set; }
        public Agencycommission agencyCommission { get; set; }
        public Contactinfo contactInfo { get; set; }
        public Amountdetail amountDetail { get; set; }
        public List<Extradata> extraData { get; set; }
        public Providerinformation providerInformation { get; set; }
        public Content content { get; set; }
    }

    public class Supplier
    {
        public string name { get; set; }
        public string vatNumber { get; set; }
    }

    public class Modality
    {
        public string code { get; set; }
        public string name { get; set; }
        public List<Rate> rates { get; set; }
        public string amountUnitType { get; set; }
    }

    public class Rate
    {
        public List<Ratedetail> rateDetails { get; set; }
    }

    public class Ratedetail
    {
        public List<DetailFull.Language> languages { get; set; }
        public List<DetailFull.Session> sessions { get; set; }
    }

    public class Language
    {
        public string code { get; set; }
    }

    public class Agencycommission
    {
        public float percentage { get; set; }
        public float amount { get; set; }
        public float vatAmount { get; set; }
    }

    public class Contactinfo
    {
        public string address { get; set; }
        public Country country { get; set; }
    }

    public class Country
    {
        public List<Destination> destinations { get; set; }
    }

    public class Destination
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Amountdetail
    {
        public List<Paxamount> paxAmounts { get; set; }
        public Totalamount totalAmount { get; set; }
    }

    public class Totalamount
    {
        public float amount { get; set; }
    }

    public class Paxamount
    {
        public string paxType { get; set; }
        public float amount { get; set; }
    }

    public class Providerinformation
    {
        public string name { get; set; }
        public string address { get; set; }
        public string reference { get; set; }
    }

    public class Content
    {
        public string name { get; set; }
        public List<string> detailedInfo { get; set; }
        public List<Featuregroup> featureGroups { get; set; }
        public Location location { get; set; }
        public Media media { get; set; }
        public List<Segmentationgroup> segmentationGroups { get; set; }
        public Geolocation geolocation { get; set; }
        public string activityFactsheetType { get; set; }
        public string activityCode { get; set; }
        public string modalityCode { get; set; }
        public string contentId { get; set; }
        public string description { get; set; }
        public List<Country> countries { get; set; }
    }

    public class Location
    {
        public List<Startingpoint> startingPoints { get; set; }
    }

    public class Startingpoint
    {
        public string type { get; set; }
        public Meetingpoint meetingPoint { get; set; }
    }

    public class Meetingpoint
    {
        public string type { get; set; }
        public Geolocation geolocation { get; set; }
        public string address { get; set; }
        public Country country { get; set; }
        public string description { get; set; }
    }

    public class Geolocation
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class Country1
    {
        public string code { get; set; }
        public string name { get; set; }
        public List<Destination> destinations { get; set; }
    }

    public class Media
    {
        public List<Image> images { get; set; }
    }

    public class Image
    {
        public int visualizationOrder { get; set; }
        public string mimeType { get; set; }
        public List<Url> urls { get; set; }
    }

    public class Url
    {
        public int dpi { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string resource { get; set; }
        public string sizeType { get; set; }
    }

    public class Featuregroup
    {
        public string groupCode { get; set; }
        public List<Included> included { get; set; }
    }

    public class Included
    {
        public string featureType { get; set; }
        public string description { get; set; }
    }

    public class Segmentationgroup
    {
        public int code { get; set; }
        public string name { get; set; }
        public List<Segment> segments { get; set; }
    }

    public class Segment
    {
        public int code { get; set; }
        public string name { get; set; }
    }

    public class Comment
    {
        public string type { get; set; }
        public string text { get; set; }
    }

    public class Cancellationpolicy
    {
        public DateTime dateFrom { get; set; }
        public float amount { get; set; }
    }

    public class Pax
    {
        public string customerId { get; set; }
        public string passport { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }
    }

    public class Extradata
    {
        public string id { get; set; }
        public string value { get; set; }
    }

    public class Voucher
    {
        public string customerId { get; set; }
        public string code { get; set; }
        public string language { get; set; }
        public string url { get; set; }
        public string mimeType { get; set; }
    }
}