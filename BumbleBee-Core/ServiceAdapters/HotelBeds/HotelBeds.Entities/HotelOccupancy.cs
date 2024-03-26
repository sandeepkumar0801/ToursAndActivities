using System.Collections.Generic;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class HotelOccupancy : EntityBase
    {
        public int RoomCount { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public List<Guest> Guests { get; set; }
    }
}