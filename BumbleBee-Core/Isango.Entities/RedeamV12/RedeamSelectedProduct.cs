using System.Collections.Generic;

namespace Isango.Entities.RedeamV12
{
    public class RedeamSelectedProduct : SelectedProduct
    {
        #region Request Property

        public Dictionary<string, string> PriceId { get; set; }
        public string RateId { get; set; }
        public string SupplierId { get; set; }

        #endregion Request Property

        #region Response Property

        public string HoldId { get; set; }
        public string HoldStatus { get; set; }
        public string BookingReferenceNumber { get; set; }
        public List<QrCode> QrCodes { get; set; }

        #endregion Response Property
    }

    public class QrCode
    {
        public string PassengerType { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}