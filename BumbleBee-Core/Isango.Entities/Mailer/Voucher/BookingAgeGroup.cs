namespace Isango.Entities.Mailer.Voucher
{
    public class BookingAgeGroup
    {
        public string BookedOptionId { get; set; }
        public string AgeGroupDesc { get; set; }
        public int PaxCount { get; set; }
        public decimal? PaxSellAmount { get; set; }
        public decimal? PaxSupplierCostAmount { get; set; }
        public int PassengerTypeId { get; set; }
        public string PassengerType { get; set; }
    }
}