namespace Isango.Entities.Master
{
    public class ActivityMinPrice
    {
        public int Serviceid { get; set; }
        public decimal BasePrice { get; set; }

        public decimal SellPrice { get; set; }
        public decimal Offer_Percent { get; set; }
        public string AffiliateID { get; set; }
    }
}