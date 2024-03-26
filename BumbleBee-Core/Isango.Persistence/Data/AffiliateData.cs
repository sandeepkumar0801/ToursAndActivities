using Isango.Entities;
using Isango.Entities.Affiliate;
using Isango.Entities.Enums;
using Isango.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Data;
using Util;

namespace Isango.Persistence.Data
{
    public class AffiliateData
    {
        /// <summary>
        /// Method will load Affiliate Filter list with Flag properties
        /// </summary>
        /// <param name="affiliateFilters"></param>
        /// <param name="affiliateFilter"></param>
        /// <param name="reader"></param>
        public void LoadAffiliateFilterList(ref List<AffiliateFilter> affiliateFilters, ref AffiliateFilter affiliateFilter, IDataReader reader)
        {
            while (reader.Read())
            {
                if (affiliateFilters == null)
                {
                    affiliateFilters = new List<AffiliateFilter>();
                }
                affiliateFilter = new AffiliateFilter
                {
                    AffiliateId = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateID"),
                    DurationTypeFilter = DbPropertyHelper.BoolPropertyFromRow(reader, "Custom_ServiceType"),
                    RegionFilter = DbPropertyHelper.BoolPropertyFromRow(reader, "Custom_GeoTree"),
                    ActivityFilter = DbPropertyHelper.BoolPropertyFromRow(reader, "Custom_Services"),
                    AffiliateActivityPriorityFilter = DbPropertyHelper.BoolPropertyFromRow(reader, "Custom_ServicePriority")
                };

                if (!string.IsNullOrEmpty(DbPropertyHelper.StringPropertyFromRow(reader, "IsServiceExclusionList")))
                    affiliateFilter.IsServiceExclusionFilter = DbPropertyHelper.BoolPropertyFromRow(reader, "IsServiceExclusionList");

                if (!string.IsNullOrEmpty(DbPropertyHelper.StringPropertyFromRow(reader, "IsMarginApplicable")))
                    affiliateFilter.IsMarginFilter = DbPropertyHelper.BoolPropertyFromRow(reader, "IsMarginApplicable");

                affiliateFilter.AffiliateMargin = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "Margin");
                affiliateFilters.Add(affiliateFilter);
            }
        }

        /// <summary>
        /// Method will load Country Regions
        /// </summary>
        /// <param name="affiliateFilters"></param>
        /// <param name="affiliateFilter"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public void LoadRegions(ref List<AffiliateFilter> affiliateFilters, ref AffiliateFilter affiliateFilter, IDataReader reader)
        {
            reader.NextResult();
            while (reader.Read())
            {
                _affiliateId = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateID");
                affiliateFilter = affiliateFilters.Find(FindAffiliatePredicate) ?? new AffiliateFilter();
                if (affiliateFilter.Regions == null)
                {
                    affiliateFilter.Regions = new List<int>();
                }
                affiliateFilter.Regions.Add(DbPropertyHelper.Int32PropertyFromRow(reader, "Country_RegionID"));
            }
        }

        /// <summary>
        /// Method will load Services which needs to be excluded
        /// </summary>
        /// <param name="affiliateFilters"></param>
        /// <param name="affiliateFilter"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public void LoadServices(ref List<AffiliateFilter> affiliateFilters, ref AffiliateFilter affiliateFilter, IDataReader reader)
        {
            reader.NextResult();
            while (reader.Read())
            {
                _affiliateId = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateID");
                affiliateFilter = affiliateFilters.Find(FindAffiliatePredicate);
                if (affiliateFilter.Activities == null)
                {
                    affiliateFilter.Activities = new List<int>();
                }
                affiliateFilter.Activities.Add(DbPropertyHelper.Int32PropertyFromRow(reader, "ServiceID"));
            }
        }

        /// <summary>
        /// Method will load Duration Types
        /// </summary>
        /// <param name="affiliateFilters"></param>
        /// <param name="affiliateFilter"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public void LoadDurationTypes(ref List<AffiliateFilter> affiliateFilters, ref AffiliateFilter affiliateFilter, IDataReader reader)
        {
            reader.NextResult();
            while (reader.Read())
            {
                _affiliateId = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateID");
                affiliateFilter = affiliateFilters.Find(FindAffiliatePredicate);
                if (affiliateFilter.DurationTypes == null)
                {
                    affiliateFilter.DurationTypes = new List<ActivityType>();
                }
                affiliateFilter.DurationTypes.Add((ActivityType)DbPropertyHelper.Int32PropertyFromRow(reader, "Service_TypeID"));
            }
        }

        /// <summary>
        /// Method will load Duration Types
        /// </summary>
        /// <param name="affiliateFilters"></param>
        /// <param name="affiliateFilter"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public void LoadAffiliateServicesPriority(ref List<AffiliateFilter> affiliateFilters, ref AffiliateFilter affiliateFilter, IDataReader reader)
        {
            reader.NextResult();
            while (reader.Read())
            {
                _affiliateId = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateID");
                affiliateFilter = affiliateFilters.Find(FindAffiliatePredicate);
                if (affiliateFilter.AffiliateServicesPriority == null)
                {
                    affiliateFilter.AffiliateServicesPriority = new List<KeyValuePair<int, int>>();
                }

                var servicePriority = new KeyValuePair<int, int>
                (
                    DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid"),
                    DbPropertyHelper.Int32PropertyFromRow(reader, "PriorityOrder")
                );

                affiliateFilter.AffiliateServicesPriority.Add(servicePriority);
            }
        }

        /// <summary>
        /// This method will return supported languages data reader mapping
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Affiliate GetAffiliateData(IDataReader reader)
        {
            var affiliateInfo = new Affiliate();
            while (reader.Read())
            {
                affiliateInfo.Id = DbPropertyHelper.StringPropertyFromRow(reader, "affiliateid");
                affiliateInfo.WebHookURL = DbPropertyHelper.StringPropertyFromRow(reader, "WebHookURL");
                affiliateInfo.Title = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateTitle");
                affiliateInfo.Name = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateName");
                affiliateInfo.CompanyWebSite = DbPropertyHelper.StringPropertyFromRow(reader, "CompanyWebsite");
                affiliateInfo.AffiliateBaseURL = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateBaseURL");
                affiliateInfo.CurrencyName = DbPropertyHelper.StringPropertyFromRow(reader, "CurrencyCode");
                affiliateInfo.CurrencySymbol = DbPropertyHelper.StringPropertyFromRow(reader, "CurrencySymbol");
                affiliateInfo.Isocode = DbPropertyHelper.StringPropertyFromRow(reader, "CurrencyCode");
                affiliateInfo.LanguageCode = DbPropertyHelper.StringPropertyFromRow(reader, "defaultlanguage").ToLower();
                affiliateInfo.B2BAffiliateId = DbPropertyHelper.StringPropertyFromRow(reader, "B2BAffiliateID");
                affiliateInfo.Alias = DbPropertyHelper.StringPropertyFromRow(reader, "Alias");
                affiliateInfo.AffiliateRegionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "AffRegionId");
                affiliateInfo.PaymentMethodType = (PaymentMethodType)Enum.Parse(typeof(PaymentMethodType),
                    DbPropertyHelper.StringDefaultPropertyFromRow(reader, "paymenttypeid"));
                affiliateInfo.DiscountCodePercentage = !string.IsNullOrEmpty(DbPropertyHelper.StringPropertyFromRow(reader, "DiscountPercent")) ? Convert.ToDecimal(reader["DiscountPercent"].ToString()) : 0;
                affiliateInfo.AffiliateKey = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "AffiliateKey");
                //string[] languages = reader["SupportedLanguages"].ToString().Split(',');
                //foreach (string lang in languages)
                //{
                //    affiliateInfo.Languages.Add(lang);
                //}
                affiliateInfo.Type = GetAffiliateType(reader["AffiliateGroupID"].ToString());

                affiliateInfo.AdyenStringentAccount = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "AdyenStringentAccount");
                affiliateInfo.AdyenStringentAccountRestrictProducts = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "AdyenStringentAccountRestrictProducts");

                affiliateInfo.SupportMultiPaymentType = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "SupportMultiPaymentType");

                affiliateInfo.GoogleDetail = new GoogleDetail
                {
                    GoogleTrackerLabel = DbPropertyHelper.StringPropertyFromRow(reader, "GoogleTrackerLabel"),
                    GTMIdentifier = DbPropertyHelper.StringPropertyFromRow(reader, "Google_TagManager"),
                    GoogleConversionId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "GoogleConversionId")
                        .Trim()
                };

                affiliateInfo.AffiliateConfiguration = new AffiliateConfiguration
                {
                    MultisavePercentage = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "multiSavePercent"),
                    DiscountPercent = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "DiscountPercent"),
                    Lob = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "LOB"),
                    PaymentTypeId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PaymentTypeId"),
                    IsMultiSave = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsMultisave"),
                    IsB2BNetPriceAffiliate = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsB2bNetPrice"),
                    IsShowBadge = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IshowBadge"),
                    IsRegular = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsRegular"),
                    IsSupplementOffer = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsSupplementOffer"),
                    IsB2BAffiliate = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsB2BAffiliate"),
                    IshowBadge = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsB2BAffiliate"),
                    IsInDestination = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsInDestination"),
                    IsCriteoEnabled = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IscriteoEnabled"),
                    IsMarginApplicable = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsMarginApplicable"),
                    DefaultDiscountCode = DbPropertyHelper.StringPropertyFromRow(reader, "DefaultDiscountCode"),
                };

                affiliateInfo.AffiliateCompanyDetail = new AffiliateCompanyDetail
                {
                    CompanyWebSite = DbPropertyHelper.StringPropertyFromRow(reader, "companywebsite"),
                    CompanyEmail = DbPropertyHelper.StringPropertyFromRow(reader, "Email")
                };

                affiliateInfo.FacebookDetail = new FacebookDetail
                {
                    FacebookAppId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "facebookAppID"),
                    FacebookAppSecret = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "facebookAppSecret").Trim()
                };

                affiliateInfo.AffiliateCredit = new AffiliateCredit
                {
                    MaxMarginLimit = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "Margin")
                };
            }

            reader.NextResult();
            affiliateInfo.SupportedLanguages = GetSupportedLanguagesData(reader);

            reader.NextResult();
            affiliateInfo.SupportedCurrencies = GetSupportedCurrenciesData(reader);

            return affiliateInfo;
        }

        private AffiliateGroup GetAffiliateType(string val)
        {
            int x = int.Parse(val);
            if (x != 6)
                return (AffiliateGroup)7;
            else
                return (AffiliateGroup)6;
        }

        /// <summary>
        /// Get supported currencies for affiliate
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<SupportedCurrency> GetSupportedCurrenciesData(IDataReader reader)
        {
            var supportedCurrencies = new List<SupportedCurrency>();
            while (reader.Read())
            {
                var supportedCurrency = new SupportedCurrency
                {
                    Name = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CurrencyName"),
                    Symbol = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CurrencySymbol"),
                    IsoCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CurrencyCode")
                };
                supportedCurrencies.Add(supportedCurrency);
            }

            return supportedCurrencies;
        }

        /// <summary>
        /// Get supported languages for affiliate
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<SupportedLanguage> GetSupportedLanguagesData(IDataReader reader)
        {
            var supportedLanguages = new List<SupportedLanguage>();
            while (reader.Read())
            {
                var supportedLanguage = new SupportedLanguage
                {
                    Id = DbPropertyHelper.Int32PropertyFromRow(reader, "ID"),
                    ShortName = DbPropertyHelper.StringPropertyFromRow(reader, "ShortName"),
                    DisplayName = DbPropertyHelper.StringPropertyFromRow(reader, "DisplayName"),
                    IsoCode = DbPropertyHelper.StringPropertyFromRow(reader, "IsoCode")
                };

                supportedLanguages.Add(supportedLanguage);
            }

            return supportedLanguages;
        }

        public List<Partner> GetWidgetPartner(IDataReader reader)
        {
            var affiliates = new List<Partner>();

            while (reader.Read())
            {
                var affiliate = new Partner
                {
                    ID = DbPropertyHelper.StringPropertyFromRow(reader, "affiliateid").Trim(),
                    Name = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateName").Trim(),
                    SiteRoot = DbPropertyHelper.StringPropertyFromRow(reader, "CompanyWebsite").Trim(),
                    HomePage = DbPropertyHelper.StringPropertyFromRow(reader, "DefaultPage").Trim(),
                    UrlPrefix = DbPropertyHelper.StringPropertyFromRow(reader, "CompanyAlias").Trim()
                };
                affiliates.Add(affiliate);
            }

            return affiliates;
        }

        public Affiliate GetAffiliateInformation(IDataReader reader, string affiliateId)
        {
            var affiliate = new Affiliate
            {
                Id = affiliateId,
                Name = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateName"),
                Title = DbPropertyHelper.StringPropertyFromRow(reader,
                    DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateTitle") == ""
                        ? "AffiliateName"
                        : "AffiliateTitle"),
                isRiskifiedEnable = DbPropertyHelper.BoolPropertyFromRow(reader, "IsRiskifiedEnabled"),
                B2BAffiliateId = DbPropertyHelper.StringPropertyFromRow(reader, "B2BAffiliateID"),
                Alias = DbPropertyHelper.StringPropertyFromRow(reader, "Alias"),
                AffiliateCompanyDetail = new AffiliateCompanyDetail
                {
                    CompanyAddress = DbPropertyHelper.StringPropertyFromRow(reader, "CompanyName"),
                    CompanyEmail = DbPropertyHelper.StringPropertyFromRow(reader, "CompanyEmail"),
                    CompanyWebSite = DbPropertyHelper.StringPropertyFromRow(reader, "CompanyWebsite"),
                },
                AffiliateCredit = new AffiliateCredit
                {
                    CreditLimit = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "CreditLimit"),
                    AvailableCredit =
                        DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "AvailableCredit"),
                    ThresholdAmount =
                        DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "ThresholdAmount"),
                    OverdraftAmount =
                        DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "OverdraftAmount"),
                    CanBreachLimit = DbPropertyHelper.BoolPropertyFromRow(reader, "CanBreachLimit"),
                    MaxMarginLimit = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "MaxMarginLimit")
                },
                AffiliateConfiguration = new AffiliateConfiguration
                {
                    IsOfflineEmail = DbPropertyHelper.BoolPropertyFromRow(reader, "IsasyncEmail"),
                    IsSupplementOffer = DbPropertyHelper.BoolPropertyFromRow(reader, "IsSupplementOffer"),
                    IsPartialBookingSupport = DbPropertyHelper.BoolPropertyFromRow(reader, "IsPartialBookingSupport"),
                    IsB2BAffiliate = DbPropertyHelper.BoolPropertyFromRow(reader, "IsB2BAffiliate"),
                    MultisavePercentage = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "multiSavePercent"),
                    IsMultiSave = DbPropertyHelper.BoolPropertyFromRow(reader, "IsMultisave"),
                    IsB2BNetPriceAffiliate = DbPropertyHelper.BoolPropertyFromRow(reader, "IsB2bNetPriceAffiliate")
                },
                PaymentMethodType = (PaymentMethodType)Enum.Parse(typeof(PaymentMethodType),
                    DbPropertyHelper.StringPropertyFromRow(reader, "paymenttypeid")),
                AffiliateBaseURL = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AffiliateBaseURL"),
                AvailabilityReferenceExpiry = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "AvailabilityReferenceExpiry"),
                AdyenStringentAccount=DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "AdyenStringentAccount"),
                AdyenStringentAccountRestrictProducts = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "AdyenStringentAccountRestrictProducts")
            };

            return affiliate;
        }

        public AffiliateReleaseTag GetAffiliateReleaseTag(IDataReader reader)
        {
            var affiliateTag = new AffiliateReleaseTag
            {
                AffiliateId = DbPropertyHelper.StringPropertyFromRow(reader, "affiliateid"),
                AffiliateName = DbPropertyHelper.StringPropertyFromRow(reader, "affiliateName"),
                CompanyWebsite = DbPropertyHelper.StringPropertyFromRow(reader, "CompanyWebsite"),
                ReleaseTag = DbPropertyHelper.StringPropertyFromRow(reader, "ReleaseTag")
            };
            return affiliateTag;
        }

        /// <summary>
        /// Method will return While Label Affiliate information
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Affiliate GetWLAffiliateInfo(IDataReader reader)
        {
            var affiliate = new Affiliate
            {
                Id = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "affiliateid"),
                Name = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "affiliateName"),
                Title = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CompanyAlias")
            };

            return affiliate;
        }

        /// <summary>
        /// Method will return affiliate Id of respective user
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public string GetAffiliateIdByUserId(IDataReader reader)
        {
            return DbPropertyHelper.StringDefaultPropertyFromRow(reader, "affiliateid");
        }

        /// <summary>
        /// Method will return affiliate Id
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public string GetAffiliateId(IDataReader reader)
        {
            return DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AffiliateID");
        }

        #region Private Methods

        private string _affiliateId = string.Empty;

        private bool FindAffiliatePredicate(AffiliateFilter affiliateFilter)
        {
            return affiliateFilter.AffiliateId == _affiliateId;
        }

        #endregion Private Methods
    }
}