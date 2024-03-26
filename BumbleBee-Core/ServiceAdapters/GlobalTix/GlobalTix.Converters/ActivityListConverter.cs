using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.GlobalTix;
using ServiceAdapters.GlobalTix.GlobalTix.Converters.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using Constant = ServiceAdapters.GlobalTix.Constants.Constant;

namespace ServiceAdapters.GlobalTix.GlobalTix.Converters
{
    //public class ActivityListConverter : ConverterBase, IActivityListConverter
    public class ActivityListConverter : AbstractActivityConverter, IActivityListConverter
    {

        public override object Convert(object objectResult, object input)
        {
            var criteria = (GlobalTixCriteria)input;
            var activitiesListRS = (ActivitiesListRS)objectResult;

            List<Activity> activities = new List<Activity>();
            foreach (ActivityInfoData activityData in activitiesListRS.ListData)
            {
                var act = Convert(activityData, activityData.TicketTypes) as Activity;
                activities.Add(act);
            }

            return activities;
        }

        /*
        public override object Convert(object objectResult, object input)
        {
            var criteria = (GlobalTixCriteria)input;
            var activitiesListRS = (ActivitiesListRS) objectResult;

            List<Activity> activities = new List<Activity>();
            foreach (Data activityData in activitiesListRS.ListData)
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


                //------ Product class variables assignment
                act.ProductType = ProductType.Ticket;
                act.ID = activityData.Id;
                act.Images = new List<ProductImage>
                {
                    new ProductImage { ID=0, Name=activityData.ImagePath, FileName=$"{Constant.GlobalTixBaseUrl}{Constant.ImageUrl}?name={activityData.ImagePath}", ImageType=ImageType.Bigproduct },
                    new ProductImage { ID=1, Name=activityData.ImagePath, FileName=$"{Constant.GlobalTixBaseUrl}{Constant.ImageUrl}?name={activityData.ImagePath}_banner", ImageType=ImageType.Smallproduct },
                };
                act.ThumbNailImage = $"{Constant.GlobalTixBaseUrl}{Constant.ImageUrl}?name={activityData.ImagePath}_thumb";
                act.ProductOptions = new List<ProductOption>();


                foreach (TicketTypeGroup ticketGroup in activityData.TicketTypeGroups)
                {
                    bool hasSeriesAndEvents = (ticketGroup.ApplyCapacity != null && ticketGroup.ApplyCapacity != false);

                    ActivityOption actOpt = new ActivityOption();
                    actOpt.Id = ticketGroup.Id;
                    actOpt.Name = ticketGroup.Name;
                    actOpt.SupplierName = Constant.GTSupplierName;
                    actOpt.Description = string.Empty;


                    //ticketGroup.ApplyCapacity
                    PriceAndAvailability dftPriceAndAvail = new DefaultPriceAndAvailability();
                    dftPriceAndAvail.PricingUnits = new List<PricingUnit>();

                    foreach (Identifier product in ticketGroup.Products)
                    {
                        TicketType prodTktType = GetTicketTypeForProductId(activityData.TicketTypes, product.Id);
                        if (prodTktType == null)
                        {
                            // TODO: Log a message here
                            Console.WriteLine($"TicketType {product.Id} not found for TicketGroup {ticketGroup.Id} in ActivityId {activityData.Id}");
                            continue;
                        }
                        AddPricingUnit(dftPriceAndAvail, prodTktType);
                    }

                    Price basePrice = new Price();
                    // TODO: Assign proper values here
                    basePrice.Amount = 0;
                    basePrice.Currency = new Currency { IsoCode = GetTicketsCurrencyCode(activityData.TicketTypes) };
                    basePrice.DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>();

                    if (hasSeriesAndEvents)
                    {
                        foreach (Identifier series in ticketGroup.Series)
                        {
                            Series seriesInstance = GetSeriesForSeriesId(activityData.Series, series.Id);
                            if (seriesInstance == null)
                            {
                                // TODO: Log a message here
                                Console.WriteLine($"Series {seriesInstance.Id} not found for TicketGroup {ticketGroup.Id} in ActivityId {activityData.Id}");
                                continue;
                            }

                            if (seriesInstance.StartDateTime >= DateTime.Now || seriesInstance.EndDateTime <= DateTime.Now)
                            {
                                // TODO: Log a message here
                                Console.WriteLine($"Series {seriesInstance.Id} with start on {seriesInstance.StartDateTime} and end on {seriesInstance.EndDateTime} not considered for TicketGroup {ticketGroup.Id} in ActivityId {activityData.Id}");
                                continue;
                            }

                            foreach (Event evt in seriesInstance.Events)
                            {
                                if (evt.EventDateTime <= DateTime.Now)
                                {
                                    continue;
                                }

                                PriceAndAvailability datePrcAndAvail = (PriceAndAvailability)dftPriceAndAvail.Clone();
                                datePrcAndAvail.AvailabilityStatus = (evt.AvailableCapacity > 0) ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE;
                                basePrice.DatePriceAndAvailabilty.Add(evt.EventDateTime, datePrcAndAvail);
                            }
                        }
                    }
                    else
                    { 
                        basePrice.DatePriceAndAvailabilty.Add(DateTime.Now, dftPriceAndAvail);
                    }

                    actOpt.BasePrice = basePrice;

                    //------
                    act.ProductOptions.Add(actOpt);

                    
                }
                act.ApiType = APIType.GlobalTix;

                activities.Add(act);
            }

            return activities;
        }
        */

        public override object Convert(object objectResult)
        {
            throw new NotImplementedException();
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
                PricingUnit prcUt = null;
                switch (psgrType)
                {
                    case PassengerType.Adult:
                        prcUt = new AdultPricingUnit();
                        break;
                    case PassengerType.Child:
                        prcUt = new ChildPricingUnit();
                        break;
                    case PassengerType.Senior:
                        prcUt = new SeniorPricingUnit();
                        break;
                    default:
                        return;
                }

                prcUt.Price = tktType.Price;

                dftPriceAndAvail.PricingUnits.Add(prcUt);
            }
        }

        private string GetTicketsCurrencyCode(List<TicketType> ticketTypes)
        {
            string currCode = string.Empty;
            foreach (TicketType ticketType in ticketTypes)
            {
                if (currCode.Equals(ticketType.CurrencyCode) == false)
                {
                    // TODO: Log a warning here
                }
                currCode = ticketType.CurrencyCode;
            }

            return currCode;
        }

        // During testing, there were product ids in response that did not map to any ticket type. In such
        // case lambdas fail and throw an exception. This method is written instead of lambdas so that these 
        // error conditions can be handled.
        private TicketType GetTicketTypeForProductId(List<TicketType> ticketTypes, int productId)
        {
            foreach (TicketType ticketType in ticketTypes)
            {
                if (ticketType.Id == productId)
                {
                    return ticketType;
                }
            }

            return null;
        }

        private Series GetSeriesForSeriesId(List<Series> seriesList, int seriesId)
        {
            foreach (Series series in seriesList)
            {
                if (series.Id == seriesId)
                {
                    return series;
                }
            }

            return null;
        }

        #endregion

    }
}
