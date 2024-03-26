namespace Isango.Entities.MyIsango
{
    public class MyPaxPriceInfo
    {
        public string Subject { get; set; }

        public int BookingId { get; set; }
        public int BookedOptionId { get; set; }

        public decimal BookedPassengerRateSellAmount { get; set; }
        public decimal BookedPassengerRateOriginalSellAmount { get; set; }
        public string PassengerType { get; set; }

        public int PassengerTypeId { get; set; }
        public int PassengerCount { get; set; }
    }
}