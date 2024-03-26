namespace Isango.Entities.Region
{
    public class TicketByRegion
    {
        public string CountryCode { get; set; }
        public ThemeparkTicket ThemeparkTicket { get; set; }
    }

    public class ThemeparkTicket
    {
        public int ProductId { get; set; }
        public int City { get; set; }
        public int Region { get; set; }
        public int Country { get; set; }
    }
}