namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels
{
    public class ToursAvailabilityRQ
    {
        public Tour[] Tours { get; set; }
        public string CurrencyCode { get; set; }
        public bool MakeBooking { get; set; }
        public Contactinformation ContactInformation { get; set; }
        public int AgentProfileId { get; set; }
        public int Source { get; set; }
        public bool SuppressPrices { get; set; }
        public bool IncludeBookingDetails { get; set; }
        public int Language { get; set; }
        public bool ReducePickupDetails { get; set; }
    }

    public class Contactinformation
    {
    }

    public class Tour
    {
        public string TourNumber { get; set; }
        public string FromDeparture { get; set; }
        public string UntilDeparture { get; set; }
        public Passenger[] Passengers { get; set; }
        public object[] AdditionalInfo { get; set; }
        public int Dimension { get; set; }
    }
}