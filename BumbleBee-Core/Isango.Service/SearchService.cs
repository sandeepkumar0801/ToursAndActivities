using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Affiliate;
using Isango.Entities.Availability;
using Isango.Entities.Enums;
using Isango.Entities.Region;
using Isango.Entities.Wrapper;
using Isango.Persistence.Contract;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using System.Diagnostics;
using Util;
using Activity = Isango.Entities.Activities.Activity;

namespace Isango.Service
{
    public class SearchService : ISearchService
    {
        private readonly IAffiliateService _affiliateService;
        private readonly IActivityPersistence _activityPersistence;
        private readonly ILogger _log;
        private readonly IActivityCacheManager _activityCacheManager;
        private readonly IMasterCacheManager _masterCacheManager;
        private readonly IMasterService _masterService;
        private readonly IActivityService _activityService;
        private readonly IHotelBedsActivitiesCacheManager _hotelBedsActivitiesCacheManager;
        private readonly int _maxParallelThreadCount;

        public SearchService(
            IAffiliateService affiliateService
            , IActivityPersistence activityPersistence
            , IActivityCacheManager activityCacheManager
            , IMasterCacheManager masterCacheManager
            , IMasterService masterService
            , IActivityService activityService
            , IHotelBedsActivitiesCacheManager hotelBedsActivitiesCacheManager
            , ILogger log)
        {
            _affiliateService = affiliateService;
            _activityPersistence = activityPersistence;
            _activityCacheManager = activityCacheManager;
            _masterCacheManager = masterCacheManager;
            _masterService = masterService;
            _activityService = activityService;
            _hotelBedsActivitiesCacheManager = hotelBedsActivitiesCacheManager;
            _log = log;
            _maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount(Constant.MaxParallelThreadCount);
        }

        /// <summary>
        /// Method will fetch the search data according to criteria
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="clientInfo"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public async Task<SearchStack> GetSearchDataAsync(SearchCriteria searchCriteria, ClientInfo clientInfo, Criteria criteria)
        {
            try
            {
                var searchStackWithActivityData = GetSearchStackWithActivityData(searchCriteria, clientInfo);
                if (searchStackWithActivityData != null)
                {
                    if (searchStackWithActivityData.Activities?.Count == 0)
                        return null;

                    var affiliateFilter = await _affiliateService.GetAffiliateFilterByIdAsync(clientInfo.AffiliateId);
                    searchStackWithActivityData.Activities?.RemoveAll(product => product.CategoryIDs == null);
                    var filteredActivities = GetFilterTickets(searchStackWithActivityData.Activities, clientInfo);
                    var activitiesAllowed = CalculatePrices(filteredActivities, affiliateFilter);
                    if (searchCriteria.IsOffer)
                    {
                        activitiesAllowed = activitiesAllowed.Where(model => model.OfferPercentage > 0);
                    }
                    if (searchCriteria.IsBundle)
                    {
                        activitiesAllowed = activitiesAllowed.Where(model => model.ActivityType == ActivityType.Bundle);
                    }
                    if (activitiesAllowed != null)
                        searchStackWithActivityData.Activities = activitiesAllowed.ToList();
                    if (searchCriteria.RegionId.Equals(0))
                    {
                        searchStackWithActivityData.Regions = searchStackWithActivityData.Activities?
                            .SelectMany(product => product.Regions, (product, region) => new { product, region })
                            .Where(t => t?.region.Type.ToString().ToLower() == Constant.City)
                            .Select(t => t.region).GroupBy(r => r.Id).Select(g => g.First()).ToList();
                    }

                    searchStackWithActivityData.Activities = _activityService.GetActivitiesWithLivePrice(clientInfo, criteria, searchStackWithActivityData.Activities);

                    foreach (var activity in searchStackWithActivityData.Activities)
                    {
                        activity.ProductOptions?.RemoveAll(x => x.BasePrice == null && x.CostPrice == null || x.BasePrice?.Amount == 0 && x.CostPrice.Amount == 0);
                    }

                    searchStackWithActivityData.Activities.RemoveAll(x => x.ProductOptions == null || x.ProductOptions.Count == 0);

                    return searchStackWithActivityData;
                }
                return null;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SearchService",
                    MethodName = "GetSearchDataAsync",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(searchCriteria)}, {SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<SearchStack> GetSearchDataV2Async(SearchCriteria searchCriteria, ClientInfo clientInfo, Criteria criteria)
        {
            try
            {
                var processorCount = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 1.0));

                if (processorCount <= 0)
                {
                    processorCount = 1;
                }

                var _parallelOption = new System.Threading.Tasks.ParallelOptions
                {
                    MaxDegreeOfParallelism = processorCount
                };

                var searchStackWithActivityData = GetSearchStackWithActivityDataV2(searchCriteria, clientInfo);
                if (searchStackWithActivityData != null)
                {
                    if (searchStackWithActivityData.Activities?.Count == 0)
                        return null;

                    var affiliateFilter = await _affiliateService.GetAffiliateFilterByIdAsync(clientInfo.AffiliateId);
                    searchStackWithActivityData.Activities?.RemoveAll(product => product.CategoryIDs == null);
                    var filteredActivities = GetFilterTickets(searchStackWithActivityData.Activities, clientInfo);
                    var activitiesAllowed = AffiliateFilter(filteredActivities, affiliateFilter);
                    if (searchCriteria.IsOffer)
                    {
                        activitiesAllowed = activitiesAllowed.Where(model => model.OfferPercentage > 0);
                    }
                    if (searchCriteria.IsBundle)
                    {
                        activitiesAllowed = activitiesAllowed.Where(model => model.ActivityType == ActivityType.Bundle);
                    }
                    if (activitiesAllowed != null)
                        searchStackWithActivityData.Activities = activitiesAllowed.ToList();

                    searchStackWithActivityData.Activities = searchStackWithActivityData.Activities.Where(x => x != null).ToList();

                    if (searchCriteria.RegionId.Equals(0))
                    {
                        searchStackWithActivityData.Regions = searchStackWithActivityData?.Activities?
                          .SelectMany(product => product?.Regions, (product, region) => new { product, region })
                          ?.Where(t => (t?.region?.Type.ToString())?.ToLower() == Constant.City)
                          ?.Select(t => t.region)?.GroupBy(r => r.Id)?.Select(g => g.FirstOrDefault())?.ToList();
                    }

                    searchStackWithActivityData.Activities = _activityService.GetActivitiesWithLivePrice(clientInfo, criteria, searchStackWithActivityData.Activities);

                    //Parallel.ForEach(searchStackWithActivityData.Activities, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, activity =>
                    //{
                    //    activity.ProductOptions?.RemoveAll(x => x.BasePrice == null && x.CostPrice == null || x.BasePrice?.Amount == 0 && x.CostPrice.Amount == 0);
                    //});

                    searchStackWithActivityData.Activities.RemoveAll(x => x.ProductOptions == null || x.ProductOptions.Count == 0);

                    return searchStackWithActivityData;
                }
                return null;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SearchService",
                    MethodName = "GetSearchDataAsync",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(searchCriteria)}, {SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <param name="searchCriteria"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public SearchStack GetSearchStackWithActivityData(SearchCriteria searchCriteria, ClientInfo clientInfo)
        {
            try
            {
                var stack = GetSearchStackData(searchCriteria, clientInfo);
                //Get data from cosmos
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var result = _activityCacheManager.SearchActivities(searchCriteria, clientInfo);
                stopWatch.Stop();
                long duration = stopWatch.ElapsedMilliseconds;
                if (result == null || result.Activities.Count == 0)
                {
                    result = _activityPersistence.SearchActivities(searchCriteria, clientInfo);
                }

                if (result?.Activities != null && result.Activities.Count > 0)
                {
                    //Date Filter on cache data
                    if (!string.IsNullOrEmpty(searchCriteria.SelectedDates))
                    {
                        result.Activities = GetSearchDataWithDateFilter(searchCriteria, result.Activities, clientInfo);
                    }

                    searchCriteria.PageNumber = searchCriteria.PageNumber <= 0 ? 1 : searchCriteria.PageNumber;

                    //Filter activity for NUll Prices.
                    //remove all the product Option from activity having Base and Cost Price Null
                    //If all the ProductOption have null price then remove the Activity.
                    result.Activities = FilterActivityForNullPrices(result.Activities);

                    result.TotalActivities = result.Activities.Count;

                    stack.Activities = result.Activities;
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
                    ClassName = "SearchService",
                    MethodName = "GetSearchStackWithActivityData",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(searchCriteria)}, {SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private SearchStack GetSearchStackWithActivityDataV2(SearchCriteria searchCriteria, ClientInfo clientInfo)
        {
            try
            {
                var stack = GetSearchStackDataV2(searchCriteria, clientInfo);
                //Get data from cosmos
                //var stopWatch = new Stopwatch();
                //stopWatch.Start();
                var result = _activityCacheManager.SearchActivities(searchCriteria, clientInfo);
                //stopWatch.Stop();
                //long duration = stopWatch.ElapsedMilliseconds;
                if (result == null || result.Activities.Count == 0)
                {
                    result = _activityPersistence.SearchActivities(searchCriteria, clientInfo);
                }

                if (result?.Activities?.Count > 0)
                {
                    //Date Filter on cache data
                    if (!string.IsNullOrEmpty(searchCriteria.SelectedDates))
                    {
                        result.Activities = GetSearchDataWithDateFilter(searchCriteria, result.Activities, clientInfo);
                    }

                    searchCriteria.PageNumber = searchCriteria.PageNumber <= 0 ? 1 : searchCriteria.PageNumber;

                    //Filter activity for NUll Prices.
                    //remove all the product Option from activity having Base and Cost Price Null
                    //If all the ProductOption have null price then remove the Activity.
                    //result.Activities = FilterActivityForNullPrices(result.Activities);

                    result.TotalActivities = result.Activities.Count;

                    stack.Activities = result.Activities;
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
                    ClassName = "SearchService",
                    MethodName = "GetSearchStackWithActivityDataV2",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(searchCriteria)}, {SerializeDeSerializeHelper.Serialize(clientInfo)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        //TODO: will be removed after reviewed by Prashant.
        /// <summary>
        /// Get search stack data by criteria and client info
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        private SearchStack GetSearchStackData(SearchCriteria criteria, ClientInfo clientInfo)
        {
            try
            {
                var stack = new SearchStack();
                var loadRegionCategoryMapping = LoadRegionCategoryMapping();
                if (loadRegionCategoryMapping?.Count > 0)
                {
                    var currentMapping = loadRegionCategoryMapping.FindAll(region =>
                        region.RegionId.Equals(criteria.RegionId) || region.CountryId.Equals(criteria.RegionId));

                    if (currentMapping.Count > 0)
                    {
                        currentMapping = currentMapping.FindAll(mapping =>
                            (mapping.Languagecode + "").ToLowerInvariant().Equals(clientInfo.LanguageCode.ToLowerInvariant()));
                    }

                    if (currentMapping.Count > 0 && criteria.CategoryId.Equals(0))
                    {
                        stack.RegionCategoryMappings = currentMapping;
                    }
                    stack.RegionMeta = _activityPersistence.LoadRegionMetaData(criteria.RegionId, criteria.CategoryId, clientInfo.LanguageCode);
                }

                return stack;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SearchService",
                    MethodName = "GetSearchStackData",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private SearchStack GetSearchStackDataV2(SearchCriteria criteria, ClientInfo clientInfo)
        {
            try
            {
                var stack = new SearchStack();
                var loadRegionCategoryMapping = LoadRegionCategoryMappingV2();
                if (loadRegionCategoryMapping?.Count > 0)
                {
                    var currentMapping = loadRegionCategoryMapping.FindAll(region =>
                        region.RegionId.Equals(criteria.RegionId) || region.CountryId.Equals(criteria.RegionId));

                    if (currentMapping.Count > 0)
                    {
                        currentMapping = currentMapping.FindAll(mapping =>
                            (mapping.Languagecode + "").ToLowerInvariant().Equals(clientInfo.LanguageCode.ToLowerInvariant()));
                    }

                    if (currentMapping.Count > 0 && criteria.CategoryId.Equals(0))
                    {
                        stack.RegionCategoryMappings = currentMapping;
                    }
                    stack.RegionMeta = _activityPersistence.LoadRegionMetaData(criteria.RegionId, criteria.CategoryId, clientInfo.LanguageCode);
                }

                return stack;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SearchService",
                    MethodName = "GetSearchStackDataV2",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<RegionCategoryMapping> LoadRegionCategoryMapping()
        {
            /* commented old code --Not working
            var regionCategoryMappings = _activityCacheManager.GetRegioncategoryMapping() ?? _activityPersistence.LoadRegionCategoryMapping();

            return regionCategoryMappings;
            */

            var regionCategoryMappings = _activityCacheManager.GetRegioncategoryMapping();
            if (regionCategoryMappings?.Any() == false)
            {
                regionCategoryMappings = _activityPersistence.LoadRegionCategoryMapping();
            }
            return regionCategoryMappings;
        }

        private List<RegionCategoryMapping> LoadRegionCategoryMappingV2()
        {
            var regionCategoryMappings = _activityCacheManager.GetRegioncategoryMappingV2("RegionCategoryMapping")?.CacheValue;
            if (regionCategoryMappings == null || regionCategoryMappings.Any() == false)
            {
                regionCategoryMappings = _activityPersistence.LoadRegionCategoryMapping();
            }
            return regionCategoryMappings;
        }

        /// <summary>
        /// This method fetches the filter tickets from cache or from Db
        /// </summary>
        /// <param name="activitiesToFilter"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        private List<Activity> GetFilterTickets(List<Activity> activitiesToFilter, ClientInfo clientInfo)
        {
            try
            {
                var activities = activitiesToFilter.FindAll(activitiesToFilter.Contains);
                // Get data from Cosmos

                var ticketsByRegionResult = _masterCacheManager.GetFilteredTickets(Constant.FilteredTickets)?.CacheValue;

                if (ticketsByRegionResult == null)
                {
                    ticketsByRegionResult = _masterService.GetFilteredThemeparkTicketsAsync().GetAwaiter().GetResult();
                    var ticketsByRegion = new CacheKey<Entities.TicketByRegion>
                    {
                        Id = Constant.FilteredTickets,
                        CacheValue = ticketsByRegionResult
                    };
                    _masterCacheManager.SetFilteredTicketsToCache(ticketsByRegion);
                }
                var activeTickets = new HashSet<int>(ticketsByRegionResult.Select(x => x.ThemeparkTicket.ProductId));

                activities.RemoveAll(p => activeTickets.Contains(p.ID));

                string[] restrictedCountries = { Constant.GB, Constant.IE, Constant.CA, Constant.AU, Constant.US };

                var clientCountry = clientInfo.CountryIp.Trim().ToUpperInvariant();

                if (!restrictedCountries.Contains(clientCountry))
                    clientCountry = Constant.ROW;

                var productsForThisCountry = ticketsByRegionResult
                    .Where(ticket => string.Equals(ticket.CountryCode, clientCountry, StringComparison.InvariantCultureIgnoreCase))
                    .Select(ticket => ticket.ThemeparkTicket.ProductId);

                activities.AddRange(activities.FindAll(p => productsForThisCountry.Contains(p.ID)));
                return activities;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SearchService",
                    MethodName = "GetFilterTickets",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This methods matches the affiliate filters and calculate the sell price
        /// </summary>
        /// <param name="filteredActivities"></param>
        /// <param name="affiliateFilter"></param>
        private IEnumerable<Activity> CalculatePrices(List<Activity> filteredActivities, AffiliateFilter affiliateFilter)
        {
            try
            {
                if ((filteredActivities?.Count < 0)) return new List<Activity>();
                var activitiesAllowed = new List<Activity>();
                if (filteredActivities != null)
                {
                    Parallel.ForEach(filteredActivities, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, filteredActivity =>
                    {
                        if (!_activityService.MatchAllAffiliateCriteria(affiliateFilter, filteredActivity))
                            return;

                        activitiesAllowed.Add(filteredActivity);
                    });
                }

                return activitiesAllowed;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SearchService",
                    MethodName = "CalculatePrices",
                 };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private IEnumerable<Activity> AffiliateFilter(List<Activity> filteredActivities, AffiliateFilter affiliateFilter)
        {
            try
            {
                if ((filteredActivities?.Count < 0)) return new List<Activity>();
                var activitiesAllowed = new List<Activity>();
                if (filteredActivities != null)
                {
                    Parallel.ForEach(filteredActivities, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, filteredActivity =>
                    {
                        if (!_activityService.MatchAllAffiliateCriteria(affiliateFilter, filteredActivity))
                        {
                            return;
                        }
                        if (filteredActivity != null)
                        {
                            activitiesAllowed.Add(filteredActivity);
                        }
                    });
                }

                return activitiesAllowed;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SearchService",
                    MethodName = "AffiliateFilter",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method will apply date filter on activity cache data
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="activities"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        private List<Activity> GetSearchDataWithDateFilter(SearchCriteria searchCriteria,
            List<Activity> activities, ClientInfo clientInfo)
        {
            try
            {
                var filteredActivities = new List<Activity>();
                if (!string.IsNullOrWhiteSpace(searchCriteria.SelectedDates))
                {
                    var userDateRange = SetDates(searchCriteria.SelectedDates.Trim().ToLower());
                    var startDate = userDateRange[0];
                    var endDate = userDateRange[1];

                    if (startDate <= DateTime.Now.AddDays(Constant.MaxDays))
                    {
                        var serviceIds = new List<int>();
                        var activityAvailabilities = GetAvailabilityData(searchCriteria, clientInfo);
                        Parallel.ForEach(activityAvailabilities, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, availability =>
                        {
                            var availableDates = availability.BaseDateWisePriceAndAvailability.Select(d => d.Key).ToList();
                            if (availableDates.Any(availableDate => availableDate.Date >= startDate && availableDate.Date <= endDate))
                            {
                                serviceIds.Add(availability.ServiceId);
                            }
                        });

                        filteredActivities = activities.Where(i => serviceIds.Contains(i.ID)).ToList();
                    }
                }

                return filteredActivities;

            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SearchService",
                    MethodName = "GetSearchDataWithDateFilter",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }


        /// <summary>
        /// Method will set start and end dates
        /// </summary>
        /// <param name="dates"></param>
        /// <returns></returns>
        private List<DateTime> SetDates(string dates)
        {
            DateTime startDate;
            DateTime endDate;
            try
            {
                //Skipped the switch cases on 'today', 'tomorrow' and 'seven', also ':' condition
                if (dates.Contains(Constant.DateSeparator))
                {
                    var allDates = dates.Split('@');
                    var startDay = allDates[0].Split('-')[0];
                    var startMonth = allDates[0].Split('-')[1];
                    var startYear = allDates[0].Split('-')[2];
                    startDate = new DateTime(int.Parse(startYear), int.Parse(startMonth), int.Parse(startDay)).Date;

                    var endDay = allDates[1].Split('-')[0];
                    var endMonth = allDates[1].Split('-')[1];
                    var endYear = allDates[1].Split('-')[2];
                    endDate = new DateTime(int.Parse(endYear), int.Parse(endMonth), int.Parse(endDay)).Date;
                }
                else
                {
                    var startDay = dates.Split('-')[0];
                    var startMonth = dates.Split('-')[1];
                    var startYear = dates.Split('-')[2];
                    startDate = new DateTime(int.Parse(startYear), int.Parse(startMonth), int.Parse(startDay)).Date;
                    endDate = startDate.AddDays(Constant.AddSixDays).Date;
                }

                var userDateRange = new List<DateTime> { startDate, endDate };
                return userDateRange;

            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SearchService",
                    MethodName = "SetDates",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method will get live isango data according to criteria
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        private List<Availability> GetAvailabilityData(SearchCriteria searchCriteria, ClientInfo clientInfo)
        {
            var availabilities = new List<Availability>();
            try
            {
                if (!string.IsNullOrEmpty(searchCriteria.Keyword) && searchCriteria.RegionId == 0)
                {
                    var mappings = _activityPersistence.GetFullTextSearchActivitiyIdMapping(string.Empty, searchCriteria.Keyword, clientInfo);
                    if (mappings != null && mappings.Count > 0)
                    {
                        var regionIds = mappings.Select(x => x.RegionId).Distinct().ToList();
                        foreach (var regionId in regionIds)
                        {
                            availabilities.AddRange(GetRegionAvailabilities(regionId.ToString()));
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(searchCriteria.RegionIDs))
                {
                    availabilities.AddRange(GetRegionAvailabilities(searchCriteria.RegionIDs));
                }
                else if (searchCriteria.CategoryId != 0)
                {
                    var regionIdsFromAttractionId =
                        _activityPersistence.GetRegionIdsFromAttractionId(clientInfo.AffiliateId,
                            searchCriteria.CategoryId);

                    foreach (var regionId in regionIdsFromAttractionId)
                    {
                        availabilities.AddRange(GetRegionAvailabilities(regionId));
                    }
                }
                else
                {
                    availabilities.AddRange(GetRegionAvailabilities(searchCriteria.RegionId.ToString()));
                }

                return availabilities;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SearchService",
                    MethodName = "GetAvailabilityData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get availability by region id
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        private List<Availability> GetRegionAvailabilities(string regionId)
        {
            //Get data from cache
            return _hotelBedsActivitiesCacheManager.GetAvailability(regionId, string.Empty);
        }

        /// <summary>
        /// Filter activity for NUll Prices.
        /// Remove all the product Option from activity having Base and Cost Price Null
        /// If all the ProductOption have null price then remove the Activity.
        /// </summary>
        /// <param name="activities"></param>
        /// <returns></returns>
        private List<Activity> FilterActivityForNullPrices(List<Activity> activities)
        {
            try
            {
                foreach (var activity in activities)
                {
                    activity.ProductOptions?.RemoveAll(x => x.BasePrice == null || x.CostPrice == null || x.BasePrice.Amount == 0 || x.CostPrice.Amount == 0);
                }

                activities.RemoveAll(x => x.ProductOptions == null || x.ProductOptions.Count == 0);

                return activities;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SearchService",
                    MethodName = "FilterActivityForNullPrices",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}