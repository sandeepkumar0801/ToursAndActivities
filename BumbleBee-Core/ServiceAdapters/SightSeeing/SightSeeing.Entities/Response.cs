namespace ServiceAdapters.SightSeeing.SightSeeing.Entities
{
    public class Response
    {
        public string TicketId { get; set; }
        public int Prog { get; set; }
        public string Pnr { get; set; }
        public int Price { get; set; }
        public int UnitPrice { get; set; }
        public string QrCode { get; set; }
        public string QrCodeImg { get; set; }
        public string QrCodeText { get; set; }
    }
}