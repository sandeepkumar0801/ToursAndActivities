using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.HB.HB.Entities.ActivityDetailFull
{
    public class ActivityDetailFullRS : EntityBase
    {
        [JsonProperty("operationId")]
        public string OperationId { get; set; }

        [JsonProperty("auditData")]
        public Auditdata AuditData { get; set; }

        [JsonProperty("activity")]
        public HBApiActivity Activity { get; set; }
    }

    public class Auditdata
    {
        [JsonProperty("processTime")]
        public float ProcessTime { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("serverId")]
        public string ServerId { get; set; }

        [JsonProperty("environment")]
        public string Environment { get; set; }
    }

    public class HBApiActivity
    {
        /// <summary>
        /// Example SANGO, Field is ActivityCode at api end.
        /// </summary>
        [JsonProperty("activityCode")]
        public string ActivityCode { get; set; }

        [JsonProperty("country")]
        public Country Country { get; set; }

        [JsonProperty("operationDays")]
        public List<Operationday> OperationDays { get; set; }

        [JsonProperty("modalities")]
        public List<Modality> Modalities { get; set; }

        [JsonProperty("currencyName")]
        public string currencyName { get; set; }

        [JsonProperty("amountsFrom")]
        public List<Amountsfrom> AmountsFrom { get; set; }

        [JsonProperty("content")]
        public Content Content { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("currency")]
        public string CurrencyISOCode { get; set; }

        /// <summary>
        /// Example E-U10-SANGO, Field is code at api end.
        /// </summary>
        [JsonProperty("code")]
        public string HBActivityCode { get; set; }

        /// <summary>
        /// Activity ticket type or excursion
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class Country
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("destinations")]
        public List<Destination> Destinations { get; set; }
    }

    public class Destination
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Content
    {
        [JsonProperty("name")]
        public string ContentName { get; set; }

        [JsonProperty("detailedInfo")]
        public List<string> DetailedInfo { get; set; }

        [JsonProperty("featureGroups")]
        public List<Featuregroup> FeatureGroups { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("media")]
        public Media Media { get; set; }

        [JsonProperty("segmentationGroups")]
        public List<Segmentationgroup> SegmentationGroups { get; set; }

        [JsonProperty("geolocation")]
        public Geolocation Geolocation { get; set; }

        [JsonProperty("activityFactsheetType")]
        public string ActivityFactsheetType { get; set; }

        [JsonProperty("activityCode")]
        public string ActivityCode { get; set; }

        [JsonProperty("contentId")]
        public string ContentId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("countries")]
        public Country Countries { get; set; }
    }

    public class Location
    {
        [JsonProperty("startingPoints")]
        public List<Startingpoint> StartingPoints { get; set; }
    }

    public class Startingpoint
    {
        [JsonProperty("type")]
        public string StartingPointType { get; set; }

        [JsonProperty("meetingPoint")]
        public Meetingpoint MeetingPoint { get; set; }
    }

    public class Meetingpoint
    {
        [JsonProperty("type")]
        public string MeetingPointType { get; set; }

        [JsonProperty("geolocation")]
        public Geolocation Geolocation { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("country")]
        public Country Country { get; set; }

        [JsonProperty("Description")]
        public string description { get; set; }
    }

    public class Geolocation
    {
        [JsonProperty("latitude")]
        public float Latitude { get; set; }

        [JsonProperty("longitude")]
        public float Longitude { get; set; }
    }

    public class Media
    {
        [JsonProperty("images")]
        public List<Image> Images { get; set; }
    }

    public class Image
    {
        [JsonProperty("visualizationOrder")]
        public int VisualizationOrder { get; set; }

        [JsonProperty("mimeType")]
        public string mimeType { get; set; }

        [JsonProperty("urls")]
        public List<Url> Urls { get; set; }
    }

    public class Url
    {
        [JsonProperty("dpi")]
        public int Dpi { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("Width")]
        public int width { get; set; }

        [JsonProperty("Resource")]
        public string resource { get; set; }

        [JsonProperty("SizeType")]
        public string sizeType { get; set; }
    }

    public class Featuregroup
    {
        [JsonProperty("groupCode")]
        public string GroupCode { get; set; }

        [JsonProperty("included")]
        public List<Included> Included { get; set; }
    }

    public class Included
    {
        [JsonProperty("featureType")]
        public string FeatureType { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class Segmentationgroup
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("segments")]
        public List<Segment> segments { get; set; }
    }

    public class Segment
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Operationday
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Modality
    {
        /// <summary>
        /// Option code at HB Api End. Example "872437426#1DAY@STANDARD||"
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("duration")]
        public Duration Duration { get; set; }

        [JsonProperty("comments")]
        public List<Comment> Comments { get; set; }

        [JsonProperty("supplierInformation")]
        public Supplierinformation SupplierInformation { get; set; }

        [JsonProperty("providerInformation")]
        public Providerinformation ProviderInformation { get; set; }

        [JsonProperty("destinationCode")]
        public string DestinationCode { get; set; }

        [JsonProperty("contract")]
        public Contract Contract { get; set; }

        [JsonProperty("amountsFrom")]
        public List<Amountsfrom> AmountsFrom { get; set; }

        [JsonProperty("rates")]
        public List<Rate> Rates { get; set; }

        /// <summary>
        /// Informative: Explains how the total price (see below) is calculated. If it’s a “PAX” unit type, then the total price depends on the number of paxes. If the unit type is “SERVICE”, then the total price is fixed.
        /// In any case, the total price is returned in the “totalPrice” attribute(see below) for the number of paxes provided
        /// </summary>
        [JsonProperty("amountUnitType")]
        public string AmountUnitType { get; set; }

        [JsonProperty("uniqueIdentifier")]
        public string UniqueIdentifier { get; set; }

        /// <summary>
        /// Are the relevant questions that are asked to end consumers and that are mandatory to ensure that he/she can enjoy the activity and to prevent issues. Example of questions are: passport number, weight, height, etc.
        /// </summary>
        [JsonProperty("questions")]
        public List<Question> Questions { get; set; }
    }

    public class Duration
    {
        /// <summary>
        /// Number of units. i.e. if metric has the value “DAYS” and the value is “1.5” then the route duration is 1 day and 12 hours
        /// </summary>
        [JsonProperty("value")]
        public float Value { get; set; }

        /// <summary>
        /// Type of duration. It can be one of the following: DAYS, HOURS, MINUTES for the specific case of minimumDuration,
        /// otherwise it’s always returned as “DAYS”
        /// </summary>
        [JsonProperty("metric")]
        public string Metric { get; set; }
    }

    public class Supplierinformation
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("vatNumber")]
        public string VatNumber { get; set; }
    }

    public class Providerinformation
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Contract
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Comment
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class Amountsfrom
    {
        [JsonProperty("paxType")]
        public string PaxType { get; set; }

        [JsonProperty("ageFrom")]
        public int AgeFrom { get; set; }

        [JsonProperty("ageTo")]
        public int AgeTo { get; set; }

        [JsonProperty("amount")]
        public float Amount { get; set; }

        [JsonProperty("boxOfficeAmount")]
        public float BoxOfficeAmount { get; set; }

        [JsonProperty("mandatoryApplyAmount")]
        public bool MandatoryApplyAmount { get; set; }
    }

    public class Rate
    {
        [JsonProperty("rateCode")]
        public string RateCode { get; set; }

        [JsonProperty("rateClass")]
        public string RateClass { get; set; }

        [JsonProperty("rateDetails")]
        public List<RateDetail> RateDetails { get; set; }

        /// <summary>
        /// Some activities need questions to be answered in the confirmation. For those activities, a question list is provided.
        /// </summary>
        [JsonProperty("paxQuestions")]
        public List<Question> Questions { get; set; }
    }

    public class RateDetail
    {
        [JsonProperty("rateKey")]
        public string RateKey { get; set; }

        [JsonProperty("operationDates")]
        public List<Operationdate> OperationDates { get; set; }

        [JsonProperty("minimumDuration")]
        public Duration MinimumDuration { get; set; }

        [JsonProperty("maximumDuration")]
        public Duration MaximumDuration { get; set; }

        [JsonProperty("totalAmount")]
        public Totalamount TotalAmount { get; set; }

        [JsonProperty("paxAmounts")]
        public List<Paxamount> PaxAmounts { get; set; }

        [JsonProperty("agencyCommission")]
        public Agencycommission AgencyCommission { get; set; }

        /// <summary>
        /// List of session available for this service. A session contains the scheduling of the activity. If more than one session is included in the rateDetails that means the sessions have the same price.
        /// </summary>
        [JsonProperty("sessions")]
        public List<Session> Sessions { get; set; }

        [JsonProperty("languages")]
        public List<Language> Languages { get; set; }
    }

    public class Language
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class Totalamount
    {
        [JsonProperty("amount")]
        public float Amount { get; set; }

        [JsonProperty("boxOfficeAmount")]
        public float BoxOfficeAmount { get; set; }

        [JsonProperty("mandatoryApplyAmount")]
        public bool MandatoryApplyAmount { get; set; }
    }

    public class Agencycommission
    {
        [JsonProperty("percentage")]
        public float Percentage { get; set; }

        [JsonProperty("amount")]
        public float Amount { get; set; }

        [JsonProperty("vatAmount")]
        public float VatAmount { get; set; }
    }

    public class Operationdate
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("cancellationPolicies")]
        public List<Cancellationpolicy> CancellationPolicies { get; set; }
    }

    public class Cancellationpolicy
    {
        [JsonProperty("dateFrom")]
        public DateTime DateFrom { get; set; }

        [JsonProperty("amount")]
        public float Amount { get; set; }
    }

    public class Paxamount
    {
        [JsonProperty("paxType")]
        public string PaxType { get; set; }

        [JsonProperty("ageFrom")]
        public int AgeFrom { get; set; }

        [JsonProperty("ageTo")]
        public int AgeTo { get; set; }

        [JsonProperty("amount")]
        public float Amount { get; set; }

        [JsonProperty("boxOfficeAmount")]
        public float BoxOfficeAmount { get; set; }

        [JsonProperty("mandatoryApplyAmount")]
        public bool MandatoryApplyAmount { get; set; }
    }

    public class Question
    {
        /// <summary>
        /// Question id at HotelBed apitude api end
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("required")]
        public bool IsRequired { get; set; }
    }

    public class Session
    {
        /// <summary>
        /// Session code at HotelBed apitude api end
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Session name. i.e. “Afternoon” or “5:00PM”
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}