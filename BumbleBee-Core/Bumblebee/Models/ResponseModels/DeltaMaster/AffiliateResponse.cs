using System;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels.DeltaMaster
{
	public class AffiliateResponse
	{
		public string DisplayName { get; set; }
		public string AffiliateID { get; set; }
		public string AffiliateName { get; set; }
		public string SupportedLanguages { get; set; }
		public string GoogleTrackerID { get; set; }
		public string GoogleTrackerLabel { get; set; }
		public string Email { get; set; }
		public int AffiliateGroupID { get; set; }
		public string Alias { get; set; }
		public string GoogleConversionId { get; set; }
		public bool IsmultiSave { get; set; }
		public Int16 MultiSavePercent { get; set; }
		public string Google_TagManager { get; set; }
		public int LOB { get; set; }
		public string CompanyWebsite { get; set; }
		public string B2BAffiliateID { get; set; }
		public double? DiscountPercent { get; set; }
		public List<ServiceidLanguageCode> ServiceidLanguageCode { get; set; }
		public List<EmailPhoneLanguageWiseResponse> EmailPhoneLanguageWise { get; set; }
		public List<string> SupportedCurrency { get; set; }
		public bool WhiteLabelPartner { get; set; }
	}
}