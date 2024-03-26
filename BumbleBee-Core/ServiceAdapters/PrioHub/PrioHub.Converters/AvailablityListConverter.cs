using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.PrioHub;
using Logger.Contract;
using ServiceAdapters.PrioHub.Constants;
using ServiceAdapters.PrioHub.PrioHub.Converters.Contracts;
using ServiceAdapters.PrioHub.PrioHub.Entities.AvailabilityListResponse;

namespace ServiceAdapters.PrioHub.PrioHub.Converters
{
    public class AvailablityListConverter : ConverterBase, IAvailablityListConverter
    {
        public AvailablityListConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="objectResult"></param>
        /// <returns></returns>
        public override object Convert(object objectResult)
        {
            return ConvertAvailablityResult(objectResult);
        }

        /// <summary>
        /// Get Price And Availability For Prio
        /// </summary>
        /// <param name="vacancies"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="availabilityActive"></param>
        /// <param name="availabilityId"></param>
        /// <param name="availabilityPricing"></param>
        /// <returns></returns>
        private PrioHubPriceAndAvailability GetPriceAndAvailabiltyForPrio(
            string vacancies, string fromDateTime, 
            string toDateTime,bool availabilityActive
            ,string availabilityId, List<AvailabilityPricing> availabilityPricing)
        {
            var defaultCapacity = System.Convert.ToInt32(Util.ConfigurationManagerHelper.GetValuefromAppSettings("DefaultCapacity"));
            int.TryParse(vacancies, out var capacity);
            if (capacity == 0) capacity = defaultCapacity;

            var pAndA = new PrioHubPriceAndAvailability
            {
                AvailabilityFromDateTime = fromDateTime,
                AvailabilityToDateTime = toDateTime,
                Vacancies = vacancies,
                Capacity = capacity,
                IsCapacityCheckRequired = true,
                AvailabilityActive= availabilityActive,
                AvailabilityId= availabilityId,
               
            };

            var prioHubAvailabilityPricing= new List<PrioHubAvailabilityPricing>();
            if (availabilityPricing != null && availabilityPricing.Count > 0)
            {
                foreach (var item in availabilityPricing)
                {
                    var data = new PrioHubAvailabilityPricing
                    {
                         AvailabilityPricingVariationAmount = item.AvailabilityPricingVariationAmount,
                         AvailabilityPricingVariationCommissionIncluded= item.AvailabilityPricingVariationCommissionIncluded,
                         AvailabilityPricingVariationPercentage= item?.AvailabilityPricingVariationPercentage,
                         AvailabilityPricingVariationPriceType= item?.AvailabilityPricingVariationPriceType,
                         AvailabilityPricingVariationProductTypeDiscountIncluded= item.AvailabilityPricingVariationProductTypeDiscountIncluded,
                         AvailabilityPricingVariationProductTypeId= item.AvailabilityPricingVariationProductTypeId,
                         AvailabilityPricingVariationType= item?.AvailabilityPricingVariationType
                    };

                    prioHubAvailabilityPricing.Add(data);
                }
                pAndA.PrioHubAvailabilityPricing = prioHubAvailabilityPricing;
            }
            if (availabilityActive==true)
            {
                pAndA.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
            }
            else
            {
                pAndA.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
            }
            return pAndA;
        }

        /// <summary>
        /// Convert Availability Result
        /// </summary>
        /// <param name="objectResult"></param>
        /// <returns></returns>
        public List<ActivityOption> ConvertAvailablityResult(object objectResult)
        {
            var finalData = new List<ActivityOption>();

            var availabilityRs = (AvailabilityListResponse)objectResult;

            if (availabilityRs?.Data?.Items != null && availabilityRs?.Data?.Items.Count>0)
            {
                var groupByProductId = availabilityRs?.Data?.Items.GroupBy(e => e.AvailabilityProductId).ToArray();

                foreach (var itemData in groupByProductId)
                {
                    var activityOption = new ActivityOption
                    {
                        PrioHubProductId = itemData?.FirstOrDefault()?.AvailabilityProductId,
                        //Same availability_product_id for all dates
                    };
                    var basePrice = new Price
                    {
                        DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
                    };

                    foreach (var availability in itemData)
                    {
                        try
                        {
                            var availabilitiesRs = availability;
                            var dtoFromDate = DateTimeOffset.ParseExact(availabilitiesRs.AvailabilityFromDateTime, Constant.DateTimeWithSecondFormat, null).DateTime;
                            var dtoToDate = DateTimeOffset.ParseExact(availabilitiesRs.AvailabilityToDateTime, Constant.DateTimeWithSecondFormat, null).DateTime;
                            var fromDate = dtoFromDate;

                            var toDate = dtoToDate;
                            var fromDateTime = availabilitiesRs.AvailabilityFromDateTime;
                            var toDateTime = availabilitiesRs.AvailabilityToDateTime;

                            var vacancies = System.Convert.ToString(availability?.AvailabilitySpots?.AvailabilitySpotsOpen);
                            var availabilityPricing = availabilitiesRs?.AvailabilityPricing;

                            //Check Available or not
                            var available = false;
                            //Have Slots
                            if (availabilitiesRs.AvailabilityActive==true && availabilitiesRs?.AvailabilitySpots?.AvailabilitySpotsTotal >= 0)
                            {
                                if (availabilitiesRs?.AvailabilitySpots?.AvailabilitySpotsOpen > 0)
                                {
                                    available = true;
                                }
                                else
                                {
                                    available = false;
                                }
                            }
                            else //no Slots
                            {
                                available = availabilitiesRs.AvailabilityActive;
                            }

                            if (fromDate.Date == toDate.Date)
                            {
                                var pAndA = GetPriceAndAvailabiltyForPrio(vacancies, fromDateTime, toDateTime, available, 
                                    availabilitiesRs.AvailabilityId, availabilityPricing);

                                if (pAndA.AvailabilityStatus != AvailabilityStatus.NOTAVAILABLE)
                                {
                                    if (basePrice?.DatePriceAndAvailabilty?.Any(x => x.Key.Date == fromDate.Date) ?? false)
                                    {
                                        activityOption.IsTimeBasedOption = true;
                                    }
                                    basePrice.DatePriceAndAvailabilty.Add(fromDate, pAndA);
                                }
                            }
                            else
                            {
                                while (fromDate.Date != toDate.Date)
                                {
                                    try
                                    {
                                        var pAndA = GetPriceAndAvailabiltyForPrio(vacancies, fromDateTime, string.Empty,
                                           available, availabilitiesRs.AvailabilityId, availabilityPricing);
                                        if (pAndA.AvailabilityStatus != AvailabilityStatus.NOTAVAILABLE)
                                        {
                                            if (basePrice?.DatePriceAndAvailabilty?.Any(x => x.Key.Date == fromDate.Date) ?? false)
                                            {
                                                activityOption.IsTimeBasedOption = true;
                                            }
                                            basePrice.DatePriceAndAvailabilty.Add(fromDate, pAndA);
                                        }
                                        fromDate = fromDate.AddDays(1);
                                        var timeZone = string.Empty;
                                        if (!string.IsNullOrEmpty(fromDateTime))
                                        {
                                            timeZone = $"T{fromDateTime.Split('T')[1]}";
                                        }
                                        var dtoFromDatetime = DateTimeOffset.ParseExact(fromDateTime, Constant.DateTimeWithSecondFormat, null);
                                        fromDateTime = dtoFromDatetime.Date.AddDays(1).ToString(Constant.DateFormat);
                                        fromDateTime = $"{fromDateTime}{timeZone}";
                                    }
                                    catch (Exception ex)
                                    {
                                        //ignored
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }
                    }
                    var isAvailable = basePrice.DatePriceAndAvailabilty.Any(x => x.Value.AvailabilityStatus == AvailabilityStatus.AVAILABLE);
                    activityOption.AvailabilityStatus = isAvailable ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE;
                    basePrice.Currency = new Currency { IsoCode = "", Name = "", IsPostFix = false, Symbol = "" };
                    activityOption.BasePrice = basePrice;
                    activityOption.CostPrice = basePrice.DeepCopy();
                    activityOption.GateBasePrice = basePrice.DeepCopy();
                    // Preparing GateBasePrice here as its needed in PriceRuleEngine
                    finalData.Add(activityOption);
                }
            }
            else
            {
                throw new Exception(Constant.Exception1);
               
            }
            return finalData;
        }
    }
}