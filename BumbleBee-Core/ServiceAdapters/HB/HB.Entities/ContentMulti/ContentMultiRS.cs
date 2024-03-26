using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.HB.HB.Entities.ContentMulti
{
    public class ContentMultiRS
    {
        [JsonProperty("operationId")]
        public string OperationId { get; set; }

        [JsonProperty("auditData")]
        public Auditdata AuditData { get; set; }

        [JsonProperty("activitiesContent")]
        public List<ActivitiesContent> ActivitiesContent { get; set; }
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

    public class ActivitiesContent
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("detailedInfo")]
        public object[] DetailedInfo { get; set; }

        [JsonProperty("featureGroups")]
        public List<Featuregroup> FeatureGroups { get; set; }

        [JsonProperty("guidingOptions")]
        public Guidingoptions GuidingOptions { get; set; }

        [JsonProperty("importantInfo")]
        public string[] ImportantInfo { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("media")]
        public Media Media { get; set; }

        [JsonProperty("redeemInfo")]
        public Redeeminfo RedeemInfo { get; set; }

        [JsonProperty("routes")]
        public List<Route> Routes { get; set; }

        [JsonProperty("scheduling")]
        public Scheduling Scheduling { get; set; }

        [JsonProperty("segmentationGroups")]
        public List<Segmentationgroup> SegmentationGroups { get; set; }

        [JsonProperty("activityFactsheetType")]
        public string ActivityFactsheetType { get; set; }

        [JsonProperty("activityCode")]
        public string ActivityCode { get; set; }

        [JsonProperty("modalityCode")]
        public string ModalityCode { get; set; }

        [JsonProperty("modalityName")]
        public string ModalityName { get; set; }

        [JsonProperty("contentId")]
        public string ContentId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("lastUpdate")]
        public string LastUpdate { get; set; }

        [JsonProperty("advancedTips")]
        public object[] AdvancedTips { get; set; }

        [JsonProperty("countries")]
        public List<Country2> Countries { get; set; }

        [JsonProperty("highligths")]
        public List<string> Highligths { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
    }

    public class Guidingoptions
    {
        [JsonProperty("guideType")]
        public string GuideType { get; set; }

        [JsonProperty("included")]
        public bool Included { get; set; }
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
        public Country Country { get; set; }

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
        public bool? DirectEntrance { get; set; }

        [JsonProperty("comments")]
        public List<Comment> Comments { get; set; }
    }

    public class Comment
    {
        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class Scheduling
    {
        [JsonProperty("duration")]
        public Duration Duration { get; set; }

        [JsonProperty("opened")]
        public List<Opened> Opened { get; set; }

        [JsonProperty("closed")]
        public List<Closed> Closed { get; set; }
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
        [JsonProperty("openingTime")]
        public string OpeningTime { get; set; }

        [JsonProperty("closeTime")]
        public string CloseTime { get; set; }

        [JsonProperty("weekDays")]
        public List<string> WeekDays { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
    }

    public class Closed
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("weekDays")]
        public object[] WeekDays { get; set; }
    }

    public class Featuregroup
    {
        [JsonProperty("groupCode")]
        public string GroupCode { get; set; }

        [JsonProperty("included")]
        public List<Included> Included { get; set; }

        [JsonProperty("excluded")]
        public List<Excluded> Excluded { get; set; }
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
        public List<Point> Points { get; set; }
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
        public Country1 Country { get; set; }

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

    public class Country1
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

    public class Country2
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
}