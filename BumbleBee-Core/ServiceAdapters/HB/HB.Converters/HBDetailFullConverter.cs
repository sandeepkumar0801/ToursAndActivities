using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using ServiceAdapters.HB.Constants;
using ServiceAdapters.HB.HB.Converters.Contracts;
using ServiceAdapters.HB.HB.Entities;
using ServiceAdapters.HB.HB.Entities.ActivityDetail;
using ServiceAdapters.HB.HB.Entities.ActivityDetailFull;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Util;

namespace ServiceAdapters.HB.HB.Converters
{
    public class HBDetailFullConverter : ConverterBase, IHbDetailFullConverter
    {
        private string _currency;

        /// <summary>
        /// Convert API Result Entities to Isnago.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>
        public object Convert(object apiResponse, MethodType methodType, object criteria = null)
        {
            var result = (ActivityDetailRS)apiResponse;
            return result != null ? ConvertAvailabilityResult(result, criteria) : null;
        }

        #region Private Methods

        private Activity ConvertAvailabilityResult(ActivityDetailRS result, object criteria)
        {
            //var lstActivityRqPaxes = result.LstActivityRqPaxes;
            var HbCriteria = criteria as HotelbedCriteriaApitude;
            var serviceTicket = result.Activity;
            var activity = new Activity();
            var options = new List<ActivityOption>();
            if (serviceTicket != null)
            {
                _currency = serviceTicket.CurrencyISOCode;
                activity.Name = serviceTicket.Name;
                activity.RegionName = serviceTicket.Country?.Destinations?.FirstOrDefault() != null ? serviceTicket.Country.Destinations.FirstOrDefault()?.Name : Constant.RegionName;

                //Exclusions & Inclusions
                if (serviceTicket.Content.FeatureGroups != null)
                {
                    var inclusions = serviceTicket.Content.FeatureGroups.FindAll(f => f.Included != null && f.Included.Count > 0).ToList();
                    if (inclusions.Count > 0)
                    {
                        var sbInclusions = new StringBuilder();
                        foreach (var inc in inclusions)
                        {
                            foreach (var item in inc.Included)
                            {
                                sbInclusions.AppendLine(item.Description);
                            }
                        }
                        activity.Inclusions = sbInclusions.ToString();
                    }
                    //var exclusions = serviceTicket.Content.FeatureGroups.FindAll(f => f.Excluded != null && f.Excluded.Count > 0).ToList();
                    //if (exclusions.Count > 0)
                    //{
                    //    var sbExclusions = new StringBuilder();
                    //    foreach (var inc in exclusions)
                    //    {
                    //        foreach (var item in inc.Excluded)
                    //        {
                    //            sbExclusions.AppendLine(item.Description);
                    //        }
                    //    }
                    //    activity.Exclusions = sbExclusions.ToString();
                    //}
                }

                activity.ID = 0;
                activity.Code = serviceTicket.ActivityCode;

                var qAdultPrice = serviceTicket.AmountsFrom.Where(x => x.PaxType == PassengerType.Adult.ToString());

                var qChildPrice = serviceTicket.AmountsFrom.Where(x => x.PaxType == PassengerType.Child.ToString());

                activity.CategoryIDs = new List<int> { 1 };
                activity.DurationString = serviceTicket.HBActivityCode.Split(Constant.Dash)[0];
                activity.ProductOptions = new List<ProductOption>();

                var productOption = activity.ProductOptions.FirstOrDefault();
                productOption.TravelInfo = new TravelInfo();

                var operationDates = from m in serviceTicket.Modalities
                                     from r in m.Rates
                                     from rd in r.RateDetails
                                     from opd in rd.OperationDates
                                     where rd.OperationDates != null
                                     select opd;

                var minDate = operationDates.OrderBy(item => System.Convert.ToDateTime(item.From)).FirstOrDefault().From;
                var toDate = operationDates.OrderByDescending(item => System.Convert.ToDateTime(item.To)).FirstOrDefault().To;
                productOption.TravelInfo.StartDate = minDate.ToDateTimeExactV1();
                var difference = toDate.ToDateTimeExactV1() - minDate.ToDateTimeExactV1();
                productOption.TravelInfo.NumberOfNights = difference.Days;

                var childCount = 0;
                var adultCount = 0;
                var child = qChildPrice.FirstOrDefault();
                var adult = qAdultPrice.FirstOrDefault();
                productOption.TravelInfo.Ages = new Dictionary<PassengerType, int>();
                //foreach (var pax in lstActivityRqPaxes)
                //{
                //    if (child != null)
                //    {
                //        if (pax.Age >= child.AgeFrom && pax.Age <= child.AgeTo)
                //        {
                //            childCount++;
                //            productOption.TravelInfo.Ages.Add(PassengerType.Child, pax.Age);
                //        }
                //        else
                //        {
                //            adultCount++;
                //        }
                //    }
                //    else if (adult != null)
                //    {
                //        if (pax.Age >= adult.AgeFrom && pax.Age <= adult.AgeTo)
                //        {
                //            adultCount++;
                //        }
                //        else
                //        {
                //            childCount++;
                //        }
                //    }
                //    else
                //    {
                //        adultCount++;
                //    }
                //}

                // Add adult count
                productOption.TravelInfo.NoOfPassengers.Add(PassengerType.Adult, adultCount);

                // Add child count
                productOption.TravelInfo.NoOfPassengers.Add(PassengerType.Child, childCount);

                activity.FactsheetId = System.Convert.ToInt32(serviceTicket.Content.ContentId);

                int modalityCount = serviceTicket.Modalities.Count;
                for (int index = 0; index < modalityCount; index++)
                {
                    var option = CreateOption(serviceTicket.Modalities[index], productOption.TravelInfo);
                    option.Contract = new Isango.Entities.Contract();

                    var comments = new List<Isango.Entities.HotelBeds.Comment>();
                    var comment = new Isango.Entities.HotelBeds.Comment
                    {
                        Type = serviceTicket.Modalities[index].Comments.FirstOrDefault()?.Type,
                        CommentText = serviceTicket.Modalities[index].Comments.FirstOrDefault()?.Text
                    };
                    comments.Add(comment);
                    option.Contract.Comments = comments;

                    options.Add(option);
                }
                activity.ProductOptions.Add(productOption);
            }
            return activity;
        }

        private ActivityOption CreateOption(Modality modality, TravelInfo travelInfo)
        {
            if (modality == null)
                return null;
            var option = new ActivityOption();
            var cancellationCost = new List<CancellationPrice>();
            option.Name = modality.Name;

            //Price and Currency
            var costPrice = new Price();
            var costAmount = decimal.MaxValue;

            var pAndA = new Dictionary<DateTime, PriceAndAvailability>();
            var childPriceRange = modality.AmountsFrom.FindAll(c => c.PaxType.ToUpperInvariant().Equals(Constant.Child));
            var adultPriceRange = modality.AmountsFrom.Find(c => c.PaxType.ToUpperInvariant().Equals(Constant.Adult));
            foreach (var rates in modality.Rates)
            {
                foreach (var rateDetail in rates.RateDetails)
                {
                    foreach (var opreationDate in rateDetail.OperationDates)
                    {
                        var price = new DefaultPriceAndAvailability { AvailabilityStatus = AvailabilityStatus.AVAILABLE };
                        var qAdultPrice = rateDetail.PaxAmounts.Where(x => x.PaxType == PassengerType.Adult.ToString());

                        var qChildPrice = rateDetail.PaxAmounts.Where(x => x.PaxType == PassengerType.Child.ToString());

                        var adultPrice = new AdultPricingUnit()
                        {
                            Price = System.Convert.ToDecimal(qAdultPrice.OrderBy(item => System.Convert.ToDecimal(item.Amount)).FirstOrDefault().Amount)
                        };

                        var childPrice = new ChildPricingUnit();
                        if (qChildPrice.FirstOrDefault() != null)
                        {
                            childPrice.Price = System.Convert.ToDecimal(qChildPrice.OrderBy(item => System.Convert.ToDecimal(item.Amount)).FirstOrDefault().Amount);
                        }
                        else
                        {
                            childPrice.Price = adultPrice.Price;
                        }

                        price.MinDuration = System.Convert.ToInt32(rateDetail.MinimumDuration.Value);
                        price.MaxDuration = System.Convert.ToInt32(rateDetail.MaximumDuration.Value);

                        price.PricingUnits.Add(adultPrice);
                        price.PricingUnits.Add(childPrice);
                        price.TotalPrice = System.Convert.ToDecimal(rateDetail.TotalAmount.Amount);

                        if (price.TotalPrice < costAmount)
                            costAmount = price.TotalPrice;

                        price.IsSelected = false;
                        if (!pAndA.Keys.Contains(opreationDate.From.ToDateTimeExactV1()))
                            pAndA.Add(opreationDate.From.ToDateTimeExactV1(), price);

                        //Cancellation cost
                        var cancellationPoliciesCount = opreationDate.CancellationPolicies.Count;
                        if (opreationDate.CancellationPolicies != null && cancellationPoliciesCount > 0)
                        {
                            for (var i = 0; i < cancellationPoliciesCount; i++)
                            {
                                var charge = opreationDate.CancellationPolicies[i];
                                var cancPrice = new CancellationPrice();
                                var canPercentage = (charge.Amount / rateDetail.TotalAmount.Amount) * 100;
                                cancPrice.Percentage = canPercentage;
                                cancPrice.CancellationFromdate = charge.DateFrom;
                                if (i + 1 < cancellationPoliciesCount)
                                {
                                    cancPrice.CancellationToDate = opreationDate.CancellationPolicies[i + 1].DateFrom;
                                }
                                cancPrice.CancellationDateRelatedToOpreationDate = opreationDate.To.ToDateTimeExactV1();
                                cancPrice.CancellationAmount = System.Convert.ToDecimal(charge.Amount);
                                cancellationCost.Add(cancPrice);
                            }
                        }
                        option.CancellationPrices = cancellationCost;
                    }
                    option.RateKey = rateDetail.RateKey;
                }
            }

            costPrice.DatePriceAndAvailabilty = pAndA;

            costPrice.Amount = costAmount;
            costPrice.Currency = new Currency { Name = _currency, IsoCode = _currency };

            option.CostPrice = costPrice;

            #region Seasons

            var seasons = new List<ActivitySeason>();
            var season = new ActivitySeason();
            var operationDates = from r in modality.Rates
                                 from rd in r.RateDetails
                                 from opd in rd.OperationDates
                                 where rd.OperationDates != null
                                 select opd;
            var minDate = operationDates.OrderBy(item => System.Convert.ToDateTime(item.From)).FirstOrDefault().From;
            var toDate = operationDates.OrderByDescending(item => System.Convert.ToDateTime(item.To)).FirstOrDefault().To;

            season.FromDate = minDate.ToDateTimeExactV1();
            season.ToDate = toDate.ToDateTimeExactV1();

            var policies = new List<ActivityPolicy>();
            var policy = new ActivityPolicy();
            var aPrices = new List<PolicyCategory>();

            foreach (var tktPrice in modality.AmountsFrom)
            {
                var aPrice = new PolicyCategory
                {
                    PerUnitPrice = new ActivityPrice
                    {
                        BaseCurrencyCode = _currency,
                        BasePrice = System.Convert.ToDecimal(tktPrice.Amount)
                    },
                    MinimumCustomers = 1,
                    MaximumCustomers = 20,
                    PolicyCategoryType = ConvertPassengerType(tktPrice.PaxType.ToUpperInvariant())
                };
                if (aPrice.PolicyCategoryType == PassengerType.Child)
                {
                    aPrice.FromAge = System.Convert.ToInt32(tktPrice.AgeFrom);
                    aPrice.ToAge = System.Convert.ToInt32(tktPrice.AgeTo);
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

            option.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
            option.Code = modality.Code;
            option.Description = null;
            option.Id = 0;

            //if (modality.Questions != null)
            //{
            //    var lstQuestions = new List<ContractQuestion>();
            //    foreach (var que in modality.Questions)
            //    {
            //        var question = new ContractQuestion()
            //        {
            //            Code = que.Code,
            //            Name = que.Code,
            //            Description = que.Text,
            //            IsRequired = que.Required
            //        };
            //        lstQuestions.Add(question);
            //    }
            //    option.ContractQuestions = lstQuestions;
            //}

            return option;
        }

        private PassengerType ConvertPassengerType(string c)
        {
            return c.ToUpper(CultureInfo.InvariantCulture).Substring(0, 2).Equals(Constant.Ch) ? PassengerType.Child : PassengerType.Adult;
        }

        #endregion Private Methods
    }
}