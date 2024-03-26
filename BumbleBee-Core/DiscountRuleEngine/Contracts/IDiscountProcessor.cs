using DiscountRuleEngine.Model;

namespace DiscountRuleEngine.Contracts
{
	public interface IDiscountProcessor
	{
		DiscountCart Process(DiscountModel discountModel);
	}
}