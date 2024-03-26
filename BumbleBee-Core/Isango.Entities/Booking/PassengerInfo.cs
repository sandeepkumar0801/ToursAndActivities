namespace Isango.Entities.Booking
{
    public class PassengerInfo
    {
        public int ActivityId { get; set; }
        public int PassengerTypeId { get; set; }
        public int FromAge { get; set; }
        public int ToAge { get; set; }
        public int MinSize { get; set; }
        public int MaxSize { get; set; }
        public string PaxDesc { get; set; }
        public bool IndependablePax { get; set; }
        public string Label { get; set; }
        public string MeasurementDesc { get; set; }
        //Added for BackGround compatibility:ageGroupId, remove when JF1 is completely remove from everywhere.
        public int AgeGroupId { get; set; }
    }
}