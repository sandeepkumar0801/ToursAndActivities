using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.PrioHub;
using Logger.Contract;
using ServiceAdapters.PrioHub.Constants;
using ServiceAdapters.PrioHub.PrioHub.Commands.Contract;
using ServiceAdapters.PrioHub.PrioHub.Converters.Contracts;
using ServiceAdapters.PrioHub.PrioHub.Entities;
using ServiceAdapters.PrioHub.PrioHub.Entities.CancelBookingResponse;
using ServiceAdapters.PrioHub.PrioHub.Entities.CancelReservationResponse;
using ServiceAdapters.PrioHub.PrioHub.Entities.GetVoucherRes;
using ServiceAdapters.PrioHub.PrioHub.Entities.ProductDetailResponse;
using ServiceAdapters.PrioHub.PrioHub.Entities.ProductListResponse;
using ServiceAdapters.PrioHub.PrioHub.Entities.RouteResponse;
using Util;
using ReservationData = ServiceAdapters.PrioHub.PrioHub.Entities.ReservationResponse;


namespace ServiceAdapters.PrioHub
{
    public class PrioHubAdapter : IPrioHubAdapter, IAdapter
    {
        private readonly List<PassengerType> _validPassengerTypes = new List<PassengerType> { PassengerType.Adult, PassengerType.Child, PassengerType.Infant, PassengerType.Senior, PassengerType.Family, PassengerType.Student, PassengerType.Youth };
        public static string _NewPrioServiceURL;
        //public static string _NewPrioApiDistributorId;

        //public static string _NewPrioServiceURLPrioOnly;
        //public static string _NewPrioApiDistributorIdPrioOnly;
        public static bool _isRollbackLiveAPIBookingsOtherThanPROD;

        #region "Private Members"

        private readonly IProductsCommandHandler _productsCommandHandler;
        private readonly IRouteCommandHandler _routesCommandHandler;

        private readonly IAvailabilityListCommandHandler _availablityListCmdHandler;
        private readonly IAvailablityListConverter _availablityListConverter;

        private readonly IProductDetailCommandHandler _productDetailCommandHandler;
        private readonly IProductDetailConverter _productDetailConverter;

        private readonly IReservationCommandHandler _reservationCommandHandler;
        private readonly IReservationConverter _reservationConverter;

        private readonly ICreateBookingCommandHandler _createBookingCommandHandler;
        private readonly ICreateBookingConverter _createBookingConverter;

        private readonly ICancelReservationCmdHandler _cancelReservationCmdHandler;
        private readonly ICancelReservationConverter _cancelReservationConverter;

        private readonly ICancelBookingCmdHandler _cancelBookingCmdHandler;
        private readonly ICancelBookingConverter _cancelBookingConverter;

        private readonly IGetVoucherCommandHandler _getVoucherCommandHandler;

        private readonly ILogger _log;
        private static readonly int maxParallelThreadCount;

        #endregion "Private Members"

        static PrioHubAdapter()
        {
            try
            {
                maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");

                _isRollbackLiveAPIBookingsOtherThanPROD = ConfigurationManagerHelper.GetValuefromAppSettings("isRollbackLiveAPIBookingsOtherThanPROD") == "1";
            }
            catch (Exception ex)
            {
                _isRollbackLiveAPIBookingsOtherThanPROD = false;
                maxParallelThreadCount = 1;
            }
        }

        public PrioHubAdapter(
             IProductsCommandHandler productsCommandHandler,
             IRouteCommandHandler routesCommandHandler,
             IAvailabilityListCommandHandler AvailabilityListCommandHandler,
             IAvailablityListConverter AvailablityListConverter,
             IProductDetailCommandHandler ProductDetailCommandHandler,
             IProductDetailConverter ProductDetailConverter,
             IReservationCommandHandler ReservationCommandHandler,
             IReservationConverter ReservationConverter,

             ICreateBookingCommandHandler CreateBookingCommandHandler,
             ICreateBookingConverter CreateBookingConverter,

             ICancelBookingCmdHandler cancelBookingCmdHandler,
             ICancelReservationCmdHandler cancelReservationCmdHandler,

             ICancelBookingConverter cancelBookingConverter,
             ICancelReservationConverter cancelReservationConverter,
             IGetVoucherCommandHandler getVoucherCommandHandler,
             ILogger log
            )
        {
            _productsCommandHandler = productsCommandHandler;
            _routesCommandHandler = routesCommandHandler;

            _availablityListCmdHandler = AvailabilityListCommandHandler;
            _availablityListConverter = AvailablityListConverter;

            _productDetailCommandHandler = ProductDetailCommandHandler;
            _productDetailConverter = ProductDetailConverter;

            _reservationCommandHandler = ReservationCommandHandler;
            _reservationConverter = ReservationConverter;

            _createBookingCommandHandler = CreateBookingCommandHandler;
            _createBookingConverter = CreateBookingConverter;

            _cancelBookingCmdHandler = cancelBookingCmdHandler;
            _cancelReservationCmdHandler = cancelReservationCmdHandler;

            _cancelBookingConverter = cancelBookingConverter;
            _cancelReservationConverter = cancelReservationConverter;
            _getVoucherCommandHandler = getVoucherCommandHandler;
            _log = log;
            _NewPrioServiceURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewPrioBaseAddress);
            //_NewPrioApiDistributorId = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewPrioApiDistributorId);

            //_NewPrioServiceURLPrioOnly = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.PrioHubBaseAddressOnlyPrioProducts);
            //_NewPrioApiDistributorIdPrioOnly = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewPrioApiDistributorIdPrioOnly);
        }

        public List<Item> ProductsAsync(
            PrioHubCriteria PrioHubCriteria,
            string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var activityDetailRs = new List<Item>();

            var actualDistributerId = PrioHubCriteria.DistributorId;
            var actualBaseURL = _NewPrioServiceURL;
            //if (actualDistributerId == Convert.ToInt32(_NewPrioApiDistributorIdPrioOnly))
            //{
            //    actualBaseURL = _NewPrioServiceURLPrioOnly;
            //}

            for (int i = 1; i < 15000; i += 50)
            {
                try
                {
                    PrioHubCriteria.PagingStartIndex = i;
                    PrioHubCriteria.ItemPerPage = 50;

                    var dataItemsFromAPI = new ProductListResponse();
                    var url = actualBaseURL
               + Constant.Products
               + "?distributor_id="
               + actualDistributerId
               + "&product_availability=true"
               + "&items_per_page=" + PrioHubCriteria.ItemPerPage
               + "&start_index=" + PrioHubCriteria.PagingStartIndex
               + "&cache=false";

                    var returnValue = _productsCommandHandler.Execute(Tuple.Create(url, actualDistributerId), token, MethodType.GetProducts, out request, out response);
                    if (returnValue != null)
                    {
                        dataItemsFromAPI = returnValue as ProductListResponse;
                        if (dataItemsFromAPI.Data.Items != null && dataItemsFromAPI.Data.Items.Count == 0)
                        {
                            break;
                        }
                        activityDetailRs.AddRange(dataItemsFromAPI?.Data?.Items);
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }
            return activityDetailRs;
        }

        public List<ItemRoute> ProductRoutesAsync(
           PrioHubCriteria PrioHubCriteria,
           string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var activityDetailRs = new List<ItemRoute>();

            var actualDistributerId = PrioHubCriteria.DistributorId;
            var actualBaseURL = _NewPrioServiceURL;
            //if (actualDistributerId == Convert.ToInt32(_NewPrioApiDistributorIdPrioOnly))
            //{
            //    actualBaseURL = _NewPrioServiceURLPrioOnly;
            //}

            for (int i = 1; i < 15000; i += 50)
            {
                try
                {
                    PrioHubCriteria.PagingStartIndex = i;
                    PrioHubCriteria.ItemPerPage = 50;

                    var dataItemsFromAPI = new RouteResponse();

                    var url = actualBaseURL
               + Constant.Products
               + Constant.Routes
               + "?distributor_id="
               + actualDistributerId
               + "&start_index=" + PrioHubCriteria.PagingStartIndex
               + "&items_per_page=" + PrioHubCriteria.ItemPerPage;

                    var returnValue = _routesCommandHandler.Execute(Tuple.Create(url, actualDistributerId), token, MethodType.GetProductRoutes, out request, out response);
                    if (returnValue != null)
                    {
                        dataItemsFromAPI = returnValue as RouteResponse;
                        if (dataItemsFromAPI.Data.Items != null && dataItemsFromAPI.Data.Items.Count == 0)
                        {
                            break;
                        }
                        activityDetailRs.AddRange(dataItemsFromAPI?.Data?.Items);
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }
            return activityDetailRs;
        }

        /// <summary>
        /// Update Option for Prio Activity
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<Activity> UpdateOptionforPrioHubActivity(PrioHubCriteria criteria, string token)
        {
            int? getIsangoId = 0;
            var activities = new List<Activity>();
            var productOptions = new List<ProductOption>();
            if (criteria?.SupplierMultipleCodes != null)
            {
                var apiActivityIds = criteria?.SupplierMultipleCodes;
                var nonEmptyValues = apiActivityIds;
                if (apiActivityIds.Contains(","))
                {
                    var nonEmptyValuesArray = apiActivityIds?.Split(',')?.Where(x => x != "")?.Select(sValue => sValue.Trim())?.ToArray();
                    apiActivityIds = string.Join(",", nonEmptyValuesArray);
                }

                //1. First API call (availabilityList)
                productOptions.AddRange(CreateActivityOptions(criteria, apiActivityIds, token));
                if (productOptions != null && productOptions.Count > 0)
                {
                    if (((ActivityOption)productOptions.FirstOrDefault()).Cluster != null)
                    {
                        var getApiCode = Convert.ToString(productOptions.Select(x => ((ActivityOption)x).SupplierOptionCode).FirstOrDefault());
                        getIsangoId = criteria?.ProductMapping?.Where(x => x.HotelBedsActivityCode == getApiCode)?.FirstOrDefault()?.IsangoHotelBedsActivityId;
                        var activity = new Activity();
                        if (getIsangoId > 0)//Dumping Case
                        {
                            activity.ID = Convert.ToInt32(getIsangoId);
                        }
                        else //Check Availability Case
                        {
                            activity.ID = int.Parse(criteria.IsangoActivityId);
                        }

                        activity.ProductOptions = productOptions;
                        activities.Add(activity);
                    }
                    else
                    {
                        var filterOptionsbyId = productOptions.GroupBy(x => ((ActivityOption)x).Code).ToList();
                        foreach (var groupItem in filterOptionsbyId)
                        {
                            var getApiCode = Convert.ToString(groupItem.Select(x => ((ActivityOption)x).Code).FirstOrDefault());
                            getIsangoId = criteria?.ProductMapping?.Where(x => x.HotelBedsActivityCode == getApiCode)?.FirstOrDefault()?.IsangoHotelBedsActivityId;
                            var activity = new Activity();
                            if (getIsangoId > 0)//Dumping Case
                            {
                                activity.ID = Convert.ToInt32(getIsangoId);
                            }
                            else //Check Availability Case
                            {
                                activity.ID = int.Parse(criteria.IsangoActivityId);
                            }
                            if (groupItem.Count() > 0)
                                activity.ProductOptions = groupItem.ToList();
                            activities.Add(activity);
                        }
                    }
                    ///Note:Reservation EndPoint call in CheckAvailability
                    ///1.) only for prioticket products exclude priohub products.
                    ///2.) Reservaiotion call not fire in pickup cases because pickup found in payment page.
                    ///3.) Token pass in case of "bookingrefenumber" because it is generate after booking.
                    ///4.) Cannot run this method for dumping otherwise for every product ,reservation call run for current date.
                    if (getIsangoId == 0 || getIsangoId == null)//non-dumping case
                    {
                        activities = CheckAvailabilityOptionsHaveReservation(activities, criteria.NoOfPassengers, token);
                    }
                }
                return activities;
            }
            return null;
        }

        private List<Activity> CheckAvailabilityOptionsHaveReservation(List<Activity> activities, Dictionary<PassengerType, int> noOfPassengers, string token)
        {
            try
            {
                var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount();
                if (maxParallelThreadCount > 4)
                {
                    maxParallelThreadCount = 4;
                }
                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount };

                var productOptions = activities?.FirstOrDefault()?.ProductOptions;

                if (productOptions != null)
                {
                    var ParallelProductOptions = new List<ProductOption>();
                    var taskArray = new Task<ProductOption>[productOptions.Count()];
                    var count = 0;
                    foreach (var item in productOptions)
                    {
                        taskArray[count] = Task.Factory.StartNew(() => PrioHubReservationandCancel(item, noOfPassengers, token));
                        count++;
                    }
                    try
                    {
                        if (taskArray?.Length > 0)
                        {
                            Task.WaitAll(taskArray);

                            Parallel.ForEach(taskArray, parallelOptions, task =>
                            {
                                //foreach (var task in taskArray?.Where(x => x != null).ToList())
                                //{
                                try
                                {
                                    var data = task?.GetAwaiter().GetResult();
                                    if (data != null)
                                        ParallelProductOptions.Add(data);
                                }
                                catch (Exception ex)
                                {
                                    Task.Run(() => _log.Error(new IsangoErrorEntity
                                    {
                                        ClassName = "FareHarborCriteriaService",
                                        MethodName = "GetAvailability",
                                        Token = token
                                    }, ex));

                                    // throw;
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        //do nothing
                    }
                    productOptions = ParallelProductOptions;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PrioHub.PrioHubAdapter",
                    MethodName = "CheckAvailabilityOptionsHaveReservation",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return activities;
        }

        public ProductOption PrioHubReservationandCancel(ProductOption productOption, Dictionary<PassengerType, int> noOfPassengers, string token)
        {
            try
            {
                var request = string.Empty;
                var response = string.Empty;

                //Pax Filter Start
                var criteriaPaxes = string.Empty;
                var puPaxes = string.Empty;

                var cPaxes = noOfPassengers?.Keys?.ToArray()?
                    .ToList()?
                    .Where(y => y != PassengerType.Infant)?
                    .OrderBy(x => x);

                criteriaPaxes = string.Join(",", cPaxes);
                //Pax Filter Start
                var datePriceAndAvailabilty = productOption?.BasePrice?.DatePriceAndAvailabilty ??
                                                         productOption?.GateBasePrice?.DatePriceAndAvailabilty ??
                                                         productOption?.CostPrice?.DatePriceAndAvailabilty ??
                                                         productOption?.GateSellPrice?.DatePriceAndAvailabilty ??
                                                         productOption?.SellPrice?.DatePriceAndAvailabilty;

                if (datePriceAndAvailabilty?.Any() == false || datePriceAndAvailabilty == null)
                {
                    return productOption;
                }

                var puQuery = from pa in datePriceAndAvailabilty
                              from pu in pa.Value?.PricingUnits
                              from nop in noOfPassengers
                              where ((PerPersonPricingUnit)pu)?.PassengerType == nop.Key
                              && ((PerPersonPricingUnit)pu)?.PassengerType != PassengerType.Infant
                              && ((PerPersonPricingUnit)pu) != null
                              select ((PerPersonPricingUnit)pu)?.PassengerType;

                if (puQuery?.Any() == false || puQuery == null)
                {
                    return productOption;
                }
                var pricingUnits = puQuery?.OrderBy(x => x)?.Distinct().ToList();

                if (pricingUnits?.Count > 0)
                {
                    puPaxes = string.Join(",", pricingUnits)?.ToLower();
                    criteriaPaxes = criteriaPaxes?.ToLower();
                    puPaxes = puPaxes?.ToLower();

                    if (criteriaPaxes != puPaxes
                        && !string.IsNullOrWhiteSpace(puPaxes)
                    )
                    {
                        return productOption;
                    }
                }
                //Pax Filter End

                var validDates = productOption?.BasePrice?.DatePriceAndAvailabilty?.Keys;
                int processedValidDatesLoop = 0;
                string processedProductOptionLoop = string.Empty;
                if (validDates != null)
                {
                    foreach (var date in validDates) //exit after 2 dates with "processedValidDatesLoop"
                    {

                        if (processedProductOptionLoop == "FINDRESERVATIONAVAILABLE")
                        {
                            break;
                        }
                        var checkInDate = date;
                        var activityOptionConversion = (ActivityOption)productOption;
                        //1.) only for prioticket products exclude priohub
                        // if ((activityOptionConversion)?.PrioHubDistributerId == _NewPrioApiDistributorIdPrioOnly)
                        //{
                        var pickupPoint = (activityOptionConversion)?.PickupPoints;
                        //2.) reservaiotion call not fire in pickup cases
                        if (String.IsNullOrEmpty(pickupPoint) || pickupPoint?.ToUpper() != "MANDATORY")
                        {
                            //only check for two  Dates 
                            var basePriceAndAvailability = (PrioHubPriceAndAvailability)GetPriceAndAvailability(productOption?.BasePrice, checkInDate);
                            var costPriceAndAvailability = (PrioHubPriceAndAvailability)GetPriceAndAvailability(productOption?.CostPrice, checkInDate);
                            var gateBasePriceAndAvailability = (PrioHubPriceAndAvailability)GetPriceAndAvailability(productOption?.GateBasePrice, checkInDate);
                            //if  not-available, then out of date loop, no need reservation call
                            if (basePriceAndAvailability.AvailabilityStatus == AvailabilityStatus.NOTAVAILABLE)
                            {
                                break;
                            }

                            if (basePriceAndAvailability != null)
                            {
                                var getPrioHubAvailabilityId = basePriceAndAvailability?.AvailabilityId;
                                //3.) token pass in case of "bookingrefenumber"
                                var reservationResponse = CreateReservationThenCancel(productOption, token, getPrioHubAvailabilityId, Convert.ToString(Math.Abs(Guid.NewGuid().GetHashCode())), out request, out response);
                                //check Reservation Success then Cancel It.
                                if (reservationResponse != null && reservationResponse?.Item3?.ToUpper() == "BOOKING_RESERVED")
                                {
                                    processedProductOptionLoop = "FINDRESERVATIONAVAILABLE";
                                    var prioHubSelectedProduct = new PrioHubSelectedProduct
                                    {
                                        PrioHubDistributerId = (activityOptionConversion)?.PrioHubDistributerId,
                                        PrioReservationReference = reservationResponse?.Item1
                                    };
                                    var cancellationStatus = CancelReservation(prioHubSelectedProduct, token, out request, out response);
                                    if (cancellationStatus?.Item3 == "BOOKING_RESERVATION_CANCELLED".ToString())
                                    {
                                        var isangoErrorEntity = new IsangoErrorEntity
                                        {
                                            ClassName = "PrioHub.PrioHubAdapter",
                                            MethodName = "CheckAvailabilityOptionsHaveReservation",
                                            Token = token,
                                            Params = cancellationStatus.Item3
                                        };
                                        _log.Info(isangoErrorEntity);
                                    }

                                }
                                else
                                {
                                    //no Reservation availability- then set no prio availability
                                    basePriceAndAvailability.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                                    costPriceAndAvailability.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                                    gateBasePriceAndAvailability.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                                    //processedProductOptionLoop = "FINDNOTAVAILABLE";
                                    //var lstError = new Error();
                                    //var errorData = new Error
                                    //{
                                    //    HttpStatus = System.Net.HttpStatusCode.NotFound,
                                    //    Message = "Reservation not available on" + checkInDate
                                    //};
                                    //activities.FirstOrDefault().Errors.Add(errorData);
                                }
                            }
                        }
                        //}
                        if (++processedValidDatesLoop == 2) break;
                    }
                }
                return productOption;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PrioHub.PrioHubAdapter",
                    MethodName = "CheckAvailabilityOptionsHaveReservation",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                return productOption;
            }
        }

        protected PriceAndAvailability GetPriceAndAvailability(Price price, DateTime startDate)
        {
            try
            {
                return price?.DatePriceAndAvailabilty[startDate];
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private List<ActivityOption> CreateActivityOptions(PrioHubCriteria criteria, string apiActivityIds, string token)
        {
            var activityOptionsFinal = new List<ActivityOption>();
            var comboSubProducts = new List<PrioHubComboSubProduct>();
            try
            {
                var isDumping = 1;
                if (criteria?.ProductMapping == null)// Non-Dumping Case(check-availability)
                {
                    isDumping = 0;
                }

                var actualDistributerId = Convert.ToString(criteria.DistributorId);
                var actualBaseURL = _NewPrioServiceURL;
                //if (actualDistributerId == _NewPrioApiDistributorIdPrioOnly)
                //{
                //    actualBaseURL = _NewPrioServiceURLPrioOnly;
                //}

                //1. First API call (availabilityList)
                var prioAvailablityMultipleItems = GetPrioHubAvailablityAsync(criteria, actualDistributerId, actualBaseURL, token, apiActivityIds, isDumping);
                if (isDumping == 0)//check availability case
                {
                    var matchingSelectionProducts = prioAvailablityMultipleItems?.Where(x => x.PrioHubProductId == apiActivityIds)?.ToList();
                    var nonMatchingSelectionProducts = prioAvailablityMultipleItems?.Where(x => x.PrioHubProductId != apiActivityIds);
                    //Means Combo Products
                    if (nonMatchingSelectionProducts != null && nonMatchingSelectionProducts.Count() > 0)
                    {
                        foreach (var item in nonMatchingSelectionProducts?.ToList())
                        {
                            if (item?.BasePrice?.DatePriceAndAvailabilty != null)
                            {
                                foreach (var dateData in item?.BasePrice?.DatePriceAndAvailabilty)
                                {
                                    var comboProduct = new PrioHubComboSubProduct
                                    {
                                        AvailabilityId = ((PrioHubPriceAndAvailability)dateData.Value)?.AvailabilityId,
                                        AvailabilityProductId = item?.PrioHubProductId,
                                        AvailabilityFromDateTime = Convert.ToString(dateData.Key)
                                    };
                                    comboSubProducts.Add(comboProduct);
                                }
                            }
                        };
                        prioAvailablityMultipleItems = matchingSelectionProducts?.ToList();
                    }
                }
                //Check Products that are ignored and have product_availability:false

                string[] values = apiActivityIds?.Split(',');
                var ignorevaluesMultiples = new List<string>();
                if (values != null && values.Length > 0)
                {
                    foreach (var ignoreItem in values)
                    {
                        if (prioAvailablityMultipleItems != null)
                        {
                            var check = prioAvailablityMultipleItems.Any(x => x.PrioHubProductId == ignoreItem);
                            if (!check)
                            {
                                ignorevaluesMultiples.Add(ignoreItem);
                            }
                        }
                        else
                        {
                            ignorevaluesMultiples.Add(ignoreItem);
                        }
                    }
                }
                //Step1: Loop of product_availability:true values
                if (prioAvailablityMultipleItems != null && prioAvailablityMultipleItems?.Count > 0)
                {
                    foreach (var prioAvailablity in prioAvailablityMultipleItems)
                    {
                        var activityOptionsList = new List<ActivityOption>();
                        if (prioAvailablity == null) return null;

                        if (prioAvailablity.AvailabilityStatus == AvailabilityStatus.AVAILABLE)
                        {
                            var taskCreateOptions = new List<ActivityOption>();
                            var taskCreateOption = new List<ActivityOption>();
                            if (prioAvailablity.IsTimeBasedOption)
                            {
                                //2.Call ProductDetail API
                                taskCreateOptions = CreateOptionsAsync(prioAvailablity, criteria, token, prioAvailablity?.PrioHubProductId, actualBaseURL);
                                activityOptionsList.AddRange(taskCreateOptions);
                            }
                            else
                            {
                                //2.Call ProductDetail API
                                taskCreateOption = CreateOptionAsync(prioAvailablity, criteria, token, prioAvailablity?.PrioHubProductId, actualBaseURL);
                                activityOptionsList.AddRange(taskCreateOption);
                            }
                            if (activityOptionsList != null && activityOptionsList?.Count > 0)
                            {
                                for (int activityOpt = 0; activityOpt < activityOptionsList.Count; activityOpt++)
                                {
                                    var isangoOptionId = 0;
                                    if (criteria.ProductMapping != null)
                                    {
                                        isangoOptionId = Convert.ToInt32(criteria?.ProductMapping?.Where(x => x.HotelBedsActivityCode == prioAvailablity.PrioHubProductId)?.FirstOrDefault()?.ServiceOptionInServiceid);
                                    }

                                    //Cluster Product
                                    if (activityOptionsList[activityOpt].Cluster != null)
                                    {
                                        activityOptionsList[activityOpt].SupplierOptionCode = prioAvailablity?.PrioHubProductId;
                                        activityOptionsList[activityOpt].Code = Convert.ToString(activityOptionsList[activityOpt]?.Cluster?.ProductId);
                                        activityOptionsList[activityOpt].TravelInfo = new TravelInfo { NoOfPassengers = criteria.NoOfPassengers };
                                        activityOptionsList[activityOpt].PrioHubComboSubProduct = comboSubProducts;
                                        activityOptionsList[activityOpt].Id = Convert.ToInt32(activityOptionsList[activityOpt]?.Cluster?.ProductId);
                                        activityOptionsList[activityOpt].PrioHubDistributerId = activityOptionsList[activityOpt]?.PrioHubDistributerId;
                                        activityOptionsList[activityOpt].ServiceOptionId = isangoOptionId;
                                        activityOptionsList[activityOpt].CancellationText = activityOptionsList[activityOpt].CancellationText;
                                        activityOptionsList[activityOpt].Cancellable = activityOptionsList[activityOpt].Cancellable;
                                        activityOptionsList[activityOpt].ApiCancellationPolicy = activityOptionsList[activityOpt].ApiCancellationPolicy;
                                    }
                                    else //Simple Product
                                    {
                                        activityOptionsList[activityOpt].SupplierOptionCode = prioAvailablity?.PrioHubProductId;
                                        activityOptionsList[activityOpt].Code = prioAvailablity?.PrioHubProductId;
                                        activityOptionsList[activityOpt].TravelInfo = new TravelInfo { NoOfPassengers = criteria.NoOfPassengers };
                                        activityOptionsList[activityOpt].PrioHubComboSubProduct = comboSubProducts;
                                        activityOptionsList[activityOpt].PrioHubDistributerId = activityOptionsList[activityOpt]?.PrioHubDistributerId;
                                        activityOptionsList[activityOpt].ServiceOptionId = isangoOptionId;
                                        activityOptionsList[activityOpt].CancellationText = activityOptionsList[activityOpt].CancellationText;
                                        activityOptionsList[activityOpt].Cancellable = activityOptionsList[activityOpt].Cancellable;
                                        activityOptionsList[activityOpt].ApiCancellationPolicy = activityOptionsList[activityOpt].ApiCancellationPolicy;
                                    }
                                }
                            }
                        }
                        if (activityOptionsList != null && activityOptionsList.Count > 0)
                        {
                            activityOptionsFinal.AddRange(activityOptionsList);
                        }
                    }
                }
                //Step2: loop of ignorevaluesMultiples values or product_availability:true
                if (ignorevaluesMultiples != null && ignorevaluesMultiples?.Count > 0)
                {

                    foreach (var ignorevaluesMultiple in ignorevaluesMultiples)
                    {
                        var activityOptionsList = new List<ActivityOption>();
                        if (ignorevaluesMultiple == null) return null;

                        var taskCreateOptions = new List<ActivityOption>();
                        var taskCreateOption = new List<ActivityOption>();

                        //2.Call ProductDetail API
                        taskCreateOption = CreateOptionAsync(null, criteria, token, ignorevaluesMultiple, actualBaseURL);
                        if (taskCreateOption != null && taskCreateOption.Count > 0)
                        {
                            activityOptionsList.AddRange(taskCreateOption);
                        }

                        if (activityOptionsList != null && activityOptionsList?.Count > 0)
                        {
                            for (int activityOpt = 0; activityOpt < activityOptionsList.Count; activityOpt++)
                            {
                                var isangoOptionId = 0;
                                if (criteria.ProductMapping != null)
                                {
                                    isangoOptionId = Convert.ToInt32(criteria?.ProductMapping?.Where(x => x.HotelBedsActivityCode == ignorevaluesMultiple)?.FirstOrDefault()?.ServiceOptionInServiceid);
                                }

                                //Cluster Product
                                if (activityOptionsList[activityOpt].Cluster != null)
                                {
                                    activityOptionsList[activityOpt].SupplierOptionCode = ignorevaluesMultiple;
                                    activityOptionsList[activityOpt].Code = Convert.ToString(activityOptionsList[activityOpt]?.Cluster?.ProductId);
                                    activityOptionsList[activityOpt].TravelInfo = new TravelInfo { NoOfPassengers = criteria.NoOfPassengers };
                                    activityOptionsList[activityOpt].PrioHubComboSubProduct = comboSubProducts;
                                    activityOptionsList[activityOpt].Id = Convert.ToInt32(activityOptionsList[activityOpt]?.Cluster?.ProductId);
                                    activityOptionsList[activityOpt].PrioHubDistributerId = activityOptionsList[activityOpt]?.PrioHubDistributerId;
                                    activityOptionsList[activityOpt].ServiceOptionId = isangoOptionId;
                                    activityOptionsList[activityOpt].CancellationText = activityOptionsList[activityOpt].CancellationText;
                                    activityOptionsList[activityOpt].Cancellable = activityOptionsList[activityOpt].Cancellable;
                                    activityOptionsList[activityOpt].ApiCancellationPolicy = activityOptionsList[activityOpt].ApiCancellationPolicy;
                                }
                                else //Simple Product
                                {
                                    activityOptionsList[activityOpt].SupplierOptionCode = ignorevaluesMultiple;
                                    activityOptionsList[activityOpt].Code = ignorevaluesMultiple;
                                    activityOptionsList[activityOpt].TravelInfo = new TravelInfo { NoOfPassengers = criteria.NoOfPassengers };
                                    activityOptionsList[activityOpt].PrioHubComboSubProduct = comboSubProducts;
                                    activityOptionsList[activityOpt].PrioHubDistributerId = activityOptionsList[activityOpt]?.PrioHubDistributerId;
                                    activityOptionsList[activityOpt].ServiceOptionId = isangoOptionId;
                                    activityOptionsList[activityOpt].CancellationText = activityOptionsList[activityOpt].CancellationText;
                                    activityOptionsList[activityOpt].Cancellable = activityOptionsList[activityOpt].Cancellable;
                                    activityOptionsList[activityOpt].ApiCancellationPolicy = activityOptionsList[activityOpt].ApiCancellationPolicy;
                                }
                            }
                        }
                        activityOptionsFinal.AddRange(activityOptionsList);
                    }

                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PrioHub.PrioAdapter",
                    MethodName = "CreateActivityOption",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return activityOptionsFinal;
        }

        public List<ActivityOption> GetPrioHubAvailablityAsync(PrioHubCriteria criteria, string distributorId,
            string actualBaseURL, string apiLoggingToken, string apiActivityIds, int isDumping)
        {
            var request = string.Empty;
            var response = string.Empty;
            var _returnValue = default(object);
            if (criteria != null)
            {
                var extraData = string.Empty;
                if (isDumping == 0)// Non-Dumping Case(check-availability)
                {
                    extraData = "&sub_products_depth=1";
                    //it will give combi products at second level,
                    //Dumping require only at first level.
                }

                //Create REQEST URL;
                var requestString =
                 actualBaseURL
               + Constant.Availability
               + "?distributor_id="
               + distributorId
               + "&product_id=" + apiActivityIds
               + "&from_date=" + DateTime.Parse(Convert.ToString(criteria.CheckinDate)).ToString(Constant.DateFormat)
               + "&to_date=" + DateTime.Parse(Convert.ToString(criteria.CheckoutDate)).ToString(Constant.DateFormat)
               + extraData;
                _returnValue = _availablityListCmdHandler.Execute(Tuple.Create(requestString, Convert.ToInt32(distributorId)), apiLoggingToken, MethodType.GetAvailability, out request, out response);
                if (_returnValue != null)
                    try
                    {
                        _returnValue = _availablityListConverter.Convert(_returnValue);
                    }
                    catch (Exception ex)
                    {
                        //ignored - Type cast Error in case of differnet response than what is expected.
                    }
            }
            return _returnValue as List<ActivityOption>;
        }

        public object GetPrioTicketDetailsAsyncV2(string apiCode,
            string actualBaseAddress, string apiLoggingToken,
            ActivityOption activityOptionAPI, int timeBased, PrioHubCriteria criteria)
        {
            var request = string.Empty;
            var response = string.Empty;
            var _returnValueAPI = default(object);
            var _returnConverterValue = default(object);
            var criteriAvail = new PrioHubCriteria();
            var criteriaConverter = new PrioHubCriteria();
            var finalDistributorId = criteria.DistributorId;

            if (!String.IsNullOrEmpty(apiCode))
            {
                //Create REQEST URL;
                var requesUrl = actualBaseAddress
               + Constant.Products + "/"
               + apiCode
               + "?distributor_id="
               + finalDistributorId
               + "&cache=false";

                _returnValueAPI = _productDetailCommandHandler.Execute(Tuple.Create(requesUrl, finalDistributorId), apiLoggingToken, MethodType.ProductDetails, out request, out response);
            }
            if (_returnValueAPI != null && !(_returnValueAPI is string))
            {
                criteria.ProductDetailResponseAPI = _returnValueAPI as ProductDetailResponse;
                criteria.ActivityOptionAPI = activityOptionAPI;
                criteria.Token = apiLoggingToken;
                criteria.TimeBased = timeBased;
                _returnConverterValue = _productDetailConverter.Convert(criteria);
            }
            else
            {
                return null;
            }
            return _returnConverterValue as List<ActivityOption>;
        }

        /// <summary>
        /// Create Reservation
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <param name="distributorReference"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Tuple<string, string, string, string> CreateReservation(Isango.Entities.PrioHub.PrioHubSelectedProduct selectedProduct,
            string distributorReference, out string request, out string response, string token, string bookingReference,
            string prioHubAvailabilityId, List<PrioHubProductPaxMapping> prioHubProductPaxMapping)
        {
            request = string.Empty;
            response = string.Empty;
            var _returnValue = default(object);
            var _inputContext = new InputContext();

            if (selectedProduct != null)
            {
                var productOptions = selectedProduct.ProductOptions.SingleOrDefault(x => x.IsSelected);
                _inputContext.ActivityId = Convert.ToString(((ActivityOption)productOptions)?.SupplierOptionCode);
                _inputContext.PrioHubProductPaxMapping = prioHubProductPaxMapping;
                _inputContext.PrioHubAvailabilityId = prioHubAvailabilityId;
                _inputContext.BookingReference = bookingReference;
                AddPassengerCountInInputContext(productOptions, _inputContext);

                _inputContext.DistributorReference = distributorReference;
                _inputContext.PickupPointId = string.Empty;
                _inputContext.PickupPoints = selectedProduct?.PickupPoints;
                _inputContext.ProductCombiDetails = selectedProduct?.ProductCombiDetails;
                _inputContext.PrioHubComboSubProduct = selectedProduct?.PrioHubComboSubProduct;
                _inputContext.PrioHubClusterProduct = selectedProduct?.Cluster;
                _inputContext.PrioHubDistributerId = selectedProduct.PrioHubDistributerId;
                if (selectedProduct.PickupPoints?.ToUpper() == "MANDATORY")
                {
                    _inputContext.PickupPointId = selectedProduct.HotelPickUpLocation.Split('-')[0]; //Picking up First PickUp Point
                }

                if (productOptions != null)
                {
                    var priceAndAvailabiltySelected = productOptions.BasePrice.DatePriceAndAvailabilty.Where(x => x.Value.IsSelected);
                    var prioPandA = priceAndAvailabiltySelected.SingleOrDefault().Value as PrioHubPriceAndAvailability;
                    _inputContext.CheckInDate = prioPandA?.FromDateTime;
                    _inputContext.CheckOutDate = prioPandA?.ToDateTime;
                }

                _inputContext.MethodType = MethodType.Reservation;

                _returnValue = _reservationCommandHandler.Execute(_inputContext, token, _inputContext.MethodType, out request, out response);

                #region Rollback/Cancel/Delete Reservation

                /*
                if (_isRollbackLiveAPIBookingsOtherThanPROD)
                {
                    var reservationCancelReq = string.Empty;
                    var reservationCancelRes = string.Empty;
                    var reservationResponse = SerializeDeSerializeHelper.DeSerialize<ServiceAdapters.PrioHub.PrioHub.Entities.ReservationResponse.ReservationResponse>(_returnValue.ToString());

                    var CancelReservationResponse = this.CancelReservation(
                        new PrioHubSelectedProduct
                        {
                            PrioHubDistributerId = reservationResponse.Data.Reservation.ReservationDistributorId,
                            PrioReservationReference = reservationResponse.Data.Reservation.ReservationReference,
                            PrioDistributorReference = reservationResponse.Data.Reservation.ReservationDetails.FirstOrDefault(x => x.BookingReservationReference?.Contains(reservationResponse.Data.Reservation.ReservationReference) == true)
                            ?.BookingReservationReference,
                            PrioHubReservationStatus = reservationResponse.Data.Reservation.ReservationDetails.FirstOrDefault(x => x.BookingReservationReference?.Contains(reservationResponse.Data.Reservation.ReservationReference) == true)
                            ?.BookingStatus
                        }
                        , token
                        , out reservationCancelReq
                        , out reservationCancelRes
                        );

                    if (CancelReservationResponse == null)
                    {
                    }
                }
                */

                #endregion Rollback/Cancel/Delete Reservation

                if (_returnValue != null)
                {
                    _returnValue = _reservationConverter.Convert((string)_returnValue);
                }
            }
            return (Tuple<string, string, string, string>)_returnValue;
        }


        /// <summary>
        /// Create Reservation
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <param name="distributorReference"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Tuple<string, string, string, string> CreateReservationThenCancel(
            ProductOption productOption, string token, string prioHubAvailabilityId
            , string refId, out string request, out string response
            )
        {
            request = string.Empty;
            response = string.Empty;
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            var activityOption = ((ActivityOption)productOption);

            if (productOption != null)
            {
                var productOptions = productOption;
                ////Assignment
                _inputContext.ActivityId = Convert.ToString(((ActivityOption)productOptions)?.SupplierOptionCode);
                _inputContext.PrioHubProductPaxMapping = activityOption?.PrioHubProductPaxMapping;
                _inputContext.PrioHubAvailabilityId = prioHubAvailabilityId;
                _inputContext.PrioHubProductGroupCode = activityOption?.PrioHubProductGroupCode;
                _inputContext.PrioHubProductTypeStatus = activityOption.PrioHubProductTypeStatus;

                _inputContext.BookingReference = refId;
                AddPassengerCountInInputContext(productOptions, _inputContext);
                _inputContext.DistributorReference = refId;
                _inputContext.PickupPointId = string.Empty;
                ///Combi Products
                _inputContext.ProductCombiDetails = activityOption?.ProductCombiDetails;
                _inputContext.PrioHubComboSubProduct = activityOption?.PrioHubComboSubProduct;
                ////Cluster Product
                _inputContext.PrioHubClusterProduct = activityOption?.Cluster;
                _inputContext.PrioHubDistributerId = activityOption?.PrioHubDistributerId;

                _inputContext.MethodType = MethodType.Reservation;
                _returnValue = _reservationCommandHandler.Execute(_inputContext, token, _inputContext.MethodType, out request, out response);

                if (_returnValue != null)
                {
                    _returnValue = _reservationConverter.Convert((string)_returnValue);
                }
            }
            return (Tuple<string, string, string, string>)_returnValue;
        }

        /// <summary>
        /// Create Reservation
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <param name="distributorReference"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ReservationData.ReservationResponse CreateReservationOnly(Isango.Entities.PrioHub.PrioHubSelectedProduct selectedProduct,
            string distributorReference, out string request, out string response, string token, string bookingReference,
            string prioHubAvailabilityId, List<PrioHubProductPaxMapping> prioHubProductPaxMapping)
        {
            request = string.Empty;
            response = string.Empty;
            var _returnValue = default(object);
            var _inputContext = new InputContext();

            if (selectedProduct != null)
            {
                var productOptions = selectedProduct.ProductOptions.SingleOrDefault(x => x.IsSelected);
                _inputContext.ActivityId = Convert.ToString(((ActivityOption)productOptions)?.SupplierOptionCode);
                _inputContext.PrioHubProductPaxMapping = prioHubProductPaxMapping;
                _inputContext.PrioHubAvailabilityId = prioHubAvailabilityId;
                _inputContext.BookingReference = bookingReference;
                AddPassengerCountInInputContext(productOptions, _inputContext);

                _inputContext.DistributorReference = distributorReference;
                _inputContext.PickupPointId = string.Empty;
                _inputContext.PickupPoints = selectedProduct?.PickupPoints;
                _inputContext.ProductCombiDetails = selectedProduct?.ProductCombiDetails;
                _inputContext.PrioHubComboSubProduct = selectedProduct?.PrioHubComboSubProduct;
                _inputContext.PrioHubClusterProduct = selectedProduct?.Cluster;
                _inputContext.PrioHubDistributerId = selectedProduct.PrioHubDistributerId;
                if (selectedProduct.PickupPoints?.ToUpper() == "MANDATORY")
                {
                    _inputContext.PickupPointId = selectedProduct.HotelPickUpLocation.Split('-')[0]; //Picking up First PickUp Point
                }

                if (productOptions != null)
                {
                    var priceAndAvailabiltySelected = productOptions.BasePrice.DatePriceAndAvailabilty.Where(x => x.Value.IsSelected);
                    var prioPandA = priceAndAvailabiltySelected.SingleOrDefault().Value as PrioHubPriceAndAvailability;
                    _inputContext.CheckInDate = prioPandA?.FromDateTime;
                    _inputContext.CheckOutDate = prioPandA?.ToDateTime;
                }

                _inputContext.MethodType = MethodType.Reservation;

                _returnValue = _reservationCommandHandler.Execute(_inputContext, token, _inputContext.MethodType, out request, out response);
                if (_returnValue != null)
                {
                    var reservationRs = SerializeDeSerializeHelper.DeSerialize<ReservationData.ReservationResponse>(_returnValue.ToString());
                    return reservationRs;
                }
                else
                {
                    return null;

                }
            }
            return null;
        }

        /// <summary>
        /// Create Booking
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public PrioHubAPITicket CreateBooking
            (PrioHubSelectedProduct selectedProduct, string token,
            out string request, out string response,
            string referenceNumber)
        {


            request = string.Empty;
            response = string.Empty;
            try
            {
                var _returnValue = default(object);
                var _inputContext = new InputContext();
                if (selectedProduct != null)
                {
                    var productOptions = selectedProduct.ProductOptions.SingleOrDefault(x => x.IsSelected);
                    _inputContext.ActivityId = Convert.ToString(((ActivityOption)productOptions)?.SupplierOptionCode);

                    var leadPassenger = productOptions?.Customers.FirstOrDefault(x => x.IsLeadCustomer);

                    _inputContext.BookingName = $"{leadPassenger?.FirstName} {leadPassenger?.LastName}";

                    _inputContext.BookingFirstName = leadPassenger?.FirstName;
                    _inputContext.BookingLastName = leadPassenger?.LastName;

                    _inputContext.BookingEmail = leadPassenger?.Email;
                    _inputContext.ReservationReference = selectedProduct.PrioReservationReference;
                    _inputContext.BookingReference = referenceNumber;
                    _inputContext.Language = "en";
                    _inputContext.BookingOptionType = "CONFIRM_RESERVATION";
                    _inputContext.PrioHubDistributerId = selectedProduct.PrioHubDistributerId;
                    _inputContext.MethodType = MethodType.CreateBooking;
                    _inputContext.Customers = productOptions?.Customers;
                    _inputContext.PrioHubProductPaxMapping = selectedProduct?.PrioHubProductPaxMapping;
                    _inputContext.TourDate = productOptions.TravelInfo.StartDate;
                    _returnValue = _createBookingCommandHandler.Execute(_inputContext, token, _inputContext.MethodType, out request, out response);

                    #region Rollback/Cancel/Delete Booking

                    /*
                    if (_isRollbackLiveAPIBookingsOtherThanPROD)
                    {
                        var bookingCancelReq = string.Empty;
                        var bookingCancelRes = string.Empty;
                        var createBookingResponseAPI = SerializeDeSerializeHelper.DeSerialize<ServiceAdapters.PrioHub.PrioHub.Entities.CreateBookingResponse.CreateBookingResponse>(_returnValue?.ToString());

                        var CancelBookingResponse = this.CancelBooking(
                            new PrioHubSelectedProduct
                            {
                                PrioHubDistributerId = createBookingResponseAPI?.Data?.Order?.OrderDistributorId,
                                PrioReservationReference = selectedProduct.PrioReservationReference,
                                PrioDistributorReference = createBookingResponseAPI?.Data?.Order?.OrderExternalReference ?? selectedProduct.PrioDistributorReference,
                                PrioHubReservationStatus = createBookingResponseAPI?.Data?.Order?.OrderStatus,
                                PrioHubApiConfirmedBooking = new PrioHubAPITicket
                                {
                                    BookingStatus = createBookingResponseAPI?.Data?.Order?.OrderStatus,
                                    BookingReference = createBookingResponseAPI?.Data?.Order?.OrderReference,
                                }
                            }
                            , token
                            , out bookingCancelReq
                            , out bookingCancelRes
                            );

                        if (CancelBookingResponse == null)
                        {
                        }
                    }
                    */

                    #endregion Rollback/Cancel/Delete Booking

                    if (_returnValue == null)
                    {
                        return null;
                    }

                    _returnValue = _createBookingConverter.Convert((string)_returnValue);
                }
                return (PrioHubAPITicket)_returnValue;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreatePrioHubProductsBooking",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                return null;
            }
        }

        /// <summary>
        /// CancelBooking
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public Tuple<string, string, string, string, string, DateTime> CancelBooking(PrioHubSelectedProduct selectedProduct, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var _returnValue = default(object);
            var _inputContext = new InputContext();

            if (selectedProduct != null)
            {
                var actualBaseURL = _NewPrioServiceURL;
                var actualDistributeID = Convert.ToInt32(selectedProduct?.PrioHubDistributerId);
                //if (actualDistributeID == Convert.ToInt32(_NewPrioApiDistributorIdPrioOnly))
                //{
                //    actualBaseURL = _NewPrioServiceURLPrioOnly;
                //}

                var requesUrl = actualBaseURL
                 + Constant.Booking + "/" + selectedProduct?.PrioHubApiConfirmedBooking?.BookingReference;

                _returnValue = _cancelBookingCmdHandler.Execute(Tuple.Create(requesUrl, actualDistributeID), token, MethodType.CancelBooking, out request, out response);
                if (_returnValue == null)
                    return null;
                _returnValue = _cancelBookingConverter.Convert((CancelBookingResponse)_returnValue);
            }

            return (Tuple<string, string, string, string, string, DateTime>)_returnValue;
        }

        /// <summary>
        /// Cancel Reservation
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public Tuple<string, string, string, DateTime> CancelReservation(PrioHubSelectedProduct selectedProduct, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            if (selectedProduct != null)
            {
                var actualBaseURL = _NewPrioServiceURL;
                var actualDistributerId = Convert.ToInt32(selectedProduct.PrioHubDistributerId);
                //if (actualDistributerId == Convert.ToInt32(_NewPrioApiDistributorIdPrioOnly))
                //{
                //    actualBaseURL = _NewPrioServiceURLPrioOnly;
                //}

                var requesUrl = actualBaseURL
                + Constant.Reservations + "/" + selectedProduct.PrioReservationReference;

                _returnValue = _cancelReservationCmdHandler.Execute(Tuple.Create(requesUrl, actualDistributerId), token, MethodType.CancelReservation, out request, out response);
                if (_returnValue != null)
                    _returnValue = _cancelReservationConverter.Convert((CancelReservationResponse)_returnValue);
            }

            return (Tuple<string, string, string, DateTime>)_returnValue;
        }


        public GetVoucherRes GetVoucher(int distributorId, string bookingOrderId,
         string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var getVoucherRes = new GetVoucherRes();
            var actualDistributerId = distributorId;
            var actualBaseURL = _NewPrioServiceURL;
            //if (actualDistributerId == Convert.ToInt32(_NewPrioApiDistributorIdPrioOnly))
            //{
            //    actualBaseURL = _NewPrioServiceURLPrioOnly;
            //}
            var url = actualBaseURL + Constant.Orders + "/" + bookingOrderId + "/voucher";
            var returnValue = _getVoucherCommandHandler.Execute(Tuple.Create(url, actualDistributerId), token, MethodType.GetVoucher, out request, out response);
            if (returnValue != null)
            {
                getVoucherRes = returnValue as GetVoucherRes;
            }
            return getVoucherRes;
        }

        private List<ActivityOption> CreateOptionsAsync(ActivityOption activityOption, PrioHubCriteria criteria, string token, string apiCode, string actualBaseAddress)
        {
            try
            {
                var timeBased = 1;
                var finalConverterDataSend = GetPrioTicketDetailsAsyncV2(apiCode, actualBaseAddress, token, activityOption, timeBased, criteria) as List<ActivityOption>;
                return finalConverterDataSend;
            }
            catch (Exception ex)
            {
                _log.Error(new Isango.Entities.IsangoErrorEntity
                {
                    ClassName = "PrioHubAdapter",
                    MethodName = "CreateOption",
                    Token = token
                }, ex);
            }
            return null;
        }

        private List<ActivityOption> CreateOptionAsync(ActivityOption activityOption, PrioHubCriteria criteria, string token, string code, string actualBaseAddress)
        {
            try
            {
                var timeBased = 0;
                var finalConverterDataSend = GetPrioTicketDetailsAsyncV2(code, actualBaseAddress, token, activityOption, timeBased, criteria) as List<ActivityOption>;
                //Mutiple Options in Cluster Products
                if (((List<ActivityOption>)finalConverterDataSend)?.FirstOrDefault()?.Cluster != null)
                {
                    return finalConverterDataSend;
                }
                else
                {
                    //Single Option in Normal Product
                    return finalConverterDataSend?.Take(1)?.ToList();
                }
            }
            catch (Exception ex)
            {
                _log.Error(new Isango.Entities.IsangoErrorEntity
                {
                    ClassName = "PrioHubAdapter",
                    MethodName = "CreateOption",
                    Token = token
                }, ex);
            }
            return null;
        }

        private void AddPassengerCountInInputContext(ProductOption productOptions, InputContext _inputContext)
        {
            var travelInfo = productOptions?.TravelInfo;

            _inputContext.TicketType = new List<string>();
            _inputContext.Count = new List<int>();
            var validPassengers = travelInfo.NoOfPassengers.Where(e => _validPassengerTypes.Contains(e.Key));
            foreach (var passengerType in validPassengers)
            {
                var count = passengerType.Value;
                if (count > 0 && passengerType.Key != PassengerType.Infant)
                {
                    _inputContext.TicketType.Add(GetPrioHubPassengerType(passengerType.Key));
                    _inputContext.Count.Add(count);
                }
            }
        }

        private static string GetPrioHubPassengerType(PassengerType passengerType)
        {
            switch (passengerType)
            {
                case PassengerType.Adult:
                    return Constant.Adult;

                case PassengerType.Child:
                    return Constant.Child;

                case PassengerType.Infant:
                    return Constant.Infant;

                case PassengerType.Senior:
                    return Constant.Senior;

                case PassengerType.Student:
                    return Constant.Student;

                case PassengerType.Family:
                    return Constant.Family;

                case PassengerType.Youth:
                    return Constant.Youth;

                default:
                    return Constant.Adult;
            }
        }
    }
}