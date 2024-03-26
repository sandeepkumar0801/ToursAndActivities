using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
	/// <summary>
	/// Request model for the Apply Dicount API call
	/// </summary>
	public class ProcessDiscountRequest
	{
		/// <summary>
		/// UTMParameter
		/// </summary>
		public string UTMParameter { get; set; }

		/// <summary>
		/// Currency selected from the UI
		/// </summary>
		[Required]
		public string CurrencyIsoCode { get; set; }

		/// <summary>
		/// AffiliateId
		/// </summary>
		[Required]
		public string AffiliateId { get; set; }

		/// <summary>
		/// Customer's email
		/// </summary>
		public string CustomerEmail { get; set; }

		/// <summary>
		/// Coupons applied by the cutomer
		/// </summary>
		public List<string> DiscountCoupons { get; set; }

		/// <summary>
		/// ReferenceIds of the table storage
		/// </summary>
		[Required]
		public List<string> AvailabilityReferenceIds { get; set; }

		/// <summary>
		/// TokenId 
		/// </summary>
		[Required]
		public string TokenId { get; set; }

		/// <summary>
		/// Customer selected language 
		/// </summary>
		public string LanguageCode { get; set; }
	}
}