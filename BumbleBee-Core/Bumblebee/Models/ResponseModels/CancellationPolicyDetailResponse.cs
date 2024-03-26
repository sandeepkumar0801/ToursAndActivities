namespace WebAPI.Models.ResponseModels
{
    public class CancellationPolicyDetailResponse
    {
        public decimal UserRefundAmount { get; set; }
        public string UserCurrencyCode { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SellingPrice { get; set; }

        public string CancellationDescription { get; set; }
    }
}