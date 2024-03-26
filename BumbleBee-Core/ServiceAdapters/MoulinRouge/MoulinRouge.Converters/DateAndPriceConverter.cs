using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.MoulinRouge;
using Logger.Contract;
using ServiceAdapters.MoulinRouge.Constants;
using ServiceAdapters.MoulinRouge.MoulinRouge.Converters.Contracts;
using ServiceAdapters.MoulinRouge.MoulinRouge.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Util;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Converters
{
    public class DateAndPriceConverter : ConverterBase, IDateAndPriceConverter
    {
        public DateAndPriceConverter(ILogger logger) : base(logger)
        {
        }

        private string _currency;

        public override object Convert<T>(T objectResult)
        {
            var result = objectResult as List<DateAndPrice>;
            if (result == null) return null;
            var activities = ConvertToActivitiesResult(result);
            return activities;
        }

        #region Convert to List<Activity>

        private List<Activity> ConvertToActivitiesResult(List<DateAndPrice> dateAndPriceResult)
        {
            var activities = new List<Activity>();
            var validItems = MoulinRougeAPIConfig.Instance.ValidRateIdContingentIDs;
            var services = dateAndPriceResult.Where(item => item.ActivityId > 0).Select(ser => ser.ActivityId).Distinct().ToList();
            var quantity = dateAndPriceResult.Where(item => item.Quantity > 0).Select(ser => ser.Quantity).Distinct().FirstOrDefault();

            if (services.Count <= 0) return activities;

            foreach (var serviceId in services)
            {
                var serviceOptionsFromApi = dateAndPriceResult.Join(validItems, dp => dp.ContingentId, vi => vi.ContingentId,
                        (dp, vi) => new { dp, vi })
                    .Where(dpvi => dpvi.dp.ActivityId == serviceId
                                   && dpvi.vi.Type == MoulinRougeServiceType.Dinner
                                   && dpvi.vi.RateId == dpvi.dp.RateId
                                   && dpvi.dp.ServiceType == dpvi.vi.Type)
                    .Select(dpvi => dpvi.dp).ToList();

                serviceOptionsFromApi.AddRange(dateAndPriceResult.Join(validItems, dp => dp.ContingentId, vi => vi.ContingentId,
                    (dp, vi) => new { dp, vi })
                .Where(dpvi => dpvi.dp.ActivityId == serviceId
                               && dpvi.vi.Type == MoulinRougeServiceType.Show
                               && dpvi.vi.RateId == dpvi.dp.RateId
                               && dpvi.dp.ServiceType == dpvi.vi.Type)
                .Select(dpvi => dpvi.dp).ToList());

                var activity = GetActivity(serviceOptionsFromApi, quantity, serviceId);
                if (activity != null && activity.ProductOptions.Count > 0)
                    activities.Add(activity);
            }
            return activities;
        }

        private Activity GetActivity(List<DateAndPrice> serviceOptionsFromApi, int quantity, int serviceId)
        {
            _currency = Constant.Eur;
            // ReSharper disable once PossibleNullReferenceException
            var firstAvailableDate = serviceOptionsFromApi.OrderBy(so => so.DateStart).FirstOrDefault().DateStart;
            var contingentId = serviceOptionsFromApi.OrderBy(so => so.ContingentId).Select(item => item.ContingentId).Distinct().FirstOrDefault();

            var activity = new Activity
            {
                ActivityType = ActivityType.HalfDay,
                ProductOptions = new List<ProductOption>(),
                ApiType = APIType.Moulinrouge,
                Name = string.Empty,
                RegionName = Constant.Paris,
                Inclusions = string.Empty,
                Exclusions = string.Empty,
                ID = serviceId,
                Code = string.Empty,
                CategoryIDs = new List<int> { 1 },
                DurationString = string.Empty,
                FactsheetId = contingentId
            };

            var serviceOptionsFoIsango = serviceOptionsFromApi.Where(item => item.RateId > 0).Select(ser => ser.OptionCode).Distinct().ToList();

            if (serviceOptionsFoIsango.Count > 0)
            {
                foreach (var serviceOptionCode in serviceOptionsFoIsango)
                {
                    var items = serviceOptionsFromApi.Where(item => item.OptionCode == serviceOptionCode).ToList();
                    if (items.Count > 0)
                    {
                        try
                        {
                            var option = CreateOption(items, quantity);

                            //Add Contract
                            option.Contract = new Contract();
                            option.Code = serviceOptionCode;
                            option.SupplierOptionCode = serviceOptionCode;
                            option.TravelInfo = new TravelInfo
                            {
                                StartDate = firstAvailableDate.ToString(Constant.DateFormat).ToDateTimeExact(),
                                NoOfPassengers = new Dictionary<PassengerType, int>
                                {
                                    {PassengerType.Adult, quantity},
                                    {PassengerType.Child, 0 }
                                },
                                NumberOfNights = 0
                            };
                            activity.ProductOptions.Add(option);
                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "MoulinRouge.DateAndPriceConverter",
                                MethodName = "CreateOption"
                            };
                            _logger.Error(isangoErrorEntity, ex);
                            continue;
                        }
                    }
                }
            }
            return activity;
        }

        /// <summary>
        /// It create Options by grouping on MoulinRouge Services for Activity
        /// </summary>
        /// <param name="modality"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private ActivityOption CreateOption(List<DateAndPrice> modality, int quantity)
        {
            decimal sellPriceValue = 0; //This contains price with margin
            var minCostPrice = modality.OrderBy(so => so.Amount).FirstOrDefault()?.Amount ?? 0 * quantity;
            var minBasePrice = modality.OrderBy(so => so.AmountDetail[8]).FirstOrDefault()?.AmountDetail[8] ?? 0 * quantity;

            var option = new ActivityOption
            {
                Name = modality.FirstOrDefault()?.ServiceTypeName
            };

            if (TimeSpan.TryParse(modality.FirstOrDefault()?.OptionCode.Split('~')[1], out TimeSpan startTime))
            {
                option.StartTime = startTime;
            }

            var priceAndAvailabiltyCost = new Dictionary<DateTime, PriceAndAvailability>();
            var priceAndAvailabiltyBase = new Dictionary<DateTime, PriceAndAvailability>();

            foreach (var availableDate in modality)
            {
                var dateKey = availableDate.DateStart.ToString(Constant.DateFormat).ToDateTimeExact();

                // Cost Price
                var adultPricingUnit = new AdultPricingUnit
                {
                    Price = availableDate.Amount
                };
                var childPricingUnit = new ChildPricingUnit
                {
                    Price = availableDate.Amount
                };

                var priceCost = new MoulinRougePriceAndAvailability
                {
                    AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                    CatalogDateID = availableDate.CatalogDateId,
                    TotalPrice = availableDate.Amount * quantity,
                    IsSelected = false,
                    Capacity = availableDate.Stock,
                    IsCapacityCheckRequired = true,
                    PricingUnits = new List<PricingUnit>
                    {
                        adultPricingUnit,
                        childPricingUnit
                    },
                    MoulinRouge = new APIContextMoulinRouge
                    {
                        Amount = availableDate.Amount * quantity,
                        BlocId = availableDate.BlocId,
                        CatalogDateId = availableDate.CatalogDateId,
                        RateId = availableDate.RateId,
                        CategoryId = availableDate.CategoryId,
                        ContingentId = availableDate.ContingentId,
                        DateStart = availableDate.DateStart,
                        DateEnd = availableDate.DateEnd,
                        ServiceType = System.Convert.ToString(availableDate.ServiceType),
                        ServiceTypeName = availableDate.ServiceTypeName,
                        FloorId = availableDate.FloorId,
                        ServiceOptionCode = availableDate.OptionCode
                    }
                };

                if (!priceAndAvailabiltyCost.Keys.Contains(dateKey))
                    priceAndAvailabiltyCost.Add(dateKey, priceCost);

                if (availableDate.AmountDetail != null && availableDate.AmountDetail[8] > 0)
                {
                    sellPriceValue = availableDate.AmountDetail[8];
                }

                // BasePrice (Sell in supplier currency)
                var adultBasePricingUnit = new AdultPricingUnit
                {
                    Price = sellPriceValue
                };
                var childBasePricingUnit = new ChildPricingUnit
                {
                    Price = sellPriceValue
                };

                var priceBase = new MoulinRougePriceAndAvailability
                {
                    AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                    CatalogDateID = availableDate.CatalogDateId,
                    TotalPrice = sellPriceValue * quantity,
                    IsSelected = false,
                    IsCapacityCheckRequired = true,
                    Capacity = availableDate.Stock,
                    PricingUnits = new List<PricingUnit>
                    {
                        adultBasePricingUnit,
                        childBasePricingUnit
                    },
                    MoulinRouge = new APIContextMoulinRouge
                    {
                        Amount = sellPriceValue * quantity,
                        BlocId = availableDate.BlocId,
                        CatalogDateId = availableDate.CatalogDateId,
                        RateId = availableDate.RateId,
                        CategoryId = availableDate.CategoryId,
                        ContingentId = availableDate.ContingentId,
                        DateStart = availableDate.DateStart,
                        DateEnd = availableDate.DateEnd,
                        ServiceType = System.Convert.ToString(availableDate.ServiceType),
                        ServiceTypeName = availableDate.ServiceTypeName,
                        FloorId = availableDate.FloorId,
                        ServiceOptionCode = availableDate.OptionCode
                    }
                };

                if (!priceAndAvailabiltyBase.Keys.Contains(dateKey))
                    priceAndAvailabiltyBase.Add(dateKey, priceBase);
            }

            //Price and Currency
            option.CostPrice = new Price
            {
                Amount = minCostPrice,
                Currency = new Currency { IsoCode = Constant.Eur, IsPostFix = true, Name = Constant.Euro, Symbol = "€" },
                DatePriceAndAvailabilty = priceAndAvailabiltyCost
            };
            option.BasePrice = new Price
            {
                Amount = minBasePrice,
                Currency = new Currency { IsoCode = Constant.Eur, IsPostFix = true, Name = Constant.Euro, Symbol = "€" },
                DatePriceAndAvailabilty = priceAndAvailabiltyBase
            };
            option.GateBasePrice = option.BasePrice.DeepCopy(); // Preparing GateBasePrice here as its needed in PriceRuleEngine

            #region Seasons

            var aPrice = new PolicyCategory
            {
                PerUnitPrice = new ActivityPrice
                {
                    BaseCurrencyCode = _currency,
                    BasePrice = minCostPrice,
                    IsPercent = false,
                    Price = minBasePrice
                },
                PolicyCategoryType = ConvertPassengerType(Constant.Ad),
                MinimumCustomers = 1,
                MaximumCustomers = 10
            };

            if (aPrice.PolicyCategoryType == PassengerType.Child)
            {
                aPrice.FromAge = 0;
                aPrice.ToAge = 5;
                aPrice.PerUnitPrice.IsAllowed = false;
            }
            if (aPrice.PolicyCategoryType == PassengerType.Adult)
            {
                aPrice.FromAge = 6;
                aPrice.ToAge = 999;
                aPrice.PerUnitPrice.IsAllowed = true;
            }

            var seasons = new List<ActivitySeason>
            {
                new ActivitySeason
                {
                    ActivityPolicies = new List<ActivityPolicy>
                    {
                        new ActivityPolicy
                        {
                            PolicyCategories = new List<PolicyCategory>
                            {
                                aPrice
                            }
                        }
                    }
                }
            };

            #endregion Seasons

            option.ActivitySeasons = seasons;

            //TODO: Check if HotelBeds activity has the equivalent of On Request.
            option.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
            option.Code = string.Empty;// modality.code;//??
            option.Description = null; //There's no equivalent in HB Activity

            //TODO: We need to determine a system to assign activity ID without making a DB call. Perhaps the ID will be assigned after convert!
            //option.ID = 0;

            var cancellationCost = new List<CancellationPrice>();
            option.CancellationPrices = cancellationCost;

            return option;
        }

        #endregion Convert to List<Activity>

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