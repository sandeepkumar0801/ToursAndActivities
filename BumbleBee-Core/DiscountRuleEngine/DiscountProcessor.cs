using DiscountRuleEngine.Constants;
using DiscountRuleEngine.Contracts;
using DiscountRuleEngine.Factory;
using DiscountRuleEngine.Model;
using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Service.Contract;
using System.Collections.Generic;
using System.Linq;

namespace DiscountRuleEngine
{
    public class DiscountProcessor : IDiscountProcessor
    {
        private readonly IVoucherValidator _voucherValidator;
        private readonly IVoucherService _voucherService;
        private readonly IAffiliateService _affiliateService;
        private readonly IMasterService _masterService;
        private readonly DiscountModuleFactory _discountModuleFactory;

        public DiscountProcessor(IVoucherService voucherService, IAffiliateService affiliateService, IMasterService masterService, IVoucherValidator voucherValidator, DiscountModuleFactory discountModuleFactory)
        {
            _voucherService = voucherService;
            _affiliateService = affiliateService;
            _masterService = masterService;
            _voucherValidator = voucherValidator;
            _discountModuleFactory = discountModuleFactory;
        }

        public DiscountCart Process(DiscountModel discountModel)
        {
            if (IsMultiSaveApplicable(discountModel))
                discountModel.Cart = ApplyMultiSaveDiscount(discountModel);

            if (discountModel.Vouchers.Count == 0)
                return discountModel.Cart;

            var discountCart = new DiscountCart();
            var messages = new List<string>();

            var vouchers = discountModel.Vouchers;
            foreach (var voucherInfo in vouchers)
            {
                var voucherDetail = _voucherService.GetVoucherAsync(voucherInfo.VoucherCode).Result;
                if (voucherDetail != null)
                {
                    voucherDetail.VoucherCode = voucherInfo.VoucherCode;
                    if (!voucherDetail.VoucherConfig.IsPercent)
                        voucherDetail.Amount = voucherDetail.Amount * _masterService
                            .GetConversionFactorAsync(voucherDetail.Currency.IsoCode, discountModel.Cart.CurrencyIsoCode).Result;
                    voucherDetail.Currency.IsoCode = discountModel.Cart.CurrencyIsoCode;

                    var voucherValidationResult = CheckVoucherValidity(discountModel, voucherDetail);
                    // ReSharper disable once PossibleNullReferenceException
                    discountModel.Vouchers.FirstOrDefault(e => e.VoucherCode == voucherInfo.VoucherCode).DiscountType = voucherDetail.DiscountType;

                    if (!string.IsNullOrEmpty(voucherValidationResult.Message))
                        messages.Add(voucherValidationResult.Message);
                    if (voucherValidationResult.IsValid)
                        discountCart = ApplyDiscount(discountModel, voucherDetail);
                }
                else
                {
                    messages.Add(Constant.VoucherValidationError);
                }
            }

            if (discountCart.SelectedProducts == null)
                discountCart = discountModel.Cart;
            discountCart.Messages = new List<string>();
            discountCart.Messages = messages;
            return discountCart;
        }

        private bool IsMultiSaveApplicable(DiscountModel discountModel)
        {
            var affiliateInfo =
                _affiliateService.GetAffiliateInformationAsync(discountModel?.AffiliateId);
            var distinctActivitiesCount = discountModel?.Cart?.SelectedProducts?.Select(x => x.Id)?.Distinct()?.ToList()?.Count ?? 0;

            var isApplicable = distinctActivitiesCount > 1 && (affiliateInfo?.Result?.AffiliateConfiguration?.IsMultiSave ?? false);

            return isApplicable;
        }

        private DiscountCart ApplyMultiSaveDiscount(DiscountModel discountModel)
        {
            var multiSaveDiscountModule = _discountModuleFactory.GetDiscountModule(DiscountType.MultiSave);
            return multiSaveDiscountModule.ApplyDiscount(discountModel);
        }

        private DiscountCart ApplyDiscount(DiscountModel discountModel, Voucher voucherDetail)
        {
            var discountModule = GetDiscountModule(voucherDetail);

            //Apply Discount
            var updatedCart = discountModule.ApplyDiscount(discountModel, voucherDetail);
            return updatedCart;
        }

        private DiscountModule GetDiscountModule(Voucher voucherDetail)
        {
            var discountModule = _discountModuleFactory.GetDiscountModule(voucherDetail.DiscountType);
            return discountModule;
        }

        private VoucherValidationResult CheckVoucherValidity(DiscountModel discountModel, Voucher voucherDetail)
        {
            //Check voucher validity and applicability
            var voucherValidationResult = _voucherValidator.IsVoucherValid(voucherDetail);
            if (!voucherValidationResult.IsValid) return voucherValidationResult;

            if (voucherDetail is GiftVoucher)
            {
                voucherValidationResult = _voucherValidator.IsVoucherValidForCustomerEmail(voucherDetail, discountModel.CustomerEmail);
                if (!voucherValidationResult.IsValid) return voucherValidationResult;
            }
            voucherValidationResult = _voucherValidator.IsVoucherApplicableForCart(discountModel, voucherDetail);

            return voucherValidationResult;
        }
    }
}