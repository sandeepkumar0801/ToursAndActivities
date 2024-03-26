using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
	public class SearchRequestCriteria
	{
		public int RegionId { get; set; }
        public string LanguageCode { get; set; }
		[Required]
		public string AffiliateId { get; set; }
		[Required]
		public string CountryIp { get; set; }

		public string CurrencyIsoCode { get; set; }
        public int CategoryId { get; set; }
        public string KeyWord { get; set; }
    }
}