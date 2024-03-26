using System;
using System.Collections.Generic;

namespace Isango.Entities.ElasticData
{
    public class ElasticAttraction
    {

        public bool Success { get; set; }

        public int Code { get; set; }

        public List<AttractionDatum> Data { get; set; }

        public AttractionMeta Meta { get; set; }
    }

    public class AttractionMeta
    {

        public AttractionPagination Pagination { get; set; }
    }

    public class AttractionPagination
    {

        public int Total { get; set; }

        public int Count { get; set; }

        public int PerPage { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }

    public class AttractionDatum
    {

        public int RegionId { get; set; }

        public string RegionName { get; set; }

        public int ServiceId { get; set; }

        public string ServiceName { get; set; }

        public int AffiliateKey { get; set; }

        public string LanguageCode { get; set; }

        public string Url { get; set; }

        public int BoosterCount { get; set; }

        public int BookingCount { get; set; }

        public bool IsActive { get; set; }

        public string ServiceType { get; set; }

        public string Location { get; set; }

        public string ServicenameRegionname { get; set; }

        public int ClickCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
