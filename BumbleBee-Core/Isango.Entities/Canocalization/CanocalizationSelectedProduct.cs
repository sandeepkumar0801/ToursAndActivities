using System.Collections.Generic;

namespace Isango.Entities.Canocalization
{
    public class CanocalizationSelectedProduct : SelectedProduct
    {
        #region REDEAM V1.2 Request Property
        public Dictionary<string, string> PriceId { get; set; }
        public string RateId { get; set; }
        public string SupplierId { get; set; }
        public string RedeamAvailabilityId { get; set; }
        public string RedeamAvailabilityStart { get; set; }
        public string SupplierEmail { get; set; }
        #endregion REDEAM V1.2 Request Property

        #region  REDEAM V1.2 Response Property

        public string HoldId { get; set; }
        public string HoldStatus { get; set; }
        public string BookingReferenceNumber { get; set; }
        public List<QrCode> QrCodes { get; set; }

        #endregion REDEAM V1.2 Response Property

       #region GlobalTix 
        public ApiExtraDetail APIDetails { get; set; }
        public List<ContractQuestion> ContractQuestions { get; set; }
        #endregion
    }

    public class QrCode
    {
        public string PassengerType { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}