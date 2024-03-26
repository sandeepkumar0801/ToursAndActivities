namespace Isango.Entities.Booking
{
    public class AlternativeTransactionResource
    {
        public string id { get; set; }
        public string mode { get; set; }
        public string status { get; set; }
        public int MyProperty { get; set; }
        public AlternativeTransactionCustomer customer { get; set; }
        public AlternativeTransactionPayment payment { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string created { get; set; }
    }
}