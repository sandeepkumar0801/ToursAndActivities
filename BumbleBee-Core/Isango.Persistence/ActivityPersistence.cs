using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Region;
using Isango.Entities.Rezdy;
using Isango.Persistence.Contract;
using Isango.Persistence.Data;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence
{
    public class ActivityPersistence : PersistenceBase, IActivityPersistence
    {
        private readonly ILogger _log;
        public ActivityPersistence(ILogger log)
        {
           _log = log;
        }
        /// <summary>
        /// This method return activity based on the search criteria
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public SearchResult SearchActivities(SearchCriteria searchCriteria, ClientInfo clientInfo)
        {
            var activitySearchResult = new SearchResult();
            try
            {
                var cloudPath = ConfigurationManagerHelper.GetValuefromAppSettings("CloudinaryUrl");
                var activityIdList = GetFullTextSearchActivitiyIDs(searchCriteria.CategoryId.ToString(), searchCriteria.RegionId.ToString(), searchCriteria.Keyword, clientInfo);
                if (activityIdList != null)
                {
                    foreach (var activityId in activityIdList)
                    {
                        var activity = GetActivity(activityId, clientInfo.LanguageCode);
                        activity?.Images?.RemoveAll(x => x.ImageType != ImageType.CloudProduct);
                        activity?.Images?.ForEach(x => x.Name = cloudPath + x.Name);
                        activitySearchResult.Activities = new List<Activity>
                    {
                        activity
                    };
                    }
                }
                
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "SearchActivities",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(searchCriteria)}, {SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activitySearchResult;
        }

        /// <summary>
        /// This method return activity mapped to region ID
        /// </summary>
        /// <param name="regionIds"></param>
        /// <param name="keywordPhrase"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public List<RegionActivityMapping> GetFullTextSearchActivitiyIdMapping(string regionIds, string keywordPhrase, ClientInfo clientInfo)
        {
            var mappings = new List<RegionActivityMapping>();
            try
            {
                var fulltextPhrase = MakeSearchClause(regionIds, string.Empty, string.Empty, keywordPhrase);
                if (!string.IsNullOrEmpty(fulltextPhrase))
                {
                    using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.SearchProductFullTextSp))
                    {
                        IsangoDataBaseLive.AddInParameter(dbCommand, Constant.LicenseKey, DbType.String, GetLicenseKey(clientInfo));
                        IsangoDataBaseLive.AddInParameter(dbCommand, Constant.Keyword, DbType.String, fulltextPhrase);
                        IsangoDataBaseLive.AddInParameter(dbCommand, Constant.LanguageCode, DbType.String, clientInfo.LanguageCode);
                        dbCommand.CommandType = CommandType.StoredProcedure;

                        using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                        {
                            var activityData = new ActivityData();
                            while (reader.Read())
                            {
                                mappings.Add(activityData.GetFullTextSearchActivitiyIdMappingData(reader));
                            }
                        }
                    }
                }
               
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetFullTextSearchActivitiyIdMapping",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(regionIds)},{SerializeDeSerializeHelper.Serialize(keywordPhrase)}, {SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return mappings;
        }

        /// <summary>
        /// This method return the list of the region id from attraction id.
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <param name="attractionId"></param>
        /// <returns></returns>
        public List<string> GetRegionIdsFromAttractionId(string affiliateId, int attractionId)
        {
            var regionIds = new List<string>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAffiliateRegionForCategorySp))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.AffiliateIdForRegion, DbType.String, affiliateId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.AttractionIdForRegion, DbType.Int64, attractionId);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var activityData = new ActivityData();
                        while (reader.Read())
                        {
                            regionIds.Add(activityData.GetRegionIdsFromAttractionIdData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetRegionIdsFromAttractionId",
                    AffiliateId = affiliateId,
                    Params = $"{attractionId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return regionIds;
        }

        /// <summary>
        /// This method return list of region and category mapping
        /// </summary>
        /// <returns></returns>

        public List<RegionCategoryMapping> LoadRegionCategoryMapping()
        {
            var categories = new List<RegionCategoryMapping>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetServiceAttractionsForSearch))
                {


                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var activityData = new ActivityData();
                        while (reader.Read())
                        {
                            categories.Add(activityData.LoadRegionCategoryMappingData(reader));
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "LoadRegionCategoryMapping",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return categories;
        }

        /// <summary>
        /// This Method returns the origin service restriction data.
        /// </summary>
        /// <returns></returns>
        public List<ActivityCountryRestriction> GetOriginServiceRestriction()
        {
            var activitiesByCountry = new List<ActivityCountryRestriction>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCountryServiceRestriction))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            var activityByCountry = new ActivityCountryRestriction();
                            activityByCountry.ProductId = int.Parse(reader["serviceid"].ToString());
                            var countries = reader["countryList"].ToString();
                            activityByCountry.Countries = countries.Split(',').ToList();
                            activityByCountry.IsShow = Boolean.Parse(reader["Flag"].ToString());
                            activitiesByCountry.Add(activityByCountry);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetOriginServiceRestriction",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activitiesByCountry;
        }

        /// <summary>
        /// This method returns region meta data based on the region ID, category Id and language code
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="catId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public RegionMetaData LoadRegionMetaData(int regionId, int catId, string languageCode)
        {
            var regionMetaData = new RegionMetaData();
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetRegionMetadata))
                {
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.RegionId, DbType.Int32, regionId);
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.AttractionId, DbType.Int32, catId);
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.LanguageCodeForLoadRegionMetaData, DbType.String,
                        languageCode);

                    dbCommand.CommandType = CommandType.StoredProcedure;

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var activityData = new ActivityData();
                        while (reader.Read())
                        {
                            regionMetaData = activityData.MapRegionMetaData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "LoadRegionMetaData",
                    Params = $"{regionId},{catId},{languageCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return regionMetaData;
        }

        /// <summary>
        /// Returns the Live Services for caching service
        /// </summary>
        /// <returns></returns>
        public Int32[] GetLiveActivityIds(string languageCode)
        {
            var serviceIds = new List<int>();
            try
            {
                using (var reader = IsangoDataBaseLive.ExecuteReader(Constant.GetLiveServicesSp, languageCode))
                {
                    var activityData = new ActivityData();

                    while (reader.Read())
                    {
                        var serviceId = activityData.GetServiceIdData(reader);
                        if (serviceId != 0)
                            serviceIds.Add(serviceId);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetLiveActivityIds",
                    Params = $"{languageCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return serviceIds.Distinct().ToArray();
        }

        /// <summary>
        /// Returns the live hotel bed activities for cache
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns>List of activities</returns>
        public List<Activity> LoadLiveHbActivities(int activityId, string languageCode)
        {
            List<Activity> hbActivities=new List<Activity>();

            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetHbActivitiesV1Sp))
                {
                    command.CommandTimeout = 0;
                    if (activityId != 0)
                    {
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamServiceId, DbType.Int32, activityId);
                    }

                    IsangoDataBaseLive.AddInParameter(command, Constant.LanguageCodeForLoadRegionMetaData, DbType.String, languageCode.ToLowerInvariant());
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var activityData = new ActivityData();
                        hbActivities = activityData.LoadLiveHbActivityData(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "LoadLiveHbActivities",
                   
                    Params = $"{activityId},{languageCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return hbActivities;
        }

        /// <summary>
        /// Method to retrieve activity list by language code and Activity ids
        /// </summary>
        /// <param name="activityIds">Comma separated ActivityIds</param>
        /// <param name="languageCode"></param>
        /// <param name="productOption"></param>
        /// <returns></returns>
        public List<Activity> GetActivitiesByActivityIds(string activityIds, string languageCode)
        {
            var activities = new List<Activity>();
            try
            {
                activities = GetActivity(activityIds, languageCode);
                
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetActivitiesByActivityIds",
                    Params = $"{activityIds},{languageCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activities;
        }

        /// <summary>
        /// Method to retrieve Calendar availability
        /// </summary>
        /// <returns></returns>
        public List<CalendarAvailability> GetCalendarAvailability(string activityIds = "")
        {
            var calendarList = new List<CalendarAvailability>();
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCalendarAvailability))
                {
                    if (activityIds != "")
                    {
                        IsangoDataBaseLive.AddInParameter(dbCmd, Constant.ServiceIds, DbType.String, activityIds);
                    }

                    dbCmd.CommandType = CommandType.StoredProcedure;

                    dbCmd.CommandTimeout = 0;
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        
                        var activityData = new ActivityData();
                        while (reader.Read())
                        {
                            calendarList.Add(activityData.GetCalendarMapping(reader));
                        }
                       
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetCalendarAvailability",
                    Params = $"{activityIds}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return calendarList;
        }

        /// <summary>
        /// whether to run calander job or not
        /// </summary>
        /// <param name="activityIds"></param>
        /// <returns></returns>
        public bool GetCalendarFlag()
        {
            var flag = false;
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCalendarFlag))
                {
                    dbCmd.CommandType = CommandType.StoredProcedure;

                    dbCmd.CommandTimeout = 0;
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {

                        var activityData = new ActivityData();
                        while (reader.Read())
                        {
                            flag = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "Isrefresh");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetCalendarFlag"
                };
                _log.Error(isangoErrorEntity, ex);
                return true;
            }
            return flag;
        }

        /// <summary>
        /// Get auto suggest data
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        public List<AutoSuggest> GetAutoSuggestData(string affiliateId)
        {
            var autoSuggestData = new List<AutoSuggest>();
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAutoCompleteText))
                {
                    IsangoDataBaseLive.AddInParameter(cmd, Constant.AffiliateId, DbType.String, affiliateId);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmd))
                    {
                        var activityData = new ActivityData();
                        while (reader.Read())
                        {
                            autoSuggestData.Add(activityData.GetAutoSuggestData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetAutoSuggestData",
                    AffiliateId = affiliateId,
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return autoSuggestData;
        }

        /// <summary>
        /// Method to Get ActivityId by ProductId
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public int GetActivityId(int productId)
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetActivityIdSp))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.ServiceIdForGetActivityId, DbType.Int32, productId);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var activityData = new ActivityData();
                        if (reader.Read())
                        {
                            return activityData.GetActivityIdData(reader);
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetActivityId",
                    Params = $"{productId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return 0;
        }

        /// <summary>
        /// Gets the price and availability for an entire week for the given date
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="clientInfo"></param>
        /// <param name="isB2BNetPriceApplied"></param>
        /// <param name="conversionFactor"></param>
        /// <returns></returns>
        public List<ProductOption> GetPriceAndAvailability(Activity activity, ClientInfo clientInfo, bool isB2BNetPriceApplied)
        {
            var travelInfo = new TravelInfo();
            try
            {
                travelInfo = activity.ProductOptions.First().TravelInfo;
                var occupancyString = CreateOccupancyString(travelInfo)?.InnerXml;

                var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(isB2BNetPriceApplied ? Constant.GetComputeActivityPricePerDayForAllB2BNetPriceSp : Constant.GetComputeActivityPricePerDayForAll);

                IsangoDataBaseLive.AddInParameter(dbCommand, Constant.ServiceId, DbType.Int32, activity.ID);
                IsangoDataBaseLive.AddInParameter(dbCommand, Constant.LicenseKey, DbType.String, GetLicenseKey(clientInfo));
                IsangoDataBaseLive.AddInParameter(dbCommand, Constant.OccupancyString, DbType.String, occupancyString);
                dbCommand.CommandType = CommandType.StoredProcedure;
                using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                {
                    var productOptions = new List<ProductOption>();
                    while (reader.Read())
                    {
                        var activityData = new ActivityData();
                        activity.ProductOptions = activityData.LoadPrice(reader, activity.ProductOptions);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetPriceAndAvailability",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{activity.ID},{SerializeDeSerializeHelper.Serialize(clientInfo)}, {SerializeDeSerializeHelper.Serialize(travelInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activity.ProductOptions;
        }

        /// <summary>
        /// Gets the availability for an activity for the given date
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param
        /// <returns></returns>
        public List<ProductOption> GetAllOptionsAvailability(Activity activity, DateTime startDate, DateTime endDate)
        {
            try
            {
                var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAlloptionAvailabilitySp);

                IsangoDataBaseLive.AddInParameter(dbCommand, Constant.ServiceId, DbType.Int32, activity.ID);
                IsangoDataBaseLive.AddInParameter(dbCommand, "@OnDate", DbType.DateTime, startDate);
                IsangoDataBaseLive.AddInParameter(dbCommand, "@EndDate", DbType.DateTime, endDate);
                dbCommand.CommandType = CommandType.StoredProcedure;
                using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                {
                    var activityData = new ActivityData();
                    activity.ProductOptions = activityData.LoadAllOptionsAvailability(reader, activity.ProductOptions);
                    return activity.ProductOptions;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetAllOptionsAvailability",
                    Params = $"{SerializeDeSerializeHelper.Serialize(activity)},{SerializeDeSerializeHelper.Serialize(startDate)}, {SerializeDeSerializeHelper.Serialize(endDate)}"
                };

                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Get Isango product Availabilities from db for multiple dates.
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<ProductOptionAvailabilty> GetAllOptionsAvailabilities(Activity activity, DateTime startDate, DateTime endDate)
        {
            try
            {
                var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAlloptionAvailabilitySp);

                IsangoDataBaseLive.AddInParameter(dbCommand, Constant.ServiceId, DbType.Int32, activity.ID);
                IsangoDataBaseLive.AddInParameter(dbCommand, "@OnDate", DbType.DateTime, startDate);
                IsangoDataBaseLive.AddInParameter(dbCommand, "@EndDate", DbType.DateTime, endDate);
                dbCommand.CommandType = CommandType.StoredProcedure;
                using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                {
                    var activityData = new ActivityData();
                    if (activity.ApiType != APIType.Redeam)
                    {
                        var OptionsAvailabilities = activityData.LoadAllOptionsAvailabilities(reader, activity.ProductOptions);
                        return OptionsAvailabilities;
                    }
                    else
                    {
                        var OptionsAvailabilities = activityData.LoadAllOptionsAvailabilitiesRedeam(reader, activity.ProductOptions);
                        return OptionsAvailabilities;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetAllOptionsAvailabilities",
                    Params = $"{SerializeDeSerializeHelper.Serialize(activity)},{SerializeDeSerializeHelper.Serialize(startDate)}, {SerializeDeSerializeHelper.Serialize(endDate)}"
                };
            
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return null;
        }

        /// <summary>
        /// return the type of the activity passed.
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public int GetActivityType(int serviceId)
        {
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.SPCheckActivityType))
                {
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.ServiceId, DbType.Int32, serviceId);
                    IsangoDataBaseLive.AddOutParameter(dbCommand, Constant.Flag, DbType.String, 300);

                    IsangoDataBaseLive.ExecuteNonQuery(dbCommand);
                    var result = Convert.ToInt32(dbCommand.Parameters[Constant.Flag].Value);
                    return result;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetActivityType",
                    Params = $"{serviceId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return 0;
        }

        /// <summary>
        /// load category and service mapping list.
        /// </summary>
        /// <returns></returns>
        public List<AttractionActivityMapping> CategoryServiceMapping()
        {
            var mappingList = new List<AttractionActivityMapping>();
            try
            {
               

                using (var reader = IsangoDataBaseLive.ExecuteReader(Constant.uspGetCategorytopservice))
                {
                    var activityData = new ActivityData();
                    while (reader.Read())
                    {
                        mappingList.Add(activityData.GetCategoryServiceMapping(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "CategoryServiceMapping"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return mappingList;
        }

        /// <summary>
        /// Load max pax details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Dictionary<string, int> LoadMaxPaxDetails(int id)
        {
            try
            {
                using (var cmdStatement = IsangoDataBaseLive.GetSqlStringCommand(Constant.LoadMaxPaxQuery))
                {
                    IsangoDataBaseLive.AddInParameter(cmdStatement, "@serviceid", DbType.Int32, id);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmdStatement))
                    {
                        var activityData = new ActivityData();
                        return activityData.LoadMaxPax(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "LoadMaxPaxDetails",
                    Params = $"{id}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Returns the Modified Services for caching service
        /// </summary>
        /// <returns></returns>
        public List<ActivityChangeTracker> GetModifiedServices()
        {
            try
            {
                using (var reader = IsangoDataBaseLive.ExecuteReader(Constant.GetIsangoModifiedServicesSp))
                {
                    var activityData = new ActivityData();
                    return activityData.GetModifiedServices(reader);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetModifiedServices"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Remove updated services from database
        /// </summary>
        /// <param name="servicedIds"></param>
        /// <returns></returns>
        public int RemoveUpdatedServices(string servicedIds)
        {
            try
            {
                return IsangoDataBaseLive.ExecuteNonQuery(Constant.UpdateIsangoModifiedServicesSp, servicedIds);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "RemoveUpdatedServices",
                    Params = $"{servicedIds}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return 0;
        }

        /// <summary>
        /// To execute the procedure which calculate and insert the option availability.
        /// </summary>
        public void InsertOptionAvailability()
        {
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.SPInsOptionAvailabilityOnlyCache))
                {
                    //This proc require 3-4 min to execute. It was throwing timeoout error. to resolve the issue, CommandTimeout is set to 0
                    cmd.CommandTimeout = 0;
                    IsangoDataBaseLive.ExecuteNonQuery(cmd);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "InsertOptionAvailability",
               };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public DataTable GetOptionAvailability(string regionIds = "", string activityIds = "")
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.SPGetOptionAvailabilityOnlyCache))
                {
                    if (regionIds != string.Empty)
                    {
                        IsangoDataBaseLive.AddInParameter(dbCmd, Constant.RegionId, DbType.String, regionIds);
                    }

                    if (activityIds != string.Empty)
                    {
                        IsangoDataBaseLive.AddInParameter(dbCmd, Constant.ServiceIds, DbType.String, activityIds);
                    }

                    dbCmd.CommandType = CommandType.StoredProcedure;

                    using (var dataSet = IsangoDataBaseLive.ExecuteDataSet(dbCmd))
                    {
                        if (dataSet.Tables.Count > 0)
                        {
                            return dataSet.Tables[0];
                        }
                        return new DataTable();
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetOptionAvailability",
                    Params = $"{regionIds},{activityIds}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return new DataTable();
        }

        public List<OptionDetail> GetPaxPrices(PaxPriceRequest paxPriceRequest)
        {
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetPaxPrices))
                {
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.AffiliateIdParam, DbType.String, paxPriceRequest.AffiliateId);
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.ServiceId, DbType.Int32, paxPriceRequest.ServiceId);
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.CheckInDate, DbType.DateTime, paxPriceRequest.CheckIn);
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.CheckOutDate, DbType.DateTime, paxPriceRequest.CheckOut);
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.PaxDetail, DbType.String, paxPriceRequest.PaxDetail);
                    dbCommand.CommandType = CommandType.StoredProcedure;

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var activityData = new ActivityData();
                        var optionDetails = activityData.MapOptionDetail(reader);
                        return optionDetails;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetPaxPrices",
                    Params = $"{SerializeDeSerializeHelper.Serialize(paxPriceRequest)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return null;
        }

        public List<WidgetMappedData> GetRegionMappedDataForWidget()
        {
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCSRegionMapping))
                {
                    dbCommand.CommandType = CommandType.StoredProcedure;

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var activityData = new ActivityData();
                        var widgetMappedDatas = activityData.MapWidgetData(reader);
                        return widgetMappedDatas;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetRegionMappedDataForWidget"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return null;
        }

        #region Private Methods

        /// <summary>
        /// This operation is used to fetch passenger information
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public List<Entities.Booking.PassengerInfo> GetPassengerInfoDetails(string activityIds = "")
        {
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetPassengerInfoSp))
                {
                    if (activityIds != string.Empty)
                        IsangoDataBaseLive.AddInParameter(dbCommand, Constant.ServiceIds, DbType.String, activityIds);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var activityData = new ActivityData();
                        return activityData.GetPassengerinfo(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetPassengerInfoDetails",
                    Params = $"{activityIds}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return null;
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
        /// <summary>
        /// This method return the license key for client.
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        private string GetLicenseKey(ClientInfo clientInfo)
        {
            var licenseKey = string.Empty;
            try
            {
               

                using (var reader = IsangoDataBaseLive.ExecuteReader(Constant.GetLicenseKeySp, clientInfo.B2BAffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183", clientInfo.Currency.IsoCode))
                {
                    var activityData = new ActivityData();

                    while (reader.Read())
                    {
                        licenseKey = activityData.GetLicenseKeyData(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetLicenseKey",
                    Params = $"{SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return licenseKey;
        }

        /// <summary>
        /// This method return activity details base on activity id and language code.
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        private Activity GetActivity(int activityId, string languageCode)
        {
            var activity = new Activity();
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetActivityDetailSp))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.ServiceId, DbType.Int32, activityId);
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.LanguageCode, DbType.String, languageCode);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var activityData = new ActivityData();
                        while (reader.Read())
                        {
                            activity = activityData.LoadActivityPropertiesData(reader, false);
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetActivity",
                    Params = $"{activityId},{languageCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activity;
        }

        /// <summary>
        /// This method return list of activity id using category id, region id.
        /// </summary>
        /// <param name="categoryIds"></param>
        /// <param name="regionIds"></param>
        /// <param name="keywordPhrase"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        private List<int> GetFullTextSearchActivitiyIDs(string categoryIds, string regionIds, string keywordPhrase, ClientInfo clientInfo)
        {
            var activityIds = new List<int>();
            try
            {
                var fulltextPhrase = MakeSearchClause(regionIds, string.Empty, categoryIds, keywordPhrase);

                if (string.IsNullOrEmpty(fulltextPhrase)) return activityIds;
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.SearchProductFullTextSp))
                {
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.LicenseKey, DbType.String, GetLicenseKey(clientInfo));
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.Keyword, DbType.String, fulltextPhrase);
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.LanguageCode, DbType.String, Constant.LanguageCodeValue);
                    dbCommand.CommandType = CommandType.StoredProcedure;

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var activityData = new ActivityData();

                        while (reader.Read())
                        {
                            activityIds.Add(activityData.GetServiceIdData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetFullTextSearchActivitiyIDs",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{categoryIds},{regionIds},{keywordPhrase}, {SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return activityIds;
        }

        /// <summary>
        /// This method making search clause
        /// </summary>
        /// <param name="regionIDs"></param>
        /// <param name="categoryTypeIDs"></param>
        /// <param name="categoryIds"></param>
        /// <param name="phrase"></param>
        /// <returns></returns>
        private string MakeSearchClause(string regionIDs, string categoryTypeIDs, string categoryIds, string phrase)
        {
            var searchClause = "";
            var keywordClause = "";
            try
            {
                if (!string.IsNullOrEmpty(regionIDs) && !regionIDs.Equals("0"))
                    searchClause = NonKeywordsPhrase(Constant.Region, regionIDs);

                if (!string.IsNullOrEmpty(categoryIds) && !categoryIds.Equals("0"))
                    keywordClause = MakeCategoryClause(categoryIds, categoryTypeIDs);

                searchClause = $"{searchClause}{(keywordClause == string.Empty || searchClause == string.Empty ? "" : " AND ")}{keywordClause}";
                keywordClause = RemoveNoiseWordsAndCreateSearchClause(phrase);
                searchClause = $"{searchClause}{(keywordClause == string.Empty || searchClause == string.Empty ? "" : " AND ")}{keywordClause}";
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "MakeSearchClause",
                    Params = $"{regionIDs},{categoryTypeIDs}, {categoryIds},{phrase}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return searchClause;
        }

        /// <summary>
        /// This method creating non keyword phrase
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string NonKeywordsPhrase(string field, string value)
        {
            try
            {
                if (value == string.Empty)
                    return string.Empty;
                var entries = value.Replace(" ", "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var result = entries.Aggregate("( ", (current, var) => $"{current}{(current == "( " ? "" : " OR ")}\"{field}{var}\"");
                return $"{result} ) ";
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "NonKeywordsPhrase",
                    Params = $"{field},{value}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return string.Empty;
        }

        /// <summary>
        /// This method making category clause
        /// </summary>
        /// <param name="categoryIds"></param>
        /// <param name="categoryTypeIds"></param>
        /// <returns></returns>
        private string MakeCategoryClause(string categoryIds, string categoryTypeIds)
        {
            try
            {
                var keywordClause = NonKeywordsPhrase(Constant.RatingType, categoryIds);
                var result = NonKeywordsPhrase(Constant.Rating, categoryTypeIds);

                result = result != string.Empty && keywordClause != string.Empty ? $"( {result} AND {keywordClause} )" : $"{result}{keywordClause}";
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "MakeCategoryClause",
                    Params = $"{categoryIds},{categoryTypeIds}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return string.Empty;
        }

        /// <summary>
        /// This method is used to remove noise words and to create search clause
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        private string RemoveNoiseWordsAndCreateSearchClause(string phrase)
        {
            try
            {
                if (string.IsNullOrEmpty(phrase)) { return string.Empty; }
                var keywords = phrase.Replace("\"", " ").Replace(",", " ").Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var result = keywords.Where(keyword => Constant.NoiseData.IndexOf($",{keyword},", StringComparison.CurrentCulture) < 0).Aggregate("(", (current, keyword) => $"{current}{(current == "(" ? "" : " AND ")}FORMSOF(INFLECTIONAL,\"{keyword.ToUpper()}\")");
                return $"{result})".Replace("()", "");
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "RemoveNoiseWordsAndCreateSearchClause",
                    Params = $"{phrase}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return string.Empty;
        }

        /// <summary>
        /// This method generates XML required for compute Price for a week with the given date kept in the middle.
        /// </summary>
        /// <param name="travelInfo"></param>
        /// <returns></returns>
        private XmlDocument CreateOccupancyString(TravelInfo travelInfo)
        {
            var xmlDoc = new XmlDocument();
            try
            {
                if (travelInfo == null)
                    return null;

                
                var childXml = new List<string>();

                var xmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "DATA", String.Empty);
                xmlDoc.AppendChild(xmlNode);

                var xmldecl = xmlDoc.CreateXmlDeclaration("1.0", null, null);
                xmldecl.Encoding = "UTF-8";
                xmldecl.Standalone = "yes";

                var root = xmlDoc.FirstChild;
                xmlDoc.InsertBefore(xmldecl, root);

                var xmlElement = xmlDoc.CreateElement("ROOM");
                xmlElement.SetAttribute("RID", "1");
                xmlElement.SetAttribute("Adult", travelInfo.NoOfPassengers.Where(x => x.Key == PassengerType.Adult).Select(s => s.Value).ToList().FirstOrDefault().ToString());
                xmlElement.SetAttribute("Qty", "1");
                xmlElement.SetAttribute("FDate", travelInfo.StartDate.AddDays(-3).ToString("dd-MMM-yy"));
                xmlElement.SetAttribute("TDate", travelInfo.StartDate.AddDays(3).ToString("dd-MMM-yy"));

                var ages = travelInfo.Ages?.OrderBy(age => age.Value).ToList();

                if ((travelInfo.NoOfPassengers.Where(i => i.Key == PassengerType.Adult).Select(i => i.Value)).Any() && ages?.Count > 0)
                {
                    var index = 1;
                    var rcounter = 1;
                    var qty = 1;

                    var firstChild = ages[0].Value;
                    var strChilXml = "<Child RID=\"1\" cAge=\"" + ages[0].Value + "\" cQty=\"1\" ></Child>";
                    childXml.Add(strChilXml);

                    for (var childCount = 1; childCount < ages.Count; childCount++)
                    {
                        if ((ages[childCount].Value != firstChild))
                        {
                            strChilXml = "<Child RID=\"1\" cAge=\"" + ages[childCount].Value + "\" cQty=\"1\" ></Child>";
                            childXml.Add(strChilXml);
                            firstChild = ages[childCount].Value;
                            index++;
                            rcounter++;
                            qty = 1;
                        }
                        else
                        {
                            childXml[rcounter - 1] = "<Child RID=\"1\" cAge=\"" + ages[childCount].Value + "\"  cQty=\"" + (++qty) + "\" ></Child>";
                        }
                    }

                    var strChildXml = new StringBuilder();
                    foreach (string str in childXml)
                    {
                        strChildXml.Append(str);
                    }
                    xmlElement.InnerXml = strChildXml.ToString();
                }

                xmlNode.AppendChild(xmlElement);
                xmlDoc.AppendChild(xmlNode);
               
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "CreateOccupancyString",
                    Params = $"{SerializeDeSerializeHelper.Serialize(travelInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return xmlDoc;
        }

        /// <summary>
        /// This method return activity details from activity id
        /// </summary>
        /// <param name="activityIds"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        private List<Activity> GetActivity(string activityIds, string languageCode)
        {
            var result = new List<Activity>();
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetActivityByMultipleIds))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.ServiceIds, DbType.String, activityIds);
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.LanguageCode, DbType.String, languageCode);
                    dbCmd.CommandType = CommandType.StoredProcedure;
                    dbCmd.CommandTimeout = 0;
                    using (var dataSet = IsangoDataBaseLive.ExecuteDataSet(dbCmd))
                    {
                       
                        if (dataSet.Tables.Count > 0)
                        {
                            var activityData = new ActivityData();
                            result = activityData.MapActivityFromDataSet(dataSet);
                        }
                       
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistence",
                    MethodName = "GetActivity",
                    Params = $"{activityIds},{languageCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return result;
        }

        public void InsertApiErrorLog(int activityId, string message, string MethodName)
        {
            try
            {
                using (var cmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertApiErrorLog))
                {
                    IsangoDataBaseLive.AddInParameter(cmd, "@ActivityId", DbType.Int32, activityId);
                    IsangoDataBaseLive.AddInParameter(cmd, "@Message", DbType.String, message);
                    IsangoDataBaseLive.AddInParameter(cmd, "@MethodName", DbType.String, MethodName);

                    cmd.CommandTimeout = 0;

                    IsangoDataBaseLive.ExecuteNonQuery(cmd);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityPersistance",
                    MethodName = "InsertApiErrorLog",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #endregion Private Methods
    }
}