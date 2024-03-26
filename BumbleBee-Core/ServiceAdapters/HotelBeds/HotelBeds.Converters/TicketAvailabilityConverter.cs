using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.HotelBeds.Constants;
using ServiceAdapters.HotelBeds.HotelBeds.Converters.Contracts;
using ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace ServiceAdapters.HotelBeds.HotelBeds.Converters
{
    public class TicketAvailabilityConverter : ConverterBase, ITicketAvailabilityConverter
    {
        public TicketAvailabilityConverter(ILogger logger) : base(logger)
        {
        }

        private string _currency;

        public object Convert(object objectresult)
        {
            var result = (TicketAvailRs)objectresult;
            if (result != null)
            {
                return ConvertAvailibilityResult(result);
            }

            return null;
        }

        private List<Activity> ConvertAvailibilityResult(TicketAvailRs result)
        {
            var activities = new List<Activity>();
            var ticketCriteria = (Isango.Entities.Ticket.TicketCriteria)result.InputCriteria;
            foreach (ServiceTicket serviceTicket in result.ServiceTicket)
            {
                var show = new Show();
                var activity = new Activity();

                //if (serviceTicket.AvailableModality[0].Contract.Name.Trim().ToUpper().StartsWith("RATE#"))
                if (!string.IsNullOrWhiteSpace(serviceTicket?.AvailableModality[0]?.SupplierOption))
                {
                    activity = show;
                    activity.ActivityType = ActivityType.Theatre;
                }

                _currency = serviceTicket?.Currency?.Code;
                activity.Name = serviceTicket?.TicketInfo?.Name;
                activity.RegionName = serviceTicket?.TicketInfo?.Destination?.Code;

                if (serviceTicket?.ContentFactSheet?.TicketFeatureList != null)
                {
                    var inclusions = serviceTicket?.ContentFactSheet?.TicketFeatureList?.FindAll(f => f.Group == 1);
                    if (inclusions.Count > 0)
                    {
                        var sb = new StringBuilder();
                        foreach (var item in inclusions[0]?.FeatureList)
                        {
                            sb.AppendLine(item.Description);
                        }
                        activity.Inclusions = sb.ToString();
                    }
                    var exclusions = serviceTicket?.ContentFactSheet?.TicketFeatureList?.FindAll(f => f.Group == 2);
                    if (exclusions.Count > 0)
                    {
                        var ex = new StringBuilder();
                        foreach (var item in exclusions[0]?.FeatureList)
                        {
                            ex.AppendLine(item.Description);
                        }
                        activity.Exclusions = ex.ToString();
                    }
                }

                //hotel.RegionID = Int32.Parse(servicehotel.HotelInfo.Destination.DestinationCode);
                //TODO: Check if TicketInfo actually does not contain geocoordinates.
                /*GeoLocation location = new GeoLocation();
                location.Latitude = float.Parse(servicehotel.TicketInfo.Destination.Position.Latitude, System.Globalization.CultureInfo.InvariantCulture);
                location.Longitute = float.Parse(servicehotel.TicketInfo.Position.Longitude, System.Globalization.CultureInfo.InvariantCulture);
                activity.GeoLocation = location;*/

                //TODO: We need to determine a system to assign activity ID without making a DB call. Perhaps the ID will be assigned after convert!
                activity.ID = 0;// Int32.Parse(servicehotel.TicketInfo.Code, System.Globalization.CultureInfo.InvariantCulture);

                activity.Code = serviceTicket?.TicketInfo?.Code;
                activity.CategoryIDs = new List<int> { 1 };

                activity.DurationString = serviceTicket?.TicketInfo?.TicketClass;

                var options = new List<ActivityOption>();
                // ReSharper disable once PossibleNullReferenceException
                var travelInfo = new TravelInfo { StartDate = serviceTicket.DateFrom.Date.ToDateTimeExact() };

                //var difference = serviceTicket.DateTo.Date.ToDateTimeExact() - serviceTicket.DateFrom.Date.ToDateTimeExact();
                //travelInfo.NumberOfNights = difference.Days;

                #region Verifying that all pax types from input are in result from api

                var query = from guest in serviceTicket.Paxes.GuestList
                            from paxAges in ticketCriteria?.Ages
                            from paxCount in ticketCriteria?.NoOfPassengers
                            where paxAges.Key != PassengerType.Adult
                            && guest.Age.ToInt() == paxAges.Value
                            && paxCount.Key == paxAges.Key
                            select new { PassengerType = paxAges.Key, Age = paxAges.Value, guestType = guest.Type, Count = paxCount.Value };
                var nonAdultGuests = query.ToList();
                travelInfo.NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    { PassengerType.Adult, serviceTicket.Paxes.AdultCount.ToInt() }
                };

                //For adult we can use "AD" from api  as identification , but for youth , child , infant the api gives CH , we can distinguish child by their ages
                var adultAgeInAPIRes = System.Convert.ToInt32(serviceTicket?.Paxes?.GuestList?.FirstOrDefault(x => ConvertPassengerType(x.Type) == PassengerType.Adult)?.Age);

                //IN CASE OF ADULT ONLY
                if (serviceTicket?.Paxes?.GuestList?.Count == 0 && adultAgeInAPIRes == 0)
                {
                    adultAgeInAPIRes = 30;// System.Convert.ToInt32((ticketCriteria?.Ages?.FirstOrDefault(x => x.Key == PassengerType.Adult))?.Value);
                }

                travelInfo.Ages = new Dictionary<PassengerType, int> {
                    { PassengerType.Adult, adultAgeInAPIRes }
                };

                foreach (var item in nonAdultGuests)
                {
                    if (!travelInfo.NoOfPassengers.Keys.Contains(item.PassengerType))
                    {
                        travelInfo.NoOfPassengers.Add(item.PassengerType, item.Count);
                    }
                    if (!travelInfo.Ages.Keys.Contains(item.PassengerType))
                    {
                        travelInfo.Ages.Add(item.PassengerType, item.Age);
                    }
                }

                #endregion Verifying that all pax types from input are in result from api

                #region saviant's code commented.

                //if (serviceTicket?.Paxes != null && serviceTicket?.Paxes?.GuestList != null)
                //{
                //    foreach (var item in serviceTicket?.Paxes?.GuestList)
                //    {
                //        if (item.Type.ToUpperInvariant().Equals("CH"))
                //        {
                //            if(!travelInfo.Ages.Keys.Contains(PassengerType.Child))
                //                travelInfo.Ages.Add(PassengerType.Child, item.Age.ToInt());
                //        }
                //    }
                //}

                #endregion saviant's code commented.

                #region was already commneted

                /*
                        List<HotelPromotion> promotions = new List<HotelPromotion>();
                        if (servicehotel.Promotions != null && servicehotel.Promotions.Count > 0)
                            promotions = CreatePromotions(servicehotel.Promotions);
                        */

                #endregion was already commneted

                activity.FactsheetId = System.Convert.ToInt32(serviceTicket?.ContentFactSheet?.Code?.ToInt());

                foreach (var t in serviceTicket.AvailableModality)
                {
                    try
                    {
                        var option = CreateOption(t, travelInfo);

                        //Add Contract
                        option.Contract = new Contract
                        {
                            Name = t.Contract.Name,
                            ClassificationCode = t.Contract.Classification,
                            InComingOfficeCode = t.Contract.IncomingOffice.Code
                        };
                        //option.Contract.Comments = modality.Contract.Comments;

                        //TODO: Need to discuss if AvailOption for activities need to be moved to Service level.
                        option.AvailToken = serviceTicket.AvailToken;
                        options.Add(option);
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "HotelBeds.TicketAvailabilityConverter",
                            MethodName = "ConvertAvailibilityResult.CreateOption"
                        };
                        _logger.Error(isangoErrorEntity, ex);
                        continue;
                    }
                }
                var productOptions = options.Cast<ProductOption>().ToList();
                activity.ProductOptions = productOptions;
                activities.Add(activity);
            }

            return activities;
        }

        private ActivityOption CreateOption(TktAvailableModality modality, TravelInfo travelInfo)
        {
            if (modality == null)
                return null;
            var option = new ActivityOption
            {
                Name = modality.Name,
                TravelInfo = new TravelInfo
                {
                    Ages = travelInfo.Ages,
                    NoOfPassengers = travelInfo.NoOfPassengers,
                    NumberOfNights = 0,
                    StartDate = travelInfo.StartDate
                }
            };

            //Price and Currency
            var costPrice = new Price();

            var baseAmount = decimal.MaxValue;

            var pAndA = new Dictionary<DateTime, PriceAndAvailability>();
            var operationDates = modality?.OperationDateList?.OperationDate;
            operationDates = operationDates?.ToList();
            if (operationDates?.Count > 0)
            {
                foreach (var item in operationDates)
                {
                    var price = new DefaultPriceAndAvailability
                    {
                        AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                        MinDuration = item.MinimumDuration.ToInt(),
                        MaxDuration = item.MaximumDuration.ToInt()
                    };
                    var daysToAdd = System.Convert.ToInt32(item?.MaximumDuration?.ToInt()) - 1;
                    if (daysToAdd < 0)
                    {
                        daysToAdd = 0;
                    }
                    option.TravelInfo.NumberOfNights = daysToAdd;

                    //Filtration of prices based on the ages passed in input , and pax type mapping
                    //Using child ages mapping to distinguish b/w in Youth, Child and Infant
                    var adultPriceRangeList = from m in modality.PriceRangeList
                                              from pax in travelInfo.Ages
                                              where pax.Value >= m.AgeFrom.ToInt() && pax.Value <= m.AgeTo.ToInt()
                                              && pax.Key == PassengerType.Adult
                                              select m.UnitPrice.ToDecimal();
                    var childPriceRangeList = from m in modality.PriceRangeList
                                              from pax in travelInfo.Ages
                                              where pax.Value >= m.AgeFrom.ToInt() && pax.Value <= m.AgeTo.ToInt()
                                              && pax.Key == PassengerType.Child
                                              select m.UnitPrice.ToDecimal();
                    var infantPriceRangeList = from m in modality.PriceRangeList
                                               from pax in travelInfo.Ages
                                               where pax.Value >= m.AgeFrom.ToInt() && pax.Value <= m.AgeTo.ToInt()
                                               && pax.Key == PassengerType.Infant
                                               select m.UnitPrice.ToDecimal();
                    var youthRangeList = from m in modality.PriceRangeList
                                         from pax in travelInfo.Ages
                                         where pax.Value >= m.AgeFrom.ToInt() && pax.Value <= m.AgeTo.ToInt()
                                         && pax.Key == PassengerType.Youth
                                         select m.UnitPrice.ToDecimal();
                    var adultCostPrice = System.Convert.ToDecimal(adultPriceRangeList?.FirstOrDefault());//modality.PriceList.Find(p => ConvertCustomerType(p.Description) == PassengerType.Adult).Amount.ToDecimal();
                    var childCostPrice = System.Convert.ToDecimal(childPriceRangeList?.FirstOrDefault());//modality.PriceList.Find(p => ConvertCustomerType(p.Description) == PassengerType.Child).Amount.ToDecimal();
                    var infantCostPrice = System.Convert.ToDecimal(infantPriceRangeList?.FirstOrDefault());//modality.PriceList.Find(p => ConvertCustomerType(p.Description) == PassengerType.Infant).Amount.ToDecimal();
                    var youthCostPrice = System.Convert.ToDecimal(youthRangeList?.FirstOrDefault());

                    price.PricingUnits = new List<PricingUnit>();
                    if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Adult))
                    {
                        var adultPrice = new AdultPricingUnit
                        {
                            Price = adultCostPrice
                        };
                        price.PricingUnits.Add(adultPrice);
                    }

                    if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Child))
                    {
                        var childPrice = new ChildPricingUnit
                        {
                            Price = childCostPrice
                        };
                        price.PricingUnits.Add(childPrice);
                    }
                    if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Youth))
                    {
                        var youthPrice = new YouthPricingUnit
                        {
                            Price = youthCostPrice
                        };
                        price.PricingUnits.Add(youthPrice);
                    }
                    if (travelInfo.NoOfPassengers.ContainsKey(PassengerType.Infant))
                    {
                        var infantPrice = new InfantPricingUnit
                        {
                            Price = infantCostPrice
                        };
                        price.PricingUnits.Add(infantPrice);
                    }

                    var servicePrice = modality.PriceList.Find(p => p.Description.ToUpperInvariant().Equals(Constant.ServicePriceDesc)).Amount.ToDecimal();

                    if (servicePrice.Equals(0))
                        price.TotalPrice = childCostPrice + adultCostPrice * travelInfo.NoOfPassengers.AsEnumerable().Where(x => x.Key == PassengerType.Adult).Select(x => x.Value).SingleOrDefault();
                    else
                        price.TotalPrice = servicePrice;

                    if (price.TotalPrice < baseAmount)
                        baseAmount = price.TotalPrice;

                    price.IsSelected = false;
                    if (!pAndA.Keys.Contains(item.Date.ToDateTimeExact()))
                        pAndA.Add(item.Date.ToDateTimeExact(), price);
                }
            }

            costPrice.DatePriceAndAvailabilty = pAndA;

            costPrice.Amount = baseAmount;
            costPrice.Currency = new Currency
            {
                Name = _currency,
                IsoCode = _currency
            };

            option.CostPrice = costPrice;

            #region Seasons

            var seasons = new List<ActivitySeason>();
            var season = new ActivitySeason();

            var operationDate = modality.OperationDateList?.OperationDate.OrderBy(s => s.Date).ToList();
            if (operationDate != null && operationDate.Count > 0)
            {
                season.FromDate = operationDate.First().Date.ToDateTimeExact();
                season.ToDate = operationDate.Last().Date.ToDateTimeExact();
            }

            var policies = new List<ActivityPolicy>();
            var policy = new ActivityPolicy();
            var aPrices = new List<PolicyCategory>();

            foreach (var tktPrice in modality.PriceList)
            {
                var aPrice = new PolicyCategory
                {
                    PerUnitPrice = new ActivityPrice
                    {
                        BaseCurrencyCode = _currency,
                        BasePrice = tktPrice.Amount.ToDecimal()
                    },
                    MinimumCustomers = 1,
                    MaximumCustomers = 20,
                    PolicyCategoryType = ConvertPassengerType(tktPrice.Description)
                };
                if (aPrice.PolicyCategoryType == PassengerType.Child)
                {
                    aPrice.FromAge = modality.ChildAge.AgeFrom.ToInt();
                    aPrice.ToAge = modality.ChildAge.AgeTo.ToInt();
                    aPrice.PerUnitPrice.IsAllowed = true;
                }

                aPrice.PerUnitPrice.IsPercent = false;
                aPrices.Add(aPrice);
            }
            policy.PolicyCategories = aPrices;

            policies.Add(policy);
            season.ActivityPolicies = policies;
            seasons.Add(season);

            #endregion Seasons

            option.ActivitySeasons = seasons;

            //TODO: Check if HotelBeds activity has the equivalent of On Request.
            option.AvailabilityStatus = AvailabilityStatus.AVAILABLE;

            option.Code = modality.Code;
            option.Description = null; //There's no equivalent in HB Activity

            //TODO: We need to determine a system to assign activity ID without making a DB call. Perhaps the ID will be assigned after convert!
            //option.ID = 0;

            if (modality.CancellationPolicyList != null)
            {
                var cancellationCost = new List<CancellationPrice>();
                foreach (var charge in modality.CancellationPolicyList)
                {
                    var price = new CancellationPrice
                    {
                        Percentage = charge.Price.Percentage.ToFloat(),
                        CancellationFromdate = charge.Price.DateTimeFrom.Date.ToDateTimeExact(),
                        CancellationAmount = charge.Price.Amount.ToDecimal()
                    };
                    cancellationCost.Add(price);
                }
                option.CancellationPrices = cancellationCost;
            }

            return option;
        }

        private PassengerType ConvertPassengerType(string customerType)
        {
            var customer = customerType.Substring(0, 2);
            if (customer.Equals(Constant.Ch))
                return PassengerType.Child;
            if (customer.Equals(Constant.In))
                return PassengerType.Infant;
            if (customer.Equals(Constant.Yu))
                return PassengerType.Youth;
            else
                return PassengerType.Adult;
        }
    }
}