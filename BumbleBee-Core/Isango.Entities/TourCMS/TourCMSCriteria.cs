using Isango.Entities.TourCMS;
using System.Collections.Generic;

namespace Isango.Entities.TourCMSCriteria
{
    public class TourCMSCriteria : Criteria
    {
        public int ChannelId { get; set; }
        public int TourId { get; set; }
        public int AccountId { get; set; }
        public string ProductId { get; set; }

        public Dictionary<string, List<string>> SupplierOptionCodesAndProductIdVsApiOptionIds { get; set; }
        public List<IsangoHBProductMapping> ProductMapping { get; set; }

        public string IsangoActivityId { get; set; }

        public string ServiceOptionId { get; set; }

        public string ActivityCode { get; set; }
        public List<TourCMSMapping> TourCMSMappings { get; set; }

        public decimal CommissionPercent { get; set; }
        public bool IsCommissionPercent { get; set; }
        public int? LineOfBusinessId { get; set; }
    }
    public class PassengerUnits
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
    }
}