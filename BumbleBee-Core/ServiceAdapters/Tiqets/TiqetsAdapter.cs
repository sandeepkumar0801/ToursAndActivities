using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.Tiqets;
using ServiceAdapters.Tiqets.Constants;
using ServiceAdapters.Tiqets.Tiqets.Commands.Contracts;
using ServiceAdapters.Tiqets.Tiqets.Converters.Contracts;
using ServiceAdapters.Tiqets.Tiqets.Entities;
using ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Util;
using BookingRequest = Isango.Entities.Tiqets.BookingRequest;
using Product = ServiceAdapters.Tiqets.Tiqets.Entities.Product;
using TiqetsCriteria = Isango.Entities.Tiqets.TiqetsCriteria;

namespace ServiceAdapters.Tiqets
{
    public class TiqetsAdapter : ITiqetsAdapter, IAdapter
    {
        #region "Private Members"

        //Command Handlers
        private readonly IGetAvailabilityByIdCommandHandler _getAvailabilityByIdCommandHandler;

        private readonly IGetAvailableDaysCommandHandler _getAvailableDaysCommandHandler;
        private readonly IGetAvailableTimeSlotsCommandHandler _getAvailableTimeSlotsCommandHandler;
        private readonly IGetVariantsCommandHandler _getVariantsCommandHandler;
        private readonly IGetProductDetailsCommandHandler _getProductDetailsCommandHandler;
        private readonly IGetBulkAvailabilityCommandHandler _getBulkAvailabilityCommandHandler;
        private readonly IGetBulkVariantsAvailabilityCommandHandler _getBulkVariantsAvailabilityCommandHandler;
        private readonly ICreateOrderCommandHandler _createOrderCommandHandler;
        private readonly IConfirmOrderCommandHandler _confirmOrderCommandHandler;
        private readonly IGetTicketCommandHandler _getTicketCommandHandler;
        private readonly IOrderInformationCmdHandler _getOrderInfoCommandHandler;
        private readonly ICancelOrderCommandHandler _cancelOrderCommandHandler;

        private readonly IGetProductFilterCommandHandler _getProductFilterCommandHandler;
        private readonly ITiqetsPackageCommandHandler _tiqetsPackageCommandHandler;

        //Converters
        private readonly IAvailabilityConverter _availabilityConverter;

        private readonly IBookingConverter _bookingConverter;

        #endregion "Private Members"

        #region "Constructor"

        public TiqetsAdapter(IGetAvailableDaysCommandHandler getAvailableDaysCommandHandler,
            IGetAvailableTimeSlotsCommandHandler getAvailableTimeSlotsCommandHandler, IGetAvailabilityByIdCommandHandler getAvailabilityByIdCommandHandler, IGetBulkVariantsAvailabilityCommandHandler getBulkVariantsAvailabilityByIdCommandHandler, IGetTicketCommandHandler getTicketCommandHandler, IGetVariantsCommandHandler getVariantsCommandHandler,
            IConfirmOrderCommandHandler confirmOrderCommandHandler,
            ICreateOrderCommandHandler createOrderCommandHandler,
            IGetProductDetailsCommandHandler getProductDetailsCommandHandler,
            IGetBulkAvailabilityCommandHandler getBulkAvailabilityCommandHandler,
            IAvailabilityConverter availabilityConverter,
            IBookingConverter bookingConverter,
            IOrderInformationCmdHandler orderInfoCommandHandler,
            ICancelOrderCommandHandler cancelOrderCommandHandler,
            IGetProductFilterCommandHandler getProductFilterCommandHandler,
            ITiqetsPackageCommandHandler tiqetsPackageCommandHandler)
        {
            _getAvailableDaysCommandHandler = getAvailableDaysCommandHandler;
            _getAvailableTimeSlotsCommandHandler = getAvailableTimeSlotsCommandHandler;
            _getAvailabilityByIdCommandHandler = getAvailabilityByIdCommandHandler;
            _getTicketCommandHandler = getTicketCommandHandler;
            _getVariantsCommandHandler = getVariantsCommandHandler;
            _confirmOrderCommandHandler = confirmOrderCommandHandler;
            _createOrderCommandHandler = createOrderCommandHandler;
            _getProductDetailsCommandHandler = getProductDetailsCommandHandler;
            _getBulkAvailabilityCommandHandler = getBulkAvailabilityCommandHandler;
            _availabilityConverter = availabilityConverter;
            _bookingConverter = bookingConverter;
            _getBulkVariantsAvailabilityCommandHandler = getBulkVariantsAvailabilityByIdCommandHandler;
            _getOrderInfoCommandHandler = orderInfoCommandHandler;
            _cancelOrderCommandHandler = cancelOrderCommandHandler;
            _getProductFilterCommandHandler = getProductFilterCommandHandler;
            _tiqetsPackageCommandHandler = tiqetsPackageCommandHandler;
        }

        #endregion "Constructor"

        /// <summary>
        /// Get Availabilities
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="language"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Product GetProductDetailsByProductId(int productId, string language, string token)
        {
            if (productId > 0 && language != null)
            {
                var criteria = new TiqetsCriteria
                {
                    ProductId = productId,
                    Language = language
                };

                //Get Product Details
                var productDetails = _getProductDetailsCommandHandler.Execute(criteria, MethodType.ProductDetails, token);

                if (productDetails != null && !productDetails.ToString().Contains(Constant.Error))
                {
                    var productDetailsSerializedData = SerializeDeSerializeHelper.DeSerialize<ProductResponse>(productDetails.ToString());
                    return productDetailsSerializedData.Product;
                }
            }
            return null;
        }

        /// <summary>
        /// Get Price and Availability
        /// </summary>
        /// <param name="availabilityCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<ProductOption> GetAvailabilities(TiqetsCriteria availabilityCriteria, string token)
        {
            if (availabilityCriteria?.ProductId > 0 && availabilityCriteria.Language != null)
            {
                //Get Bulk Availability of Product
                var bulkAvailability = GetBulkVariantsAvailabilityByProductId(availabilityCriteria.ProductId,
                availabilityCriteria.CheckinDate, availabilityCriteria.CheckoutDate, token, availabilityCriteria.AffiliateId);

                if (bulkAvailability != null && bulkAvailability.Success == true && bulkAvailability.Dates?.Count > 0)
                {
                    var productOptionList = new List<ProductOption>();
                    try
                    {
                        foreach (var date in bulkAvailability.Dates)
                        {
                            var dateVariantsDict = new Dictionary<DateTime, List<ProductVariant>>();
                            foreach (var timeslot in date.TimeSlots)
                            {
                                var time = timeslot.Time.ToString();
                                var variants = new List<ProductVariant>();
                                foreach (var variant in timeslot.Variants)
                                {
                                    //Get the variant details from the variant received at Base object(BulkVariantsAvailabilityResponse) level and assign its
                                    //cancellation related details to the following productVariant as it will be used in converter for setting options cancellation text.
                                    var variantDetailsInParentObj = bulkAvailability.Variants.Find(thisVar => variant.Id.Equals(thisVar.Id));
                                    var productVariant = new ProductVariant()
                                    {
                                        Description = bulkAvailability.Variants?.Where(x => x.Id == variant.Id)?.FirstOrDefault().Description?.ToString(),
                                        Id = variant.Id,
                                        Label = bulkAvailability.Variants?.Where(x => x.Id == variant.Id)?.FirstOrDefault().Label.ToString(),
                                        MaxTickets = variant.MaxTickets,
                                        ValidWithVariantIds = bulkAvailability.Variants?.Where(x => x.Id == variant.Id)?.FirstOrDefault().ValidWithVariantIds,
                                        PriceComponentsEur = variant.PriceMediation,
                                        Cancellation = new Isango.Entities.Tiqets.Cancellation
                                        {
                                            Window = variantDetailsInParentObj.Cancellation.Window,
                                            Policy = variantDetailsInParentObj?.Cancellation.Policy
                                        },
                                        RequiresVisitorsDetails = bulkAvailability.Variants?.Where(x => x.Id == variant.Id)?.FirstOrDefault()?.RequiresVisitorsDetails
                                    };
                                    //productVariant.RequiresVisitorsDetailsWithVariant = new Dictionary<int, List<string>>();
                                    //if (!productVariant.RequiresVisitorsDetailsWithVariant.ContainsKey(productVariant.Id))
                                    //{
                                    //    productVariant.RequiresVisitorsDetailsWithVariant.Add(productVariant.Id, productVariant.RequiresVisitorsDetails);
                                    //}
                                    variants.Add(productVariant);
                                }
                                if (variants != null)
                                    dateVariantsDict[date.Date.ToDateTime()] = variants;

                                availabilityCriteria.TimeSlot = time;
                                var productOptions = _availabilityConverter.Convert(dateVariantsDict, availabilityCriteria);
                                if (productOptions != null)
                                    productOptionList.Add((ProductOption)productOptions);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //ignored //Logging required here
                    }
                    productOptionList?.RemoveAll(x => x.BasePrice == null && x.CostPrice == null || x.BasePrice?.Amount == 0 && x.CostPrice.Amount == 0);
                    return productOptionList;
                }
            }
            return null;
        }

        /// <summary>
        /// Get Variant Details to dump in the database
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<ProductVariant> GetVariantsForDumpingApplication(TiqetsCriteria criteria, string token)
        {
            if (criteria.ProductId > 0 && criteria.Language != null)
            {
                //Get Bulk Availability of Product
                var bulkAvailability = GetBulkVariantsAvailabilityByProductId(criteria.ProductId, criteria.CheckinDate, criteria.CheckoutDate, token, criteria?.AffiliateId);

                if (bulkAvailability != null && bulkAvailability.Success == true && bulkAvailability.Variants?.Count > 0 && bulkAvailability.Dates != null && bulkAvailability.Dates?.Count > 0)
                {
                    try
                    {
                        var variants = new List<ProductVariant>();

                        var query = from date in bulkAvailability.Dates
                                    where date.TimeSlots != null
                                    && date.TimeSlots?.Count > 0
                                    from timeslot in date.TimeSlots
                                    where timeslot.Variants != null
                                    && timeslot.Variants.Count > 0
                                    from Variant in timeslot.Variants
                                    select Variant;
                        var distinctVariants = query?.Distinct()?.ToList();

                        foreach (var variant in bulkAvailability.Variants)
                        {
                            try
                            {
                                var price = distinctVariants?.Where(x => x.Id == variant.Id)?.FirstOrDefault()?.PriceMediation;
                                if (price == null)
                                {
                                    price = new PriceComponent();
                                    price.SaleTicketValueIncVat = 0;
                                    price.DistributorCommissionExclVat = 0;
                                    price.BookingFeeIncVat = 0;
                                    price.TotalRetailPriceIncVat = 0;
                                }

                                var productVariant = new ProductVariant()
                                {
                                    Description = variant?.Description?.ToString(),
                                    Id = variant?.Id ?? 0,
                                    Label = variant?.Label?.ToString(),
                                    MaxTickets = distinctVariants?.Where(x => x.Id == variant.Id)?.FirstOrDefault()?.MaxTickets ?? 0,
                                    ValidWithVariantIds = variant?.ValidWithVariantIds,
                                    PriceComponentsEur = price,
                                    Cancellation = new Isango.Entities.Tiqets.Cancellation
                                    {
                                        Window = variant?.Cancellation?.Window ?? 0,
                                        Policy = variant?.Cancellation?.Policy
                                    },
                                    RequiresVisitorsDetails = variant.RequiresVisitorsDetails
                                };
                                variants.Add(productVariant);
                            }
                            catch (Exception a)
                            {
                                //ignore
                            }
                        }

                        return variants;
                    }
                    catch (Exception ex)
                    {
                        //Old Method
                        //Get First DayAvailability
                        var availability = bulkAvailability.Dates.Where(x => x.Availability != 0).FirstOrDefault();

                        //Get First Available TimeSlot
                        var timeSlot = availability.TimeSlots?.FirstOrDefault();

                        var variants = new List<ProductVariant>();

                        if (timeSlot != null && timeSlot.Variants?.Count > 0)
                        {
                            foreach (var variant in timeSlot.Variants)
                            {
                                //Find the cancellation details for this variant
                                var variantDetailsForThisVariantInBaseResponseObject = bulkAvailability.Variants.Find(thisVar => thisVar.Id.Equals(variant.Id));
                                var cancellationDetailsForThisVariant = variantDetailsForThisVariantInBaseResponseObject.Cancellation;
                                var requireVisitor = variantDetailsForThisVariantInBaseResponseObject?.RequiresVisitorsDetails;
                                var productVariant = new ProductVariant()
                                {
                                    Description = bulkAvailability.Variants?.Where(x => x.Id == variant.Id)?.FirstOrDefault()?.Description?.ToString(),
                                    Id = variant.Id,
                                    Label = bulkAvailability.Variants?.Where(x => x.Id == variant.Id)?.FirstOrDefault()?.Label?.ToString(),
                                    MaxTickets = variant.MaxTickets,
                                    ValidWithVariantIds = bulkAvailability.Variants?.Where(x => x.Id == variant.Id)?.FirstOrDefault()?.ValidWithVariantIds,
                                    PriceComponentsEur = variant.PriceMediation,
                                    Cancellation = new Isango.Entities.Tiqets.Cancellation
                                    {
                                        Window = cancellationDetailsForThisVariant.Window,
                                        Policy = cancellationDetailsForThisVariant?.Policy
                                    },
                                    RequiresVisitorsDetails = requireVisitor
                                };
                                variants.Add(productVariant);
                            }
                        }

                        return variants;
                    }
                    //Get variants
                    //var variants = GetVariants(availabilityCriteria, token);

                    //return variants;
                }
            }

#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
        }

        /// <summary>
        /// Get Bulk Availability By Product Id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public BulkAvailabilityResponse GetBulkAvailabilityByProductId(int productId, DateTime startDate,
            DateTime endDate, string token)
        {
            if (productId > 0)
            {
                //Create criteria
                var availabilityCriteria = new TiqetsCriteria
                {
                    ProductId = productId,
                    CheckinDate = startDate,
                    CheckoutDate = endDate,
                };

                //Get Bulk Availability
                var availabilities = _getBulkAvailabilityCommandHandler.Execute(availabilityCriteria, MethodType.BulkAvailability, token);

                if (availabilities != null && !availabilities.ToString().Contains(Constant.Error))
                {
                    var availabilitySerializedData = SerializeDeSerializeHelper.DeSerialize<BulkAvailabilityResponse>(availabilities.ToString());

                    if (availabilitySerializedData != null && availabilitySerializedData.Success)
                    {
                        return availabilitySerializedData;
                    }
                }
            }

            return null;
        }

        public BulkVariantsAvailabilityResponse GetBulkVariantsAvailabilityByProductId(int productId, DateTime startDate,
            DateTime endDate, string token, string affiliateID = "")
        {
            if (productId > 0)
            {
                //Create criteria
                var availabilityCriteria = new TiqetsCriteria
                {
                    ProductId = productId,
                    CheckinDate = startDate,
                    CheckoutDate = endDate,
                    AffiliateId = affiliateID
                };

                //Get Bulk Availability
                var availabilities = _getBulkVariantsAvailabilityCommandHandler.Execute(availabilityCriteria, MethodType.BulkAvailability, token);

                /*
                var isMock = false;
                var apiRequest = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\9 Tiquets 1 ApiREQ Avaialbility.json");
                var apiResponse = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\9 Tiquets 1 ApiRES Avaialbility.json");
                var bookingConfirmRS = (apiResponse);
                var availabilities = bookingConfirmRS;
                isMock = true;
                //*/

                if (availabilities != null && !availabilities.ToString().Contains(Constant.Error))
                {
                    var availabilitySerializedData = SerializeDeSerializeHelper.DeSerializeWithNullValueHandling<BulkVariantsAvailabilityResponse>(availabilities.ToString());

                    if (availabilitySerializedData != null && availabilitySerializedData.Success == true)
                    {
                        return availabilitySerializedData;
                    }
                }
            }

            return null;
        }

        public ProductFilter GetProductFilter(string token, int pageNumber)
        {
            //Create criteria
            var availabilityCriteria = new TiqetsCriteria
            {
                PageNumber = pageNumber
            };
            //Get Product Filter
            var products = _getProductFilterCommandHandler.Execute(availabilityCriteria, MethodType.ProductFilter, token);
            //if (products != null && !products.ToString().Contains(Constant.Error))
            //{
            var productsFilterSerializedData = SerializeDeSerializeHelper.DeSerializeWithNullValueHandling<ProductFilter>(products.ToString());

            if (productsFilterSerializedData != null && productsFilterSerializedData.Success == true)
            {
                return productsFilterSerializedData;
            }
            //}
            return null;
        }

        /// <summary>
        /// Get Price and Availability by Product Id
        /// </summary>
        /// <param name="availabilityCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<ProductOption> GetPriceAndAvailabilityByProductId(TiqetsCriteria availabilityCriteria, string token)
        {
            if (availabilityCriteria != null && availabilityCriteria.ProductId > 0 && availabilityCriteria.Language != null)
            {
                //Get Available Days
                var availableDays = GetAvailableDays(availabilityCriteria, token);

                //Check that the product available on the check in date or not
                var availableDate = availableDays?.FirstOrDefault(x => x.Date == availabilityCriteria.CheckinDate.Date);

                if (availableDate.HasValue)
                {
                    //Get Checkout Information
                    var checkoutInformation = GetCheckoutInformation(availabilityCriteria, token);
                    if (checkoutInformation != null)
                    {
                        var productOptionList = new List<ProductOption>();

                        if (checkoutInformation.HasTimeSlots)
                        {
                            //Get TimeSlots
                            var availableTimeSlots = GetAvailableTimeSlots(availabilityCriteria, token);
                            if (availableTimeSlots?.Count > 0)
                            {
                                //Get Variants through day and timeSlots
                                foreach (var timeSlot in availableTimeSlots)
                                {
                                    availabilityCriteria.TimeSlot = timeSlot.Slot;
                                    var variants = GetVariants(availabilityCriteria, token);
                                    if (variants == null) continue;
                                    var productOptions = _availabilityConverter.Convert(variants, availabilityCriteria);
                                    if (productOptions != null)
                                        productOptionList.Add((ProductOption)productOptions);
                                }
                            }
                        }
                        else
                        {
                            var variants = GetVariants(availabilityCriteria, token);
                            if (variants != null)
                            {
                                var productOptions = _availabilityConverter.Convert(variants, availabilityCriteria);
                                if (productOptions != null)
                                    productOptionList.Add((ProductOption)productOptions);
                            }
                        }

                        return productOptionList;
                    }
                }
            }

#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
        }

        /// <summary>
        /// Create Order
        /// </summary>
        /// <param name="bookingRequest"></param>
        /// /// <param name="token"></param>
        /// <param name="apiRequest"></param>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public CreateOrderResponse CreateOrder(BookingRequest bookingRequest, string token, out string apiRequest, out string apiResponse, out HttpStatusCode httpStatusCode)
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;
            Object createOrderResponse = null;

            if (bookingRequest.PackageId != null && bookingRequest.PackageId.Count() > 0)
            {

                createOrderResponse = _tiqetsPackageCommandHandler.Execute(bookingRequest, token, MethodType.CreateOrder, out apiRequest, out apiResponse, out httpStatusCode);
            }
            else
            {
                createOrderResponse = _createOrderCommandHandler.Execute(bookingRequest, token, MethodType.CreateOrder, out apiRequest, out apiResponse, out httpStatusCode);
            }
            /*
            var isMock = false;
            apiRequest = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Cancellation Policy in Booking Response\09 Tiqets\9 Tiquets 2 ApiREQ CreateOrder.json");
            apiResponse = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Cancellation Policy in Booking Response\09 Tiqets\9 Tiquets 2 ApiRES CreateOrder.json");
            var bookingConfirmRS = (apiResponse);
            var createOrderResponse = bookingConfirmRS;
            isMock = true;
            //*/

            if (createOrderResponse != null && !createOrderResponse.ToString().Contains(Constant.Error))
            {
                var createOrderSerializedResponse =
                    SerializeDeSerializeHelper.DeSerialize<CreateOrderResponse>(createOrderResponse.ToString());
                if (createOrderSerializedResponse.Success)
                {
                    return createOrderSerializedResponse;
                }
            }

            return null;
        }

        /// <summary>
        /// Confirm Order
        /// </summary>
        /// <param name="bookingRequest"></param>
        /// /// <param name="token"></param>
        /// <param name="apiRequest"></param>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public ConfirmOrderResponse ConfirmOrder(BookingRequest bookingRequest, string token, out string apiRequest, out string apiResponse, out HttpStatusCode httpStatusCode)
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;

            if (bookingRequest.RequestObject != null)
            {
                var confirmOrderResponse = _confirmOrderCommandHandler.Execute(bookingRequest, token, MethodType.ConfirmOrder, out apiRequest, out apiResponse, out httpStatusCode);

                //Mocking api booking
                /*
                var isMock = false;
                apiRequest = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Cancellation Policy in Booking Response\09 Tiqets\9 Tiquets 3 ApiREQ ConfirmOrder.json");
                apiResponse = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Cancellation Policy in Booking Response\09 Tiqets\9 Tiquets 3 ApiRES ConfirmOrder.json");
                var bookingConfirmRS = (apiResponse);
                var confirmOrderResponse = bookingConfirmRS;
                isMock = true;
                //*/

                if (confirmOrderResponse != null && !confirmOrderResponse.ToString().Contains(Constant.Error))
                {
                    var confirmOrderSerializedResponse =
                        SerializeDeSerializeHelper.DeSerialize<ConfirmOrderResponse>(confirmOrderResponse.ToString());
                    return confirmOrderSerializedResponse;
                }
            }
            httpStatusCode = HttpStatusCode.BadGateway;
            return null;
        }

        public OrderInformationResponse GetOrderInformation(TiqetsSelectedProduct tiqetsProduct, string bookingReferenceNo, string token, string languageCode, out string apiRequest, out string apiResponse,
            out HttpStatusCode httpStatusCode, string affiliateId = "")
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;

            if (tiqetsProduct != null)
            {
                var bookingRequest = new BookingRequest()
                {
                    RequestObject = tiqetsProduct,
                    LanguageCode = languageCode,
                    IsangoBookingReference = bookingReferenceNo,
                    AffiliateId = affiliateId
                };

                var orderInfoResponse =
                    _getOrderInfoCommandHandler.Execute(bookingRequest, token, MethodType.GetOrderInfo, out apiRequest, out apiResponse, out httpStatusCode);
                if (orderInfoResponse != null)
                {
                    var orderInfoSerializedResponse =
                        SerializeDeSerializeHelper.DeSerializeWithNullValueHandling<OrderInformationResponse>(orderInfoResponse.ToString());
                    return orderInfoSerializedResponse;
                }
            }
            httpStatusCode = HttpStatusCode.BadGateway;
            return null;
        }

        public bool CancelOrder(TiqetsSelectedProduct tiqetsProduct, string bookingReferenceNo,
            string token, string languageCode, out string apiRequest, out string apiResponse,
            out HttpStatusCode httpStatusCode, string affiliateId = "")
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;

            if (tiqetsProduct != null)
            {
                var bookingRequest = new BookingRequest()
                {
                    RequestObject = tiqetsProduct,
                    LanguageCode = languageCode,
                    IsangoBookingReference = bookingReferenceNo,
                    AffiliateId = affiliateId
                };

                var cancellationResponse = _cancelOrderCommandHandler.Execute(bookingRequest, token, MethodType.CancelOrder, out apiRequest, out apiResponse, out httpStatusCode);
                if (cancellationResponse != null)
                {
                    var cancelOrderSerializedResponse =
                        SerializeDeSerializeHelper.DeSerializeWithNullValueHandling<CancelOrderResponse>(cancellationResponse.ToString());
                    return cancelOrderSerializedResponse.Success;
                }
                return false;
            }
            httpStatusCode = HttpStatusCode.BadGateway;
            return false;
        }

        /// <summary>
        /// Get Ticket of confirmed booking
        /// </summary>
        /// <param name="bookingRequest"></param>
        /// <param name="token"></param>
        /// <param name="apiRequest"></param>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public Booking GetTicket(BookingRequest bookingRequest, string token,
            out string apiRequest, out string apiResponse, out HttpStatusCode httpStatusCode)
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;

            if (bookingRequest.RequestObject != null)
            {
                var getTicketsResponse = _getTicketCommandHandler.Execute(bookingRequest, token, MethodType.GetTicket, out apiRequest, out apiResponse, out httpStatusCode);

                /*
                var isMock = false;
                apiRequest = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\9 Tiquets 4 ApiREQ GetTciket.json");
                apiResponse = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\9 Tiquets 4 ApiRES GetTciket.json");
                var bookingConfirmRS = (apiResponse);
                var getTicketsResponse = bookingConfirmRS;
                isMock = true;
                //*/

                if (getTicketsResponse != null && !getTicketsResponse.ToString().Contains(Constant.Error))
                {
                    apiResponse = getTicketsResponse.ToString();
                    //apiResponse = apiResponse.Replace("new", "New");
                    var getTicketsSerializedResponse = SerializeDeSerializeHelper.DeSerialize<GetTicketResponse>(apiResponse);
                    if (getTicketsSerializedResponse.Success)
                    {
                        var bookingResponse = _bookingConverter.Convert(getTicketsSerializedResponse, string.Empty);
                        return (Booking)bookingResponse;
                    }
                }
            }
            httpStatusCode = HttpStatusCode.BadGateway;
            return null;
        }

        #region "Private Methods"

        /// <summary>
        /// Get Checkout Information of Product
        /// </summary>
        /// <param name="availabilityCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private CheckoutInformation GetCheckoutInformation(TiqetsCriteria availabilityCriteria, string token)
        {
            var availability = _getAvailabilityByIdCommandHandler.Execute(availabilityCriteria, MethodType.CheckoutInformation, token);
            if (availability != null && !availability.ToString().Contains(Constant.Error))
            {
                var availabilitySerializedData = SerializeDeSerializeHelper.DeSerialize<CheckoutInformation>(availability.ToString());
                if (availabilitySerializedData != null && availabilitySerializedData.Success)
                {
                    return availabilitySerializedData;
                }
            }

            return null;
        }

        /// <summary>
        /// Get Available Days of Product
        /// </summary>
        /// <param name="availabilityCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private DateTime[] GetAvailableDays(TiqetsCriteria availabilityCriteria, string token)
        {
            var availabilityDays = _getAvailableDaysCommandHandler.Execute(availabilityCriteria, MethodType.AvailableDays, token);
            if (availabilityDays != null && !availabilityDays.ToString().Contains(Constant.Error))
            {
                var availabilityDaysSerializedData =
                    SerializeDeSerializeHelper.DeSerialize<AvailableDays>(availabilityDays.ToString());
                if (availabilityDaysSerializedData != null && availabilityDaysSerializedData.Success && availabilityDaysSerializedData.Days?.Length > 0)
                {
                    return availabilityDaysSerializedData.Days;
                }
            }

#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
        }

        /// <summary>
        /// Get Available TimeSlots of a Day
        /// </summary>
        /// <param name="availabilityCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<TimeSlot> GetAvailableTimeSlots(TiqetsCriteria availabilityCriteria, string token)
        {
            var timeSlots = _getAvailableTimeSlotsCommandHandler.Execute(availabilityCriteria, MethodType.AvailableTimeSlots, token);
            if (timeSlots != null && !timeSlots.ToString().Contains(Constant.Error))
            {
                var timeSlotsSerializedData = SerializeDeSerializeHelper.DeSerialize<AvailableTimeSlots>(
                    timeSlots.ToString());

                if (timeSlotsSerializedData != null && timeSlotsSerializedData.Success &&
                    timeSlotsSerializedData.TimeSlots?.Count > 0)
                {
                    var availableTimeSlots = timeSlotsSerializedData.TimeSlots.Where(x => x.IsAvailable).ToList();
                    return availableTimeSlots;
                }
            }

#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
        }

        /// <summary>
        /// Get Variants through criteria
        /// </summary>
        /// <param name="availabilityCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<ProductVariant> GetVariants(TiqetsCriteria availabilityCriteria, string token)
        {
            var variants = _getVariantsCommandHandler.Execute(availabilityCriteria, MethodType.Variant, token);
            if (variants != null && !variants.ToString().Contains(Constant.Error))
            {
                var variantsSerializedData = SerializeDeSerializeHelper.DeSerialize<ProductVariantResponse>(
                    variants.ToString());
                if (variantsSerializedData != null && variantsSerializedData.Success &&
                    variantsSerializedData.Variants.Count > 0)
                {
                    return variantsSerializedData.Variants;
                }
            }

#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
        }

        #endregion "Private Methods"
    }
}