using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Rezdy
{
    public class RezdyCriteria : Criteria
    {
        public List<string> ProductCodes { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string SupplierName { get; set; }
        public string Currency { get; set; }
        public List<RezdyPassengerMapping> PassengerMappings { get; set; }
        public List<RezdyPaxMapping> RezdyPaxMappings { get; set; }

        public string Dumping { get; set; }

        public decimal CommissionPercent { get; set; }
        public bool IsCommissionPercent { get; set; }

        public Int32 Limit { get; set; }
        public Int32 OffSet { get; set; }
    }
}