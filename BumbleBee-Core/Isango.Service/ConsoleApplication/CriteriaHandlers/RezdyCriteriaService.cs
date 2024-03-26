using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.Enums;
using Isango.Entities.Rezdy;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Rezdy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class RezdyCriteriaService : IRezdyCriteriaService
    {
        private readonly IRezdyAdapter _rezdyAdapter;
        private readonly ILogger _log;
        private readonly IMasterCacheManager _masterCacheManager;
        private readonly IActivityPersistence _activityPersistence;

        public RezdyCriteriaService(IRezdyAdapter rezdyAdapter, ILogger log, IMasterCacheManager masterCacheManager, IActivityPersistence activityPersistence)
        {
            _rezdyAdapter = rezdyAdapter;
            _log = log;
            _masterCacheManager = masterCacheManager;
            _activityPersistence = activityPersistence;
        }

        public List<Activity> GetAvailability(Entities.ConsoleApplication.ServiceAvailability.Criteria serviceCriteria)
        {
            try
            {
                var counter = serviceCriteria?.Counter;

#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
                if (!(counter > 0)) return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

                var lstActivity = new List<Activity>();

                var rezdyPaxMapping = _masterCacheManager.GetRezdysPaxMappings();
                if (rezdyPaxMapping == null)
                {
                    rezdyPaxMapping = _activityPersistence.GetRezdyPaxMappings().Where(x => x.APIType == APIType.Rezdy).ToList();

                }
                var count = 0;
                for (var i = 1; i <= counter; i++)
                {
                    for (var start = DateTime.Now.AddDays(serviceCriteria.Days2Fetch * (i - 1)).Date; start.Date < DateTime.Now.AddDays(serviceCriteria.Days2Fetch * i).Date; start = start.AddDays(serviceCriteria.Days2Fetch).Date)
                    {
                        var mappedProducts = serviceCriteria.MappedProducts;

                        foreach (var item in mappedProducts)
                        {
                            try
                            {
                            var activityAgeGroups = _activityPersistence.GetPassengerInfoDetails(item.IsangoHotelBedsActivityId.ToString());

                            var rezdyProduct = _rezdyAdapter.GetProductDetails(item.HotelBedsActivityCode, string.Empty);
                            //From API , we get below AgentPaymentType:
                            //1.PAYOUTS, 2.FULL_AGENT, 3.DOWNPAYMENT, 4.FULL_SUPPLIER, 5.NONE
                            if (rezdyProduct?.AgentPaymentType.ToLowerInvariant() != "full_agent")
                            {
                                continue;
                            }

                            var criteria = new RezdyCriteria
                            {
                                ProductCodes = new List<string> { item.HotelBedsActivityCode },
                                CheckinDate = start,
                                CheckoutDate = start.AddDays(serviceCriteria.Days2Fetch),
                                PassengerMappings = new List<RezdyPassengerMapping>(),
                                NoOfPassengers = activityAgeGroups.ToDictionary(x => (PassengerType)x.PassengerTypeId, x => item.MinAdultCount),
                                PassengerAgeGroupIds = activityAgeGroups.ToDictionary(x => (PassengerType)x.PassengerTypeId, x => x.AgeGroupId),
                                RezdyPaxMappings = rezdyPaxMapping,
                                CommissionPercent= item.MarginAmount,
                                IsCommissionPercent=item.IsMarginPercent
                            };

                            if (rezdyProduct?.PriceOptions == null)
                            {
                                continue;
                            }

                            foreach (var priceOption in rezdyProduct.PriceOptions)
                            {
                                var passengerinfo = rezdyPaxMapping.FirstOrDefault(x => x.AgeGroupCode.ToLowerInvariant()==priceOption.Label.ToLowerInvariant()
            && x.SupplierCode.ToLowerInvariant()==priceOption.ProductCode.ToLowerInvariant());
                                if (passengerinfo != null)
                                {
                                    criteria.PassengerMappings.Add(
                                        new RezdyPassengerMapping
                                        {
                                            PassengerLabel = priceOption.Label,
                                            PassengerTypeId = Convert.ToInt32(passengerinfo.PassengerType)
                                        }
                                   );
                                }
                            }

                            var productOptions = _rezdyAdapter.GetAvailability(criteria, serviceCriteria.Token)?.GetAwaiter().GetResult();
                            count++;
                            if (productOptions != null)
                            {
                                var activity = new Activity
                                {
                                    ProductOptions = new List<ProductOption>()
                                };

                                activity.ProductOptions = productOptions;
                                activity.Code = item.HotelBedsActivityCode;
                                //activity.CurrencyIsoCode = item.CurrencyISOCode;
                                activity.CurrencyIsoCode = productOptions?.FirstOrDefault()?.BasePrice?.Currency?.IsoCode;
                                lstActivity.Add(activity);
                            }
                            }
                            catch (Exception ex)
                            {
                                //ignore
                                _log.Error("RezdyCriteriaService|GetAvailability", ex);
                            }
                        }
                    }
                }

                return lstActivity;
            }
            catch (Exception ex)
            {
                _log.Error("RezdyCriteriaService|GetAvailability", ex);
                throw;
            }
        }

        public List<TempHBServiceDetail> GetServiceDetails(List<Activity> activities, List<IsangoHBProductMapping> mappedProducts)
        {
            try
            {
                var serviceDetails = new List<TempHBServiceDetail>();

                //Not added null check in this method as it is already added in the parent method
                foreach (var activity in activities)
                {
                    if (activity == null) continue;
                    var mappedProduct = mappedProducts?.Where(x => x.HotelBedsActivityCode.Equals(activity.Code))?.ToList();

                    if (mappedProduct == null) continue;
                    var serviceMapper = new ServiceMapper();
                    var details = serviceMapper.ProcessServiceDetailsWithBasePrice(activity, mappedProduct);
                    serviceDetails.AddRange(details);
                }

                return serviceDetails;
            }
            catch (Exception ex)
            {
                _log.Error("GrayLineIceLandCriteriaService|GetServiceDetails", ex);
                throw;
            }
        }
    }
}