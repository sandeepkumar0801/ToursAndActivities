using System;
using System.Collections.Generic;

namespace Isango.Entities.Bokun
{
    public class BokunAgeGroupWrapper
    {
        public List<Product> Products { get; set; }
        public List<CancellationPolicy> CancellationPolicies { get; set; }
        public List<Rate> Rates { get; set; }
        public List<BookableExtras> BookableExtras { get; set; }
    }

    public class Product
    {
        public int ProductId { get; set; }
        public Nullable<int> IsangoServiceID { get; set; }
        public Nullable<int> VendorId { get; set; }
        public string VendorTitle { get; set; }
        public string VendorCurrency { get; set; }
        public Nullable<bool> PassporTrequired { get; set; }
        public string Inclusions { get; set; }
        public string Exclusions { get; set; }
        public string Attention { get; set; }
        public int PriceCategoryId { get; set; }
        public string PriceCategoryTitle { get; set; }
        public string PriceCategoryTicketCategory { get; set; }
        public string PriceCategoryFullTitle { get; set; }
        public Nullable<int> ActivityMinAge { get; set; }
        public Nullable<int> PriceCategoryMinAge { get; set; }
        public Nullable<int> PriceCategoryMaxAge { get; set; }
        public Nullable<bool> PriceCategoryAgeQualified { get; set; }
        public Nullable<int> PriceCategoryOccupancy { get; set; }
        public Nullable<bool> PriceCategoryDependent { get; set; }
        public Nullable<int> PriceCategoryMasterCategoryId { get; set; }
        public Nullable<int> PriceCategoryMaxPerMaster { get; set; }
        public Nullable<bool> PriceCategorySumDependentCategories { get; set; }
        public Nullable<int> PriceCategoryMaxDependentSum { get; set; }
        public Nullable<bool> PriceCategoryInternalUseOnly { get; set; }
        public Nullable<bool> PriceCategoryDefaultCategory { get; set; }
        public string BookingType { get; set; }
    }

    public class CancellationPolicy
    {
        public int IsangoServiceID { get; set; }
        public int APIServiceId { get; set; }
        public int CancellationPolicyId { get; set; }
        public string Title { get; set; }
        public int PenaltyRuleId { get; set; }
        public Nullable<int> CutOffHours { get; set; }
        public Nullable<decimal> Charge { get; set; }
        public string ChargeType { get; set; }
    }

    public class Rate
    {
        /// <summary>
        /// Isango service id
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// isango service option id
        /// </summary>
        public int ServiceOptionId { get; set; }

        /// <summary>
        /// bokun activity id
        /// </summary>
        public int SupplierActivityId { get; set; }

        /// <summary>
        /// isango product mapping prefix code , mapped to bokun rate id
        /// </summary>
        public int MappedRateId { get; set; }

        /// <summary>
        /// RateId being dumped in do to see changes if any
        /// </summary>
        public int RateId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string StartTimeIds { get; set; }
        public string PricingCategoryIds { get; set; }
        public int MinPerBooking { get; set; }
        public int MaxPerBooking { get; set; }
        //public int Index { get; set; }
        //public bool PricedPerPerson { get; set; }
        //public int PassValidForDays { get; set; }
        //public string PickupSelectionType { get; set; }
        //public string PickupPricingType { get; set; }
        //public bool PickupPricedPerPerson { get; set; }
        //public string DropoffSelectionType { get; set; }
        //public string DropoffPricingType { get; set; }
        //public bool DropoffPricedPerPerson { get; set; }

        //public bool AllStartTimes { get; set; }
        //public bool TieredPricingEnabled { get; set; }

        //public bool AllPricingCategories { get; set; }

        //#region arrays/object to be saved in db as serialized string

        //public string RateCode { get; set; }
        //public string FixedPassExpiryDate { get; set; }

        //public string Tiers { get; set; }
        //public string Details { get; set; }

        //#endregion arrays/object to be saved in db as serialized string
    }

    public class ExtraConfigs
    {
        public int? ServiceID { get; set; }
        public int? ServiceOptionID { get; set; }
        public string ActivityExtraId { get; set; }
        public string SelectionType { get; set; }
        public bool? IsPricedPerPerson { get; set; }
    }

    public class BookableExtras
    {
        public string SelectionType { get; set; }
        public bool? IsPricedPerPerson { get; set; }
        public int? ServiceID { get; set; }
        public int? ServiceOptionID { get; set; }
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public string Title { get; set; }
        public string Information { get; set; }
        public bool? Included { get; set; }
        public bool? Free { get; set; }
        public string ProductGroupId { get; set; }
        public string PricingType { get; set; }
        public string PricingTypeLabel { get; set; }
        public string Price { get; set; }
        public bool? IncreasesCapacity { get; set; }
        public string MaxPerBooking { get; set; }
        public bool? LimitByPax { get; set; }
        public string Flags { get; set; }
        public List<BookableExtraQuestions> Questions { get; set; }
    }

    public class BookableExtraQuestions
    {
        public string Id { get; set; }
        public bool? Active { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Options { get; set; }
        public bool? AnswerRequired { get; set; }
        public string Flags { get; set; }
    }
}