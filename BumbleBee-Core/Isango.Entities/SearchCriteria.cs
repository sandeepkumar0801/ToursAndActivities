using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class SearchCriteria
    {
        public ProductType ProductType { get; set; }
        public int RegionId { get; set; }
        public int CategoryId { get; set; }
        public string Keyword { get; set; }
        public ProductSortType SortType { get; set; }
        public int MaxNoOfProducts { get; set; }
        public string ProductIDs { get; set; }
        public string CategoryIDs { get; set; }
        public string RegionIDs { get; set; }
        public Criteria HotelBedsCriteria { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string RegionFilterIds { get; set; }
        public string AttractionFilterIds { get; set; }
        public bool IsOffer { get; set; }
        public bool IsSmartphoneFilter { get; set; }
        public string SelectedDates { get; set; }
        public bool IsBundle { get; set; }
    }
}