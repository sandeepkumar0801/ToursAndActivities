using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels.DeltaMaster
{
	public class EmailPhoneLanguageWiseResponse
	{
		public string LanguageCode { get; set; }
		public string Email { get; set; }
		public List<PhoneResponse> Phone { get; set; }
	}
}