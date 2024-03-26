using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using System.Text;
using Util;

namespace ServiceAdapters.GlobalTix.GlobalTix.Converters
{
    public abstract class AbstractActivityConverter : ConverterBase
    {
        protected Activity ConvertInternal(ActivityInfo activityData, List<TicketTypeDetail> paxTypeDetails, Dictionary<int, TicketTypeDetail> ticketDetails, ActivityInfoInputContext inputContext)
        {
            var act = new Activity();

            //------ Activity class variables assignment
            act.Id = activityData.Id.ToString();
            act.Schedule = activityData.OpHours;
            act.IsPaxDetailRequired = false;

            ActivityItinerary actItin = new ActivityItinerary
            {
                Description = activityData.Desc,
                Title = activityData.Title,
                Order = 0
            };
            act.Itineraries = new List<ActivityItinerary> { actItin };

            act.CategoryTypes = new List<ActivityCategoryType> { ActivityCategoryType.Attractions };
            foreach (IdentifierWithName catType in activityData.Types)
            {
                if (Constant.Mapper_CategoryType.ContainsKey(catType.Id))
                {
                    act.CategoryTypes.Add(Constant.Mapper_CategoryType[catType.Id]);
                }
            }

            //------ ActivityLite class variables assignment
            act.Code = activityData.Id.ToString();
            // TODO: Set value for DurationString. Need to check what value to set here.
            act.FactsheetId = inputContext.FactSheetId;
            //Need to confirm the currency
            //Changed
            act.CurrencyIsoCode = paxTypeDetails?.FirstOrDefault()?.Currency ?? string.Empty;

            //------ Product class variables assignment
            act.ProductType = ProductType.Ticket;
            act.ID = activityData.Id;
            SetImagePaths(act, activityData.ImagePath);

            act.ProductOptions = new List<ProductOption>();
            foreach (TicketTypeGroup ticketGroup in activityData.TicketTypeGroups)
            {
                
                bool hasSeriesAndEvents = (ticketGroup.ApplyCapacity != null && ticketGroup.ApplyCapacity != false);

                ActivityOption actOpt = new ActivityOption();
                actOpt.Id = ticketGroup.Id;
                actOpt.ServiceOptionId = inputContext?.ServiceOptionID ?? 0;
                actOpt.Code = ticketGroup.Id.ToString();
                actOpt.Name = ticketGroup.Name;
                actOpt.SupplierName = Constant.Const_GTSupplierName;
                actOpt.Description = string.Empty;
                actOpt.TravelInfo = new TravelInfo
                {
                    NoOfPassengers = inputContext.NoOfPassengers ?? new Dictionary<PassengerType, int>(),
                    StartDate = (inputContext.CheckinDate != null)
                                    ? new DateTime(inputContext.CheckinDate.Year, inputContext.CheckinDate.Month, inputContext.CheckinDate.Day)
                                        : new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                };

                // Add questions for TicketTypes
                //actOpt.ContractQuestions = GetContractQuestions(ticketGroup.Products, ticketDetails);
                //PickUp Locations
                //actOpt.PickupLocations = GetPickUpLocations(ticketGroup.Products, ticketDetails);

                //Create Pricing units for both Cost and Base Price. Uptill now only costPrice was created.
                PriceAndAvailability dftPriceAndAvail = new DefaultPriceAndAvailability();
                PriceAndAvailability basePriceAndAvail = new DefaultPriceAndAvailability();
                dftPriceAndAvail.PricingUnits = new List<PricingUnit>();
                basePriceAndAvail.PricingUnits = new List<PricingUnit>();

                actOpt.ProductIDs = new List<int?>();

                decimal totalAmount = 0;
                decimal totalBasePriceAmount = 0;
                StringBuilder strBldr = new StringBuilder();
                string ticketTypeIds = string.Empty;
                foreach (Identifier product in ticketGroup.Products)
                {
                    actOpt.ProductIDs.Add(product.Id);
                    ticketTypeIds += product.Id + ",";
                    TicketTypeDetail productPaxTypeDetail = paxTypeDetails?.FirstOrDefault(paxType => paxType.Id == product.Id);
                    if (productPaxTypeDetail == null)
                    {
                        // TODO: Log a message here
                        //Console.WriteLine($"TicketType {product.Id} not found for TicketGroup {ticketGroup.Id} in ActivityId {activityData.Id}");
                        continue;
                    }

                    if (productPaxTypeDetail.CancellationNotesSettings != null && productPaxTypeDetail.CancellationNotesSettings.Count > 0 && string.IsNullOrEmpty(actOpt.CancellationText))
                    {
                        var cancellationNote = productPaxTypeDetail.CancellationNotesSettings.Find(thisNote => thisNote.IsActive = true);
                        if (cancellationNote != null && !string.IsNullOrEmpty(cancellationNote.Value))
                        {
                            actOpt.Cancellable = true;
                            actOpt.CancellationText = cancellationNote.Value;
                        }
                        else
                        {
                            actOpt.Cancellable = false;
                            actOpt.CancellationText = Constant.CancellationTextNonRefundable;
                        }
                        actOpt.ApiCancellationPolicy = Util.SerializeDeSerializeHelper.Serialize(productPaxTypeDetail.CancellationNotesSettings);
                    }

                    PricingUnit prcUnit = GetPricingUnit(productPaxTypeDetail);
                    PricingUnit prcUnitForBasePrice = GetPricingUnit(productPaxTypeDetail, true);
                    if (prcUnit == null || prcUnitForBasePrice == null)
                    {
                        // TODO: Log a message here
                        //Console.WriteLine($"TicketType {product.Id} not found for TicketGroup {ticketGroup.Id} in ActivityId {activityData.Id}");
                        continue;
                    }

                    dftPriceAndAvail.PricingUnits.Add(prcUnit);
                    basePriceAndAvail.PricingUnits.Add(prcUnitForBasePrice);

                    if (prcUnit is PerPersonPricingUnit)
                    {
                        PerPersonPricingUnit perPaxPrcUnit = prcUnit as PerPersonPricingUnit;
                        strBldr.Append($"{(int)perPaxPrcUnit.PassengerType}:{productPaxTypeDetail.Id}:");
                        if (inputContext.NoOfPassengers == null)
                        {
                            actOpt.TravelInfo.NoOfPassengers.Add(perPaxPrcUnit.PassengerType, 1);
                        }

                        if (actOpt.TravelInfo.NoOfPassengers.ContainsKey(perPaxPrcUnit.PassengerType))
                        {
                            totalAmount += actOpt.TravelInfo.NoOfPassengers[perPaxPrcUnit.PassengerType] * prcUnit.Price;
                        }
                    }

                    if (prcUnitForBasePrice is PerPersonPricingUnit)
                    {
                        PerPersonPricingUnit perPaxPrcUnit = prcUnit as PerPersonPricingUnit;
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
                Price costPrice = new Price();
                // TODO: Assign proper values here
                costPrice.Amount = totalAmount;
                costPrice.Currency = new Currency { IsoCode = GetTicketsCurrencyCode(paxTypeDetails) };
                costPrice.DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>();

                Price basePrice = new Price();
                // TODO: Assign proper values here
                basePrice.Amount = totalBasePriceAmount;
                basePrice.Currency = new Currency { IsoCode = GetTicketsCurrencyCode(paxTypeDetails) };
                basePrice.DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>();

                if (hasSeriesAndEvents)
                {
                    DateTime fetchStart = (inputContext.CheckinDate != null)
                                                ? new DateTime(inputContext.CheckinDate.Year, inputContext.CheckinDate.Month, inputContext.CheckinDate.Day)
                                                : new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    DateTime endDateTime = fetchStart.AddDays(inputContext.Days2Fetch);
                    DateTime fetchEnd = new DateTime(endDateTime.Year, endDateTime.Month, endDateTime.Day, 23, 59, 59, 999);

                    foreach (Identifier series in ticketGroup.Series)
                    {
                        //Series seriesInstance = GetSeriesForSeriesId(activityData.Series, series.Id);
                        SeriesInner seriesInstance = null;
                        foreach (var innerSeries in activityData.Series)
                        {
                            seriesInstance= innerSeries.Series.FirstOrDefault(actSr => actSr.Id == series.Id);
                            if (seriesInstance != null)
                            {
                                break;
                            }
                        }
                        
                        if (seriesInstance == null)
                        {
                            // TODO: Log a message here
                            //Console.WriteLine($"Series {seriesInstance.Id} not found for TicketGroup {ticketGroup.Id} in ActivityId {activityData.Id}");
                            continue;
                        }

                        // If the series start and end dates fall within range of number of days to fetch, then
                        // get availability information for the event.
                        if (seriesInstance.Start <= fetchStart)// && seriesInstance.End >= fetchEnd)
                        {
                            foreach (EventInner evt in seriesInstance.Events)
                            {
                                if (evt.Time >= fetchStart && evt.Time <= fetchEnd)
                                {
                                    PriceAndAvailability datePrcAndAvail = (PriceAndAvailability)dftPriceAndAvail.Clone();
                                    datePrcAndAvail.AvailabilityStatus = (evt.Available > 0) ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE;
                                    datePrcAndAvail.TourDepartureId = System.Convert.ToInt32(evt.Id);
                                    costPrice.DatePriceAndAvailabilty.Add(evt.Time, datePrcAndAvail);

                                    PriceAndAvailability baseDatePrcAndAvail = (PriceAndAvailability)basePriceAndAvail.Clone();
                                    baseDatePrcAndAvail.AvailabilityStatus = (evt.Available > 0) ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE;
                                    baseDatePrcAndAvail.TourDepartureId = System.Convert.ToInt32(evt.Id);
                                    basePrice.DatePriceAndAvailabilty.Add(evt.Time, baseDatePrcAndAvail);
                                }
                            }
                        }
                    }

                    // If no series is applicable (basePrice.DatePriceAndAvailabilty is empty), then log a message and continue with next TicketTypeGroup (that is, activity option).
                    if (costPrice.DatePriceAndAvailabilty.Count == 0)
                    {
                        // TODO: Log a message - No applicable series found for activity option
                        continue;
                    }
                }
                else
                {
                    dftPriceAndAvail.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                    basePriceAndAvail.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                    for (var date = 0; date < inputContext.Days2Fetch; date++)
                    {
                        costPrice.DatePriceAndAvailabilty.Add((inputContext.CheckinDate != null) ? inputContext.CheckinDate.AddDays(date) : DateTime.Now, dftPriceAndAvail);
                        basePrice.DatePriceAndAvailabilty.Add((inputContext.CheckinDate != null) ? inputContext.CheckinDate.AddDays(date) : DateTime.Now, basePriceAndAvail);
                    }
                }

                //actOpt.BasePrice = basePrice;
                actOpt.CostPrice = costPrice.DeepCopy();
                actOpt.BasePrice = basePrice.DeepCopy();

                // When booking a ticket, GlobalTix API needs ids of ticket types. As of now, in PricingUnit and its derived classes, there
                // is no provision to store this id. Therefore, storing it in ProductOption.RateKey as a workaround
                if (strBldr.Length > 0)
                {
                    // Decrease length to get rid of last ':' character
                    strBldr.Length--;
                    actOpt.RateKey = strBldr.ToString();
                }

                if (actOpt?.CostPrice?.Amount != null && actOpt?.CostPrice?.Amount > 0)
                {
                    actOpt.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                }
                //------
                act.ProductOptions.Add(actOpt);
            }
            act.ApiType = APIType.GlobalTix;

            return act as Activity;
        }

        #region Private Methods

        private void AddPricingUnit(PriceAndAvailability dftPriceAndAvail, TicketType tktType)
        {
            string psgrTypeStr = tktType?.Variation?.Name;
            if (psgrTypeStr == null)
            {
                return;
            }

            if (Constant.Mapper_PassengerType.TryGetValue(psgrTypeStr, out PassengerType psgrType))
            {
                PricingUnit prcUt = GetPricingUnitInstance(psgrType);
                if (prcUt != null)
                {
                    prcUt.Price = tktType.Price;
                    dftPriceAndAvail.PricingUnits.Add(prcUt);
                }
            }
        }

        private List<ContractQuestion> GetContractQuestions(List<Identifier> tktIds, Dictionary<int, TicketTypeDetail> tktDtls)
        {
            if (tktIds == null || tktIds.Count <= 0 || tktDtls == null || tktDtls.Count <= 0)
            {
                return null;
            }

            IEnumerable<List<ContractQuestion>> contractQuestions =
                tktIds.Where(tktId => tktDtls.ContainsKey(tktId.Id) && tktDtls[tktId.Id].Questions != null && tktDtls[tktId.Id].Questions.Count > 0)
                    .Select(tktId => ConvertQuestionsToContractQuestions(tktDtls[tktId.Id].Questions, Constant.QUESTION));

            List<ContractQuestion> optionQuestions = new List<ContractQuestion>();
            List<string> questionCodes = new List<string>();
            foreach (List<ContractQuestion> questions in contractQuestions)
            {
                foreach (ContractQuestion q in questions)
                {
                    if (q != null && questionCodes.Contains(q.Code) == false)
                    {
                        optionQuestions.Add(q);
                        questionCodes.Add(q.Code);
                    }
                }
            }

            return optionQuestions;
        }

        /// <summary>
        /// Get PickUp Locations
        /// </summary>
        /// <param name="tktIds"></param>
        /// <param name="tktDtls"></param>
        /// <returns></returns>
        private Dictionary<int, string> GetPickUpLocations(List<Identifier> tktIds, Dictionary<int, TicketTypeDetail> tktDtls)
        {
            if (tktIds == null || tktIds.Count <= 0 || tktDtls == null || tktDtls.Count <= 0)
            {
                return null;
            }

            IEnumerable<List<ContractQuestion>> contractQuestions =
                tktIds.Where(tktId => tktDtls.ContainsKey(tktId.Id) && tktDtls[tktId.Id].Questions != null && tktDtls[tktId.Id].Questions.Count > 0)
                    .Select(tktId => ConvertQuestionsToContractQuestions(tktDtls[tktId.Id].Questions, Constant.PICKUP));

            var pickUpDetails = new Dictionary<int, string>();
            List<string> questionCodes = new List<string>();
            foreach (List<ContractQuestion> questions in contractQuestions)
            {
                foreach (ContractQuestion q in questions)
                {
                    if (q != null && !string.IsNullOrEmpty(q.Answer) && questionCodes.Contains(q.Code) == false)
                    {
                        string[] splitOptions = q.Answer.Split('|');
                        if (splitOptions.Count() > 1)
                        {
                            foreach (var item in splitOptions)
                            {
                                pickUpDetails.Add(Math.Abs(Guid.NewGuid().GetHashCode()), item.ToString());
                            }
                        }
                    }
                }
            }
            return pickUpDetails;
        }

        private PricingUnit GetPricingUnit(TicketTypeDetail paxTypeDetail, bool getBasePricePricingUnit = false)
        {
            string psgrTypeStr = paxTypeDetail?.Variation?.Name;
            if (psgrTypeStr == null || Constant.Mapper_PassengerType.TryGetValue(psgrTypeStr, out PassengerType psgrType) == false)
            {
                return null;
            }

            PricingUnit prcUt = GetPricingUnitInstance(psgrType);
            if (prcUt != null)
            {
                prcUt.Price = getBasePricePricingUnit ? paxTypeDetail.OriginalPrice : paxTypeDetail.PayableAmount;
                prcUt.Currency = paxTypeDetail.Currency;
                prcUt.MinimumSellingPrice = string.IsNullOrEmpty(paxTypeDetail.MinimumSellingPrice) ? 0 : paxTypeDetail.MinimumSellingPrice.ToDecimal();
                prcUt.IsMinimumSellingPriceRestrictionApplicable = string.IsNullOrEmpty(paxTypeDetail.MinimumSellingPrice) ? false : true;
            }

            return prcUt;
        }

        private string GetTicketsCurrencyCode(List<TicketTypeDetail> paxTypeDetails)
        {
            string currCode = string.Empty;
            foreach (TicketTypeDetail paxType in paxTypeDetails)
            {
                if (currCode.Equals(paxType.Currency) == false)
                {
                    // TODO: Log a warning here
                }
                currCode = paxType.Currency;
            }

            return currCode;
        }

        #endregion Private Methods
    }
}