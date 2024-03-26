using System.Collections.Generic;

namespace DiscountRuleEngine.Model
{
	public class DiscountCart
	{
		public List<string> Messages { get; set; }
		public bool IsMultiSaveApplied { get; set; }
		public decimal TotalPrice { get; set; }
		public decimal TotalNotSaleProductsPrice { get; set; }
		public string CurrencyIsoCode { get; set; }
		public List<DiscountSelectedProduct> SelectedProducts { get; set; }
	}
}
