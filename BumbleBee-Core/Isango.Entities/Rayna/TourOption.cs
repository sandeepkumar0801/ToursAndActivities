namespace Isango.Entities.Rayna
{
    public class TourOption
    {
        public int TourId { get; set; }
        public int TourOptionId { get; set; }
        public string OptionName { get; set; }
        public string ChildAge { get; set; }
        public string InfantAge { get; set; }
        public string OptionDescription { get; set; }
        public string CancellationPolicy { get; set; }
        public string CancellationPolicyDescription { get; set; }
        public string ChildPolicyDescription { get; set; }
        public string Xmlcode { get; set; }
        public string Xmloptioncode { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public int MinPax { get; set; }
        public int MaxPax { get; set; }
        public string Duration { get; set; }
        public string TimeZone { get; set; }
        public bool IsWithoutAdult { get; set; }
        public int IsTourGuide { get; set; }
        public bool CompulsoryOptions { get; set; }
        public bool HourlyBasisTypeTour { get; set; }
        public bool IsHideRateBreakup { get; set; }
    }
}
