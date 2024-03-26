namespace WebAPI.Models.ResponseModels
{
	public class CurrencyExchangeRateResponse
	{
		public string FromCurrency { get; set; }
		public string ToCurrency { get; set; }
		public decimal ExchangeRateValue { get; set; }
	}
}