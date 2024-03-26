using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.v1Css
{
    public class CancellationResponse
    {
        public Status Status { get; set; }
        public string Remark { get; set; }
    }

    public class Status
    {
        public string Message { get; set; }
        public AllCancelStatus AllCancelStatus { get; set; }
    }

    public class AllCancelStatus
    {
        public string IsangoBookingCancel { get; set; }
        public string SupplierBookingCancel { get; set; }
        public string PaymentRefundStatus { get; set; }
    }
}
