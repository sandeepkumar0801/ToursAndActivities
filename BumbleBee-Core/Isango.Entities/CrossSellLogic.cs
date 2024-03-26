using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities
{
    public class CrossSellLogic
    {
        public int? LineofBusinessid { get; set; }
        public string LINEOFBUSINESSNAME { get; set; }
        public int? CrossSellBusiness1 { get; set; }
        public string CROSSSELLBUSINESSNAME1 { get; set; }
        public int? CrossSellBusiness2 { get; set; }
        public string CROSSSELLBUSINESSNAME2 { get; set; }
    }
}
