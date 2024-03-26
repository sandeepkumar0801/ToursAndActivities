using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels.CheckAvailability
{
	public class Price
	{
		public virtual string CurrencyIsoCode { get; set; }

		public virtual List<PriceAndAvailability> PriceAndAvailabilities { get; set; }
	}
}