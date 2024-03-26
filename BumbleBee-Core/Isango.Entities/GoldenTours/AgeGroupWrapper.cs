using System.Collections.Generic;

namespace Isango.Entities.GoldenTours
{
    public class AgeGroupWrapper
    {
        public List<ProductDetail> ProductDetails { get; set; }
        public List<AgeGroup> AgeGroups { get; set; }
        public List<Periods> PricePeriods { get; set; }
    }

    public class Periods
    {
        public string ProductId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string MinCapacity { get; set; }
        public string MaxCapacity { get; set; }
    }

    public class ProductDetail
    {
        public string ProductId { get; set; }
        public string Ref { get; set; }
        public string City { get; set; }
        public string ProductType { get; set; }
        public string PricingUnitType { get; set; }
        public string Duration { get; set; }
        public string BookBefore { get; set; }
        public string BookBeforeType { get; set; }
        public string WhenRun { get; set; }
        public string StartTime { get; set; }
        public string MeetingPoint { get; set; }
        public string ReturnTime { get; set; }
        public string Inclusion { get; set; }
        public string Exclusion { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
    }

    public class AgeGroup
    {
        public string ProductId { get; set; }
        public string UnitID { get; set; }
        public string UnitTitle { get; set; }
    }
}