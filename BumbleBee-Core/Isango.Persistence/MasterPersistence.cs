using Isango.Entities;
using Isango.Entities.Bokun;
using Isango.Entities.ConsoleApplication.DataDumping;
using Isango.Entities.ElasticData;
using Isango.Entities.GlobalTixV3;
using Isango.Entities.Hotel;
using Isango.Entities.HotelBeds;
using Isango.Entities.Master;
using Isango.Entities.Region;
using Isango.Entities.Rezdy;
using Isango.Entities.SiteMap;
using Isango.Entities.Tiqets;
using Isango.Entities.TourCMS;
using Isango.Entities.v1Css;
using Isango.Entities.Ventrata;
using Isango.Persistence.Contract;
using Isango.Persistence.Data;
using Logger.Contract;
using Microsoft.Practices.EnterpriseLibrary.Data;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Util;
using Constant = Isango.Persistence.Constants.Constants;
using GoldenTours = Isango.Entities.GoldenTours;
using TicketByRegion = Isango.Entities.TicketByRegion;

namespace Isango.Persistence
{
    public class MasterPersistence : PersistenceBase, IMasterPersistence
    {
        private readonly ILogger _log;

        public MasterPersistence(ILogger log)
        {
            _log = log;
        }

        #region Public methods

        /// <summary>
        /// This Method with return the list of currencies related to the affiliateID.
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <returns> List of Currency</returns>
        public List<Currency> GetCurrencies(string affiliateId)
        {
            var currencies = new List<Currency>();
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCurrencyForAffiliates))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.AffiliateId, DbType.String, affiliateId);

                    dbCmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            currencies.Add(masterData.GetCurrencyData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetCurrencies",
                    Params = $"{affiliateId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return currencies;
        }

        /// <summary>
        /// This method will return teh Currency object for the provided Currency Code
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <returns> Currency Object</returns>
        public Currency GetCurrency(string currencyCode)
        {
            var currency = new Currency();
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCurrencyfromCurrencyCode))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.Currecycode, DbType.String, currencyCode);
                    dbCmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            currency = masterData.GetCurrencyData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetCurrency",
                    Params = $"{currencyCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return currency;
        }

        /// <summary>
        /// This method will return Currency code for the provided Country code.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public string GetCurrencyCodeForCountry(string countryCode)
        {
            object currencyCode;
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCurrencyFromCountryCode))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.CountryCode, DbType.String, countryCode);
                    currencyCode = IsangoDataBaseLive.ExecuteScalar(command);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetCurrencyCodeForCountry",
                    Params = $"{countryCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return currencyCode?.ToString();
        }

        /// <summary>
        /// Get all the countries related to the passed language code.
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public List<Region> GetCountries(string languageCode)
        {
            var countries = new List<Region>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCountriesSp))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.LanguageCode, DbType.String, languageCode);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            countries.Add(masterData.GetCountryData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetCountries",
                    Params = $"{languageCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return countries;
        }

        /// <summary>
        /// Get Region ID from the passed Activity ID
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public int GetRegionIdFromGeotree(int activityId)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetRegionIdFromGeotreeSp))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.ServiceId, DbType.Int32, activityId);
                    return Convert.ToInt32(IsangoDataBaseLive.ExecuteScalar(command) ?? -1);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetRegionIdFromGeotree",
                    Params = $"{activityId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get all the tickets of the theme park for the country.
        /// </summary>
        /// <returns></returns>
        public List<TicketByRegion> GetFilteredThemeparkTickets()
        {
            var ticketsByRegion = new List<TicketByRegion>();
            try
            {
                using (var cmdStatement = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetFilteredThemeparkTicketsSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmdStatement))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            ticketsByRegion.Add(masterData.GetFilteredThemeParkTicketsData(reader));
                        }
                    }
                    return ticketsByRegion;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetFilteredThemeparkTickets",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get region details data.
        /// </summary>
        /// <returns></returns>
        public List<LatLongVsurlMapping> GetRegionData()
        {
            var listOfCoordVsUrlMapping = new List<LatLongVsurlMapping>();
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetRegionCoordinatesSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            var coordVsRegionUrlMapping = masterData.GetRegionData(reader);
                            if (coordVsRegionUrlMapping != null)
                                listOfCoordVsUrlMapping.Add(coordVsRegionUrlMapping);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetRegionData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return listOfCoordVsUrlMapping;
        }

        /// <summary>
        /// This method will return list of the country and its service no.
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetSupportPhonesWithCountryCode(string affiliateId, string language)
        {
            var supportNumber = new Dictionary<string, string>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAffiliateServiceNo))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.AffiliateId, DbType.String, affiliateId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.LanguageCode, DbType.String, language);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            var countryCodeData = masterData.GetSupportPhonesWithCountryCodeData(reader);
                            foreach (var item in countryCodeData)
                            {
                                supportNumber.Add(item.Key, item.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetSupportPhonesWithCountryCode",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return supportNumber;
        }

        /// <summary>
        /// This method will return all the cross sell product list.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, List<CrossSellProduct>> GetAllCrossSellProducts()
        {
            Dictionary<int, List<CrossSellProduct>> allXProducts;
            try
            {
                using (var databaseCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCrossSale))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(databaseCommand))
                    {
                        var masterData = new MasterData();
                        allXProducts = masterData.GetCrossSellProductData(reader);
                    }
                }
                allXProducts.Remove(0);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetAllCrossSellProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return allXProducts;
        }

        public List<CrossSellLogic> GetCrossSellLogic()
        {
            List<CrossSellLogic> crossSellLogics;
            //var crossSellDataFetchingTime = Stopwatch.StartNew();
            using (var databaseCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCrossSaleLogic))
            {
                using (var reader = IsangoDataBaseLive.ExecuteReader(databaseCommand))
                {
                    var masterData = new MasterData();
                    crossSellLogics = masterData.GetCrossSellLogicData(reader);
                }
            }
            //crossSellDataFetchingTime.Stop();
            //var timeElapsed = crossSellDataFetchingTime.Elapsed;
            return crossSellLogics;
        }

        /// <summary>
        /// This method returns the list of the Site map data.
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <param name="siteType"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public Tuple<List<SiteMapData>, int> GetSiteMapData(string affiliateId, string languageCode = "en")
        {
            var siteMapDataList = new List<SiteMapData>();
            int siteType;
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetWebsiteSiteMap))
                {
                    IsangoDataBaseLive.AddInParameter(cmd, Constant.AffiliateId, DbType.String, affiliateId);
                    IsangoDataBaseLive.AddInParameter(cmd, Constant.LanguageCode, DbType.String, languageCode);
                    IsangoDataBaseLive.AddOutParameter(cmd, Constant.SiteType, DbType.Int32, 2);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            siteMapDataList.Add(masterData.GetSiteMapData(reader));
                        }
                        reader.Close();
                        siteType = Convert.ToInt32(IsangoDataBaseLive.GetParameterValue(cmd, "@SiteType"));
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetSiteMapData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            var tuple = Tuple.Create(siteMapDataList, siteType);
            return tuple;
        }

        /// <summary>
        /// This method returns list url for redirection of attractions
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> LoadIndexedAttractionToRegionUrls()
        {
            var indexedAttractionToRegionUrls = new Dictionary<string, string>();
            try
            {
                using (var databaseCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAttractionUrLtoRedirectList))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(databaseCommand))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            var urlData = masterData.IndexedAttractionToRegionUrlsData(reader);

                            foreach (var url in urlData)
                            {
                                if (!string.IsNullOrEmpty(url.Key) && !string.IsNullOrEmpty(url.Value) && !indexedAttractionToRegionUrls.ContainsKey(url.Key))
                                {
                                    indexedAttractionToRegionUrls.Add(url.Key, url.Value);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadIndexedAttractionToRegionUrls",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return indexedAttractionToRegionUrls;
        }

        /// <summary>
        /// This method returns list of the currency exchange table.
        /// </summary>
        /// <returns></returns>
        public List<CurrencyExchangeRates> LoadExchangeRates()
        {
            var currencyExchangeRates = new List<CurrencyExchangeRates>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetExchangeRatesForChangeCurrency))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            currencyExchangeRates.Add(masterData.ExchangeRangeData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadExchangeRates",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return currencyExchangeRates;
        }

        /// <summary>
        /// Returns a list of all the supported languages by the isango platform
        /// </summary>
        /// <returns>List of all the supported languages</returns>
        public List<Language> GetSupportedLanguages()
        {
            var languages = new List<Language>();
            try
            {
                using (var reader = IsangoDataBaseLive.ExecuteReader(Constant.GetSupportedLanguagesSp))
                {
                    var masterData = new MasterData();
                    while (reader.Read())
                    {
                        languages.Add(masterData.SupportedLanguageData(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetSupportedLanguages",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return languages;
        }

        /// <summary>
        /// Get master facilities for all hotels
        /// </summary>
        /// <returns></returns>
        public List<Facility> GetAllFacilities()
        {
            var facilities = new List<Facility>();
            try
            {
                using (var reader = IsangoDataBaseLive.ExecuteReader(Constant.GetFacilityHotelSp))
                {
                    var masterData = new MasterData();
                    while (reader.Read())
                    {
                        facilities.Add(masterData.GetAllFacilityData(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetAllFacilities",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return facilities;
        }

        /// <summary>
        /// This method returns list of region Category similar products.
        /// </summary>
        /// <returns></returns>
        public List<RegionCategorySimilarProducts> GetRegionCategoryMapping()
        {
            var listRegionCategory = new List<RegionCategorySimilarProducts>();
            try
            {
                using (var databaseCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAlternateWorkFlowCategory))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(databaseCommand))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            listRegionCategory.Add(masterData.GetRegionCategoryMappingData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetRegionCategoryMapping",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return listRegionCategory;
        }

        /// <summary>
        /// This method will return the list of the net prices of Product with affiliate commission.
        /// </summary>
        /// <returns></returns>
        public List<NetPriceMasterData> LoadNetPriceMasterData()
        {
            var netPriceMasterDataList = new List<NetPriceMasterData>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetProductWithAffiliateCommission))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            netPriceMasterDataList.Add(masterData.LoadNetPriceMasterData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadNetPriceMasterData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return netPriceMasterDataList;
        }

        public List<IsangoHBProductMapping> LoadFactSheetMapping()
        {
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetHBLiveService))
                {
                    var mappingCollection = new List<IsangoHBProductMapping>();

                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            mappingCollection.Add(masterData.LoadFactSheetMappingData(reader));
                        }
                    }
                    return mappingCollection;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadFactSheetMapping",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Load Region Vs Destination
        /// </summary>
        /// <returns>List of mapped regions</returns>
        public List<MappedRegion> RegionVsDestination()
        {
            var mappedRegions = new List<MappedRegion>();
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetHbRegionMappingSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            masterData.MappedRegionVsDestinationData(reader, ref mappedRegions);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "RegionVsDestination",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return mappedRegions;
        }

        /// <summary>
        /// Load mapped language
        /// </summary>
        /// <returns>List of mapped languages</returns>
        public List<MappedLanguage> LoadMappedLanguage()
        {
            var mappedLanguages = new List<MappedLanguage>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAffiliateExtendedLanguageMappingSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            mappedLanguages.Add(masterData.LoadMappedLanguageData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadMappedLanguage",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return mappedLanguages;
        }

        /// <summary>
        /// Get list of age groups by activities.
        /// </summary>
        /// <returns></returns>
        public List<AgeGroup> GetGliAgeGroupsByActivity()
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetGliAgeGroupsAllActivities))
                {
                    var ageGroups = new List<AgeGroup>();
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            var ageGroup = masterData.GetGliAgeGroupByActivityData(reader);
                            ageGroups.Add(ageGroup);
                        }
                    }
                    return ageGroups;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetGliAgeGroupsByActivity",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public List<AgeGroup> GetPrioAgeGroupsByActivity()
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetPrioAgeGroupsAllActivities))
                {
                    var ageGroups = new List<AgeGroup>();
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            ageGroups.Add(masterData.GetPrioAgeGroupByActivityData(reader));
                        }
                    }
                    return ageGroups;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetPrioAgeGroupsByActivity",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public List<AgeGroup> GetPrioHubAgeGroupsByActivity()
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetPrioHubAgeGroupsAllActivities))
                {
                    var ageGroups = new List<AgeGroup>();
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            ageGroups.Add(masterData.GetPrioHubAgeGroupByActivityData(reader));
                        }
                    }
                    return ageGroups;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetPrioAgeGroupsByActivity",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public List<AgeGroup> GetAotAgeGroupsByActivity()
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAotAgeGroupsAllActivities))
                {
                    var ageGroups = new List<AgeGroup>();
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            ageGroups.Add(masterData.GetAotAgeGroupByActivityData(reader));
                        }
                    }
                    return ageGroups;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetAotAgeGroupsByActivity",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get FareHarbor Customer Prototype For All ActivitiesSp
        /// </summary>
        /// <returns></returns>
        public List<CustomerPrototype> GetCustomerPrototypeByActivity()
        {
            var customerPrototypes = new List<CustomerPrototype>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetFareHarborCustomerPrototypeForAllActivitiesSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            try
                            {
                                customerPrototypes.Add(masterData.GetCustomerPrototypeByActivityData(reader));
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetCustomerPrototypeByActivity",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return customerPrototypes;
        }

        /// <summary>
        /// Get fare harbor age groups by activity
        /// </summary>
        /// <returns></returns>
        public List<FareHarborAgeGroup> GetFareHarborAgeGroupsByActivity()
        {
            var ageGroups = new List<FareHarborAgeGroup>();
            try
            {
                using (var command =
                IsangoDataBaseLive.GetStoredProcCommand(Constant.GetFareHarborAgeGroupForAllActivitiesSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            ageGroups.Add(masterData.GetFareHarborAgeGroupsByActivityData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetFareHarborAgeGroupsByActivity",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return ageGroups;
        }

        /// <summary>
        /// Get all fare harbor user keys
        /// </summary>
        /// <returns></returns>
        public List<FareHarborUserKey> GetAllFareHarborUserKeys()
        {
            var userKeys = new List<FareHarborUserKey>();
            try
            {
                using (var command =
                IsangoDataBaseLive.GetStoredProcCommand(Constant.GetFareHarborKeySp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            userKeys.Add(masterData.GetAllFareHarborUserKeysData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetAllFareHarborUserKeys",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return userKeys;
        }

        /// <summary>
        /// Get Price Category By Activity Bokun
        /// </summary>
        /// <returns></returns>
        public List<PriceCategory> GetBokunPriceCategoryByActivity()
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetBokunPriceCategoryForAllActivities))
                {
                    var priceCategories = new List<PriceCategory>();
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            var priceCategory = masterData.GetPricingCategoryData(reader);
                            priceCategories.Add(priceCategory);
                        }
                    }
                    return priceCategories;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetBokunPriceCategoryByActivity",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public List<LocalizedDestinations> GetLocalizedDestinations()
        {
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.uspGetLivecitiesAPI))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var masterData = new MasterData();
                        return masterData.GetLocalizedDestinations(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetLocalizedDestinations",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public List<LocalizedCategories> GetLocalizedCategories()
        {
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.uspGetLivecategoriesAPI))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var masterData = new MasterData();
                        return masterData.GetLocalizedCategories(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetLocalizedCategories",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get Url vs Page Id mapping.
        /// </summary>
        /// <returns></returns>
        public List<UrlPageIdMapping> UrlPageIdMappingList()
        {
            var UrlVsPageIDList = new List<UrlPageIdMapping>();
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.SPGetPageURList))
                {
                    using (IDataReader reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            UrlVsPageIDList.Add(masterData.GetUrlPageID(reader));
                        }
                    }
                    return UrlVsPageIDList;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "UrlPageIdMappingList",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Load Hotel Beds authorization Data
        /// </summary>
        /// <returns></returns>
        public List<HotelBedsCredentials> LoadHBauthData()
        {
            var authData = new List<HotelBedsCredentials>();
            try
            {
                var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetHotelBedsCredentialsSp);
                using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                {
                    var masterData = new MasterData();
                    while (reader.Read())
                    {
                        authData.Add(masterData.GetHotelBedsCredentials(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadHBauthData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return authData;
        }

        /// <summary>
        /// Load Gli Pickup Location For All Activities
        /// </summary>
        /// <returns></returns>
        public List<PickupLocation> GetPickupLocationsByActivity()
        {
            var pickupLocations = new List<PickupLocation>();
            try
            {
                var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetGliPickupLocationForAllActivitiesSp);
                using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                {
                    var masterData = new MasterData();
                    while (reader.Read())
                    {
                        pickupLocations.Add(masterData.GetPickupLocationsByActivity(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetPickupLocationsByActivity",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return pickupLocations;
        }

        public List<Entities.Ventrata.SupplierDetails> GetVentrataSupplierDetails()
        {
            var supplierDetails = new List<Entities.Ventrata.SupplierDetails>();
            try
            {
                var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.VentrataSupplierDetailsProcedure);
                using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                {
                    var masterData = new MasterData();
                    while (reader.Read())
                    {
                        supplierDetails.Add(masterData.GetVentrataSupplierDetail(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetVentrataSupplierDetails",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return supplierDetails;
        }

        /// <summary>
        /// Load AOT FactSheet Mappings
        /// </summary>
        /// <returns></returns>
        public List<IsangoHBProductMapping> LoadLiveHBOptions()
        {
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetHBLiveOptionsSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var masterData = new MasterData();
                        return masterData.LoadLiveHBOptionsData(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadLiveHBOptions",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get Mappings Details
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="activityOptionId"></param>
        /// <param name=""></param>
        /// <returns>List of all current mappings </returns>
        public List<IsangoHBProductMapping> LoadLiveHBOptions(int? activityId, int? activityOptionId)
        {
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetHBLiveOptionsSp))
                {
                    if (activityOptionId > 0)
                    {
                        IsangoDataBaseLive.AddInParameter(cmd, "@ServiceOptionInServiceid", DbType.Int32, activityOptionId);
                    }
                    if (activityId > 0)
                    {
                        IsangoDataBaseLive.AddInParameter(cmd, "@Serviceid", DbType.Int32, activityId);
                    }
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var masterData = new MasterData();
                        return masterData.LoadLiveHBOptionsData(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadLiveHBOptions",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public int GetActivityIdByOptionId(int? activityOptionId)
        {
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetActivityIdFromOptionId))
                {
                    IsangoDataBaseLive.AddInParameter(cmd, "@serviceOptionID", DbType.Int32, activityOptionId);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var masterData = new MasterData();
                        var activityId = masterData.GetActivityIdByOptionId(reader);
                        return activityId;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetActivityIdByOptionId",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save tokenId and availablityRefereceIds in db
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="availablityRefereceIds"></param>
        public void SaveTokenAndRefIds(string tokenId, List<string> availablityRefereceIds)
        {
            availablityRefereceIds = availablityRefereceIds?.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()?.Select(x => x.Trim())?.ToList();
            var refIdsJoined = string.Join(",", availablityRefereceIds);
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertTokenAndRefIds))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, "@tokenID", DbType.String, tokenId);
                    IsangoDataBaseLive.AddInParameter(command, "@AvailabilityRefID", DbType.String, refIdsJoined);
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "SaveTokenAndRefIds",
                    Token = tokenId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(new { tokenId, availablityRefereceIds })}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
        }

        /// <summary>
        /// Get Token By AvailabilityReferenceId
        /// </summary>
        /// <param name="AvailabilityReferenceId"></param>
        /// <returns></returns>
        public string GetTokenByAvailabilityReferenceId(string AvailabilityReferenceId)
        {
            var result = string.Empty;
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetTokenByAvailabilityReferenceId))
                {
                    IsangoDataBaseLive.AddInParameter(cmd, "@AvailabilityRefID", DbType.String, AvailabilityReferenceId);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var masterData = new MasterData();
                        var tokens = masterData.GetTokenByAvailabilityReferenceId(reader);
                        result = tokens.Where(x => !string.IsNullOrWhiteSpace(x))?.Distinct()?.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetActivityIdByOptionId",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return result;
        }

        public List<TourCMSChannelList> LoadTourCMSSelectedChannel()
        {
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetTourCMSChannelListSP))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var masterData = new MasterData();
                        return masterData.GetTourCMSChannelList(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadTourCMSSelectedChannel",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Load APITUDE Content Mappings
        /// </summary>
        /// <returns></returns>
        public List<IsangoHBProductMapping> LoadLiveHBOptionsApiTudeContent()
        {
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetHBLiveOptionsApiTudeContentSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var masterData = new MasterData();
                        return masterData.LoadLiveHBOptionsDataApitudeContent(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadLiveHBOptionsApiTudeContent",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the Delta Affiliate table.
        /// </summary>
        /// <returns></returns>
        public AffiliateAPI LoadDeltaAffiliate()
        {
            var affiliateAPI = new AffiliateAPI();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetDeltaAffiliate))
                {
                    command.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        affiliateAPI.AffiliateAPIData = new List<AffiliateAPIData>();
                        affiliateAPI.AffiliateAPIData = masterData.AffiliateAPIData(reader);

                        if (reader.NextResult())
                        {
                            affiliateAPI.AffiliateServices = new List<AffiliateServices>();
                            affiliateAPI.AffiliateServices = masterData.AffiliateServiceData(reader);
                        }
                        if (reader.NextResult())
                        {
                            affiliateAPI.AffiliateEmail = new List<AffiliateEmail>();
                            affiliateAPI.AffiliateEmail = masterData.AffiliateEmailData(reader);
                        }
                        if (reader.NextResult())
                        {
                            affiliateAPI.AffiliatePhone = new List<AffiliatePhone>();
                            affiliateAPI.AffiliatePhone = masterData.AffiliatePhoneData(reader);
                        }
                        if (reader.NextResult())
                        {
                            affiliateAPI.AffiliateCurrency = new List<AffiliateCurrency>();
                            affiliateAPI.AffiliateCurrency = masterData.AffiliateCurrencyData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadDeltaAffiliate",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return affiliateAPI;
        }

        /// <summary>
        /// This method return blog data.
        /// </summary>
        /// <returns></returns>
        public List<BlogData> GetBlogs()
        {
            var BlogData = new List<BlogData>();
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetBlogs))
                {
                    dbCmd.CommandType = CommandType.StoredProcedure;
                    dbCmd.CommandTimeout = 300;
                    using (var dataSet = IsangoDataBaseLive.ExecuteDataSet(dbCmd))
                    {
                        var masterData = new MasterData();
                        if (dataSet.Tables.Count > 0)
                        {
                            BlogData = masterData.MapBlogData(dataSet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetBlogs",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return BlogData;
        }

        /// <summary>
        /// Dictionary to be used canonicalising Product page urls, Isango
        /// </summary>
        /// <returns></returns>
        public List<SEOCanonical> LoadCanonicalVsIdMapping()
        {
            List<SEOCanonical> ListOfCanonicalData = new List<SEOCanonical>();
            try
            {
                var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetMicrositeCanonicalsURL);
                using (IDataReader reader = IsangoDataBaseLive.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        SEOCanonical CanonicalVsIdMapping = new SEOCanonical();
                        if (!string.IsNullOrEmpty(reader["serviceId"].ToString().Trim()))
                            CanonicalVsIdMapping.ServiceId = Convert.ToInt32((reader["serviceId"].ToString().Trim()));
                        if (!string.IsNullOrEmpty(reader["CanonicalURL"].ToString().Trim()))
                            CanonicalVsIdMapping.CanonicalUrl = reader["canonicalurl"].ToString().Trim();
                        if (!string.IsNullOrEmpty(reader["LanguageCode"].ToString().Trim()))
                            CanonicalVsIdMapping.LanguageCode = reader["LanguageCode"].ToString().Trim();

                        ListOfCanonicalData.Add(CanonicalVsIdMapping);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadCanonicalVsIdMapping",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return ListOfCanonicalData;
        }

        /// <summary>
        /// This method returns list of the Localized Merchandising Mapping table.
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public List<LocalizedMerchandising> LoadDeltaAttractions()
        {
            var localizedMerchandising = new List<LocalizedMerchandising>();
            var localizedMerchandisingFilter = new List<LocalizedMerchandising>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetDeltaAttractions))
                {
                    command.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            localizedMerchandising.Add(masterData.AttractionsData(reader));
                        }
                    }
                }

                foreach (var item in localizedMerchandising)
                {
                    if (item.Language.ToLower() == "en")
                    {
                        localizedMerchandisingFilter.Add(item);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(item.Language) && !String.IsNullOrEmpty(item.MultilingualName))
                        {
                            localizedMerchandisingFilter.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadDeltaAttractions",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return localizedMerchandisingFilter;
        }

        /// <summary>
        /// This method returns list of the Region Category Mapping table.
        /// </summary>
        /// <returns></returns>
        public List<RegionCategoryMapping> LoadDeltaRegionAttraction()
        {
            var regionCategoryMapping = new List<RegionCategoryMapping>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetDeltaRegionAttraction))
                {
                    command.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            regionCategoryMapping.Add(masterData.RegionCategoryMappingData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadDeltaRegionAttraction",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return regionCategoryMapping;
        }

        /// <summary>
        /// private This method returns list  of the  Delta Region Sub  Attraction Mapping table.
        /// </summary>
        /// <returns></returns>

        public List<RegionSubAttraction> LoadDeltaRegionSubAttraction()
        {
            var regionSubAttraction = new List<RegionSubAttraction>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetDeltaRegionSubAttraction))
                {
                    command.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            regionSubAttraction.Add(masterData.RegionSubAttractionData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadDeltaRegionSubAttraction",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return regionSubAttraction;
        }

        /// <summary>
        /// This method returns list of the Marketing CJFeed Mapping table.
        /// </summary>
        /// <returns></returns>
        public string LoadMarketingCJFeed(int currencyid)
        {
            string marketingCJFeed;
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetMarketingCJFeed))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.Currencyid, DbType.Int32, currencyid);
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        marketingCJFeed = masterData.CJFeedMappingData(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadMarketingCJFeed",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return marketingCJFeed;
        }

        /// <summary>
        /// This method returns list of the Pax Detail Mapping of Tiqets API.
        /// </summary>
        /// <returns></returns>
        public List<TiqetsPaxMapping> LoadTiqetsPaxMappings()
        {
            var paxMappings = new List<TiqetsPaxMapping>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.APIAgeGroupSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            try
                            {
                                paxMappings.Add(masterData.TiqetsPaxMappingData(reader));
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadTiqetsPaxMappings",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return paxMappings;
        }

        public List<VentrataPackages> LoadVentrataPackages(string productid, string optionid)
        {
            var packages = new List<VentrataPackages>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.ventrataPackages))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParentProductId, DbType.String, productid);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParentOptionId, DbType.String, optionid);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            packages.Add(masterData.VentrataPackages(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadVentrataPackages",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return packages;
        }


        public List<TiqetsPackage> LoadTiqetsPackages(string Product_ID)
        {
            var packages = new List<TiqetsPackage>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.TiqetsPackages))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.Package_ID, DbType.Int32, Product_ID);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            packages.Add(masterData.TiqetsPackages(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadTiqetsPackages",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return packages;
        }
        ///// <summary>
        ///// This method returns list of the Badge table.
        ///// </summary>
        ///// <returns></returns>
        //public List<Badge> LoadBadge()
        //{
        //    var badge = new List<Badge>();
        //    using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetBadge))
        //    {
        //        using (var reader = IsangoDataBaseLive.ExecuteReader(command))
        //        {
        //            var masterData = new MasterData();
        //            while (reader.Read())
        //            {
        //                badge.Add(masterData.BadgeData(reader));
        //            }
        //        }
        //    }
        //    return badge;
        //}

        /// <summary>
        /// This method returns list of the Marketing Criteo Mapping table.
        /// </summary>
        /// <returns></returns>
        public List<CJFeedProduct> LoadMarketingCriteoFeed(string currencycode)
        {
            var cJFeedProduct = new List<CJFeedProduct>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetMarketingCriteoFeed))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.Currecycode, DbType.String, currencycode);
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            cJFeedProduct.Add(masterData.CJFeedProductMappingData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadMarketingCriteoFeed",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return cJFeedProduct;
        }

        /// <summary>
        /// This method retrieves the passenger mappings
        /// </summary>
        /// <returns></returns>
        public List<GoldenTours.PassengerMapping> GetPassengerMapping()
        {
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetApiPassengerType))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var masterData = new MasterData();
                        var passengerMappings = masterData.GetPassengerMappingForGoldenTours(reader);
                        return passengerMappings;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetPassengerMapping",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// public This method returns list  of the  Delta GeoDetails table.
        /// </summary>
        /// <returns></returns>

        public List<GeoDetails> LoadDeltaGeoDetails()
        {
            var geoDetail = new List<GeoDetails>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetDeltaGeoDetails))
                {
                    command.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            geoDetail.Add(masterData.GeoDetailsData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadDeltaGeoDetails",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return geoDetail;
        }

        /// <summary>
        /// public This method returns list  of the  Delta Destination table.
        /// </summary>
        /// <returns></returns>

        public List<Entities.Destination> LoadDeltaDestination()
        {
            var destination = new List<Entities.Destination>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetDeltaDestination))
                {
                    command.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            destination.Add(masterData.GeoDestinationData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadDeltaDestination",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return destination;
        }

        /// <summary>
        /// public This method returns list  of the  delta product supplier table.
        /// </summary>
        /// <returns></returns>

        public List<ProductSupplier> LoadDeltaProductSupplier()
        {
            var productSupplier = new List<ProductSupplier>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetDeltaProductSupplier))
                {
                    command.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            productSupplier.Add(masterData.GeoProductSupplierData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadDeltaProductSupplier",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return productSupplier;
        }

        /// <summary>
		/// public This method returns list  of the master Currency
		/// </summary>
		/// <returns></returns>
        public List<Currency> LoadMasterCurrency()
        {
            var currency = new List<Currency>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetMasterCurrency))
                {
                    command.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            currency.Add(masterData.CurrencyData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadMasterCurrency",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return currency;
        }

        /// <summary>
        /// public This method returns list  of the master languages
        /// </summary>
        /// <returns></returns>
        public List<Language> LoadMasterLanguages()
        {
            var language = new List<Language>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetMasterLanguages))
                {
                    command.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            language.Add(masterData.LanguageData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadMasterLanguages",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return language;
        }

        /// <summary>
        /// public This method returns list  of the master geo detail
        /// </summary>
        /// <returns></returns>
        public List<GeoDetails> LoadMasterGeoDetail(string language)
        {
            var geodetail = new List<GeoDetails>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetMasterGeoDetails))
                {
                    if (!string.IsNullOrEmpty(language))
                    {
                        IsangoDataBaseLive.AddInParameter(command, Constant.LanguageCode, DbType.String, language);
                        command.CommandType = CommandType.StoredProcedure;
                    }
                    command.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            geodetail.Add(masterData.GeoDetailsMasterData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadMasterGeoDetail",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return geodetail;
        }

        /// <summary>
        /// public This method returns list  of the master region wise product detail
        /// </summary>
        /// <returns></returns>
        public List<RegionWiseProductDetails> LoadMasterRegionWise(string affiliateId, string categoryid = null)
        {
            var regionWiseProductDetails = new List<RegionWiseProductDetails>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetMasterRegionWise))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.AffiliateId, DbType.String, affiliateId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Categoryid, DbType.String, categoryid);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            regionWiseProductDetails.Add(masterData.RegionWiseMasterData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadMasterRegionWise",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return regionWiseProductDetails;
        }

        public List<ServiceCancellationPolicy> GetServiceCancellationPolicies()
        {
            var cancellationPolicies = new List<ServiceCancellationPolicy>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.ServiceCancellationPolicy))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            cancellationPolicies.Add(masterData.ServiceCancellationPolicyData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetServiceCancellationPolicies",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return cancellationPolicies;
        }

        public List<RezdyLabelDetail> GetRezdyLabelDetails()
        {
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.RezdyLabelDetails))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var masterData = new MasterData();
                        var labelDetailsMappings = masterData.GetLabelDetailsMappingForRezdy(reader);
                        return labelDetailsMappings;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetRezdyLabelDetails",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public List<RezdyPaxMapping> GetRezdyPaxMappings()
        {
            var paxMappings = new List<RezdyPaxMapping>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.APIAgeGroupSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            paxMappings.Add(masterData.RezdyPaxMappingData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetRezdyPaxMappings",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return paxMappings;
        }

        public List<TourCMSMapping> GetTourCMSPaxMappings()
        {
            var paxMappings = new List<TourCMSMapping>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.APIAgeGroupSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            paxMappings.Add(masterData.TourCMSPaxMappingData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetTourCMSPaxMappings",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return paxMappings;
        }

        public List<VentrataPaxMapping> GetVentrataPaxMappings()
        {
            var paxMappings = new List<VentrataPaxMapping>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.APIAgeGroupSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            paxMappings.Add(masterData.VentrataPaxMappingData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetVentrataPaxMappings",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return paxMappings;
        }
        public List<GlobalTixV3Mapping> GetGlobalTixV3PaxMappings()
        {
            var paxMappings = new List<GlobalTixV3Mapping>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.APIAgeGroupSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            paxMappings.Add(masterData.GlobalTixV3PaxMappingData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetGlobalTixV3PaxMappings",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return paxMappings;
        }

        public List<string> LoadAgeDumpngAPIs()
        {
            List<string> ageDumpingAPIsArray = new List<string>();
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.AgeDumpingAPIsProcedure))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            ageDumpingAPIsArray.Add(masterData.LoadAgeDumpngAPIs(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadAgeDumpngAPIs",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return ageDumpingAPIsArray;
        }

        public Tuple<List<APIImages>, List<APIImages>> GetAPIImages()
        {
            List<APIImages> aPIImages = new List<APIImages>();
            List<APIImages> aPIImagesToDelete = new List<APIImages>();
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.APIImagesProcedure))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            aPIImages.Add(masterData.LoadAPIImages(reader));
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            aPIImagesToDelete.Add(masterData.LoadAPIImagesToDelete(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetAPIImages",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return new Tuple<List<APIImages>, List<APIImages>>(aPIImages, aPIImagesToDelete);
        }

        public void SaveImagesUploadResult(List<ImagesUploadResult> imagesUploadResults, List<ImagesDeleteResult> imagesDeleteResults)
        {
            var dataTable = new DataTable { TableName = "dbo.APIimagelist" };
            try
            {
                dataTable.Columns.Add(new DataColumn("serviceid", typeof(int)));
                dataTable.Columns.Add(new DataColumn("imagekey", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Imagepath", typeof(string)));
                dataTable.Columns.Add(new DataColumn("imagesorder", typeof(int)));
                dataTable.Columns.Add(new DataColumn("APIurl", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Apitypeid", typeof(int)));
                dataTable.Columns.Add(new DataColumn("Supplierproductid", typeof(string)));

                foreach (var result in imagesUploadResults)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in result.GetType().GetFields())
                    {
                        newRow[property.Name] = result.GetType().GetField(property.Name)
                            ?.GetValue(result);
                    }
                    dataTable.Rows.Add(newRow);
                }

                if (dataTable.Rows.Count == 0)
                {
                    var newRow = dataTable.NewRow();
                    newRow["serviceid"] = 0;
                    newRow["imagekey"] = "";
                    newRow["Imagepath"] = "";
                    newRow["imagesorder"] = 0;
                    newRow["APIurl"] = "";
                    newRow["Apitypeid"] = 0;
                    newRow["Supplierproductid"] = "";
                    dataTable.Rows.Add(newRow);
                }

                var dataTableDelete = new DataTable { TableName = "dbo.APIDeleteimagelist" };
                try
                {
                    dataTableDelete.Columns.Add(new DataColumn("ID", typeof(int)));
                    dataTableDelete.Columns.Add(new DataColumn("serviceid", typeof(int)));
                    dataTableDelete.Columns.Add(new DataColumn("Apitypeid", typeof(int)));
                    dataTableDelete.Columns.Add(new DataColumn("Supplierproductid", typeof(string)));
                    dataTableDelete.Columns.Add(new DataColumn("CloudinaryURL", typeof(string)));

                    foreach (var result in imagesDeleteResults)
                    {
                        var newRow = dataTableDelete.NewRow();
                        foreach (var property in result.GetType().GetFields())
                        {
                            newRow[property.Name] = result.GetType().GetField(property.Name)
                                ?.GetValue(result);
                        }
                        dataTableDelete.Rows.Add(newRow);
                    }
                }
                catch (Exception ex)
                {
                    //ignore
                }

                if (dataTableDelete.Rows.Count == 0)
                {
                    var newRow = dataTableDelete.NewRow();
                    newRow["ID"] = 0;
                    newRow["serviceid"] = 0;
                    newRow["Apitypeid"] = 0;
                    newRow["Supplierproductid"] = "";
                    newRow["CloudinaryURL"] = "";
                    dataTableDelete.Rows.Add(newRow);
                }

                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.APIImagesUploadResultProcedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.APIImagesUploadResulttParameter, dataTable);
                    var tvpFactSheet2 = insertCommand.Parameters.AddWithValue(Constant.APIImagesDeleteResulttParameter, dataTableDelete);
                    tvpFactSheet.SqlDbType = SqlDbType.Structured;
                    tvpFactSheet2.SqlDbType = SqlDbType.Structured;

                    try
                    {
                        connection.Open();
                        insertCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "SaveImagesUploadResult",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method returns list of the currency exchange table.
        /// </summary>
        /// <returns></returns>
        public List<ElasticDestinations> LoadElasticSearchDestinations()
        {
            var elasticDestinations = new List<ElasticDestinations>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetElasticDestinations))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            elasticDestinations.Add(masterData.ElasticSearchDestinations(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadElasticSearchDestinations",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return elasticDestinations;
        }

        public List<ElasticProducts> LoadElasticSearchProducts()
        {
            var elasticProducts = new List<ElasticProducts>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetElasticProducts))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            elasticProducts.Add(masterData.ElasticSearchProducts(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadElasticSearchProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return elasticProducts;
        }

        public List<ElasticAttractions> LoadElasticSearchAttractions()
        {
            var elasticAttractions = new List<ElasticAttractions>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetElasticAttraction))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            elasticAttractions.Add(masterData.ElasticSearchAttractions(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadElasticSearchAttractions",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return elasticAttractions;
        }

        public List<ElasticAffiliate> LoadElasticAffiliate()
        {
            var elasticAffiliate = new List<ElasticAffiliate>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetElasticAffiliate))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var masterData = new MasterData();
                        while (reader.Read())
                        {
                            elasticAffiliate.Add(masterData.ElasticAffiliate(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "LoadElasticSearchAttractions",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return elasticAffiliate;
        }
        /* public List<AffiliateDetails> LoadAffiliateDetails()
         {
             var affiliateDetails = new List<AffiliateDetails>();
             try
             {
                 using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.AffiliateDetail))
                 {
                     using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                     {
                         var masterData = new MasterData();
                         while (reader.Read())
                         {
                             affiliateDetails.Add(masterData.AffiliateDetail(reader));
                         }
                     }
                 }
             }
             catch (Exception ex)
             {
                 var isangoErrorEntity = new IsangoErrorEntity
                 {
                     ClassName = "MasterPersistence",
                     MethodName = "LoadAfiliateDetails",
                 };
                 _log.Error(isangoErrorEntity, ex);
                 throw;
             }
             return affiliateDetails;
         }*/

        public void SaveElasticDestination(List<DestinationDatum> datum)
        {
            var destinationDataTable = SetElasticDestination(datum);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand("Elastic_usp_ins_Destination", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue("@ElasticDestination", destinationDataTable);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = "[dbo].[ElasticDestination]";
                    try
                    {
                        connection.Open();
                        insertCommand.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "SaveElasticDestination",
                    Params = $"{SerializeDeSerializeHelper.Serialize(datum)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private DataTable SetElasticDestination(List<DestinationDatum> datum)
        {
            var dataTable = new DataTable { TableName = "ElasticDestination" };
            try
            {
                foreach (var property in datum[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var productDetail in datum)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in productDetail.GetType().GetProperties())
                    {
                        newRow[property.Name] = productDetail.GetType().GetProperty(property.Name)
                            ?.GetValue(productDetail, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "SetProductDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(datum)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        public void SaveElasticProducts(List<ProductDatum> datum)
        {
            var productDataTable = SetElasticProduct(datum);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand("Elastic_usp_ins_Product", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue("@ElasticProduct", productDataTable);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = "[dbo].[ElasticProduct]";
                    try
                    {
                        connection.Open();
                        insertCommand.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "SaveElasticProducts",
                    Params = $"{SerializeDeSerializeHelper.Serialize(datum)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private DataTable SetElasticProduct(List<ProductDatum> datum)
        {
            var dataTable = new DataTable { TableName = "ElasticProduct" };
            try
            {
                foreach (var property in datum[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var productDetail in datum)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in productDetail.GetType().GetProperties())
                    {
                        newRow[property.Name] = productDetail.GetType().GetProperty(property.Name)
                            ?.GetValue(productDetail, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "SetElasticProduct",
                    Params = $"{SerializeDeSerializeHelper.Serialize(datum)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        public void SaveElasticAttraction(List<AttractionDatum> datum)
        {
            var attractionDataTable = SetElasticAttraction(datum);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand("Elastic_usp_ins_Attraction", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue("@ElasticAttraction", attractionDataTable);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = "[dbo].[ElasticAttraction]";
                    try
                    {
                        connection.Open();
                        insertCommand.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "SaveElasticAttraction",
                    Params = $"{SerializeDeSerializeHelper.Serialize(datum)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private DataTable SetElasticAttraction(List<AttractionDatum> datum)
        {
            var dataTable = new DataTable { TableName = "ElasticAttraction" };
            try
            {
                foreach (var property in datum[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var productDetail in datum)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in productDetail.GetType().GetProperties())
                    {
                        newRow[property.Name] = productDetail.GetType().GetProperty(property.Name)
                            ?.GetValue(productDetail, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "SetElasticAttraction",
                    Params = $"{SerializeDeSerializeHelper.Serialize(datum)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        #endregion Public methods

        public AvailablePersonTypes GetPersonTypeOptionCacheAvailability(int? activityId, int? serviceOptionId, DateTime? fromDate, DateTime? toDate)
        {
            var result = new AvailablePersonTypes
            {
                AvailableDates = new List<PersonTypeOptionCacheAvailability>(),
                AvailablePassengerTypes = new List<PersonTypeOptionDateRanges>(),
            };

            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetPersonTypeOptionCacheAvailability))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, "@serviceId", DbType.Int32, activityId);
                    IsangoDataBaseLive.AddInParameter(dbCmd, "@serviceOptionId", DbType.Int32, serviceOptionId);
                    IsangoDataBaseLive.AddInParameter(dbCmd, "@fromDate", DbType.DateTime, fromDate);
                    IsangoDataBaseLive.AddInParameter(dbCmd, "@toDate", DbType.DateTime, toDate);

                    dbCmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var masterData = new MasterData();
                        result = masterData.ReadPersonTypeOptionCacheAvailability(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetPersonTypeOptionCacheAvailability",
                    Params = $"{activityId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return result;
        }

        public void SaveImageAltText(string ImageName, string AltText)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertImageAltText))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, "@ImageKEY", DbType.String, ImageName);
                    IsangoDataBaseLive.AddInParameter(command, "@AltText", DbType.String, AltText);
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "SaveImageAltText",
                    Params = $"{SerializeDeSerializeHelper.Serialize(new { ImageName, AltText })}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
        }

        public void SaveAllCssExternalProducts(List<ExternalProducts> externalproducts)
        {
            try
            {
                if (externalproducts?.Count > 0)
                {
                    foreach (var externalproduct in externalproducts)
                    {
                        //Prepare Command
                        using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.SaveAllCssExternalProducts))
                        {
                            // Prepare parameter collection
                            IsangoDataBaseLive.AddInParameter(command, Constant.CssProductOptionId, DbType.Int32, externalproduct.CssProductOptionId);
                            IsangoDataBaseLive.AddInParameter(command, Constant.IsangoProductOptionId, DbType.String, externalproduct.IsangoProductOptionId);
                            IsangoDataBaseLive.AddInParameter(command, Constant.productName, DbType.String, externalproduct.productName);
                            IsangoDataBaseLive.AddInParameter(command, Constant.CssProductId, DbType.Int32, externalproduct.CssProductId);
                            IsangoDataBaseLive.AddInParameter(command, Constant.supplierId, DbType.Int32, externalproduct.supplierId);

                            IsangoDataBaseLive.ExecuteNonQuery(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CssExternalProduct",
                    MethodName = "SaveallExternalProduct",
                    Params = $"{SerializeDeSerializeHelper.Serialize(externalproducts)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveAllCssBooking(string IdempotancyKey, string Process, CssBookingDatas data, string Status, string OTAReferenceId, string bookingReferenceNumber = null, bool IsCancelled = false, string cssrequest = null, string cssresponse = null, string barcode = null)
        {
            try
            {

                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.SaveAllCssBooking))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.CssReferenceNumber, DbType.String, bookingReferenceNumber);
                    IsangoDataBaseLive.AddInParameter(command, Constant.IdempotancyKey, DbType.String, IdempotancyKey);
                    IsangoDataBaseLive.AddInParameter(command, Constant.supplierId, DbType.Int32, data.SupplierId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.CssProductId, DbType.Int32, data.CssProductId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Process, DbType.String, Process);

                    IsangoDataBaseLive.AddInParameter(command, Constant.CssProductOptionId, DbType.Int32, data.CssProductOptionId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.IsangoProductOptionId, DbType.Int32, data.serviceoptioninserviceid);
                    IsangoDataBaseLive.AddInParameter(command, "@Status", DbType.String, Status);
                    IsangoDataBaseLive.AddInParameter(command, "@IsCancelled", DbType.Boolean, IsCancelled);
                    IsangoDataBaseLive.AddInParameter(command, "@CssRequest", DbType.String, cssrequest);
                    IsangoDataBaseLive.AddInParameter(command, "@CssResponse", DbType.String, cssresponse);
                    IsangoDataBaseLive.AddInParameter(command, "@OTAReferenceId", DbType.String, OTAReferenceId);
                    IsangoDataBaseLive.AddInParameter(command, "@BarCode", DbType.String, barcode);


                    IsangoDataBaseLive.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CssBooking",
                    MethodName = "SaveAllCssBooking",
                    Params = $"{(IdempotancyKey)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }



        public List<string> IsBookingDoneWithCss(int CssProductOptionId = 0, string CssReferenceNumber = null)
        {
            List<string> cssReferenceNumbers = new List<string>(); // List to hold reference numbers
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.CssBookingRequired))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.CssProductOptionId, DbType.Int32, CssProductOptionId);
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.CssReferenceNumber, DbType.String, CssReferenceNumber);

                    dbCmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        while (reader.Read())
                        {
                            var referenceNumber = Convert.ToString(reader["CssReferenceNumber"]);
                            cssReferenceNumbers.Add(referenceNumber); // Add each reference number to the list
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "IsBookingDoneWithCss",
                    Params = $"{CssReferenceNumber}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return cssReferenceNumbers;
        }

        public List<Entities.Booking.CssCancellation> GetCssCancellation()
        {
            List<Entities.Booking.CssCancellation> cssCancellations = new List<Entities.Booking.CssCancellation>();

            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCssCancellation))
                {
                    dbCmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        while (reader.Read())
                        {
                            Entities.Booking.CssCancellation cssCancellation = new Entities.Booking.CssCancellation
                            {
                                bookedoptionid = Convert.ToString(reader["bookedoptionid"]),
                                CssReferenceNumber = Convert.ToString(reader["CssReferenceNumber"]),
                                SupplierId = Convert.ToInt32(reader["SupplierId"]),
                                isangoserviceoptionid = Convert.ToInt32(reader["serviceoptioninserviceid"]),
                                Barcode = Convert.ToString(reader["barcode"])
                            };

                            cssCancellations.Add(cssCancellation);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetCssExternalOption"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return cssCancellations;
        }
        public CssBookings GetBookingData()
        {
            var bookingDataList = new CssBookings();
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetBookingData))
                {
                    dbCmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var masterData = new MasterData();

                        bookingDataList.cssBookingDatas = masterData.GetCssBookingDatas(reader);
                        if (reader.NextResult())
                        {
                            bookingDataList.cssPassengerDetails = masterData.GetCssPassengerDetail(reader);
                        }
                        if (reader.NextResult())
                        {
                            bookingDataList.cssQrCodes = masterData.GetCssQRCode(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetBookingData",
                    Params = "No parameters"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return bookingDataList;
        }

        public void SaveAllCssCancellation(string referenceNumber, string IdempotancyKey, string Process, int isangooptionid, string OTAReferenceId = null, bool IsCancelled = false, string cssrequest = null, string cssresponse = null)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.SaveAllCssBooking))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.CssReferenceNumber, DbType.String, referenceNumber);
                    IsangoDataBaseLive.AddInParameter(command, Constant.IdempotancyKey, DbType.String, IdempotancyKey);
                    IsangoDataBaseLive.AddInParameter(command, Constant.IsangoProductOptionId, DbType.Int32, isangooptionid);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Process, DbType.String, Process);

                    IsangoDataBaseLive.AddInParameter(command, "@IsCancelled", DbType.Boolean, IsCancelled);
                    IsangoDataBaseLive.AddInParameter(command, "@CssRequest", DbType.String, cssrequest);
                    IsangoDataBaseLive.AddInParameter(command, "@CssResponse", DbType.String, cssresponse);
                    IsangoDataBaseLive.AddInParameter(command, "@OTAReferenceId", DbType.String, OTAReferenceId);



                    IsangoDataBaseLive.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CssBooking",
                    MethodName = "SaveAllCssBooking",
                    Params = "No parameters"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public List<RedemptionRequest> GetRedemptionData()
        {
            var cssRedemptions = new List<RedemptionRequest>();

            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCssRedemption))
                {
                    dbCmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        while (reader.Read())
                        {
                            var cssRedemption = new RedemptionRequest
                            {
                                referenceId = Convert.ToString(reader["CssReferenceNumber"]),
                                otaReferenceId = Convert.ToString(reader["OtaReferenceId"])
                            };

                            cssRedemptions.Add(cssRedemption);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MasterPersistence",
                    MethodName = "GetRedemptionData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return cssRedemptions;
        }

        public bool IsSqlServerHealthy()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig("IsangoLiveDB")))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT 1", connection))
                    {
                        object result = command.ExecuteScalar();
                        return result != null && result != DBNull.Value && Convert.ToInt32(result) == 1;
                    }
                }

            }
            catch (Exception)
            {
                return false;
            }
           
        }

    }
}
