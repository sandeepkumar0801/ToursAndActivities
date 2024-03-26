namespace Isango.Entities
{
    public class AgeGroupDescription
    {
        public int Id { get; set; }

        public string Description { get; set; }
        public int PaxCount { get; set; }

        public string PassengerType { get; set; }

        public int FromAge { get; set; }
        public int ToAge { get; set; }
        public decimal? PaxSellAmount { get; set; }
        public decimal? PaxSupplierCostAmount { get; set; }
    }

    public class PaxWisePdfEntity {
        public int BookedOptionId { get; set; }
        public string PassengerType { get; set; }
        public string CodeValue { get; set; }

    }
}