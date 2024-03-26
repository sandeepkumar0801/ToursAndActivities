namespace Isango.Entities.Booking
{
    public class AlternativeTransactionPayment
    {
        public string id { get; set; }
        public string mode { get; set; }
        public string paymentOption { get; set; }
        public string customerId { get; set; }
        public string holder { get; set; }
        public string created { get; set; }
    }
}