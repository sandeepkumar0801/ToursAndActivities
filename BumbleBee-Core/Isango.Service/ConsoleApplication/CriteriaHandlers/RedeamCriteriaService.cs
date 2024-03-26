using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Redeam;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Redeam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class RedeamCriteriaService : IRedeamCriteriaService
    {
        private readonly IRedeamAdapter _redeamAdapter;
        private readonly ILogger _log;

        public RedeamCriteriaService(IRedeamAdapter redeamAdapter, ILogger log)
        {
            _redeamAdapter = redeamAdapter;
            _log = log;
        }

        /// <summary>
        /// Get Availability
        /// </summary>
        /// <returns></returns>
        public List<Activity> GetAvailability(ServiceAvailability.Criteria criteria)
        {
            try
            {
                var counter = criteria?.Counter;
                if (counter <= 0) return null;
                var tokenId = Guid.NewGuid().ToString();
                var activities = new List<Activity>();
                for (var i = 1; i <= counter; i++)
                {
                    var dateTimeNow = DateTime.Now;
                    var daysToFetch = criteria.Days2Fetch;

                    for (var start = dateTimeNow.AddDays(daysToFetch * (i - 1)).Date; start.Date < dateTimeNow.AddDays(daysToFetch * i).Date; start = start.AddDays(daysToFetch).Date)
                    {
                        var mappedProducts = criteria.MappedProducts;
                        foreach (var item in mappedProducts)
                        {
                            try
                            {
                                var activity = GetRedeamActivities(item, criteria, start, tokenId);
                                if (activity == null) continue;
                                activities.Add(activity);
                            }
                            catch (Exception ex)
                            {
                                _log.Error($"RedeamCriteriaService|GetAvailability IsangoServiceId{item.IsangoHotelBedsActivityId}, APIServiceId {item.HotelBedsActivityCode}", ex);
                                // ignored // failing one item should not fail entire dumping.
                            }
                        }
                    }
                }

                return activities;
            }
            catch (Exception ex)
            {
                _log.Error("RedeamCriteriaService|GetAvailability", ex);
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
                foreach (var activity in activities)
                {
                    if (activity == null) continue;

                    var mappedProduct = mappedProducts
                                        .FirstOrDefault(x => x.IsangoHotelBedsActivityId.Equals(activity.ID)
                                                        && activity.Code == x.HotelBedsActivityCode
                                        );

                    if (mappedProduct == null) continue;
                    var serviceMapper = new ServiceMapper();
                    var details = serviceMapper.ProcessServiceDetailsWithCostPrice(activity, mappedProduct);
                    if (details != null)
                        serviceDetails.AddRange(details);
                }

                return serviceDetails;
            }
            catch (Exception ex)
            {
                _log.Error("RedeamCriteriaService|GetServiceDetails", ex);
                throw;
            }
        }

        #region "Private Methods"

        private Activity GetRedeamActivities(IsangoHBProductMapping item, ServiceAvailability.Criteria criteria, DateTime startDate, string tokenId = null)
        {
            var splitResult = item?.HotelBedsActivityCode?.Split('#');

            if (splitResult?.Length != 2)
            {
                var message = "Incorrect mapping SupplierOptionCode should be in  'ProductId#RateId' format";
                //var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                //{
                //    ReasonPhrase = message
                //};
                //throw new HttpResponseException(data);
                throw new WebApiException(message, HttpStatusCode.NotFound);

            }

            var supplierId = item.SupplierCode;

            if (string.IsNullOrWhiteSpace(tokenId))
            {
                tokenId = Guid.NewGuid().ToString();
            }
            var redeamCriteria = new RedeamCriteria
            {
                RateIds = new List<string>
                {
                    item.HotelBedsActivityCode
                },
                SupplierId = supplierId,
                CheckinDate = startDate,
                CheckoutDate = startDate.AddDays(criteria.Days2Fetch),
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    { PassengerType.Adult, 1 }
                },
                RateIdAndType = new Dictionary<string, string>
                {
                    { item.HotelBedsActivityCode, item.PrefixServiceCode }
                },
                ServiceOptionId = item.ServiceOptionInServiceid
            };

            //Get price and availability from supplier api
            var productOptions = _redeamAdapter.GetAvailabilities(redeamCriteria, tokenId).Result;
            if (!(productOptions?.Count > 0)) return null;

            // Passing OptionId and margin here as we are getting only one option from the adapter.
            productOptions.ForEach(option =>
            {
                option.Id = item.ServiceOptionInServiceid;
                option.Margin = new Margin
                {
                    Value = item.MarginAmount,
                    IsPercentage = true
                };
            });
            var activity = new Activity
            {
                ID = item.IsangoHotelBedsActivityId,
                Code = item.HotelBedsActivityCode,
                FactsheetId = item.FactSheetId,
                ApiType = APIType.Redeam,
                CurrencyIsoCode = item.CurrencyISOCode,
                ProductOptions = productOptions //filteredOptions.Cast<ProductOption>().ToList()
            };
            return activity;
        }

        #endregion "Private Methods"
    }
}