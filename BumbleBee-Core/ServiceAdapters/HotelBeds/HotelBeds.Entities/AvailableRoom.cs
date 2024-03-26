namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class AvailableRoom : EntityBase
    {
        public HotelOccupancy HotelOccupancy { get; set; }
        public HotelRoom HotelRoom { get; set; }
    }
}