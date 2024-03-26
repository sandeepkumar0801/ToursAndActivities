using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
	public class CancellationPolicyRequest
	{
		[Required]
		[RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Invalid ActivityId")]
		public int ActivityId { get; set; }
		[Required]
		[RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Invalid ServiceOptionId")]
		public int ServiceOptionId { get; set; }
		[Required]
		public string LanguageCode { get; set; }
		[Required]
		public string AffiliateId { get; set; }
		[Required]
		public string TokenId { get; set; }
	}
}