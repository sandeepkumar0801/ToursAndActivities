using Isango.Entities;
using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using Util;

namespace Isango.Persistence.Data
{
    public class VoucherData
    {
        public Voucher GetVoucher(IDataReader reader, string voucherCode)
        {
            Voucher voucher = null;
            while (reader.Read())
            {
                voucher = new Voucher
                {
                    Id = DbPropertyHelper.Int32PropertyFromRow(reader, "PromoID"),
                    DiscountType =
                        (DiscountType)Enum.Parse(typeof(DiscountType), DbPropertyHelper.StringPropertyFromRow(reader, "PromoTypeID")),
                    DiscountCategoryType =
                        (DiscountCategoryType)Enum.Parse(typeof(DiscountCategoryType), DbPropertyHelper.StringPropertyFromRow(reader, "PromoCategoryTypeID")),
                    StartDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "StartDate"),
                    ExpiryDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "ExpiryDate"),
                    IsValid = DbPropertyHelper.BoolPropertyFromRow(reader, "IsValid"),
                    Amount = DbPropertyHelper.DecimalPropertyFromRow(reader, "Amount"),
                    Currency = GetCurrency(DbPropertyHelper.StringPropertyFromRow(reader, "Currency")),
                    VoucherConfig = new VoucherConfig
                    {
                        IsPercent = DbPropertyHelper.BoolPropertyFromRow(reader, "IsPercent"),
                        isServiceInclusion = DbPropertyHelper.BoolPropertyFromRow(reader, "ISSERVICEINCLUDEDLIST"),
                        isCategoryInclusion = DbPropertyHelper.BoolPropertyFromRow(reader, "ISCATEGORYINCLUDEDLIST"),
                        isDestinationInclusion = DbPropertyHelper.BoolPropertyFromRow(reader, "ISDESTINATIONINCLUDEDLIST"),
                        isLobInclusion = DbPropertyHelper.BoolPropertyFromRow(reader, "ISLOBINCLUDEDLIST"),
                        ThresholdProductMargin = DbPropertyHelper.DecimalPropertyFromRow(reader, "ThresholdProductMargin"),
                        ThresholdIsPercent = DbPropertyHelper.BoolPropertyFromRow(reader, "ThresholdIsPercent"),
                        UTMParameter = DbPropertyHelper.StringPropertyFromRow(reader, "UTMParameter")
                    },
                    VoucherCode = voucherCode,
                };
            }

            return voucher;
        }

        public GiftVoucher GetGiftVoucherData(IDataReader reader, Voucher voucher)
        {
            var giftVoucher = VoucherToGiftVoucher(voucher);
            if (reader.NextResult() && giftVoucher != null)
            {
                while (reader.Read())
                {
                    giftVoucher.BuyerName = DbPropertyHelper.StringPropertyFromRow(reader, "BuyerName");
                    giftVoucher.BuyerEmail = DbPropertyHelper.StringPropertyFromRow(reader, "BuyerEmail");
                    giftVoucher.ReceiverName = DbPropertyHelper.StringPropertyFromRow(reader, "ReceiverName");
                    giftVoucher.ReceiverEmail = DbPropertyHelper.StringPropertyFromRow(reader, "ReceiverEmail");
                    giftVoucher.Message = DbPropertyHelper.StringPropertyFromRow(reader, "Message");
                    giftVoucher.SendOnDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "sendondate");
                }
            }

            return giftVoucher;
        }

        public List<int> GetValidProductIds(IDataReader reader)
        {
            var productIds = new List<int>();
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    productIds.Add(DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid"));
                }
            }
            return productIds;
        }

        public List<string> GetValidAffiliateIds(IDataReader reader)
        {
            var affiliateIds = new List<string>();
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    affiliateIds.Add(DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateId"));
                }
            }
            return affiliateIds;
        }

        public List<int> GetValidCategoryIds(IDataReader reader)
        {
            var productList = new List<int>();
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    productList.Add(DbPropertyHelper.Int32PropertyFromRow(reader, "CategoryId"));
                }
            }
            return productList;
        }

        public List<int> GetValidDestinationIds(IDataReader reader)
        {
            var productList = new List<int>();
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    productList.Add(DbPropertyHelper.Int32PropertyFromRow(reader, "DestinationID"));
                }
            }
            return productList;
        }

        public List<int> GetValidLobIds(IDataReader reader)
        {
            var lobList = new List<int>();
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    lobList.Add(DbPropertyHelper.Int32PropertyFromRow(reader, "lineofbusinessid"));
                }
            }
            return lobList;
        }

        public List<DiscountCategoryConfig> GetAllDiscountCategoryConfig(IDataReader reader)
        {
            var discountCategoryConfigs = new List<DiscountCategoryConfig>();
            while (reader.Read())
            {
                var discountCategoryConfig = new DiscountCategoryConfig
                {
                    DiscountCategoryType =
                        (DiscountCategoryType)Enum.Parse(typeof(DiscountCategoryType), DbPropertyHelper.StringPropertyFromRow(reader, "PromoCategoryTypeID")),
                    DiscountType =
                        (DiscountType)Enum.Parse(typeof(DiscountType), DbPropertyHelper.StringPropertyFromRow(reader, "PromoTypeID")),
                    RequiredExpiryDate = DbPropertyHelper.BoolPropertyFromRow(reader, "RequiredExpiryDate"),
                    ValidOnSaleProduct = DbPropertyHelper.BoolPropertyFromRow(reader, "ValidOnSaleProduct"),
                    HasMinCartCap = DbPropertyHelper.BoolPropertyFromRow(reader, "HasMinCartCap"),
                    HasMaxValueCap = DbPropertyHelper.BoolPropertyFromRow(reader, "HasMaxValueCap"),
                    ApplicableWithMultiSave = DbPropertyHelper.BoolPropertyFromRow(reader, "ApplicableWithMultiSave")
                };
                discountCategoryConfigs.Add(discountCategoryConfig);
            }

            return discountCategoryConfigs;
        }

        public List<DiscountCategoryCap> GetAllDiscountCategoryCap(IDataReader reader)
        {
            var discountCategoryCaps = new List<DiscountCategoryCap>();
            while (reader.Read())
            {
                var discountCategoryCap = new DiscountCategoryCap
                {
                    DiscountCategoryType =
                        (DiscountCategoryType)Enum.Parse(typeof(DiscountCategoryType), DbPropertyHelper.StringPropertyFromRow(reader, "PromoCategoryTypeID")),
                    MinCartCap = DbPropertyHelper.DecimalPropertyFromRow(reader, "MinCartCap"),
                    MaxValueCap = DbPropertyHelper.DecimalPropertyFromRow(reader, "MaxValueAllowed"),
                    Currency = GetCurrency(DbPropertyHelper.StringPropertyFromRow(reader, "CURRENCYISOCODE"))
                };
                discountCategoryCaps.Add(discountCategoryCap);
            }

            return discountCategoryCaps;
        }

        private Currency GetCurrency(string isoCode)
        {
            return new Currency { IsoCode = isoCode };
        }

        private GiftVoucher VoucherToGiftVoucher(Voucher voucher)
        {
            return new GiftVoucher
            {
                Id = voucher.Id,
                VoucherCode = voucher.VoucherCode,
                StartDate = voucher.StartDate,
                Currency = voucher.Currency,
                DiscountType = voucher.DiscountType,
                Amount = voucher.Amount,
                VoucherConfig = voucher.VoucherConfig,
                DiscountCategoryType = voucher.DiscountCategoryType,
                ExpiryDate = voucher.ExpiryDate,
                IsValid = voucher.IsValid,
            };
        }
    }
}