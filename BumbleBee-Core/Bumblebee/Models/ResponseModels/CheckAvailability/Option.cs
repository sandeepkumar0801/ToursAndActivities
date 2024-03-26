using Isango.Entities;
using Isango.Entities.Enums;
using Newtonsoft.Json;

namespace WebAPI.Models.ResponseModels.CheckAvailability
{
    public class Option
    {
        /// <summary>
        /// Dynamically Generated Id when multiple option mapped with single option in case of multiple availability in same day, else its same as ServiceOptionId.
        /// </summary>
		public virtual int Id { get; set; }

        public virtual int BundleOptionID { get; set; }

        public virtual int ServiceOptionId { get; set; }
        public virtual string OptionName { get; set; }

        //[JsonIgnore]
        //public virtual string? ReferenceId { get; set; }

        public virtual int OptionOrder { get; set; }
        public virtual string Description { get; set; }

        [JsonIgnore]
        public virtual string? AvailabilityStatus { get; set; }

        /// <summary>
        /// Sell Price in supplier currency after discount if any
        /// </summary>
        public virtual Price BasePrice { get; set; }

        /// <summary>
        /// Sell Price is supplier currency without any discount
        /// </summary>
        public virtual Price GateBasePrice { get; set; }

        [JsonIgnore]
        public virtual int ComponentServiceID { get; set; }

        [JsonIgnore]
        public virtual bool? IsCapacityCheckRequired { get; set; }

        [JsonIgnore]
        public virtual int? Capacity { get; set; }

        public virtual string? Variant { get; set; }
        public virtual TimeSpan? StartTime { get; set; }
        public virtual TimeSpan? EndTime { get; set; }
        public virtual string CancellationPolicy { get; set; }
        public virtual PickUpDropOffOptionType PickupOptionType { get; set; }

        public virtual BundleDetails BundleDetails { get; set; }
        
    }

    public class BundleDetails
    {
        public int BundleOptionID { get; set; }
        public int ComponentServiceID { get; set; }
        public string BundleOptionName { get; set; }
        public decimal BasePrice { get; set; }
        public decimal GateBasePrice { get; set; }

        [JsonIgnore]
        public string BundleOptionReferenceIds { get; set; }

        public string CurrencyIsoCode { get; set; }
        public int BundleOptionOrder { get; set; }

        [JsonIgnore]
        public TimeSpan? StartTime { get; set; }

        [JsonIgnore]
        public TimeSpan? EndTime { get; set; }

        [JsonIgnore]
        public string Variant { get; set; }
    }

    #region B2C CitySightSeeing

    public class B2C_Option
    {
        /// <summary>
        /// Dynamically Generated Id when multiple option mapped with single option in case of multiple availability in same day, else its same as ServiceOptionId.
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public int BundleOptionID { get; set; }

        public int ServiceOptionId { get; set; }
        public string OptionName { get; set; }

        public int OptionOrder { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public Price BasePrice { get; set; }

        /// <summary>
        /// Sell Price in supplier currency without any discount
        /// </summary>
        public B2C_Price GateBasePrice { get; set; }

        [JsonIgnore]
        public int ComponentServiceID { get; set; }

        [JsonIgnore]
        public bool IsCapacityCheckRequired { get; set; }

        [JsonIgnore]
        public int Capacity { get; set; }

        [JsonIgnore]
        public string Variant { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string CancellationPolicy { get; set; }

        [JsonIgnore]
        public PickUpDropOffOptionType PickupOptionType { get; set; }

        [JsonIgnore]
        public BundleDetails BundleDetails { get; set; }
    }

    public class B2C_Price
    {
        public string CurrencyIsoCode { get; set; }

        public List<B2C_PriceAndAvailability> PriceAndAvailabilities { get; set; }
    }

    public class B2C_PriceAndAvailability
    {
        public DateTime DateAndTime { get; set; }
        public string ReferenceId { get; set; }
        public string AvailabilityStatus { get; set; }
        public Decimal TotalPrice { get; set; }

        [JsonIgnore]
        public int Quantity { get; set; }

        public string UnitType { get; set; }
        public bool IsCapacityCheckRequired { get; set; }
        public int Capacity { get; set; }
        public List<B2C_PricingUnit> PricingUnits { get; set; }
    }

    public class B2C_PricingUnit
    {
        public string PassengerTypeName { get; set; }
        public PassengerType PassengerTypeId { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }

        [JsonIgnore]
        public decimal MinimumSellingPrice { get; set; }

        [JsonIgnore]
        public string Currency { get; set; }

        public bool IsMinimumSellingPriceRestrictionApplicable { get; set; }
    }

    #endregion B2C CitySightSeeing
}