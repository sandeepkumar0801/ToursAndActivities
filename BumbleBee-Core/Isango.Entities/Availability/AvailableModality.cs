using System;

namespace Isango.Entities.Availability
{
    public class AvailableModality
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal AdultPrice { get; set; }
        public decimal ChildPrice { get; set; }
        public DateTime AvailableOn { get; set; }
        public int MinDuration { get; set; }
        public int MaxDuration { get; set; }
        public int ChildFromAge { get; set; }
        public int ChildToAge { get; set; }
    }
}