namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class Paxes : EntityBase
    {
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public GuestList GuestList { get; set; }
    }
}