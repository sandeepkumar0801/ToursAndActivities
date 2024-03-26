using Isango.Entities.Enums;
using System.Collections.Generic;

namespace Isango.Entities.GrayLineIceLand
{
    public class GrayLineIceLandSelectedProduct : SelectedProduct
    {
        public Dictionary<PassengerType, int> PaxAgeGroupIds { get; set; }

        public AvailabilityStatus AvailabilityStatus { get; set; }

        public int AdultPriceId { get; set; }

        public PickupPointDetails[] PickupPointDetails { get; set; }

        public int CategoryId { get; set; }

        public int BlocId { get; set; }

        public int ReservationId { get; set; }

        public string Code { get; set; }

        public string Destination { get; set; }

        public int FactsheetId { get; set; }

        public string HotelPickup { set; get; }

        public string BookingReferenceNumber { get; set; }
    }
}