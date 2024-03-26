using DiscountRuleEngine.Contracts;
using Isango.Entities.Enums;
using Isango.Service.Contract;

namespace DiscountRuleEngine.Factory
{
	public class DiscountModuleFactory
	{
		private readonly IVoucherValidator _voucherValidator;
		private readonly IDiscountValidator _discountValidator;
		private readonly IVoucherService _voucherService;
		private readonly IAffiliateService _affiliateService;
		public DiscountModuleFactory(IVoucherValidator voucherValidator, IDiscountValidator discountValidator, IVoucherService voucherService, IAffiliateService affiliateService)
		{
			_voucherValidator = voucherValidator;
			_discountValidator = discountValidator;
			_voucherService = voucherService;
			_affiliateService = affiliateService;
		}
		public DiscountModule GetDiscountModule(DiscountType discountType)
		{
			//TODO:: Add case for promotional discount module
			switch (discountType)
			{
				case DiscountType.MultiSave:
					return new MultiSaveDiscountModule(_voucherService, _affiliateService);
				default:
					return new PromotionalDiscountModule(_voucherValidator, _discountValidator, _voucherService);
			}
		}
	}
}
