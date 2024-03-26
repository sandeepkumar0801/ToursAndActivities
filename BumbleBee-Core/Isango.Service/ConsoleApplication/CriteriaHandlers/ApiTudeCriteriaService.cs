using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.HB;
using ServiceAdapters.HB.HB.Entities.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;
using ActivityCalendar = ServiceAdapters.HB.HB.Entities.Calendar.Activity;
using Constant = Isango.Service.Constants.Constant;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class ApiTudeCriteriaService : IApiTudeCriteriaService
    {
        private readonly IHBAdapter _ticketAdapter;
        private readonly IActivityPersistence _activityPersistence;
        private readonly ILogger _log;
        private readonly int _criteriaRecords;
        private readonly int _daystoFecth;
        private readonly string _className;

        public ApiTudeCriteriaService(IHBAdapter ticketAdapter, ILogger logger, IActivityPersistence activityPersistence)

        {
            _ticketAdapter = ticketAdapter;
            _activityPersistence = activityPersistence;
            _log = logger;
            _criteriaRecords = 100;
            _daystoFecth = 30;
            _className = nameof(ApiTudeCriteriaService);
            try
            {
                _criteriaRecords = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.APiTudeCalendarRecordsAtTimeData));
                _daystoFecth = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Days2FetchForApiTudeData));
            }
            catch (Exception ex)
            {
                _log.Error("ApiTudeCriteriaService", ex);
                throw;
            }
        }

        /// <summary>
        /// Get Availabilities
        /// </summary>
        /// <param name="serviceCriteria"></param>
        /// <returns></returns>
        public Tuple<List<Isango.Entities.Activities.Activity>, List<ApiTudeAgeGroup>> GetAvailability(ServiceAvailability.Criteria serviceCriteria)
        {
            try
            {
                var processorCount = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.50) * 1.0));

                if (processorCount <= 0)
                {
                    processorCount = 1;
                }

                var _parallelOption = new System.Threading.Tasks.ParallelOptions
                {
                    MaxDegreeOfParallelism = processorCount
                };

                if (serviceCriteria == null) return null;

                var activities = new List<Isango.Entities.Activities.Activity>();

                var apiTudeAgeGroup = new List<ApiTudeAgeGroup>();

                var criteriaRecords = _criteriaRecords;

                //GroupBy MinAdult
                var groupByMinAdult = serviceCriteria?.MappedProducts.OrderBy(x => x.MinAdultCount).GroupBy(x => x.MinAdultCount);

                foreach (var groupItem in groupByMinAdult)
                {
                    var groupItemList = groupItem?.ToList();
                    var count = groupItemList?.Count ?? 0;

                    //Take n records at a Time in for loop
                    for (int itemCriteria = 0; itemCriteria < count; itemCriteria = (itemCriteria + criteriaRecords))
                    {
                        _log.Info("ApiTudeCriteriaService| GetAvailability|item index: " + itemCriteria);

                        if (groupItemList[itemCriteria] != null && groupItemList?.Count > 0)
                        {
                            //
                            var criteriaRecordsAtTime = groupItemList.Skip(itemCriteria).Take(criteriaRecords);

                            if (criteriaRecordsAtTime != null && criteriaRecordsAtTime.ToList().Count > 0)
                            {
                                var _token = serviceCriteria?.Token;
                                //Create Criteria Request For Calendar //_daystoFecth should not be > 120 else hotelbeds api will fail
                                var daystoFecth = _daystoFecth;
                                var checkinDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                                var checkoutDate = Convert.ToDateTime((DateTime.Now.AddDays(30)).ToString("yyyy-MM-dd"));

                                var counter = Math.Ceiling(Convert.ToDecimal(_daystoFecth / 30));

                                //daystoFecth = 3; //Uncomment line after Testing
                                for (int i = 1; i <= counter; i++)
                                {
                                    HotelbedCriteriaApitudeFilter calendarRq = null;
                                    try
                                    {
                                        calendarRq = GetCalendarCriteriaRequest(criteriaRecordsAtTime?.ToList(), daystoFecth);
                                        calendarRq.CheckinDate = checkinDate;
                                        calendarRq.CheckoutDate = checkoutDate;

                                        //Call Calendar API and Get Response
                                        var tupleResponse = _ticketAdapter.CalendarAsync(calendarRq, _token.ToString()).GetAwaiter().GetResult();

                                        var result = tupleResponse.Item2;

                                        if (Convert.ToInt32(result?.Activities?.Count) == 0) continue;

                                        activities.AddRange(tupleResponse.Item1);

                                        #region Commented

                                        //Age Group Dumping Modality Wise Data
                                        //apiTudeAgeGroup.AddRange(AgeGroupsModalityWise(result)); // may be use in future, comment for now

                                        #endregion Commented

                                        result.Activities.ForEach(apiActivity =>
                                        {
                                            apiActivity.IsangoServiceId = groupItemList?
                                                .Where(m => m.SupplierCode == apiActivity.Code)?
                                                .Select(x => x.IsangoHotelBedsActivityId)?
                                                .FirstOrDefault();
                                        });

                                        var listApiTudeAgeGroup = AgeGroupsActivityWise(result);

                                        if (listApiTudeAgeGroup?.Count > 0)
                                        {
                                            foreach (var apiTudeAge in listApiTudeAgeGroup)
                                            {
                                                if (apiTudeAgeGroup?.Any(x =>
                                                      x.AgeType == apiTudeAge.AgeType &&
                                                      x.ToAge == apiTudeAge.ToAge &&
                                                      x.FromAge == apiTudeAge.FromAge &&
                                                      x.FactsheetID == apiTudeAge.FactsheetID &&
                                                      x.Name == apiTudeAge.Name &&
                                                      x.ServiceCode == apiTudeAge.ServiceCode &&
                                                      x.ServiceID == apiTudeAge.ServiceID
                                                    ) != true
                                                )
                                                {
                                                    apiTudeAgeGroup.Add(apiTudeAge);
                                                }
                                            }
                                        }

                                        var ageGroupsToCheck = apiTudeAgeGroup.GroupBy(x => x.ServiceCode);

                                        if (ageGroupsToCheck?.Any() == true)
                                        {
                                            foreach (var ageGroupByActivityCode in ageGroupsToCheck)
                                            {
                                                var activityCode = ageGroupByActivityCode.Key;
                                                var activityCodeAgeGroups = ageGroupByActivityCode.ToList();
                                                var minAgeInAgeGroupResponse = activityCodeAgeGroups.Min(x => x.FromAge);
                                                bool isinfantCheckNeeded = minAgeInAgeGroupResponse > 0;

                                                if (isinfantCheckNeeded)
                                                {
                                                    var activityFromApi = result.Activities.FirstOrDefault(x => x.Code == activityCode);
                                                    var serviceMapping = serviceCriteria.MappedProducts.FirstOrDefault(x => x.SupplierCode == activityCode);

                                                    var minAgeGroupNode = activityCodeAgeGroups
                                                                          .FirstOrDefault(x =>
                                                                                            x.FromAge == minAgeInAgeGroupResponse
                                                                                            && x.ServiceCode == activityCode
                                                                                        );

                                                    var adultAgeGroupNode = activityCodeAgeGroups
                                                                            .FirstOrDefault(x =>
                                                                                    string.Equals(x.AgeType, "adult", StringComparison.CurrentCultureIgnoreCase)
                                                                                    && x.ServiceCode == activityCode
                                                                                    );

                                                    #region Prepare Request for CheckAvailablity / Activity Detail call

                                                    var activityRq = default(HotelbedCriteriaApitude);
                                                    if (activityFromApi != null && serviceMapping != null)
                                                    {
                                                        activityRq = new HotelbedCriteriaApitude
                                                        {
                                                            ActivityCode = activityFromApi.Code,
                                                            CheckinDate = checkinDate,
                                                            CheckoutDate = checkoutDate,
                                                            Language = "en",
                                                            Ages = new Dictionary<PassengerType, int>(),
                                                            //PassengerAgeGroupIds = new Dictionary<PassengerType, int>(),
                                                            NoOfPassengers = new Dictionary<PassengerType, int>(),
                                                            Destination = activityFromApi?.Country?.Destinations?.FirstOrDefault()?.Code
                                                        };

                                                        #region Add adult customer fro detail call

                                                        if (!activityRq.Ages.ContainsKey(PassengerType.Adult))
                                                            activityRq.Ages.Add(PassengerType.Adult, 30);

                                                        //if (!activityRq.PassengerAgeGroupIds.ContainsKey(PassengerType.Adult))
                                                        //    activityRq.PassengerAgeGroupIds.Add(PassengerType.Adult, 0);

                                                        if (!activityRq.NoOfPassengers.ContainsKey(PassengerType.Adult))
                                                            activityRq.NoOfPassengers.Add(PassengerType.Adult, serviceMapping.MinAdultCount);
                                                        else
                                                            activityRq.NoOfPassengers[PassengerType.Adult]++;

                                                        #endregion Add adult customer fro detail call

                                                        #region Add missing child node

                                                        if (!activityRq.Ages.ContainsKey(PassengerType.Child))
                                                            activityRq.Ages.Add(PassengerType.Child, minAgeInAgeGroupResponse - 1 > 0 ? minAgeInAgeGroupResponse - 1 : 1);

                                                        //if (!activityRq.PassengerAgeGroupIds.ContainsKey(PassengerType.Child))
                                                        //    activityRq.PassengerAgeGroupIds.Add(PassengerType.Child, 0);

                                                        if (!activityRq.NoOfPassengers.ContainsKey(PassengerType.Child))
                                                            activityRq.NoOfPassengers.Add(PassengerType.Child, 1);
                                                        else
                                                            activityRq.NoOfPassengers[PassengerType.Child]++;

                                                        #endregion Add missing child node
                                                    }

                                                    #endregion Prepare Request for CheckAvailablity / Activity Detail call

                                                    var activityAvailability = _ticketAdapter.ActivityDetailsAsync(activityRq, _token.ToString()).GetAwaiter().GetResult();

                                                    if (activityAvailability != null
                                                        && activityAvailability?.ProductOptions?.Any(x => x.AvailabilityStatus == AvailabilityStatus.AVAILABLE) == true
                                                    )
                                                    {
                                                        var newAgeGroup = new ApiTudeAgeGroup
                                                        {
                                                            AgeType = "CHILD",
                                                            FactsheetID = minAgeGroupNode.FactsheetID,
                                                            FromAge = 0,
                                                            ModalityCode = minAgeGroupNode.ModalityCode,
                                                            Name = minAgeGroupNode.Name,
                                                            ServiceCode = activityCode,
                                                            ServiceID = minAgeGroupNode.ServiceID,
                                                            ToAge = minAgeInAgeGroupResponse - 1,
                                                        };
                                                        apiTudeAgeGroup.Add(newAgeGroup);
                                                    }
                                                }
                                            }
                                        }

                                        checkinDate = checkoutDate.AddDays(1);
                                        checkoutDate = checkinDate.AddDays(30);
                                    }
                                    catch (Exception ex)
                                    {
                                        var logdetails = SerializeDeSerializeHelper.Serialize(calendarRq);

                                        Task.Run(() =>
                                        _log.Error(
                                            new IsangoErrorEntity
                                            {
                                                ClassName = _className,
                                                MethodName = nameof(GetAvailability),
                                                Params = logdetails,
                                                Token = _token
                                            }, ex
                                            )
                                        );
                                        //throw;
                                    }
                                }

                                #region Removed detail call from calendar call as calender is sufficient to load day wise prices

                                /*--Removed detail call from calendar call as calender is sufficient to load day wise prices
                                //Loop through all activities get from Calendar API
                                foreach (var itemActivity in result?.Activities)
                                {
                                    if (itemActivity != null)
                                    {
                                        //Get DateRange for a Particular Activity (Calendar API)
                                        var distinctOperationalDate = GetCalendarDateRange(itemActivity);

                                        if (distinctOperationalDate != null && distinctOperationalDate.Count > 0)
                                        {
                                            distinctOperationalDate = distinctOperationalDate.Where(x => x != null).ToList();

                                            // Loop through all distinct date ranges and Call Get Availability
                                            Parallel.ForEach(distinctOperationalDate, _parallelOption, item =>
                                             {
                                                 try
                                                 {
                                                     //Create Criteria Request For Availability
                                                     var language = "en";
                                                     var mappedItem = groupItemList[itemCriteria];

                                                     var activityDetailReq = GetActivityCriteriaRequest(language, item, itemActivity.Code, mappedItem);

                                                     var activityAvailability = new Isango.Entities.Activities.Activity();

                                                     activityAvailability = _ticketAdapter.ActivityDetailsAsync(activityDetailReq, _token.ToString()).GetAwaiter().GetResult();

                                                     if (activityAvailability != null)
                                                     {
                                                         activities.Add(activityAvailability);
                                                     }
                                                 }
                                                 catch (Exception ex)
                                                 {
                                                     _log.Error("ApiTudeCriteriaService|GetAvailability|Parallel.ForEach(distinctOperationalDate", ex);
                                                 }
                                             });
                                        }
                                    }
                                }

                                */

                                #endregion Removed detail call from calendar call as calender is sufficient to load day wise prices
                            }
                        }
                    }
                }
                return Tuple.Create(activities, apiTudeAgeGroup);
            }
            catch (Exception ex)
            {
                _log.Error("ApiTudeCriteriaService|GetAvailability", ex);
                throw;
            }
        }

        /// <summary>
        /// Get service details
        /// </summary>
        /// <param name="activities"></param>
        /// <param name="mappedProducts"></param>
        /// <returns></returns>
        public List<ServiceAvailability.TempHBServiceDetail> GetServiceDetails(List<Isango.Entities.Activities.Activity> activities, List<IsangoHBProductMapping> mappedProducts)
        {
            try
            {
                var serviceDetails = new List<ServiceAvailability.TempHBServiceDetail>();

                if (activities != null && activities.Count > 0)
                {
                    foreach (var activity in activities)
                    {
                        if (activity == null) continue;
                        var mappedProduct = mappedProducts?.FirstOrDefault(x => x.SupplierCode.Equals(activity.Code));

                        if (mappedProduct == null) continue;
                        var serviceMapper = new ServiceMapper();
                        var details = serviceMapper.ProcessServiceDetailsWithCostPrice(activity, mappedProduct);
                        serviceDetails.AddRange(details);
                    }
                }

                return serviceDetails;
            }
            catch (Exception ex)
            {
                _log.Error("ApiTudeCriteriaService|GetServiceDetails", ex);
                throw;
            }
        }

        /// <summary>
        /// Get Distinct Operational Dates: Get Calendar Date Range
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>

        private List<Operationdate> GetCalendarDateRange(ActivityCalendar activity)
        {
            try
            {
                //Gives us final filtered dates
                var finalOperationalDates = new List<Operationdate>();
                //Gives us filtered dates
                var filterOperationalDates = new List<Operationdate>();
                if (activity != null && activity?.Modalities != null && activity?.Modalities?.Count > 0)
                {
                    foreach (var itemModality in activity?.Modalities)
                    {
                        if (itemModality != null && itemModality?.Rates != null && itemModality?.Rates?.Count > 0)
                        {
                            foreach (var itemRate in itemModality?.Rates)
                            {
                                if (itemRate != null && itemRate?.RateDetails != null && itemRate?.RateDetails?.Count > 0)
                                {
                                    foreach (var itemDetail in itemRate?.RateDetails)
                                    {
                                        if (itemDetail != null && itemDetail?.OperationDates != null && itemDetail?.OperationDates?.Count > 0)
                                        {
                                            foreach (var itemOperationalDetail in itemDetail?.OperationDates)
                                            {
                                                bool alreadyExists = filterOperationalDates.Any(x => x.From == itemOperationalDetail.From && x.To == itemOperationalDetail.To);
                                                if (!alreadyExists)
                                                {
                                                    filterOperationalDates.Add(itemOperationalDetail);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //Below checks are used because of below scenerios
                //(Case 1: 'fromdate =2019-09-08', 'todate =2019-09-08')
                //(Case2:'fromdate =2019-09-08', 'todate=2019-09-09')
                if (filterOperationalDates != null)
                {
                    foreach (var item in filterOperationalDates)
                    {
                        if (item.From == item.To)
                        {
                            bool alreadyExists = finalOperationalDates.Any(x => x.From == item.From && x.To == item.To);
                            if (!alreadyExists)
                            {
                                finalOperationalDates.Add(item);
                            }
                        }
                        else
                        {
                            while (item.From != item.To)
                            {
                                var operationdate = new Operationdate
                                {
                                    From = item.From,
                                    To = item.From
                                };
                                bool alreadyExists = finalOperationalDates.Any(x => x.From == item.From && x.To == item.From);
                                if (!alreadyExists)
                                {
                                    finalOperationalDates.Add(operationdate);
                                }
                                item.From = (Convert.ToDateTime(item.From).AddDays(1).Date).ToString("yyyy-MM-dd");
                            }
                        }
                    }
                }
                return finalOperationalDates;
            }
            catch (Exception ex)
            {
                _log.Error("ApiTudeCriteriaService|GetCalendarDateRange", ex);
                return new List<Operationdate>();
            }
        }

        /// <summary>
        /// /// Calendar Filter Request: Get Calendar Criteria Request
        /// </summary>
        /// <param name="itemCriteria"></param>
        /// <returns></returns>

        private HotelbedCriteriaApitudeFilter GetCalendarCriteriaRequest(List<IsangoHBProductMapping> itemCriteria, int daysToFetch)
        {
            try
            {
                var filters = new List<Filters>();
                var filter = new Filters();
                var activitiesIds = string.Empty;
                var searchFilterItems = new List<SearchFilterItems>();
                var passengerInfos = default(List<Entities.Booking.PassengerInfo>);
                var isangoActivitiesIds = itemCriteria?.Select(x => x.IsangoHotelBedsActivityId)?.Distinct()?.ToArray();

                if (isangoActivitiesIds.Length > 0)
                {
                    activitiesIds = string.Join(",", isangoActivitiesIds);
                    passengerInfos = _activityPersistence.GetPassengerInfoDetails(activitiesIds);
                    //if (passengerInfos?.Any() == true)
                    //{
                    //    var filteredActivitiesHavingAgeGroupQuery =
                    //              from ag in passengerInfos
                    //              from pm in itemCriteria
                    //              where ag.ActivityId == pm.IsangoHotelBedsActivityId
                    //              select pm;

                    //    itemCriteria = filteredActivitiesHavingAgeGroupQuery.Distinct().ToList();
                    //}
                }
                foreach (var item in itemCriteria)
                {
                    var searchItems = new SearchFilterItems
                    {
                        Type = "service",
                        Value = item.SupplierCode
                    };
                    searchFilterItems.Add(searchItems);
                }

                filter.SearchFilterItems = searchFilterItems;
                filters.Add(filter);

                var activityRq = new HotelbedCriteriaApitudeFilter
                {
                    Filters = filters,
                    CheckinDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),
                    CheckoutDate = Convert.ToDateTime((DateTime.Now.AddDays(daysToFetch)).ToString("yyyy-MM-dd")),
                    Language = "en",
                    Ages = new Dictionary<PassengerType, int>(),
                    //PassengerAgeGroupIds = new Dictionary<PassengerType, int>(),
                    NoOfPassengers = new Dictionary<PassengerType, int>(),
                    ProductMapping = itemCriteria,
                    PassengerInfo = passengerInfos
                };

                activityRq.NoOfPassengers.Add(PassengerType.Adult, itemCriteria[0].MinAdultCount);
                activityRq.Ages.Add(PassengerType.Adult, 30);

                //activityRq.NoOfPassengers.Add(PassengerType.Child, itemCriteria[0].MinAdultCount);
                //activityRq.Ages.Add(PassengerType.Child, 0);

                return activityRq;
            }
            catch (Exception ex)
            {
                _log.Error("ApiTudeCriteriaService|GetCalendarCriteriaRequest", ex);
                throw;
            }
        }

        /// <summary>
        /// Activity Detail Criteria Request: Get Activity Criteria Request
        /// </summary>
        /// <param name="language"></param>
        /// <param name="operationdate"></param>
        /// <param name="supplierCode"></param>
        /// <returns></returns>

        private HotelbedCriteriaApitude GetActivityCriteriaRequest(string language, Operationdate operationdate, string supplierCode, IsangoHBProductMapping productMapping)
        {
            try
            {
                var minAdult = productMapping?.MinAdultCount ?? 1;
                var destination_Code = productMapping?.HotelBedsActivityCode?.Split('~')?.LastOrDefault();
                var activityRq = new HotelbedCriteriaApitude
                {
                    ActivityCode = supplierCode,
                    CheckinDate = Convert.ToDateTime(Convert.ToDateTime(operationdate.From).ToString("yyyy-MM-dd")),
                    CheckoutDate = Convert.ToDateTime(Convert.ToDateTime(operationdate.To).ToString("yyyy-MM-dd")),
                    Language = language,
                    Ages = new Dictionary<PassengerType, int>(),
                    //PassengerAgeGroupIds = new Dictionary<PassengerType, int>(),
                    NoOfPassengers = new Dictionary<PassengerType, int>(),

                    Destination = destination_Code
                };
                activityRq.NoOfPassengers.Add(PassengerType.Adult, minAdult);
                activityRq.Ages.Add(PassengerType.Adult, 30);
                return activityRq;
            }
            catch (Exception ex)
            {
                _log.Error("ApiTudeCriteriaService|GetActivityCriteriaRequest", ex);
                throw;
            }
        }

        /// <summary>
        /// Age Group Modality Wise Dumping
        /// </summary>
        /// <param name="calendarResponse"></param>
        /// <returns></returns>
        private List<ApiTudeAgeGroup> AgeGroupsModalityWise(CalendarRs calendarResponse)
        {
            var apiTudeAgeGroup = new List<ApiTudeAgeGroup>();

            try
            {
                if (calendarResponse != null && calendarResponse?.Activities != null)
                {
                    foreach (var itemActivity in calendarResponse?.Activities)
                    {
                        if (itemActivity != null && itemActivity?.Modalities != null && itemActivity?.Modalities?.Count > 0)
                        {
                            foreach (var itemModality in itemActivity?.Modalities)
                            {
                                if (itemModality != null && itemModality?.Rates != null && itemModality?.Rates?.Count > 0)
                                {
                                    foreach (var itemRates in itemModality?.Rates)
                                    {
                                        if (itemRates != null && itemRates?.RateDetails != null && itemRates?.RateDetails?.Count > 0)
                                        {
                                            foreach (var itemDetail in itemRates?.RateDetails)
                                            {
                                                if (itemDetail != null && itemDetail?.PaxAmounts != null && itemDetail?.PaxAmounts?.Count > 0)
                                                {
                                                    foreach (var itemPax in itemDetail?.PaxAmounts)
                                                    {
                                                        if (itemPax != null)
                                                        {
                                                            var apiTudeAge = new ApiTudeAgeGroup
                                                            {
                                                                FromAge = itemPax.AgeFrom,
                                                                ToAge = itemPax.AgeTo,
                                                                AgeType = itemPax?.PaxType,
                                                                FactsheetID = itemActivity?.Content?.ContentId,
                                                                ModalityCode = itemModality?.Code,
                                                                Name = itemModality?.Name,
                                                                ServiceCode = itemActivity?.Code,
                                                                ServiceID = Convert.ToInt32(itemActivity?.IsangoServiceId)
                                                            };

                                                            apiTudeAgeGroup.Add(apiTudeAge);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return apiTudeAgeGroup;
            }
            catch (Exception ex)
            {
                _log.Error("ApiTudeCriteriaService|AgeGroupsModalityWise", ex);
                return new List<ApiTudeAgeGroup>();
            }
        }

        /// <summary>
        /// Age Group Activity Wise Dumping
        /// </summary>
        /// <param name="calendarResponse"></param>
        /// <returns></returns>
        private List<ApiTudeAgeGroup> AgeGroupsActivityWise(CalendarRs calendarResponse)
        {
            var apiTudeAgeGroup = new List<ApiTudeAgeGroup>();
            try
            {
                if (calendarResponse != null && calendarResponse?.Activities != null && calendarResponse?.Activities?.Count > 0)
                {
                    foreach (var itemActivity in calendarResponse?.Activities)
                    {
                        if (itemActivity != null && itemActivity?.AmountsFrom != null && itemActivity?.AmountsFrom?.Count > 0)
                        {
                            foreach (var itemPax in itemActivity?.AmountsFrom)
                            {
                                try
                                {
                                    if (itemPax != null)
                                    {
                                        var apiTudeAge = new ApiTudeAgeGroup
                                        {
                                            FromAge = itemPax.AgeFrom,
                                            ToAge = itemPax.AgeTo,
                                            AgeType = itemPax?.PaxType,
                                            FactsheetID = itemActivity?.Content?.ContentId,
                                            ModalityCode = string.Empty,
                                            Name = itemActivity?.Name,
                                            ServiceCode = itemActivity?.Code,
                                            ServiceID = Convert.ToInt32(itemActivity?.IsangoServiceId)
                                        };

                                        if (apiTudeAgeGroup?.Any(x =>
                                           x.AgeType == apiTudeAge.AgeType &&
                                           x.ToAge == apiTudeAge.ToAge &&
                                           x.FromAge == apiTudeAge.FromAge &&
                                           x.FactsheetID == apiTudeAge.FactsheetID &&
                                           x.Name == apiTudeAge.Name &&
                                           x.ServiceCode == apiTudeAge.ServiceCode &&
                                           x.ServiceID == apiTudeAge.ServiceID
                                            ) != true
                                        )
                                        {
                                            apiTudeAgeGroup.Add(apiTudeAge);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //ignored
                                    //throw;
                                }
                            }
                        }
                    }
                }
                return apiTudeAgeGroup;
            }
            catch (Exception ex)
            {
                _log.Error("ApiTudeCriteriaService|AgeGroupsActivityWise", ex);
                return new List<ApiTudeAgeGroup>();
            }
        }
    }
}