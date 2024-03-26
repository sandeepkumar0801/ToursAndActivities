using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.v1Css
{
    public class CancellationPolicyEntitieResponse
    {
        public decimal UserRefundAmount { get; set; }
        public string UserCurrencyCode { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SellingPrice { get; set; }

        public string CancellationDescription { get; set; }
    }
}
