using System;
using System.Collections.Generic;


namespace Isango.Entities.ElasticData
{
    public class ElasticProduct
    {

        public bool Success { get; set; }

        public int Code { get; set; }

        public List<ProductDatum> Data { get; set; }

        public ProductMeta Meta { get; set; }
    }

    public class ProductMeta
    {

        public ProductPagination Pagination { get; set; }
    }

    public class ProductPagination
    {

        public int Total { get; set; }

        public int Count { get; set; }

        public int PerPage { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }

    public class ProductDatum
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public int ServiceId { get; set; }
       
        public string ServiceName { get; set; }

        public int OfferPercent { get; set; }
       
        public string Affiliatekey { get; set; }
        public string LanguageCode { get; set; }
        public string Url { get; set; }

        public int BoosterCount { get; set; }

        public int BookingCount { get; set; }

        public string Location { get; set; }

        public string ServicenameRegionName { get; set; }

        public bool IsActive { get; set; }

        public int ClickCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string Image_link { get; set; }

        public decimal Price { get; set; }
        public string Currency { get; set; }
    }
}

