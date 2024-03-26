using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Tiqets
{
     public class PackageVariant
    {
        public int package_variant_id { get; set; }
        public int count { get; set; }
        public List<VisitorsDetail> visitors_details { get; set; }
    }
    public class VisitorsDetail
    {
        public string full_name { get; set; }
    }
}
