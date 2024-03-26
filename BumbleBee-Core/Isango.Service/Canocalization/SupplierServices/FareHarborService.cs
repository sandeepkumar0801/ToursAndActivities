using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.FareHarbor;
using Isango.Persistence.Contract;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.FareHarbor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Util;

namespace Isango.Service.SupplierServices
{
    public class FareHarborService : SupplierServiceBase, ISupplierService
    {
        private readonly IFareHarborAdapter _fareHarborAdapter;
        private readonly IFareHarborUserKeysCacheManager _fareHarborUserKeysCacheManager;
        private readonly IFareHarborCustomerPrototypesCacheManager _fareHarborCustomerPrototypesCacheManager;
        private readonly IMasterPersistence _masterPersistence;
        private readonly ILogger _log;

        public FareHarborService(IFareHarborAdapter fareharborAdapter
            , IFareHarborUserKeysCacheManager fareHarborUserKeysCacheManager
            , IFareHarborCustomerPrototypesCacheManager fareHarborCustomerPrototypesCacheManager
            , IMasterPersistence masterPersistence, ILogger log = null
        )
        {
            _fareHarborAdapter = fareharborAdapter;
            _fareHarborUserKeysCacheManager = fareHarborUserKeysCacheManager;
            _fareHarborCustomerPrototypesCacheManager = fareHarborCustomerPrototypesCacheManager;
            _masterPersistence = masterPersistence;
            _log = log;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var fareHarborCriteria = (FareHarborCriteria)criteria;
            var activities = _fareHarborAdapter.GetAvailabilities(fareHarborCriteria, token);
            if (activities?.Count > 0)
            {
                if (activities.FirstOrDefault().ProductOptions == null)
                {
                    var message = Constant.APIActivityOptionsNot + Constant.FareHarbourAPI + " .Id:" + activity.ID;
                    SendException(activity.ID, message);
                }

                return MapActivity(activity, activities, fareHarborCriteria);
            }
            else
            {
                var message = Constant.APIActivityNot + Constant.FareHarbourAPI + " .Id:" + activity.ID;
                SendException(activity.ID, message);
            }
            return activity;
        }

        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var userKey = _fareHarborUserKeysCacheManager.GetFareHarborUserKeys()?
                               .Where(x => x.CompanyShortName.Trim()
                                   .Equals(activity.Code, StringComparison.InvariantCultureIgnoreCase))?
                               .Select(s => s.UserKey).First();

            var customerPrototypeMapping = _fareHarborCustomerPrototypesCacheManager?
                        .GetCustomerPrototypeList().Where(x => x.ServiceId == activity.ID);

            var multiplePaxMappingOfSameType = customerPrototypeMapping.GroupBy(x =>
                                                        new CustomerPrototype
                                                        {
                                                            ServiceId = x.ServiceId,
                                                            ServiceOptionId = x.ServiceOptionId,
                                                            PassengerType = x.PassengerType
                                                        }
                                                    ).FirstOrDefault(y => y.Count() > 1);
            if (multiplePaxMappingOfSameType?.Any() == true)
            {
                var mp = multiplePaxMappingOfSameType.FirstOrDefault();
                var message = $"Multiple passengerType mappings found for same option {mp.ServiceOptionId}";
                var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    ReasonPhrase = message
                };
                throw new HttpResponseException(data);
            }
            if (customerPrototypeMapping?.Any() == false)
            {
                var allFHbCustomerProtoTypes = _masterPersistence.GetCustomerPrototypeByActivity();
                customerPrototypeMapping = allFHbCustomerProtoTypes?
                    .Where(x => x.ServiceId == activity.ID);
            }

            if (customerPrototypeMapping?.Any() == true)
            {
                var paxCountBasedMappingQuery = from mp in customerPrototypeMapping
                                                from pax in criteria.NoOfPassengers
                                                where mp.PassengerType == pax.Key
                                                && pax.Value >= mp.PassengersInUnitMinimum
                                                && pax.Value <= mp.PassengersInUnitMaximum
                                                select mp;

                if (paxCountBasedMappingQuery?.Any() == true)
                {
                    customerPrototypeMapping = paxCountBasedMappingQuery;
                }
            }

            var fhbCriteria = new FareHarborCriteria
            {
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,
                NoOfPassengers = criteria.NoOfPassengers,
                UserKey = userKey,
                Ages = criteria.Ages,
                ActivityCode = activity.FactsheetId.ToString(),
                CompanyName = activity.Code,
                CustomerPrototypes = customerPrototypeMapping?.ToList()
                ,
                Token = criteria.Token
            };
            return fhbCriteria;
        }

        public Activity MapActivity(Activity activity, List<Activity> activitesFromAPI, Criteria criteria)
        {
            var fhbcriteria = (FareHarborCriteria)criteria;
            if (activity?.ProductOptions != null && activity.ProductOptions.Count > 0)
            {
                var apiOptions = activitesFromAPI.FirstOrDefault().ProductOptions.ConvertAll(instance => (ActivityOption)instance);
                var mappedProductOption = new List<ProductOption>();
                foreach (var productOptionDatabase in activity.ProductOptions)
                {
                    var optionMapping = fhbcriteria.CustomerPrototypes.Where(x => x.ServiceOptionId.Equals(productOptionDatabase.Id)).ToList();
                    var optionPrototypes = optionMapping.Select(x => x.CustomerPrototypeId).ToArray();
                    var actOptions = apiOptions.Where(x => optionPrototypes.Contains(Convert.ToInt32(x.Code))).ToList();

                    var option = actOptions.FirstOrDefault();
                    if (option == null) continue;

                    //API Activity Group by time
                    var apiOptionByTime = actOptions?.GroupBy(x => x.BasePrice.DatePriceAndAvailabilty.FirstOrDefault().Key.TimeOfDay).OrderBy(x => x.Key);
                    //API Activities Group by  different Time
                    foreach (var itemAPIProductOptionsByTime in apiOptionByTime) // if activity occurs multiple times a day. eg: (10 am and 4 pm)
                    {
                        var addOption = true;
                        var mappedActivityOption = MapActivityOption(itemAPIProductOptionsByTime.FirstOrDefault(), productOptionDatabase, criteria);

                        mappedActivityOption.UserKey = fhbcriteria.UserKey;
                        mappedActivityOption.SupplierName = activity.ShortName;
                        mappedActivityOption.Id = Math.Abs(Guid.NewGuid().GetHashCode()); //Create unique identifier
                        mappedActivityOption.OptionKey = productOptionDatabase.Id.ToString(); //Database Service Option ID
                        mappedActivityOption.AvailToken = itemAPIProductOptionsByTime?.FirstOrDefault().Id.ToString(); //API Primary Key Service Option ID

                        var getSelectedDateTime = itemAPIProductOptionsByTime.Select(z => z.BasePrice.DatePriceAndAvailabilty.FirstOrDefault().Key).FirstOrDefault();
                        if (getSelectedDateTime.TimeOfDay != TimeSpan.Zero)
                        {
                            mappedActivityOption.StartTime = getSelectedDateTime.TimeOfDay;
                        }
                        //Group all API items according to activity Primary key and different customer types are inside them.
                        //This is used to fill DatePriceAndAvailabilty
                        var combineAPICustomerProtoTypeByPk = itemAPIProductOptionsByTime.GroupBy(x => x.Id);

                        //Fill Date and Price Availability
                        var price = new Price
                        {
                            Currency = new Currency { IsoCode = activity.CurrencyIsoCode },
                            DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
                        };
                        //Each Activity loop and fill DatePriceAndAvailabilty
                        foreach (var optionDatesAvailability in combineAPICustomerProtoTypeByPk)
                        {
                            var OptionAPIPKSelecteddate = optionDatesAvailability.FirstOrDefault().BasePrice.DatePriceAndAvailabilty.FirstOrDefault().Key;
                            var CacheAdultCustomerTypeIDs = optionMapping.Where(x => x.PassengerType == PassengerType.Adult).Select(y => y.CustomerPrototypeId.ToString()).ToList();
                            var CacheStudentCustomerTypeIDs = optionMapping.Where(x => x.PassengerType == PassengerType.Student).Select(y => y.CustomerPrototypeId.ToString()).ToList();
                            var CacheSeniorCustomerTypeIDs = optionMapping.Where(x => x.PassengerType == PassengerType.Senior).Select(y => y.CustomerPrototypeId.ToString()).ToList();
                            var adultPrices = actOptions.Where(x => CacheAdultCustomerTypeIDs.Contains(x.Code) && x.BasePrice.DatePriceAndAvailabilty.ContainsKey(OptionAPIPKSelecteddate));
                            var studentPrices = actOptions.Where(x => CacheStudentCustomerTypeIDs.Contains(x.Code) && x.BasePrice.DatePriceAndAvailabilty.ContainsKey(OptionAPIPKSelecteddate));
                            var seniorPrices = actOptions.Where(x => CacheSeniorCustomerTypeIDs.Contains(x.Code) && x.BasePrice.DatePriceAndAvailabilty.ContainsKey(OptionAPIPKSelecteddate));
                            var adultPrice = adultPrices?.OrderBy(z => z.BasePrice.Amount)?.FirstOrDefault();
                            var studentPrice = studentPrices?.OrderBy(z => z.BasePrice.Amount)?.FirstOrDefault();
                            var seniorPrice = seniorPrices?.OrderBy(z => z.BasePrice.Amount)?.FirstOrDefault();

                            if (adultPrice == null && studentPrice == null && seniorPrice == null)
                            {
                                price.DatePriceAndAvailabilty.Add(OptionAPIPKSelecteddate.Date, new FareHarborPriceAndAvailability()
                                {
                                    AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE,
                                    CustomerTypePriceIds = new Dictionary<PassengerType, Int64>
                                {
                                    { PassengerType.Adult, 0 },
                                    { PassengerType.Child, 0 },
                                    { PassengerType.Infant, 0 },
                                    { PassengerType.Youth, 0 },
                                    {PassengerType.Student,0 },
                                    { PassengerType.Senior, 0 }
                                },
                                    TotalPrice = 0
                                });
                                mappedActivityOption.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                                continue;
                            }
                            ActivityOption childPrice = null;
                            ActivityOption youthPrice = null;
                            ActivityOption lapChildPrice = null;
                            //ActivityOption studentPrice = null;
                            //ActivityOption seniorPrice = null;

                            var noOfChildren = GetPaxCountByPaxType(criteria, PassengerType.Child);
                            var noOfInfant = GetPaxCountByPaxType(criteria, PassengerType.Infant);
                            var noOfYouth = GetPaxCountByPaxType(criteria, PassengerType.Youth);
                            var noOfAdults = GetPaxCountByPaxType(criteria, PassengerType.Adult);
                            var noOfStudent = GetPaxCountByPaxType(criteria, PassengerType.Student);
                            var noOfSenior = GetPaxCountByPaxType(criteria, PassengerType.Senior);

                            if (noOfChildren > 0)
                            {
                                var queryChild = from o in actOptions
                                                 from m in optionMapping
                                                 where m.PassengerType == PassengerType.Child
                                                 && m.CustomerPrototypeId.ToString() == o.Code
                                                 && o.BasePrice.DatePriceAndAvailabilty.ContainsKey(OptionAPIPKSelecteddate)
                                                 select o;
                                //if child is pass through input and that not exist in option then don't add that option:
                                if (queryChild.FirstOrDefault() == null)
                                {
                                    addOption = false;
                                    break;
                                }

                                childPrice = queryChild?.OrderBy(z => z.BasePrice.Amount)?.FirstOrDefault();
                            }
                            if (noOfInfant > 0)
                            {
                                var queryInfant = from o in actOptions
                                                  from m in optionMapping
                                                  where m.PassengerType == PassengerType.Infant
                                                  && m.CustomerPrototypeId.ToString() == o.Code
                                                  && o.BasePrice.DatePriceAndAvailabilty.ContainsKey(OptionAPIPKSelecteddate)
                                                  select o;

                                //if infant is pass through input and that not exist in option then don't add that option:
                                if (queryInfant.FirstOrDefault() == null)
                                {
                                    addOption = false;
                                    break;
                                }
                                lapChildPrice = queryInfant?.FirstOrDefault();
                            }
                            if (noOfYouth > 0)
                            {
                                var queryYouth = from o in actOptions
                                                 from m in optionMapping
                                                 where m.PassengerType == PassengerType.Youth
                                                 && m.CustomerPrototypeId.ToString() == o.Code
                                                 && o.BasePrice.DatePriceAndAvailabilty.ContainsKey(OptionAPIPKSelecteddate)
                                                 select o;

                                //if youth is pass through input and that not exist in option then don't add that option:
                                if (queryYouth.FirstOrDefault() == null)
                                {
                                    addOption = false;
                                    break;
                                }
                                youthPrice = queryYouth?.FirstOrDefault();
                            }
                            if (noOfStudent > 0)
                            {
                                var queryStudent = from o in actOptions
                                                   from m in optionMapping
                                                   where m.PassengerType == PassengerType.Student
                                                   && m.CustomerPrototypeId.ToString() == o.Code
                                                   && o.BasePrice.DatePriceAndAvailabilty.ContainsKey(OptionAPIPKSelecteddate)
                                                   select o;

                                //if youth is pass through input and that not exist in option then don't add that option:
                                if (queryStudent.FirstOrDefault() == null)
                                {
                                    addOption = false;
                                    break;
                                }
                                studentPrice = queryStudent?.FirstOrDefault();
                            }
                            if (noOfSenior > 0)
                            {
                                var querySenior = from o in actOptions
                                                  from m in optionMapping
                                                  where m.PassengerType == PassengerType.Senior
                                                  && m.CustomerPrototypeId.ToString() == o.Code
                                                  && o.BasePrice.DatePriceAndAvailabilty.ContainsKey(OptionAPIPKSelecteddate)
                                                  select o;

                                //if youth is pass through input and that not exist in option then don't add that option:
                                if (querySenior.FirstOrDefault() == null)
                                {
                                    addOption = false;
                                    break;
                                }
                                seniorPrice = querySenior?.FirstOrDefault();
                            }

                            var customerTypePriceIds = new Dictionary<PassengerType, Int64>();
                            var adultPriceAvailability = adultPrice?.BasePrice.DatePriceAndAvailabilty.Values.Cast<FareHarborPriceAndAvailability>().FirstOrDefault();
                            var childPriceAvailability = childPrice?.BasePrice.DatePriceAndAvailabilty.Values.Cast<FareHarborPriceAndAvailability>().FirstOrDefault();
                            var labChildPriceAvailability = lapChildPrice?.BasePrice.DatePriceAndAvailabilty.Values.Cast<FareHarborPriceAndAvailability>().FirstOrDefault();
                            var youthPriceAvailability = youthPrice?.BasePrice.DatePriceAndAvailabilty.Values.Cast<FareHarborPriceAndAvailability>().FirstOrDefault();
                            var studentPriceAvailabilty = studentPrice?.BasePrice.DatePriceAndAvailabilty.Values.Cast<FareHarborPriceAndAvailability>().FirstOrDefault();
                            var seniorPriceAvailabilty = seniorPrice?.BasePrice.DatePriceAndAvailabilty.Values.Cast<FareHarborPriceAndAvailability>().FirstOrDefault();

                            if (adultPriceAvailability != null)
                            {
                                customerTypePriceIds.Add(PassengerType.Adult, adultPriceAvailability.CustomerTypePriceIds.FirstOrDefault().Value);
                            }

                            if (noOfChildren > 0 && childPriceAvailability != null)
                            {
                                customerTypePriceIds.Add(PassengerType.Child, childPriceAvailability.CustomerTypePriceIds.FirstOrDefault().Value);
                            }
                            if (noOfInfant > 0 && labChildPriceAvailability != null)
                            {
                                customerTypePriceIds.Add(PassengerType.Infant, labChildPriceAvailability.CustomerTypePriceIds.FirstOrDefault().Value);
                            }
                            if (noOfYouth > 0 && youthPriceAvailability != null)
                            {
                                customerTypePriceIds.Add(PassengerType.Youth, youthPriceAvailability.CustomerTypePriceIds.FirstOrDefault().Value);
                            }
                            if (noOfStudent > 0 && studentPriceAvailabilty != null)
                            {
                                customerTypePriceIds.Add(PassengerType.Youth, studentPriceAvailabilty.CustomerTypePriceIds.FirstOrDefault().Value);
                            }
                            if (noOfSenior > 0 && seniorPriceAvailabilty != null)
                            {
                                customerTypePriceIds.Add(PassengerType.Senior, seniorPriceAvailabilty.CustomerTypePriceIds.FirstOrDefault().Value);
                            }

                            var totalPrice = (adultPrice?.BasePrice?.Amount ?? 0.0m)
                                + (childPrice?.BasePrice?.Amount ?? 0.0m)
                                + (youthPrice?.BasePrice?.Amount ?? 0.0m)
                                + (lapChildPrice?.BasePrice?.Amount ?? 0.0m)
                                + (studentPrice?.BasePrice?.Amount ?? 0.0m)
                                + (seniorPrice?.BasePrice?.Amount ?? 0.0m);

                            var queryPaxUnits = from o in actOptions
                                                from pm in optionMapping
                                                from dt in o?.BasePrice?.DatePriceAndAvailabilty
                                                where o.Code == pm?.CustomerPrototypeId.ToString()
                                                && dt.Key.Date == OptionAPIPKSelecteddate.Date
                                                && dt.Value?.PricingUnits?.FirstOrDefault() != null
                                                select dt.Value?.PricingUnits?.FirstOrDefault();

                            var pricingUnit = queryPaxUnits?.DistinctBy(x => ((PerPersonPricingUnit)x).PassengerType.ToString())?.OrderBy(y => y.Price).ToList();
                            var pusDisntict = new List<PricingUnit>();
                            foreach (var pu in pricingUnit)
                            {
                                try
                                {
                                    if (pusDisntict.Any(x => ((PerPersonPricingUnit)x).PassengerType == ((PerPersonPricingUnit)pu).PassengerType) == false)
                                    {
                                        var tt = pu.DeepCopy();
                                        pusDisntict.Add(tt);
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }

                            price.Amount = Convert.ToDecimal(totalPrice);

                            if (!price.DatePriceAndAvailabilty.Keys.Contains(OptionAPIPKSelecteddate.Date))
                            {
                                price.DatePriceAndAvailabilty.Add(OptionAPIPKSelecteddate.Date, new FareHarborPriceAndAvailability
                                {
                                    AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                                    TotalPrice = Convert.ToDecimal(totalPrice),
                                    CustomerTypePriceIds = customerTypePriceIds,
                                    PricingUnits = pusDisntict,
                                    //As we are mapping date level capacity in all the options-->BasePrice, we can fetch capacity from any of the option. Here we are fetching from Adult Option-->BasePrice
                                    Capacity = adultPriceAvailability?.Capacity ?? studentPriceAvailabilty?.Capacity ?? seniorPriceAvailabilty?.Capacity ?? 0,
                                    IsCapacityCheckRequired = true
                                });
                            }
                            mappedActivityOption.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                        }
                        mappedActivityOption.BasePrice = price.DeepCopy();
                        mappedActivityOption.GateBasePrice = price.DeepCopy();
                        if (addOption == true)
                        {
                            mappedProductOption.Add(mappedActivityOption);
                        }
                        mappedActivityOption.HotelPickUpLocation = activity.HotelPickUpLocation;
                    }
                }
                activity.ProductOptions = mappedProductOption;
                return activity;
            }
            return new Activity();
        }

        /*
        private PricingUnit CreatePricingUnit(PassengerType passengerType, decimal sellPrice)
        {
            var pricingUnit = PricingUnitFactory.GetPricingUnit(passengerType);
            pricingUnit.Price = sellPrice;
            pricingUnit.PriceType = PriceType.PerPerson;
            pricingUnit.UnitType = UnitType.PerPerson;
            return pricingUnit;
        }
        */

        private IsangoErrorEntity SendException(Int32 activityId, string message)
        {
            var isangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = "FareHarbourService",
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