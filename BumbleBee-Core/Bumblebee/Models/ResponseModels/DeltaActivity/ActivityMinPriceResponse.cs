namespace WebAPI.Models.ResponseModels.DeltaActivity
{
	public class ActivityMinPriceResponse
	{
		public int Serviceid { get; set; }
		public decimal BasePrice { get; set; }

		public decimal SellPrice { get; set; }
		public decimal OfferPercent { get; set; }
		public string AffiliateID { get; set; }
	}
}