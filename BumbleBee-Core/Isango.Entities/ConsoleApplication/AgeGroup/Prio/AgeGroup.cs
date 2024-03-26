using System;

namespace Isango.Entities.ConsoleApplication.AgeGroup.Prio
{
    public class AgeGroup
    {
        public int? TicketId { get; set; }
        public string Description { get; set; }
        public int FromAge { get; set; }
        public int ToAge { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int MinCapacity { get; set; }
        public int MaxCapacity { get; set; }

    }
}