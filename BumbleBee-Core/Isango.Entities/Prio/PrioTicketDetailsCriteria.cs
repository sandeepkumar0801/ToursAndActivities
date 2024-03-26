using Isango.Entities.Activities;

namespace Isango.Entities.Prio
{
    public class PrioTicketDetailsCriteria
    {
        public Activity Activity { get; set; }

        public string DistributorId { get; set; }

        public string TokenKey { get; set; }

        public Criteria ApiCriteria { get; set; }

        public string Token { get; set; }
    }
}