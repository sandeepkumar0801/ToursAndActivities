using Isango.Entities.Activities;
using Isango.Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Isango.Entities
{
    //[BsonIgnoreExtraElements]
    public class ProductOption : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public APIType ApiType { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// This is the name that typically is used for supplier communications
        /// </summary>
        /// ToDo: Entity discussion with Prashant (Please check the reference)
        public string SupplierName { get; set; }

        public string Description { get; set; }
        public Price SellPrice { get; set; }
        public Price BasePrice { get; set; }
        public Price CostPrice { get; set; }
        public Price GateBasePrice { get; set; }
        public Price GateSellPrice { get; set; }
        public AvailabilityStatus AvailabilityStatus { get; set; }
        public OptionBookingStatus BookingStatus { get; set; }
        public bool IsSelected { get; set; }
        public string OptionKey { get; set; }
        public int Capacity { get; set; }
        public int Quantity { get; set; }
        public string SupplierOptionCode { get; set; }
        public List<Customer> Customers { get; set; }
        public TravelInfo TravelInfo { get; set; }
        public Margin Margin { get; set; }
        public decimal CommisionPercent { get; set; }
        public List<PriceOffer> PriceOffers { get; set; }

        /// <summary>
        /// Populated cancellation policy text (Processed)
        /// </summary>
        public string CancellationText { get; set; }

        /// <summary>
        /// This is transformed cancellation policy in case of API (Processed)
        /// </summary>
        public List<CancellationPrice> CancellationPrices { get; set; }

        /// <summary>
        /// This is original Api cancellation policy, as it is returned by API (Original in Serialized Form)
        /// </summary>
        public string ApiCancellationPolicy { get; set; }

        public bool ShowSale { get; set; }
        public int ServiceOptionId { get; set; }

        /// <summary>
        /// This field contains Hotelbeds Apitude api activity code i.e "E-U10-SANGO"
        /// This field can be used for additional mappings where alphanumeric api id is required (Product level mappings.)
        /// </summary>
        public string PrefixServiceCode { get; set; }

        public int OptionOrder { get; set; }
        public bool IsCapacityCheckRequired { get; set; }
        public int AllocationCapacity { get; set; }
        public List<TimesOfDay> TimesOfDays { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Variant { get; set; }

        public List<CloseOut> CloseOuts { get; set; }

        public PickUpDropOffOptionType PickUpOption { get; set; }

        public List<ActivityPickupLocations> OptionPickupLocations { get; set; }

        public bool IsIsangoMarginApplicable { get; set; }

        #region Bundle related properties

        public int ComponentOrder { get; set; }
        public int ComponentServiceID { get; set; }
        public int PriceTypeID { get; set; }
        public int BundleOptionID { get; set; }
        public string BundleOptionName { get; set; }
        public bool IsSameDayBookable { get; set; }
        public string ComponentServiceName { get; set; }

        #endregion Bundle related properties
    }

    public class ProductOptionAvailabilty
    {
        public int ServiceId { get; set; }
        public int ServiceOptionId { get; set; }
        public int PriceDateId { get; set; }
        public DateTime PriceDate { get; set; }
        public AvailabilityStatus AvailableState { get; set; }

        public bool IsCapacityCheckRequired { get; set; }
        public int Capacity { get; set; }
    }
}