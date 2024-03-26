using System;

namespace Isango.Entities.Ventrata
{
    public class PickupPointsDetailsForVentrata {
        public int RandomIntegerId { get; set; }
        public string Id { get; set; }
        public string PickupPointAddress { get; set; }
        public string LocalDateTime { get; set; }
        public string GooglePlaceId { get; set; }
    }

    public class MeetingPointDetails {
        public string MeetingPointAddresses { get; set; }
        //TODO Maybe an object. No product available yet
        public string MeetingPointCoordinates { get; set; }
        public string TimeAndDates { get; set; }
    }

    public class OpeningHours {
        public string Date { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
