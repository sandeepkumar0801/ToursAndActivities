namespace Isango.Entities.FareHarbor
{
    public class FareHarborSelectedProduct : SelectedProduct
    {
        public string UuId { get; set; }

        public int AdultPriceId { get; set; }

        public int ChildPriceId { get; set; }

        public int InfantPriceId { get; set; }

        public string Code { get; set; }

        public string Destination { get; set; }

        public int FactsheetId { get; set; }

        public string ActivityPleaseNote { get; set; }

        public string BookingReferenceNumber { get; set; }

        public int ReservationId { get; set; }
    }
}