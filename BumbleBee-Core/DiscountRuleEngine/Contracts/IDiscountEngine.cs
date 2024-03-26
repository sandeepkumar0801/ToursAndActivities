using DiscountRuleEngine.Model;

namespace DiscountRuleEngine.Contracts
{
	public interface IDiscountEngine
	{
		DiscountCart Process(DiscountModel discountModel);
	}
}