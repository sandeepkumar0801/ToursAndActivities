namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class Passenger
    {
        public int OrderDetailId { get; set; }
        public float Quantity { get; set; }
        public int AgeGroup { get; set; }
        public string AgeGroupDescription { get; set; }
        public int CheckInStatus { get; set; }
        public int PickupStatus { get; set; }
        public string ExternalReference { get; set; }
        public string[] Notes { get; set; }
        public int NumberOfPax { get; set; }
    }

    public class Payment
    {
        public int PaymentType { get; set; }
        public string Reference { get; set; }
        public float Amount { get; set; }
        public string CardNumber { get; set; }
        public string Cvc { get; set; }
        public int ValidUntilMonth { get; set; }
        public int ValidUntilYear { get; set; }
        public string ChargeDescription { get; set; }
        public string AuthorisationCode { get; set; }
        public string CurrencyCode { get; set; }
        public float ActualAmount { get; set; }
        public int KioskId { get; set; }
    }
}