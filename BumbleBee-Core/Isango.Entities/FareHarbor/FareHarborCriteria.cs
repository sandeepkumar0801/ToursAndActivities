using System.Collections.Generic;

namespace Isango.Entities.FareHarbor
{
    public class FareHarborCriteria : Criteria
    {
        public string CompanyName { get; set; }
        public string ActivityCode { get; set; }
        public string UserKey { get; set; }
        public List<CustomerPrototype> CustomerPrototypes { get; set; }
    }
}