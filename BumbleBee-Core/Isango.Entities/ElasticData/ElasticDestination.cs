using System;
using System.Collections.Generic;
namespace Isango.Entities.ElasticData
{
    public class ElasticDestination
    {

        public bool Success { get; set; }

        public int Code { get; set; }

        public List<DestinationDatum> Data { get; set; }

        public DestinationMeta Meta { get; set; }
    }

    public class DestinationMeta
    {

        public DestinationPagination Pagination { get; set; }
    }

    public class DestinationPagination
    {

        public int Total { get; set; }

        public int Count { get; set; }

        public int PerPage { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }

    public class DestinationDatum
    {

        public int Regionid { get; set; }

        public string RegionName { get; set; }

        public string Url { get; set; }

        public string Affiliatekey { get; set; }

        public string Languagecode { get; set; }

        public int BookingCount { get; set; }

        public int BoosterCount { get; set; }

        public string Location { get; set; }

        public bool IsActive { get; set; }

        public int ClickCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

