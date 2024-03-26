using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Tiqets;
using Isango.Persistence.Contract;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Tiqets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class TiqetsCriteriaService : ITiqetsCriteriaService
    {
        private readonly ITiqetsAdapter _tiqetsAdapter;
        private readonly ITiqetsPaxMappingCacheManager _cacheManager;
        private readonly IMasterPersistence _masterPersistence;
        private readonly ILogger _log;

        public TiqetsCriteriaService(ITiqetsAdapter tiqetsAdapter, ITiqetsPaxMappingCacheManager cacheManager, ILogger logger, IMasterPersistence masterPersistence)
        {
            _tiqetsAdapter = tiqetsAdapter;
            _cacheManager = cacheManager;
            _log = logger;
            _masterPersistence = masterPersistence;
        }

        /// <summary>
        /// Get Availability
        /// </summary>
        /// <returns></returns>
        public List<Activity> GetAvailability(ServiceAvailability.Criteria criteria)
        {
            var token = criteria.Token;
            var counter = criteria?.Counter ?? 1;
            //var taskLength = criteria.MappedProducts.Count * counter;
            //var taskArray = new Task<Activity>[taskLength];
            

            if (counter <= 0) return null;

            var activities = new List<Activity>();
            try
            {

                var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount();
                if (maxParallelThreadCount > 4)
                {
                    maxParallelThreadCount = 4;
                }
                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount };

                for (var i = 1; i <= counter; i++)
                {
                    var count = 0;
                    for (var start = DateTime.Now.AddDays(criteria.Days2Fetch * (i - 1)).Date; start.Date < DateTime.Now.AddDays(criteria.Days2Fetch * i).Date; start = start.AddDays(criteria.Days2Fetch).Date)
                    {
                        var mappedProducts = criteria.MappedProducts;
                        foreach (var batch in mappedProducts?.Batch(10))
                        {

                            try
                            {
                                var taskArray = new Task<Activity>[batch.Count()];

                                #region batch processing
                                int j = 0;
                                foreach (var item in batch)
                                {

                                    taskArray[j] = Task.Run(() => GetTiqetsActivities(item, criteria, start, token));
                                    count++; j++;
                                }
                                try
                                {
                                    if (taskArray?.Length > 0)
                                    {
                                        Task.WaitAll(taskArray);
                                        Parallel.ForEach(taskArray, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, task =>
                                        {
                                            var data = task.GetAwaiter().GetResult();
                                            if (data?.ProductOptions?.Count > 0)
                                                activities.Add(data);
                                        });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var isangoErrorEntity = new IsangoErrorEntity
                                    {
                                        ClassName = "TiqetsCriteriaService",
                                        MethodName = "GetAvailability",
                                        Token = criteria.Token
                                    };
                                    _log.Error(isangoErrorEntity, ex);

                                }
                            }
                            catch (Exception ex)
                            {

                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = "TiqetsCriteriaService",
                                    MethodName = "GetAvailability",
                                    Token = criteria.Token
                                };
                                _log.Error(isangoErrorEntity, ex);
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return activities;
        }

        /// <summary>
        /// Get service details
        /// </summary>
        /// <param name="activities"></param>
        /// <param name="mappedProducts"></param>
        /// <returns></returns>
        public List<ServiceAvailability.TempHBServiceDetail> GetServiceDetails(List<Activity> activities,
            List<IsangoHBProductMapping> mappedProducts)
        {
            try
            {
                var serviceDetails = new List<ServiceAvailability.TempHBServiceDetail>();

                //Not added null check in this method as it is already added in the parent method
                foreach (var activity in activities)
                {
                    if (activity == null) continue;
                    var mappedProduct = mappedProducts.FirstOrDefault(x => x.IsangoHotelBedsActivityId.Equals(activity.ID));

                    if (mappedProduct == null) continue;
                    var serviceMapper = new ServiceMapper();
                    var details = serviceMapper.ProcessServiceDetailsWithBaseAndCostPrices(activity, mappedProduct);
                    serviceDetails.AddRange(details);
                }

                return serviceDetails;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TiqetsCriteriaService",
                    MethodName = "GetServiceDetails"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #region "Private Methods"

        private Task<Activity> GetTiqetsActivities(IsangoHBProductMapping item, ServiceAvailability.Criteria criteria, DateTime startDate, string token = null)
        {
            if (string.IsNullOrEmpty(token))
            {
                token = Guid.NewGuid().ToString();
            }
            //Get Pax Mappings from cache
            var paxMappings = _cacheManager.GetPaxMappings()?.Where(x => x.ServiceOptionId == item.ServiceOptionInServiceid).ToList();
            if (paxMappings == null)
            {
                paxMappings = _masterPersistence.LoadTiqetsPaxMappings()?.Where(x => x.APIType == APIType.Tiqets).ToList()?.Where(x => x.ServiceOptionId == item.ServiceOptionInServiceid).ToList();
            }
            var noOfPassengers = paxMappings?.Select(e => e.PassengerType).Distinct().ToDictionary(x => x, x => item.MinAdultCount);
            //Prepare criteria
            var tiqetsCriteria = new TiqetsCriteria
            {
                ProductId = item.FactSheetId,
                Language = Constant.DefaultLanguage,
                CheckinDate = startDate.Date,
                CheckoutDate = (startDate.AddDays(criteria.Days2Fetch)).Date,
                TiqetsPaxMappings = paxMappings,
                NoOfPassengers = noOfPassengers,
                OptionId = item.ServiceOptionInServiceid
            };

            var activity = new Activity
            {
                ID = item.IsangoHotelBedsActivityId,
                Code = item.HotelBedsActivityCode,
                FactsheetId = item.FactSheetId,
                CurrencyIsoCode = item.CurrencyISOCode,
                ApiType = APIType.Tiqets,
                PriceTypeId = (PriceTypeId)item.PriceTypeId,
                ProductOptions = new List<ProductOption>()
            };

            //Get price and availability from supplier api
            var productOptions = _tiqetsAdapter.GetAvailabilities(tiqetsCriteria, token);

            if (productOptions?.Count > 0)
            {
                activity.ProductOptions = SetOptionIds(productOptions); //Setting ServiceOptionId to Id fields of ProductOption as we need to dump ServiceOptionId
                
            }
            return Task.FromResult<Activity>(activity);
        }

        private List<ProductOption> SetOptionIds(List<ProductOption> productOptions)
        {
            foreach (var option in productOptions)
            {
                option.Id = option.ServiceOptionId;
            }

            return productOptions;
        }

        #endregion "Private Methods"
    }
}