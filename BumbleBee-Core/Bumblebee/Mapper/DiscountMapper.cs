using DiscountRuleEngine.Model;
using Isango.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using TableStorageOperations.Contracts;
using TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities;
using WebAPI.Models.RequestModels;
using WebAPI.Models.ResponseModels;
using Constant = WebAPI.Constants.Constant;
using DiscountCoupon = WebAPI.Models.ResponseModels.DiscountCoupon;
using SelectedProduct = WebAPI.Models.ResponseModels.SelectedProduct;

namespace WebAPI.Mapper
{
    public class DiscountMapper
    {
        private readonly ITableStorageOperation _TableStorageOperations;
        private readonly IActivityService _activityService;
        private readonly IMasterService _masterService;

        /// <summary>
        /// DiscountMapper Constructor
        /// </summary>
        /// <param name="TableStorageOperations"></param>
        /// <param name="activityService"></param>
        /// <param name="masterService"></param>
        public DiscountMapper(ITableStorageOperation TableStorageOperations, IActivityService activityService, IMasterService masterService)
        {
            _TableStorageOperations = TableStorageOperations;
            _activityService = activityService;
            _masterService = masterService;
        }

        /// <summary>
        /// This method prepare the request model for DRE from the API request and availabilties storage
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DiscountModel MapDiscountRequest(ProcessDiscountRequest request)
        {
            var totalCartPrice = 0M;
            var selectedProducts = new List<DiscountSelectedProduct>();

            var count = 0;
            var parentBundleId = 0;
            foreach (var availabilityReferenceId in request.AvailabilityReferenceIds)
            {
                var splittedAvailabilityReferenceIds = availabilityReferenceId.Split(Constant.PipeSeparator);
                foreach (var referenceId in splittedAvailabilityReferenceIds)
                {
                    var availabilitiesData = _TableStorageOperations.RetrieveData<BaseAvailabilitiesEntity>(referenceId, request.TokenId);
                    if (availabilitiesData == null) return null;

                    var activity = _activityService
                        .GetActivityById(availabilitiesData.ActivityId, DateTime.Today, request.LanguageCode)?.GetAwaiter().GetResult();
                    if (activity == null) return null;

                    count = GetSequenceNumber(availabilitiesData.BundleOptionID, parentBundleId, count);
                    parentBundleId = availabilitiesData.BundleOptionID;

                    var xFactor = _masterService
                        .GetConversionFactorAsync(availabilitiesData.CurrencyCode, request.CurrencyIsoCode).Result;
                    var product = new DiscountSelectedProduct
                    {
                        Id = availabilitiesData.ActivityId,
                        AvailabilityReferenceId = referenceId,
                        SellPrice = Convert.ToDecimal(availabilitiesData.BasePrice) * xFactor,
                        CurrencyIsoCode = request.CurrencyIsoCode,
                        IsSaleProduct = availabilitiesData.OnSale,
                        Margin = availabilitiesData.Margin,
                        CategoryIds = activity.CategoryIDs,
                        DestinationIds = activity.Regions?.Select(x => x.Id).ToList(),
                        ComponentOrderNumber = availabilitiesData.ComponentOrder,
                        ParentBundleId = availabilitiesData.BundleOptionID,
                        SequenceNumber = count,
                        LineOfBusiness = activity.LineOfBusinessId,
                        GatePrice = Convert.ToDecimal(availabilitiesData.GateBasePrice) * xFactor,
                        IsBundle = availabilitiesData.BundleOptionID > 0,
                        BundleOptionId = availabilitiesData.BundleOptionID
                    };

                    if (product.GatePrice > product.SellPrice)
                    {
                        product.IsSaleProduct = product.GatePrice > product.SellPrice;
                    }

                    totalCartPrice += product.SellPrice;
                    selectedProducts.Add(product);
                }
            }

            // Preparing vouchers from the API request
            var vouchers = new List<VoucherInfo>();
            if (request.DiscountCoupons != null)
            {
                foreach (var discountCoupon in request.DiscountCoupons)
                {
                    var voucher = new VoucherInfo
                    {
                        VoucherCode = discountCoupon
                    };
                    vouchers.Add(voucher);
                }
            }
            //Preparing DiscountModel
            var discountModel = new DiscountModel
            {
                AffiliateId = request.AffiliateId,
                UTMParameter = request.UTMParameter,
                CustomerEmail = request.CustomerEmail,
                Cart = new DiscountCart
                {
                    TotalPrice = totalCartPrice,
                    CurrencyIsoCode = request.CurrencyIsoCode,
                    SelectedProducts = selectedProducts
                },
                Vouchers = vouchers,
            };

            return discountModel;
        }

        /// <summary>
        /// This method maps the data in the ProcessDiscountResponse model
        /// </summary>
        /// <param name="discountCart"></param>
        /// <returns></returns>
        public ProcessDiscountResponse MapDiscountResponse(DiscountCart discountCart)
        {
            if (discountCart?.SelectedProducts == null) return null;

            var totalPrice = 0M;
            var totalDiscountedPrice = 0M;
            var selectedProducts = new List<SelectedProduct>();
            var sequenceNumber = 0;
            var totalOriginalPrice = 0M;
            var product = new SelectedProduct();
            var restrictSaleDiscount = false;
            foreach (var discountSelectedProduct in discountCart.SelectedProducts)
            {
                if (discountSelectedProduct == null) continue;
                var totalProductPrice = 0M;

                var saleDiscount = discountCart.SelectedProducts.Sum(x => x.GatePrice) - discountCart.SelectedProducts.Sum(x => x.SellPrice);
                var voucherDetailAmount = discountSelectedProduct?.AppliedDiscountCoupons?.FirstOrDefault()?.DiscountedPrice;

                totalProductPrice = CalculateProductPrice(discountSelectedProduct);
                totalPrice += totalProductPrice;
                totalDiscountedPrice += discountSelectedProduct.ProductDiscountedPrice + discountSelectedProduct.MultiSaveDiscountedPrice;

                if (discountSelectedProduct.IsSaleProduct == true && discountSelectedProduct?.AppliedDiscountCoupons != null &&
                    (int)discountSelectedProduct?.AppliedDiscountCoupons?.FirstOrDefault()?.DiscountType == 2
                    &&
                   voucherDetailAmount >= saleDiscount)
                {
                    totalOriginalPrice += discountSelectedProduct.GatePrice;
                    restrictSaleDiscount = true;
                }

                var selectedProduct = new SelectedProduct
                {
                    Id = discountSelectedProduct.Id,
                    GatePricePrice = discountSelectedProduct.GatePrice,
                    SellPrice = discountSelectedProduct.SellPrice,
                    CurrencyIsoCode = discountSelectedProduct.CurrencyIsoCode,
                    FinalPrice = totalProductPrice < 0 ? 0 : totalProductPrice,
                    ProductDiscountedPrice = discountSelectedProduct.ProductDiscountedPrice,
                    AvailabilityReferenceId = discountSelectedProduct.AvailabilityReferenceId,
                    IsMultiSaveApplied = discountSelectedProduct.IsMultiSaveApplied,
                    MultiSaveDiscountedPrice = discountSelectedProduct.MultiSaveDiscountedPrice,
                    AppliedDiscountCoupons = GetDiscountCoupons(discountSelectedProduct.AppliedDiscountCoupons),
                    IsBundle = discountSelectedProduct.IsBundle,
                    BundleOptionID = discountSelectedProduct.BundleOptionId,
                };

                if (discountSelectedProduct.ComponentOrderNumber > 0)
                {
                    if (sequenceNumber != discountSelectedProduct.SequenceNumber)
                    {
                        sequenceNumber = discountSelectedProduct.SequenceNumber;
                        product = selectedProduct;
                        product.AvailabilityReferenceId = discountSelectedProduct.AvailabilityReferenceId;
                        product.SellPrice = discountSelectedProduct.SellPrice;
                        product.FinalPrice = totalProductPrice < 0 ? 0 : totalProductPrice;
                        product.ProductDiscountedPrice = discountSelectedProduct.ProductDiscountedPrice;
                        product.MultiSaveDiscountedPrice = discountSelectedProduct.MultiSaveDiscountedPrice;
                    }
                    else
                    {
                        product.AvailabilityReferenceId = $"{product.AvailabilityReferenceId}|{discountSelectedProduct.AvailabilityReferenceId}";
                        product.SellPrice += discountSelectedProduct.SellPrice;
                        product.FinalPrice += totalProductPrice < 0 ? 0 : totalProductPrice;
                        product.ProductDiscountedPrice += discountSelectedProduct.ProductDiscountedPrice;
                        product.MultiSaveDiscountedPrice += discountSelectedProduct.MultiSaveDiscountedPrice;
                    }
                    if (product.AppliedDiscountCoupons?.Count > 0)
                        // ReSharper disable once PossibleNullReferenceException
                        product.AppliedDiscountCoupons.FirstOrDefault().DiscountedPrice = product.ProductDiscountedPrice;
                    if (discountCart.SelectedProducts.IndexOf(discountSelectedProduct) == discountCart.SelectedProducts.Count - 1)
                    {
                        if (product.AvailabilityReferenceId != null)
                        {
                            selectedProducts.Add(product);
                        }
                    }
                }
                if (sequenceNumber != discountSelectedProduct.SequenceNumber)
                {
                    if (product.AvailabilityReferenceId != null)
                    {
                        selectedProducts.Add(product);
                        product = new SelectedProduct();
                    }
                }
                if (discountSelectedProduct.ComponentOrderNumber == 0)
                {
                    selectedProducts.Add(selectedProduct);
                }

            }

            if (totalOriginalPrice > 0)
            {
                discountCart.TotalPrice = totalOriginalPrice;
            }

            var discountResponse = new ProcessDiscountResponse
            {
                FinalPrice = totalPrice < 0 ? 0 : totalPrice,
                TotalDiscountedPrice = totalDiscountedPrice,
                OriginalPrice = discountCart.TotalPrice,
                CurrencyIsoCode = discountCart.CurrencyIsoCode,
                SelectedProducts = selectedProducts,
                Messages = new List<string>(),
                RestrictSaleDiscount = restrictSaleDiscount
            };

            if (discountCart.Messages != null)
            {
                foreach (var message in discountCart.Messages)
                {
                    discountResponse.Messages.Add(message);
                }
            }

            return discountResponse;
        }

        #region Private Methods

        private List<DiscountCoupon> GetDiscountCoupons(List<DiscountRuleEngine.Model.DiscountCoupon> coupons)
        {
            var discountCoupons = new List<DiscountCoupon>();
            if (coupons == null) return discountCoupons;

            foreach (var coupon in coupons)
            {
                var discountCoupon = new DiscountCoupon
                {
                    DiscountCouponCode = coupon.DiscountCouponCode,
                    Message = coupon.Message,
                    DiscountedPrice = coupon.DiscountedPrice,
                    IsValid = coupon.IsValid
                };
                discountCoupons.Add(discountCoupon);
            }
            return discountCoupons;
        }

        private decimal CalculateProductPrice(DiscountSelectedProduct product)
        {
            return product.SellPrice - (product.ProductDiscountedPrice + product.MultiSaveDiscountedPrice);
        }


        private int GetSequenceNumber(int bundleOptionId, int parentBundleId, int count)
        {
            if (bundleOptionId > 0)
            {
                if (parentBundleId != bundleOptionId)
                {
                    count++;
                }
            }
            else
            {
                count++;
            }
            return count;
        }

        #endregion Private Methods
    }
}