using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
	public class ActivityCriteria
	{
		[Required]
		[RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Invalid ActivityId")]
		public int ActivityId { get; set; }

		[Required]
		public string LanguageCode { get; set; }

		//[Required]
		public string AffiliateId { get; set; }

		[Required]
		public string CurrencyIsoCode { get; set; }

		public string? CountryIp { get; set; }
        public string? TokenId { get; set; }

    }
}