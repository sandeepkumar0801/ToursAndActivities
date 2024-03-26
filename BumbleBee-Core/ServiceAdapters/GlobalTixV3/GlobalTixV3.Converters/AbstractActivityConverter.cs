using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.GlobalTixV3;
using ServiceAdapters.GlobalTixV3.Constants;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Converters
{
    public abstract class AbstractActivityConverter : ConverterBase
    {
        protected Activity ConvertInternal(ActivityInfoInputContext inputContext)
        {
            var activityInfo = inputContext.ActivityInfo;//activityData
            var paxTypeDetails = inputContext.PaxTypeDetails;
            var productOption = inputContext.ProductOption;

            var currency = productOption?.FirstOrDefault()?.Currency;

            var act = CreateActivity(activityInfo, inputContext.FactSheetId, currency);
            act.ProductOptions = new List<ProductOption>();


            foreach (var productInnerOption in productOption)
            {
                var isangoAPIMapping = inputContext.GlobalTixV3Mapping;
                //filter mapping by option
                isangoAPIMapping = isangoAPIMapping?.Where(x => x.SupplierCode == productInnerOption.Id.ToString())?.ToList();

                var getAllPaxIdofOption = productInnerOption?.TicketType?.Select(x => x.Id)?.ToList();
                    if (getAllPaxIdofOption == null)
                    {
                        continue;
                    }
                    var availableDataResultFilter = paxTypeDetails?.Where(x => getAllPaxIdofOption.Contains(x.ticketTypeID))?.ToList();
                    if (availableDataResultFilter == null)
                    {
                        continue;
                    }


                    var isSeriesorEventScenario = availableDataResultFilter.Any(x => x?.Id > 0);
                    if (!isSeriesorEventScenario)
                    {

                        var actOpt = returnActivityData(activityInfo, paxTypeDetails, productInnerOption, inputContext, isangoAPIMapping);
                        act.ProductOptions.Add(actOpt);
                    }
                    else
                    {

                        var actOpt = returnActivitySeriesLst(activityInfo, paxTypeDetails, productInnerOption, inputContext, isangoAPIMapping);
                        act.ProductOptions.AddRange(actOpt);
                    }
                
            }
            return act as Activity;


        }

       


        private List<ActivityOption> returnActivitySeriesLst(Data activityInfo, List<ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.DatumAvailability>
            paxTypeDetails, GlobalTix.Entities.RequestResponseModels.ProductOption.Datum productInnerOption,
            ActivityInfoInputContext inputContext, List<GlobalTixV3Mapping> globalTixV3Mapping)
        {
            var currency = productInnerOption?.Currency;
            var lst = new List<ActivityOption>();

            //option :ticketTypeid of adult, child etc
            var getAllPaxofOption = productInnerOption?.TicketType?.Select(x => x.Id)?.ToList();

            //"12:00","15:00"
            var multipleTimeSlots = productInnerOption?.TimeSlot;
            if (multipleTimeSlots != null)
            {
                foreach (var timeSlot in multipleTimeSlots)
                {
                    var actOpt = createActivityOption(inputContext, productInnerOption);
                    actOpt.ProductIDs = new List<int?>();

                    //var apiContractQuestions = productInnerOption?.Questions?.Where(x => x.Type.ToLower() != "date")?.ToList();
                    var apiContractQuestions = productInnerOption?.Questions?.ToList();
                    //Questions
                    //apiContractQuestions = apiContractQuestions?.Where(x => x.Type.ToLower() != "option")?.ToList();
                    if (apiContractQuestions != null && apiContractQuestions.Count > 0)
                    {
                        actOpt.ContractQuestionForGlobalTix3 = GetContractQuestions(apiContractQuestions);
                    }
                    

                    CancellationNotes(productInnerOption, actOpt);

                    //Update vales according to TimeSlots
                    actOpt.Name = actOpt.Name + " @ " + timeSlot;
                    actOpt.StartTime = System.Convert.ToDateTime(timeSlot).TimeOfDay;
                    actOpt.Id = Math.Abs(Guid.NewGuid().GetHashCode());
                    Price costPrice = new Price
                    {
                        Currency = new Currency { IsoCode = currency },
                        DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
                    };

                    Price basePrice = new Price
                    {
                        Currency = new Currency { IsoCode = currency },
                        DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
                    };
                    for (var date = 0; date < inputContext.Days2Fetch; date++)
                    {
                       
                        //Create Pricing units for both Cost and Base Price. Uptill now only costPrice was created.
                        PriceAndAvailability costPriceAndAvail = new DefaultPriceAndAvailability
                        {
                            AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE
                        };

                        PriceAndAvailability baseSellPriceAndAvail = new DefaultPriceAndAvailability
                        {
                            AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE
                        };
                        var availableDataResult = new List<DatumAvailability>();
                        if (paxTypeDetails != null && paxTypeDetails.Count > 0)
                        {
                            //Filter data by Date
                            availableDataResult = paxTypeDetails?.Where(x => x.Time.Date == inputContext.CheckinDate.AddDays(date).Date)?.ToList();
                            if (availableDataResult != null)
                            {
                                //Filter data by ticketTypeID-> multiple result-adult,child
                                availableDataResult = availableDataResult?.Where(x => getAllPaxofOption.Contains(x.ticketTypeID))?.ToList();
                                if (availableDataResult != null)
                                {
                                    //Filter data by TimeSlot->multiple result-adult,child
                                    availableDataResult = availableDataResult?.Where(x => (x.Time.TimeOfDay.ToString().Substring(0, x.Time.TimeOfDay.ToString().Length - 3))== timeSlot)?.ToList();

                                    if (availableDataResult == null || availableDataResult.Count==0)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                            }
                            else
                            {
                                continue;
                            }
                        }


                        costPriceAndAvail.PricingUnits = new List<PricingUnit>();
                        baseSellPriceAndAvail.PricingUnits = new List<PricingUnit>();
                        string ticketTypeIds = string.Empty;
                        StringBuilder strBldr = new StringBuilder();
                        decimal totalAmount = 0;
                        decimal totalBasePriceAmount = 0;
                        foreach (var availableData in availableDataResult)
                        {
                            //only give 1 paxtype data in loop
                            var ticketTypeSingle = productInnerOption?.TicketType?.Where(x => x.Id == availableData.ticketTypeID)?.FirstOrDefault();

                            var productPaxTypeDetail = ticketTypeSingle;
                            if (productPaxTypeDetail == null)
                            {
                                continue;
                            }

                            if (availableData != null && availableData?.Available > 0)
                            {
                                costPriceAndAvail.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                                baseSellPriceAndAvail.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                                //blocked dates
                                var blockedDates = activityInfo?.BlockedDate;
                                if (blockedDates != null && blockedDates.Count > 0)
                                {
                                    foreach (var itemBlocked in blockedDates)
                                    {
                                        if (itemBlocked.Date == availableData.Time)
                                        {
                                            costPriceAndAvail.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                                            baseSellPriceAndAvail.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                                            break;
                                        }
                                    }
                                }


                                costPriceAndAvail.IsCapacityCheckRequired = true;
                                costPriceAndAvail.Capacity = availableData.Available;
                                //Update value according to TimeSlot
                                costPriceAndAvail.TourDepartureId = System.Convert.ToInt32(availableData.Id);


                                baseSellPriceAndAvail.IsCapacityCheckRequired = true;
                                baseSellPriceAndAvail.Capacity = availableData.Available;
                                //Update value according to TimeSlot
                                baseSellPriceAndAvail.TourDepartureId = System.Convert.ToInt32(availableData.Id);
                            }
                            else
                            {
                                availableData.Available = 0;
                                costPriceAndAvail.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                                costPriceAndAvail.IsCapacityCheckRequired = false;

                                baseSellPriceAndAvail.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                                baseSellPriceAndAvail.IsCapacityCheckRequired = false;
                            }

                            actOpt.ProductIDs.Add(productPaxTypeDetail.Id);
                            ticketTypeIds += productPaxTypeDetail.Id + ",";

                            PricingUnit prcUnitCostPrice = GetPricingUnit(ticketTypeSingle, currency, availableData.Available, globalTixV3Mapping, false);
                            PricingUnit prcUnitForBasePrice = GetPricingUnit(ticketTypeSingle, currency, availableData.Available, globalTixV3Mapping, true);
                            if (prcUnitCostPrice == null || prcUnitForBasePrice == null)
                            {
                                // TODO: Log a message here
                                //Console.WriteLine($"TicketType {product.Id} not found for TicketGroup {ticketGroup.Id} in ActivityId {activityData.Id}");
                                continue;
                            }

                            costPriceAndAvail.PricingUnits.Add(prcUnitCostPrice);
                            baseSellPriceAndAvail.PricingUnits.Add(prcUnitForBasePrice);

                            if (prcUnitCostPrice is PerPersonPricingUnit)
                            {
                                PerPersonPricingUnit perPaxPrcUnit = prcUnitCostPrice as PerPersonPricingUnit;
                                strBldr.Append($"{(int)perPaxPrcUnit.PassengerType}:{productPaxTypeDetail.Id}:");
                                if (inputContext.NoOfPassengers == null)
                                {
                                    actOpt.TravelInfo.NoOfPassengers.Add(perPaxPrcUnit.PassengerType, 1);
                                }

                                if (actOpt.TravelInfo.NoOfPassengers.ContainsKey(perPaxPrcUnit.PassengerType))
                                {
                                    totalAmount += actOpt.TravelInfo.NoOfPassengers[perPaxPrcUnit.PassengerType] * prcUnitCostPrice.Price;
                                }
                            }

                            if (prcUnitForBasePrice is PerPersonPricingUnit)
                            {
                                PerPersonPricingUnit perPaxPrcUnit = prcUnitForBasePrice as PerPersonPricingUnit;
                                if (actOpt.TravelInfo.NoOfPassengers.ContainsKey(perPaxPrcUnit.PassengerType))
                                {
                                    totalBasePriceAmount += actOpt.TravelInfo.NoOfPassengers[perPaxPrcUnit.PassengerType] * prcUnitForBasePrice.Price;
                                }
                            }


                          
                        }

                        if (!string.IsNullOrEmpty(ticketTypeIds))
                        {
                            ticketTypeIds = ticketTypeIds.Substring(0, ticketTypeIds.Length - 1);
                            actOpt.TicketTypeIds = ticketTypeIds;
                        }
                        costPrice.Amount = totalAmount;
                        basePrice.Amount = totalBasePriceAmount;
                        //set here
                        if (!costPrice.DatePriceAndAvailabilty.ContainsKey(inputContext.CheckinDate.AddDays(date).Date))
                        {
                            costPrice.DatePriceAndAvailabilty.Add(inputContext.CheckinDate.AddDays(date).Date, costPriceAndAvail);
                        }
                        if (!basePrice.DatePriceAndAvailabilty.ContainsKey(inputContext.CheckinDate.AddDays(date).Date))
                        {
                            basePrice.DatePriceAndAvailabilty.Add(inputContext.CheckinDate.AddDays(date).Date, baseSellPriceAndAvail);
                        }
                        if (strBldr.Length > 0)
                        {
                            // Decrease length to get rid of last ':' character
                            strBldr.Length--;
                            actOpt.RateKey = strBldr.ToString();
                        }
                        var isAvailable = basePrice.DatePriceAndAvailabilty.Any(x => x.Value.AvailabilityStatus == AvailabilityStatus.AVAILABLE);
                        actOpt.AvailabilityStatus = isAvailable ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE;
                    }
                    actOpt.CostPrice = costPrice.DeepCopy();
                    actOpt.BasePrice = basePrice.DeepCopy();

                    lst.Add(actOpt);
                }
            }
            return lst;
        }

        private ActivityOption returnActivityData(Data activityInfo, List<ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.DatumAvailability>
               paxTypeDetails, GlobalTix.Entities.RequestResponseModels.ProductOption.Datum productInnerOption,
               ActivityInfoInputContext inputContext, List<GlobalTixV3Mapping> globalTixV3Mapping)
        {

            var currency = productInnerOption?.Currency;

            Price costPrice = new Price
            {
                Currency = new Currency { IsoCode = currency },
                DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
            };

            Price basePrice = new Price
            {
                Currency = new Currency { IsoCode = currency },
                DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
            };
           


            var actOpt = createActivityOption(inputContext, productInnerOption);

            actOpt.ProductIDs = new List<int?>();

            //var apiContractQuestions = productInnerOption?.Questions?.Where(x => x.Type.ToLower() != "date")?.ToList();
            var apiContractQuestions = productInnerOption?.Questions?.ToList();
            //Questions

            if (apiContractQuestions != null && apiContractQuestions.Count > 0)
            {
                actOpt.ContractQuestionForGlobalTix3 = GetContractQuestions(apiContractQuestions);
            }
            
            CancellationNotes(productInnerOption, actOpt);


            for (var date = 0; date < inputContext.Days2Fetch + 1; date++)
            {
                //Create Pricing units for both Cost and Base Price. Uptill now only costPrice was created.
                PriceAndAvailability costPriceAndAvail = new DefaultPriceAndAvailability
                {
                    AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE
                };

                PriceAndAvailability baseSellPriceAndAvail = new DefaultPriceAndAvailability
                {
                    AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE
                };
                StringBuilder strBldr = new StringBuilder();
                costPriceAndAvail.PricingUnits = new List<PricingUnit>();
                baseSellPriceAndAvail.PricingUnits = new List<PricingUnit>();
                string ticketTypeIds = string.Empty;
                decimal totalAmount = 0;
                decimal totalBasePriceAmount = 0;

                foreach (var ticketTypeSingle in productInnerOption.TicketType)
                {
                    var productPaxTypeDetail = ticketTypeSingle;
                    if (productPaxTypeDetail == null)
                    {
                        continue;
                    }
                    var availableData = new DatumAvailability
                    {
                        Available = 0
                    };
                    if (paxTypeDetails != null && paxTypeDetails.Count > 0)
                    {
                        availableData = paxTypeDetails.Where(x => x.ticketTypeID == productPaxTypeDetail.Id && x.Time.Date == inputContext.CheckinDate.AddDays(date).Date)?.FirstOrDefault();
                    }



                    if (availableData != null && availableData?.Available > 0)
                    {

                        costPriceAndAvail.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                        baseSellPriceAndAvail.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                        //blocked dates
                        var blockedDates = activityInfo?.BlockedDate;
                        if (blockedDates != null && blockedDates.Count > 0)
                        {
                            foreach (var itemBlocked in blockedDates)
                            {
                                if (itemBlocked.Date == availableData.Time)
                                {
                                    costPriceAndAvail.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                                    baseSellPriceAndAvail.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                                    break;
                                }
                            }
                        }
                        costPriceAndAvail.IsCapacityCheckRequired = true;
                        costPriceAndAvail.Capacity = availableData.Available;
                        baseSellPriceAndAvail.IsCapacityCheckRequired = true;
                        baseSellPriceAndAvail.Capacity = availableData.Available;
                    }
                    else
                    {
                        availableData = new DatumAvailability();
                        availableData.Available = 0;

                        costPriceAndAvail.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                        costPriceAndAvail.IsCapacityCheckRequired = false;

                        baseSellPriceAndAvail.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                        baseSellPriceAndAvail.IsCapacityCheckRequired = false;
                    }

                    actOpt.ProductIDs.Add(productPaxTypeDetail.Id);
                    ticketTypeIds += productPaxTypeDetail.Id + ",";

                    PricingUnit prcUnitCostPrice = GetPricingUnit(ticketTypeSingle, currency, availableData.Available, globalTixV3Mapping, false);
                    PricingUnit prcUnitForBasePrice = GetPricingUnit(ticketTypeSingle, currency, availableData.Available, globalTixV3Mapping, true);
                    if (prcUnitCostPrice == null || prcUnitForBasePrice == null)
                    {
                        // TODO: Log a message here
                        //Console.WriteLine($"TicketType {product.Id} not found for TicketGroup {ticketGroup.Id} in ActivityId {activityData.Id}");
                        continue;
                    }

                    costPriceAndAvail.PricingUnits.Add(prcUnitCostPrice);
                    baseSellPriceAndAvail.PricingUnits.Add(prcUnitForBasePrice);

                    if (prcUnitCostPrice is PerPersonPricingUnit)
                    {
                        PerPersonPricingUnit perPaxPrcUnit = prcUnitCostPrice as PerPersonPricingUnit;
                        strBldr.Append($"{(int)perPaxPrcUnit.PassengerType}:{productPaxTypeDetail.Id}:");
                        if (inputContext.NoOfPassengers == null)
                        {
                            actOpt.TravelInfo.NoOfPassengers.Add(perPaxPrcUnit.PassengerType, 1);
                        }

                        if (actOpt.TravelInfo.NoOfPassengers.ContainsKey(perPaxPrcUnit.PassengerType))
                        {
                            totalAmount += actOpt.TravelInfo.NoOfPassengers[perPaxPrcUnit.PassengerType] * prcUnitCostPrice.Price;
                        }
                    }

                    if (prcUnitForBasePrice is PerPersonPricingUnit)
                    {
                        PerPersonPricingUnit perPaxPrcUnit = prcUnitForBasePrice as PerPersonPricingUnit;
                        if (actOpt.TravelInfo.NoOfPassengers.ContainsKey(perPaxPrcUnit.PassengerType))
                        {
                            totalBasePriceAmount += actOpt.TravelInfo.NoOfPassengers[perPaxPrcUnit.PassengerType] * prcUnitForBasePrice.Price;
                        }
                    }


                    if (!string.IsNullOrEmpty(ticketTypeIds))
                    {
                        ticketTypeIds = ticketTypeIds.Substring(0, ticketTypeIds.Length - 1);
                        actOpt.TicketTypeIds = ticketTypeIds;
                    }
                    costPrice.Amount = totalAmount;
                    basePrice.Amount = totalBasePriceAmount;
                }
                costPrice.DatePriceAndAvailabilty.Add((inputContext.CheckinDate != null) ? inputContext.CheckinDate.AddDays(date) : DateTime.Now, costPriceAndAvail);
                basePrice.DatePriceAndAvailabilty.Add((inputContext.CheckinDate != null) ? inputContext.CheckinDate.AddDays(date) : DateTime.Now, baseSellPriceAndAvail);
                if (strBldr.Length > 0)
                {
                    // Decrease length to get rid of last ':' character
                    strBldr.Length--;
                    actOpt.RateKey = strBldr.ToString();
                }

                var isAvailable = basePrice.DatePriceAndAvailabilty.Any(x => x.Value.AvailabilityStatus == AvailabilityStatus.AVAILABLE);
                actOpt.AvailabilityStatus = isAvailable ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE;
            }
            actOpt.CostPrice = costPrice.DeepCopy();
            actOpt.BasePrice = basePrice.DeepCopy();
            return actOpt;
        }



        private ActivityOption CancellationNotes(GlobalTix.Entities.RequestResponseModels.ProductOption.Datum productInnerOption,
            ActivityOption actOpt)
        {

            if (productInnerOption.CancellationNotes != null && productInnerOption.CancellationNotes.Count > 0)//&& string.IsNullOrEmpty(actOpt.CancellationText))
            {
                var cancellationNote = productInnerOption?.CancellationNotes;
                if (productInnerOption?.IsCancellable == true)
                {
                    actOpt.Cancellable = true;
                    if (cancellationNote != null)
                    {
                        actOpt.CancellationText = cancellationNote?.FirstOrDefault();
                    }
                }
                else
                {
                    actOpt.Cancellable = false;
                    actOpt.CancellationText = Constant.CancellationTextNonRefundable;
                }
                actOpt.ApiCancellationPolicy = Util.SerializeDeSerializeHelper.Serialize(productInnerOption.CancellationNotes);
            }
            return actOpt;
        }

        private Activity CreateActivity(GlobalTix.Entities.RequestResponseModels.Data activityInfo,
            int factsheetId,string currency)
        {
            var act = new Activity
            {
                ////------ Activity class variables assignment
                Id = activityInfo.Id.ToString(),
                //act.Schedule = activityInfo.OpHour;//operatingHours
                IsPaxDetailRequired = false
            };

            ActivityItinerary actItin = new ActivityItinerary
            {
                Description = activityInfo.Description,
                Title = activityInfo.Name,
                Order = 0
            };
            act.Itineraries = new List<ActivityItinerary> { actItin };

            act.CategoryTypes = new List<ActivityCategoryType> { ActivityCategoryType.Attractions };

            ////------ ActivityLite class variables assignment
            act.Code = activityInfo.Id.ToString();
            //// TODO: Set value for DurationString. Need to check what value to set here.
            act.FactsheetId = factsheetId;
            ////Need to confirm the currency
            ////Changed
            act.CurrencyIsoCode = currency ?? string.Empty;

            ////------ Product class variables assignment
            act.ProductType = ProductType.Ticket;
            act.ID = activityInfo.Id;
            return act;
        }

        private ActivityOption createActivityOption(ActivityInfoInputContext inputContext,
            GlobalTix.Entities.RequestResponseModels.ProductOption.Datum productInnerOption)
        {

            var actOpt = new ActivityOption
            {
                Id = productInnerOption.Id,
                ServiceOptionId = inputContext?.ServiceOptionID ?? 0,
                Code = productInnerOption.Id.ToString(),
                Name = productInnerOption.Name,
                SupplierName = Constant.Const_GTSupplierName,
                Description = productInnerOption?.Description,

                TravelInfo = new TravelInfo
                {
                    NoOfPassengers = inputContext.NoOfPassengers ?? new Dictionary<PassengerType, int>(),
                    StartDate = (inputContext.CheckinDate != null)
                                ? new DateTime(inputContext.CheckinDate.Year, inputContext.CheckinDate.Month, inputContext.CheckinDate.Day)
                                    : new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                }
            };
            return actOpt;
        }

        #region Private Methods
        private PricingUnit GetPricingUnit(GlobalTix.Entities.RequestResponseModels.ProductOption.Tickettype paxTypeDetail,
            string currency, int totalCapacity, List<GlobalTixV3Mapping> globalTixV3Mapping,
            bool getBasePricePricingUnit = false)
        {
            string psgrTypeStr = paxTypeDetail?.Name?.ToUpper();
            if (psgrTypeStr == null)
            {
                return null;
            }
            var psgrType = globalTixV3Mapping.Where(x => x.AgeGroupCode.ToUpper() == psgrTypeStr).FirstOrDefault().PassengerType;

            
            PricingUnit prcUt = GetPricingUnitInstance(psgrType);
            if (prcUt != null)
            {
                var priceData = (getBasePricePricingUnit ? paxTypeDetail.OriginalPrice : paxTypeDetail.NettPrice);
                prcUt.Price = (decimal)(Math.Round(priceData, 2));
                prcUt.Currency = currency;
                var minSellPrice = paxTypeDetail.MinimumSellingPrice;
                prcUt.MinimumSellingPrice = string.IsNullOrEmpty(minSellPrice.ToString()) ? 0 : (decimal)(Math.Round(System.Convert.ToDecimal(minSellPrice), 2));
                prcUt.IsMinimumSellingPriceRestrictionApplicable = string.IsNullOrEmpty(paxTypeDetail.MinimumSellingPrice?.ToString()) ? false : true;
                prcUt.TotalCapacity = totalCapacity;
            }
            return prcUt;
        }


        private List<ContractQuestionsForGlobalTix3> GetContractQuestions(List<GlobalTix.Entities.RequestResponseModels.ProductOption.Question> lstQuestions)
        {
            var optionQuestions = new List<ContractQuestionsForGlobalTix3>();
            if (lstQuestions != null && lstQuestions.Count > 0)
            {
                foreach (var questions in lstQuestions)
                {
                    if (questions.Id != null && questions.Id > 0)
                    {
                        var optionQuestion = new ContractQuestionsForGlobalTix3
                        {
                            Id = System.Convert.ToString(questions.Id),
                            Optional = questions.Optional == true ? false : true,
                            Question = questions.QuestionData,
                            Type = questions.Type
                        };
                        var lstAnswers = new List<AnswerOptionForGlobalTix3>();
                        if (questions.Options!=null && questions.Options.Count>0)
                        {
                            foreach (var itemQuestion in questions.Options)
                            {
                                var itemAnnswer = new AnswerOptionForGlobalTix3
                                {
                                    Label = itemQuestion,
                                    Value = itemQuestion
                                };
                                lstAnswers.Add(itemAnnswer);
                            }
                            optionQuestion.AnswerOptions = lstAnswers;
                        }
                        optionQuestions.Add(optionQuestion);
                    }
                }
            }

            return optionQuestions;
        }

       
        #endregion Private Methods
    }
}