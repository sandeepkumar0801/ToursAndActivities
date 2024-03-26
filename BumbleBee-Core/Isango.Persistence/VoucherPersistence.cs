using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Persistence.Contract;
using Isango.Persistence.Data;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence
{
    public class VoucherPersistence : PersistenceBase, IVoucherPersistence
    {
        private readonly ILogger _log;
        public VoucherPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Get discount voucher details against the voucher code.
        /// </summary>
        /// <param name="voucherCode"></param>
        /// <returns></returns>
        public Voucher GetVoucher(string voucherCode)
        {
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetVoucherDetailSp))
                {
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.ParamPromoCode, DbType.String, voucherCode.ToUpperInvariant());
                    dbCommand.CommandType = CommandType.StoredProcedure;

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var voucherData = new VoucherData();
                        var voucher = voucherData.GetVoucher(reader, voucherCode);
                        if (voucher != null)
                        {
                            if (voucher.DiscountType == DiscountType.Gift)
                            {
                                voucher = voucherData.GetGiftVoucherData(reader, voucher);
                            }
                            else
                            {
                                reader.NextResult();
                            }

                            var validProducts = voucherData.GetValidProductIds(reader);
                            var validAffiliates = voucherData.GetValidAffiliateIds(reader);
                            var validCategories = voucherData.GetValidCategoryIds(reader);
                            var validDestinations = voucherData.GetValidDestinationIds(reader);
                            var validLOBs = voucherData.GetValidLobIds(reader);

                            voucher.VoucherConfig.ValidProducts = validProducts.Any() ? validProducts : null;
                            voucher.VoucherConfig.ValidAffiliates = validAffiliates.Any() ? validAffiliates : null;
                            voucher.VoucherConfig.ValidCategories = validCategories.Any() ? validCategories : null;
                            voucher.VoucherConfig.ValidDestinations = validDestinations.Any() ? validDestinations : null;
                            voucher.VoucherConfig.ValidLobsIds = validLOBs.Any() ? validLOBs : null;
                        }
                        return voucher;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "VoucherPersistence",
                    MethodName = "GetVoucher",
                    Params = $"{voucherCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get all the discount category configurations
        /// </summary>
        /// <returns></returns>
        public List<DiscountCategoryConfig> GetAllDiscountCategoryConfig()
        {
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetDiscountCategoryConfigSp))
                {
                    dbCommand.CommandType = CommandType.StoredProcedure;

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var voucherData = new VoucherData();
                        var discountCategoryCaps = new List<DiscountCategoryCap>();
                        var discountCategoryConfigs = voucherData.GetAllDiscountCategoryConfig(reader);
                        if (reader.NextResult())
                        {
                            discountCategoryCaps = voucherData.GetAllDiscountCategoryCap(reader);
                        }

                        foreach (var discountCategoryConfig in discountCategoryConfigs)
                        {
                            discountCategoryConfig.DiscountCategoryCaps = discountCategoryCaps.Where(e =>
                                e.DiscountCategoryType == discountCategoryConfig.DiscountCategoryType).ToList();
                        }
                        return discountCategoryConfigs;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "VoucherPersistence",
                    MethodName = "GetAllDiscountCategoryConfig",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}