using System;
using System.Collections.Generic;

namespace ServiceAdapters.HB.HB.Entities.Search
{
    #region SearchActivities Response classes https://api.test.hotelbeds.com/activity-api/3.0/activities

    public class SearchRs : EntityBase
    {
        public string OperationId { get; set; }

        public List<Error> Errors { get; set; }
        public Pagination Pagination { get; set; }
        public Auditdata AuditData { get; set; }
        public List<Activity> Activities { get; set; }
    }

    public class Auditdata
    {
        public string ServerId { get; set; }
        public string Environment { get; set; }
        public float ProcessTime { get; set; }
        public DateTime Time { get; set; }
    }

    public class Error
    {
        public string Code { get; set; }
        public string Text { get; set; }
    }

    public class Activity
    {
        public int Order { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public string CurrencyName { get; set; }
        public List<Amountsfrom> AmountsFrom { get; set; }
        public List<Modality> Modalities { get; set; }
        public Country Country { get; set; }
        public Content Content { get; set; }
    }

    public class Country
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<Destination> Destinations { get; set; }
    }

    public class Destination
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Content
    {
        public string ContentId { get; set; }
        public string Name { get; set; }
        public Geolocation Geolocation { get; set; }
        public Location Location { get; set; }
        public Media Media { get; set; }
        public string Description { get; set; }
        public List<Featuregroup> FeatureGroups { get; set; }
        public List<Segmentationgroup> SegmentationGroups { get; set; }
    }

    public class Geolocation
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class Location
    {
        public List<Startingpoint> StartingPoints { get; set; }
    }

    public class Startingpoint
    {
        public string Type { get; set; }
        public Meetingpoint MeetingPoint { get; set; }
    }

    public class Meetingpoint
    {
        public string Type { get; set; }
        public Geolocation Geolocation { get; set; }
        public string Address { get; set; }
        public Country Country { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public string Zip { get; set; }
    }

    public class Media
    {
        public List<Image> Images { get; set; }
    }

    public class Image
    {
        public int VisualizationOrder { get; set; }
        public string MimeType { get; set; }
        public List<Url> Urls { get; set; }
    }

    public class Url
    {
        public int Dpi { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Resource { get; set; }
        public string SizeType { get; set; }
    }

    public class Featuregroup
    {
        public string GroupCode { get; set; }
        public List<Included> Included { get; set; }
    }

    public class Included
    {
        public string FeatureType { get; set; }
        public string Description { get; set; }
    }

    public class Segmentationgroup
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public List<Segment> Segments { get; set; }
    }

    public class Segment
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class Amountsfrom
    {
        public string PaxType { get; set; }
        public int AgeFrom { get; set; }
        public int AgeTo { get; set; }
        public float Amount { get; set; }
        public float BoxOfficeAmount { get; set; }
    }

    public class Modality
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Duration Duration { get; set; }
        public List<Amountsfrom> AmountsFrom { get; set; }
        public List<Cancellationpolicy> CancellationPolicies { get; set; }
    }

    public class Duration
    {
        public float Value { get; set; }
        public string Metric { get; set; }
    }

    public class Cancellationpolicy
    {
        public DateTime DateFrom { get; set; }
        public float Amount { get; set; }
    }

    #endregion SearchActivities Response classes https://api.test.hotelbeds.com/activity-api/3.0/activities
}