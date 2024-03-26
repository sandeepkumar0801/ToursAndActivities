using System;

namespace Isango.Entities.ConsoleApplication.AgeGroup.GrayLineIceLand
{
    public class ActivityPickupLocation
    {
        public int ActivityPickupLocationId { get; set; }
        public int ActivityId { get; set; }
        public int PickupLocationId { get; set; }
        public DateTime? PickupTime { get; set; }
        public int TimePUMinutes { get; set; }
    }
}