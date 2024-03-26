using Isango.Entities.Enums;

namespace Isango.Entities.CitySightseeing
{
    public class CitySightseeingSelectedProduct : SelectedProduct
    {
        public AvailabilityStatus AvailabilityStatus { get; set; }

        public string Pnr { get; set; }

        public string QrCode { get; set; }

        public string Code { get; set; }

        public string Destination { get; set; }

        public int FactsheetId { get; set; }

        public int Quantity { get; set; }
    }
}