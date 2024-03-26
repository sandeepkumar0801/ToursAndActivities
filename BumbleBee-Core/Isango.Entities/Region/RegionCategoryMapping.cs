namespace Isango.Entities.Region
{
    public class RegionCategoryMapping
    {
        public string CategoryName { get; set; }
        public string CategoryType { get; set; }
        public int CategoryId { get; set; }
        public int Order { get; set; }
        public bool IsTopCategory { get; set; }
        public bool IsVisibleOnSearch { get; set; }
        public int RegionId { get; set; }
        public int CountryId { get; set; }
        public int NumberOfProducts { get; set; }
        public string CoOrdinates { get; set; }
        public string ImageName { get; set; }
        public string ImageAltText { get; set; }
        public int ImageId { get; set; }
        public string Languagecode { get; set; }
        public int ReviewCount { get; set; }
        public string AverageReviewRating { get; set; }

        public string RegionName { get; set; }
        public string Type_ { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}