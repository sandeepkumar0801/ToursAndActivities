using System.Collections.Generic;

namespace Isango.Entities.Tiqets
{
    public class TiqetsCriteria : Criteria
    {
        public int ProductId { get; set; }
        public string Language { get; set; }
        public string TimeSlot { get; set; }
        public int OptionId { get; set; }
        public string OptionName { get; set; }
        public List<TiqetsPaxMapping> TiqetsPaxMappings { get; set; }

        public int PageNumber { get; set; }
        public string AffiliateId { get; set; }
    }
}