using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.v1Css
{
    public class CancellationRequestB2C
    {
        public string BookingRefNo { get; set; }
        public string UserName { get; set; }
        public string TokenId { get; set; }
        public CancellationParameters CancellationParameters { get; set; }
        public bool IsBookingManager { get; set; }
    }
    public class CancellationParameters
    {
        public int BookedOptionId { get; set; }
        public decimal UserRefundAmount { get; set; }
        //public string SellingPrice { get; set; }
        //public string TotalAmount { get; set; }
        public string UserCurrencyCode { get; set; }
        public string Reason { get; set; }
        public string AlternativeTours { get; set; }
        public string SupplierNotes { get; set; }
        public string CustomerNotes { get; set; }
        public string[] AlternativeDates { get; set; }
        public string RequestedBy { get; set; }
    }
}
