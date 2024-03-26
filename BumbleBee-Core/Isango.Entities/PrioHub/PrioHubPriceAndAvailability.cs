using System;
using System.Collections.Generic;

namespace Isango.Entities.PrioHub
{
    public class PrioHubPriceAndAvailability : PriceAndAvailability
    {
        public string Name { get; set; }
        public string FromDateTime { get; set; }
        public string ToDateTime { get; set; }
        public string Vacancies { get; set; }
        public string AvailabilityId { get; set; }
        public string AvailabilityCapacityId { get; set; }
        public bool AvailabilityActive { get; set; }
        public string AvailabilityProductId { get; set; }
        public string AvailabilityAdmissionType { get; set; }
        public string AvailabilityFromDateTime { get; set; }
        public string AvailabilityToDateTime { get; set; }
        public AvailabilitySpots AvailabilitySpots { get; set; }
        public DateTime AvailabilityCreated { get; set; }
        public DateTime AvailabilityModified { get; set; }
        public List<PrioHubAvailabilityPricing> PrioHubAvailabilityPricing { get; set; }
     }


    public class AvailabilitySpots
    {
        public int AvailabilitySpotsTotal { get; set; }
        public int AvailabilitySpotsReserved { get; set; }
        public int AvailabilitySpotsBooked { get; set; }
        public int AvailabilitySpotsOpen { get; set; }
    }

    public class PrioHubAvailabilityPricing
    {
        public decimal AvailabilityPricingVariationAmount { get; set; }
        public string AvailabilityPricingVariationPriceType { get; set; }
        public int AvailabilityPricingVariationProductTypeId { get; set; }
        public string AvailabilityPricingVariationType { get; set; }
        public bool AvailabilityPricingVariationProductTypeDiscountIncluded { get; set; }
        public bool AvailabilityPricingVariationCommissionIncluded { get; set; }

        public string AvailabilityPricingVariationPercentage { get; set; }
    }
}