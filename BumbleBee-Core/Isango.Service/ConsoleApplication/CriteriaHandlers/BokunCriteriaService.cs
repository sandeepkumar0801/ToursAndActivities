using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Bokun;
using Isango.Entities.Enums;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Bokun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class BokunCriteriaService : IBokunCriteriaService
    {
        private readonly IBokunAdapter _bokunAdapter;
        private readonly ILogger _log;
        private readonly IActivityService _activityService;

        public BokunCriteriaService(IBokunAdapter bokunAdapter, IActivityService activityService, ILogger logger)
        {
            _bokunAdapter = bokunAdapter;
            _activityService = activityService;
            _log = logger;
        }

        /// <summary>
        /// Get Bokun Availabilities
        /// </summary>
        /// <param name="serviceCriteria"></param>
        /// <returns></returns>
        public List<Activity> GetAvailability(ServiceAvailability.Criteria serviceCriteria, List<PriceCategory> priceCategoryIdMapping)
        {
            try
            {
                var mappedProducts = serviceCriteria?.MappedProducts;

                if (mappedProducts == null || mappedProducts?.Count <= 0) return null;

                var activityIdsList = new List<int>();
                var activityList = new List<Activity>();

                foreach (var item in mappedProducts)
                {
                    //var taskArrayLength = mappedProducts.Count() * serviceCriteria.Counter;
                    //var taskArray = new Task<List<Activity>>[serviceCriteria.Counter];
                    var count = 0;

                    //var isangoActivity = _activityService.GetActivityById(item.IsangoHotelBedsActivityId, DateTime.Today, Constants.Constant.En)?.GetAwaiter().GetResult();

                    //if (isangoActivity != null)
                    //{
                        if (!activityIdsList.Contains(item.IsangoHotelBedsActivityId))
                        {
                            var factsheetIds = mappedProducts.Where(x => x.IsangoHotelBedsActivityId == item.IsangoHotelBedsActivityId).Select(x => Convert.ToInt32(x.HotelBedsActivityCode)).Distinct().ToList();

                            var priceCategories = priceCategoryIdMapping.Where(x => x.ServiceId == item.IsangoHotelBedsActivityId).ToList();
                            var validPassengerTypes = priceCategories.Select(x => x.PassengerTypeId).Distinct();
                            var noOfPassengers = validPassengerTypes.ToDictionary(x => x, x => item.MinAdultCount);

                            try
                            {
                                for (int i = 1; i <= serviceCriteria.Counter; i++)
                                {
                                    var criteria = new BokunCriteria
                                    {
                                        ActivityId = item.IsangoHotelBedsActivityId,
                                        FactSheetIds = factsheetIds,
                                        CheckinDate = DateTime.Now.Date.AddDays(serviceCriteria.Days2Fetch * (i - 1)),
                                        CheckoutDate = DateTime.Now.Date.AddDays(serviceCriteria.Days2Fetch * i),
                                        NoOfPassengers = noOfPassengers,
                                        CurrencyIsoCode = item.CurrencyISOCode,
                                        Token = serviceCriteria.Token
                                    };

                                    var query = from priceCategory in priceCategories
                                                from noOfPassenger in criteria.NoOfPassengers
                                                where priceCategory.PassengerTypeId == noOfPassenger.Key
                                              && priceCategory.ServiceId == item.IsangoHotelBedsActivityId
                                                select priceCategory;
                                    criteria.PriceCategoryIdMapping = query.ToList();

                                    //Commented to reduce parallel hit counts
                                    //taskArray[count] = Task.Run(() => _bokunAdapter.CheckAvailabilities(criteria, serviceCriteria.Token));
                                    count++;

                                    //TODO: Console Application: Getting null FactSheet ids from "isangolive.dbo.usp_get_HBLiveOptions" sp for Bokun API that's y API return null
                                    //Un-Commented to reduce parallel hit counts
                                    var activities = _bokunAdapter.CheckAvailabilities(criteria, null, serviceCriteria.Token);

                                    try
                                    {
                                        if (activities?.Count > 0)
                                        {
                                            foreach (var activity in activities)
                                            {
                                                activity.ProductOptions?.ForEach(x =>
                                                {
                                                    x.Id = item.ServiceOptionInServiceid;
                                                    x.CommisionPercent = item.MarginAmount;
                                                });
                                                //Adding the PriceTypeId of MappedProduct in the Activity
                                                activity.PriceTypeId = (PriceTypeId)item.PriceTypeId;
                                                activityList.Add(activity);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _log.Error("BokunCriteriaService|GetAvailability|taskArray", ex);
                                        continue;
                                    }
                                }
                            }


                            catch (Exception ex)
                            {
                                _log.Error("BokunCriteriaService|GetAvailability", ex);
                            }

                            //Commented to reduce parallel hit counts
                            //try
                            //{
                            //    if (taskArray.Length > 0)
                            //    {
                            //        Task.WaitAll(taskArray);
                            //        foreach (var task in taskArray)
                            //        {
                            //            try
                            //            {
                            //                var data = task.GetAwaiter().GetResult();
                            //                if (data?.Count > 0)
                            //                {
                            //                    var activity = data.FirstOrDefault();
                            //                    activity.ProductOptions?.ForEach(x =>
                            //                    {
                            //                        x.Id = item.ServiceOptionInServiceid;
                            //                        x.CommisionPercent = item.MarginAmount;
                            //                    });
                            //                    //Adding the PriceTypeId of MappedProduct in the Activity
                            //                    activity.PriceTypeId = (PriceTypeId)item.PriceTypeId;
                            //                    activityList.Add(activity);
                            //                }
                            //            }
                            //            catch(Exception ex)
                            //            {
                            //                _log.Error("BokunCriteriaService|GetAvailability|taskArray", ex);
                            //                continue;
                            //            }

                            //        }

                            //    }
                            //}
                            //catch (Exception ex)
                            //{
                            //    _log.Error("BokunCriteriaService|GetAvailability|taskArray", ex);
                            //}

                            activityIdsList.Add(item.IsangoHotelBedsActivityId);
                        }
                    //}
                }
                return activityList;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BokunCriteriaService",
                    MethodName = "GetAvailability",
                    Token = serviceCriteria.Token
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get service details
        /// </summary>
        /// <param name="activities"></param>
        /// <param name="mappedProducts"></param>
        /// <returns></returns>
        public List<ServiceAvailability.TempHBServiceDetail> GetServiceDetails(List<Activity> activities, List<IsangoHBProductMapping> mappedProducts)
        {
            try
            {
                var serviceDetails = new List<ServiceAvailability.TempHBServiceDetail>();

                //Not added null check in this method as it is already added in the parent method
                foreach (var activity in activities)
                {
                    if (activity == null) continue;
                    var mappedProductsById = mappedProducts?.Where(x => x.IsangoHotelBedsActivityId.Equals(activity.ID)).ToList();

                    if (mappedProductsById?.Count <= 0) continue;
                    var serviceMapper = new ServiceMapper();
                    var details = serviceMapper.ProcessServiceDetailsWithBasePrice(activity, mappedProductsById);
                    serviceDetails.AddRange(details);
                }

                return serviceDetails;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BokunCriteriaService",
                    MethodName = "GetServiceDetails"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public List<GoogleCancellationPolicy> GetBokunCancellationPolicies(List<IsangoHBProductMapping> products, string token)
        {
            var cancellationPolicies = new List<GoogleCancellationPolicy>();
            foreach (var product in products)
            {
                var activityDetail = _bokunAdapter.GetActivity(product.HotelBedsActivityCode, token);
                if (activityDetail == null) continue;

                var cancellationPrices = new List<GoogleCancellationPrice>();
                foreach (var penaltyRule in activityDetail.CancellationPolicy.PenaltyRules)
                {
                    var cancellationPrice = new GoogleCancellationPrice
                    {
                        CutoffHours = penaltyRule.CutoffHours.ToString(),
                        CancellationCharge = penaltyRule.Charge.Value,
                        IsPercentage = penaltyRule.ChargeType == Constants.Constant.Percentage
                    };
                    cancellationPrices.Add(cancellationPrice);
                }
                var cancellationPolicy = new GoogleCancellationPolicy
                {
                    ActivityId = product.IsangoHotelBedsActivityId,
                    OptionId = product.ServiceOptionInServiceid,
                    CancellationPrices = cancellationPrices
                };
                cancellationPolicies.Add(cancellationPolicy);
            }
            return cancellationPolicies;
        }
    }
}