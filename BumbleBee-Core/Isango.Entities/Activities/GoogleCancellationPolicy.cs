using System.Collections.Generic;

namespace Isango.Entities.Activities
{
    public class GoogleCancellationPolicy
    {
        public int ActivityId { get; set; }
        public int OptionId { get; set; }
        public List<GoogleCancellationPrice> CancellationPrices { get; set; }
    }

    public class GoogleCancellationPrice
    {
        public string CutoffHours { get; set; }
        public decimal CancellationCharge { get; set; }
        public bool IsPercentage { get; set; }
    }
}