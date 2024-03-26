using Isango.Entities.Enums;

namespace WebAPI.Models.ResponseModels.CheckAvailability
{
    public class PricingUnit
    {
        public virtual string PassengerTypeName { get; set; }
        public virtual PassengerType PassengerTypeId { get; set; }
        public virtual decimal Price { get; set; }
        public virtual int Count { get; set; }

        public virtual decimal MinimumSellingPrice
        { get; set; }

        public virtual string Currency
        { get; set; }

        public virtual bool IsMinimumSellingPriceRestrictionApplicable { get; set; }

        
    }
}