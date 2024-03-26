namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class Seating : EntityBase
    {
        public int Seat { get; set; }

        public string Gate { get; set; }

        public string Row { get; set; }

        public string EntranceDoor { get; set; }

        public int PaxId { get; set; }
    }
}