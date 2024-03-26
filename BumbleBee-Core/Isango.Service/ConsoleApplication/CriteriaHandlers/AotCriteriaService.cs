using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Aot;
using Isango.Entities.Enums;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Aot;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Util;
using Activity = Isango.Entities.Activities.Activity;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class AotCriteriaService : IAotCriteriaService
    {
        private readonly IAotAdapter _aotAdapter;
        private readonly ILogger _log;

        public AotCriteriaService(IAotAdapter aotAdapter, ILogger logger)
        {
            _aotAdapter = aotAdapter;
            _log = logger;
        }

        /// <summary>
        /// Get Availability
        /// </summary>
        /// <returns></returns>
        public List<Activity> GetAvailability(ServiceAvailability.Criteria criteria)
        {
            try
            {
                var watch4 = Stopwatch.StartNew();
                var mappedProduct = criteria?.MappedProducts;
                _log.WriteTimer("GetAvailability", $"DataDumping_MappedProducts_Count:{mappedProduct.Count}", APIType.Aot.ToString(), watch4.Elapsed.ToString());
                if (mappedProduct == null)
                {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
                    return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
                }

                var activities = new List<Activity>();

                foreach (var product in mappedProduct)
                {
                    SetAgentIdPassword(product.CountryId);
                    var ausDateTime = DateTime.Now.Date;
                    const int daysToFetch = 30;
                    //var fetchLoop = criteria.Days2Fetch / daysToFetch;//i.e 90/30=3
                    var fetchLoop = criteria.Counter;//i.e 90/30=3
                    for (var i = 0; i < fetchLoop; i++) //loop run for 3 months
                    {
                        if (i > 0)
                        {
                            ausDateTime = ausDateTime.AddDays(daysToFetch + 1);
                        }
                        var aotCriteria = new AotCriteria
                        {
                            ActivityId = product.IsangoHotelBedsActivityId,
                            CheckinDate = ausDateTime,
                            CheckoutDate = ausDateTime.AddDays(daysToFetch),
                            NoOfPassengers = new Dictionary<PassengerType, int>
                            {{PassengerType.Adult, product.MinAdultCount}, {PassengerType.Child, product.MinAdultCount}, {PassengerType.Infant, product.MinAdultCount}},
                            OptCode = new List<string>()
                        };
                        if (string.IsNullOrEmpty(product.HotelBedsActivityCode)) continue;
                        aotCriteria.OptCode.Add(product.HotelBedsActivityCode);
                        var activityList = _aotAdapter.GetPricingAvailabilityForDumping(aotCriteria, criteria.Token) as List<Activity>;
                        if (activityList == null || activityList.Count <= 0) continue;

                        foreach (var activity in activityList)
                        {
                            activity.ProductOptions.ForEach(y => y.Id = product.ServiceOptionInServiceid);
                            activity.PriceTypeId = (PriceTypeId)product.PriceTypeId;
                            activities.Add(activity);
                        }
                    }
                }

                watch4.Stop();
                _log.WriteTimer("GetAotAvailabilities", $"DataDumping_Availabilities_Count:{activities.Count}", APIType.Aot.ToString(), watch4.Elapsed.ToString());
                return activities;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AOTCriteriaService",
                    MethodName = "GetAvailability",
                    Token = criteria.Token
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
                    var mappedProductsById = mappedProducts.Where(x => x.IsangoHotelBedsActivityId.Equals(activity.ID)).ToList();
                    if (mappedProductsById?.Count < 0) continue;
                    var serviceMapper = new ServiceMapper();
                    var details = serviceMapper.ProcessAotServiceDetail(activity, mappedProductsById);
                    serviceDetails.AddRange(details);
                }

                return serviceDetails;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AOTCriteriaService",
                    MethodName = "GetServiceDetails"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get Cancellaion Policies of passed options
        /// </summary>
        /// <param name="aotProducts"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<GoogleCancellationPolicy> GetCancellationPolicies(List<IsangoHBProductMapping> aotProducts, string token)
        {
            var countryTypeAustralia = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CountryTypeAustralia));
            var countryTypeNewZealand = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CountryTypeNewZealand));

            var cancellationPolicies = new List<GoogleCancellationPolicy>();
            foreach (var product in aotProducts)
            {
                var opts = new Opts { Opt = new List<string> { product.HotelBedsActivityCode } };

                //Find country from region Id
                var countryType = (product.CountryId == countryTypeAustralia) ? CountryType.Australia : product.CountryId == countryTypeNewZealand ? CountryType.NewZealand : CountryType.Fiji;

                //Set AgentId and Password through a country type
                _aotAdapter.SetAgentIdPassword(countryType);
                var criteria = new AotCriteria
                {
                    OptCode = opts.Opt,
                    NoOfPassengers = new Dictionary<PassengerType, int>
                    {
                        { PassengerType.Adult, 1}
                    },
                    CheckinDate = DateTime.Now.AddDays(1),
                    CheckoutDate = DateTime.Now.AddDays(2),
                    CancellationPolicy = true
                };

                var response = _aotAdapter.GetDetailedPricingAvailability(criteria, token, false);
                if (response == null) continue;

                var result = response as Dictionary<DateTime, OptionStayPricingResponse>;
                var optCode = criteria.OptCode.FirstOrDefault();
                var optStayResult =
                result?.FirstOrDefault().Value.OptStayResults?.FirstOrDefault(x => x.Opt.Equals(optCode));
                var cancellationHours = optStayResult?.CancelHours;

                var cancellation = new GoogleCancellationPolicy
                {
                    ActivityId = product.IsangoHotelBedsActivityId,
                    OptionId = product.ServiceOptionInServiceid,
                    CancellationPrices = new List<GoogleCancellationPrice>
                        {
                            new GoogleCancellationPrice
                            {
                                CancellationCharge = 100,
                                CutoffHours = cancellationHours
                            }
                        }
                };
                cancellationPolicies.Add(cancellation);
            }
            return cancellationPolicies;
        }

        #region Private method

        private void SetAgentIdPassword(int countryId)
        {
            var countryIdAustralia = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CountryTypeAustralia));
            var countryIdNewZealand = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CountryTypeNewZealand));
            var countryIdFiji = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CountryTypeFiji));

            if (countryId == countryIdAustralia)
            {
                _aotAdapter.SetAgentIdPassword(CountryType.Australia);
            }
            else if (countryId == countryIdNewZealand)
            {
                _aotAdapter.SetAgentIdPassword(CountryType.NewZealand);
            }
            else if (countryId == countryIdFiji)
            {
                _aotAdapter.SetAgentIdPassword(CountryType.Fiji);
            }
        }

        #endregion Private method
    }
}