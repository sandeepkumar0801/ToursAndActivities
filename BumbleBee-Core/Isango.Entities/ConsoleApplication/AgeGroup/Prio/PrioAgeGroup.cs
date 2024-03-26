using System;

namespace Isango.Entities.ConsoleApplication.AgeGroup.Prio
{
    public class PrioAgeGroup
    {
        public int Id { get; set; }
        public int? TicketId { get; set; }
        public string Description { get; set; }
        public int FromAge { get; set; }
        public int ToAge { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
