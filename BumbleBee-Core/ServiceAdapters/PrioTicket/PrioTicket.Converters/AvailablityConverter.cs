using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Prio;
using Logger.Contract;
using ServiceAdapters.PrioTicket.Constants;
using ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceAdapters.PrioTicket.PrioTicket.Converters
{
    public class AvailablityConverter : ConverterBase, IAvailablityConverter
    {
        public AvailablityConverter(ILogger logger) : base(logger)
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
        /// <returns></returns>
        private PrioPriceAndAvailability GetPriceAndAvailabiltyForPrio(string vacancies, string fromDateTime, string toDateTime)
        {
            var defaultCapacity = System.Convert.ToInt32(Util.ConfigurationManagerHelper.GetValuefromAppSettings("DefaultCapacity"));
            int.TryParse(vacancies, out var capacity);
            if (capacity == 0) capacity = defaultCapacity;

            var pAndA = new PrioPriceAndAvailability
            {
                FromDateTime = fromDateTime,
                ToDateTime = toDateTime,
                Vacancies = vacancies,
                Capacity = capacity,
                IsCapacityCheckRequired = true
            };
            if ((pAndA.Vacancies.ToLower() == PrioApiStatus.Nolimit.ToLower())
                || ((!string.IsNullOrEmpty(pAndA.Vacancies))
                && ((System.Convert.ToInt32(pAndA.Vacancies)) > 0))
            )
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
        public ActivityOption ConvertAvailablityResult(object objectResult)
        {
            var availabilityRs = (AvailablityRs)objectResult;

            if (availabilityRs?.Data?.Availabilities != null)
            {
                var activityOption = new ActivityOption();
                var basePrice = new Price
                {
                    DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
                };

                foreach (var availability in availabilityRs.Data.Availabilities)
                {
                    try
                    {
                        var availabilitiesRs = availability;
                        var dtoFromDate = DateTimeOffset.ParseExact(availabilitiesRs.FromDateTime, Constant.DateTimeWithSecondFormat, null).DateTime;
                        var dtoToDate = DateTimeOffset.ParseExact(availabilitiesRs.ToDateTime, Constant.DateTimeWithSecondFormat, null).DateTime;
                        var fromDate = dtoFromDate;
                        var toDate = dtoToDate;
                        var fromDateTime = availabilitiesRs.FromDateTime;
                        var toDateTime = availabilitiesRs.ToDateTime;

                        var vacancies = availability.Vacancies;
                        if (fromDate.Date == toDate.Date)
                        {
                            var pAndA = GetPriceAndAvailabiltyForPrio(vacancies, fromDateTime, toDateTime);

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
                                    var pAndA = GetPriceAndAvailabiltyForPrio(vacancies, fromDateTime, string.Empty);
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
                activityOption.GateBasePrice = basePrice.DeepCopy(); // Preparing GateBasePrice here as its needed in PriceRuleEngine

                return activityOption;
            }
            else
            {
                throw new Exception(Constant.Exception1);
            }
        }
    }
}