namespace Isango.Entities.Booking
{
    public class AlternativeTransactionResponse
    {
        public string id { get; set; }
        public string type { get; set; }
        public string resourceName { get; set; }
        public AlternativeTransactionResource resource { get; set; }
        public string created { get; set; }
    }
}