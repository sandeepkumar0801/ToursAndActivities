using System.Collections.Generic;

namespace Isango.Entities.Aot
{
    public class AotCriteria : Criteria
    {
        public List<string> OptCode { get; set; }

        public bool CancellationPolicy { get; set; }

        public int ActivityId { get; set; }
    }
}