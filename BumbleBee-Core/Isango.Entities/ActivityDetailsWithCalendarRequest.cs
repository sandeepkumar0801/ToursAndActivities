namespace Isango.Entities
{
    public class ActivityDetailsWithCalendarRequest
    {
        public int ActivityId { get; set; }
        public ClientInfo ClientInfo { get; set; }
        public Criteria Criteria { get; set; }
        public int RegionId { get; set; }
        public string AffiliateId { get; set; }
    }
}