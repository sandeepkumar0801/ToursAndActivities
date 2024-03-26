namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class Customer : EntityBase
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
    }
}