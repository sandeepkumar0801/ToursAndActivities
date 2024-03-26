using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.HB.HB.Entities.Calendar
{
    public class CalendarRs
    {
        [JsonProperty("operationId")]
        public string OperationId { get; set; }

        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }

        [JsonProperty("auditData")]
        public Auditdata AuditData { get; set; }

        [JsonProperty("activities")]
        public List<Activity> Activities { get; set; }

        [JsonProperty("summaryLog")]
        public string SummaryLog { get; set; }
    }

    public class Pagination
    {
        [JsonProperty("itemsPerPage")]
        public int ItemsPerPage { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("totalItems")]
        public int TotalItems { get; set; }
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

    public class Activity
    {
        [JsonProperty("activityCode")]
        public string ActivityCode { get; set; }

        [JsonProperty("country")]
        public Country Country { get; set; }

        [JsonProperty("operationDays")]
        public List<Operationday> OperationDays { get; set; }

        [JsonProperty("modalities")]
        public List<Modality> Modalities { get; set; }

        [JsonProperty("currencyName")]
        public string CurrencyName { get; set; }

        [JsonProperty("amountsFrom")]
        public List<Amountsfrom1> AmountsFrom { get; set; }

        [JsonProperty("content")]
        public Content Content { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("IsangoServiceId")]
        public int? IsangoServiceId { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
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
        [JsonProperty("detailedInfo")]
        public string[] DetailedInfo { get; set; }

        [JsonProperty("importantInfo")]
        public string[] ImportantInfo { get; set; }

        [JsonProperty("redeemInfo")]
        public Redeeminfo RedeemInfo { get; set; }

        [JsonProperty("routes")]
        public Route[] Routes { get; set; }

        [JsonProperty("scheduling")]
        public Scheduling Scheduling { get; set; }

        [JsonProperty("activityFactsheetType")]
        public string ActivityFactsheetType { get; set; }

        [JsonProperty("activityCode")]
        public string ActivityCode { get; set; }

        [JsonProperty("modalityCode")]
        public string ModalityCode { get; set; }

        [JsonProperty("modalityName")]
        public string ModalityName { get; set; }

        [JsonProperty("lastUpdate")]
        public string LastUpdate { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("advancedTips")]
        public object[] AdvancedTips { get; set; }

        [JsonProperty("countries")]
        public Country3[] Countries { get; set; }

        [JsonProperty("highligths")]
        public List<string> Highligths { get; set; }

        [JsonProperty("guidingOptions")]
        public Guidingoptions GuidingOptions { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("featureGroups")]
        public List<Featuregroup> FeatureGroups { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("media")]
        public Media Media { get; set; }

        [JsonProperty("segmentationGroups")]
        public List<Segmentationgroup> SegmentationGroups { get; set; }

        [JsonProperty("contentId")]
        public string ContentId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
    }

    public class Location
    {
        [JsonProperty("endPoints")]
        public object[] EndPoints { get; set; }

        [JsonProperty("startingPoints")]
        public List<Startingpoint> StartingPoints { get; set; }
    }

    public class Startingpoint
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("meetingPoint")]
        public Meetingpoint MeetingPoint { get; set; }
    }

    public class Meetingpoint
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("geolocation")]
        public Geolocation Geolocation { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("country")]
        public Country1 Country { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class Geolocation
    {
        [JsonProperty("latitude")]
        public float Latitude { get; set; }

        [JsonProperty("longitude")]
        public float Longitude { get; set; }
    }

    public class Country1
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("destinations")]
        public List<Destination1> Destinations { get; set; }
    }

    public class Destination1
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
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
        public string MimeType { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("urls")]
        public List<Url> Urls { get; set; }
    }

    public class Url
    {
        [JsonProperty("dpi")]
        public int Dpi { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("resource")]
        public string Resource { get; set; }

        [JsonProperty("sizeType")]
        public string SizeType { get; set; }
    }

    public class Redeeminfo
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("directEntrance")]
        public bool DirectEntrance { get; set; }

        [JsonProperty("comments")]
        public Comment[] Comments { get; set; }
    }

    public class Comment
    {
        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class Scheduling
    {
        [JsonProperty("opened")]
        public List<Opened> Opened { get; set; }

        [JsonProperty("duration")]
        public Duration Duration { get; set; }
    }

    public class Duration
    {
        [JsonProperty("value")]
        public float Value { get; set; }

        [JsonProperty("metric")]
        public string Metric { get; set; }
    }

    public class Opened
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("openingTime")]
        public string OpeningTime { get; set; }

        [JsonProperty("closeTime")]
        public string CloseTime { get; set; }

        [JsonProperty("weekDays")]
        public string[] WeekDays { get; set; }

        [JsonProperty("lastAdmissionTime")]
        public string LastAdmissionTime { get; set; }
    }

    public class Guidingoptions
    {
        [JsonProperty("guideType")]
        public string GuideType { get; set; }

        [JsonProperty("included")]
        public bool Included { get; set; }

        [JsonProperty("groupType")]
        public string GroupType { get; set; }
    }

    public class Featuregroup
    {
        [JsonProperty("groupCode")]
        public string GroupCode { get; set; }

        [JsonProperty("included")]
        public Included[] Included { get; set; }

        [JsonProperty("excluded")]
        public Excluded[] Excluded { get; set; }
    }

    public class Included
    {
        [JsonProperty("featureType")]
        public string FeatureType { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class Excluded
    {
        [JsonProperty("featureType")]
        public string FeatureType { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class Route
    {
        [JsonProperty("duration")]
        public Duration1 Duration { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("timeFrom")]
        public string TimeFrom { get; set; }

        [JsonProperty("timeTo")]
        public string TimeTo { get; set; }

        [JsonProperty("points")]
        public Point[] Points { get; set; }
    }

    public class Duration1
    {
        [JsonProperty("value")]
        public float Value { get; set; }

        [JsonProperty("metric")]
        public string Metric { get; set; }
    }

    public class Point
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("stop")]
        public bool Stop { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("pointOfInterest")]
        public Pointofinterest PointOfInterest { get; set; }
    }

    public class Pointofinterest
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("geolocation")]
        public Geolocation1 Geolocation { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("country")]
        public Country2 Country { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class Geolocation1
    {
        [JsonProperty("latitude")]
        public float Latitude { get; set; }

        [JsonProperty("longitude")]
        public float Longitude { get; set; }
    }

    public class Country2
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public class Segmentationgroup
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("segments")]
        public List<Segment> Segments { get; set; }
    }

    public class Segment
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Country3
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("destinations")]
        public Destination2[] Destinations { get; set; }
    }

    public class Destination2
    {
        [JsonProperty("code")]
        public string Code { get; set; }

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
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("duration")]
        public Duration Duration { get; set; }

        [JsonProperty("destinationCode")]
        public string DestinationCode { get; set; }

        [JsonProperty("contract")]
        public Contract Contract { get; set; }

        [JsonProperty("languages")]
        public object[] Languages { get; set; }

        [JsonProperty("questions")]
        public Questions[] Questions { get; set; }

        [JsonProperty("amountsFrom")]
        public Amountsfrom[] AmountsFrom { get; set; }

        [JsonProperty("rates")]
        public List<Rate> Rates { get; set; }

        [JsonProperty("amountUnitType")]
        public string AmountUnitType { get; set; }

        [JsonProperty("uniqueIdentifier")]
        public string UniqueIdentifier { get; set; }
    }

    public class Questions
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("text")]
        public string Question { get; set; }

        [JsonProperty("required")]
        public bool? Required { get; set; } 
    }

    public class Duration2
    {
        [JsonProperty("value")]
        public float Value { get; set; }

        [JsonProperty("metric")]
        public string Metric { get; set; }
    }

    public class Contract
    {
        [JsonProperty("incomingOffice")]
        public int IncomingOffice { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
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

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rateDetails")]
        public List<Ratedetail> RateDetails { get; set; }
    }

    public class Ratedetail
    {
        [JsonProperty("rateKey")]
        public string RateKey { get; set; }

        [JsonProperty("languages")]
        public object[] Languages { get; set; }

        [JsonProperty("sessions")]
        public object[] Sessions { get; set; }

        [JsonProperty("operationDates")]
        public List<Operationdate> OperationDates { get; set; }

        [JsonProperty("minimumDuration")]
        public Minimumduration MinimumDuration { get; set; }

        [JsonProperty("maximumDuration")]
        public Maximumduration MaximumDuration { get; set; }

        [JsonProperty("totalAmount")]
        public Totalamount TotalAmount { get; set; }

        [JsonProperty("paxAmounts")]
        public List<Paxamount> PaxAmounts { get; set; }

        [JsonProperty("agencyCommission")]
        public Agencycommission AgencyCommission { get; set; }
    }

    public class Minimumduration
    {
        [JsonProperty("value")]
        public float Value { get; set; }

        [JsonProperty("metric")]
        public string Metric { get; set; }
    }

    public class Maximumduration
    {
        [JsonProperty("value")]
        public float Value { get; set; }

        [JsonProperty("metric")]
        public string Metric { get; set; }
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

    public class Amountsfrom1
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
}