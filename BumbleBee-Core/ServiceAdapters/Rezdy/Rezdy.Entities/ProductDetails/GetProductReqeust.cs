namespace ServiceAdapters.Rezdy.Rezdy.Entities.ProductDetails
{
    public class GetProductReqeust
    {
        public string ProductCode { get; set; }

        public string SupplierAlias { get; set; }

        public int SupplierId { get; set; }

        public int Limit { get; set; }

        public int Offset { get; set; }
    }
}