using System;

namespace Isango.Entities
{
    public class GiftVoucher : Voucher
    {
        public DateTime? SendOnDate { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverEmail { get; set; }
        public string Message { get; set; }
    }
}