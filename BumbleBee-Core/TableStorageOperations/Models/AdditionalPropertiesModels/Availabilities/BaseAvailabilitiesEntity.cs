namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class BaseAvailabilitiesEntity : CustomTableEntity
    {
        public decimal BasePrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal GateBasePrice { get; set; }
        public string AvailabilityStatus { get; set; }
        public string SupplierOptionCode { get; set; }
        public string CurrencyCode { get; set; }
        public int ActivityId { get; set; }
        public int OptionId { get; set; }
        public int ServiceOptionId { get; set; }
        public int ApiType { get; set; }
        public decimal Margin { get; set; }
        public bool OnSale { get; set; }
        public string UnitType { get; set; }
        public string PriceOfferReferenceId { get; set; }
        public string BasePricingUnits { get; set; }
        public string CostPricingUnits { get; set; }
        public string GateBasePricingUnits { get; set; }
        public string TravelInfo { get; set; }
        public string OptionName { get; set; }
        public virtual string RateKey { get; set; }
        public string Variant { get; set; }
        public string TimeSlot { get; set; }
        public string EndTimeSlot { get; set; }

        public string LanguageCode { get; set; }

        /// <summary>
        /// Serialized Api Cancellation object (Processed)
        /// </summary>
        public string CancellationPrices { get; set; }

        /// <summary>
        /// Serialized Api Cancellation Policy (Original)
        /// </summary>
        public string ApiCancellationPolicy { get; set; }

        /// <summary>
        /// Cancellation Policy Text (Processed)
        /// </summary>
        public string CancellationText { get; set; }

        /// <summary>
        /// Serialized List of Isango.Entities.ContractQuestion
        /// </summary>
        public string ContractQuestions { get; set; }

        #region Bundle Related Properties

        public int ComponentOrder { get; set; }
        public int ComponentServiceID { get; set; }
        public int PriceTypeID { get; set; }
        public int BundleOptionID { get; set; }
        public string BundleOptionName { get; set; }
        public bool IsSameDayBookable { get; set; }

        #endregion Bundle Related Properties
        
    }
}