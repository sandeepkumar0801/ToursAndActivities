namespace Isango.Entities.MoulinRouge
{
    public class MoulinRougePriceAndAvailability : PriceAndAvailability
    {
        public MoulinRougePriceAndAvailability()
        {
            MoulinRouge = new APIContextMoulinRouge();
        }

        public APIContextMoulinRouge MoulinRouge { get; set; }

        public int CatalogDateID { get; set; }
    }
}