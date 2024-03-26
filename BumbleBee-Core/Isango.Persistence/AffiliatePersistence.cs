using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Persistence.Contract;
using Isango.Persistence.Data;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence
{
    public class AffiliatePersistence : PersistenceBase, IAffiliatePersistence
    {
        private readonly ILogger _log;
        public AffiliatePersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// This method return the affiliate info.
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="alias"></param>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        public Affiliate GetAffiliateInfo(string domain, string alias, string affiliateId)
        {
            Affiliate affiliateInfo;
            try
            {
               
                using (var cmdStatement = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAffiliateByDomain))
                {
                    IsangoDataBaseLive.AddInParameter(cmdStatement, Constant.CompanyWebSite, DbType.String, domain);
                    IsangoDataBaseLive.AddInParameter(cmdStatement, Constant.Alias, DbType.String, alias);
                    IsangoDataBaseLive.AddInParameter(cmdStatement, Constant.AffiliateId, DbType.String, affiliateId);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmdStatement))
                    {
                        var affiliateData = new AffiliateData();
                        affiliateInfo = affiliateData.GetAffiliateData(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliatePersistence",
                    MethodName = "GetAffiliateInfo",
                    AffiliateId = affiliateId,
                    Params = $"{domain}, {alias}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return affiliateInfo;
        }

        /// <summary>
        /// this method return the list of the affiliate filters
        /// </summary>
        /// <returns></returns>
        public List<AffiliateFilter> GetAffiliateFilter()
        {
            List<AffiliateFilter> affiliateFilters = null;
            AffiliateFilter affiliateFilter = null;
            try
            {
                using (var cmdStatement = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAffiliateFilter))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmdStatement))
                    {
                        var affiliateData = new AffiliateData();

                        affiliateData.LoadAffiliateFilterList(ref affiliateFilters, ref affiliateFilter, reader);
                        affiliateData.LoadRegions(ref affiliateFilters, ref affiliateFilter, reader);
                        affiliateData.LoadServices(ref affiliateFilters, ref affiliateFilter, reader);
                        affiliateData.LoadDurationTypes(ref affiliateFilters, ref affiliateFilter, reader);
                        affiliateData.LoadAffiliateServicesPriority(ref affiliateFilters, ref affiliateFilter, reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliatePersistence",
                    MethodName = "GetAffiliateFilter",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return affiliateFilters;
        }

        /// <summary>
        /// Get widget partners
        /// </summary>
        /// <returns></returns>
        public List<Partner> GetWidgetPartners()
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.uspGetAffiliatesWidgetPartner))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var affiliateData = new AffiliateData();
                        return affiliateData.GetWidgetPartner(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliatePersistence",
                    MethodName = "GetWidgetPartners",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

        }

        /// <summary>
        /// Get Affiliate Information
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public Affiliate GetAffiliateInformation(string affiliateId, string languageCode)
        {
            Affiliate affiliate = null;
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.SpGetCustomerService))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.AffiliateId, DbType.Guid, new Guid(affiliateId));
                    IsangoDataBaseLive.AddInParameter(command, Constant.LanguageCodeParam, DbType.String, languageCode);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            var affiliateData = new AffiliateData();
                            affiliate = affiliateData.GetAffiliateInformation(reader, affiliateId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliatePersistence",
                    MethodName = "GetAffiliateInformation",
                    AffiliateId = affiliateId,
                    Params = $"{languageCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return affiliate;
        }

        /// <summary>
        /// Get Affiliate Release Tags
        /// </summary>
        /// <returns></returns>
        public List<AffiliateReleaseTag> GetAffiliateReleaseTags()
        {
            var affiliateTags = new List<AffiliateReleaseTag>();
            try
            {
                using (var cmdStatement = IsangoDataBaseLive.GetStoredProcCommand(Constant.SpGetAffiliateReleaseTag))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmdStatement))
                    {
                        while (reader.Read())
                        {
                            var affiliateData = new AffiliateData();

                            affiliateTags.Add(affiliateData.GetAffiliateReleaseTag(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliatePersistence",
                    MethodName = "GetAffiliateReleaseTags",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return affiliateTags;
        }

        /// <summary>
        /// Update Affiliate Release Tags
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <param name="releaseTag"></param>
        /// <param name="isForAll"></param>
        /// <returns></returns>
        public string UpdateAffiliateReleaseTags(string affiliateId, string releaseTag, bool isForAll = false)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.SpUpdateCustomerService))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.AffiliateId, DbType.String, affiliateId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.IsForAllParam, DbType.Boolean, isForAll);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ReleaseTagParam, DbType.String, releaseTag);
                    IsangoDataBaseLive.AddOutParameter(command, Constant.ResultParam, DbType.String, 500);
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                    var result = Convert.ToString(IsangoDataBaseLive.GetParameterValue(command, Constant.ResultParam));
                    return result;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliatePersistence",
                    MethodName = "UpdateAffiliateReleaseTags",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get White Label Affiliates Information
        /// </summary>
        /// <returns></returns>
        public List<Affiliate> GetWLAffiliateInfo()
        {
            var affiliates = new List<Affiliate>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetB2BWhileLabelSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            var affiliateData = new AffiliateData();
                            affiliates.Add(affiliateData.GetWLAffiliateInfo(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliatePersistence",
                    MethodName = "GetWLAffiliateInfo",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return affiliates;
        }

        /// <summary>
        /// Get Affiliate Id through User Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetAffiliateIdByUserId(string userId)
        {
            var affiliateId = string.Empty;
            try
            {
                using (var command = PrimalIdentitiesDb.GetStoredProcCommand(Constant.GetUsersAffiliateSp))
                {
                    PrimalIdentitiesDb.AddInParameter(command, "@UserId", DbType.String, userId);
                    using (var reader = PrimalIdentitiesDb.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            var affiliateData = new AffiliateData();
                            affiliateId = affiliateData.GetAffiliateIdByUserId(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliatePersistence",
                    MethodName = "GetAffiliateIdByUserId",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return affiliateId;
        }

        /// <summary>
        /// Get modified affiliates
        /// </summary>
        /// <returns></returns>
        public List<string> GetModifiedAffiliates()
        {
            var affiliateIds = new List<string>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetModifiedAffiliatesSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            var affiliateData = new AffiliateData();
                            affiliateIds.Add(affiliateData.GetAffiliateId(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliatePersistence",
                    MethodName = "GetModifiedAffiliates",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return affiliateIds;
        }


        /// <summary>
        /// Get AffiliateWise Service Min Price
        /// </summary>
        /// <returns></returns>
        public List<AffiliateWiseServiceMinPrice> GetAffiliateWiseServiceMinPrice()
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAffiliateWiseServiceMinPrice))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.Affliateid, DbType.String, "default");
                    dbCmd.CommandType = CommandType.StoredProcedure;
                    dbCmd.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var activityAvailableDaysList = new List<AffiliateWiseServiceMinPrice>();
                        var activityData = new AffiliateWiseServiceMinPrice();
                        while (reader.Read())
                        {
                            try
                            {
                                var affiliateWiseServiceMinPrice = new AffiliateWiseServiceMinPrice
                                {
                                    AffiliateId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AffiliateId"),
                                    ServiceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceId"),
                                    CurrencyIsoCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CurrencyIsoCode"),
                                    OfferPercent = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "OfferPercent"),
                                    BasePrice = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "BasePrice"),
                                    SellPrice = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "SellPrice"),
                                    CostPrice = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "CostPrice"),
                                };
                                activityAvailableDaysList.Add(affiliateWiseServiceMinPrice);
                            }
                            catch
                            {
                                //Ignore 
                            }
                        }
                        return activityAvailableDaysList;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AffiliatePersistence",
                    MethodName = "GetAffiliateWiseServiceMinPrice",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

    }
}