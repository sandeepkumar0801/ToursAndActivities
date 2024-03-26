using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Region;
using Isango.Entities.Wrapper;
using Logger.Contract;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;

namespace CacheManager
{
    public class ActivityCacheManager : IActivityCacheManager
    {
        private readonly int _maxNoOfProducts = 0;
        
        private readonly ILogger _log;
        private readonly CollectionDataFactory<RegionCategoryMapping> _collectionDataRegionCategoryMapping;
        private readonly CollectionDataFactory<ActivityWithApiType> _collectionDataActivityIdApiType;
        private readonly CollectionDataFactory<CacheKey<RegionCategoryMapping>> _collectionDataRegionMapping;
        private static string cloudPath = ConfigurationManagerHelper.GetValuefromAppSettings("CloudinaryUrl");
        private readonly CollectionDataFactory<Activity> _collectionDataFactory;
        public ActivityCacheManager(
              CollectionDataFactory<RegionCategoryMapping> collectionDataRegionCategoryMapping
            , CollectionDataFactory<ActivityWithApiType> collectionDataActivityIdApiType
            , CollectionDataFactory<CacheKey<RegionCategoryMapping>> collectionDataRegionMapping
            , ILogger log, CollectionDataFactory<Activity> collectionDataFactory
            )
        {

            _collectionDataRegionCategoryMapping = collectionDataRegionCategoryMapping;
            _collectionDataActivityIdApiType = collectionDataActivityIdApiType;
            _collectionDataRegionMapping = collectionDataRegionMapping;
            _log = log;
            _collectionDataFactory = collectionDataFactory;
        }

        public SearchResult SearchActivities(SearchCriteria searchCriteria, ClientInfo clientInfo)
        {
            var maxProducts = searchCriteria.MaxNoOfProducts == 0 ? _maxNoOfProducts : searchCriteria.MaxNoOfProducts;
            SearchResult searchResult;

            if (searchCriteria.CategoryId.Equals(0) && string.IsNullOrEmpty(searchCriteria.Keyword) && (!string.IsNullOrEmpty(searchCriteria.ProductIDs)))
            {
                var activityIDs = searchCriteria.ProductIDs.Trim().Split(',').Select(int.Parse).ToList();
                searchResult = GetSearchResultByProductIds(activityIDs.ToArray(), maxProducts, clientInfo.LanguageCode.ToLowerInvariant());
            }
            else
            {
                searchResult = GetSearchResultByParamters(searchCriteria, clientInfo.LanguageCode.ToLowerInvariant());
            }

            if (searchResult.Activities != null && searchResult.Activities.Count > 0)
            {
                searchResult.Activities = GetFilteredActivities(searchCriteria, searchResult);
            }
            return searchResult;
        }

        public Activity GetActivity(string activityId, string languageCode = Constant.LanguageCodeEnglish)
        {
            var activity = default(Activity);
            try
            {
                var key = $"Activity_{activityId}_{languageCode}";

                if (!CacheHelper.Exists(key) || !CacheHelper.Get<Activity>(key, out activity))
                {
                    var configKeyActivityCollection = ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection");

                    var collectionName = languageCode.ToLowerInvariant() != Constant.LanguageCodeEnglish ? $"{configKeyActivityCollection}_{languageCode.ToLowerInvariant()}" : configKeyActivityCollection;
                    //for mongoDB
                    var filter = Builders<Activity>.Filter.Eq("_id", activityId);
                    //end mongo
                    activity = _collectionDataFactory.GetCollectionDataHelper().GetResult(collectionName, $"{Constant.GetSearchResultByProductIdQuery}{activityId}", filter);

                    if (activity != null)
                    {
                        try
                        {
                            activity?.Images?.RemoveAll(x => x.ImageType != ImageType.CloudProduct);
                            activity?.Images?.ForEach(x => x.Name = cloudPath + x.Name);
                        }
                        catch (Exception ex)
                        {
                            Task.Run(() =>
                                 _log.Error(new IsangoErrorEntity
                                 {
                                     ClassName = "ActivityCacheManager",
                                     MethodName = "GetActivity",
                                     Params = $"ActivityId - {activityId}\n ,languageCode-{languageCode}"
                                 }, ex)
                                 );
                        }

                        if (activity?.ProductOptions?.Any() == true)
                        {
                            CacheHelper.Set<Activity>(key, activity);
                        }
                    }
                    else
                    {
                        var message = Constant.ActivityNotInCosmos +" id:"+ activityId;
                        SaveException(Int32.Parse(activityId), message, "GetActivity", "ActivityCacheManager");
                    }
                }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                        _log.Error(new IsangoErrorEntity
                        {
                            ClassName = "ActivityCacheManager",
                            MethodName = "GetActivity",
                            Params = $"ActivityId - {activityId}\n ,languageCode-{languageCode}"
                        }, ex)
                        );
            }
            return activity;
        }

        public bool DeleteAndCreateCollection(string languageCode = Constant.LanguageCodeEnglish)
        {
            var collectionName = languageCode.ToLowerInvariant() != Constant.LanguageCodeEnglish ? $"{ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection")}_{languageCode.ToLowerInvariant()}" : ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection");
            if (_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(collectionName).Result)
            {
                _collectionDataFactory.GetCollectionDataHelper().DeleteCollection(collectionName).Wait();
            }

            return _collectionDataFactory.GetCollectionDataHelper().CreateCollection(collectionName, Constant.PartitionKeyActivityCollection).Result;
        }

        public List<RegionCategoryMapping> GetRegioncategoryMapping()
        {
            var query = $"select VALUE M from M";
            //for mongoDB
            var filter = Builders<RegionCategoryMapping>.Filter.Empty;
            //end mongo
            var result = _collectionDataRegionCategoryMapping.GetCollectionDataHelper().GetResultList(ConfigurationManagerHelper.GetValuefromAppSettings("RegionCategoryMappingCollection"), query, filter);
            return result;
        }

        public CacheKey<RegionCategoryMapping> GetRegioncategoryMappingV2(string key)
        {
            var collection = ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection");
            var query = $"select VALUE M from {collection} M where M.id='{key}'";
            //for mongoDB
            var filter = Builders<CacheKey<RegionCategoryMapping>>.Filter.Eq("_id", key);
            //end mongo
            return _collectionDataRegionMapping.GetCollectionDataHelper().GetResult(collection, query, filter);
        }

        public bool InsertActivity(Activity activity, string languageCode = Constant.LanguageCodeEnglish)
        {
            var collectionName = languageCode.ToLowerInvariant() != Constant.LanguageCodeEnglish ? $"{ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection")}_{languageCode.ToLowerInvariant()}" : ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection");
            return _collectionDataFactory.GetCollectionDataHelper().InsertDocument(collectionName, activity).Result;
        }
        public bool InsertManyActivity(List<Activity> activity, string languageCode = Constant.LanguageCodeEnglish)
        {
            var collectionName = languageCode.ToLowerInvariant() != Constant.LanguageCodeEnglish ? $"{ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection")}_{languageCode.ToLowerInvariant()}" : ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection");
            return _collectionDataFactory.GetCollectionDataHelper().InsertManyDocuments(collectionName, activity).Result;
        }
        public bool UpdateActivity(Activity activity, string languageCode = Constant.LanguageCodeEnglish)
        {
            var collectionName = languageCode.ToLowerInvariant() != Constant.LanguageCodeEnglish ? $"{ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection")}_{languageCode.ToLowerInvariant()}" : ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection");
            return _collectionDataFactory.GetCollectionDataHelper().UpdateDocument(collectionName, activity).Result;
        }

        public List<Activity> GetActivityWithProductOption(string languageCode = Constant.LanguageCodeEnglish)
        {
            var collectionName = languageCode.ToLowerInvariant() != Constant.LanguageCodeEnglish ? $"{ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection")}_{languageCode.ToLowerInvariant()}" : ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection");
            var query = "SELECT * FROM c where c.ProductOptions != null";
            //for mongoDB
            var filter = Builders<Activity>.Filter.Ne("ProductOptions", BsonNull.Value);
            //end mongo
            var result = _collectionDataFactory.GetCollectionDataHelper().GetResultList(collectionName, query, filter);
            return result;
        }

        public List<Activity> GetAllActivities(string languageCode = Constant.LanguageCodeEnglish)
        {
            var collectionName = languageCode.ToLowerInvariant() != Constant.LanguageCodeEnglish ? $"{ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection")}_{languageCode.ToLowerInvariant()}" : ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection");
            var query = "SELECT * FROM c";
            //for mongoDB
            var filter = Builders<Activity>.Filter.Empty;
            //end mongo
            var result = _collectionDataFactory.GetCollectionDataHelper().GetResultList(collectionName, query, filter);
            return result;
        }

        public List<Activity> GetAllActivitiesWithLineOfBusiness(int? lineOfBusinessId = 1, string languageCode = Constant.LanguageCodeEnglish, string regionid = "7243")
        {
            var collectionName = languageCode.ToLowerInvariant() != Constant.LanguageCodeEnglish ? $"{ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection")}_{languageCode.ToLowerInvariant()}" : ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection");
            var query = $"SELECT VALUE c FROM c Join a in c.Regions where a.Id = {regionid} and c.LineOfBusinessId = {lineOfBusinessId}";
            var filter = Builders<Activity>.Filter.And(
                                               Builders<Activity>.Filter.ElemMatch(reg => reg.Regions, region => region.Id == Convert.ToInt32(regionid)),
                                               Builders<Activity>.Filter.Eq("LineOfBusinessId", Convert.ToInt32(lineOfBusinessId))
                                                        );
            var result = _collectionDataFactory.GetCollectionDataHelper().GetResultList(collectionName, query, filter);
            return result;
        }

        public List<ActivityWithApiType> GetActivityWithApiType(string apiTypeId = "", string languageCode = Constant.LanguageCodeEnglish)
        {
            var collectionName = languageCode.ToLowerInvariant() != Constant.LanguageCodeEnglish ? $"{ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection")}_{languageCode.ToLowerInvariant()}" : ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection");
            var query = "SELECT c.id, c.ApiType FROM c";
            //for mongoDB
            var filter = Builders<ActivityWithApiType>.Filter.Empty;
            //end mongo
            if (apiTypeId != string.Empty)
            {
                query = query + " WHERE c.ApiType = " + apiTypeId;
                filter = Builders<ActivityWithApiType>.Filter.Eq("ApiType", apiTypeId);
            }
            var result = _collectionDataActivityIdApiType.GetCollectionDataHelper().GetResultList(collectionName, query, filter);
            return result;
        }

        #region Private Methods

        private List<Activity> GetFilteredActivities(SearchCriteria searchCriteria, SearchResult searchResult)
        {
            var activities = searchResult.Activities;

            //HOTELBEDS & GRAYLINEICELAND product filter
            if (!string.IsNullOrWhiteSpace(searchCriteria.CategoryIDs))
            {
                //Filter HB products
                var hbActivities = activities.Where(x => x.ApiType.Equals(APIType.Hotelbeds)).ToList();
                if (hbActivities.Count > 0)
                {
                    activities = FilterProductsByApiType(hbActivities, searchCriteria.CategoryIDs, searchResult.Activities);
                }

                //Filter GLI products
                var gliActivities = activities.Where(x => x.ApiType.Equals(APIType.Graylineiceland)).ToList();
                if (gliActivities.Count > 0)
                {
                    activities = FilterProductsByApiType(gliActivities, searchCriteria.CategoryIDs, searchResult.Activities);
                }
            }

            //Region & Attraction Filter
            if (!string.IsNullOrEmpty(searchCriteria.RegionFilterIds))
            {
                var regionIds = searchCriteria.RegionFilterIds.Split(':');

                activities = (activities
                    .Select(i => i.Regions.Where(j => j.Type.ToString().ToLowerInvariant() == Constant.RegionTypeFilter && regionIds.Contains(j.Id.ToString()))
                        .GroupBy(p => p.Id)
                        .Select(g => g.First()).ToList())) as List<Activity>;
            }
            else if (!string.IsNullOrEmpty(searchCriteria.AttractionFilterIds))
            {
                var attractionIds = searchCriteria.AttractionFilterIds.Split(':');

                activities = activities
                    .Where(i => i.CategoryIDs.Select(j => j.ToString()).Intersect(attractionIds).Any()).AsEnumerable()
                    .GroupBy(p => p.ID).Select(g => g.First()).ToList();
            }

            //Category Filter
            if (!string.IsNullOrWhiteSpace(searchCriteria.CategoryIDs))
            {
                var categoryIDs = searchCriteria.CategoryIDs.Split(',').ToList();
                activities = activities?.Where(i => i.CategoryIDs.Select(j => j.ToString())
                                .Intersect(categoryIDs).Any()).ToList();
            }

            //SmartPhone Filter
            if (searchCriteria.IsSmartphoneFilter)
            {
                activities = activities?.FindAll(f => f.Badges != null).Where(i => i.Badges.Select(j => j.Id).Equals(Constant.BadgeFilterId)).ToList();
            }
            return activities;
        }

        private List<Activity> FilterProductsByApiType(List<Activity> activityLites, string categoryId, List<Activity> activities)
        {
            var activityHavingDiffCatId = activityLites.FindAll(act => act.CategoryIDs.Any(cid => cid.Equals(categoryId)));
            var activitiesToBeRemoved = activityLites.Except(activityHavingDiffCatId);
            foreach (var activity in activitiesToBeRemoved)
            {
                activities.Remove(activity);
            }
            return activities;
        }

        private SearchResult GetSearchResultByParamters(SearchCriteria criteria, string languageCode)
        {
            var activitySearchResult = new SearchResult { Activities = new List<Activity>() };
            string query;
            //for mongoDB
            var filter = Builders<Activity>.Filter.And(
                                               Builders<Activity>.Filter.ElemMatch(reg => reg.Regions, region => region.Id == criteria.RegionId),
                                               Builders<Activity>.Filter.Ne("CategoryIDs", BsonNull.Value)
                                                        );

            //end mongo
            var collectionName = languageCode.ToLowerInvariant() != Constant.LanguageCodeEnglish ? $"{ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection")}_{languageCode.ToLowerInvariant()}" : ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection");
            //This code will be refactor and generic logic will be implemented
            if (!string.IsNullOrEmpty(criteria.RegionId.ToString()) && criteria.RegionId == 0 && criteria.CategoryId != 0)
            {
                query = $"SELECT VALUE c FROM c Join a in c.CategoryIDs where  a={criteria.CategoryId} and c.CategoryIDs != null";
                //for mongoDB
                filter = Builders<Activity>.Filter.And(
                                                   Builders<Activity>.Filter.Eq("CategoryIDs", criteria.CategoryId),
                                                   Builders<Activity>.Filter.Ne("CategoryIDs", BsonNull.Value)
                                                            );

                //end mongo
            }
            else if (!string.IsNullOrEmpty(criteria.RegionId.ToString()) && criteria.CategoryId != 0)
            {
                query = $"SELECT VALUE c FROM c Join a in c.Regions join b in c.CategoryIDs where a.Id = {criteria.RegionId} and b={criteria.CategoryId} and c.CategoryIDs != null";

                //for mongoDB
                filter = Builders<Activity>.Filter.And(
                                                   Builders<Activity>.Filter.Eq("Regions._id", criteria.RegionId),
                                                   Builders<Activity>.Filter.Eq("CategoryIDs", criteria.CategoryId),
                                                   Builders<Activity>.Filter.Ne("CategoryIDs", BsonNull.Value)
                                                            );

                //end mongo
            }
            else if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = $"SELECT VALUE c FROM c Join a in c.Regions where CONTAINS(LOWER(a.Name) ,'{criteria.Keyword.ToLower()}') and c.CategoryIDs != null";

                filter = Builders<Activity>.Filter.Or(
                                                    Builders<Activity>.Filter.Where(p => p.Name.ToLower().Contains(criteria.Keyword.ToLower())),
                                                    Builders<Activity>.Filter.ElemMatch(reg => reg.Regions, region => region.Name.ToLower().Contains(criteria.Keyword.ToLower()))
                                                             );
            }
            else
            {
                query = $"SELECT VALUE c FROM c Join a in c.Regions where a.Id = {criteria.RegionId} and c.CategoryIDs != null";
            }

            var activityList = _collectionDataFactory.GetCollectionDataHelper().GetResultList(collectionName, query, filter);
            foreach (var activity in activityList)
            {
                try
                {
                    activity?.Images?.RemoveAll(x => x.ImageType != ImageType.CloudProduct);
                    activity?.Images?.ForEach(x => x.Name = cloudPath + x.Name);
                }
                catch (Exception ex)
                {
                    Task.Run(() =>
                         _log.Error(new IsangoErrorEntity
                         {
                             ClassName = "ActivityCacheManager",
                             MethodName = "GetSearchResultByProductIds",
                             Params = $"SearchCriteria - {criteria}\n ,languageCode-{languageCode}"
                         }, ex)
                         );
                }
            }
            activitySearchResult.Activities.AddRange(activityList);
            return activitySearchResult;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="activityIds"></param>
        /// <param name="maxActivities"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        private SearchResult GetSearchResultByProductIds(int[] activityIds, int maxActivities, string languageCode)
        {
            var activitySearchResult = new SearchResult
            {
                Products = new List<Product>(),
                Activities = new List<Activity>()
            };
            var counter = 0;
            var collectionName = languageCode.ToLowerInvariant() != Constant.LanguageCodeEnglish ? $"{ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection")}_{languageCode.ToLowerInvariant()}" : ConfigurationManagerHelper.GetValuefromAppSettings("ActivityCollection");
            foreach (var activityId in activityIds)
            {
                try
                {
                    var query = $"SELECT TOP 1 * FROM Activity A Where A.id = '{activityId}' and A.CategoryIDs != null";
                    //for mongoDB
                    var filter = Builders<Activity>.Filter.And(
                                                                Builders<Activity>.Filter.Eq("_id", activityId.ToString()),
                                                                Builders<Activity>.Filter.Ne("CategoryIDs", BsonNull.Value)
                                                               );

                    //end mongo
                    var activity = _collectionDataFactory.GetCollectionDataHelper().GetResult(collectionName, query, filter);
                    if (activity != null)
                    {
                        activity?.Images?.RemoveAll(x => x.ImageType != ImageType.CloudProduct);
                        activity?.Images?.ForEach(x => x.Name = cloudPath + x.Name);
                        activitySearchResult.Products.Add(activity);
                        activitySearchResult.Activities.Add(activity);
                    }
                }
                catch (Exception ex)
                {
                    Task.Run(() =>
                         _log.Error(new IsangoErrorEntity
                         {
                             ClassName = "ActivityCacheManager",
                             MethodName = "GetSearchResultByProductIds",
                             Params = $"activityIds - {activityIds}\n,maxActivities-{maxActivities}\n,languageCode-{languageCode}"
                         }, ex)
                         );
                }
                if (++counter >= maxActivities && maxActivities != 0)
                    break;
            }

            return activitySearchResult;
        }

        private IsangoErrorEntity SaveException(Int32 activityId, string message, string MethodName, string className, Exception ex = null)
        {
            var isangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = className,
                MethodName = MethodName,
                Params = $"{activityId}"
            };
            if (!string.IsNullOrEmpty(message))
            {
                _log.Error(isangoErrorEntity, new Exception(message));
            }
            else
            {
                _log.Error(isangoErrorEntity, ex);
            }
            return isangoErrorEntity;
        }

        #endregion Private Methods
    }
}