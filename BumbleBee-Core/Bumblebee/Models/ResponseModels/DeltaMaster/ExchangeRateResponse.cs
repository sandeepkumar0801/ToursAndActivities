namespace WebAPI.Models.ResponseModels.DeltaMaster
{
	public class ExchangeRateResponse
	{
		public string FromCurrency { get; set; }

		public string ToCurrency { get; set; }

		public decimal ExchangeRateValue { get; set; }
	}
}