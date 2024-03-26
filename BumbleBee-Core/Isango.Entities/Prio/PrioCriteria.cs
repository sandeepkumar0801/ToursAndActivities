using System.Collections.Generic;

namespace Isango.Entities.Prio
{
    public class PrioCriteria : Criteria
    {
        public string ActivityCode { get; set; }
        public List<string> SupplierOptionCodes { get; set; }
    }
}