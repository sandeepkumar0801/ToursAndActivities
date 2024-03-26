using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Ticket;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.HotelBeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Util;

namespace Isango.Service.SupplierServices
{
    public class HotelBedsService : SupplierServiceBase, ISupplierService
    {
        private ITicketAdapter _ticketAdapter;
        private readonly IMemCache _memCache;
        private readonly ILogger _log;
        public HotelBedsService(ITicketAdapter ticketAdapter, IMemCache memCache, ILogger log = null)
        {
            _ticketAdapter = ticketAdapter;
            _memCache = memCache;
            _log = log;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var hbActivities = _ticketAdapter.GetTicketAvailability((TicketCriteria)criteria, Constant.GetTicketAuthString, token);
            if (hbActivities?.Count > 0)
            {
                if (hbActivities.FirstOrDefault().ProductOptions == null)
                {
                    var message = Constant.APIActivityOptionsNot + Constant.HotelBedsAPI + " .Id:" + activity.ID;
                    SendException(activity.ID, message);
                }
                return MapActivity(activity, hbActivities, criteria);
            }
            else
            {
                var message = Constant.APIActivityNot + Constant.HotelBedsAPI + " .Id:" + activity.ID;
                SendException(activity.ID, message);
            }
            return activity;
        }

        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var regionId = activity.Regions.Find(city => city.Type.Equals(RegionType.City)).Id;
            var destinationCode = _memCache.GetRegionDestinationMapping().Find(mapping => mapping.RegionId == regionId).DestinationCode;

            var ticketCriteria = new TicketCriteria
            {
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                FactSheetIds = new List<int> { activity.FactsheetId },
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,//.AddDays(Constant.AddSixDays),

                Language = _memCache.GetMappedLanguage().Find(x => x.IsangoLanguageCode.Equals(clientInfo?.LanguageCode, StringComparison.InvariantCultureIgnoreCase))
                    ?.SupplierLanguageCode?.ToUpperInvariant(),

                Destination = destinationCode,
                PassengerInfo = criteria.PassengerInfo
                , Token = criteria.Token
            };

            return ticketCriteria;
        }

        public Activity MapActivity(Activity activity, List<Activity> activitiesFromApi, Criteria criteria)
        {
            if (activity?.Regions != null && activity.ProductOptions != null && activitiesFromApi?.Count > 0)
            {
                var activities = activitiesFromApi.FindAll(x =>
                    x.FactsheetId.Equals(activity.FactsheetId) && !string.IsNullOrWhiteSpace(x.RegionName));
                if (!activities.Any()) return null;

                var hbActivity = activities.FirstOrDefault();
                if (hbActivity != null)
                {
                    activity.CancellationPolicy = hbActivity.CancellationPolicy;

                    var productOptions = new List<ProductOption>();
                    var activityProductOption = activity.ProductOptions;
                    foreach (var item in activityProductOption)
                    {
                        if (string.IsNullOrWhiteSpace(item.SupplierOptionCode)) continue;
                        var details = item.SupplierOptionCode.Trim()
                            .Split(new[] { "~#~" }, StringSplitOptions.RemoveEmptyEntries);
                        var code = details[0];
                        int index = 2;
                        var destinationCode = (index <= details.Length) ? details[2] : string.Empty;
                        var filteredActivity =
                            activities.FirstOrDefault(p => p.Code.Equals(code) && p.RegionName.Equals(destinationCode));
                        var prodOptions = filteredActivity?.ProductOptions;

                        if (prodOptions?.Count > 0)
                        {
                            productOptions.AddRange(prodOptions
                                .Select(p =>
                                {
                                    ((ActivityOption)p).SupplierOptionCode = item.SupplierOptionCode;
                                    p.Id = Math.Abs(Guid.NewGuid().GetHashCode());
                                    p.ServiceOptionId = item.Id;
                                    p.Margin = item.Margin;
                                    return p;
                                }).ToList());
                        }
                    }

                    if (productOptions.Any())
                    {
                        activity.ProductOptions =
                            UpdateBasePrices(productOptions, activity.HotelPickUpLocation, criteria);

                        activity.Code = hbActivity.Code;
                    }
                }
            }

            return activity;
        }

        private IsangoErrorEntity SendException(Int32 activityId, string message)
        {
            var isangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = "HotelBedsService",
                MethodName = "GetAvailability",
                Params = $"{activityId}"
            };
            _log.Error(isangoErrorEntity, new Exception(message));
            var data = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                ReasonPhrase = message
            };
            throw new HttpResponseException(data);
        }
    }
}