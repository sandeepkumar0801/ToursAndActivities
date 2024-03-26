namespace Isango.Entities.HotelBeds
{
    public class MappedRegion
    {
        public string RegionName { get; set; }
        public string DestinationCode { get; set; }
        public int RegionId { get; set; }

        public MappedRegion()
        {
        }

        public MappedRegion(int id, string name, string code)
        {
            RegionId = id;
            RegionName = name;
            DestinationCode = code;
        }
    }
}