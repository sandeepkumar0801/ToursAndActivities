using System;

namespace Isango.Entities.ConsoleApplication.DataDumping
{
    public class ExtraDetail : CustomTableEntity
    {
        public string TokenId { get; set; }
        public int ActivityId { get; set; }
        public int OptionId { get; set; }
        public string Variant { get; set; }
        public TimeSpan StartTime { get; set; }
        public string Questions { get; set; }
        public string PickUpLocations { get; set; }
        public string DropOffLocations { get; set; }
    }
}