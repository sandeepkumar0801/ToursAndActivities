using System;

namespace Isango.Entities.Cancellation
{
    public class CancellationPolicyDetail
    {
        public int BookedOptionId { get; set; }
        public int BookedServiceId { get; set; }
        public int ServiceId { get; set; }
        public decimal SellingPrice { get; set; }

        public DateTime? BookedOptionInDate { get; set; }
        public DateTime? CancelDate { get; set; }

        public string CancellationChargeDescription { get; set; }

        public decimal UserCancellationCharges { get; set; }
        public decimal UserRefundAmount { get; set; }
        public string UserCurrencyCode { get; set; }
        public string ApiTypeId { get; set; }
        public int RegPaxId { get; set; }
        public string Guwid { get; set; }

        //public int TransactionId { get; set; }
        //public decimal SupplierRefundAmount { get; set; }
        //public decimal CostPrice { get; set; }
        //public decimal SupplierCancellationCharges { get; set; }
        //public string SupplierCurrencySymbol { get; set; }
        //public string SupplierCurrencyCode { get; set; }
        //public string AuthorizationCode { get; set; }
    }
}