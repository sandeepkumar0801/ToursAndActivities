using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.PrioHub;
using Logger.Contract;
using ServiceAdapters.PrioHub.Constants;
using ServiceAdapters.PrioHub.PrioHub.Converters.Contracts;
using ServiceAdapters.PrioHub.PrioHub.Entities;
using ServiceAdapters.PrioHub.PrioHub.Entities.ProductDetailResponse;
using Util;
using CONSTANT = Util.CommonUtilConstantCancellation;
using RESOURCEMANAGER = Util.CommonResourceManager;

namespace ServiceAdapters.PrioHub.PrioHub.Converters
{
    public class ProductDetailConverter : ConverterBase, IProductDetailConverter
    {

        public ProductDetailConverter(ILogger logger) : base(logger)
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
        private PrioHubPriceAndAvailability GetPriceAndAvailabiltyForPrio(string vacancies, string fromDateTime, string toDateTime)
        {
            return null;
        }

        /// <summary>
        /// Convert Availability Result
        /// </summary>
        /// <param name="objectResult"></param>
        /// <returns></returns>
        public List<ActivityOption> ConvertAvailablityResult(object objectResult)
        {
            var optionsList = new List<ActivityOption>();
            var criteria = objectResult as PrioHubCriteria;
            var detail = criteria.ProductDetailResponseAPI as ProductDetailResponse;
            var activityOption = criteria.ActivityOptionAPI;
            var timeBased = criteria.TimeBased;
            var token = "";
            //ProductAvailability true and availability is null, return null
            if (activityOption == null && detail.Data.Product.ProductAvailability == true)
            {
                detail = null;
            }
            if (detail != null)
            {
                var ticketDetailRsObj = (ProductDetailResponse)detail;
                var getProduct = ticketDetailRsObj?.Data?.Product;
                var getProductTypeSeason = getProduct?.ProductTypeSeasons;
                var pickupDataList = getProduct?.ProductPickupPointDetails;

                var returnProductTypeSeason = PrioHubProductPaxMapping(getProductTypeSeason, criteria.CheckinDate);
                var returnProductTypeStatus = GetProductTypeStatus(getProduct);
                var returnPickUpLocations = PickUpLocations(pickupDataList);
                var returnProductGroupCode = getProduct?.ProductCodeSettings?.ProductGroupCode;
                var returnCombiProducts = CombiProducts(getProduct?.ProductCombiDetails);
                var returnClusterProducts =ClusterProducts(getProduct?.ProductClusterDetails);
                if (timeBased == 1)//timebased products
                {
                    //Cluster Products
                    if (returnClusterProducts != null && returnClusterProducts.Count > 0)
                    {
                        foreach (var cluster in returnClusterProducts)
                        {
                            foreach (var timeslot in activityOption?.BasePrice?.DatePriceAndAvailabilty)
                            {
                                optionsList = ProductOptionsListTimeBased(ticketDetailRsObj, criteria, activityOption,
                                returnPickUpLocations, returnProductTypeStatus, returnProductTypeSeason,
                                returnProductGroupCode, returnCombiProducts, token, optionsList, timeslot,
                                cluster);
                            }
                        }
                    }
                    else
                    {
                        foreach (var timeslot in activityOption?.BasePrice?.DatePriceAndAvailabilty)
                        {
                            optionsList = ProductOptionsListTimeBased(ticketDetailRsObj, criteria, activityOption,
                            returnPickUpLocations, returnProductTypeStatus, returnProductTypeSeason,
                            returnProductGroupCode, returnCombiProducts, token, optionsList, timeslot,
                            null);
                        }
                    }
                }
                else
                {

                    //Cluster Products
                    if (returnClusterProducts != null && returnClusterProducts.Count > 0)
                    {
                        foreach (var cluster in returnClusterProducts)
                        {
                            optionsList = ProductOptionsList(ticketDetailRsObj, criteria, activityOption,
                                                returnPickUpLocations, returnProductTypeStatus, returnProductTypeSeason,
                                                returnProductGroupCode, returnCombiProducts, token, optionsList,
                                                cluster);
                        }
                    }
                    else//Simple Products
                    {
                    optionsList = ProductOptionsList(ticketDetailRsObj, criteria, activityOption,
                    returnPickUpLocations, returnProductTypeStatus, returnProductTypeSeason,
                    returnProductGroupCode, returnCombiProducts, token, optionsList,
                    null);
                    }
                }
            }
            return optionsList;
        }


        #region Private methods


        private List<ActivityOption> ProductOptionsListTimeBased(
           ProductDetailResponse ticketDetailRsObj,
           PrioHubCriteria criteria, ActivityOption activityOption,
           List<PickUpPointForPrioHub> returnPickUpLocations,
           int returnProductTypeStatus, 
           List<PrioHubProductPaxMapping> returnProductTypeSeason,
           bool? returnProductGroupCode,
           List<Isango.Entities.PrioHub.ProductCombiDetails> returnCombiProducts,
           string token,
           List<ActivityOption> optionsList,
           KeyValuePair<DateTime, PriceAndAvailability> timeslot,
           Isango.Entities.PrioHub.ProductCluster cluster)
        {
            try
            {
                if (ticketDetailRsObj != null)
                {
                    try
                    {
                        var PandA = new Dictionary<DateTime, PriceAndAvailability>
                        {
                            { timeslot.Key, timeslot.Value }
                        };
                        var basePrice = CreatePrice(criteria, ticketDetailRsObj, PandA, Constant.BasePrice, cluster);

                        var costPrice = CreatePrice(criteria, ticketDetailRsObj, PandA, Constant.CostPrice, cluster);

                        var gatePrice = CreatePrice(criteria, ticketDetailRsObj, PandA, Constant.GatePrice, cluster);

                        var checkAvailabilityStatus = activityOption != null;
                        var pickupRs = ticketDetailRsObj?.Data?.Product?.ProductPickupPoint;
                        

                        var option = new ActivityOption
                        {
                            AvailabilityStatus = checkAvailabilityStatus ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE,
                            BasePrice = basePrice,
                            CostPrice = costPrice,
                            GateBasePrice = gatePrice,
                            StartTime = timeslot.Key.TimeOfDay,
                            Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                            PrioHubTicketClass = ticketDetailRsObj?.Data?.Product?.ProductTypeSeasons?.FirstOrDefault()?.ProductTypeSeasonDetails?.FirstOrDefault()?.ProductTypeClass,
                            PickupPoints = pickupRs, //check pickp required or not
                            PrioHubDistributerId = ticketDetailRsObj?.Data?.Product?.ProductDistributorid
                        };
                        SetCancellationPolicyText(ticketDetailRsObj?.Data?.Product?.ProductCancellationAllowed, ticketDetailRsObj?.Data?.Product?.ProductCancellationPolicies?.FirstOrDefault(),option, criteria);
                        if (cluster != null)
                        {
                            option.Name = cluster?.ProductTitle;
                        }
                        //Pickup Points
                        if (!string.IsNullOrEmpty(option?.PickupPoints) && pickupRs != null && pickupRs.ToUpper() == "MANDATORY")
                        {
                            option.PickUpPointForPrioHub = new List<PickUpPointForPrioHub>();
                            option.PickUpPointForPrioHub = returnPickUpLocations;
                        }
                        option = CommonData(option, returnProductTypeStatus, returnProductTypeSeason,
                        returnProductGroupCode, returnCombiProducts, cluster);
                        if (option != null)
                        {
                            optionsList.Add(option);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(new IsangoErrorEntity
                        {
                            ClassName = "PrioHub.PrioAdapter",
                            MethodName = "ProductOptionsListTimeBased",
                            Token = token
                        }, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(new IsangoErrorEntity
                {
                    ClassName = "PrioHub.PrioAdapter",
                    MethodName = "ProductOptionsListTimeBased",
                    Token = token
                }, ex);
            }
            return optionsList;
        }

        private void SetCancellationPolicyText(
            bool? apiCancellationAllowed, ProductCancellationPolicies apiCancellationPolicies,
            ActivityOption option,PrioHubCriteria criteria)
        {
            option.Cancellable = System.Convert.ToBoolean(apiCancellationAllowed);
            //1. In case of "cancellation_type": "TRAVEL_DATE",
            //it is mandatory to pass the cancellation_fee_threshold value.(in minutes)
            //2.BOOKING_DATE: it is open-ticket and cancel any time.
            var language = criteria?.Language ?? "en";
            if (option?.Cancellable == false)
            {
                option.CancellationText = RESOURCEMANAGER.GetString(language, CONSTANT.CancellationPolicyNonRefundable);
            }
            else
            {
                if (apiCancellationPolicies?.CancellationType == "TRAVEL_DATE")
                {
                    if (!string.IsNullOrEmpty(apiCancellationPolicies?.CancellationDescription))
                    {
                        try
                        {
                            option.CancellationText = Util.TranslateHelper.Translate(apiCancellationPolicies?.CancellationDescription, language);
                        }
                        catch(Exception ex)
                        {
                            option.CancellationText = apiCancellationPolicies?.CancellationDescription;
                        }
                    }
                    else
                    {
                        if (apiCancellationPolicies?.CancellationFeeThreshold > 0)
                        {
                            //it is mandatory to pass the cancellation_fee_threshold value.(in minutes)
                            int noOfHours = (apiCancellationPolicies.CancellationFeeThreshold / 60);
                            option.CancellationText = $"{string.Format(RESOURCEMANAGER.GetString(language, CONSTANT.CancellationPolicy100ChargableBeforeNhours), noOfHours, noOfHours)}";
                        }
                    }

                }
                else if (apiCancellationPolicies?.CancellationType == "BOOKING_DATE")
                {
                    if (!string.IsNullOrEmpty(apiCancellationPolicies?.CancellationDescription))
                    {
                        try
                        {
                            option.CancellationText = Util.TranslateHelper.Translate(apiCancellationPolicies?.CancellationDescription, language);
                        }
                        catch (Exception ex)
                        {
                            option.CancellationText = apiCancellationPolicies?.CancellationDescription;
                        }
                    }
                    else
                    {
                        //it is open-ticket and cancel any time.
                        option.CancellationText = RESOURCEMANAGER.GetString(language, CONSTANT.FreeCancellation);
                    }
                }
            }

            option.ApiCancellationPolicy = Util.SerializeDeSerializeHelper.Serialize(new
            {
                apiCancellationPolicies
            });

        }
            private ActivityOption CommonData(ActivityOption option,
            int returnProductTypeStatus, 
            List<PrioHubProductPaxMapping> returnProductTypeSeason,
            bool? returnProductGroupCode,
            List<Isango.Entities.PrioHub.ProductCombiDetails> returnCombiProducts,
            Isango.Entities.PrioHub.ProductCluster returnCluster)
        {
            //Product Status,like //product_combi,/product_third_party
            option.PrioHubProductTypeStatus = returnProductTypeStatus;
            //PrioHub ProductPaxMapping
            option.PrioHubProductPaxMapping = new List<PrioHubProductPaxMapping>();
            option.PrioHubProductPaxMapping = returnProductTypeSeason;
            option.PrioHubProductGroupCode = returnProductGroupCode;
            option.ProductCombiDetails = returnCombiProducts;
            option.Cluster = returnCluster;
            return option;
        }

            private List<ActivityOption> ProductOptionsList(
            ProductDetailResponse ticketDetailRsObj,
            PrioHubCriteria criteria, ActivityOption activityOption,
            List<PickUpPointForPrioHub> returnPickUpLocations,
            int returnProductTypeStatus,
            List<PrioHubProductPaxMapping> returnProductTypeSeason,
            bool? returnProductGroupCode,
            List<Isango.Entities.PrioHub.ProductCombiDetails> returnCombiProducts,
            string token, List<ActivityOption> optionsList,
            Isango.Entities.PrioHub.ProductCluster cluster)
        {
            
            if (ticketDetailRsObj != null)
            {
                try
                {
                    var basePrice = CreatePrice(criteria, ticketDetailRsObj, activityOption?.BasePrice?.DatePriceAndAvailabilty, Constant.BasePrice, cluster);

                    var costPrice = CreatePrice(criteria, ticketDetailRsObj, activityOption?.CostPrice?.DatePriceAndAvailabilty, Constant.CostPrice, cluster);

                    var gatePrice = CreatePrice(criteria, ticketDetailRsObj, activityOption?.GateBasePrice?.DatePriceAndAvailabilty, Constant.GatePrice, cluster);

                    var checkAvailabilityStatus = activityOption != null || basePrice != null;
                    var pickupRs = ticketDetailRsObj?.Data?.Product?.ProductPickupPoint;

                    
                    var option = new ActivityOption
                    {
                        AvailabilityStatus = checkAvailabilityStatus ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE,
                        BasePrice = basePrice,
                        CostPrice = costPrice,
                        GateBasePrice= gatePrice,
                        PrioHubTicketClass = ticketDetailRsObj?.Data?.Product?.ProductTypeSeasons?.FirstOrDefault()?.ProductTypeSeasonDetails?.FirstOrDefault()?.ProductTypeClass,
                        StartTime = activityOption?.BasePrice?.DatePriceAndAvailabilty?.FirstOrDefault().Key.TimeOfDay ?? default(TimeSpan),
                        PickupPoints = pickupRs, //check pickp required or not
                        PrioHubDistributerId= ticketDetailRsObj?.Data?.Product?.ProductDistributorid
                    };

                    SetCancellationPolicyText(ticketDetailRsObj?.Data?.Product?.ProductCancellationAllowed, ticketDetailRsObj?.Data?.Product?.ProductCancellationPolicies?.FirstOrDefault(), option, criteria);
                    if (cluster != null)
                    {
                        option.Name = cluster?.ProductTitle;
                    }
                    //Pickup Points
                    if (!string.IsNullOrEmpty(option?.PickupPoints) && pickupRs != null && pickupRs.ToUpper() == "MANDATORY")
                    {
                        option.PickUpPointForPrioHub = new List<PickUpPointForPrioHub>();
                        option.PickUpPointForPrioHub = returnPickUpLocations;
                    }
                     option=CommonData(option, returnProductTypeStatus, returnProductTypeSeason,
                         returnProductGroupCode, returnCombiProducts, cluster);
                    optionsList.Add(option);
                }
                catch (Exception ex)
                {
                    _logger.Error(new IsangoErrorEntity
                    {
                        ClassName = "PrioHub.PrioAdapter",
                        MethodName = "ProductOptionsList",
                        Token = token
                    }, ex);

                    return null;
                }
            }
            return optionsList;
        }


        private Price CreatePrice(PrioHubCriteria criteria,
          ProductDetailResponse ticketDetailRsObj,
          Dictionary<DateTime, PriceAndAvailability> prioDatePriceAndAvailabilty,
          string priceType, Isango.Entities.PrioHub.ProductCluster cluster)
        {
            var priceAndAvailability = GetPrice(ticketDetailRsObj, criteria, priceType, cluster);

            var price = new Price
            {
                Amount = priceAndAvailability?.FirstOrDefault().Value?.TotalPrice ?? 0,
                Currency = new Currency { IsoCode = ticketDetailRsObj.Data?.Product?.ProductPaymentDetail?.ProductPaymentCurrency?.CurrencyCode },
                DatePriceAndAvailabilty = priceAndAvailability
            };

            if (prioDatePriceAndAvailabilty?.Any() == true)
            {
                price.DatePriceAndAvailabilty = CreateDatePriceAndAvailabilty(criteria, priceAndAvailability, prioDatePriceAndAvailabilty);
            }
            else
            {
                price.DatePriceAndAvailabilty = CreateDatePriceAndAvailabilty(criteria, priceAndAvailability, null);
            }
            return price;
        }
        private Dictionary<DateTime, PriceAndAvailability> GetPrice(
            ProductDetailResponse ticketDetailRs, Criteria criteria, string priceType,
            Isango.Entities.PrioHub.ProductCluster cluster)
        {
            var priceAndAvailabilities = new Dictionary<DateTime, PriceAndAvailability>();
            var currencyAPI = ticketDetailRs.Data?.Product?.ProductPaymentDetail?.ProductPaymentCurrency?.CurrencyCode;
            var productTypeSeasons = ticketDetailRs?.Data?.Product?.ProductTypeSeasons;
            var inputCheckinDate = criteria.CheckinDate;
            var inputCheckoutDate = criteria.CheckoutDate;

            var dates = new List<DateTime>();
            for (var dt = criteria.CheckinDate; dt <= inputCheckoutDate; dt = dt.AddDays(1))
            {
                dates.Add(dt);
            }
            foreach (var ticketTypeDetail in productTypeSeasons)
            {
                ticketTypeDetail.StartDateAsDate = ticketTypeDetail.ProductTypeSeasonStartDate.Date;
                ticketTypeDetail.EndDateAsDate = ticketTypeDetail.ProductTypeSeasonEndDate.Date;

                foreach (var paxItem in ticketTypeDetail?.ProductTypeSeasonDetails)
                {
                    paxItem.StartDateAsDate = ticketTypeDetail.StartDateAsDate;
                    paxItem.EndDateAsDate = ticketTypeDetail.EndDateAsDate;
                }
            }
            var checkEndDateNotNull = productTypeSeasons.Any(x => x.EndDateAsDate != DateTime.MinValue);
            //Filter response price based on dates in criteria
            //if (checkEndDateNotNull == true)
           // {
                //productTypeSeasons = productTypeSeasons
                //                    ?.Where(x =>
                //                                    (inputCheckinDate >= x.StartDateAsDate
                //                                &&
                //                                    inputCheckinDate <= x.EndDateAsDate)
                //                                ||
                //                                (inputCheckoutDate >= x.StartDateAsDate
                //                                &&
                //                                    inputCheckoutDate <= x.EndDateAsDate)

                //                    )
                //                    ?.ToList();

            //}
            //else
            //{
                productTypeSeasons = productTypeSeasons
                                        ?.Where(x =>
                                                        (inputCheckinDate >= x.StartDateAsDate)
                                                    ||
                                                    (inputCheckoutDate >= x.StartDateAsDate
                                                    )

                                        )?.ToList();

            //}

            if (productTypeSeasons?.Any() != true)
            {
                return null;
            }

            //Create P&A for each matching date in input and response from api
            //Parallel.ForEach(dates, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, (date) =>
            foreach (var date in dates)
            {
                try
                {
                    var ticketTypeDetailsForDate = new List<ServiceAdapters.PrioHub.PrioHub.Entities.ProductDetailResponse.ProductTypeSeasons>();
                    if (checkEndDateNotNull == true)
                    {
                        ticketTypeDetailsForDate = productTypeSeasons.Where(x =>
                               date >= x.StartDateAsDate &&
                               date <= x.EndDateAsDate
                           )?.ToList();
                    }
                    else
                    {
                        ticketTypeDetailsForDate = productTypeSeasons.Where(x =>
                                                       date >= x.StartDateAsDate
                                                   )?.ToList();
                    }


                    if (ticketTypeDetailsForDate?.Count > 0)
                    {
                        var priceAndAvailability = new PrioHubPriceAndAvailability
                        {
                            PricingUnits = new List<PricingUnit>()
                        };

                        //Create pricing unit for each type of pax
                                foreach (var productTypeSeasonDetails in ticketTypeDetailsForDate?.LastOrDefault()?.ProductTypeSeasonDetails)
                                {
                                    var startDate = productTypeSeasonDetails.StartDateAsDate;
                                    var endDate = productTypeSeasonDetails.EndDateAsDate;
                                    if (date >= startDate)
                                    {
                                        try
                                        {
                                            //1.product_type_sales_price:
                                            //Standard price after discount for the end-customer set by the reseller. 
                                            //2.product_type_list_price
                                            //Standard price before discount for the end customer of this product type set by the supplier
                                            var price = 0m;
                                            if (priceType == Constant.BasePrice)//sellprice
                                            {
                                                price = System.Convert.ToDecimal(productTypeSeasonDetails?.ProductTypePricing?.ProductTypeSalesPrice);
                                            }
                                            else if (priceType == Constant.CostPrice)//costprice
                                            {
                                                price = System.Convert.ToDecimal(productTypeSeasonDetails?.ProductTypePricing?.ProductTypeResalePrice);
                                            }
                                            else if (priceType == Constant.GatePrice)//gatePrice
                                            {
                                                price = System.Convert.ToDecimal(productTypeSeasonDetails?.ProductTypePricing?.ProductTypelistPrice);
                                            }
                                            //check , Is it have availability_pricing node, then price plus or minus
                                            //Filter with date.
                                            if (priceType == Constant.BasePrice)
                                            {
                                                var checkAvailabilityPricingNode = ((PrioHubCriteria)criteria)?.ActivityOptionAPI?.BasePrice?.DatePriceAndAvailabilty?.Where(x => x.Key.Date == date)?.FirstOrDefault().Value;
                                                if (checkAvailabilityPricingNode != null)
                                                {
                                                    //price increase or decrease according to it.
                                                    var getPriceData = ((PrioHubPriceAndAvailability)checkAvailabilityPricingNode)?.PrioHubAvailabilityPricing?.FirstOrDefault();
                                                    if (getPriceData != null)
                                                    {
                                                        //Non-percentage amount
                                                        if (String.IsNullOrEmpty(getPriceData.AvailabilityPricingVariationPercentage))
                                                        {
                                                            var priceChangeAmount = getPriceData?.AvailabilityPricingVariationAmount;
                                                            price = price + System.Convert.ToDecimal(priceChangeAmount);
                                                        }
                                                        else //percentage amount
                                                        {
                                                            var priceChangePercentage = getPriceData?.AvailabilityPricingVariationAmount;
                                                            var findpercentageAmount = price * (priceChangePercentage / 100);
                                                            price = price + System.Convert.ToDecimal(findpercentageAmount);
                                                        }
                                                    }
                                                }
                                            }

                                            var ticketType = productTypeSeasonDetails?.ProductType?.ToUpper();
                                            var maxCapacity = ticketDetailRs?.Data?.Product?.ProductBookingQuantityMax;
                                            var minCapacity = ticketDetailRs?.Data?.Product?.ProductBookingQuantityMin;
                                            var productTypePriceType = ticketDetailRs?.Data?.Product?.ProductTypeSeasons?.FirstOrDefault()?.ProductTypeSeasonDetails?.FirstOrDefault()?.ProductTypePriceType;
                                            var productTypeQuantityVariations = productTypeSeasonDetails?.ProductTypeQuantityVariations;


                                            var totalNumberOfPassengers = criteria.NoOfPassengers.Sum(x => x.Value);
                                            if (minCapacity != null && maxCapacity != null && minCapacity != 0 && maxCapacity != 0)
                                            {
                                                if (totalNumberOfPassengers >= minCapacity && totalNumberOfPassengers <= maxCapacity)
                                                { }
                                                else
                                                {
                                                    continue;
                                                }
                                            }

                                            //Add pricing unit if not already added and its in input criteria
                                            if (productTypePriceType.ToUpper() != System.Convert.ToString(ProductTypePriceType.GROUP))
                                            {
                                                var pricingUnit = CreatePricingUnit(ticketType, price, minCapacity, productTypePriceType);
                                                if (
                                                priceAndAvailability?.PricingUnits.Cast<PerPersonPricingUnit>()?
                                                .Any(x => x.PassengerType == ((PerPersonPricingUnit)pricingUnit).PassengerType) == false

                                             && (criteria.NoOfPassengers.Keys
                                                    .Contains(((PerPersonPricingUnit)pricingUnit).PassengerType)
                                                    || !criteria.NoOfPassengers.Any()
                                                )
                                            )
                                                {
                                                    priceAndAvailability.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                                                    pricingUnit.Currency = currencyAPI;
                                                    priceAndAvailability.PricingUnits.Add(pricingUnit);
                                                }
                                            }
                                            else //GROUP Case
                                            {
                                                var passengerType = GetPassengerType(ticketType);
                                                var pricingUnit = PricingUnitFactory.GetPricingUnit(passengerType);
                                                //pricingUnit.UnitType = UnitType.PerUnit;
                                                //pricingUnit.PriceType = PriceType.PerUnit;

                                                pricingUnit.UnitType = UnitType.PerPerson;
                                                pricingUnit.PriceType = PriceType.PerPerson;
                                                var noOfPassengers = criteria?.NoOfPassengers?.Where(x => x.Key == passengerType)?.FirstOrDefault().Value;
                                                pricingUnit.Price = System.Convert.ToDecimal((price / System.Convert.ToDecimal(noOfPassengers)).ToString("0.00"));
                                                priceAndAvailability.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                                                //Check Price Variation//

                                                //Filter Group Range
                                                var getPrice = new ProductTypeQuantityVariations();
                                                if (noOfPassengers != null && noOfPassengers > 0)
                                                {
                                                    getPrice = productTypeQuantityVariations?.Where(x => x.ProductTypeQuantityVariationMin <= noOfPassengers && x.ProductTypeQuantityVariationMax > noOfPassengers)?.FirstOrDefault();
                                                    if (getPrice != null && getPrice?.ProductTypeQuantityVariationAmount > 0)
                                                    {

                                                        var finalPrice = price + getPrice?.ProductTypeQuantityVariationAmount;
                                                        pricingUnit.Price = System.Convert.ToDecimal(finalPrice / System.Convert.ToDecimal(noOfPassengers));
                                                        pricingUnit.Price = System.Convert.ToDecimal(pricingUnit.Price.ToString("0.00"));
                                                    }
                                                }
                                                pricingUnit.TotalCapacity = System.Convert.ToInt32(getPrice?.ProductTypeQuantityVariationMax);
                                                pricingUnit.Mincapacity = System.Convert.ToInt32(getPrice?.ProductTypeQuantityVariationMin);
                                                pricingUnit.Currency = currencyAPI;
                                                priceAndAvailability.PricingUnits.Add(pricingUnit);
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            Task.Run(() =>
                                                _logger.Error(new IsangoErrorEntity
                                                {
                                                    ClassName = "PrioHub.PrioAdapter",
                                                    MethodName = "GetPrice",
                                                    Params = SerializeDeSerializeHelper.Serialize(productTypeSeasonDetails),
                                                }, ex)
                                            );
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                           }
                        if (cluster != null)
                        {
                            priceAndAvailability.TourDepartureId = cluster.ProductId;
                        }
                        else
                        {
                            priceAndAvailability.TourDepartureId = int.Parse(ticketDetailRs?.Data?.Product?.ProductId);
                        }
                        priceAndAvailability.Name = ticketDetailRs?.Data?.Product?.ProductSourceName;
                        priceAndAvailability.TotalPrice = priceAndAvailability.PricingUnits.Sum(e => e.Price);

                        if (!priceAndAvailabilities.Keys.Contains(date))
                        {
                            priceAndAvailabilities.Add(date, priceAndAvailability);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Task.Run(() =>
                        _logger.Error(new IsangoErrorEntity
                        {
                            ClassName = "PrioHub.PrioAdapter",
                            MethodName = "GetPrice",
                            Params = $"Error for date :-{date.ToString()}"
                        }, ex)
                    );
                }
            }
            //);
            priceAndAvailabilities?.OrderBy(x => x.Key);
            return priceAndAvailabilities;
        }

        private static Dictionary<DateTime, PriceAndAvailability> CreateDatePriceAndAvailabilty(
            Criteria criteria,
            Dictionary<DateTime, PriceAndAvailability> prioPriceAndAvailabilities,
            Dictionary<DateTime, PriceAndAvailability> prioDatePriceAndAvailabilty)
        {
            try
            {
                var datePriceAndAvailabilities = new Dictionary<DateTime, PriceAndAvailability>();
                var startDateTime = DateTime.Now.Date;
                //api product_availability:true
                if (prioPriceAndAvailabilities?.Any() == true && prioDatePriceAndAvailabilty != null)
                {
                    foreach (var item in prioPriceAndAvailabilities)
                    {
                        var prioPriceAndAvailabilty = item.Value;
                        startDateTime = item.Key;

                        if (prioDatePriceAndAvailabilty.Any(x => x.Key.Date == startDateTime))
                        {
                            var pandAfromAPI = prioDatePriceAndAvailabilty.Where(x => x.Key.Date == startDateTime).FirstOrDefault().Value;
                            var prioPriceAndAvailabiltyCloned = (PrioHubPriceAndAvailability)prioPriceAndAvailabilty.Clone();
                            if (pandAfromAPI != null)
                            {
                                var prioPandA = (PrioHubPriceAndAvailability)pandAfromAPI;
                                prioPriceAndAvailabiltyCloned.Vacancies = prioPandA.Vacancies;
                                prioPriceAndAvailabiltyCloned.FromDateTime = prioPandA.AvailabilityFromDateTime;
                                prioPriceAndAvailabiltyCloned.ToDateTime = prioPandA.AvailabilityToDateTime;
                                prioPriceAndAvailabiltyCloned.AvailabilityStatus = pandAfromAPI.AvailabilityStatus;
                                prioPriceAndAvailabiltyCloned.IsCapacityCheckRequired = prioPandA.IsCapacityCheckRequired;
                                prioPriceAndAvailabiltyCloned.Capacity = prioPandA.Capacity;
                                prioPriceAndAvailabiltyCloned.AvailabilityId = prioPandA?.AvailabilityId;
                                if (!datePriceAndAvailabilities.Keys.Contains(startDateTime))
                                {
                                    datePriceAndAvailabilities.Add(startDateTime, prioPriceAndAvailabiltyCloned);
                                }
                            }
                        }
                    }
                }
                else//api product_availability:false
                {
                    var defaultCapacity = System.Convert.ToInt32(Util.ConfigurationManagerHelper.GetValuefromAppSettings("DefaultCapacity"));
                    var capacity = defaultCapacity;
                    foreach (var item in prioPriceAndAvailabilities)
                    {
                        var prioPriceAndAvailabilty = item.Value;
                        startDateTime = item.Key;

                        var prioPriceAndAvailabiltyCloned = (PrioHubPriceAndAvailability)prioPriceAndAvailabilty.Clone();
                        prioPriceAndAvailabiltyCloned.Vacancies = "";
                        prioPriceAndAvailabiltyCloned.FromDateTime = System.Convert.ToString(startDateTime);
                        prioPriceAndAvailabiltyCloned.ToDateTime = System.Convert.ToString(startDateTime);
                        prioPriceAndAvailabiltyCloned.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                        prioPriceAndAvailabiltyCloned.IsCapacityCheckRequired = false;
                        prioPriceAndAvailabiltyCloned.Capacity = capacity;
                        prioPriceAndAvailabiltyCloned.AvailabilityId = "";
                        if (!datePriceAndAvailabilities.Keys.Contains(startDateTime))
                        {
                            datePriceAndAvailabilities.Add(startDateTime, prioPriceAndAvailabiltyCloned);
                        }
                    }
                }
                return datePriceAndAvailabilities;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private PricingUnit CreatePricingUnit(string ticketType, decimal price, int? minCapacity, string productTypePriceType)
        {
            var passengerType = GetPassengerType(ticketType);
            var pricingUnit = (PerPersonPricingUnit)PricingUnitFactory.GetPricingUnit(passengerType);
            pricingUnit.Price = price;
            pricingUnit.Mincapacity = ((minCapacity == null || minCapacity == 0) ? 1 : minCapacity);
            return pricingUnit;

        }
        private PassengerType GetPassengerType(string ticketType)
        {
            //"ADULT" "CHILD" "SENIOR" "YOUTH" "INFANT" "PERSON" 
            //"STUDENT" "RESIDENT" "MILITARY" "IMPAIRED" "ITEM"
            //"GROUP" "FAMILY" "CUSTOM"
            switch (ticketType.ToUpper())
            {
                case Constant.Adult:
                    return PassengerType.Adult;

                case Constant.Child:
                    return PassengerType.Child;

                case Constant.Senior:
                    return PassengerType.Senior;

                case Constant.Youth:
                    return PassengerType.Youth;

                case Constant.Infant:
                    return PassengerType.Infant;

                case Constant.Student:
                    return PassengerType.Student;

                case Constant.Family:
                    return PassengerType.Family;

                default:
                    return PassengerType.Adult;
            }
        }

        private int GetProductTypeStatus(ServiceAdapters.PrioHub.PrioHub.Entities.ProductDetailResponse.Product product)
        {
            int returnProductTypeStatus = 0;
            if (product.ProductThirdParty == true)
            {
                returnProductTypeStatus = 0;
            }
            else if (product.ProductSeasonalPricing == true)
            {
                returnProductTypeStatus = 1;
            }
            else if (product.ProductQuantityPricing == true)
            {
                returnProductTypeStatus = 2;
            }
            else if (product.ProductDailyPricing == true)
            {
                returnProductTypeStatus = 3;
            }
            else if (product.ProductDynamicPricing == true)
            {
                returnProductTypeStatus = 4;
            }

            else if (product.ProductCluster == true)
            {
                returnProductTypeStatus = 5;
            }
            else if (product.ProductCombi == true)
            {
                returnProductTypeStatus = 6;
            }
            else if (product.ProductAddon == true)
            {
                returnProductTypeStatus = 7;
            }
            else if (product.ProductRelationDetailsVisible == true)
            {
                returnProductTypeStatus = 8;
            }
            else if (product.ProductTimePickerVisible == true)
            {
                returnProductTypeStatus = 9;
            }
            return returnProductTypeStatus;
        }
        private List<PickUpPointForPrioHub> PickUpLocations(List<ProductPickupPointDetails> pickupDataList)
        {
            var pickupList = new List<PickUpPointForPrioHub>();
            if (pickupDataList != null && pickupDataList?.Count > 0)
            {
                foreach (var pickupItem in pickupDataList)
                {
                    var pickUpPointDataItem = new PickUpPointForPrioHub
                    {
                        PickupPointAvailabilityDependency = pickupItem.PickupPointAvailabilityDependency,
                        PickupPointDescription = pickupItem?.PickupPointDescription,
                        PickupPointId = pickupItem?.PickupPointId,
                        PickupPointLocation = pickupItem?.PickupPointLocation,
                        PickupPointName = pickupItem?.PickupPointName,
                        PickupPointTimes = pickupItem?.PickupPointTimes
                    };
                    pickupList.Add(pickUpPointDataItem);
                }
            }
            return pickupList;
        }

        private List<Isango.Entities.PrioHub.ProductCombiDetails> CombiProducts(List<ServiceAdapters.PrioHub.PrioHub.Entities.ProductDetailResponse.ProductCombiDetails> combiDataList)
        {
            var combiProductList = new List<Isango.Entities.PrioHub.ProductCombiDetails>();
            if (combiDataList != null && combiDataList?.Count > 0)
            {
                foreach (var combiItem in combiDataList)
                {
                    var combiDataItem = new Isango.Entities.PrioHub.ProductCombiDetails
                    {
                        ProductAdmissionType = combiItem.ProductAdmissionType,
                        ProductBookingWindowEndTime = combiItem.ProductBookingWindowEndTime,
                        ProductBookingWindowProductId = combiItem.ProductBookingWindowProductId,
                        ProductBookingWindowStartTime = combiItem.ProductBookingWindowStartTime,
                        ProductCurrencyCode = combiItem.ProductCurrencyCode,
                        ProductFromPrice = combiItem.ProductFromPrice,
                        ProductId = combiItem.ProductId,
                        ProductParentId = combiItem.ProductParentId,
                        ProductStartDate = combiItem.ProductStartDate,
                        ProductSupplierId = combiItem.ProductSupplierId,
                        ProductSupplierName = combiItem.ProductSupplierName,
                        ProductTimepickerVisible = combiItem.ProductTimepickerVisible,
                        ProductTitle = combiItem.ProductTitle
                    };
                    combiProductList.Add(combiDataItem);
                }
            }
            return combiProductList;
        }

        private List<Isango.Entities.PrioHub.ProductCluster> ClusterProducts(List<ServiceAdapters.PrioHub.PrioHub.Entities.ProductDetailResponse.ProductClusterDetails> clusterDataList)
        {
            var clusterProductList = new List<Isango.Entities.PrioHub.ProductCluster>();
            if (clusterDataList != null && clusterDataList?.Count > 0)
            {
                foreach (var combiItem in clusterDataList)
                {
                    var clusterDataItem = new Isango.Entities.PrioHub.ProductCluster
                    {
                       ProductAdmissionType= combiItem.ProductAdmissionType, 
                       ProductCurrencyCode= combiItem.ProductCurrencyCode,
                       ProductFromPrice= combiItem.ProductFromPrice,
                       ProductId= combiItem.ProductId,
                       ProductParentId= combiItem.ProductParentId,
                       ProductStartDate= combiItem.ProductStartDate,
                       ProductSupplierId= combiItem.ProductSupplierId,
                       ProductSupplierName= combiItem.ProductSupplierName,
                       ProductTimepickerVisible= combiItem.ProductTimepickerVisible,
                       ProductTitle= combiItem.ProductTitle
                    };
                    clusterProductList.Add(clusterDataItem);
                }
            }
            return clusterProductList;
        }

        private List<PrioHubProductPaxMapping> PrioHubProductPaxMapping(List<ProductTypeSeasons> productTypeSeasons, DateTime CheckinDate)
        {
            var list = new List<PrioHubProductPaxMapping>();
            if (productTypeSeasons != null && productTypeSeasons?.Count > 1)
            {
                productTypeSeasons = productTypeSeasons?.Where(t => CheckinDate.Date >= t.ProductTypeSeasonStartDate.Date && CheckinDate.Date <= t.ProductTypeSeasonEndDate.Date)?.ToList();
            }

            if (productTypeSeasons != null && productTypeSeasons.Count > 0)
            {
                foreach (var season in productTypeSeasons)
                {
                    var seasonDetail = season?.ProductTypeSeasonDetails;
                    if (seasonDetail != null && seasonDetail.Count > 0)
                    {
                        foreach (var sessionItem in seasonDetail)
                        {
                            var item = new PrioHubProductPaxMapping
                            {
                                ProductType = sessionItem?.ProductType,
                                ProductTypeAgeFrom = sessionItem.ProductTypeAgeFrom,
                                ProductTypeAgeTo = sessionItem.ProductTypeAgeTo,
                                ProductTypeClass = sessionItem?.ProductTypeClass,
                                ProductTypeId = sessionItem?.ProductTypeId,
                                ProductTypeLabel = sessionItem?.ProductTypeLabel,
                                ProductTypePriceType = sessionItem?.ProductTypePriceType
                            };
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }
        #endregion
    }
}