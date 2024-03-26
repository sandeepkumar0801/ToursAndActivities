using System.Collections.Generic;

namespace Isango.Entities.Region
{
    public class RegionMetaData
    {
        public string Name { get; set; }
        public List<Region> Regions { get; set; }
        public string Description { get; set; }
        public List<Review.Review> Reviews { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string Title { get; set; }
        public List<string> QuickLinks { get; set; }
        public bool IsCountry { get; set; }
        public string Tips { get; set; }
        public string BestTime { get; set; }
        public string GettingAround { get; set; }
        public string StaySafe { get; set; }
        public string MoneySavingTips { get; set; }
        public string DidYouKnow { get; set; }
        public string ImageName { get; set; }
        public int ImageId { get; set; }
        public int HeroImageId { get; set; }
        public string Heading { get; set; }
    }
}