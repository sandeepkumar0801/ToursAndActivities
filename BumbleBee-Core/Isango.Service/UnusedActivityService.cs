using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Entities.HotelBeds;
using Isango.Entities.Wrapper;
using Isango.Persistence.Contract;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.HB;
using Util;

namespace Isango.Service
{
    public class UnusedActivityService : IUnusedActivityService
    {
        private readonly IActivityPersistence _activityPersistence;
        private readonly ILogger _log;
        private readonly IMasterCacheManager _masterCacheManager;
        private readonly IMasterService _masterService;
        private readonly IAttractionActivityMappingCacheManager _attractionActivityMappingCacheManager;
        private readonly IAffiliateService _affiliateService;
        private readonly INetPriceCacheManager _netPriceCacheManager;
        private readonly IHBAdapter _hBAdapter;
        private readonly ISearchService _searchService;

        public UnusedActivityService(
            IActivityPersistence activityPersistence
            , ILogger log
            , IMasterCacheManager masterCacheManager
            , IMasterService masterService
            , IAttractionActivityMappingCacheManager attractionActivityMappingCacheManager
            , IAffiliateService affiliateService
            , INetPriceCacheManager netPriceCacheManager
            , IHBAdapter hBAdapter
            , ISearchService searchService)
        {
            _activityPersistence = activityPersistence;
            _log = log;
            _masterCacheManager = masterCacheManager;
            _masterService = masterService;
            _attractionActivityMappingCacheManager = attractionActivityMappingCacheManager;
            _affiliateService = affiliateService;
            _netPriceCacheManager = netPriceCacheManager;
            _hBAdapter = hBAdapter;
            _searchService = searchService;
        }

        /// <summary>
        /// Method will fetch the activity data through activity Ids
        /// </summary>
        /// <param name="activityIds"></param>
        /// <param name="clientInfo"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public async Task<List<Activity>> GetActivitiesAsync(string activityIds, ClientInfo clientInfo, DateTime startDate)
        {
            try
            {
                var result = _activityPersistence.GetActivitiesByActivityIds(activityIds, clientInfo.LanguageCode);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetActivitiesAsync",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{clientInfo.ApiToken}|{SerializeDeSerializeHelper.Serialize(clientInfo)},{activityIds}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get live services
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public async Task<int[]> GetLiveActivityIdsAsync(string languageCode)
        {
            try
            {
                var result = _activityPersistence.GetLiveActivityIds(languageCode);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetLiveActivityIds",
                    Params = $"{languageCode}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method will load live HB activities
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public async Task<List<Activity>> LoadLiveHbActivitiesAsync(int activityId, ClientInfo clientInfo)
        {
            try
            {
                var result = _activityPersistence.LoadLiveHbActivities(activityId, clientInfo.LanguageCode);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "LoadLiveHbActivities",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get AutoSuggest Data by affiliateId
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        public async Task<List<AutoSuggest>> GetAutoSuggestByAffiliateIdAsync(string affiliateId)
        {
            try
            {
                var autoSuggestList = _masterCacheManager.GetAutoSuggestByAffiliateId(affiliateId) ?? _activityPersistence.GetAutoSuggestData(affiliateId);
                return await Task.FromResult(autoSuggestList);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetAutoSuggestByAffiliateId",
                    AffiliateId = affiliateId,
                    Params = $"{affiliateId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get ActivityId by ProductId
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<int> GetActivityIdAsync(int productId)
        {
            try
            {
                var result = _activityPersistence.GetActivityId(productId);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetActivityId",
                    Params = $"{productId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        /// <summary>
        /// Get auto suggest data
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public async Task<List<AutoSuggest>> GetAutoSuggestDataAsync(ClientInfo clientInfo)
        {
            try
            {
                var autoSuggestData = new List<AutoSuggest>();
                var autoSuggestDataWithCacheKey = new CacheKey<AutoSuggest>();
                if (!string.IsNullOrEmpty(clientInfo.AffiliateId))
                {
                    autoSuggestData = _activityPersistence.GetAutoSuggestData(clientInfo.AffiliateId);

                    autoSuggestDataWithCacheKey.Id = $"{Constant.AutoSuggestData}-{clientInfo.LanguageCode}";
                    autoSuggestDataWithCacheKey.CacheValue = autoSuggestData;
                }
                if (autoSuggestData?.Count == 0)
                {
                    if (clientInfo.LanguageCode.ToLowerInvariant().Equals(Constant.En))
                    {
                        autoSuggestDataWithCacheKey = GetAutoSuggestDataForLanguage(Constant.AffiliateId_En.ToLowerInvariant(), Constant.En);
                    }
                    else if (clientInfo.LanguageCode.ToLowerInvariant().Equals(Constant.De))
                    {
                        autoSuggestDataWithCacheKey = GetAutoSuggestDataForLanguage(Constant.AffiliateId_De.ToLowerInvariant(), Constant.De);
                    }
                    else if (clientInfo.LanguageCode.ToLowerInvariant().Equals(Constant.Es))
                    {
                        autoSuggestDataWithCacheKey = GetAutoSuggestDataForLanguage(Constant.AffiliateId_Es.ToLowerInvariant(), Constant.Es);
                    }
                    autoSuggestData = autoSuggestDataWithCacheKey.CacheValue;
                }

                _masterCacheManager.SetAutoSuggestData(autoSuggestDataWithCacheKey);
                return await Task.FromResult(autoSuggestData);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetAutoSuggestDataAsync",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<List<ProductOption>> GetPriceAndAvailabilityAsync(Activity activity, ClientInfo clientInfo)
        {
            try
            {
                var isB2BNetPriceApplied = false;
                var netPriceCache = await _masterService.LoadNetPriceMasterDataAsync();
                if (clientInfo.IsB2BNetPriceAffiliate && netPriceCache != null)
                {
                    var b2BNetPriceProduct = netPriceCache.Find(entry => entry.AffiliateId.ToLowerInvariant().Equals(clientInfo.AffiliateId.ToLowerInvariant()) && entry.ProductId.Equals(activity.ID));
                    if (b2BNetPriceProduct != null)
                    {
                        isB2BNetPriceApplied = true;
                    }
                }

                var productOptionList = _activityPersistence.GetPriceAndAvailability(activity, clientInfo, isB2BNetPriceApplied);

                return await Task.FromResult(productOptionList);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetPriceAndAvailability",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(activity)},{SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get activity data in async.
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public async Task<SearchStack> GetOfferDataAsync(SearchCriteria searchCriteria, ClientInfo clientInfo)
        {
            try
            {
                var stack = new SearchStack();

                if (searchCriteria.RegionId.Equals(0) && searchCriteria.CategoryId > 0)
                {
                    var mapping = _attractionActivityMappingCacheManager.GetAttractionActivityList(searchCriteria.CategoryId);
                    if (mapping != null)
                    {
                        searchCriteria.ProductIDs = mapping[searchCriteria.CategoryId].ToString();
                        searchCriteria.CategoryId = 0;
                    }
                }
                else
                {
                    _attractionActivityMappingCacheManager.InsertDocuments(_activityPersistence.CategoryServiceMapping());
                }

                var result = _activityPersistence.SearchActivities(searchCriteria, clientInfo);

                if (result?.Activities != null && result.Activities.Count > 0)
                {
                    var affiliateFilter = await _affiliateService.GetAffiliateFilterByIdAsync(clientInfo.AffiliateId.ToLowerInvariant());
                    var finalActivitiesToPage = FilterTickets(result.Activities, clientInfo);
                    var activitiesAllowed = new List<Activity>();

                    var isB2Bnetpriceaffiliate = clientInfo.IsB2BNetPriceAffiliate;
                    if (isB2Bnetpriceaffiliate)
                        _netPriceCacheManager.GetNetPriceMasterData();

                    if (finalActivitiesToPage != null && finalActivitiesToPage.Count > 0)
                    {
                        foreach (var activity in finalActivitiesToPage)
                        {
                            if (MatchAffiliateCriteria(affiliateFilter, activity))
                            {
                                activitiesAllowed.Add(activity);
                            }
                        }
                    }

                    stack.IsShowMoreVisible = activitiesAllowed.Count > (searchCriteria.PageSize * searchCriteria.PageNumber);
                    result.TotalActivities = activitiesAllowed.Count;

                    //Paging
                    searchCriteria.PageNumber = searchCriteria.PageNumber <= 0 ? 1 : searchCriteria.PageNumber;
                    activitiesAllowed = activitiesAllowed.Skip(searchCriteria.PageSize * (searchCriteria.PageNumber)).Take(searchCriteria.PageSize).ToList();

                    stack.Activities = activitiesAllowed;
                    stack.CategoryIds = result.CategoryIds;
                    stack.TotalActivities = result.TotalActivities;

                    return stack;
                }
                return null;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetOfferDataAsync",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(searchCriteria)}|{SerializeDeSerializeHelper.Serialize(clientInfo)}|,{SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// get type of service id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public async Task<int> GetActivityTypeAsync(int serviceId)
        {
            try
            {
                var result = _activityPersistence.GetActivityType(serviceId);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetActivityTypeAsync",
                    Params = $"{serviceId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// get max pax details of activity
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, int>> LoadMaxPaxDetailsAsync(int activityId)
        {
            try
            {
                var result = _activityPersistence.LoadMaxPaxDetails(activityId);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _log.Error($"ActivityService|LoadMaxPaxDetailsAsync|{activityId}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get activity details from HB adapter.
        /// </summary>
        /// <param name="activityRq"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Activity> GetActivityDetailAsync(HotelbedCriteriaApitude hotelbedCriteriaApitude, string token)
        {
            try
            {
                var result = await _hBAdapter.ActivityDetailsAsync(hotelbedCriteriaApitude, token) as Activity;
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "LoadMaxPaxDetailsAsync",
                    Params = $"{hotelbedCriteriaApitude?.IsangoActivityId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        /// <summary>
        /// Get product similar to the searched activity if it's available
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public async Task<List<Activity>> GetSimilarProductsAsync(SearchCriteria criteria, ClientInfo clientInfo)
        {
            try
            {
                var searchStackWithActivityData =_searchService.GetSearchStackWithActivityData(criteria, clientInfo);
                if (searchStackWithActivityData == null) return await Task.FromResult(new List<Activity>());
                if (searchStackWithActivityData.Activities?.Count == 0)
                    return await Task.FromResult(new List<Activity>());

                searchStackWithActivityData.Activities?.RemoveAll(product => product.CategoryIDs == null);
                var activities = searchStackWithActivityData.Activities;
                activities?.Sort((x, y) => (y).Priority.CompareTo((x).Priority));
                activities = ActivitiesPostCatPriorityFilter(criteria, activities);
                return await Task.FromResult(activities);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetSimilarProducts",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}, {SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        #region Private methods

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        /// <summary>
        /// Method will return auto suggest data for respective language code
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        private CacheKey<AutoSuggest> GetAutoSuggestDataForLanguage(string affiliateId, string languageCode)
        {
            var autoSuggestData = _activityPersistence.GetAutoSuggestData(affiliateId);
            autoSuggestData = autoSuggestData?.FindAll(data => data.Languagecode.ToLowerInvariant().Equals(languageCode));

            var autoSuggestDataWithCacheKey = new CacheKey<AutoSuggest>
            {
                Id = $"{Constant.AutoSuggestData}-{languageCode}",
                CacheValue = autoSuggestData
            };

            return autoSuggestDataWithCacheKey;
        }

        /// <summary>
        /// Get filtered ticket in async
        /// </summary>
        /// <param name="activitiesToFilter"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        private List<Activity> FilterTickets(List<Activity> activitiesToFilter, ClientInfo clientInfo)
        {
            var activities = activitiesToFilter;
            var filteredActivities = activities.FindAll(a => activities.Contains(a));

            // Get data from Cosmos
            var ticketsByRegion = _masterCacheManager.GetFilteredTickets(Constant.FilteredTickets)?.CacheValue;

            if (ticketsByRegion == null)
            {
                #region Theme Park Tickets By Country

                ticketsByRegion = _masterService.GetFilteredThemeparkTicketsAsync().GetAwaiter().GetResult();

                var setTicketToCache = new CacheKey<Entities.TicketByRegion>
                {
                    Id = Constant.FilteredTickets,
                    CacheValue = ticketsByRegion
                };

                _masterCacheManager.SetFilteredTicketsToCache(setTicketToCache);

                #endregion Theme Park Tickets By Country
            }

            var activeTickets = new HashSet<int>(ticketsByRegion.Select(x => x.ThemeparkTicket.ProductId));
            filteredActivities.RemoveAll(p => activeTickets.Contains(p.ID));

            var restrictedCountries = new string[] { "GB", "IE", "CA", "AU", "US" };
            var clientCountry = clientInfo.CountryIp.Trim().ToUpperInvariant();

            if (!restrictedCountries.Contains(clientCountry))
                clientCountry = "ROW";

            var productsForThisCountry = from ticket in ticketsByRegion
                                         where ticket.CountryCode.ToUpperInvariant() == clientCountry.ToUpperInvariant()
                                         select ticket.ThemeparkTicket.ProductId;

            filteredActivities.AddRange(activities.FindAll(p => productsForThisCountry.Contains(p.ID)));
            activities = filteredActivities;
            return activities;
        }

        /// <summary>
        /// Match different affiliate criteria in async
        /// </summary>
        /// <param name="affiliateFilter"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool MatchAffiliateCriteria(AffiliateFilter affiliateFilter, Product product)
        {
            if (product == null)
                return false;

            bool matchesAffiliateCriteria = true;
            if (affiliateFilter != null)
            {
                if (affiliateFilter.DurationTypeFilter)
                {
                    matchesAffiliateCriteria = MatchAffiliateFilterDurationType(affiliateFilter, product);
                }
                if (matchesAffiliateCriteria && affiliateFilter.RegionFilter)
                {
                    matchesAffiliateCriteria = MatchAffiliateRegionCriteria(affiliateFilter, product);
                }
                if (matchesAffiliateCriteria && affiliateFilter.ActivityFilter)
                {
                    if (affiliateFilter.IsServiceExclusionFilter)
                        matchesAffiliateCriteria = !MatchAffiliateServiceCriteria(affiliateFilter, product); // Exculsion
                    else if (!affiliateFilter.IsServiceExclusionFilter)
                        matchesAffiliateCriteria = MatchAffiliateServiceCriteria(affiliateFilter, product); // Inclusion
                }
                if (matchesAffiliateCriteria && affiliateFilter.AffiliateActivityPriorityFilter)
                {
                    matchesAffiliateCriteria = MatchAffiliatePriority(affiliateFilter, product);
                }

                if (matchesAffiliateCriteria && affiliateFilter.IsMarginFilter)
                {
                    matchesAffiliateCriteria = MatchAffiliateMarginCriteria(affiliateFilter, product);
                }
            }
            return matchesAffiliateCriteria;
        }

        /// <summary>
        /// match affilaite duration criteria in async
        /// </summary>
        /// <param name="affiliateFilter"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool MatchAffiliateFilterDurationType(AffiliateFilter affiliateFilter, Product product)
        {
            var activityLite = product as ActivityLite;

            if (product == null || (activityLite == null))
                return false;

            var matchesDurationType = false;
            if (affiliateFilter.DurationTypeFilter && affiliateFilter.DurationTypes != null && affiliateFilter.DurationTypes.Count > 0)
            {
                var type = activityLite.ActivityType;

                foreach (var durationType in affiliateFilter.DurationTypes)
                {
                    if (type == durationType)
                    {
                        matchesDurationType = true;
                        break;
                    }
                }
            }
            return matchesDurationType;
        }

        /// <summary>
        /// match affiliate region criteria in async
        /// </summary>
        /// <param name="affiliateFilter"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool MatchAffiliateRegionCriteria(AffiliateFilter affiliateFilter, Product product)
        {
            if (product == null)
                return false;

            var matchesService = false;
            if (affiliateFilter.RegionFilter && affiliateFilter.Regions != null && affiliateFilter.Regions.Count > 0)
            {
                foreach (int regionId in affiliateFilter.Regions)
                {
                    var filteredRegions = product.Regions.FindAll(region => region.Id.Equals(regionId));
                    if (filteredRegions.Count > 0)
                    {
                        matchesService = true;
                        break;
                    }
                }
            }
            return matchesService;
        }

        /// <summary>
        /// match affiliate service criteria in async
        /// </summary>
        /// <param name="affiliateFilter"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool MatchAffiliateServiceCriteria(AffiliateFilter affiliateFilter, Product product)
        {
            if (product == null || product.ID == 0)
                return false;

            var matchesServices = affiliateFilter.ActivityFilter && affiliateFilter.Activities != null && affiliateFilter.Activities.Count > 0 && affiliateFilter.Activities.Any(activityId => product.ID == activityId && activityId != 0);

            return matchesServices;
        }

        /// <summary>
        /// match affiliate Margin criteria in async
        /// </summary>
        /// <param name="affiliateFilter"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool MatchAffiliateMarginCriteria(AffiliateFilter affiliateFilter, Product product)
        {
            if (product == null || product.ID == 0)
                return false;

            if (affiliateFilter.IsMarginFilter)
            {
                if (product.Margin != null && product.Margin.Value > affiliateFilter.AffiliateMargin)
                    return true;
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// This method is used to match the Affiliate Services Priority for the given product
        /// </summary>
        /// <param name="affiliateFilter"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool MatchAffiliatePriority(AffiliateFilter affiliateFilter, Product product)
        {
            if (product == null || product.ID == 0)
                return false;

            if (affiliateFilter.AffiliateActivityPriorityFilter && affiliateFilter.AffiliateServicesPriority != null
                                                                && affiliateFilter.AffiliateServicesPriority.Count > 0)
            {
                var servicePriority = affiliateFilter.AffiliateServicesPriority.Find(x => x.Key.Equals(product.ID));

                if (servicePriority.Value >= 0 && product is ActivityLite activity)
                {
                    activity.Priority = servicePriority.Value;
                }
            }

            return true;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private List<Activity> ActivitiesPostCatPriorityFilter(SearchCriteria searchCriteria, List<Activity> activitiesAllowed)
        {
            var catIds = new List<string>();

            if (!string.IsNullOrEmpty(searchCriteria.AttractionFilterIds))
            {
                //Split and store category ids in array
                catIds = searchCriteria.AttractionFilterIds.Split(':').ToList();
            }

            if (searchCriteria.CategoryId != 0 && string.IsNullOrEmpty(searchCriteria.AttractionFilterIds))
            {
                catIds.Add(searchCriteria.CategoryId.ToString());
            }

            if (catIds?.Count > 0 && activitiesAllowed?.Count > 0)
            {
                var categoryBidWiseList = new Dictionary<Activity, int>();
                var activitiesWithNoValue = new List<Activity>();
                foreach (var activity in activitiesAllowed)
                {
                    var catPrioritiesForActivity = new List<int>();
                    if (activity.PriorityWiseCategory == null || activity.PriorityWiseCategory?.Count == 0)
                    {
                        activitiesWithNoValue.Add(activity);
                    }
                    else
                    {
                        catIds = catIds.Where(categoryId => !string.IsNullOrEmpty(categoryId)).ToList();
                        foreach (var selectedCatId in catIds)
                        {
                            activity.PriorityWiseCategory.TryGetValue(Convert.ToInt32(selectedCatId), out var catIdWthHighestOrder);
                            if (catIdWthHighestOrder != 0)
                            {
                                catPrioritiesForActivity.Add(catIdWthHighestOrder);
                            }
                        }
                        if (catPrioritiesForActivity?.Count > 0)
                        {
                            catPrioritiesForActivity = catPrioritiesForActivity.OrderByDescending(priority => priority).ToList();
                            categoryBidWiseList.Add(activity, catPrioritiesForActivity.First());
                        }
                        else
                        {
                            activitiesWithNoValue.Add(activity);
                        }
                    }
                }

                if (categoryBidWiseList?.Count > 0)
                {
                    var sortedListToAddToStack = categoryBidWiseList.OrderByDescending(thisEntry => thisEntry.Value);
                    activitiesAllowed = new List<Activity>();
                    activitiesAllowed.AddRange(sortedListToAddToStack.Select(act => act.Key));
                    if (activitiesWithNoValue?.Count > 0)
                    {
                        activitiesAllowed.AddRange(activitiesWithNoValue);
                    }
                }
            }
            return activitiesAllowed;
        }

        #endregion Private methods
    }
}