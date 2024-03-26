using System.Collections.Generic;

namespace Isango.Entities.Redeam
{
    public class RedeamCriteria : Criteria
    {
        public string SupplierId { get; set; }
        public string ProductId { get; set; }
        public List<string> RateIds { get; set; }
        public string RateId { get; set; }
        public string Quantity { get; set; }
        public Dictionary<string, string> RateIdAndType { get; set; }
        public int ServiceOptionId { get; set; }
        public string ApiToken { get; set; }
    }
}