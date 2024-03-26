using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Aot;
using Isango.Entities.BigBus;
using Isango.Entities.Bokun;
using Isango.Entities.Booking;
using Isango.Entities.CitySightseeing;
using Isango.Entities.Enums;
using Isango.Entities.FareHarbor;
using Isango.Entities.GoCity;
using Isango.Entities.GoldenTours;
using Isango.Entities.GrayLineIceLand;
using Isango.Entities.HotelBeds;
using Isango.Entities.MoulinRouge;
using Isango.Entities.NewCitySightSeeing;
using Isango.Entities.Prio;
using Isango.Entities.PrioHub;
using Isango.Entities.Rayna;
using Isango.Entities.Redeam;
using Isango.Entities.Rezdy;
using Isango.Entities.Tiqets;
using Isango.Entities.TourCMS;
using Isango.Entities.Ventrata;
using Isango.Persistence.Contract;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Aot;
using ServiceAdapters.BigBus;
using ServiceAdapters.BigBus.BigBus.Entities;
using ServiceAdapters.Bokun;
using ServiceAdapters.FareHarbor;
using ServiceAdapters.GlobalTix;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using ServiceAdapters.GoCity;
using ServiceAdapters.GoldenTours;
using ServiceAdapters.GrayLineIceLand;
using ServiceAdapters.HB;
using ServiceAdapters.HotelBeds;
using ServiceAdapters.MoulinRouge;
using ServiceAdapters.NewCitySightSeeing;
using ServiceAdapters.PrioHub;
using ServiceAdapters.PrioTicket;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using ServiceAdapters.Rayna;
using ServiceAdapters.Redeam;
using ServiceAdapters.Rezdy;
using ServiceAdapters.SightSeeing;
using ServiceAdapters.Tiqets;
using ServiceAdapters.TourCMS;
using ServiceAdapters.TourCMS.TourCMS.Entities;
using ServiceAdapters.Ventrata;
using ServiceAdapters.Ventrata.Ventrata.Entities.Response;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using TableStorageOperations.Contracts;
using TableStorageOperations.Models.Booking;
using Util;
using BookedProduct = Isango.Entities.BookedProduct;
using ConstantForVentrata = ServiceAdapters.Ventrata.Constants.Constants;
using VentrataEntities = ServiceAdapters.Ventrata.Ventrata.Entities;
using ReservationData = ServiceAdapters.PrioHub.PrioHub.Entities.ReservationResponse;
using ServiceAdapters.TourCMS.TourCMS.Entities.NewBooking;
using System.Threading;
using ServiceAdapters.Redeam.Redeam.Entities.CreateHold;
using ServiceAdapters.MoulinRouge.MoulinRouge.Entities.AllocSeatsAutomatic;
using ServiceAdapters.PrioHub.PrioHub.Entities;
using ServiceAdapters.PrioHub.PrioHub.Entities.GetVoucherRes;
using System.Net;

namespace Isango.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SupplierBookingService : ISupplierBookingService
    {
        private readonly ISupplierBookingPersistence _supplierBookingPersistence;
        private readonly ITicketAdapter _ticketAdapter;
        private readonly IGrayLineIceLandAdapter _grayLineIceLandAdapter;
        private readonly IPrioTicketAdapter _prioTicketAdapter;
        private readonly IVentrataAdapter _ventrataAdapter;
        private readonly IMoulinRougeAdapter _moulinRougeAdapter;
        private readonly ISightSeeingAdapter _sightSeeingAdapter;
        private readonly ILogger _log;
        private readonly IFareHarborAdapter _fareHarborAdapter;
        private readonly IBokunAdapter _bokunAdapter;
        private readonly IAotAdapter _aotAdapter;
        private readonly ITiqetsAdapter _tiqetsAdapter;
        private readonly IGoldenToursAdapter _goldenToursAdapter;
        private readonly IMemCache _memCache;
        private readonly ITiqetsPaxMappingCacheManager _tiqetsPaxMappingCacheManager;
        private readonly IMasterCacheManager _masterCacheManager;
        private readonly IBigBusAdapter _bigBusAdapter;
        private readonly IHBAdapter _hbAdapter;
        private readonly IRedeamAdapter _redeamAdapter;
        private readonly IMasterPersistence _masterPersistence;
        private readonly ITableStorageOperation _tableStorageOperation;
        private readonly IMasterService _masterService;
        private readonly IRezdyAdapter _rezdyAdapter;

        private readonly IGlobalTixAdapter _globalTixAdapter;
        private readonly ITourCMSAdapter _tourCMSAdapter;

        private readonly INewCitySightSeeingAdapter _newCitySightSeeingAdapter;

        private readonly IGoCityAdapter _goCityAdapter;

        private readonly IPrioHubAdapter _prioHubAdapter;

        private readonly IRaynaAdapter _raynaAdapter;

        public SupplierBookingService(ISupplierBookingPersistence supplierBookingPersistence
            //, IEncoreAdapter encoreAdapter
            , ITicketAdapter ticketAdapter
            , IGrayLineIceLandAdapter grayLineIceLandAdapter
            , IPrioTicketAdapter prioTicketAdapter
            , IMoulinRougeAdapter moulinRougeAdapter
            , ISightSeeingAdapter sightSeeingAdapter
            , ILogger log
            , IFareHarborAdapter fareHarborAdapter
            , IBokunAdapter bokunAdapter
            , IAotAdapter aotAdapter
            , ITiqetsAdapter tiqetsAdapter
            , IMemCache memCache
            , ITiqetsPaxMappingCacheManager tiqetsPaxMappingCacheManager
            , IMasterCacheManager masterCacheManager
            , IGoldenToursAdapter goldenToursAdapter
            , IBigBusAdapter bigBusAdapter
            , IHBAdapter hbAdapter
            , IRedeamAdapter redeamAdapter
            , IMasterPersistence masterPersistence
            , ITableStorageOperation tableStorageOperation
            , IRezdyAdapter rezdyAdapter,
              IMasterService masterService
            , IGlobalTixAdapter globalTixAdapter
            , IVentrataAdapter ventrataAdapter
            , ITourCMSAdapter tourCMSAdapter
            , INewCitySightSeeingAdapter newCitySightSeeingAdapter
            , IGoCityAdapter goCityAdapter
            , IPrioHubAdapter prioHubAdapter
            , IRaynaAdapter raynaAdapter
            )
        {
            _supplierBookingPersistence = supplierBookingPersistence;
            _ticketAdapter = ticketAdapter;
            _grayLineIceLandAdapter = grayLineIceLandAdapter;
            _prioTicketAdapter = prioTicketAdapter;
            _moulinRougeAdapter = moulinRougeAdapter;
            _sightSeeingAdapter = sightSeeingAdapter;
            _log = log;
            _fareHarborAdapter = fareHarborAdapter;
            _bokunAdapter = bokunAdapter;
            _aotAdapter = aotAdapter;
            _tiqetsAdapter = tiqetsAdapter;
            _goldenToursAdapter = goldenToursAdapter;
            _memCache = memCache;
            _tiqetsPaxMappingCacheManager = tiqetsPaxMappingCacheManager;
            _masterCacheManager = masterCacheManager;
            _bigBusAdapter = bigBusAdapter;
            _hbAdapter = hbAdapter;
            _redeamAdapter = redeamAdapter;
            _masterPersistence = masterPersistence;
            _tableStorageOperation = tableStorageOperation;
            _rezdyAdapter = rezdyAdapter;
            _masterService = masterService;
            _globalTixAdapter = globalTixAdapter;
            _ventrataAdapter = ventrataAdapter;
            _tourCMSAdapter = tourCMSAdapter;
            _newCitySightSeeingAdapter = newCitySightSeeingAdapter;
            _goCityAdapter = goCityAdapter;
            _prioHubAdapter = prioHubAdapter;
            _raynaAdapter = raynaAdapter;
        }

        public List<BookedProduct> CreateGraylineIcelandBooking(ActivityBookingCriteria criteria)
        {
            var bookingId = criteria.Booking.BookingId;
            var request = string.Empty;
            var response = string.Empty;
            var grayLineSelectedProducts = new List<SelectedProduct>();
            var bookedProducts = new List<BookedProduct>();
            Booking gliBooking = null;

            try
            {
                var selectedProducts = criteria.Booking.SelectedProducts.Where(x => x.APIType.Equals(APIType.Graylineiceland)).ToList();

                grayLineSelectedProducts.AddRange(selectedProducts);

                if (grayLineSelectedProducts.Count > 0)
                {
                    gliBooking = _grayLineIceLandAdapter.CreateBooking(grayLineSelectedProducts.Cast<GrayLineIceLandSelectedProduct>().ToList(), criteria.Booking.ReferenceNumber,
                        criteria.Token, out request, out response);

                    if (gliBooking == null)
                    {
                        //Assign failed option status to GLI bookedProducts
                        foreach (var product in grayLineSelectedProducts)
                        {
                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == product.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;
                            bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();

                            bookedProduct.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, grayLineSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    bookedProduct?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.Graylineiceland), grayLineSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    grayLineSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, grayLineSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            bookedProducts.Add(bookedProduct);
                        }

                        //Api booking failed
                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);

                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, grayLineSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.Graylineiceland), grayLineSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                grayLineSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, grayLineSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }

                        return bookedProducts;
                    }

                    foreach (var product in grayLineSelectedProducts)
                    {
                        var selectedProduct = product as GrayLineIceLandSelectedProduct;
                        if (selectedProduct == null) continue;
                        var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                            x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);
                        if (bookedProduct == null) continue;

                        var bookedGliProduct = (GrayLineIceLandSelectedProduct)gliBooking.SelectedProducts.FirstOrDefault(x => ((GrayLineIceLandSelectedProduct)x).Code.Equals(selectedProduct.Code));
                        if (bookedGliProduct != null)
                        {
                            selectedProduct.ReservationId = bookedGliProduct.ReservationId;
                            bookedGliProduct.ProductOptions = UpdateOptionStatus(selectedProduct.ProductOptions, gliBooking.ReferenceNumber);
                            bookedProduct = MapProductForGLI(bookedProduct, bookedGliProduct, gliBooking.ReferenceNumber);
                        }
                        else
                        {
                            bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                        }

                        bookedProducts.Add(bookedProduct);
                    }
                }
                var logCriteria = new LogPurchaseXmlCriteria
                {
                    RequestXml = request,
                    ResponseXml = response,
                    Status = grayLineSelectedProducts.Any() ? Constant.StatusSuccess : Constant.StatusFailed,
                    BookingId = bookingId,
                    APIType = APIType.Graylineiceland,
                    BookingReferenceNumber = criteria.Booking.ReferenceNumber
                };
                _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                return bookedProducts;
            }
            catch (Exception ex)
            {
                if (gliBooking != null)
                {
                    bookedProducts.Clear();
                    var referenceNumber = gliBooking.ReferenceNumber == "0" ? string.Empty : gliBooking.ReferenceNumber;
                    foreach (var product in grayLineSelectedProducts)
                    {
                        var option = product.ProductOptions.FirstOrDefault(x => x.IsSelected);
                        var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                            x.AvailabilityReferenceId == product.AvailabilityReferenceId);
                        if (bookedProduct == null) continue;
                        bookedProduct.OptionStatus = GetBookingStatusNumber(referenceNumber, option?.AvailabilityStatus);
                        bookedProduct.APIExtraDetail.SupplieReferenceNumber = referenceNumber;
                        bookedProducts.Add(bookedProduct);
                    }
                }
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateGraylineIcelandBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError, System.Net.HttpStatusCode.BadGateway, $"Exception\n {ex.Message}\n{response}");

                try
                {
                    //bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, grayLineSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        gliBooking?.ReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.Graylineiceland), grayLineSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        grayLineSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, grayLineSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                return bookedProducts;
            }
        }

        public List<BookedProduct> CreateHbActivityBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var hBSelectedProducts = criteria.Booking.SelectedProducts.Where(product =>
                product.APIType.Equals(APIType.Hotelbeds)).ToList();
            var hotelBedsSelectedProducts = new List<HotelBedsSelectedProduct>();
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (hBSelectedProducts?.Count > 0)
                {
                    #region set suppler language to download voucher

                    //var voucherLanguage = GetMappedLanguages()
                    //               ?.Find(x => x.IsangoLanguageCode == criteria?.Booking?.Language?.Code)
                    //               ?.SupplierLanguageCode ?? criteria.Booking.Language.Code;

                    //criteria.Booking.SelectedProducts.Where(x => x.APIType.Equals(APIType.Hotelbeds)).ToList().ForEach(hbsp =>
                    //  {
                    //      var sp = (hbsp as HotelBedsSelectedProduct);
                    //      sp.Language = string.IsNullOrWhiteSpace(sp?.Language) ? voucherLanguage : sp?.Language;
                    //  });

                    #endregion set suppler language to download voucher

                    hotelBedsSelectedProducts = _hbAdapter.BookingConfirm(criteria.Booking, criteria.Token, out request, out response);

                    var hbSelectProductsCasted = hBSelectedProducts?.OfType<HotelBedsSelectedProduct>()?.ToList();
                    if (hotelBedsSelectedProducts == null
                        || Convert.ToInt32(hotelBedsSelectedProducts?.Count) == 0
                        || hbSelectProductsCasted?
                            .Any(x => string.IsNullOrWhiteSpace(x?.FileNumber))
                            == true
                    )
                    {
                        //Api booking failed
                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);

                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, hBSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                hotelBedsSelectedProducts?.FirstOrDefault(x => !string.IsNullOrEmpty(x.FileNumber))?.FileNumber ?? string.Empty, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.Hotelbeds), hBSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                hBSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, hBSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }

                        return SetFailedStatusOfHBBookedProduct(criteria, hBSelectedProducts);
                    }

                    foreach (var hbSelectedProduct in hbSelectProductsCasted)
                    {
                        var bookedhotelBedsProduct = hotelBedsSelectedProducts?.FirstOrDefault(x => x?.AvailabilityReferenceId?.ToLower() == hbSelectedProduct?.AvailabilityReferenceId?.ToLower());
                        if (bookedhotelBedsProduct != null)
                        {
                            var bookedOption = bookedhotelBedsProduct.ProductOptions
                                .FirstOrDefault(x => x.IsSelected);

                            var supplierCancellationPrices = bookedOption.CancellationPrices;

                            foreach (var bcp in bookedOption.CancellationPrices)
                            {
                                if (string.IsNullOrWhiteSpace(bcp.CancellationDescription))
                                {
                                    bcp.CancellationDescription = bookedOption.CancellationText;
                                }
                            }
                            if (supplierCancellationPrices != null)
                            {
                                hbSelectedProduct.SupplierCancellationPolicy = SerializeDeSerializeHelper.Serialize(supplierCancellationPrices);
                                hbSelectedProduct.CancellationPolicy = bookedOption.CancellationText;
                            }
                        }

                        hbSelectedProduct.ProductOptions = UpdateOptionStatus(hbSelectedProduct.ProductOptions,
                            hbSelectedProduct.FileNumber);

                        var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                            x.AvailabilityReferenceId == hbSelectedProduct.AvailabilityReferenceId);
                        if (bookedProduct == null) continue;
                        bookedProduct = MapProductForHB(bookedProduct, hbSelectedProduct);
                        bookedProducts.Add(bookedProduct);
                    }

                    var apiBookingRefNumbers = hotelBedsSelectedProducts?
                                                .Where(x => !string.IsNullOrWhiteSpace(x.FileNumber))?
                                                .Select(y => y.FileNumber)?.Distinct()?
                                                .ToArray();

                    var apiBookingRefNumber = "N/A";
                    if (apiBookingRefNumbers?.Length > 0)
                    {
                        apiBookingRefNumber = string.Join(",", apiBookingRefNumbers);
                    }

                    var isBookingSucess = hotelBedsSelectedProducts?
                                .Any(x => x.ProductOptions?
                                        .Any(y => y.BookingStatus == OptionBookingStatus.Confirmed) == true)
                                == true
                                ? Constant.StatusSuccess
                                : Constant.StatusFailed;

                    var logCriteria = new LogPurchaseXmlCriteria
                    {
                        RequestXml = request,
                        ResponseXml = response,

                        Status = isBookingSucess,

                        BookingId = criteria.Booking.BookingId,
                        APIType = APIType.Hotelbeds,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber,
                        ApiRefNumber = apiBookingRefNumber
                    };
                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                }
            }
            catch (Exception ex)
            {
                foreach (var hbProduct in hBSelectedProducts)
                {
                    var option = hbProduct?.ProductOptions?.FirstOrDefault(x => x.IsSelected);
                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == hbProduct.AvailabilityReferenceId);

                    var fileNumber = string.IsNullOrWhiteSpace(((HotelBedsSelectedProduct)hbProduct).FileNumber)
                        ? string.Empty
                        : ((HotelBedsSelectedProduct)hbProduct)?.FileNumber;

                    bookedProduct.OptionStatus = GetBookingStatusNumber(fileNumber, option.AvailabilityStatus);
                    if (!bookedProducts.Any(x => x.AvailabilityReferenceId == bookedProduct.AvailabilityReferenceId))
                    {
                        bookedProducts.Add(bookedProduct);
                    }
                }

                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateHbActivityBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError, System.Net.HttpStatusCode.BadGateway, $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, hBSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber ?? string.Empty, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.Hotelbeds), hBSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        hBSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, hBSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }

        public List<BookedProduct> CreateMoulinRougeProductsBooking(ActivityBookingCriteria criteria)
        {
            var request = string.Empty;
            var response = string.Empty;
            var moulinRougeSelectedProducts = new List<MoulinRougeSelectedProduct>();
            var bookedProducts = new List<BookedProduct>();
            var selectedProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.
                                    Equals(APIType.Moulinrouge)).ToList();
            try
            {
                #region AddToCart call

                foreach (var item in selectedProducts.OfType<MoulinRougeSelectedProduct>())
                {

                    var rowKey = $"{criteria.Booking.ReferenceNumber}-{item.AvailabilityReferenceId}";
                    var ReservationDetails = _tableStorageOperation.RetrieveReservationData(rowKey);
                    var createOrderResponse = SerializeDeSerializeHelper.DeSerialize<Response>(ReservationDetails?.ReservationResponse);
                    var result = new Tuple<string, string>("", "");

                    if (ReservationDetails != null)
                    {
                        result = MoulinRougeReservationGet(item, createOrderResponse, criteria.Booking, criteria.Token);
                    }
                    if (ReservationDetails == null && String.IsNullOrEmpty(item.TemporaryOrderId))
                    {
                        var apiResult = MoulinRougeAddtoCart(item, criteria.Token);
                    }
                    item.FirstName = criteria.Booking.User.FirstName;
                    item.FullName = $"{criteria.Booking.User.FirstName} {criteria.Booking.User.LastName}";
                    item.IsangoBookingReferenceNumber = criteria.Booking.ReferenceNumber;
                    item.TemporaryOrderId = item.TemporaryOrderId;
                    item.TemporaryOrderRowId = item.TemporaryOrderRowId;
                    item.Expiry = item.Expiry;
                    moulinRougeSelectedProducts.Add(item);
                }

                #endregion AddToCart call

                #region Create Booking Call

                foreach (var moulinSelecteditem in moulinRougeSelectedProducts)
                {
                    var mrSelectedProduct = moulinSelecteditem;
                    try
                    {
                        if (string.IsNullOrWhiteSpace(moulinSelecteditem.OrderId))
                        {
                            mrSelectedProduct = _moulinRougeAdapter.OrderConfirmCombined(moulinSelecteditem,
                                out request, out response, criteria.Token);
                            mrSelectedProduct.HotelPickUpLocation = moulinSelecteditem.HotelPickUpLocation;
                        }

                        mrSelectedProduct.ProductOptions = UpdateOptionStatus(mrSelectedProduct.ProductOptions,
                            moulinSelecteditem.OrderId);

                        var logCriteria = new LogPurchaseXmlCriteria
                        {
                            RequestXml = request,
                            ResponseXml = response,
                            Status = !string.IsNullOrWhiteSpace(moulinSelecteditem.OrderId)
                                ? Constant.StatusSuccess : Constant.StatusFailed,
                            BookingId = criteria.Booking.BookingId,
                            BookingReferenceNumber = criteria.Booking.ReferenceNumber,
                            APIType = APIType.Moulinrouge,
                            ApiRefNumber = moulinSelecteditem.OrderId
                        };
                        _supplierBookingPersistence.SaveMoulinRougeTicketBytes(mrSelectedProduct);
                        _supplierBookingPersistence.LogPurchaseXML(logCriteria);

                        var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                            x.AvailabilityReferenceId == moulinSelecteditem.AvailabilityReferenceId);

                        if (bookedProduct == null || mrSelectedProduct == null)
                        {
                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);

                            break;
                        }
                        bookedProduct = MapProductForMR(bookedProduct, mrSelectedProduct);
                        bookedProducts.Add(bookedProduct);
                    }
                    catch (Exception ex)
                    {
                        mrSelectedProduct.ProductOptions = UpdateOptionStatus(mrSelectedProduct.ProductOptions,
                            moulinSelecteditem.OrderId);
                        var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                            x.AvailabilityReferenceId == moulinSelecteditem.AvailabilityReferenceId);
                        //if (bookedProduct == null) continue;

                        var option = mrSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                        bookedProduct.OptionStatus = GetBookingStatusNumber(mrSelectedProduct.OrderId, option.AvailabilityStatus);
                        if (!string.IsNullOrWhiteSpace(mrSelectedProduct.OrderId))
                        {
                            if (bookedProduct.APIExtraDetail == null)
                                bookedProduct.APIExtraDetail = new ApiExtraDetail();
                            bookedProduct.APIExtraDetail.SupplieReferenceNumber = mrSelectedProduct.OrderId;
                        }

                        bookedProducts.Add(bookedProduct);
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "SupplierBookingService",
                            MethodName = "CreateMoulinRougeProductsBooking-OrderConfirmCombined",
                            Token = criteria.Token,
                            Params = $"{mrSelectedProduct.Id}|{SerializeDeSerializeHelper.Serialize(criteria)}"
                        };
                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError, System.Net.HttpStatusCode.BadGateway, $"Exception\n {ex.Message}\n{response}");


                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, moulinRougeSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber ?? string.Empty, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.Moulinrouge), moulinRougeSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                moulinRougeSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, moulinRougeSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }

                        _log.Error(isangoErrorEntity, ex);
                        break;
                    }
                }

                #endregion Create Booking Call

                return bookedProducts;
            }
            catch (Exception ex)
            {
                var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
                if (isangoBookedProducts == null) return bookedProducts;

                foreach (var selectedProduct in selectedProducts)
                {
                    var bookedProduct = isangoBookedProducts.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(selectedProduct.AvailabilityReferenceId));
                    if (bookedProduct == null) continue;
                    bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                    bookedProducts.Add(bookedProduct);
                }
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateMoulinRougeProductsBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError, System.Net.HttpStatusCode.BadGateway, $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, moulinRougeSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber ?? string.Empty, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.Moulinrouge), moulinRougeSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        moulinRougeSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, moulinRougeSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ex);
                return bookedProducts;
            }
        }

        public List<BookedProduct> CreateMoulinRougeBookingReservation(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var selectedProducts = new List<SelectedProduct>();
            var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (criteria.Booking.SelectedProducts != null && criteria.Booking.SelectedProducts.Count > 0)
                {
                    selectedProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.Moulinrouge)).ToList();
                    var logPurchaseCriteria = new LogPurchaseXmlCriteria
                    {
                        BookingId = criteria.Booking.BookingId,
                        APIType = APIType.Moulinrouge,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber
                    };


                    var createOrderProducts = new Dictionary<string, Response>();
                    foreach (var product in selectedProducts)
                    {
                        try
                        {
                            _supplierBookingPersistence.InsertReserveRequest(criteria.Token, product.AvailabilityReferenceId);
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }
                        var selectedOption = (ActivityOption)product.ProductOptions.FirstOrDefault(x => x.IsSelected);
                        var selectedProduct = product as MoulinRougeSelectedProduct;
                        if (selectedProduct == null) continue;
                        selectedProduct.Supplier = new Supplier
                        {
                            AddressLine1 = criteria.Booking.User.Address1,
                            ZipCode = criteria.Booking.User.ZipCode,
                            City = criteria.Booking.User.City,
                            PhoneNumber = string.Empty
                        };
                        //selectedProduct.ReservationExpiry = criteria.Booking.ReferenceNumber;
                        selectedProduct.ProductOptions[0].Customers[0].Email = criteria.Booking.VoucherEmailAddress;

                        //Reservation API Call if ticket class is two or three

                        var serviceId = selectedProduct.Id.ToString();
                        var travelDate = selectedProduct.ProductOptions[0].TravelInfo.StartDate.ToShortDateString();


                        var moulinRougeReservationResult = MoulinRougeAddtoCartAPI(selectedProduct, criteria.Token);
                        if (moulinRougeReservationResult != null)
                        {
                            selectedProduct.TemporaryOrderId = moulinRougeReservationResult.Body.ACP_AllocSeatsAutomaticRequestResponse.id_TemporaryOrder;
                            selectedProduct.TemporaryOrderRowId = selectedProduct.TemporaryOrderId;
                        }

                        selectedProduct.FirstName = criteria.Booking.User.FirstName;
                        selectedProduct.FullName = $"{criteria.Booking.User.FirstName} {criteria.Booking.User.LastName}";
                        selectedProduct.IsangoBookingReferenceNumber = criteria.Booking.ReferenceNumber;

                        if (!string.IsNullOrWhiteSpace(selectedProduct.TemporaryOrderId))
                        {

                            var reservationDetails = new SupplierBookingReservationResponse()
                            {
                                ApiType = Convert.ToInt32(APIType.Moulinrouge),
                                ServiceOptionId = selectedOption?.ServiceOptionId ?? selectedOption?.BundleOptionID ?? 0,
                                AvailabilityReferenceId = product.AvailabilityReferenceId,
                                Status = !string.IsNullOrWhiteSpace(selectedProduct.TemporaryOrderId) ? Constant.StatusSuccess : Constant.StatusFailed,
                                BookedOptionId = criteria.Booking.BookingId,
                                BookingReferenceNo = criteria.Booking.ReferenceNumber,
                                OptionName = selectedOption?.Name ?? selectedOption.BundleOptionName,
                                ReservationResponse = SerializeDeSerializeHelper.Serialize(moulinRougeReservationResult),
                                ReservationReferenceId = selectedProduct.TemporaryOrderId,
                                Token = criteria.Token
                            };

                            _tableStorageOperation.InsertReservationDetails(reservationDetails);

                            InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.Reservation, Constant.StatusSuccess);

                            try
                            {
                                _supplierBookingPersistence.UpdateReserveRequest(criteria.Token, product.AvailabilityReferenceId, criteria.Booking.ReferenceNumber);
                            }
                            catch (Exception ex)
                            {
                                //ignore
                            }

                            createOrderProducts.Add(product.AvailabilityReferenceId, moulinRougeReservationResult);
                        }
                        else
                        {
                            InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.Reservation, Constant.StatusFailed);

                            //If the create order call is failed for any of the product then set all product booking status as failed
                            bookedProducts = SetFailedBookingStatus(selectedProducts, criteria);

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, product?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    selectedProduct.TemporaryOrderId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.Moulinrouge), product?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    product?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, product?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();


                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            return bookedProducts;
                        }

                        //throw new NullReferenceException("for testing"); //for booking cancel testing
                    }

                    foreach (var product in createOrderProducts)
                    {
                        var moulinRougeSelectedProduct = selectedProducts.OfType<MoulinRougeSelectedProduct>().FirstOrDefault(x => x.AvailabilityReferenceId == product.Key);
                        try
                        {
                            moulinRougeSelectedProduct.ProductOptions = UpdateOptionStatus(moulinRougeSelectedProduct.ProductOptions, moulinRougeSelectedProduct.TemporaryOrderId);

                            var bookedProduct = criteria.Booking.IsangoBookingData?.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == moulinRougeSelectedProduct.AvailabilityReferenceId);

                            if (bookedProduct == null) continue;

                            bookedProduct = MapProductForMR(bookedProduct, moulinRougeSelectedProduct);
                            bookedProduct.OptionStatus = Constant.StatusSuccess;
                            bookedProducts.Add(bookedProduct);
                        }
                        catch (Exception ex)
                        {
                            moulinRougeSelectedProduct.ProductOptions = UpdateOptionStatus(moulinRougeSelectedProduct.ProductOptions, moulinRougeSelectedProduct.TemporaryOrderId);
                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == moulinRougeSelectedProduct.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;

                            var option = moulinRougeSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                            bookedProduct.OptionStatus = GetBookingStatusNumber(moulinRougeSelectedProduct.TemporaryOrderId, option.AvailabilityStatus);
                            if (!string.IsNullOrWhiteSpace(moulinRougeSelectedProduct.TemporaryOrderId))
                            {
                                if (bookedProduct.APIExtraDetail == null)
                                    bookedProduct.APIExtraDetail = new ApiExtraDetail();
                                bookedProduct.APIExtraDetail.SupplieReferenceNumber = moulinRougeSelectedProduct.TemporaryOrderId;
                            }

                            bookedProducts.Add(bookedProduct);
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "SupplierBookingService",
                                MethodName = "CreateMoulinRougeBookingReservation",
                                Token = criteria.Token,
                                Params = $"{moulinRougeSelectedProduct.Id}|{SerializeDeSerializeHelper.Serialize(moulinRougeSelectedProduct)}"
                            };

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, moulinRougeSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                               moulinRougeSelectedProduct.TemporaryOrderId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.Moulinrouge), moulinRougeSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                moulinRougeSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, moulinRougeSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (selectedProducts == null || !selectedProducts.Any()) return bookedProducts;
                selectedProducts.ForEach(product =>
                {
                    var bookedProduct = isangoBookedProducts?.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(product.AvailabilityReferenceId));
                    if (bookedProduct != null)
                    {
                        bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                        bookedProducts.Add(bookedProduct);
                    }
                });
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateMoulinRougeBookingReservation",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }

        public List<BookedProduct> CreateVentrataProductsBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var ventrataProducts = new List<SelectedProduct>();
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (criteria.Booking.SelectedProducts != null && criteria.Booking.SelectedProducts.Count > 0)
                {
                    ventrataProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.Ventrata)).ToList();
                    var logPurchaseCriteria = new LogPurchaseXmlCriteria
                    {
                        BookingId = criteria.Booking.BookingId,
                        APIType = APIType.Ventrata,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber
                    };

                    foreach (var product in ventrataProducts)
                    {
                        var ventrataSelectedProduct = product as VentrataSelectedProduct;
                        if (ventrataSelectedProduct == null) continue;
                        ventrataSelectedProduct.Supplier = new Supplier
                        {
                            AddressLine1 = criteria.Booking.User.Address1,
                            ZipCode = criteria.Booking.User.ZipCode,
                            City = criteria.Booking.User.City,
                            PhoneNumber = string.Empty
                        };

                        var serviceId = ventrataSelectedProduct.Id.ToString();
                        var travelDate = ventrataSelectedProduct.ProductOptions[0].TravelInfo.StartDate.ToShortDateString();

                        ventrataSelectedProduct.Uuid = criteria.Booking.ReferenceNumber;
                        ventrataSelectedProduct.ResellerReference = criteria.Booking.ReferenceNumber;
                        ventrataSelectedProduct.ProductOptions[0].Customers[0].Email = criteria.Booking.VoucherEmailAddress;

                        //Start- Get Reservation from Storage
                        var rowKey = $"{criteria.Booking.ReferenceNumber}-{product.AvailabilityReferenceId}";
                        var ReservationDetails = _tableStorageOperation.RetrieveReservationData(rowKey);
                        var createOrderResponse = SerializeDeSerializeHelper.DeSerialize<BookingReservationRes>(ReservationDetails?.ReservationResponse);
                        var result = new Tuple<string, string>("", "");
                        if (ReservationDetails != null)
                        {
                            result = VentrataReservationGet(ventrataSelectedProduct, createOrderResponse, criteria.Booking, criteria.Token);
                        }
                        //End- Get Reservation from Storage
                        //if (ReservationDetails == null && String.IsNullOrEmpty(ventrataSelectedProduct.SupplierReference))
                        else// 2d flow not work due to above condition.
                        {
                            result = VentrataReservationWithLogging(ventrataSelectedProduct, criteria.Token, criteria);
                        }
                        //End
                        var reservationReq = result?.Item1;
                        var reservationRes = result?.Item2 + "serviceID : " + serviceId + ", travelDate : " + travelDate;

                        if (criteria?.Booking?.Errors?.Any() == true)
                        {
                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                    x.AvailabilityReferenceId == ventrataSelectedProduct.AvailabilityReferenceId);
                            if (bookedProduct != null)
                            {
                                bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                                bookedProducts.Add(bookedProduct);
                            }
                            break;
                        }

                        //if it is successfully reserved then go for Create Booking API Call
                        if (ventrataSelectedProduct.BookingStatus?.ToLower() == VentrataEntities.VentrataBookingStatus.OnHold.ToLower())
                        {
                            if (!string.IsNullOrEmpty(result?.Item1))
                            {
                                logPurchaseCriteria.RequestXml = reservationReq;
                                logPurchaseCriteria.ResponseXml = reservationRes;
                                logPurchaseCriteria.Status = Constant.StatusSuccess;
                                logPurchaseCriteria.Bookingtype = Constant.StatusReservation;
                                _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);
                            }
                            ventrataSelectedProduct.ApiBookingDetails = (VentrataApiBookingDetails)_ventrataAdapter.CreateBooking(ventrataSelectedProduct, criteria.Token, out request, out response);
                        }
                        else
                        {
                            logPurchaseCriteria.RequestXml = reservationReq;
                            logPurchaseCriteria.ResponseXml = reservationRes;
                            logPurchaseCriteria.Status = Constant.StatusFailed;
                            logPurchaseCriteria.Bookingtype = Constant.StatusReservation;
                            _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                            foreach (var ventrataProduct in ventrataProducts)
                            {
                                var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                    x.AvailabilityReferenceId == ventrataProduct.AvailabilityReferenceId);
                                if (bookedProduct == null) continue;
                                bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                                bookedProducts.Add(bookedProduct);
                            }

                            return bookedProducts;
                        }

                        if (ventrataSelectedProduct.ApiBookingDetails != null && string.Equals(ventrataSelectedProduct.ApiBookingDetails.Status, VentrataEntities.VentrataBookingStatus.Confirmed, StringComparison.CurrentCultureIgnoreCase))
                        {
                            logPurchaseCriteria.RequestXml = request;
                            logPurchaseCriteria.ResponseXml = response;
                            logPurchaseCriteria.Bookingtype = Constant.StatusBooking;
                            logPurchaseCriteria.Status = Constant.StatusSuccess;
                            _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == ventrataSelectedProduct.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;
                            //Todo
                            bookedProduct = MapProductForVentrata(bookedProduct, ventrataSelectedProduct);
                            bookedProducts.Add(bookedProduct);
                        }
                        else
                        {
                            logPurchaseCriteria.RequestXml = request;
                            logPurchaseCriteria.ResponseXml = response;
                            logPurchaseCriteria.Bookingtype = Constant.StatusBooking;
                            logPurchaseCriteria.Status = Constant.StatusFailed;
                            _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                            foreach (var ventrataProduct in ventrataProducts)
                            {
                                var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                    x.AvailabilityReferenceId == ventrataProduct.AvailabilityReferenceId);
                                if (bookedProduct == null) continue;
                                bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                                bookedProducts.Add(bookedProduct);
                            }
                            //Api booking failed
                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                    , System.Net.HttpStatusCode.BadGateway
                                    , response);

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, ventrataSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.Ventrata), ventrataSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    ventrataSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, ventrataSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }


                            return bookedProducts;
                        }

                        #region Update Product Option Status

                        ventrataSelectedProduct.ProductOptions = UpdateOptionStatus(ventrataSelectedProduct.ProductOptions, ventrataSelectedProduct.SupplierReference);

                        #endregion Update Product Option Status
                    }
                }
            }
            catch (Exception ex)
            {
                foreach (var ventrataProduct in ventrataProducts)
                {
                    var option = ventrataProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == ventrataProduct.AvailabilityReferenceId);

                    if (bookedProduct == null) continue;

                    var ventrataBookingReference = ((VentrataSelectedProduct)ventrataProduct)?.SupplierReference;

                    var bookingReference = ventrataBookingReference == "0" || string.IsNullOrWhiteSpace(ventrataBookingReference)
                        ? string.Empty
                        : ventrataBookingReference;

                    bookedProduct.OptionStatus = GetBookingStatusNumber(bookingReference, option?.AvailabilityStatus);
                    bookedProducts.Add(bookedProduct);
                }

                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = nameof(SupplierBookingService),
                    MethodName = nameof(CreateVentrataProductsBooking),
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError, System.Net.HttpStatusCode.BadGateway, $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, ventrataProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.Ventrata), ventrataProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        ventrataProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, ventrataProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }

        private Tuple<string, string> VentrataReservationWithLogging(VentrataSelectedProduct selectedProduct, string token, ActivityBookingCriteria criteria)
        {
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                var castedReservationResObject = new BookingReservationRes();

                //check Package or Not
                var selectedOption = (ActivityOption)selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                var productid = (selectedOption)?.VentrataProductId;
                var optionid = selectedOption.SupplierOptionCode;
                var ventrataPackagesOrNot = GetVentrataPackages(productid, optionid);

                var ventrataPaxMapping = _masterCacheManager.GetVentrataPaxMappings();
                if (ventrataPaxMapping?.Any() != true)
                {
                    ventrataPaxMapping = GetPaxMappingsForVentrataAPI(APIType.Ventrata);
                }

                selectedProduct.VentrataPaxMappings = ventrataPaxMapping.Where(x => x.ServiceOptionId == selectedOption.ServiceOptionId)?.ToList();

                var reservationResponseObject = _ventrataAdapter.CreateReservation(selectedProduct, out request, out response, token, ventrataPackagesOrNot);
                if (reservationResponseObject != null)
                {
                    castedReservationResObject = ((BookingReservationRes)reservationResponseObject);
                    selectedProduct.Uuid = castedReservationResObject.uuid;
                    selectedProduct.SupplierReference = castedReservationResObject.supplierReference;
                    selectedProduct.TestMode = castedReservationResObject.testMode;
                    selectedProduct.BookingStatus = castedReservationResObject.status;
                    selectedProduct.IsCancellable = castedReservationResObject.cancellable;
                    selectedProduct.IsPackage = Convert.ToBoolean(castedReservationResObject.isPackage);
                }
                else
                {
                    //Api booking failed
                    criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);

                    try
                    {
                        LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, selectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                            selectedProduct?.SupplierReference, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.Ventrata), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                        criteria?.Booking?.UpdateDBLogFlag();
                    }
                    catch (Exception e)
                    {
                        //ignore
                    }

                }

                return new Tuple<string, string>(request, response);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "VentrataReservationWithLogging",
                    Token = token,
                    Params = $"{selectedProduct}"
                };

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError, System.Net.HttpStatusCode.BadGateway, $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, selectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        selectedProduct?.SupplierReference, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.Ventrata), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public Dictionary<string, bool> VentrataCancelReservationAndBooking(List<VentrataSelectedProduct> ventrataSelectedProducts, string bookingReferenceNo, string token, Booking booking)
        {
            var status = new Dictionary<string, bool>();
            var request = String.Empty;
            var response = String.Empty;
            ventrataSelectedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });

            var isCancelled = false;

            var logCriteria = new LogPurchaseXmlCriteria
            {
                BookingId = 0,
                APIType = APIType.Ventrata,
                BookingReferenceNumber = bookingReferenceNo
            };
            var supplierBaseURLlist = GetVentrataData();
            foreach (var ventrataSelectedProduct in ventrataSelectedProducts)
            {
                try
                {
                    if (supplierBaseURLlist != null && supplierBaseURLlist.Count > 0)
                    {
                        ventrataSelectedProduct.VentrataBaseURL = supplierBaseURLlist?.Where(x => x.SupplierBearerToken == ventrataSelectedProduct.ActivityCode)?.ToList()?.FirstOrDefault()?.BaseURL;
                    }

                    var ventrataBookingStatus = ventrataSelectedProduct.ApiBookingDetails?.Status;
                    var reservationStatus = ventrataSelectedProduct.BookingStatus;
                    var isBookingMade = string.Equals(ventrataBookingStatus, VentrataEntities.VentrataBookingStatus.Confirmed, StringComparison.CurrentCultureIgnoreCase) ? true : false;
                    var isReservationMade = string.Equals(reservationStatus, VentrataEntities.VentrataBookingStatus.OnHold, StringComparison.CurrentCultureIgnoreCase) ? true : false;
                    if (ventrataSelectedProduct.ApiBookingDetails != null && ventrataSelectedProduct.IsCancellable && (isBookingMade || isReservationMade))
                    {
                        //Cancel Booking
                        var cancellationStatus = _ventrataAdapter.CancelReservationAndBooking(ventrataSelectedProduct, token, out request, out response);
                        if (cancellationStatus?.ToLowerInvariant() == BookingStatus.Cancelled.ToString().ToLowerInvariant() ||
                            cancellationStatus?.ToLowerInvariant() == BookingStatus.EXPIRED.ToString().ToLowerInvariant())
                        {
                            logCriteria.Bookingtype = isBookingMade ? "Cancel Booking" : "Cancel Reservation";
                            isCancelled = true;
                        }
                    }

                    logCriteria.RequestXml = request;
                    logCriteria.ResponseXml = response;
                    logCriteria.Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed;

                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                    status[ventrataSelectedProduct.AvailabilityReferenceId] = isCancelled;
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = nameof(SupplierBookingService),
                        MethodName = nameof(VentrataCancelReservationAndBooking),
                        Token = token,
                        Params = $"{SerializeDeSerializeHelper.Serialize(ventrataSelectedProducts)}{bookingReferenceNo}{token}"
                    };

                    _log.Error(isangoErrorEntity, ex);
                }
            }

            return status;
        }

        public List<BookedProduct> CreatePrioProductsBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var prioProducts = new List<SelectedProduct>();
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (criteria.Booking.SelectedProducts != null && criteria.Booking.SelectedProducts.Count > 0)
                {
                    prioProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.Prio)).ToList();
                    var logPurchaseCriteria = new LogPurchaseXmlCriteria
                    {
                        BookingId = criteria.Booking.BookingId,
                        APIType = APIType.Prio,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber
                    };

                    foreach (var product in prioProducts)
                    {
                        var selectedProduct = product as PrioSelectedProduct;
                        if (selectedProduct == null) continue;
                        selectedProduct.Supplier = new Supplier
                        {
                            AddressLine1 = criteria.Booking.User.Address1,
                            ZipCode = criteria.Booking.User.ZipCode,
                            City = criteria.Booking.User.City,
                            PhoneNumber = string.Empty
                        };
                        selectedProduct.ReservationExpiry = criteria.Booking.ReferenceNumber;
                        selectedProduct.ProductOptions[0].Customers[0].Email = criteria.Booking.VoucherEmailAddress;
                        //Reservation API Call if ticket class is two or three

                        if (selectedProduct.PrioTicketClass == (int)TicketClass.TicketClassTwo || selectedProduct.PrioTicketClass == (int)TicketClass.TicketClassThree)
                        {
                            var serviceId = selectedProduct.Id.ToString();
                            var travelDate = selectedProduct.ProductOptions[0].TravelInfo.StartDate.ToShortDateString();
                            //Set values in  selectedProduct as PrioReservationReference, PrioDistributorReference and PrioBookingStatus

                            var result = PrioReservationWithLogging(selectedProduct, selectedProduct.ReservationExpiry, criteria.Token, criteria.Booking);
                            var reservationReq = result.Item1;
                            var reservationRes = result.Item2 + "serviceID : " + serviceId + ", travelDate : " + travelDate;

                            //if it is successfully reserved then go for Create Booking API Call
                            if (selectedProduct.PrioBookingStatus?.ToLower() == PrioApiStatus.Reserved.ToLower())
                            {
                                logPurchaseCriteria.RequestXml = reservationReq;
                                logPurchaseCriteria.ResponseXml = reservationRes;
                                logPurchaseCriteria.Status = Constant.StatusSuccess;
                                logPurchaseCriteria.Bookingtype = Constant.StatusReservation;
                                _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                                selectedProduct.PrioApiConfirmedBooking = _prioTicketAdapter.CreateBooking(selectedProduct, criteria.Token, out request, out response);
                            }
                            else
                            {
                                logPurchaseCriteria.RequestXml = reservationReq;
                                logPurchaseCriteria.ResponseXml = reservationRes;
                                logPurchaseCriteria.Status = Constant.StatusFailed;
                                logPurchaseCriteria.Bookingtype = Constant.StatusReservation;
                                _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                                foreach (var prioProduct in prioProducts)
                                {
                                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                        x.AvailabilityReferenceId == prioProduct.AvailabilityReferenceId);
                                    if (bookedProduct == null) continue;
                                    bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                                    bookedProducts.Add(bookedProduct);
                                }

                                return bookedProducts;
                                //break;
                            }
                        }
                        else
                        {
                            selectedProduct.PrioApiConfirmedBooking = _prioTicketAdapter.CreateBooking(selectedProduct, criteria.Token, out request, out response);
                        }

                        if (selectedProduct.PrioApiConfirmedBooking != null && string.IsNullOrEmpty(selectedProduct.PrioApiConfirmedBooking.ErrorCode) &&
                            string.Equals(selectedProduct.PrioApiConfirmedBooking.BookingStatus, PrioApiStatus.Confirmed, StringComparison.CurrentCultureIgnoreCase))
                        {
                            logPurchaseCriteria.RequestXml = request;
                            logPurchaseCriteria.ResponseXml = response;
                            logPurchaseCriteria.Bookingtype = Constant.StatusBooking;
                            logPurchaseCriteria.Status = Constant.StatusSuccess;
                            _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;
                            bookedProduct = MapProductForPrio(bookedProduct, selectedProduct);
                            bookedProducts.Add(bookedProduct);
                        }
                        else
                        {
                            logPurchaseCriteria.RequestXml = request;
                            logPurchaseCriteria.ResponseXml = response;
                            logPurchaseCriteria.Bookingtype = Constant.StatusBooking;
                            logPurchaseCriteria.Status = Constant.StatusFailed;
                            _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                            foreach (var prioProduct in prioProducts)
                            {
                                var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                    x.AvailabilityReferenceId == prioProduct.AvailabilityReferenceId);
                                if (bookedProduct == null) continue;
                                bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                                bookedProducts.Add(bookedProduct);
                            }
                            //Api booking failed
                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                    , System.Net.HttpStatusCode.BadGateway
                                    , response);

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, prioProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.Prio), prioProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    prioProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, prioProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            return bookedProducts;
                            //break;
                        }

                        #region Update Product Option Status

                        selectedProduct.ProductOptions = UpdateOptionStatus(selectedProduct.ProductOptions, selectedProduct.PrioApiConfirmedBooking?.BookingReference);

                        #endregion Update Product Option Status
                    }
                }
            }
            catch (Exception ex)
            {
                foreach (var prioProduct in prioProducts)
                {
                    var option = prioProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == prioProduct.AvailabilityReferenceId);

                    if (bookedProduct == null) continue;

                    var prioBookingReference = ((PrioSelectedProduct)prioProduct)?.PrioApiConfirmedBooking?.BookingReference;

                    var bookingReference = prioBookingReference == "0" || string.IsNullOrWhiteSpace(prioBookingReference)
                        ? string.Empty
                        : prioBookingReference;

                    bookedProduct.OptionStatus = GetBookingStatusNumber(bookingReference, option?.AvailabilityStatus);
                    bookedProducts.Add(bookedProduct);
                }

                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreatePrioProductsBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, prioProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.Prio), prioProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        prioProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, prioProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }


                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }

        public List<BookedProduct> CreatePrioHubProductsBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var prioHubProducts = new List<SelectedProduct>();
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (criteria.Booking.SelectedProducts != null && criteria.Booking.SelectedProducts.Count > 0)
                {
                    prioHubProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.PrioHub)).ToList();
                    var logPurchaseCriteria = new LogPurchaseXmlCriteria
                    {
                        BookingId = criteria.Booking.BookingId,
                        APIType = APIType.PrioHub,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber
                    };

                    foreach (var product in prioHubProducts)
                    {
                        var selectedProduct = product as PrioHubSelectedProduct;
                        if (selectedProduct == null) continue;
                        selectedProduct.Supplier = new Supplier
                        {
                            AddressLine1 = criteria.Booking.User.Address1,
                            ZipCode = criteria.Booking.User.ZipCode,
                            City = criteria.Booking.User.City,
                            PhoneNumber = string.Empty
                        };
                        selectedProduct.ReservationExpiry = criteria.Booking.ReferenceNumber;
                        selectedProduct.ProductOptions[0].Customers[0].Email = criteria.Booking.VoucherEmailAddress;

                        //Reservation API Call if ticket class is two or three

                        var serviceId = selectedProduct.Id.ToString();
                        var travelDate = selectedProduct.ProductOptions[0].TravelInfo.StartDate.ToShortDateString();
                        //Set values in  selectedProduct as PrioReservationReference, PrioDistributorReference and PrioBookingStatus
                        //Start- Get Reservation from Storage
                        var rowKey = $"{criteria.Booking.ReferenceNumber}-{product.AvailabilityReferenceId}";
                        var ReservationDetails = _tableStorageOperation.RetrieveReservationData(rowKey);
                        var createOrderResponse = SerializeDeSerializeHelper.DeSerialize<ReservationData.ReservationResponse>(ReservationDetails?.ReservationResponse);
                        var result = new Tuple<string, string>("", "");
                        if (ReservationDetails != null)
                        {
                            result = PrioHubReservationGet(selectedProduct, createOrderResponse, criteria.Booking, criteria.Token);
                        }
                        //End- Get Reservation from Storage
                        if (ReservationDetails == null && String.IsNullOrEmpty(selectedProduct.PrioReservationReference))
                        {
                            result = PrioHubReservationWithLogging(selectedProduct, selectedProduct.ReservationExpiry, criteria.Token, criteria.Booking);
                        }
                        var reservationReq = result?.Item1;
                        var reservationRes = result?.Item2 + "serviceID : " + serviceId + ", travelDate : " + travelDate;

                        //if it is successfully reserved then go for Create Booking API Call
                        if (selectedProduct.PrioHubReservationStatus?.ToUpper() == "BOOKING_RESERVED")
                        {
                            if (!string.IsNullOrEmpty(result?.Item1))
                            {
                                logPurchaseCriteria.RequestXml = reservationReq;
                                logPurchaseCriteria.ResponseXml = reservationRes;
                                logPurchaseCriteria.Status = Constant.StatusSuccess;
                                logPurchaseCriteria.Bookingtype = Constant.StatusReservation;
                                _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);
                            }
                            selectedProduct.PrioHubApiConfirmedBooking = _prioHubAdapter.CreateBooking
                            (selectedProduct, criteria.Token, out request, out response,
                            criteria.Booking.ReferenceNumber);

                        }
                        else
                        {
                            logPurchaseCriteria.RequestXml = reservationReq;
                            logPurchaseCriteria.ResponseXml = reservationRes;
                            logPurchaseCriteria.Status = Constant.StatusFailed;
                            logPurchaseCriteria.Bookingtype = Constant.StatusReservation;
                            _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                            foreach (var prioProduct in prioHubProducts)
                            {
                                var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                    x.AvailabilityReferenceId == prioProduct.AvailabilityReferenceId);
                                if (bookedProduct == null) continue;
                                bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                                bookedProducts.Add(bookedProduct);
                            }

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, prioHubProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.PrioHub), prioHubProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    prioHubProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, prioHubProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            return bookedProducts;
                            //break;
                        }


                        if (selectedProduct.PrioHubApiConfirmedBooking != null && string.IsNullOrEmpty(selectedProduct.PrioHubApiConfirmedBooking.ErrorCode) &&
                            string.Equals(selectedProduct.PrioHubApiConfirmedBooking.BookingStatus.ToUpper(), ConstantPrioHub.BOOKINGCONFIRMED, StringComparison.CurrentCultureIgnoreCase)
                            || string.Equals(selectedProduct.PrioHubApiConfirmedBooking.BookingStatus.ToUpper(), ConstantPrioHub.ORDERPENDING, StringComparison.CurrentCultureIgnoreCase)
                            || string.Equals(selectedProduct.PrioHubApiConfirmedBooking.BookingStatus.ToUpper(), ConstantPrioHub.BOOKINGPROCESSINGCONFIRMATION, StringComparison.CurrentCultureIgnoreCase))
                        {
                            logPurchaseCriteria.RequestXml = request;
                            logPurchaseCriteria.ResponseXml = response;
                            logPurchaseCriteria.Bookingtype = Constant.StatusBooking;
                            logPurchaseCriteria.Status = Constant.StatusSuccess;
                            _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                            if (string.Equals(selectedProduct.PrioHubApiConfirmedBooking.BookingStatus.ToUpper(), ConstantPrioHub.ORDERPENDING, StringComparison.CurrentCultureIgnoreCase)
                                || string.Equals(selectedProduct.PrioHubApiConfirmedBooking.BookingStatus.ToUpper(), ConstantPrioHub.BOOKINGPROCESSINGCONFIRMATION, StringComparison.CurrentCultureIgnoreCase))
                            {

                                selectedProduct.ProductOptions.ForEach(x =>
                                {
                                    x.AvailabilityStatus = AvailabilityStatus.ONREQUEST;
                                });

                                try
                                {
                                    var retryThreshold = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsRetryThreshold));
                                    InsertAsyncJob(selectedProduct?.PrioHubApiConfirmedBooking?.BookingReference,
                                       criteria.Booking.Language.Code.ToLowerInvariant(),
                                       criteria.Token, criteria, selectedProduct, criteria?.Booking?.Affiliate?.Id, Convert.ToInt32(APIType.PrioHub), "booking",
                                       selectedProduct?.PrioHubDistributerId, retryThreshold);
                                }
                                catch (Exception ex)
                                {
                                    var isangoErrorEntity = new IsangoErrorEntity
                                    {
                                        ClassName = "SupplierBookingService",
                                        MethodName = "InsertAsyncJob",
                                        Token = criteria.Token,
                                        Params = $"{selectedProduct.Id}|{SerializeDeSerializeHelper.Serialize(selectedProduct)}"
                                    };
                                    _log.Error(isangoErrorEntity, ex);
                                }
                            }

                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;
                            bookedProduct = MapProductForPrioHub(bookedProduct, selectedProduct, criteria.Token);
                            bookedProducts.Add(bookedProduct);
                        }
                        else
                        {
                            logPurchaseCriteria.RequestXml = request;
                            logPurchaseCriteria.ResponseXml = response;
                            logPurchaseCriteria.Bookingtype = Constant.StatusBooking;
                            logPurchaseCriteria.Status = Constant.StatusFailed;
                            _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                            foreach (var prioProduct in prioHubProducts)
                            {
                                var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                    x.AvailabilityReferenceId == prioProduct.AvailabilityReferenceId);
                                if (bookedProduct == null) continue;
                                bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                                bookedProducts.Add(bookedProduct);
                            }
                            //Api booking failed
                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                    , System.Net.HttpStatusCode.BadGateway
                                    , response);

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, prioHubProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.PrioHub), prioHubProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    prioHubProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, prioHubProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }


                            return bookedProducts;
                            //break;
                        }

                        #region Update Product Option Status

                        selectedProduct.ProductOptions = UpdateOptionStatus(selectedProduct.ProductOptions, selectedProduct.PrioHubApiConfirmedBooking?.BookingReference);

                        #endregion Update Product Option Status
                        //throw new NullReferenceException("for testing"); //for booking cancel testing
                    }
                }
            }
            catch (Exception ex)
            {
                foreach (var prioProduct in prioHubProducts)
                {
                    var option = prioProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == prioProduct.AvailabilityReferenceId);

                    if (bookedProduct == null) continue;

                    var prioBookingReference = ((PrioHubSelectedProduct)prioProduct)?.PrioHubApiConfirmedBooking?.BookingReference;

                    var bookingReference = prioBookingReference == "0" || string.IsNullOrWhiteSpace(prioBookingReference)
                        ? string.Empty
                        : prioBookingReference;

                    bookedProduct.OptionStatus = GetBookingStatusNumber(bookingReference, option?.AvailabilityStatus);
                    bookedProducts.Add(bookedProduct);
                }

                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreatePrioHubProductsBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, prioHubProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.PrioHub), prioHubProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        prioHubProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, prioHubProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }

        public List<BookedProduct> CreateNewCitySightSeeingBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var newCityselectedProducts = criteria.Booking.SelectedProducts.
                Where(x => x.APIType.Equals(APIType.NewCitySightSeeing)).ToList();

            var request = string.Empty;
            var response = string.Empty;

            try
            {
                var logPurchaseCriteria = new LogPurchaseXmlCriteria
                {
                    BookingId = criteria.Booking.BookingId,
                    APIType = APIType.NewCitySightSeeing,
                    BookingReferenceNumber = criteria.Booking.ReferenceNumber
                };

                if (newCityselectedProducts != null && newCityselectedProducts.Count > 0)
                {
                    foreach (var selectedProduct in newCityselectedProducts)
                    {
                        var newCitySightSelectedProduct = (NewCitySightSeeingSelectedProduct)selectedProduct;
                        var rowKey = $"{criteria.Booking.ReferenceNumber}-{selectedProduct.AvailabilityReferenceId}";
                        var ReservationDetails = _tableStorageOperation.RetrieveReservationData(rowKey);
                        var createOrderResponse = SerializeDeSerializeHelper.DeSerialize<ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Reservation.ReservationResponse>(ReservationDetails?.ReservationResponse);
                        var result = new Tuple<string, string>("", "");
                        if (ReservationDetails != null)
                        {
                            result = NewCitySightSeeingReservationGet(newCitySightSelectedProduct, createOrderResponse, criteria.Booking, criteria.Token);
                        }
                        //End- Get Reservation from Storage
                        if (ReservationDetails == null && (String.IsNullOrEmpty(Convert.ToString(newCitySightSelectedProduct.NewCitySightSeeingReservationId))))
                        {
                            var getSelectedProducts = _newCitySightSeeingAdapter.CreateReservationProduct
                            (selectedProduct, criteria.Booking?.Language?.Code, criteria.Booking?.VoucherEmailAddress,
                            criteria.Booking?.VoucherPhoneNumber, criteria.Booking?.ReferenceNumber, criteria.Booking?.User?.ZipCode,
                            criteria.Booking?.User?.Address1, criteria.Booking?.User?.City,
                            criteria.Token, out request, out response);
                        }

                        if (!string.IsNullOrEmpty(((NewCitySightSeeingSelectedProduct)selectedProduct)?.NewCitySightSeeingReservationId))
                        {
                            if (!string.IsNullOrEmpty(result?.Item1))
                            {
                                logPurchaseCriteria.RequestXml = request;
                                logPurchaseCriteria.ResponseXml = response;
                                logPurchaseCriteria.Status = Constant.StatusSuccess;
                                logPurchaseCriteria.Bookingtype = Constant.StatusReservation;
                                _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);
                            }
                            var bookedSelectedProducts = _newCitySightSeeingAdapter.CreateBookingSingle
                                (selectedProduct, criteria.Booking?.Language?.Code, criteria.Booking?.VoucherEmailAddress,
                            criteria.Booking?.VoucherPhoneNumber, criteria.Booking?.ReferenceNumber, criteria.Token, out request, out response);
                        }
                        else
                        {
                            logPurchaseCriteria.RequestXml = request;
                            logPurchaseCriteria.ResponseXml = response;
                            logPurchaseCriteria.Status = Constant.StatusFailed;
                            logPurchaseCriteria.Bookingtype = Constant.StatusReservation;
                            _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                            foreach (var newCityProduct in newCityselectedProducts)
                            {
                                var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                    x.AvailabilityReferenceId == newCityProduct.AvailabilityReferenceId);
                                if (bookedProduct == null) continue;
                                bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                                bookedProducts.Add(bookedProduct);
                            }

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber,
                                    selectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.NewCitySightSeeing), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name,
                                    selectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            return bookedProducts;
                        }


                        var bookingStatus = String.IsNullOrEmpty(newCitySightSelectedProduct.SupplierReferenceNumber) ? "CANCELLED" : "Confirmed";
                        if (bookingStatus == "Confirmed")
                        {
                            logPurchaseCriteria.RequestXml = request;
                            logPurchaseCriteria.ResponseXml = response;
                            logPurchaseCriteria.Bookingtype = Constant.StatusBooking;
                            logPurchaseCriteria.Status = bookingStatus == "Confirmed" ?
                            Constant.StatusSuccess : Constant.StatusFailed;
                            _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;
                            #region Update Product Option Status
                            selectedProduct.ProductOptions = UpdateOptionStatus(selectedProduct.ProductOptions, newCitySightSelectedProduct.NewCitySightSeeingOrderCode);
                            #endregion Update Product Option Status
                            bookedProduct = MapProductForNewCitySightSeeing(bookedProduct, newCitySightSelectedProduct);
                            bookedProducts.Add(bookedProduct);
                        }
                        else
                        {
                            logPurchaseCriteria.RequestXml = request;
                            logPurchaseCriteria.ResponseXml = response;
                            logPurchaseCriteria.Bookingtype = Constant.StatusBooking;
                            logPurchaseCriteria.Status = Constant.StatusFailed;
                            _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                            foreach (var newCityProduct in newCityselectedProducts)
                            {
                                var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                    x.AvailabilityReferenceId == newCityProduct.AvailabilityReferenceId);
                                if (bookedProduct == null) continue;
                                bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                                bookedProducts.Add(bookedProduct);
                            }
                            //Api booking failed
                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                    , System.Net.HttpStatusCode.BadGateway
                                    , response);

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber,
                                 selectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id,
                                 criteria.Token,
                                 bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                 Convert.ToInt32(APIType.NewCitySightSeeing), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                 selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name,
                                 selectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            return bookedProducts;

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                foreach (var selectedProduct in newCityselectedProducts)
                {
                    var option = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);

                    var bookingReference = ((NewCitySightSeeingSelectedProduct)selectedProduct).NewCitySightSeeingReservationId;
                    if (bookedProduct == null) continue;
                    bookedProduct.OptionStatus = GetBookingStatusNumber(bookingReference, option?.AvailabilityStatus);
                    bookedProducts.Add(bookedProduct);
                }

                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateNewCitySightSeeingBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError

                                , System.Net.HttpStatusCode.BadGateway

                                , $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, newCityselectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.NewCitySightSeeing), newCityselectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        newCityselectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, newCityselectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }



        public List<BookedProduct> GenerateSightSeeingQRCode(ActivityBookingCriteria criteria)
        {
            var sightseeingSelectedProducts = new List<SelectedProduct>();
            var citySightSelectedProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.Citysightseeing)).ToList();
            var bookedProducts = new List<BookedProduct>();
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                foreach (var product in citySightSelectedProducts)
                {
                    var selectedActivityOption = (ActivityOption)product.ProductOptions.Find(o => o.IsSelected);
                    List<CitySightseeingMapping> sightseeingMapping;
                    //Get master mapping data
                    try
                    {
                        sightseeingMapping = _supplierBookingPersistence.GetSightseeingMapping(selectedActivityOption.Id);
                    }
                    catch (Exception ex)
                    {
                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}");

                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, product?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                citySightSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.Citysightseeing), product?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                product?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, product?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }

                        throw;
                    }

                    CitySightseeingSelectedProduct sightseeingSelectedProduct;
                    CitySightseeingMapping mapping;

                    //Process Children
                    if (selectedActivityOption.TravelInfo.Ages != null)
                    {
                        var childCount = selectedActivityOption.TravelInfo.NoOfPassengers
                            ?.Where(x => x.Key == PassengerType.Child).Select(x => x.Value).FirstOrDefault();
                        var childQuantity = 0;
                        for (var i = 0; i < childCount; i++)
                        {
                            var age = selectedActivityOption.TravelInfo.Ages.FirstOrDefault(x => x.Key.Equals(PassengerType.Child));
                            sightseeingSelectedProduct = new CitySightseeingSelectedProduct();
                            mapping = sightseeingMapping.FirstOrDefault(item => item.ChildFromAge <= age.Value && item.ChildToAge >= age.Value);
                            if (string.IsNullOrWhiteSpace(mapping?.SupplierCode)) continue;
                            var index = sightseeingSelectedProducts.FindIndex(item => mapping != null && item.ActivityCode.Equals(mapping.SupplierCode));

                            sightseeingSelectedProduct.Id = product.Id;
                            sightseeingSelectedProduct.AvailabilityReferenceId = product.AvailabilityReferenceId;
                            sightseeingSelectedProduct.ActivityCode = mapping.SupplierCode;

                            if (index < 0)
                            {
                                childQuantity = 1;
                                sightseeingSelectedProduct.Quantity = childQuantity;
                                sightseeingSelectedProduct.ProductOptions = product.ProductOptions;
                                sightseeingSelectedProducts.Add(sightseeingSelectedProduct);
                            }
                            else
                            {
                                childQuantity++;
                                sightseeingSelectedProduct.ProductOptions = product.ProductOptions;
                                sightseeingSelectedProducts.Remove(sightseeingSelectedProducts.FirstOrDefault(x => x.ActivityCode.Equals(mapping.SupplierCode)));
                                sightseeingSelectedProduct.Quantity = childQuantity;
                                sightseeingSelectedProducts.Add(sightseeingSelectedProduct);
                            }
                        }
                    }

                    //Process Adults
                    mapping = sightseeingMapping.FirstOrDefault(item => item.PassengerType.Equals(PassengerType.Adult));
                    var data = sightseeingSelectedProducts.Where(item => item.ActivityCode.Equals(mapping?.SupplierCode)).ToList();

                    sightseeingSelectedProduct = new CitySightseeingSelectedProduct();

                    if (mapping != null)
                    {
                        //In case Child code is similar to Adult, needs to be processed as Adult
                        if (data.Any())
                        {
                            var adultCount = selectedActivityOption.TravelInfo?.NoOfPassengers?.FirstOrDefault(x => x.Key == PassengerType.Adult).Value ?? 0;
                            sightseeingSelectedProducts.RemoveAll(x => x.ActivityCode.Equals(mapping.SupplierCode));
                            sightseeingSelectedProduct.Quantity = adultCount + data.Sum(x => ((CitySightseeingSelectedProduct)x).Quantity);
                        }
                        else
                        {
                            // ReSharper disable once AssignNullToNotNullAttribute
                            sightseeingSelectedProduct.Quantity = (int)selectedActivityOption?.TravelInfo?.NoOfPassengers.FirstOrDefault(x => x.Key == PassengerType.Adult).Value;
                        }

                        sightseeingSelectedProduct.Id = product.Id;
                        sightseeingSelectedProduct.AvailabilityReferenceId = product.AvailabilityReferenceId;
                        sightseeingSelectedProduct.ActivityCode = mapping.SupplierCode;
                        sightseeingSelectedProduct.ProductOptions = product.ProductOptions;
                        sightseeingSelectedProducts.Add(sightseeingSelectedProduct);
                    }
                }

                //API hits
                if (sightseeingSelectedProducts.Count > 0)
                {
                    var sightseeingProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.Citysightseeing)).ToList();
                    var issueData = _sightSeeingAdapter.IssueTicket(sightseeingSelectedProducts, criteria.Booking.ReferenceNumber, criteria.Token, out request, out response);

                    var logPurchaseCriteria = new LogPurchaseXmlCriteria
                    {
                        RequestXml = request,
                        ResponseXml = response,
                        BookingId = criteria.Booking.BookingId,
                        Bookingtype = Constant.StatusReservation,
                        Status = issueData?.Count > 0 ? Constant.StatusSuccess : Constant.StatusFailed,
                        APIType = APIType.Citysightseeing,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber
                    };

                    _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                    var selectedProductList = new List<SelectedProduct>();
                    if (issueData?.Count > 0)
                    {
                        foreach (var selectedProduct in sightseeingProducts.OfType<CitySightseeingSelectedProduct>())
                        {
                            var data = issueData.FirstOrDefault(x => x.Id.Equals(selectedProduct.Id));
                            if (data == null) continue;
                            selectedProduct.QrCode = data.QrCode;
                            selectedProduct.Pnr = data.Pnr;
                            selectedProduct.ProductOptions = UpdateOptionStatus(selectedProduct.ProductOptions, data.Pnr);
                            selectedProductList.Add(selectedProduct);
                        }

                        sightseeingProducts = selectedProductList;
                        var isConfirmSuccessful = _sightSeeingAdapter.ConfirmTicket(sightseeingProducts, criteria.Token, out request, out response);

                        logPurchaseCriteria.RequestXml = request;
                        logPurchaseCriteria.ResponseXml = response;
                        logPurchaseCriteria.Bookingtype = Constant.StatusConfirmTicket;
                        logPurchaseCriteria.Status =
                            isConfirmSuccessful ? Constant.StatusSuccess : Constant.StatusFailed;

                        _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                        foreach (var sightseeingSelectedProduct in sightseeingProducts)
                        {
                            var product = (CitySightseeingSelectedProduct)sightseeingSelectedProduct;
                            if (!isConfirmSuccessful) product.Pnr = "";

                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == sightseeingSelectedProduct.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;

                            bookedProduct = MapProductForCitySightSeeing(bookedProduct, product);
                            bookedProducts.Add(bookedProduct);
                        }

                        return bookedProducts;
                    }
                    else
                    {
                        foreach (var sightseeingSelectedProduct in sightseeingSelectedProducts)
                        {
                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == sightseeingSelectedProduct.AvailabilityReferenceId);

                            if (bookedProduct == null) continue;
                            bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                            bookedProducts.Add(bookedProduct);
                        }

                        //Api booking failed
                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);

                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, sightseeingSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.Citysightseeing), sightseeingSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                sightseeingSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, sightseeingSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }

                        return bookedProducts;
                    }
                }

                //update status to failed if any product is not in bookedProducts list but in citySightSelectedProducts(input for booking)
                if (citySightSelectedProducts.Count != bookedProducts.Count)
                {
                    foreach (var csp in citySightSelectedProducts)
                    {
                        var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                            x.AvailabilityReferenceId == csp.AvailabilityReferenceId);

                        if (bookedProduct == null) continue;
                        bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();

                        if (bookedProducts?
                                .Any(x => x.AvailabilityReferenceId == bookedProduct.AvailabilityReferenceId)
                                == false
                        )
                        {
                            bookedProducts.Add(bookedProduct);
                        }
                    }
                }
                return bookedProducts;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "GenerateSightSeeingQRCode",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
                foreach (var sightseeingSelectedProduct in sightseeingSelectedProducts)
                {
                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                        x.AvailabilityReferenceId == sightseeingSelectedProduct.AvailabilityReferenceId);
                    bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                    bookedProducts.Add(bookedProduct);
                }

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, sightseeingSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.Citysightseeing), sightseeingSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        sightseeingSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, sightseeingSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                return bookedProducts;
            }
        }

        public Dictionary<string, bool> GrayLineIceLandDeleteBooking(List<SelectedProduct> gliSelectedProducts, string supplierReferenceNumber, string bookingReference, string token)
        {
            var status = new Dictionary<string, bool>();
            if (string.IsNullOrEmpty(supplierReferenceNumber))
                return status;
            gliSelectedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });
            try
            {
                var isCancelled = _grayLineIceLandAdapter.DeleteBooking(supplierReferenceNumber, token, out string requestXml, out var responseXml);
                foreach (var gliSelectedProduct in gliSelectedProducts)
                {
                    status[gliSelectedProduct.AvailabilityReferenceId] = isCancelled;
                }

                var logCriteria = new LogPurchaseXmlCriteria
                {
                    RequestXml = requestXml,
                    ResponseXml = responseXml,
                    Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed,
                    BookingId = 0,
                    APIType = APIType.Graylineiceland,
                    BookingReferenceNumber = bookingReference,
                    Bookingtype = "Cancel Booking"
                };

                _supplierBookingPersistence.LogPurchaseXML(logCriteria);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "GrayLineIceLandDeleteBooking",
                    Token = token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(gliSelectedProducts)}{supplierReferenceNumber}{token}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return status;
        }

        public Dictionary<string, bool> PrioCancelReservationAndBooking(List<PrioSelectedProduct> prioSelectedProducts, string bookingReferenceNumber, string token)
        {
            var status = new Dictionary<string, bool>();
            var request = String.Empty;
            var response = String.Empty;
            prioSelectedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });

            var isCancelled = false;

            var logCriteria = new LogPurchaseXmlCriteria
            {
                BookingId = 0,
                APIType = APIType.Prio,
                BookingReferenceNumber = bookingReferenceNumber
            };

            foreach (var prioSelectedProduct in prioSelectedProducts)
            {
                try
                {
                    //Case1: If Product is Reserved and not Booked.
                    //Case2: If Product is Reserved and Booked.
                    //Cancel Reservation Only in case of Ticket Class 2 and Ticket Class 3
                    var errorCode = prioSelectedProduct.PrioApiConfirmedBooking.ErrorCode;
                    var prioApiConfirmedBooking = prioSelectedProduct.PrioApiConfirmedBooking.BookingStatus;

                    if (prioSelectedProduct.PrioTicketClass == (int)TicketClass.TicketClassTwo ||
                        prioSelectedProduct.PrioTicketClass == (int)TicketClass.TicketClassThree)
                    {
                        //Cancel Reservation
                        //Case1: If Product is Reserved and not Booked.
                        if (string.Equals(prioSelectedProduct.PrioBookingStatus, PrioApiStatus.Reserved,
                                StringComparison.CurrentCultureIgnoreCase) &&
                            prioSelectedProduct.PrioApiConfirmedBooking == null)
                        {
                            var cancellationStatus =
                                _prioTicketAdapter.CancelReservation(prioSelectedProduct, bookingReferenceNumber,
                                    token, out request, out response);
                            if (cancellationStatus.Item3 == BookingStatus.Cancelled.ToString())
                            {
                                logCriteria.Bookingtype = "Cancel Reservation";
                                isCancelled = true;
                            }
                        }
                        //Case2: If Product is Reserved and Booked.
                        else if (string.Equals(prioSelectedProduct.PrioBookingStatus, PrioApiStatus.Reserved,
                                     StringComparison.CurrentCultureIgnoreCase)
                                     && prioSelectedProduct.PrioApiConfirmedBooking != null
                                     && string.Equals(prioApiConfirmedBooking, PrioApiStatus.Confirmed,
                                     StringComparison.CurrentCultureIgnoreCase)
                                     && string.IsNullOrEmpty(errorCode)
                                )
                        {
                            //Cancel Booking
                            var cancellationStatus = _prioTicketAdapter.CancelBooking(prioSelectedProduct, token, out request, out response).Item3;
                            if (cancellationStatus == BookingStatus.Cancelled.ToString())
                            {
                                logCriteria.Bookingtype = "Cancel Booking";
                                isCancelled = true;
                            }
                        }
                    }
                    //Cancel Ticket Class 1 Case
                    else
                    {
                        if (prioSelectedProduct.PrioApiConfirmedBooking != null
                            && string.IsNullOrEmpty(errorCode)
                            && string.Equals(prioSelectedProduct.PrioApiConfirmedBooking.BookingStatus,
                                PrioApiStatus.Confirmed,
                                StringComparison.CurrentCultureIgnoreCase)
                        )
                        {
                            //Cancel Booking
                            var cancellationStatus = _prioTicketAdapter.CancelBooking(prioSelectedProduct, token, out request, out response)?.Item3;
                            if (cancellationStatus != null && cancellationStatus == BookingStatus.Cancelled.ToString())
                            {
                                logCriteria.Bookingtype = "Cancel Booking";
                                isCancelled = true;
                            }
                        }
                    }

                    logCriteria.RequestXml = request;
                    logCriteria.ResponseXml = response;
                    logCriteria.Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed;

                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                    status[prioSelectedProduct.AvailabilityReferenceId] = isCancelled;
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "SupplierBookingService",
                        MethodName = "PrioCancelReservationAndBooking",
                        Token = token,
                        Params = $"{SerializeDeSerializeHelper.Serialize(prioSelectedProducts)}{bookingReferenceNumber}{token}"
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            }

            return status;
        }

        private Tuple<string, string> PrioReservationWithLogging(PrioSelectedProduct selectedProduct, string distributorReference, string token, Booking booking)
        {
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (selectedProduct.PrioTicketClass == (int)TicketClass.TicketClassTwo ||
                    selectedProduct.PrioTicketClass == (int)TicketClass.TicketClassThree)
                {
                    var getReservationResponse = _prioTicketAdapter.CreateReservation(selectedProduct, distributorReference, out request, out response, token);
                    if (getReservationResponse != null)
                    {
                        selectedProduct.PrioReservationReference = getReservationResponse.Item1;
                        selectedProduct.PrioDistributorReference = getReservationResponse.Item2;
                        selectedProduct.PrioBookingStatus = getReservationResponse.Item3;
                    }
                    else
                    {
                        //Api booking failed
                        booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);

                        try
                        {
                            LogBookingFailureInDB(booking, booking?.ReferenceNumber, selectedProduct?.Id ?? booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                                selectedProduct?.PrioReservationReference ?? selectedProduct?.PrioDistributorReference, booking?.User.EmailAddress, booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.Prio), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProduct?.AvailabilityReferenceId ?? booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            booking?.UpdateDBLogFlag();
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }


                    }
                }

                return new Tuple<string, string>(request, response);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "PrioReservationWithLogging",
                    Token = token,
                    Params = $"{selectedProduct}"
                };
                _log.Error(isangoErrorEntity, ex);

                booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(booking, booking?.ReferenceNumber, selectedProduct?.Id ?? booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                        selectedProduct?.PrioReservationReference ?? selectedProduct?.PrioDistributorReference, booking?.User.EmailAddress, booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.Prio), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProduct?.AvailabilityReferenceId ?? booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                throw;
            }
        }
        private Tuple<string, string> PrioHubReservationWithLogging(PrioHubSelectedProduct selectedProduct, string isangoReference, string token, Booking booking)
        {
            var request = string.Empty;
            var response = string.Empty;
            try
            {

                var getReservationResponse =
                _prioHubAdapter.CreateReservation(selectedProduct, isangoReference, out request,
                    out response, token, booking?.ReferenceNumber, selectedProduct?.PrioHubAvailabilityId, selectedProduct?.PrioHubProductPaxMapping);
                if (getReservationResponse != null && getReservationResponse.Item1 != null)
                {
                    selectedProduct.PrioReservationReference = getReservationResponse.Item1;
                    selectedProduct.PrioDistributorReference = getReservationResponse.Item2;
                    selectedProduct.PrioHubReservationStatus = getReservationResponse.Item3;
                    selectedProduct.PrioHubDistributerId = getReservationResponse.Item4;
                }
                else
                {
                    //Api booking failed
                    booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, selectedProduct?.Id ?? booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                            selectedProduct?.PrioReservationReference ?? selectedProduct?.PrioDistributorReference, booking?.User.EmailAddress, booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.PrioHub), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProduct?.AvailabilityReferenceId ?? booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                        booking?.UpdateDBLogFlag();
                    }
                    catch (Exception e)
                    {
                        //ignore
                    }
                }


                return new Tuple<string, string>(request, response);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "PrioHubReservationWithLogging",
                    Token = token,
                    Params = $"{selectedProduct}"
                };
                _log.Error(isangoErrorEntity, ex);

                booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                throw;
            }
        }

        private ReservationData.ReservationResponse PrioHubReservationOnly(PrioHubSelectedProduct selectedProduct,
            string isangoReference, string token, Booking booking, out string request,
                   out string response)
        {

            request = string.Empty;
            response = string.Empty;
            try
            {
                var getReservationResponse =
               _prioHubAdapter.CreateReservationOnly(selectedProduct, isangoReference, out request,
                   out response, token, booking?.ReferenceNumber, selectedProduct?.PrioHubAvailabilityId, selectedProduct?.PrioHubProductPaxMapping);
                return getReservationResponse;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "PrioHubReservationOnly",
                    Token = token,
                    Params = $"{selectedProduct}"
                };
                _log.Error(isangoErrorEntity, ex);

                booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                throw;
            }
            return null;
        }

        private NewBookingResponse TourCMSReservationOnly(SelectedProduct selectedProduct,
             string languageCode, string voucherEmailAddress, string voucherPhoneNumber,
             string referenceNumber,
             string token, Booking booking, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            try
            {
                var getReservationResponse = _tourCMSAdapter.CreateReservationOnly(
                selectedProduct, languageCode,
                voucherEmailAddress, voucherPhoneNumber,
                referenceNumber, token,
                out request, out response);

                return getReservationResponse;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "TourCMSReservationOnly",
                    Token = token,
                    Params = $"{selectedProduct}"
                };
                _log.Error(isangoErrorEntity, ex);

                booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                throw;
            }
            return null;
        }

        private Tuple<string, string> PrioHubReservationGet(
            PrioHubSelectedProduct selectedProduct,
            ReservationData.ReservationResponse reservationRs, Booking booking,
             string token)
        {
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (reservationRs != null)
                {
                    selectedProduct.PrioReservationReference = reservationRs?.Data.Reservation.ReservationReference;
                    selectedProduct.PrioDistributorReference = reservationRs?.Data?.Reservation?.ReservationDetails?.FirstOrDefault()?.BookingReservationReference;
                    selectedProduct.PrioHubReservationStatus = reservationRs?.Data?.Reservation?.ReservationDetails?.FirstOrDefault()?.BookingStatus;
                    selectedProduct.PrioHubDistributerId = reservationRs?.Data?.Reservation?.ReservationDistributorId;
                }
                else
                {
                    //Api booking failed
                    booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, selectedProduct?.Id ?? booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                            selectedProduct?.PrioReservationReference ?? selectedProduct?.PrioDistributorReference, booking?.User.EmailAddress, booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.PrioHub), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProduct?.AvailabilityReferenceId ?? booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                        booking?.UpdateDBLogFlag();
                    }
                    catch (Exception e)
                    {
                        //ignore
                    }
                }

                return new Tuple<string, string>(request, response);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "PrioHubReservationGet",
                    Token = token,
                    Params = $"{selectedProduct}"
                };
                _log.Error(isangoErrorEntity, ex);

                booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                throw;
            }
        }

        private Tuple<string, string> MoulinRougeReservationGet(
          MoulinRougeSelectedProduct selectedProduct,
          Response reservationRs, Booking booking,
           string token)
        {
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (reservationRs != null)
                {

                    var resp = reservationRs.Body.ACP_AllocSeatsAutomaticRequestResponse;
                    if (!resp.ACP_AllocSeatsAutomaticRequestResult || resp.result != 0) return null;

                    var totalPrice = resp.listAllocResponse.ACPO_AllocSAResponse.ListAllocSeat
                        .Where(item => item.Seat_Detail.ID_Seat > 0 && item.Seat_Detail.ID_PhysicalSeat > 0)
                        .Select(item => item.AmountDetail[8]).FirstOrDefault();
                    var seatIds = resp.listAllocResponse.ACPO_AllocSAResponse.ListAllocSeat.Where(item => item.Seat_Detail.ID_Seat > 0 && item.Seat_Detail.ID_PhysicalSeat > 0).Select(item => item.Seat_Detail.ID_Seat).Distinct().ToList();
                    selectedProduct.Amount = totalPrice;
                    selectedProduct.Ids = seatIds;
                    selectedProduct.Price = totalPrice;
                    selectedProduct.TemporaryOrderRowId = resp.listAllocResponse.ACPO_AllocSAResponse.ID_TemporaryOrderRow;
                    selectedProduct.TemporaryOrderId = resp.id_TemporaryOrder;
                    selectedProduct.CartReferenceId = resp.id_TemporaryOrder;


                    selectedProduct.CategoryId = 1;
                    selectedProduct.CatalogDateId = selectedProduct.CatalogDateId;
                    selectedProduct.CategoryId = selectedProduct.CategoryId;
                    selectedProduct.ContingentId = selectedProduct.ContingentId;
                    selectedProduct.BlocId = selectedProduct.BlocId;
                    selectedProduct.FloorId = selectedProduct.FloorId;
                    selectedProduct.RateId = selectedProduct.RateId;
                    selectedProduct.Quantity = selectedProduct.Quantity;
                }
                else
                {
                    //Api booking failed
                    booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, selectedProduct?.Id ?? booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                            selectedProduct?.TemporaryOrderId ?? selectedProduct?.TemporaryOrderId, booking?.User.EmailAddress, booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.Moulinrouge), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProduct?.AvailabilityReferenceId ?? booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                        booking?.UpdateDBLogFlag();
                    }
                    catch (Exception e)
                    {
                        //ignore
                    }
                }

                return new Tuple<string, string>(request, response);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "MoulinRougeReservationGet",
                    Token = token,
                    Params = $"{selectedProduct}"
                };
                _log.Error(isangoErrorEntity, ex);

                booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                throw;
            }
        }

        private Tuple<string, string> RedeamReservationGet(
            RedeamSelectedProduct selectedProduct,
            CreateHoldResponse createHoldResponse, Booking booking,
             string token)
        {
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (createHoldResponse != null)
                {
                    selectedProduct.HoldId = createHoldResponse.Hold.Id.ToString();
                    selectedProduct.HoldStatus = createHoldResponse.Hold.Status;
                }
                else
                {
                    //Api booking failed
                    booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, selectedProduct?.Id ?? booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                             selectedProduct.HoldId, booking?.User.EmailAddress, booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.Redeam), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProduct?.AvailabilityReferenceId ?? booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                        booking?.UpdateDBLogFlag();
                    }
                    catch (Exception e)
                    {
                        //ignore
                    }
                }

                return new Tuple<string, string>(request, response);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "RedeamReservationGet",
                    Token = token,
                    Params = $"{selectedProduct}"
                };
                _log.Error(isangoErrorEntity, ex);

                booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                throw;
            }
        }

        private Tuple<string, string> VentrataReservationGet(
            VentrataSelectedProduct selectedProduct,
            BookingReservationRes reservationRs, Booking booking,
             string token)
        {
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (reservationRs != null)
                {
                    var castedReservationResObject = reservationRs;
                    selectedProduct.Uuid = castedReservationResObject.uuid;
                    selectedProduct.SupplierReference = castedReservationResObject.supplierReference;
                    selectedProduct.TestMode = castedReservationResObject.testMode;
                    selectedProduct.BookingStatus = castedReservationResObject.status;
                    selectedProduct.IsCancellable = castedReservationResObject.cancellable;

                }
                else
                {
                    //Api booking failed
                    booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, selectedProduct?.Id ?? booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                            selectedProduct?.SupplierReference ?? selectedProduct?.SupplierReference, booking?.User.EmailAddress, booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.Ventrata), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name,
                            selectedProduct?.AvailabilityReferenceId ?? booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId,
                            CommonErrorCodes.SupplierBookingError.ToString());
                        booking?.UpdateDBLogFlag();
                    }
                    catch (Exception e)
                    {
                        //ignore
                    }
                }

                return new Tuple<string, string>(request, response);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "VentrataReservationGet",
                    Token = token,
                    Params = $"{selectedProduct}"
                };
                _log.Error(isangoErrorEntity, ex);

                booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                throw;
            }
        }

        private Tuple<string, string> TourCMSReservationGet(
         TourCMSSelectedProduct selectedProduct,
        NewBookingResponse reservationRs, Booking booking,
          string token)
        {
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (reservationRs != null)
                {
                    selectedProduct.BookingId = Convert.ToInt32(reservationRs.Booking.BookingId);

                }
                else
                {
                    //Api booking failed
                    booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, selectedProduct?.Id ?? booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                           Convert.ToString(selectedProduct?.BookingId), booking?.User.EmailAddress, booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.TourCMS), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name,
                            selectedProduct?.AvailabilityReferenceId ?? booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId,
                            CommonErrorCodes.SupplierBookingError.ToString());
                        booking?.UpdateDBLogFlag();
                    }
                    catch (Exception e)
                    {
                        //ignore
                    }
                }

                return new Tuple<string, string>(request, response);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "PrioHubReservationGet",
                    Token = token,
                    Params = $"{selectedProduct}"
                };
                _log.Error(isangoErrorEntity, ex);

                booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                throw;
            }
        }


        private Tuple<string, string> NewCitySightSeeingReservationGet(
        NewCitySightSeeingSelectedProduct selectedProduct,
        ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Reservation.ReservationResponse
            reservationRs, Booking booking,
         string token)
        {
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (reservationRs != null)
                {
                    selectedProduct.NewCitySightSeeingReservationId = reservationRs.ReservationId;
                }
                else
                {
                    //Api booking failed
                    booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, selectedProduct?.Id ?? booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                           Convert.ToString(selectedProduct?.NewCitySightSeeingReservationId), booking?.User.EmailAddress, booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.NewCitySightSeeing), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name,
                            selectedProduct?.AvailabilityReferenceId ?? booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId,
                            CommonErrorCodes.SupplierBookingError.ToString());
                        booking?.UpdateDBLogFlag();
                    }
                    catch (Exception e)
                    {
                        //ignore
                    }
                }

                return new Tuple<string, string>(request, response);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "NewCitySightSeeingReservationGet",
                    Token = token,
                    Params = $"{selectedProduct}"
                };
                _log.Error(isangoErrorEntity, ex);

                booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                throw;
            }
        }
        public Dictionary<string, bool> TicketAdapterPurchaseCancel(List<HotelBedsSelectedProduct> hotelBedsSelectedProducts, string authentication, string bookingReferenceNumber, string token)
        {
            var status = new Dictionary<string, bool>();

            hotelBedsSelectedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });
            try
            {
                if (hotelBedsSelectedProducts.Count > 0 && !string.IsNullOrWhiteSpace(hotelBedsSelectedProducts?.FirstOrDefault()?.FileNumber))
                {
                    var isCancelledData = _hbAdapter.BookingCancel(hotelBedsSelectedProducts?.FirstOrDefault().FileNumber, authentication, token, out string requestXml, out string responseXml);
                    var isCancelled = isCancelledData?.Booking?.Status?.ToUpperInvariant() == Constant.Cancelled ? true : false;
                    foreach (var hotelBedsSelectedProduct in hotelBedsSelectedProducts)
                    {
                        status[hotelBedsSelectedProduct.AvailabilityReferenceId] = isCancelled;
                    }

                    var logCriteria = new LogPurchaseXmlCriteria
                    {
                        RequestXml = requestXml,
                        ResponseXml = responseXml,
                        Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed,
                        BookingId = 0,
                        APIType = APIType.Hotelbeds,
                        BookingReferenceNumber = bookingReferenceNumber,
                        Bookingtype = "Cancel Booking"
                    };

                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "TicketAdapterPurchaseCancel",
                    Token = token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(hotelBedsSelectedProducts)}{authentication}{token}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return status;
        }

        public List<BookedProduct> CreateFareHarborBooking(ActivityBookingCriteria criteria)
        {
            var bookingId = criteria.Booking.BookingId;
            var fareHarborSelectedProducts = new List<FareHarborSelectedProduct>();
            var bookedProducts = new List<BookedProduct>();
            var request = string.Empty;
            var response = string.Empty;

            try
            {
                var selectedProducts = criteria.Booking.SelectedProducts.Where(x => x.APIType.Equals(APIType.Fareharbor)).ToList();
                foreach (var selectedProduct in selectedProducts)
                {
                    var fareHarborSelectedProduct = selectedProduct as FareHarborSelectedProduct;
                    if (fareHarborSelectedProduct?.ProductOptions == null || !fareHarborSelectedProduct.ProductOptions.Any()) continue;
                    fareHarborSelectedProducts.Add(fareHarborSelectedProduct);
                }

                if (fareHarborSelectedProducts.Count > 0)
                {
                    var fhBookedProducts = new List<FareHarborSelectedProduct>();

                    foreach (var product in fareHarborSelectedProducts)
                    {
                        var bookedProduct = _fareHarborAdapter.CreateBooking(product, criteria.Booking.ReferenceNumber, criteria.Token, out request, out response);

                        if (bookedProduct != null && !string.IsNullOrWhiteSpace(bookedProduct.UuId))
                        {
                            fhBookedProducts.Add(bookedProduct);
                        }
                        else
                        {
                            //Api booking failed
                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                    , System.Net.HttpStatusCode.BadGateway
                                    , response);

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, fareHarborSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    bookedProduct?.UuId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.Fareharbor), fareHarborSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    fareHarborSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, fareHarborSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            //break;
                        }
                        var logPurchase = new LogPurchaseXmlCriteria
                        {
                            RequestXml = request,
                            ResponseXml = response,
                            BookingId = bookingId,
                            Status = (product != null) ? "Success" : "Failed",
                            APIType = APIType.Fareharbor,
                            BookingReferenceNumber = criteria.Booking.ReferenceNumber
                        };
                        _supplierBookingPersistence.LogPurchaseXML(logPurchase);
                    }

                    //var fhBookedProducts = _fareHarborAdapter.CreateBooking(fareHarborSelectedProducts, criteria.Booking.ReferenceNumber, criteria.Token, out request, out response);

                    if (fareHarborSelectedProducts?.Count > 0)
                    {
                        foreach (var product in fareHarborSelectedProducts)
                        {
                            product.Code = product.ActivityCode;
                            var bookedFareHarborProduct = fhBookedProducts?.Find(x => x.ActivityCode.Equals(product.Code));
                            if (bookedFareHarborProduct != null)
                            {
                                product.UuId = bookedFareHarborProduct.UuId;
                                product.ProductOptions = UpdateOptionStatus(product.ProductOptions, product.UuId);
                                product.SupplierCancellationPolicy = bookedFareHarborProduct.SupplierCancellationPolicy;
                                product.HotelPickUpLocation = bookedFareHarborProduct.HotelPickUpLocation;

                                var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                    x.AvailabilityReferenceId == product.AvailabilityReferenceId);
                                if (bookedProduct == null) continue;
                                bookedProduct = MapProductForFareharbor(bookedProduct, product);
                                bookedProducts.Add(bookedProduct);
                            }
                            else
                            {
                                var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                    x.AvailabilityReferenceId == product.AvailabilityReferenceId);
                                if (bookedProduct == null) continue;
                                bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                                bookedProducts.Add(bookedProduct);
                            }
                        }
                    }
                }

                return bookedProducts;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "TicketAdapterPurchaseCancel",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
                criteria?.Booking?.UpdateErrors(CommonErrorCodes.BookingError, System.Net.HttpStatusCode.BadGateway, $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, fareHarborSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        fareHarborSelectedProducts?.FirstOrDefault()?.UuId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.Fareharbor), fareHarborSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        fareHarborSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, fareHarborSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                return bookedProducts;
            }
        }

        public List<BookedProduct> CreateBokunBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var bokunProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.Bokun)).ToList();
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                foreach (var bokunSelectedProduct in bokunProducts.OfType<BokunSelectedProduct>())
                {
                    try
                    {
                        var selectedOption =
                            (ActivityOption)bokunSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                        bokunSelectedProduct.BookingReferenceNumber = criteria.Booking.ReferenceNumber;
                        bokunSelectedProduct.FactsheetId = Convert.ToInt32(selectedOption?.SupplierOptionCode);
                        //bokunSelectedProduct.RateId = Convert.ToInt32(bokunSelectedProduct?.RateId);
                        //bokunSelectedProduct.StartTimeId = Convert.ToInt32(bokunSelectedProduct?.StartTimeId);

                        bokunSelectedProduct.DateStart = selectedOption.TravelInfo.StartDate;

                        var result = _bokunAdapter.SubmitCheckout(bokunSelectedProduct, criteria.Token, out request,
                            out response);
                        var data = result?.SelectedProducts.OfType<BokunSelectedProduct>().FirstOrDefault(x =>
                            x.FactsheetId == Convert.ToInt32(selectedOption?.SupplierOptionCode));
                        if (data != null)
                        {
                            bokunSelectedProduct.QrCode = data.QrCode;
                            bokunSelectedProduct.ConfirmationCode = data.ConfirmationCode;
                            bokunSelectedProduct.ProductConfirmationCode = data.ProductConfirmationCode;
                            bokunSelectedProduct.BookingReferenceNumber = result.ReferenceNumber;
                            try
                            {
                                bokunSelectedProduct.Barcodes = data.Barcodes;
                            }
                            catch (Exception ex)
                            {
                                //ignore
                            }
                            selectedOption.ApiCancellationPolicy = bokunSelectedProduct.SupplierCancellationPolicy = data.SupplierCancellationPolicy;

                            //var bookedOptionNameFromAPI = data?.Name;
                            //if (!string.IsNullOrWhiteSpace(bookedOptionNameFromAPI))
                            //    bokunSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected).Name = bookedOptionNameFromAPI;
                        }
                        if (data == null || string.IsNullOrWhiteSpace(result?.ReferenceNumber))
                        {
                            //Api booking failed
                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                    , System.Net.HttpStatusCode.BadGateway
                                    , response);

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, bokunProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    result?.ReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.Bokun), bokunProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    bokunProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, bookedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                        }

                        bokunSelectedProduct.ProductOptions =
                            UpdateOptionStatus(bokunSelectedProduct.ProductOptions, bokunSelectedProduct.ConfirmationCode);
                        var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                            x.AvailabilityReferenceId == bokunSelectedProduct.AvailabilityReferenceId);
                        if (bookedProduct == null) continue;
                        bookedProduct = MapProductForBokun(bookedProduct, bokunSelectedProduct);
                        bookedProducts.Add(bookedProduct);

                        var logPurchase = new LogPurchaseXmlCriteria
                        {
                            RequestXml = request,
                            ResponseXml = response,
                            BookingId = criteria.Booking.BookingId,
                            Status = (result != null) ? "Success" : "Failed",
                            APIType = APIType.Bokun,
                            BookingReferenceNumber = criteria.Booking.ReferenceNumber
                        };
                        _supplierBookingPersistence.LogPurchaseXML(logPurchase);
                    }
                    catch (Exception ex)
                    {
                        bokunSelectedProduct.ProductOptions = UpdateOptionStatus(bokunSelectedProduct.ProductOptions, bokunSelectedProduct.ConfirmationCode);
                        var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                            x.AvailabilityReferenceId == bokunSelectedProduct.AvailabilityReferenceId);
                        if (bookedProduct == null) continue;

                        var option = bokunSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                        bookedProduct.OptionStatus = GetBookingStatusNumber(bokunSelectedProduct.ConfirmationCode, option.AvailabilityStatus);
                        if (!string.IsNullOrWhiteSpace(bokunSelectedProduct.ConfirmationCode))
                        {
                            if (bookedProduct.APIExtraDetail == null)
                                bookedProduct.APIExtraDetail = new ApiExtraDetail();
                            bookedProduct.APIExtraDetail.SupplieReferenceNumber = bokunSelectedProduct.ConfirmationCode;
                        }

                        bookedProducts.Add(bookedProduct);
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "SupplierBookingService",
                            MethodName = "CreateBokunBooking-SubmitCheckout",
                            Token = criteria.Token,
                            Params = $"{bokunSelectedProduct.Id}|{SerializeDeSerializeHelper.Serialize(bokunSelectedProduct)}"
                        };
                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.BookingError, System.Net.HttpStatusCode.BadGateway, $"Exception\n {ex.Message}\n{response}");

                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, bookedProducts?.FirstOrDefault()?.ServiceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                bookedProduct?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.Bokun), bookedProducts?.FirstOrDefault()?.OptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                bookedProducts?.FirstOrDefault()?.OptionName ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, bookedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }

                        _log.Error(isangoErrorEntity, ex);
                        continue;
                    }
                }
                return bookedProducts;
            }
            catch (Exception ex)
            {
                var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
                if (isangoBookedProducts == null) return bookedProducts;

                foreach (var bokunSelectedProduct in bokunProducts)
                {
                    var bookedProduct = isangoBookedProducts.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(bokunSelectedProduct.AvailabilityReferenceId));
                    if (bookedProduct == null) continue;
                    bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                    bookedProducts.Add(bookedProduct);
                }
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateBokunBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                criteria?.Booking?.UpdateErrors(CommonErrorCodes.BookingError, System.Net.HttpStatusCode.BadGateway, $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, bookedProducts?.FirstOrDefault()?.ServiceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.Bokun), bookedProducts?.FirstOrDefault()?.OptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        bookedProducts?.FirstOrDefault()?.OptionName ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, bookedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ex);
                return bookedProducts;
            }
        }

        public List<BookedProduct> CreateAotBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var aotSelectedProducts = criteria?.Booking?.SelectedProducts?.Where(product => product.APIType.Equals(APIType.Aot))?.ToList();
            var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
            string request = string.Empty;
            string response = string.Empty;
            try
            {
                if (aotSelectedProducts != null && aotSelectedProducts.Count > 0)
                {
                    var australiaCountryId = Convert.ToInt32(Constant.CountryIdAustralia);
                    var newZelandCountryId = Convert.ToInt32(Constant.CountryIdNewZealand);
                    var fijiCountryId = Convert.ToInt32(Constant.CountryIdFiji);

                    var australiaProducts = aotSelectedProducts.Where(e =>
                        e.Regions.Where(d => d.Type == RegionType.Country).Select(x => x.Id).Contains(australiaCountryId)).ToList();
                    var fijiProducts = aotSelectedProducts.Where(e =>
                        e.Regions.Where(d => d.Type == RegionType.Country).Select(x => x.Id).Contains(fijiCountryId)).ToList();
                    var newZealandProducts = aotSelectedProducts.Where(e =>
                        e.Regions.Where(d => d.Type == RegionType.Country).Select(x => x.Id).Contains(newZelandCountryId)).ToList();

                    if (australiaProducts?.Count > 0)
                    {
                        var bookingReferenceNumber = string.Empty;
                        try
                        {
                            _aotAdapter.SetAgentIdPassword(CountryType.Australia);
                            var aotResponse = _aotAdapter.CreateBooking(australiaProducts, criteria.Token, criteria.Booking.ReferenceNumber,
                                out request, out response);

                            var selectedProducts = criteria.SelectedProducts;
                            var bookingResponse = aotResponse as Booking;
                            if (!string.IsNullOrWhiteSpace(bookingResponse?.ReferenceNumber))
                            {
                                bookingReferenceNumber = bookingResponse.ReferenceNumber;
                                selectedProducts = bookingResponse.SelectedProducts;
                            }
                            else
                            {
                                //Api booking failed
                                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                        , System.Net.HttpStatusCode.BadGateway
                                        , response);

                                try
                                {
                                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, aotSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                        bookingResponse?.ReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                        Convert.ToInt32(APIType.Aot), aotSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                        aotSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, aotSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                    criteria?.Booking?.UpdateDBLogFlag();
                                }
                                catch (Exception e)
                                {
                                    //ignore
                                }
                            }

                            bookedProducts = CreateAotBookedProducts(selectedProducts, isangoBookedProducts, bookingReferenceNumber,
                                CountryType.Australia);
                            LogPurchaseXmlForAot(criteria.Booking, request, response, bookingReferenceNumber);
                        }
                        catch (Exception ex)
                        {
                            bookedProducts = UpdateOptionStatusForAotProduct(australiaProducts, isangoBookedProducts, bookingReferenceNumber);
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "SupplierBookingService",
                                MethodName = "CreateAotBooking",
                                Token = criteria.Token,
                                Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                            };

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, aotSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    bookingReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.Aot), aotSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    aotSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, aotSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                    if (fijiProducts?.Count > 0)
                    {
                        var bookingReferenceNumber = string.Empty;
                        try
                        {
                            _aotAdapter.SetAgentIdPassword(CountryType.Fiji);
                            var aotResponse = _aotAdapter.CreateBooking(fijiProducts, criteria.Token, criteria.Booking.ReferenceNumber,
                                out request, out response);

                            var selectedProducts = criteria.SelectedProducts;
                            var bookingResponse = aotResponse as Booking;
                            if (!string.IsNullOrWhiteSpace(bookingResponse?.ReferenceNumber))
                            {
                                bookingReferenceNumber = bookingResponse.ReferenceNumber;
                                selectedProducts = bookingResponse.SelectedProducts;
                            }
                            else
                            {
                                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);

                                try
                                {
                                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, aotSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                        bookingResponse?.ReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                        Convert.ToInt32(APIType.Aot), aotSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                        aotSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, aotSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                    criteria?.Booking?.UpdateDBLogFlag();
                                }
                                catch (Exception e)
                                {
                                    //ignore
                                }
                            }

                            bookedProducts = CreateAotBookedProducts(selectedProducts,
                                criteria.Booking.IsangoBookingData?.BookedProducts, bookingReferenceNumber,
                                CountryType.Fiji);
                            LogPurchaseXmlForAot(criteria.Booking, request, response, bookingReferenceNumber);
                        }
                        catch (Exception ex)
                        {
                            bookedProducts = UpdateOptionStatusForAotProduct(fijiProducts, isangoBookedProducts, bookingReferenceNumber);
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "SupplierBookingService",
                                MethodName = "CreateAotBooking",
                                Token = criteria.Token,
                                Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                            };

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, aotSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.Aot), aotSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    aotSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, aotSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                    if (newZealandProducts?.Count > 0)
                    {
                        var bookingReferenceNumber = string.Empty;
                        try
                        {
                            _aotAdapter.SetAgentIdPassword(CountryType.NewZealand);
                            var aotResponse = _aotAdapter.CreateBooking(newZealandProducts, criteria.Token, criteria.Booking.ReferenceNumber,
                                out request, out response);

                            var selectedProducts = criteria.SelectedProducts;
                            var bookingResponse = aotResponse as Booking;
                            if (!string.IsNullOrWhiteSpace(bookingResponse?.ReferenceNumber))
                            {
                                bookingReferenceNumber = bookingResponse.ReferenceNumber;
                                selectedProducts = bookingResponse.SelectedProducts;
                            }
                            else
                            {
                                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);

                                try
                                {
                                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, aotSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                        bookingResponse?.ReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                        Convert.ToInt32(APIType.Aot), aotSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                        aotSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, aotSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                    criteria?.Booking?.UpdateDBLogFlag();
                                }
                                catch (Exception e)
                                {
                                    //ignore
                                }
                            }

                            bookedProducts = CreateAotBookedProducts(selectedProducts,
                                criteria.Booking.IsangoBookingData?.BookedProducts, bookingReferenceNumber,
                                CountryType.NewZealand);
                            LogPurchaseXmlForAot(criteria.Booking, request, response, bookingReferenceNumber);
                        }
                        catch (Exception ex)
                        {
                            bookedProducts = UpdateOptionStatusForAotProduct(newZealandProducts, isangoBookedProducts, bookingReferenceNumber);
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "SupplierBookingService",
                                MethodName = "CreateAotBooking",
                                Token = criteria.Token,
                                Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                            };

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (aotSelectedProducts == null || !aotSelectedProducts.Any()) return bookedProducts;
                aotSelectedProducts.ForEach(product =>
                {
                    var bookedProduct = isangoBookedProducts.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(product.AvailabilityReferenceId));
                    if (bookedProduct == null) return;
                    bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                    bookedProducts.Add(bookedProduct);
                });
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateAotBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                               , System.Net.HttpStatusCode.BadGateway
                               , $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, bookedProducts?.FirstOrDefault()?.ServiceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.Aot), bookedProducts?.FirstOrDefault()?.OptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        bookedProducts?.FirstOrDefault()?.OptionName ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, bookedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }

        public Dictionary<string, bool> FareharborDeleteBooking(List<FareHarborSelectedProduct> fareHarborSelectedProducts, string bookingReferenceNumber, string token)
        {
            var status = new Dictionary<string, bool>();
            fareHarborSelectedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });

            var isCancelled = false;
            foreach (var fareHarborSelectedProduct in fareHarborSelectedProducts)
            {
                try
                {
                    var activityOption = (ActivityOption)fareHarborSelectedProduct.ProductOptions.FirstOrDefault();
                    var bookingResponse = _fareHarborAdapter.DeleteBooking(fareHarborSelectedProduct.Code, fareHarborSelectedProduct.UuId, activityOption?.UserKey,
                        out var request, out var response, token);
                    if (bookingResponse?.Status == BookingStatus.Cancelled)
                        isCancelled = true;
                    status[fareHarborSelectedProduct.AvailabilityReferenceId] = isCancelled;

                    var logCriteria = new LogPurchaseXmlCriteria
                    {
                        RequestXml = request,
                        ResponseXml = response,
                        Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed,
                        BookingId = 0,
                        APIType = APIType.Fareharbor,
                        BookingReferenceNumber = bookingReferenceNumber,
                        Bookingtype = "Cancel Booking"
                    };
                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "SupplierBookingService",
                        MethodName = "FareharborDeleteBooking",
                        Token = token,
                        Params = $"{SerializeDeSerializeHelper.Serialize(fareHarborSelectedProducts)}{token}"
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            }

            return status;
        }

        public Dictionary<string, bool> CancelBokunBooking(List<SelectedProduct> selectedProducts, string confirmationCode, string bookingReferenceNumber, string token)
        {
            var status = new Dictionary<string, bool>();
            selectedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });

            foreach (var selectedProduct in selectedProducts)
            {
                try
                {
                    var isCancelled = _bokunAdapter.CancelBooking(confirmationCode, token, out var request, out var response);
                    status[selectedProduct.AvailabilityReferenceId] = isCancelled;

                    var logCriteria = new LogPurchaseXmlCriteria
                    {
                        RequestXml = request,
                        ResponseXml = response,
                        Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed,
                        BookingId = 0,
                        APIType = APIType.Bokun,
                        BookingReferenceNumber = bookingReferenceNumber,
                        Bookingtype = "Cancel Booking"
                    };

                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "SupplierBookingService",
                        MethodName = "CancelBokunBooking",
                        Token = token,
                        Params = $"{SerializeDeSerializeHelper.Serialize(selectedProduct)}{confirmationCode}{token}"
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            }

            return status;
        }

        public List<BookedProduct> CreateBigBusBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var selectedProducts = criteria.Booking.SelectedProducts.Where(x => x.APIType.Equals(APIType.BigBus)).ToList();

            var request = string.Empty;
            var response = string.Empty;

            try
            {
                var logPurchaseCriteria = new LogPurchaseXmlCriteria
                {
                    BookingId = criteria.Booking.BookingId,
                    APIType = APIType.BigBus,
                    BookingReferenceNumber = criteria.Booking.ReferenceNumber
                };

                selectedProducts = _bigBusAdapter.CreateReservation(selectedProducts, criteria.Token, out request, out response);

                if (((BigBusSelectedProduct)selectedProducts.FirstOrDefault())?.BookingStatus == BigBusApiStatus.Reserved)
                {
                    var ticketPerPassenger = Convert.ToBoolean(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TicketPerPassenger));
                    logPurchaseCriteria.RequestXml = request;
                    logPurchaseCriteria.ResponseXml = response;
                    logPurchaseCriteria.Bookingtype = Constant.StatusReservation;
                    logPurchaseCriteria.Status = Constant.StatusSuccess;

                    // log purchase criteria for CreateReservation call
                    InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.CreateReservation, Constant.StatusSuccess);

                    selectedProducts.ForEach(x => ((BigBusSelectedProduct)x).TicketPerPassenger = ticketPerPassenger);

                    var bookedSelectedProducts = _bigBusAdapter.CreateBooking(selectedProducts, criteria.Token, out request, out response);

                    var bbSelectedProduct = (BigBusSelectedProduct)bookedSelectedProducts?.FirstOrDefault();
                    var bookingStatus = bookedSelectedProducts == null ? BigBusApiStatus.Cancelled : bbSelectedProduct?.BookingStatus;

                    logPurchaseCriteria.RequestXml = request;
                    logPurchaseCriteria.ResponseXml = response;
                    logPurchaseCriteria.Bookingtype = Constant.StatusBooking;
                    logPurchaseCriteria.Status = bookingStatus == BigBusApiStatus.Booked ?
                        Constant.StatusSuccess : Constant.StatusFailed;

                    // log purchase criteria for CreateBooking call
                    InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.CreateBooking, bookingStatus);

                    foreach (var selectedProduct in selectedProducts)
                    {
                        var bookedSelectedProduct = (BigBusSelectedProduct)(bookedSelectedProducts?.FirstOrDefault(x => x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId) ?? selectedProduct);

                        bookedSelectedProduct.ProductOptions = UpdateOptionStatus(selectedProduct.ProductOptions, bookedSelectedProduct.BookingReference);

                        var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == bookedSelectedProduct.AvailabilityReferenceId);

                        if (bookedProduct == null)
                        {
                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, bookedProducts?.FirstOrDefault()?.ServiceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    bookedProduct?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.BigBus), bookedProducts?.FirstOrDefault()?.OptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    bookedProducts?.FirstOrDefault()?.OptionName ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, bookedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            continue;
                        }
                        bookedProduct = MapProductForBigBus(bookedProduct, bookedSelectedProduct);
                        bookedProducts.Add(bookedProduct);
                    }
                }
                else
                {
                    logPurchaseCriteria.RequestXml = request;
                    logPurchaseCriteria.ResponseXml = response;
                    logPurchaseCriteria.Bookingtype = Constant.StatusReservation;
                    logPurchaseCriteria.Status = Constant.StatusFailed;

                    InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.CreateReservation, Constant.StatusFailed);

                    foreach (var bigBusProduct in selectedProducts)
                    {
                        var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                            x.AvailabilityReferenceId == bigBusProduct.AvailabilityReferenceId);
                        if (bookedProduct == null) continue;
                        bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                        bookedProducts.Add(bookedProduct);
                    }
                    //Api booking failed
                    criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);
                    try
                    {
                        LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, bookedProducts?.FirstOrDefault()?.ServiceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                            bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.BigBus), bookedProducts?.FirstOrDefault()?.OptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            bookedProducts?.FirstOrDefault()?.OptionName ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, bookedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                        criteria?.Booking?.UpdateDBLogFlag();
                    }
                    catch (Exception e)
                    {
                        //ignore
                    }

                    return bookedProducts;
                }
            }
            catch (Exception ex)
            {
                foreach (var selectedProduct in selectedProducts)
                {
                    var option = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);

                    var bookingReference = ((BigBusSelectedProduct)selectedProduct).ReservationReference;
                    if (bookedProduct == null) continue;
                    bookedProduct.OptionStatus = GetBookingStatusNumber(bookingReference, option?.AvailabilityStatus);
                    bookedProducts.Add(bookedProduct);
                }

                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateBigBusBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, bookedProducts?.FirstOrDefault()?.ServiceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.BigBus), bookedProducts?.FirstOrDefault()?.OptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        bookedProducts?.FirstOrDefault()?.OptionName ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, bookedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }

        public Dictionary<string, bool> CancelBigBusBooking(List<SelectedProduct> selectedProducts, string token)
        {
            var status = new Dictionary<string, bool>();
            try
            {
                if (((BigBusSelectedProduct)selectedProducts.FirstOrDefault())?.BookingStatus == BigBusApiStatus.Booked)
                {
                    status = _bigBusAdapter.CancelBooking(selectedProducts, token, out var request, out var response);
                }
                else if (((BigBusSelectedProduct)selectedProducts.FirstOrDefault())?.BookingStatus == BigBusApiStatus.Reserved)
                {
                    status = _bigBusAdapter.CancelReservation(selectedProducts, token, out var request, out var response);
                }
                else
                {
                    status.Add(selectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, false);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CancelBokunBooking",
                    Token = token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(selectedProducts)}{token}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return status;
        }

        /// <summary>
        /// AOT Cancel entire Booking
        /// </summary>
        public Dictionary<string, bool> CancelAotBooking(List<BookedProduct> bookedProducts, string bookingReferenceNumber, string token)
        {
            var isCancelled = false;
            var status = new Dictionary<string, bool>();
            bookedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });

            var logCriteria = new LogPurchaseXmlCriteria
            {
                BookingId = 0,
                APIType = APIType.Aot,
                BookingReferenceNumber = bookingReferenceNumber,
                Bookingtype = "Cancel Booking"
            };

            foreach (var bookedProduct in bookedProducts)
            {
                try
                {
                    var apiExtraDetail = bookedProduct.APIExtraDetail;
                    Enum.TryParse(bookedProduct.CountryCode, true, out CountryType countryType);
                    _aotAdapter.SetAgentIdPassword(countryType);
                    var serviceStatus = _aotAdapter.CancelSingleServiceBooking(apiExtraDetail.SupplieReferenceNumber, apiExtraDetail.SupplierLineNumber, token, out var request, out var response);

                    logCriteria.RequestXml = request;
                    logCriteria.ResponseXml = response;

                    if (serviceStatus == null)
                    {
                        logCriteria.Status = Constant.StatusFailed;
                        _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                        continue;
                    }

                    isCancelled = serviceStatus.Status.ToLowerInvariant().Equals("no", StringComparison.InvariantCultureIgnoreCase);
                    status[bookedProduct.AvailabilityReferenceId] = isCancelled;

                    logCriteria.Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed;
                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                }
                catch (Exception ex)
                {
                    status[bookedProduct.AvailabilityReferenceId] = isCancelled;
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "SupplierBookingService",
                        MethodName = "CancelAotBooking",
                        Token = token,
                        Params = $"{SerializeDeSerializeHelper.Serialize(bookedProduct)}{token}"
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            }

            return status;
        }

        public Dictionary<string, bool> CancelSightSeeingBooking(List<SelectedProduct> selectedProducts, string token)
        {
            var status = new Dictionary<string, bool>();
            try
            {
                selectedProducts.RemoveAll(x => string.IsNullOrEmpty(((CitySightseeingSelectedProduct)x).Pnr));
                if (selectedProducts.Count > 0)
                {
                    status = _sightSeeingAdapter.CancelTicket(selectedProducts, token, out var request, out var response);
                }
            }
            catch (Exception ex)
            {
                selectedProducts?.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CancelSightSeeingBooking",
                    Token = token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(selectedProducts)}{token}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return status;
        }

        private MoulinRougeSelectedProduct MoulinRougeAddtoCart(MoulinRougeSelectedProduct moulinRougeProduct, string token)
        {
            var selectedOption = moulinRougeProduct.ProductOptions.SingleOrDefault(x => x.IsSelected);
            var apiContextMoulinRouge = selectedOption?.SellPrice.DatePriceAndAvailabilty.Select(x => x.Value).Cast<MoulinRougePriceAndAvailability>().FirstOrDefault()?.MoulinRouge;

            if (string.IsNullOrEmpty(moulinRougeProduct.CartReferenceId) || (!string.IsNullOrEmpty(moulinRougeProduct.CartReferenceId) && moulinRougeProduct.Expiry.Minute > 15))
            {
                if (apiContextMoulinRouge != null)
                {
                    var noOfPassengers = moulinRougeProduct.ProductOptions.FirstOrDefault(s => s.IsSelected)?.TravelInfo
                        .NoOfPassengers;
                    moulinRougeProduct.CatalogDateId = apiContextMoulinRouge.CatalogDateId;
                    moulinRougeProduct.CategoryId = apiContextMoulinRouge.CategoryId;
                    moulinRougeProduct.ContingentId = apiContextMoulinRouge.ContingentId;
                    moulinRougeProduct.BlocId = apiContextMoulinRouge.BlocId;
                    moulinRougeProduct.FloorId = apiContextMoulinRouge.FloorId;
                    moulinRougeProduct.RateId = apiContextMoulinRouge.RateId;
                    moulinRougeProduct.Quantity =
                        noOfPassengers?.Where(s => s.Key.Equals(PassengerType.Adult)).Select(x => x.Value)
                            .FirstOrDefault() + noOfPassengers?.Where(s => s.Key.Equals(PassengerType.Child))
                            .Select(x => x.Value).FirstOrDefault() ?? 0;
                    moulinRougeProduct.Amount = apiContextMoulinRouge.Amount;
                }

                if (moulinRougeProduct.Quantity > 0)
                {
                    var addCartOutput = _moulinRougeAdapter.AddToCart(moulinRougeProduct, token);

                    if (addCartOutput != null)
                    {
                        moulinRougeProduct.TemporaryOrderId = addCartOutput.TemporaryOrderId;
                        moulinRougeProduct.TemporaryOrderRowId = addCartOutput.TemporaryOrderRowId;
                        moulinRougeProduct.CartReferenceId = addCartOutput.TemporaryOrderId;
                        moulinRougeProduct.Expiry = addCartOutput.Expiry;
                    }
                }
            }

            return moulinRougeProduct;
        }

        private Response MoulinRougeAddtoCartAPI(MoulinRougeSelectedProduct moulinRougeProduct, string token)
        {
            var selectedOption = moulinRougeProduct.ProductOptions.SingleOrDefault(x => x.IsSelected);
            var apiContextMoulinRouge = selectedOption?.SellPrice.DatePriceAndAvailabilty.Select(x => x.Value).Cast<MoulinRougePriceAndAvailability>().FirstOrDefault()?.MoulinRouge;

            if (string.IsNullOrEmpty(moulinRougeProduct.CartReferenceId) || (!string.IsNullOrEmpty(moulinRougeProduct.CartReferenceId) && moulinRougeProduct.Expiry.Minute > 15))
            {
                if (apiContextMoulinRouge != null)
                {
                    var noOfPassengers = moulinRougeProduct.ProductOptions.FirstOrDefault(s => s.IsSelected)?.TravelInfo
                        .NoOfPassengers;
                    moulinRougeProduct.CatalogDateId = apiContextMoulinRouge.CatalogDateId;
                    moulinRougeProduct.CategoryId = apiContextMoulinRouge.CategoryId;
                    moulinRougeProduct.ContingentId = apiContextMoulinRouge.ContingentId;
                    moulinRougeProduct.BlocId = apiContextMoulinRouge.BlocId;
                    moulinRougeProduct.FloorId = apiContextMoulinRouge.FloorId;
                    moulinRougeProduct.RateId = apiContextMoulinRouge.RateId;
                    moulinRougeProduct.Quantity =
                        noOfPassengers?.Where(s => s.Key.Equals(PassengerType.Adult)).Select(x => x.Value)
                            .FirstOrDefault() + noOfPassengers?.Where(s => s.Key.Equals(PassengerType.Child))
                            .Select(x => x.Value).FirstOrDefault() ?? 0;
                    moulinRougeProduct.Amount = apiContextMoulinRouge.Amount;
                }

                if (moulinRougeProduct.Quantity > 0)
                {
                    var addCartOutput = _moulinRougeAdapter.AddToCartAPI(moulinRougeProduct, token);
                    return addCartOutput;

                }
            }
            return null;
        }

        private List<BookedProduct> SetFailedStatusOfHBBookedProduct(ActivityBookingCriteria criteria,
    List<SelectedProduct> hbSelectedProducts)
        {
            var bookedProducts = new List<BookedProduct>();
            foreach (var hbSelectedProduct in hbSelectedProducts)
            {
                var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                    x.AvailabilityReferenceId == hbSelectedProduct.AvailabilityReferenceId);
                if (bookedProduct == null) continue;
                bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                bookedProduct.Errors = criteria.Booking.Errors;
                bookedProducts.Add(bookedProduct);
            }
            return bookedProducts;
        }

        private List<Variant> GetVariants(List<TiqetsPaxMapping> paxMappings, Dictionary<PassengerType, int> numberOfPassengers)
        {
            if (numberOfPassengers?.Count > 0)
            {
                var variants = new List<Variant>();

                foreach (var passenger in numberOfPassengers)
                {
                    var paxInfo = paxMappings.FirstOrDefault(x => x.PassengerType == passenger.Key);
                    if (paxInfo != null)
                    {
                        var variant = new Variant
                        {
                            Count = passenger.Value //Pax Count
                        };
                        int.TryParse(paxInfo.AgeGroupCode, out var ageGroupCode);
                        variant.Id = ageGroupCode;
                        variants.Add(variant);
                    }
                }

                return variants;
            }

            return null;
        }

        private List<BookedProduct> SetFailedBookingStatus(List<SelectedProduct> tiqetsSelectedProducts, ActivityBookingCriteria activityBookingCriteria)
        {
            var bookedProducts = new List<BookedProduct>();
            foreach (var tiqetsProduct in tiqetsSelectedProducts)
            {
                var failedProduct = activityBookingCriteria.Booking.IsangoBookingData?.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == tiqetsProduct.AvailabilityReferenceId);
                if (failedProduct == null) continue;
                failedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                failedProduct.Errors = activityBookingCriteria.Booking.Errors;
                bookedProducts.Add(failedProduct);
            }

            return bookedProducts;
        }

        private void InsertLogPurchaseInDb(LogPurchaseXmlCriteria logPurchaseXmlCriteria, string requestXml, string responseXml, string methodName, string status)
        {
            logPurchaseXmlCriteria.RequestXml = requestXml;
            logPurchaseXmlCriteria.ResponseXml = responseXml;
            logPurchaseXmlCriteria.Bookingtype = methodName;
            logPurchaseXmlCriteria.Status = status;
            _supplierBookingPersistence.LogPurchaseXML(logPurchaseXmlCriteria);
        }

        #region Response Mapper

        public BookedProduct MapProductForGLI(BookedProduct bookedProduct, GrayLineIceLandSelectedProduct selectedProduct, string referenceNumer)
        {
            var apiExtraDetail = bookedProduct.APIExtraDetail ?? new ApiExtraDetail();
            var selectedOption = (ActivityOption)selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);

            apiExtraDetail.SupplieReferenceNumber = referenceNumer;
            apiExtraDetail.SupplierLineNumber = selectedProduct.ReservationId.ToString();
            apiExtraDetail.APIOptionName = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected)?.Name;
            apiExtraDetail.PickUpId = selectedProduct.HotelPickUpLocation;

            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(referenceNumer, selectedOption?.AvailabilityStatus);

            return bookedProduct;
        }

        public BookedProduct MapProductForHB(BookedProduct bookedProduct, HotelBedsSelectedProduct selectedProduct)
        {
            var apiExtraDetail = bookedProduct.APIExtraDetail ?? new ApiExtraDetail();
            var selectedOption = (ActivityOption)selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
            bookedProduct.InvoicingCompany = selectedProduct.InvoicingCompany;

            apiExtraDetail.SupplieReferenceNumber = selectedProduct.FileNumber;
            apiExtraDetail.VATNo = selectedProduct.VatNumber;
            apiExtraDetail.OfficeCode = selectedProduct.OfficeCode;
            apiExtraDetail.APIOptionName = selectedOption?.Name;
            apiExtraDetail.SupplierCancellationPolicy = selectedOption.ApiCancellationPolicy;
            apiExtraDetail.SupplierLineNumber = selectedProduct?.SPUI;
            apiExtraDetail.SupplierName = selectedProduct?.SupplierName;
            apiExtraDetail.ProviderInformation = selectedProduct?.ProviderInformation;

            var qrcode = selectedProduct?.BookingVouchers?
                         .FirstOrDefault(x =>
                                         //x.Language?.ToLower()?.Contains("en") == true
                                         //&&
                                         x.Type == BookingVoucherType.PDF
                                        )?
                                        .Url;

            if (!string.IsNullOrWhiteSpace(qrcode))
            {
                apiExtraDetail.QRCodeType = "LINK";
                apiExtraDetail.QRCode = qrcode;
            }
            else if (string.IsNullOrWhiteSpace(qrcode)
                && selectedProduct.IsVocuherCustomizable == true)
            {
                var qrImagesQuery = from bv in selectedProduct?.BookingVouchers
                                    from df in bv.DownloadFileNames
                                        //where df.QRCodeType == BookingVoucherQRCodeType.HTMLIMAGE
                                    select df;

                var customers = selectedProduct?.ProductOptions?.FirstOrDefault(x => x.IsSelected)?.Customers?.ToList();
                var qrImages = qrImagesQuery.ToList();
                if (qrImages?.Count > 0)
                {
                    apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
                    int i = 0;
                    foreach (var qrImage in qrImages)
                    {
                        var apiTicketDetail = new ApiTicketDetail
                        {
                            APIOrderId = string.Empty,
                            BarCode = qrImage.CodeValue,
                            SeatId = string.Empty,
                            CodeType = qrImage.CodeType,//string, link
                            ResourceLocal = qrImage.DownloadedFile,
                            ResouceRemoteUrl = qrImage.Url,
                            //ResourceType = qrImage.QRCodeType.ToString(),
                            ResourceType = MapAPICodeFormatWithIsangoCode(qrImage.QRCodeType.ToString(), APIType.Hotelbeds),
                            CodeValue = qrImage.CodeValue,
                            IsResourceApply = true
                        };

                        try
                        {
                            if (customers?.Count == qrImages.Count)
                            {
                                apiTicketDetail.PassengerType = Convert.ToString(customers[i].PassengerType);
                                i++;
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error($"SupplierBookingService|CreateTiqetsBooking|{SerializeDeSerializeHelper.Serialize(qrImages)}", ex);
                        }
                        apiExtraDetail.APITicketDetails.Add(apiTicketDetail);
                    }
                }
            }

            var code = new StringBuilder();
            if (selectedProduct?.BookedSeats != null && selectedProduct?.BookedSeats?.Count > 1)
            {
                for (int i = 0; i < selectedProduct.BookedSeats.Count; i++)
                {
                    var bookedSeats = $"{selectedProduct.BookedSeats[i].Row} - {selectedProduct.BookedSeats[i].Seat},";
                    code.Append(bookedSeats);
                }
            }
            else
            {
                code.Append(selectedOption?.Code);
            }

            if (!string.IsNullOrWhiteSpace(selectedOption?.SupplierOptionCode))
            {
                //Modality code
                var supplierOptionCode = selectedOption.Code;

                //Supplier Activity Code
                var productCode = selectedOption?.SupplierOptionCode;
                apiExtraDetail.ProductCode = $"{productCode}~{supplierOptionCode}";
            }

            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.ContractComment = string.Join("\n", selectedOption?.Contract?.Comments?.Where(x => x.Type.ToUpper().Contains("CONTRACT"))?.Select(y => y.CommentText)?.ToArray());
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct.FileNumber, selectedOption?.AvailabilityStatus);

            return bookedProduct;
        }

        public BookedProduct MapProductForMR(BookedProduct bookedProduct, MoulinRougeSelectedProduct selectedProduct)
        {
            var apiExtraDetail = bookedProduct.APIExtraDetail ?? new ApiExtraDetail();
            var selectedOption = (ActivityOption)selectedProduct?.ProductOptions?.FirstOrDefault(x => x.IsSelected);

            apiExtraDetail.SupplieReferenceNumber = selectedProduct?.OrderId;
            apiExtraDetail.APIOptionName = selectedOption?.Name;
            apiExtraDetail.PickUpId = selectedProduct?.HotelPickUpLocation;
            apiExtraDetail.ETicketGUIDs = selectedProduct.ETicketGuiDs != null ? string.Join(",", selectedProduct.ETicketGuiDs) : "";

            if (selectedProduct.MrConfirmedTickets != null)
            {
                apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
                foreach (var mrConfirmedTicket in selectedProduct.MrConfirmedTickets)
                {
                    var apiTicketDetail = new ApiTicketDetail
                    {
                        APIOrderId = selectedProduct?.OrderId,
                        BarCode = mrConfirmedTicket?.BarCode,
                        FiscalNumber = mrConfirmedTicket?.FiscalNumber,
                        SeatId = mrConfirmedTicket?.SeatId
                    };
                    apiExtraDetail.APITicketDetails.Add(apiTicketDetail);
                }
            }
            apiExtraDetail.ConfirmedTicketAttachments = selectedProduct?.ConfirmedTicketAttachments;
            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct?.OrderId, selectedOption?.AvailabilityStatus);

            return bookedProduct;
        }

        public BookedProduct MapProductForCitySightSeeing(BookedProduct bookedProduct, CitySightseeingSelectedProduct selectedProduct)
        {
            var apiExtraDetail = bookedProduct.APIExtraDetail ?? new ApiExtraDetail();
            var selectedOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);

            apiExtraDetail.SupplieReferenceNumber = selectedProduct.Pnr;
            apiExtraDetail.QRCode = selectedProduct.QrCode;
            apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(Constant.String,APIType.Citysightseeing);
            apiExtraDetail.APIOptionName = selectedOption?.Name;

            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct.Pnr, selectedOption?.AvailabilityStatus);

            return bookedProduct;
        }

        public BookedProduct MapProductForPrio(BookedProduct bookedProduct, PrioSelectedProduct selectedProduct)
        {
            var apiExtraDetail = bookedProduct.APIExtraDetail ?? new ApiExtraDetail();
            var prioApiConfig = selectedProduct.PrioApiConfirmedBooking;
            var selectedOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
            var prioBookingDetail = prioApiConfig.BookingDetails.FirstOrDefault();

            apiExtraDetail.SupplieReferenceNumber = prioApiConfig.BookingReference;
            apiExtraDetail.APIOptionName = selectedOption?.Name;
            apiExtraDetail.PickUpId = selectedProduct.HotelPickUpLocation;

            apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
            if (prioBookingDetail?.TicketDetails != null)
                apiExtraDetail.ETicketGUIDs = prioBookingDetail?.TicketDetails.FirstOrDefault()?.TicketCode;

            if (!string.IsNullOrEmpty(prioApiConfig.BookingDetails[0].GroupCode))
            {
                //var barCodeString = prioApiConfig.BookingDetails[0].GroupCode;
                var barCodeString = GetBarCodeString(prioApiConfig);
                //apiExtraDetail.QRCode = $"{prioBookingDetail?.CodeType}~{barCodeString}";
                apiExtraDetail.QRCode = $"{MapAPICodeFormatWithIsangoCode(prioBookingDetail?.CodeType, APIType.Prio)}~{barCodeString}";
                apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(prioApiConfig.BookingDetails[0].CodeType,APIType.Prio);
            }
            else
            {
                apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
                if (prioApiConfig.BookingDetails.FirstOrDefault()?.TicketDetails != null)
                {
                    foreach (var ticketDetail in prioApiConfig.BookingDetails.FirstOrDefault()?.TicketDetails)
                    {
                        var apiTicketDetail = new ApiTicketDetail
                        {
                            BarCode = ticketDetail.TicketCode,
                            FiscalNumber = ticketDetail.TicketType,
                            SeatId = "1"
                        };

                        apiExtraDetail.APITicketDetails.Add(apiTicketDetail);
                    }

                    apiExtraDetail.IsQRCodePerPax = true;
                }
            }

            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(prioApiConfig.BookingReference, selectedOption?.AvailabilityStatus);

            return bookedProduct;
        }

        public BookedProduct MapProductForPrioHub(BookedProduct bookedProduct, PrioHubSelectedProduct selectedProduct, string token = "")
        {
            var request = string.Empty;
            var response = string.Empty;
            var apiExtraDetail = bookedProduct.APIExtraDetail ?? new ApiExtraDetail();
            var prioApiConfig = selectedProduct?.PrioHubApiConfirmedBooking;
            var selectedOption = selectedProduct?.ProductOptions?.FirstOrDefault(x => x.IsSelected);
            var prioBookingDetail = prioApiConfig?.BookingDetails?.FirstOrDefault();

            apiExtraDetail.SupplieReferenceNumber = prioApiConfig?.BookingReference;
            apiExtraDetail.APIOptionName = selectedOption?.Name;
            apiExtraDetail.PickUpId = selectedProduct?.HotelPickUpLocation;
            apiExtraDetail.OfficeCode = prioApiConfig?.DistributorId;
            apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
            if (prioBookingDetail?.TicketDetails != null)
                apiExtraDetail.ETicketGUIDs = prioBookingDetail?.TicketDetails.FirstOrDefault()?.TicketCode;

            if (!string.IsNullOrEmpty(prioApiConfig?.BookingDetails[0]?.GroupCode))
            {
                //var barCodeString = prioApiConfig.BookingDetails[0].GroupCode;
                var barCodeString = GetBarCodeStringPrioHub(prioApiConfig);
                //apiExtraDetail.QRCode = $"{prioBookingDetail?.CodeType}~{barCodeString}";
                //apiExtraDetail.QRCodeType = prioApiConfig.BookingDetails[0].CodeType;
                apiExtraDetail.QRCode = $"{MapAPICodeFormatWithIsangoCode(prioBookingDetail?.CodeType, APIType.PrioHub)}~{barCodeString}";
                apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(prioApiConfig.BookingDetails[0].CodeType, APIType.PrioHub);

            }
            //if no QrCode return from API, the hit API GetVocher EndPoint
            else if (string.Equals(selectedProduct?.PrioHubApiConfirmedBooking?.BookingStatus?.ToUpper(), ConstantPrioHub.BOOKINGCONFIRMED, StringComparison.CurrentCultureIgnoreCase) &&
               !string.IsNullOrEmpty(selectedProduct?.PrioHubApiConfirmedBooking?.BookingReference) &&
                string.IsNullOrEmpty(prioApiConfig?.BookingDetails[0]?.GroupCode) &&
                prioBookingDetail?.TicketDetails == null
               )
            {

                GetVoucherRes getURL = _prioHubAdapter.GetVoucher(Convert.ToInt32(selectedProduct.PrioHubDistributerId), selectedProduct.PrioHubApiConfirmedBooking.BookingReference,
                   token, out request, out response);
                var getURLData = getURL.Url;
                if (!string.IsNullOrEmpty(getURLData))
                {
                    apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(Constant.TicketPdfType,APIType.PrioHub);
                    apiExtraDetail.QRCode = getURLData;
                }

            }
            else
            {
                apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
                if (prioApiConfig?.BookingDetails?.FirstOrDefault()?.TicketDetails != null)
                {
                    foreach (var ticketDetail in prioApiConfig.BookingDetails.FirstOrDefault()?.TicketDetails)
                    {
                        var apiTicketDetail = new ApiTicketDetail
                        {
                            BarCode = ticketDetail?.TicketCode,
                            FiscalNumber = ticketDetail?.TicketType,
                            SeatId = "1"
                        };

                        apiExtraDetail.APITicketDetails.Add(apiTicketDetail);
                    }

                    apiExtraDetail.IsQRCodePerPax = true;
                    apiExtraDetail.OfficeCode = prioApiConfig.DistributorId;
                }
            }

            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(prioApiConfig?.BookingReference, selectedOption?.AvailabilityStatus);

            return bookedProduct;
        }
        //TODO - Map products for Ventrata
        public BookedProduct MapProductForVentrata(BookedProduct bookedProduct, VentrataSelectedProduct selectedProduct)
        {
            var apiExtraDetail = bookedProduct?.APIExtraDetail ?? new ApiExtraDetail();
            var ventrataBookingDetails = selectedProduct?.ApiBookingDetails;
            var selectedOption = selectedProduct?.ProductOptions?.FirstOrDefault(x => x.IsSelected);
            apiExtraDetail.SupplieReferenceNumber = selectedProduct?.SupplierReference;
            apiExtraDetail.SupplierLineNumber = selectedProduct?.Uuid;
            apiExtraDetail.APIOptionName = selectedOption?.Name;
            //TODO - ToCheck
            apiExtraDetail.PickUpId = ventrataBookingDetails?.PickUpPoint;
            apiExtraDetail.SupplierCancellationPolicy = selectedProduct?.ApiBookingDetails?.ApiCancellationPolicy;

            var voucherConditionDB = selectedProduct?.VentrataIsPerPaxQRCode;

            //Package condition check- in case of package, pick up the supplier voucher
            //var packageCheck = selectedProduct?.ApiBookingDetails?.IsPackage ?? selectedProduct?.IsPackage;


            //Special Condition for Golden tours supplier only:
            var ventrataGoldenTour = ConfigurationManagerHelper.GetValuefromAppSettings("VentrataGoldenTourValue");
            var checkGoldenTourProduct = selectedProduct?.TsProductCode;
            if (ventrataGoldenTour?.ToLower() == checkGoldenTourProduct?.ToLower())
            {
                //if this is GoldenTour Supplier then ignore database condition 
                //and apply API condition
                var tickets = ventrataBookingDetails?.DeliveryMethods?.Any(thisMethod => thisMethod.Equals(ConstantForVentrata.TicketDeliveryMethod));
                if (tickets == true && (!bookedProduct?.IsShowSupplierVoucher ?? false))
                {
                    voucherConditionDB = "True";
                }
                else
                {
                    voucherConditionDB = string.Empty;
                }
            }
            //In case of packages, always pickup supplier voucher for every case
            //if (packageCheck == true)
            //{
            //    voucherConditionDB = string.Empty;
            //}

            if (ventrataBookingDetails != null && (voucherConditionDB == "" || voucherConditionDB.ToLower() == "false"))
            {
                var isVoucherAvailable = ventrataBookingDetails.VoucherAtBookingLevel.DeliveryOptions.Any(thisDelOpt => thisDelOpt.DeliveryFormat.Equals(ConstantForVentrata.PDFDeliveryFormat));

                var isQrAvailable = ventrataBookingDetails.VoucherAtBookingLevel.DeliveryOptions.Any(thisDelOpt => thisDelOpt.DeliveryFormat.Equals(ConstantForVentrata.QRCodeDeliveryFormat));

                try
                {
                    //if (packageCheck == true)// package product
                    //{
                    //    apiExtraDetail.IsQRCodePerPax = false;
                    //    apiExtraDetail.QRCode = ventrataBookingDetails?.VoucherAtBookingLevel?.DeliveryOptions?.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.PDFDeliveryFormat)).DeliveryValue;
                    //    apiExtraDetail.QRCodeType = ConstantForVentrata.LinkTypeForPDF;
                    //}
                    if ((!bookedProduct?.IsShowSupplierVoucher ?? false) && isQrAvailable)
                    {
                        apiExtraDetail.IsQRCodePerPax = false;
                        apiExtraDetail.QRCode = ventrataBookingDetails?.VoucherAtBookingLevel?.DeliveryOptions?.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.QRCodeDeliveryFormat)).DeliveryValue;
                        apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.StringTypeForQRCode,APIType.Ventrata);

                    }
                    else if (isVoucherAvailable)
                    {
                        apiExtraDetail.IsQRCodePerPax = false;
                        apiExtraDetail.QRCode = ventrataBookingDetails?.VoucherAtBookingLevel?.DeliveryOptions?.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.PDFDeliveryFormat)).DeliveryValue;
                        apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.LinkTypeForPDF, APIType.Ventrata);
                    }
                    else if (isQrAvailable)
                    {
                        apiExtraDetail.IsQRCodePerPax = false;
                        apiExtraDetail.QRCode = ventrataBookingDetails?.VoucherAtBookingLevel?.DeliveryOptions?.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.QRCodeDeliveryFormat)).DeliveryValue;
                        apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.StringTypeForQRCode, APIType.Ventrata);
                    }
                }
                catch (Exception e)
                {
                    if (isQrAvailable)
                    {
                        apiExtraDetail.IsQRCodePerPax = false;
                        apiExtraDetail.QRCode = ventrataBookingDetails?.VoucherAtBookingLevel?.DeliveryOptions?.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.QRCodeDeliveryFormat)).DeliveryValue;
                        apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.StringTypeForQRCode,APIType.Ventrata);
                    }
                    else if (isVoucherAvailable)
                    {
                        apiExtraDetail.IsQRCodePerPax = false;
                        apiExtraDetail.QRCode = ventrataBookingDetails?.VoucherAtBookingLevel?.DeliveryOptions?.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.PDFDeliveryFormat)).DeliveryValue;
                        apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.LinkTypeForPDF, APIType.Ventrata);
                    }

                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "SupplierBookingService",
                        MethodName = "MapProductForVentrata",
                        Token = "Ventrata_PDF_Error",
                        Params = $"{SerializeDeSerializeHelper.Serialize(selectedProduct)}"
                    };
                    _log.Error(isangoErrorEntity, e);
                }
            }
            //If booking details has ticket level delivery method and no Voucher at product level then set the voucher or QR code at Pax level
            else if (ventrataBookingDetails != null && ventrataBookingDetails?.UnitItems != null && ventrataBookingDetails?.UnitItems?.Count > 0
                && ventrataBookingDetails?.UnitItems?.FirstOrDefault()?.TicketPerUnitItem != null && ventrataBookingDetails?.UnitItems?.FirstOrDefault()?.TicketPerUnitItem?.DeliveryOptions?.Count > 0)
            //else if (ventrataBookingDetails.DeliveryMethods.Any(thisMethod => thisMethod.Equals(ConstantForVentrata.TicketDeliveryMethod)))
            {
                //Set at pax level the details of Pdf url and QR code
                apiExtraDetail.IsQRCodePerPax = true;
                apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
                if (ventrataBookingDetails?.UnitItems != null && ventrataBookingDetails?.UnitItems.Count > 0)
                {
                    foreach (var unitItemInBooking in ventrataBookingDetails?.UnitItems)
                    {
                        var apiTicketDetailsAtPaxLevel = new ApiTicketDetail();
                        if (unitItemInBooking.TicketPerUnitItem != null && unitItemInBooking?.TicketPerUnitItem?.DeliveryOptions?.Count > 0)
                        {
                            var isVoucherAvailable = unitItemInBooking.TicketPerUnitItem.DeliveryOptions.Any(thisDelOpt => thisDelOpt.DeliveryFormat.Equals(ConstantForVentrata.PDFDeliveryFormat));

                            var isQrAvailable = unitItemInBooking.TicketPerUnitItem.DeliveryOptions.Any(thisDelOpt => thisDelOpt.DeliveryFormat.Equals(ConstantForVentrata.QRCodeDeliveryFormat));

                            try
                            {
                                if ((!bookedProduct?.IsShowSupplierVoucher ?? false) && isQrAvailable)
                                {
                                    apiTicketDetailsAtPaxLevel.CodeValue = apiTicketDetailsAtPaxLevel.BarCode = unitItemInBooking.TicketPerUnitItem.DeliveryOptions.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.QRCodeDeliveryFormat)).DeliveryValue;
                                    apiTicketDetailsAtPaxLevel.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.StringTypeForQRCode,APIType.Ventrata);
                                    apiTicketDetailsAtPaxLevel.CodeType = ConstantForVentrata.QRCodeDeliveryFormat;
                                    try
                                    {
                                        ConstantForVentrata.CrossMapper_PassengerType_Isango_VentrataString.TryGetValue(unitItemInBooking?.Unit?.Title?.ToLower(), out var isangoPaxtype);
                                        apiTicketDetailsAtPaxLevel.PassengerType = isangoPaxtype.ToString();
                                    }
                                    catch (Exception e)
                                    {
                                        if (selectedProduct.VentrataPaxMappings == null || selectedProduct.VentrataPaxMappings.Count == 0)
                                        {
                                            var ventrataPaxMapping = _masterCacheManager.GetVentrataPaxMappings();
                                            if (ventrataPaxMapping?.Any() != true)
                                            {
                                                ventrataPaxMapping = GetPaxMappingsForVentrataAPI(APIType.Ventrata);
                                            }

                                            selectedProduct.VentrataPaxMappings = ventrataPaxMapping.Where(x => x.ServiceOptionId == selectedOption.ServiceOptionId)?.ToList();
                                        }

                                        var isangoPaxtype = selectedProduct.VentrataPaxMappings?.Where(x => x.AgeGroupCode == unitItemInBooking.UnitId)?.FirstOrDefault()?.PassengerType ?? PassengerType.Undefined;

                                        //ConstantForVentrata.CrossMapper_PassengerType_Isango_VentrataString.TryGetValue(unitItemInBooking.UnitId, out var isangoPaxtype);
                                        apiTicketDetailsAtPaxLevel.PassengerType = isangoPaxtype.ToString();
                                    }
                                    //Todo Tocheck
                                    apiTicketDetailsAtPaxLevel.APIOrderId = selectedProduct.Uuid;
                                }
                                else if (isVoucherAvailable)
                                {
                                    apiTicketDetailsAtPaxLevel.BarCode = unitItemInBooking?.TicketPerUnitItem?.DeliveryOptions.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.PDFDeliveryFormat)).DeliveryValue;
                                    apiTicketDetailsAtPaxLevel.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.LinkTypeForPDF,APIType.Ventrata);
                                    apiTicketDetailsAtPaxLevel.CodeType = ConstantForVentrata.LinkTypeForPDF;
                                    try
                                    {
                                        ConstantForVentrata.CrossMapper_PassengerType_Isango_VentrataString.TryGetValue(unitItemInBooking?.Unit?.Title?.ToLower(), out var isangoPaxtype);
                                        apiTicketDetailsAtPaxLevel.PassengerType = isangoPaxtype.ToString();
                                    }
                                    catch (Exception e)
                                    {
                                        if (selectedProduct.VentrataPaxMappings == null || selectedProduct.VentrataPaxMappings.Count == 0)
                                        {
                                            var ventrataPaxMapping = _masterCacheManager.GetVentrataPaxMappings();
                                            if (ventrataPaxMapping?.Any() != true)
                                            {
                                                ventrataPaxMapping = GetPaxMappingsForVentrataAPI(APIType.Ventrata);
                                            }

                                            selectedProduct.VentrataPaxMappings = ventrataPaxMapping.Where(x => x.ServiceOptionId == selectedOption.ServiceOptionId)?.ToList();
                                        }

                                        var isangoPaxtype = selectedProduct.VentrataPaxMappings?.Where(x => x.AgeGroupCode == unitItemInBooking.UnitId)?.FirstOrDefault()?.PassengerType ?? PassengerType.Undefined;

                                        //ConstantForVentrata.CrossMapper_PassengerType_Isango_VentrataString.TryGetValue(unitItemInBooking.UnitId, out var isangoPaxtype);
                                        apiTicketDetailsAtPaxLevel.PassengerType = isangoPaxtype.ToString();
                                    }
                                    //Todo Tocheck
                                    apiTicketDetailsAtPaxLevel.APIOrderId = selectedProduct.Uuid;
                                }
                                else if (isQrAvailable)
                                {
                                    apiTicketDetailsAtPaxLevel.CodeValue = apiTicketDetailsAtPaxLevel.BarCode = unitItemInBooking.TicketPerUnitItem.DeliveryOptions.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.QRCodeDeliveryFormat)).DeliveryValue;
                                    apiTicketDetailsAtPaxLevel.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.StringTypeForQRCode,APIType.Ventrata);
                                    apiTicketDetailsAtPaxLevel.CodeType = ConstantForVentrata.QRCodeDeliveryFormat;
                                    try
                                    {
                                        ConstantForVentrata.CrossMapper_PassengerType_Isango_VentrataString.TryGetValue(unitItemInBooking?.Unit?.Title?.ToLower(), out var isangoPaxtype);
                                        apiTicketDetailsAtPaxLevel.PassengerType = isangoPaxtype.ToString();
                                    }
                                    catch (Exception e)
                                    {
                                        if (selectedProduct.VentrataPaxMappings == null || selectedProduct.VentrataPaxMappings.Count == 0)
                                        {
                                            var ventrataPaxMapping = _masterCacheManager.GetVentrataPaxMappings();
                                            if (ventrataPaxMapping?.Any() != true)
                                            {
                                                ventrataPaxMapping = GetPaxMappingsForVentrataAPI(APIType.Ventrata);
                                            }

                                            selectedProduct.VentrataPaxMappings = ventrataPaxMapping.Where(x => x.ServiceOptionId == selectedOption.ServiceOptionId)?.ToList();
                                        }

                                        var isangoPaxtype = selectedProduct.VentrataPaxMappings?.Where(x => x.AgeGroupCode == unitItemInBooking.UnitId)?.FirstOrDefault()?.PassengerType ?? PassengerType.Undefined;

                                        //ConstantForVentrata.CrossMapper_PassengerType_Isango_VentrataString.TryGetValue(unitItemInBooking.UnitId, out var isangoPaxtype);
                                        apiTicketDetailsAtPaxLevel.PassengerType = isangoPaxtype.ToString();
                                    }
                                    //Todo Tocheck
                                    apiTicketDetailsAtPaxLevel.APIOrderId = selectedProduct.Uuid;
                                }
                            }
                            catch (Exception z)
                            {
                                if (isQrAvailable)
                                {
                                    apiTicketDetailsAtPaxLevel.CodeValue = apiTicketDetailsAtPaxLevel.BarCode = unitItemInBooking.TicketPerUnitItem.DeliveryOptions.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.QRCodeDeliveryFormat)).DeliveryValue;
                                    apiTicketDetailsAtPaxLevel.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.StringTypeForQRCode,APIType.Ventrata);
                                    apiTicketDetailsAtPaxLevel.CodeType = ConstantForVentrata.QRCodeDeliveryFormat;
                                    try
                                    {
                                        ConstantForVentrata.CrossMapper_PassengerType_Isango_VentrataString.TryGetValue(unitItemInBooking?.Unit?.Title?.ToLower(), out var isangoPaxtype);
                                        apiTicketDetailsAtPaxLevel.PassengerType = isangoPaxtype.ToString();
                                    }
                                    catch (Exception e)
                                    {
                                        if (selectedProduct.VentrataPaxMappings == null || selectedProduct.VentrataPaxMappings.Count == 0)
                                        {
                                            var ventrataPaxMapping = _masterCacheManager.GetVentrataPaxMappings();
                                            if (ventrataPaxMapping?.Any() != true)
                                            {
                                                ventrataPaxMapping = GetPaxMappingsForVentrataAPI(APIType.Ventrata);
                                            }

                                            selectedProduct.VentrataPaxMappings = ventrataPaxMapping.Where(x => x.ServiceOptionId == selectedOption.ServiceOptionId)?.ToList();
                                        }

                                        var isangoPaxtype = selectedProduct.VentrataPaxMappings?.Where(x => x.AgeGroupCode == unitItemInBooking.UnitId)?.FirstOrDefault()?.PassengerType ?? PassengerType.Undefined;
                                        //ConstantForVentrata.CrossMapper_PassengerType_Isango_VentrataString.TryGetValue(unitItemInBooking.UnitId, out var isangoPaxtype);
                                        apiTicketDetailsAtPaxLevel.PassengerType = isangoPaxtype.ToString();
                                    }
                                    //Todo Tocheck
                                    apiTicketDetailsAtPaxLevel.APIOrderId = selectedProduct.Uuid;
                                }
                                else if (isVoucherAvailable)
                                {
                                    apiTicketDetailsAtPaxLevel.BarCode = unitItemInBooking.TicketPerUnitItem.DeliveryOptions.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.PDFDeliveryFormat)).DeliveryValue;
                                    apiTicketDetailsAtPaxLevel.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.LinkTypeForPDF,APIType.Ventrata);
                                    apiTicketDetailsAtPaxLevel.CodeType = ConstantForVentrata.LinkTypeForPDF;
                                    try
                                    {
                                        ConstantForVentrata.CrossMapper_PassengerType_Isango_VentrataString.TryGetValue(unitItemInBooking?.Unit?.Title?.ToLower(), out var isangoPaxtype);
                                        apiTicketDetailsAtPaxLevel.PassengerType = isangoPaxtype.ToString();
                                    }
                                    catch (Exception e)
                                    {
                                        if (selectedProduct.VentrataPaxMappings == null || selectedProduct.VentrataPaxMappings.Count == 0)
                                        {
                                            var ventrataPaxMapping = _masterCacheManager.GetVentrataPaxMappings();
                                            if (ventrataPaxMapping?.Any() != true)
                                            {
                                                ventrataPaxMapping = GetPaxMappingsForVentrataAPI(APIType.Ventrata);
                                            }

                                            selectedProduct.VentrataPaxMappings = ventrataPaxMapping.Where(x => x.ServiceOptionId == selectedOption.ServiceOptionId)?.ToList();
                                        }

                                        var isangoPaxtype = selectedProduct.VentrataPaxMappings?.Where(x => x.AgeGroupCode == unitItemInBooking.UnitId)?.FirstOrDefault()?.PassengerType ?? PassengerType.Undefined;
                                        //ConstantForVentrata.CrossMapper_PassengerType_Isango_VentrataString.TryGetValue(unitItemInBooking.UnitId, out var isangoPaxtype);
                                        apiTicketDetailsAtPaxLevel.PassengerType = isangoPaxtype.ToString();
                                    }
                                    //Todo Tocheck
                                    apiTicketDetailsAtPaxLevel.APIOrderId = selectedProduct.Uuid;
                                }

                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = "SupplierBookingService",
                                    MethodName = "MapProductForVentrata",
                                    Token = "Ventrata_PDF_Error",
                                    Params = $"{SerializeDeSerializeHelper.Serialize(selectedProduct)}"
                                };
                                _log.Error(isangoErrorEntity, z);
                            }

                            apiExtraDetail.APITicketDetails.Add(apiTicketDetailsAtPaxLevel);
                        }
                    }
                }
            }
            else
            {
                var isVoucherAvailable = ventrataBookingDetails?.VoucherAtBookingLevel?.DeliveryOptions?.Any(thisDelOpt => thisDelOpt.DeliveryFormat.Equals(ConstantForVentrata.PDFDeliveryFormat));

                var isQrAvailable = ventrataBookingDetails?.VoucherAtBookingLevel?.DeliveryOptions?.Any(thisDelOpt => thisDelOpt.DeliveryFormat.Equals(ConstantForVentrata.QRCodeDeliveryFormat));

                try
                {
                    if ((!bookedProduct?.IsShowSupplierVoucher ?? false) && (isQrAvailable ?? false))
                    {
                        apiExtraDetail.IsQRCodePerPax = false;
                        apiExtraDetail.QRCode = ventrataBookingDetails?.VoucherAtBookingLevel?.DeliveryOptions.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.QRCodeDeliveryFormat)).DeliveryValue;
                        apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.StringTypeForQRCode,APIType.Ventrata);
                    }
                    else if (isVoucherAvailable ?? false)
                    {
                        apiExtraDetail.IsQRCodePerPax = false;
                        apiExtraDetail.QRCode = ventrataBookingDetails?.VoucherAtBookingLevel?.DeliveryOptions.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.PDFDeliveryFormat)).DeliveryValue;
                        apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.LinkTypeForPDF,APIType.Ventrata);
                    }
                    else if (isQrAvailable ?? false)
                    {
                        apiExtraDetail.IsQRCodePerPax = false;
                        apiExtraDetail.QRCode = ventrataBookingDetails?.VoucherAtBookingLevel?.DeliveryOptions.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.QRCodeDeliveryFormat)).DeliveryValue;
                        apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.StringTypeForQRCode,APIType.Ventrata);
                    }
                }
                catch (Exception e)
                {
                    if (isQrAvailable ?? false)
                    {
                        apiExtraDetail.IsQRCodePerPax = false;
                        apiExtraDetail.QRCode = ventrataBookingDetails?.VoucherAtBookingLevel?.DeliveryOptions.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.QRCodeDeliveryFormat)).DeliveryValue;
                        apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.StringTypeForQRCode,APIType.Ventrata);
                    }
                    else if (isVoucherAvailable ?? false)
                    {
                        apiExtraDetail.IsQRCodePerPax = false;
                        apiExtraDetail.QRCode = ventrataBookingDetails?.VoucherAtBookingLevel?.DeliveryOptions.Find(thisOpt => thisOpt.DeliveryFormat.Equals(ConstantForVentrata.PDFDeliveryFormat)).DeliveryValue;
                        apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(ConstantForVentrata.LinkTypeForPDF,APIType.Ventrata);
                    }

                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "SupplierBookingService",
                        MethodName = "MapProductForVentrata",
                        Token = "Ventrata_PDF_Error",
                        Params = $"{SerializeDeSerializeHelper.Serialize(selectedProduct)}"
                    };
                    _log.Error(isangoErrorEntity, e);
                }
            }

            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct.SupplierReference, selectedOption?.AvailabilityStatus);

            return bookedProduct;
        }

        private BookedProduct MapProductForFareharbor(BookedProduct bookedProduct, FareHarborSelectedProduct selectedProduct)
        {
            var apiExtraDetail = bookedProduct.APIExtraDetail ?? new ApiExtraDetail();
            var selectedOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);

            apiExtraDetail.SupplieReferenceNumber = selectedProduct.UuId;
            apiExtraDetail.APIOptionName = selectedOption?.Name;
            apiExtraDetail.PickUpId = selectedProduct.HotelPickUpLocation;
            apiExtraDetail.SupplierCancellationPolicy = selectedProduct.SupplierCancellationPolicy;
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct.UuId, selectedOption?.AvailabilityStatus);
            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.PickUpLocation = selectedProduct.HotelPickUpLocation;

            return bookedProduct;
        }

        private BookedProduct MapProductForBokun(BookedProduct bookedProduct, BokunSelectedProduct selectedProduct)
        {
            var apiExtraDetail = bookedProduct.APIExtraDetail ?? new ApiExtraDetail();
            apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
            var selectedOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);

            apiExtraDetail.SupplieReferenceNumber = selectedProduct.ConfirmationCode;
            //apiExtraDetail.SupplierLineNumber =
            apiExtraDetail.APIOptionName = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected)?.Name;
            apiExtraDetail.PickUpId = selectedProduct.HotelPickUpLocation;
            apiExtraDetail.QRCode = selectedProduct.QrCode;
            apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(Constant.String,APIType.Bokun);
            apiExtraDetail.SupplierCancellationPolicy = selectedProduct.SupplierCancellationPolicy;

            try
            {
                var noOfBarcodeCodes = selectedProduct?.Barcodes?.Count();
                var isQRCodePerPax = noOfBarcodeCodes > 1;
                apiExtraDetail.IsQRCodePerPax = isQRCodePerPax;
                var barCodeString = GetBarCodeStringForRezdy(selectedProduct.Barcodes);
                if (noOfBarcodeCodes > 1)
                {
                    foreach (var barcodeField in selectedProduct.Barcodes)
                    {
                        var apiTicketDetail = new ApiTicketDetail
                        {
                            BarCode = barcodeField.BarCode,
                            CodeType = barcodeField.BarcodeType,
                            PassengerType = barcodeField.PassengerType,
                            APIAgeGroupCode = barcodeField.PricingCategoryId
                        };
                        apiExtraDetail.APITicketDetails.Add(apiTicketDetail);
                    }
                }
                apiExtraDetail.QRCode = noOfBarcodeCodes == 1 ? $"{Isango.Mailer.Constants.Constant.QRCode}~{barCodeString}" : string.Empty;
            }
            catch (Exception ex)
            {
                //ignore
            }

            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct.ConfirmationCode, selectedOption?.AvailabilityStatus);
            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.CancellationPolicy = selectedOption.CancellationPrices;

            return bookedProduct;
        }

        private BookedProduct MapProductForAOT(BookedProduct bookedProduct, AotSelectedProduct selectedProduct, string referenceNumber)
        {
            var apiExtraDetail = bookedProduct.APIExtraDetail ?? new ApiExtraDetail();
            var selectedOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
            selectedProduct.CancellationPolicy = selectedOption.CancellationText;
            apiExtraDetail.SupplieReferenceNumber = referenceNumber;
            apiExtraDetail.SupplierLineNumber = selectedProduct.ServiceLineId;
            apiExtraDetail.APIOptionName = selectedOption?.Name;
            apiExtraDetail.SupplierCancellationPolicy = selectedOption.ApiCancellationPolicy;
            bookedProduct.CancellationPolicy = selectedOption?.CancellationPrices;
            bookedProduct.OptionStatus = GetBookingStatusNumber(referenceNumber, selectedOption?.AvailabilityStatus);
            bookedProduct.APIExtraDetail = apiExtraDetail;

            return bookedProduct;
        }

        private BookedProduct MapProductForGoldenTours(BookedProduct bookedProduct, GoldenToursSelectedProduct selectedProduct)
        {
            var apiExtraDetail = bookedProduct.APIExtraDetail ?? new ApiExtraDetail();
            var selectedOption = (ActivityOption)selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);

            apiExtraDetail.SupplieReferenceNumber = selectedProduct.TicketReferenceNumber;
            apiExtraDetail.APIOptionName = selectedOption?.Name;
            apiExtraDetail.PickUpId = selectedProduct.HotelPickUpLocation;

            // Passing TicketPdfUrl in the QrCode field and Setting QrCodeType as TicketPdfType
            apiExtraDetail.QRCode = selectedProduct.TicketUrl;
            //apiExtraDetail.QRCodeType = string.IsNullOrWhiteSpace(apiExtraDetail.QRCode) ? "" : Constant.TicketPdfType;
            apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(string.IsNullOrWhiteSpace(apiExtraDetail.QRCode) ? Constant.String : Constant.TicketPdfType, APIType.Goldentours);//LinkMapProductForRedeam
            // Looping on the QrCodes and setting them in the ApiTicketDetail
            if (selectedProduct.QrCodes != null)
            {
                apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
                foreach (var qrCode in selectedProduct.QrCodes)
                {
                    var apiTicketDetail = new ApiTicketDetail
                    {
                        BarCode = qrCode
                    };
                    apiExtraDetail.APITicketDetails.Add(apiTicketDetail);
                }
            }

            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct.TicketReferenceNumber, selectedOption?.AvailabilityStatus);

            return bookedProduct;
        }

        private BookedProduct MapProductForTiqets(BookedProduct bookedProduct, TiqetsSelectedProduct selectedProduct)
        {
            var apiExtraDetail = bookedProduct.APIExtraDetail ?? new ApiExtraDetail();
            var selectedOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);

            apiExtraDetail.SupplieReferenceNumber = selectedProduct.OrderReferenceId;
            apiExtraDetail.APIOptionName = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected)?.Name;

            //Storing 'Link' as a type if pdf url is available
            apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(Constant.TicketPdfType,APIType.Tiqets);

            //Requires proper varification to uncomment following code
            //apiExtraDetail.QRCodeType = !string.IsNullOrWhiteSpace(selectedProduct?.TicketPdfUrl) ?
            //                            Constant.TicketPdfType
            //                            : string.Empty;

            apiExtraDetail.QRCode = selectedProduct.TicketPdfUrl; //This will be used on the confirmation page and email
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct.OrderReferenceId, selectedOption?.AvailabilityStatus);
            apiExtraDetail.SupplierCancellationPolicy = selectedOption.ApiCancellationPolicy;
            bookedProduct.APIExtraDetail = apiExtraDetail;

            return bookedProduct;
        }

        private BookedProduct MapProductForBigBus(BookedProduct bookedProduct, BigBusSelectedProduct selectedProduct)
        {
            bookedProduct.APIExtraDetail = new ApiExtraDetail();
            var apiExtraDetail = bookedProduct.APIExtraDetail;
            var selectedOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
            apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
            apiExtraDetail.SupplierLineNumber = selectedProduct.ShortReference;
            apiExtraDetail.SupplieReferenceNumber = selectedProduct.SupplierReferenceNumber;

            var noOfQRCodes = selectedProduct.BigBusTickets?.Count();
            var isQRCodePerPax = noOfQRCodes > 1;
            apiExtraDetail.IsQRCodePerPax = isQRCodePerPax;

            if (!isQRCodePerPax)
            {
                apiExtraDetail.QRCode = selectedProduct.BigBusTickets?.FirstOrDefault().TicketBarCode;
                apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(Constant.String,APIType.BigBus);
            }

            if (selectedProduct.BigBusTickets != null)
            {
                foreach (var ticket in selectedProduct.BigBusTickets)
                {
                    var apiTicketDetail = new ApiTicketDetail
                    {
                        BarCode = ticket.TicketBarCode,
                        FiscalNumber = ticket.PassengerType,
                        SeatId = ticket.Quantity
                    };
                    apiExtraDetail.APITicketDetails.Add(apiTicketDetail);
                }
            }

            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct.BookingReference, selectedOption?.AvailabilityStatus);

            return bookedProduct;
        }

        private BookedProduct MapProductForTourCMS(BookedProduct bookedProduct, TourCMSSelectedProduct selectedProduct)
        {
            var qrCodeBarCodeForTesting = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("TourCMSQRCodeOrBarCodeTesting"));
            bookedProduct.APIExtraDetail = new ApiExtraDetail();
            var apiExtraDetail = bookedProduct?.APIExtraDetail;
            var selectedOption = selectedProduct?.ProductOptions?.FirstOrDefault(x => x.IsSelected);
            apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
            apiExtraDetail.SupplierLineNumber = selectedProduct?.ShortReference;
            apiExtraDetail.SupplieReferenceNumber = selectedProduct?.SupplierReferenceNumber;
            apiExtraDetail.APIOptionName = selectedOption?.Name;
            var noOfQRCodes = selectedProduct?.TourCMSTicket?.Count();
            var isQRCodePerPax = noOfQRCodes > 1;
            apiExtraDetail.IsQRCodePerPax = isQRCodePerPax;

            if (!isQRCodePerPax)
            {
                apiExtraDetail.QRCode = selectedProduct?.TourCMSTicket?.FirstOrDefault()?.TicketBarCode;

                var barCodeSymbol = selectedProduct?.TourCMSTicket?.FirstOrDefault()?.BarcodeSymbology;
                if (
                    (!string.IsNullOrEmpty(barCodeSymbol) && barCodeSymbol?.ToUpper() == "CODE_128")
                    ||
                    (qrCodeBarCodeForTesting == "2")// For Testing Only
                    )
                {
                    apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode("bar",APIType.TourCMS); //Barcode
                }
                else
                {
                    apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(Constant.String,APIType.TourCMS);//QrCode
                }
            }

            else if (selectedProduct?.TourCMSTicket != null)
            {
                foreach (var ticket in selectedProduct.TourCMSTicket)
                {
                    var apiTicketDetail = new ApiTicketDetail
                    {
                        BarCode = ticket?.TicketBarCode,
                        FiscalNumber = ticket?.PassengerType,
                        SeatId = ticket?.Quantity,
                        PassengerType = ticket?.PassengerType
                    };
                    var barCodeSymbol = ticket?.BarcodeSymbology;
                    if (
                        (!string.IsNullOrEmpty(barCodeSymbol) && barCodeSymbol?.ToUpper() == "CODE_128")
                        ||
                        (qrCodeBarCodeForTesting == "2")// For Testing Only
                        )
                    {
                        apiTicketDetail.CodeType = "bar";//Barcode
                    }
                    else
                    {
                        apiTicketDetail.CodeType = "";//QrCode
                    }
                    apiExtraDetail.APITicketDetails.Add(apiTicketDetail);
                }
            }

            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct?.BookingReference, selectedOption?.AvailabilityStatus);

            return bookedProduct;
        }

        private BookedProduct MapProductForRedeam(BookedProduct bookedProduct, RedeamSelectedProduct selectedProduct)
        {
            var apiExtraDetail = bookedProduct?.APIExtraDetail ?? new ApiExtraDetail();
            var selectedOption = (ActivityOption)selectedProduct?.ProductOptions?.FirstOrDefault(x => x.IsSelected);
            var noOfPassengers = selectedOption?.TravelInfo?.NoOfPassengers?.Values.Sum(x => x);
            var noOfQRCodes = selectedProduct.QrCodes?.Count();
            var isQRCodePerPax = noOfQRCodes > 1;

            apiExtraDetail.SupplieReferenceNumber = selectedProduct?.BookingReferenceNumber;
            apiExtraDetail.APIOptionName = selectedOption?.Name;
            apiExtraDetail.PickUpId = selectedProduct?.HotelPickUpLocation;
            apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(Constant.String,APIType.Redeam);
            apiExtraDetail.IsQRCodePerPax = isQRCodePerPax;
            apiExtraDetail.SupplierCancellationPolicy = selectedOption?.ApiCancellationPolicy;

            //var codeType = selectedProduct?.QrCodes?.FirstOrDefault().Type;
            var codeType = MapAPICodeFormatWithIsangoCode(selectedProduct?.QrCodes?.FirstOrDefault().Type, APIType.Redeam);
            if (!isQRCodePerPax)
            {
                var qrCodeValues = selectedProduct?.QrCodes?.Select(x => x.Value).ToList();
                if (qrCodeValues != null)
                {
                    var qrCodeValue = string.Join(", ", qrCodeValues);
                    var qrCodeString = $"{codeType}~{qrCodeValue}";
                    apiExtraDetail.QRCode = qrCodeString;
                }
            }
            else if (selectedProduct?.QrCodes != null)
            {
                apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
                // Looping on the QrCodes and setting them in the ApiTicketDetail
                foreach (var qrCode in selectedProduct?.QrCodes)
                {
                    var apiTicketDetail = new ApiTicketDetail
                    {
                        BarCode = qrCode?.Value,
                        FiscalNumber = qrCode?.PassengerType,
                        APIOrderId = selectedProduct?.BookingReferenceNumber,
                        QRCodeType = MapAPICodeFormatWithIsangoCode(codeType,APIType.Redeam)
                    };
                    apiExtraDetail?.APITicketDetails.Add(apiTicketDetail);
                }
            }

            bookedProduct.Time = selectedOption?.Time;
            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct?.BookingReferenceNumber, selectedOption?.AvailabilityStatus);

            return bookedProduct;
        }

        private BookedProduct MapProductForRayna(BookedProduct bookedProduct, RaynaSelectedProduct selectedProduct)
        {
            bookedProduct.APIExtraDetail = new ApiExtraDetail();
            var apiExtraDetail = bookedProduct?.APIExtraDetail;
            var selectedOption = selectedProduct?.ProductOptions?.FirstOrDefault(x => x.IsSelected);
            apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();

            if (!string.IsNullOrEmpty(selectedProduct?.TicketPdfUrl))
            {
                apiExtraDetail.SupplierLineNumber = selectedProduct.BookingId;
                apiExtraDetail.SupplieReferenceNumber = selectedProduct.OrderReferenceId;
                apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(Constant.TicketPdfType,APIType.Rayna);
                apiExtraDetail.QRCode = selectedProduct.TicketPdfUrl;
            }
            else
            {
                apiExtraDetail.SupplierLineNumber = selectedProduct.BookingId;
                apiExtraDetail.SupplieReferenceNumber = selectedProduct.OrderReferenceId;
                apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(Constant.String,APIType.Rayna);
                apiExtraDetail.QRCode = selectedProduct.OrderReferenceId;
                apiExtraDetail.APIOptionName = selectedOption?.Name;
            }
            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct.OrderReferenceId, selectedOption?.AvailabilityStatus);
            return bookedProduct;
        }

        private string GetBookingStatusNumber(string referenceNumber, AvailabilityStatus? availabilityStatus)
        {
            return ((int)GetOptionBookingStatus(referenceNumber, availabilityStatus)).ToString();
        }

        private OptionBookingStatus GetOptionBookingStatus(string referenceNumber, AvailabilityStatus? availabilityStatus)
        {
            var status = OptionBookingStatus.Failed;
            if (!string.IsNullOrWhiteSpace(referenceNumber) && availabilityStatus != null)
            {
                if (availabilityStatus.Equals(AvailabilityStatus.ONREQUEST))
                    status = OptionBookingStatus.Requested;
                else if (availabilityStatus.Equals(AvailabilityStatus.AVAILABLE))
                    status = OptionBookingStatus.Confirmed;
            }
            return status;
        }

        private List<ProductOption> UpdateOptionStatus(List<ProductOption> productOptions, string referenceNumber)
        {
            if (productOptions == null) return productOptions;
            foreach (var productOption in productOptions)
            {
                productOption.BookingStatus = GetOptionBookingStatus(referenceNumber, productOption.AvailabilityStatus);
            }
            return productOptions;
        }

        private List<BookedProduct> CreateAotBookedProducts(List<SelectedProduct> selectedProducts, List<BookedProduct> isangoBookedProducts, string bookingReferenceNumber, CountryType countryType)
        {
            var bookedProducts = new List<BookedProduct>();
            foreach (var selectedProduct in selectedProducts)
            {
                selectedProduct.ProductOptions = UpdateOptionStatus(selectedProduct.ProductOptions,
                    bookingReferenceNumber);

                var bookedProduct = isangoBookedProducts.FirstOrDefault(x =>
                    x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);
                if (bookedProduct == null) continue;
                bookedProduct = MapProductForAOT(bookedProduct, selectedProduct as AotSelectedProduct, bookingReferenceNumber);
                bookedProduct.CountryCode = countryType.ToString();
                bookedProducts.Add(bookedProduct);
            }
            return bookedProducts;
        }

        private void LogPurchaseXmlForAot(Booking booking, string request, string response, string bookingReferenceNumber)
        {
            var logPurchase = new LogPurchaseXmlCriteria
            {
                BookingId = booking.BookingId,
                APIType = APIType.Aot,
                BookingReferenceNumber = booking.ReferenceNumber,
                RequestXml = request,
                ResponseXml = response,
                Status = string.IsNullOrWhiteSpace(bookingReferenceNumber)
                    ? Constant.FailedStatus
                    : Constant.SuccessStatus,
                ApiRefNumber = bookingReferenceNumber
            };
            _supplierBookingPersistence.LogPurchaseXML(logPurchase);
        }

        private List<BookedProduct> UpdateOptionStatusForAotProduct(List<SelectedProduct> selectedProducts,
            List<BookedProduct> isangoBookedProducts, string bookingReferenceNumber)
        {
            var bookedProducts = new List<BookedProduct>();
            foreach (var product in selectedProducts)
            {
                var option = product.ProductOptions.FirstOrDefault(x => x.IsSelected);
                var bookedProduct = isangoBookedProducts.FirstOrDefault(x =>
                    x.AvailabilityReferenceId.Equals(product.AvailabilityReferenceId));
                if (bookedProduct == null) continue;

                bookedProduct.OptionStatus = GetBookingStatusNumber(bookingReferenceNumber, option?.AvailabilityStatus);
                if (bookedProduct.APIExtraDetail == null)
                    bookedProduct.APIExtraDetail = new ApiExtraDetail();
                bookedProduct.APIExtraDetail.SupplieReferenceNumber = bookingReferenceNumber;
                bookedProducts.Add(bookedProduct);
            }
            return bookedProducts;
        }

        private string GetBarCodeString(PrioApi prioApi)
        {
            var groupCode = prioApi?.BookingDetails?.FirstOrDefault()?.GroupCode;
            var concatenatedCodes = groupCode;
            prioApi?.BookingDetails?.ToList().ForEach(Item =>
            {
                var distinctCodes = Item?.TicketDetails?.Where(ItemTkt => ItemTkt.TicketCode != groupCode).Select(ItemTkt1 => ItemTkt1.TicketCode).Distinct();
                var CombinedCodes = (distinctCodes != null && distinctCodes.Any()) ? string.Join(", ", distinctCodes) : "";
                if (distinctCodes != null)
                {
                    if (!distinctCodes.Contains(groupCode) && !String.IsNullOrEmpty(CombinedCodes))
                        concatenatedCodes += ", " + CombinedCodes;
                }
            });
            return concatenatedCodes;
        }
        private string GetBarCodeStringPrioHub(PrioHubAPITicket prioApi)
        {
            var groupCode = prioApi?.BookingDetails?.FirstOrDefault()?.GroupCode;
            var concatenatedCodes = groupCode;
            prioApi?.BookingDetails?.ToList().ForEach(Item =>
            {
                var distinctCodes = Item?.TicketDetails?.Where(ItemTkt => ItemTkt.TicketCode != groupCode).Select(ItemTkt1 => ItemTkt1.TicketCode).Distinct();
                var CombinedCodes = (distinctCodes != null && distinctCodes.Any()) ? string.Join(", ", distinctCodes) : "";
                if (distinctCodes != null)
                {
                    if (!distinctCodes.Contains(groupCode) && !String.IsNullOrEmpty(CombinedCodes))
                        concatenatedCodes += ", " + CombinedCodes;
                }
            });
            return concatenatedCodes;
        }

        #endregion Response Mapper

        #region Supplier API booking Methods

        /// <summary>
        /// Create Tiqets Booking
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<BookedProduct> CreateTiqetsBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var selectedProducts = criteria?.Booking?.SelectedProducts?.Where(product => product.APIType.Equals(APIType.Tiqets)).ToList();
            var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
            var affiliateId = criteria?.Booking?.Affiliate?.Id;
            var request = string.Empty;
            var response = string.Empty;
            var apiHttpStatusCode = HttpStatusCode.BadGateway;
            try
            {
                if (selectedProducts != null && selectedProducts.Count > 0)
                {
                    var logPurchase = new LogPurchaseXmlCriteria
                    {
                        BookingId = criteria.Booking.BookingId,
                        APIType = APIType.Tiqets,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber
                    };

                    var bookingRequest = new Entities.Tiqets.BookingRequest
                    {
                        LanguageCode = criteria.Booking.Language.Code.ToLowerInvariant(),
                        IsangoBookingReference = criteria.Booking.ReferenceNumber,
                        AffiliateId = affiliateId,
                        TiquetsLanguageCode = criteria.Booking.TiquetsLanguageCode.ToLowerInvariant()
                    };
                    var createOrderProducts = new Dictionary<string, CreateOrderResponse>();

                    var tiqetsPaxMapping = _tiqetsPaxMappingCacheManager.GetPaxMappings();
                    if (tiqetsPaxMapping?.Any() != true)
                    {
                        tiqetsPaxMapping = GetPaxMappingsForAPI(APIType.Tiqets);
                    }
                    var optionCounter = 1;
                    foreach (var tiqetsSelectedProduct in selectedProducts.OfType<TiqetsSelectedProduct>())
                    {
                        try
                        {
                            var selectedOption = (ActivityOption)tiqetsSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);

                            var Product_ID = (selectedOption)?.VentrataProductId;

                            var TiqetsPackagesOrNot = GetTiqetsPackages(Product_ID);
                            if (TiqetsPackagesOrNot != null && TiqetsPackagesOrNot.Count() > 0)
                            {
                                bookingRequest.PackageId = TiqetsPackagesOrNot.ToList();
                            }
                            bookingRequest.IsangoBookingReference = $"{criteria.Booking.ReferenceNumber}-{optionCounter}";
                            optionCounter++;

                            //Get Pax Mappings from cache
                            var paxMappings = tiqetsPaxMapping?.Where(x => x.ServiceOptionId == selectedOption?.ServiceOptionId)?.ToList();

                            tiqetsSelectedProduct.Variants = GetVariants(paxMappings, selectedOption?.TravelInfo.NoOfPassengers);

                            #region CreateOrder API Call

                            bookingRequest.RequestObject = tiqetsSelectedProduct;
                            //var createOrderResponse = _tiqetsAdapter.CreateOrder(bookingRequest, criteria.Token, out request, out response);

                            var rowKey = $"{criteria.Booking.ReferenceNumber}-{tiqetsSelectedProduct.AvailabilityReferenceId}";
                            var ReservationDetails = _tableStorageOperation.RetrieveReservationData(rowKey);

                            var createOrderResponse = SerializeDeSerializeHelper.DeSerialize<CreateOrderResponse>(ReservationDetails?.ReservationResponse);

                            //#todo abhishek verify 
                            if (ReservationDetails == null)
                            {
                                createOrderResponse = _tiqetsAdapter.CreateOrder(bookingRequest, criteria.Token, out request, out response, out apiHttpStatusCode);
                            }
                            #endregion CreateOrder API Call

                            if (createOrderResponse != null && createOrderResponse.Success)
                            {
                                //InsertLogPurchaseInDb(logPurchase, request, response, Constant.CreateOrder, Constant.StatusSuccess);

                                createOrderProducts.Add(tiqetsSelectedProduct.AvailabilityReferenceId, createOrderResponse);
                            }
                            else
                            {
                                //InsertLogPurchaseInDb(logPurchase, request, response, Constant.CreateOrder, Constant.StatusFailed);

                                //If the create order call is failed for any of the product then set all product booking status as failed
                                bookedProducts = SetFailedBookingStatus(selectedProducts, criteria);

                                var createOrderSerializedResponse = SerializeDeSerializeHelper.DeSerialize<CreateOrderResponse>(Convert.ToString(response));

                                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , apiHttpStatusCode
                                , createOrderSerializedResponse?.Message);

                                try
                                {
                                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, tiqetsSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                        createOrderResponse?.OrderReferenceId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                        Convert.ToInt32(APIType.Tiqets), tiqetsSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                        tiqetsSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, tiqetsSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                    criteria?.Booking?.UpdateDBLogFlag();
                                }
                                catch (Exception e)
                                {
                                    //ignore
                                }

                                return bookedProducts;
                            }
                        }
                        catch (Exception ex)
                        {
                            //If the create order call is failed for any of the product then set all product booking status as failed
                            bookedProducts = SetFailedBookingStatus(selectedProducts, criteria);

                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "SupplierBookingService",
                                MethodName = "CreateTiqetsBooking",
                                Token = criteria.Token,
                                Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                            };

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.BookingError
                                , apiHttpStatusCode
                                , $"Exception\n {ex.Message}\n{response}");
                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, bookedProducts?.FirstOrDefault()?.ServiceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.Tiqets), bookedProducts?.FirstOrDefault()?.OptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    bookedProducts?.FirstOrDefault()?.OptionName ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, bookedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            _log.Error(isangoErrorEntity, ex);
                            return bookedProducts;
                        }
                    }

                    var apiHttpStatusCodeData = HttpStatusCode.BadGateway;
                    //Confirm Booking after creating order for all products
                    foreach (var product in createOrderProducts)
                    {
                        var tiqetsSelectedProduct = selectedProducts.OfType<TiqetsSelectedProduct>().FirstOrDefault(x => x.AvailabilityReferenceId == product.Key);

                        try
                        {
                            #region ConfirmOrder API Call

                            bookingRequest.IsangoBookingReference = product.Value?.OrderReferenceId ?? bookingRequest.IsangoBookingReference;
                            bookingRequest.RequestObject = product.Value;
                            var confirmOrderResponse = _tiqetsAdapter.ConfirmOrder(bookingRequest, criteria.Token, out var confirmOrderReq, out var confirmOrderRes, out apiHttpStatusCodeData);

                            request = confirmOrderReq;
                            response = confirmOrderRes;

                            var confirmOrderStatus = confirmOrderResponse != null ? Constant.StatusSuccess : Constant.StatusFailed;
                            InsertLogPurchaseInDb(logPurchase, confirmOrderReq, confirmOrderRes, Constant.ConfirmOrder, confirmOrderStatus);

                            #endregion ConfirmOrder API Call

                            if (confirmOrderResponse != null)
                            {
                                #region GetTicket API Call

                                bookingRequest.RequestObject = confirmOrderResponse;

                                //Below code wait for 3 seconds, so that GetTicket method not give pending status.
                                var tiqetTicketDelay = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsTicketDelay));
                                if (tiqetTicketDelay > 0)
                                {
                                    Thread.Sleep(tiqetTicketDelay * 1000);//milliseconds-> n seconds by config
                                }
                                else
                                {
                                    Thread.Sleep(5000);// milliseconds->3seconds by default
                                }

                                var getTicketResponse = _tiqetsAdapter.GetTicket(bookingRequest, criteria.Token, out var getTicketReq, out var getTicketRes, out apiHttpStatusCodeData);

                                var getTicketStatus = getTicketResponse != null ? Constant.StatusSuccess : Constant.StatusFailed;
                                InsertLogPurchaseInDb(logPurchase, getTicketReq, getTicketRes, Constant.GetTicket, getTicketStatus);

                                #endregion GetTicket API Call

                                var bookedSelectedProduct = getTicketResponse?.SelectedProducts?.OfType<TiqetsSelectedProduct>()?.FirstOrDefault();

                                var isStatusPending = bookedSelectedProduct?.OrderStatus == TiqetsOrderStatus.Processing ||
                                                       bookedSelectedProduct?.OrderStatus == TiqetsOrderStatus.Pending ||
                                                       bookedSelectedProduct?.OrderStatus == TiqetsOrderStatus.New;

                                if (isStatusPending)
                                {
                                    tiqetsSelectedProduct.ProductOptions.ForEach(x =>
                                    {
                                        x.AvailabilityStatus = AvailabilityStatus.ONREQUEST;
                                    });

                                    try
                                    {
                                        var retryThreshold = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsRetryThreshold));
                                        InsertAsyncJob(confirmOrderResponse.OrderReferenceId, bookingRequest?.LanguageCode, criteria.Token,
                                            criteria, tiqetsSelectedProduct, bookingRequest?.AffiliateId, Convert.ToInt32(APIType.Tiqets), "booking", string.Empty, retryThreshold);
                                    }
                                    catch (Exception ex)
                                    {
                                        var isangoErrorEntity = new IsangoErrorEntity
                                        {
                                            ClassName = "SupplierBookingService",
                                            MethodName = "InsertAsyncJob",
                                            Token = criteria.Token,
                                            Params = $"{tiqetsSelectedProduct.Id}|{SerializeDeSerializeHelper.Serialize(tiqetsSelectedProduct)}"
                                        };
                                        _log.Error(isangoErrorEntity, ex);
                                    }
                                }

                                tiqetsSelectedProduct.OrderStatus = bookedSelectedProduct?.OrderStatus ?? TiqetsOrderStatus.Done;
                                tiqetsSelectedProduct.TicketPdfUrl = bookedSelectedProduct?.TicketPdfUrl;
                                tiqetsSelectedProduct.Success = true;
                                tiqetsSelectedProduct.OrderReferenceId = confirmOrderResponse.OrderReferenceId;
                            }
                            else
                            {
                                var createOrderSerializedResponse = SerializeDeSerializeHelper.DeSerialize<ConfirmOrderResponse>(Convert.ToString(response));
                                //Api booking failed
                                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                        , apiHttpStatusCodeData
                                        , createOrderSerializedResponse?.Message);
                                try
                                {
                                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, tiqetsSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    tiqetsSelectedProduct?.OrderReferenceId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.Tiqets), tiqetsSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    tiqetsSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, tiqetsSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                    criteria?.Booking?.UpdateDBLogFlag();
                                }
                                catch (Exception ex)
                                {
                                    //ignore
                                }
                            }

                            tiqetsSelectedProduct.ProductOptions = UpdateOptionStatus(tiqetsSelectedProduct.ProductOptions, confirmOrderResponse?.OrderReferenceId);

                            var bookedProduct = criteria.Booking.IsangoBookingData?.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == tiqetsSelectedProduct.AvailabilityReferenceId);

                            if (bookedProduct == null) continue;

                            bookedProduct = MapProductForTiqets(bookedProduct, tiqetsSelectedProduct);
                            bookedProducts.Add(bookedProduct);
                        }
                        catch (Exception ex)
                        {
                            tiqetsSelectedProduct.ProductOptions = UpdateOptionStatus(tiqetsSelectedProduct.ProductOptions, tiqetsSelectedProduct.OrderReferenceId);
                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == tiqetsSelectedProduct.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;

                            var option = tiqetsSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                            bookedProduct.OptionStatus = GetBookingStatusNumber(tiqetsSelectedProduct.OrderReferenceId, option.AvailabilityStatus);
                            if (!string.IsNullOrWhiteSpace(tiqetsSelectedProduct.OrderReferenceId))
                            {
                                if (bookedProduct.APIExtraDetail == null)
                                    bookedProduct.APIExtraDetail = new ApiExtraDetail();
                                bookedProduct.APIExtraDetail.SupplieReferenceNumber = tiqetsSelectedProduct.OrderReferenceId;
                            }

                            bookedProducts.Add(bookedProduct);
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "SupplierBookingService",
                                MethodName = "CreateTiqetsBooking",
                                Token = criteria.Token,
                                Params = $"{tiqetsSelectedProduct.Id}|{SerializeDeSerializeHelper.Serialize(tiqetsSelectedProduct)}"
                            };

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.BookingError
                                , apiHttpStatusCodeData
                                , $"Exception\n {ex.Message}\n{response}");
                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, tiqetsSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                tiqetsSelectedProduct?.OrderReferenceId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.Tiqets), tiqetsSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                tiqetsSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, tiqetsSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (selectedProducts == null || !selectedProducts.Any()) return bookedProducts;
                selectedProducts.ForEach(product =>
                {
                    var bookedProduct = isangoBookedProducts?.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(product.AvailabilityReferenceId));
                    if (bookedProduct != null)
                    {
                        bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                        bookedProducts.Add(bookedProduct);
                    }
                });
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateTiqetsBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }

        public List<BookedProduct> CreateTiqetsBookingReservation(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var selectedProducts = criteria?.Booking?.SelectedProducts?.Where(product => product.APIType.Equals(APIType.Tiqets)).ToList();
            var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
            var request = string.Empty;
            var response = string.Empty;
            var apiResponseStatus = HttpStatusCode.BadGateway;
            try
            {
                if (selectedProducts != null && selectedProducts.Count > 0)
                {
                    var logPurchase = new LogPurchaseXmlCriteria
                    {
                        BookingId = criteria.Booking.BookingId,
                        APIType = APIType.Tiqets,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber
                    };

                    var bookingRequest = new Entities.Tiqets.BookingRequest
                    {
                        LanguageCode = criteria.Booking.Language.Code.ToLowerInvariant(),
                        IsangoBookingReference = criteria.Booking.ReferenceNumber,
                        TiquetsLanguageCode = criteria.Booking.TiquetsLanguageCode.ToLowerInvariant()
                    };
                    var createOrderProducts = new Dictionary<string, CreateOrderResponse>();

                    var tiqetsPaxMapping = _tiqetsPaxMappingCacheManager.GetPaxMappings();
                    if (tiqetsPaxMapping?.Any() != true)
                    {
                        tiqetsPaxMapping = GetPaxMappingsForAPI(APIType.Tiqets);
                    }
                    var optionCounter = 1;
                    foreach (var tiqetsSelectedProduct in selectedProducts.OfType<TiqetsSelectedProduct>())
                    {
                        try
                        {
                            try
                            {
                                _supplierBookingPersistence.InsertReserveRequest(criteria.Token, tiqetsSelectedProduct.AvailabilityReferenceId);
                            }
                            catch (Exception ex)
                            {
                                //ignore
                            }
                            var selectedOption = (ActivityOption)tiqetsSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                            bookingRequest.IsangoBookingReference = $"{criteria.Booking.ReferenceNumber}-{optionCounter}";
                            optionCounter++;

                            //Get Pax Mappings from cache
                            var paxMappings = tiqetsPaxMapping?.Where(x => x.ServiceOptionId == selectedOption?.ServiceOptionId)?.ToList();

                            tiqetsSelectedProduct.Variants = GetVariants(paxMappings, selectedOption?.TravelInfo.NoOfPassengers);

                            #region CreateOrder API Call

                            var Product_ID = (selectedOption)?.VentrataProductId;
                            var TiqetsPackagesOrNot = GetTiqetsPackages(Product_ID);
                            if (TiqetsPackagesOrNot != null && TiqetsPackagesOrNot.Count() >= 0)
                            {
                                bookingRequest.PackageId = TiqetsPackagesOrNot.ToList();
                            }


                            bookingRequest.RequestObject = tiqetsSelectedProduct;
                            var createOrderResponse = _tiqetsAdapter.CreateOrder(bookingRequest, criteria.Token, out request, out response, out apiResponseStatus);

                            var reservationDetails = new SupplierBookingReservationResponse()
                            {
                                ApiType = Convert.ToInt32(APIType.Tiqets),
                                ServiceOptionId = selectedOption?.ServiceOptionId ?? selectedOption?.BundleOptionID ?? 0,
                                AvailabilityReferenceId = tiqetsSelectedProduct.AvailabilityReferenceId,
                                Status = createOrderResponse != null ? (createOrderResponse.Success ? Constant.StatusSuccess : Constant.StatusFailed) : Constant.StatusFailed,
                                BookedOptionId = criteria.Booking.BookingId,
                                BookingReferenceNo = criteria.Booking.ReferenceNumber,
                                OptionName = selectedOption?.Name ?? selectedOption?.BundleOptionName,
                                ReservationResponse = response,
                                ReservationReferenceId = createOrderResponse?.OrderReferenceId,
                                Token = criteria.Token
                            };

                            _tableStorageOperation.InsertReservationDetails(reservationDetails);

                            #endregion CreateOrder API Call

                            if (createOrderResponse != null && createOrderResponse.Success)
                            {
                                InsertLogPurchaseInDb(logPurchase, request, response, Constant.CreateOrder, Constant.StatusSuccess);

                                try
                                {
                                    _supplierBookingPersistence.UpdateReserveRequest(criteria.Token, tiqetsSelectedProduct.AvailabilityReferenceId, criteria.Booking.ReferenceNumber);
                                }
                                catch (Exception ex)
                                {
                                    //ignore
                                }

                                createOrderProducts.Add(tiqetsSelectedProduct.AvailabilityReferenceId, createOrderResponse);
                            }
                            else
                            {
                                InsertLogPurchaseInDb(logPurchase, request, response, Constant.CreateOrder, Constant.StatusFailed);

                                //If the create order call is failed for any of the product then set all product booking status as failed
                                bookedProducts = SetFailedBookingStatus(selectedProducts, criteria);

                                var createOrderSerializedResponse = SerializeDeSerializeHelper.DeSerialize<CreateOrderResponse>(Convert.ToString(response));

                                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , apiResponseStatus
                                , createOrderSerializedResponse.Message);

                                try
                                {
                                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, tiqetsSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                        createOrderResponse?.OrderReferenceId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                        Convert.ToInt32(APIType.Tiqets), tiqetsSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                        tiqetsSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, tiqetsSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                    criteria?.Booking?.UpdateDBLogFlag();
                                }
                                catch (Exception e)
                                {
                                    //ignore
                                }

                                return bookedProducts;
                            }
                        }
                        catch (Exception ex)
                        {
                            //If the create order call is failed for any of the product then set all product booking status as failed
                            bookedProducts = SetFailedBookingStatus(selectedProducts, criteria);

                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "SupplierBookingService",
                                MethodName = "CreateTiqetsBooking",
                                Token = criteria.Token,
                                Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                            };

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.BookingError
                                , apiResponseStatus
                                , $"Exception\n {ex.Message}\n{response}");
                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, bookedProducts?.FirstOrDefault()?.ServiceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.Tiqets), bookedProducts?.FirstOrDefault()?.OptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    bookedProducts?.FirstOrDefault()?.OptionName ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, bookedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            _log.Error(isangoErrorEntity, ex);
                            return bookedProducts;
                        }
                    }

                    //Confirm Booking after creating order for all products
                    foreach (var product in createOrderProducts)
                    {
                        var tiqetsSelectedProduct = selectedProducts.OfType<TiqetsSelectedProduct>().FirstOrDefault(x => x.AvailabilityReferenceId == product.Key);

                        try
                        {

                            bookingRequest.IsangoBookingReference = product.Value?.OrderReferenceId ?? bookingRequest.IsangoBookingReference;
                            bookingRequest.RequestObject = product.Value;

                            if (createOrderProducts[product.Key] != null)
                            {
                                tiqetsSelectedProduct.OrderStatus = TiqetsOrderStatus.New;
                                tiqetsSelectedProduct.Success = createOrderProducts[product.Key].Success;
                                tiqetsSelectedProduct.OrderReferenceId = createOrderProducts[product.Key].OrderReferenceId;
                            }
                            else
                            {
                                var createOrderSerializedResponse = SerializeDeSerializeHelper.DeSerialize<CreateOrderResponse>(Convert.ToString(response));

                                //Api booking failed
                                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                        , apiResponseStatus
                                        , createOrderSerializedResponse?.Message);
                                try
                                {
                                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, tiqetsSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                    tiqetsSelectedProduct?.OrderReferenceId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.Tiqets), tiqetsSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    tiqetsSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, tiqetsSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                    criteria?.Booking?.UpdateDBLogFlag();
                                }
                                catch (Exception ex)
                                {
                                    //ignore
                                }
                            }

                            tiqetsSelectedProduct.ProductOptions = UpdateOptionStatus(tiqetsSelectedProduct.ProductOptions, createOrderProducts[product.Key].OrderReferenceId);

                            var bookedProduct = criteria.Booking.IsangoBookingData?.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == tiqetsSelectedProduct.AvailabilityReferenceId);

                            if (bookedProduct == null) continue;

                            bookedProduct = MapProductForTiqets(bookedProduct, tiqetsSelectedProduct);
                            bookedProduct.OptionStatus = Constant.StatusSuccess;
                            bookedProducts.Add(bookedProduct);
                        }
                        catch (Exception ex)
                        {
                            tiqetsSelectedProduct.ProductOptions = UpdateOptionStatus(tiqetsSelectedProduct.ProductOptions, tiqetsSelectedProduct.OrderReferenceId);
                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == tiqetsSelectedProduct.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;

                            var option = tiqetsSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                            bookedProduct.OptionStatus = GetBookingStatusNumber(tiqetsSelectedProduct.OrderReferenceId, option.AvailabilityStatus);
                            if (!string.IsNullOrWhiteSpace(tiqetsSelectedProduct.OrderReferenceId))
                            {
                                if (bookedProduct.APIExtraDetail == null)
                                    bookedProduct.APIExtraDetail = new ApiExtraDetail();
                                bookedProduct.APIExtraDetail.SupplieReferenceNumber = tiqetsSelectedProduct.OrderReferenceId;
                            }

                            bookedProducts.Add(bookedProduct);
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "SupplierBookingService",
                                MethodName = "CreateTiqetsBooking",
                                Token = criteria.Token,
                                Params = $"{tiqetsSelectedProduct.Id}|{SerializeDeSerializeHelper.Serialize(tiqetsSelectedProduct)}"
                            };

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.BookingError
                                , apiResponseStatus
                                , $"Exception\n {ex.Message}\n{response}");
                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, tiqetsSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                tiqetsSelectedProduct?.OrderReferenceId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.Tiqets), tiqetsSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                tiqetsSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, tiqetsSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (selectedProducts == null || !selectedProducts.Any()) return bookedProducts;
                selectedProducts.ForEach(product =>
                {
                    var bookedProduct = isangoBookedProducts?.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(product.AvailabilityReferenceId));
                    if (bookedProduct != null)
                    {
                        bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                        bookedProducts.Add(bookedProduct);
                    }
                });
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateTiqetsBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }
        public List<BookedProduct> CreatePrioHubProductsBookingReservation(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var selectedProducts = new List<SelectedProduct>();
            var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (criteria.Booking.SelectedProducts != null && criteria.Booking.SelectedProducts.Count > 0)
                {
                    selectedProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.PrioHub)).ToList();
                    var logPurchaseCriteria = new LogPurchaseXmlCriteria
                    {
                        BookingId = criteria.Booking.BookingId,
                        APIType = APIType.PrioHub,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber
                    };


                    var createOrderProducts = new Dictionary<string, ReservationData.ReservationResponse>();
                    foreach (var product in selectedProducts)
                    {
                        try
                        {
                            _supplierBookingPersistence.InsertReserveRequest(criteria.Token, product.AvailabilityReferenceId);
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }
                        var selectedOption = (ActivityOption)product.ProductOptions.FirstOrDefault(x => x.IsSelected);
                        var selectedProduct = product as PrioHubSelectedProduct;
                        if (selectedProduct == null) continue;
                        selectedProduct.Supplier = new Supplier
                        {
                            AddressLine1 = criteria.Booking.User.Address1,
                            ZipCode = criteria.Booking.User.ZipCode,
                            City = criteria.Booking.User.City,
                            PhoneNumber = string.Empty
                        };
                        selectedProduct.ReservationExpiry = criteria.Booking.ReferenceNumber;
                        selectedProduct.ProductOptions[0].Customers[0].Email = criteria.Booking.VoucherEmailAddress;

                        //Reservation API Call if ticket class is two or three

                        var serviceId = selectedProduct.Id.ToString();
                        var travelDate = selectedProduct.ProductOptions[0].TravelInfo.StartDate.ToShortDateString();
                        //Set values in  selectedProduct as PrioReservationReference, PrioDistributorReference and PrioBookingStatus

                        var PrioHubReservationResult = PrioHubReservationOnly(selectedProduct, selectedProduct.ReservationExpiry, criteria.Token, criteria.Booking, out request, out response);
                        var apiReservationStatus = PrioHubReservationResult?.Data?.Reservation?.ReservationDetails?.FirstOrDefault()?.BookingStatus;
                        if (PrioHubReservationResult != null && apiReservationStatus == "BOOKING_RESERVED")
                        {
                            var item1ReservationReference = PrioHubReservationResult?.Data.Reservation.ReservationReference;
                            var item2BookingReservationReference = PrioHubReservationResult?.Data?.Reservation?.ReservationDetails?.FirstOrDefault()?.BookingReservationReference;
                            var item3BookingStatus = PrioHubReservationResult?.Data?.Reservation?.ReservationDetails?.FirstOrDefault()?.BookingStatus;
                            var item4ReservationDistributorId = PrioHubReservationResult?.Data?.Reservation?.ReservationDistributorId;

                            selectedProduct.PrioReservationReference = item1ReservationReference;
                            selectedProduct.PrioDistributorReference = item2BookingReservationReference;
                            selectedProduct.PrioHubReservationStatus = item3BookingStatus;
                            selectedProduct.PrioHubDistributerId = item4ReservationDistributorId;

                            var reservationDetails = new SupplierBookingReservationResponse()
                            {
                                ApiType = Convert.ToInt32(APIType.PrioHub),
                                ServiceOptionId = selectedOption?.ServiceOptionId ?? selectedOption?.BundleOptionID ?? 0,
                                AvailabilityReferenceId = product.AvailabilityReferenceId,
                                Status = selectedProduct.PrioHubReservationStatus?.ToUpper() == "BOOKING_RESERVED" ? Constant.StatusSuccess : Constant.StatusFailed,
                                BookedOptionId = criteria.Booking.BookingId,
                                BookingReferenceNo = criteria.Booking.ReferenceNumber,
                                OptionName = selectedOption?.Name ?? selectedOption.BundleOptionName,
                                ReservationResponse = SerializeDeSerializeHelper.Serialize(PrioHubReservationResult),
                                ReservationReferenceId = selectedProduct.PrioReservationReference,
                                Token = criteria.Token
                            };

                            _tableStorageOperation.InsertReservationDetails(reservationDetails);

                            var reservationReq = item1ReservationReference;
                            var reservationRes = item2BookingReservationReference + "serviceID : " + serviceId + ", travelDate : " + travelDate;

                            InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.Reservation, Constant.StatusSuccess);

                            try
                            {
                                _supplierBookingPersistence.UpdateReserveRequest(criteria.Token, product.AvailabilityReferenceId, criteria.Booking.ReferenceNumber);
                            }
                            catch (Exception ex)
                            {
                                //ignore
                            }

                            createOrderProducts.Add(product.AvailabilityReferenceId, PrioHubReservationResult);
                        }
                        else
                        {
                            InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.Reservation, Constant.StatusFailed);

                            //If the create order call is failed for any of the product then set all product booking status as failed
                            bookedProducts = SetFailedBookingStatus(selectedProducts, criteria);

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, product?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                     selectedProduct.PrioReservationReference, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.PrioHub), product?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    product?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, product?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();


                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            return bookedProducts;
                        }



                        //throw new NullReferenceException("for testing"); //for booking cancel testing
                    }

                    foreach (var product in createOrderProducts)
                    {
                        var prioHubSelectedProduct = selectedProducts.OfType<PrioHubSelectedProduct>().FirstOrDefault(x => x.AvailabilityReferenceId == product.Key);
                        try
                        {
                            prioHubSelectedProduct.ProductOptions = UpdateOptionStatus(prioHubSelectedProduct.ProductOptions, prioHubSelectedProduct.PrioReservationReference);

                            var bookedProduct = criteria.Booking.IsangoBookingData?.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == prioHubSelectedProduct.AvailabilityReferenceId);

                            if (bookedProduct == null) continue;

                            bookedProduct = MapProductForPrioHub(bookedProduct, prioHubSelectedProduct);
                            bookedProduct.OptionStatus = Constant.StatusSuccess;
                            bookedProducts.Add(bookedProduct);
                        }
                        catch (Exception ex)
                        {
                            prioHubSelectedProduct.ProductOptions = UpdateOptionStatus(prioHubSelectedProduct.ProductOptions, prioHubSelectedProduct.PrioReservationReference);
                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == prioHubSelectedProduct.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;

                            var option = prioHubSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                            bookedProduct.OptionStatus = GetBookingStatusNumber(prioHubSelectedProduct.PrioReservationReference, option.AvailabilityStatus);
                            if (!string.IsNullOrWhiteSpace(prioHubSelectedProduct.PrioReservationReference))
                            {
                                if (bookedProduct.APIExtraDetail == null)
                                    bookedProduct.APIExtraDetail = new ApiExtraDetail();
                                bookedProduct.APIExtraDetail.SupplieReferenceNumber = prioHubSelectedProduct.PrioReservationReference;
                            }

                            bookedProducts.Add(bookedProduct);
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "SupplierBookingService",
                                MethodName = "CreatePriohubReservation",
                                Token = criteria.Token,
                                Params = $"{prioHubSelectedProduct.Id}|{SerializeDeSerializeHelper.Serialize(prioHubSelectedProduct)}"
                            };

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, prioHubSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                               prioHubSelectedProduct.PrioReservationReference, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.PrioHub), prioHubSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                prioHubSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, prioHubSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (selectedProducts == null || !selectedProducts.Any()) return bookedProducts;
                selectedProducts.ForEach(product =>
                {
                    var bookedProduct = isangoBookedProducts?.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(product.AvailabilityReferenceId));
                    if (bookedProduct != null)
                    {
                        bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                        bookedProducts.Add(bookedProduct);
                    }
                });
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreatePriohubReservation",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }


        public List<BookedProduct> CreateTourCMSProductsBookingReservation(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var selectedProducts = new List<SelectedProduct>();
            var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (criteria.Booking.SelectedProducts != null && criteria.Booking.SelectedProducts.Count > 0)
                {
                    selectedProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.TourCMS)).ToList();
                    var logPurchaseCriteria = new LogPurchaseXmlCriteria
                    {
                        BookingId = criteria.Booking.BookingId,
                        APIType = APIType.TourCMS,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber
                    };


                    var createOrderProducts = new Dictionary<string, NewBookingResponse>();
                    foreach (var product in selectedProducts)
                    {
                        var apiId = 1L;
                        try
                        {
                            _supplierBookingPersistence.InsertReserveRequest(criteria.Token, product.AvailabilityReferenceId);
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }
                        var selectedOption = (ActivityOption)product.ProductOptions.FirstOrDefault(x => x.IsSelected);
                        var selectedProduct = product as TourCMSSelectedProduct;
                        if (selectedProduct == null) continue;
                        selectedProduct.Supplier = new Supplier
                        {
                            AddressLine1 = criteria.Booking.User.Address1,
                            ZipCode = criteria.Booking.User.ZipCode,
                            City = criteria.Booking.User.City,
                            PhoneNumber = string.Empty
                        };
                        //selectedProduct.ReservationExpiry = criteria.Booking.ReferenceNumber;
                        selectedProduct.ProductOptions[0].Customers[0].Email = criteria.Booking.VoucherEmailAddress;

                        //Reservation API Call if ticket class is two or three

                        var serviceId = selectedProduct.Id.ToString();
                        var travelDate = selectedProduct.ProductOptions[0].TravelInfo.StartDate.ToShortDateString();

                        var tourCMSReservationResult = TourCMSReservationOnly(selectedProduct, criteria.Booking?.Language?.Code,
                            criteria.Booking?.VoucherEmailAddress, criteria.Booking?.VoucherPhoneNumber, criteria.Booking?.ReferenceNumber,
                            criteria?.Token, criteria?.Booking, out request, out response);


                        if (tourCMSReservationResult != null && tourCMSReservationResult.Error.ToLowerInvariant() == "ok")
                        {
                            apiId = tourCMSReservationResult.Booking.BookingId;
                            ((TourCMSSelectedProduct)selectedProduct).BookingId = System.Convert.ToInt32(apiId);

                            var reservationDetails = new SupplierBookingReservationResponse()
                            {
                                ApiType = Convert.ToInt32(APIType.TourCMS),
                                ServiceOptionId = selectedOption?.ServiceOptionId ?? selectedOption?.BundleOptionID ?? 0,
                                AvailabilityReferenceId = product.AvailabilityReferenceId,
                                Status = tourCMSReservationResult.Error.ToLowerInvariant() == "ok" ? Constant.StatusSuccess : Constant.StatusFailed,
                                BookedOptionId = criteria.Booking.BookingId,
                                BookingReferenceNo = criteria.Booking.ReferenceNumber,
                                OptionName = selectedOption?.Name ?? selectedOption.BundleOptionName,
                                ReservationResponse = SerializeDeSerializeHelper.Serialize(tourCMSReservationResult),
                                ReservationReferenceId = System.Convert.ToString(apiId),
                                Token = criteria.Token
                            };

                            _tableStorageOperation.InsertReservationDetails(reservationDetails);

                            //var reservationReq = item1ReservationReference;
                            var reservationRes = apiId + "serviceID : " + serviceId + ", travelDate : " + travelDate;

                            InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.Reservation, Constant.StatusSuccess);

                            try
                            {
                                _supplierBookingPersistence.UpdateReserveRequest(criteria.Token, product.AvailabilityReferenceId, criteria.Booking.ReferenceNumber);
                            }
                            catch (Exception ex)
                            {
                                //ignore
                            }

                            createOrderProducts.Add(product.AvailabilityReferenceId, tourCMSReservationResult);
                        }
                        else
                        {
                            InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.Reservation, Constant.StatusFailed);

                            //If the create order call is failed for any of the product then set all product booking status as failed
                            bookedProducts = SetFailedBookingStatus(selectedProducts, criteria);

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, product?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                     Convert.ToString(apiId), criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.TourCMS), product?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    product?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, product?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();


                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            return bookedProducts;
                        }



                        //throw new NullReferenceException("for testing"); //for booking cancel testing
                    }

                    foreach (var product in createOrderProducts)
                    {
                        var tourCMSSelectedProduct = selectedProducts.OfType<TourCMSSelectedProduct>().FirstOrDefault(x => x.AvailabilityReferenceId == product.Key);
                        try
                        {
                            tourCMSSelectedProduct.ProductOptions = UpdateOptionStatus(tourCMSSelectedProduct.ProductOptions, Convert.ToString(tourCMSSelectedProduct.BookingId));

                            var bookedProduct = criteria.Booking.IsangoBookingData?.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == tourCMSSelectedProduct.AvailabilityReferenceId);

                            if (bookedProduct == null) continue;

                            bookedProduct = MapProductForTourCMS(bookedProduct, tourCMSSelectedProduct);
                            bookedProduct.OptionStatus = Constant.StatusSuccess;
                            bookedProducts.Add(bookedProduct);
                        }
                        catch (Exception ex)
                        {
                            tourCMSSelectedProduct.ProductOptions = UpdateOptionStatus(tourCMSSelectedProduct.ProductOptions, Convert.ToString(tourCMSSelectedProduct.BookingId));
                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == tourCMSSelectedProduct.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;

                            var option = tourCMSSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                            bookedProduct.OptionStatus = GetBookingStatusNumber(Convert.ToString(tourCMSSelectedProduct.BookingId), option.AvailabilityStatus);
                            if (!string.IsNullOrWhiteSpace(Convert.ToString(tourCMSSelectedProduct.BookingId)))
                            {
                                if (bookedProduct.APIExtraDetail == null)
                                    bookedProduct.APIExtraDetail = new ApiExtraDetail();
                                bookedProduct.APIExtraDetail.SupplieReferenceNumber = Convert.ToString(tourCMSSelectedProduct.BookingId);
                            }

                            bookedProducts.Add(bookedProduct);
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "SupplierBookingService",
                                MethodName = "CreateTourCMSReservation",
                                Token = criteria.Token,
                                Params = $"{tourCMSSelectedProduct.Id}|{SerializeDeSerializeHelper.Serialize(tourCMSSelectedProduct)}"
                            };

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, tourCMSSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                Convert.ToString(tourCMSSelectedProduct.BookingId), criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.TourCMS), tourCMSSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                tourCMSSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, tourCMSSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (selectedProducts == null || !selectedProducts.Any()) return bookedProducts;
                selectedProducts.ForEach(product =>
                {
                    var bookedProduct = isangoBookedProducts?.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(product.AvailabilityReferenceId));
                    if (bookedProduct != null)
                    {
                        bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                        bookedProducts.Add(bookedProduct);
                    }
                });
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateTourCMSReservation",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }

        public List<BookedProduct> CreateVentrataProductsBookingReservation(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var selectedProducts = new List<SelectedProduct>();
            var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (criteria.Booking.SelectedProducts != null && criteria.Booking.SelectedProducts.Count > 0)
                {
                    selectedProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.Ventrata)).ToList();
                    var logPurchaseCriteria = new LogPurchaseXmlCriteria
                    {
                        BookingId = criteria.Booking.BookingId,
                        APIType = APIType.Ventrata,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber
                    };


                    var createOrderProducts = new Dictionary<string, BookingReservationRes>();
                    foreach (var product in selectedProducts)
                    {
                        try
                        {
                            _supplierBookingPersistence.InsertReserveRequest(criteria.Token, product.AvailabilityReferenceId);
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }
                        var selectedOption = (ActivityOption)product.ProductOptions.FirstOrDefault(x => x.IsSelected);
                        var selectedProduct = product as VentrataSelectedProduct;
                        if (selectedProduct == null) continue;
                        selectedProduct.Supplier = new Supplier
                        {
                            AddressLine1 = criteria.Booking.User.Address1,
                            ZipCode = criteria.Booking.User.ZipCode,
                            City = criteria.Booking.User.City,
                            PhoneNumber = string.Empty
                        };

                        var serviceId = selectedProduct.Id.ToString();
                        var travelDate = selectedProduct.ProductOptions[0].TravelInfo.StartDate.ToShortDateString();

                        selectedProduct.Uuid = criteria.Booking.ReferenceNumber;
                        selectedProduct.ResellerReference = criteria.Booking.ReferenceNumber;
                        selectedProduct.ProductOptions[0].Customers[0].Email = criteria.Booking.VoucherEmailAddress;

                        //check Package or Not
                        var productid = ((ActivityOption)selectedOption)?.VentrataProductId;
                        var optionid = selectedOption.SupplierOptionCode;
                        var ventrataPackagesOrNot = GetVentrataPackages(productid, optionid);

                        var ventrataPaxMapping = _masterCacheManager.GetVentrataPaxMappings();
                        if (ventrataPaxMapping?.Any() != true)
                        {
                            ventrataPaxMapping = GetPaxMappingsForVentrataAPI(APIType.Ventrata);
                        }

                        selectedProduct.VentrataPaxMappings = ventrataPaxMapping.Where(x => x.ServiceOptionId == selectedOption.ServiceOptionId)?.ToList();

                        var reservationResponseObject = _ventrataAdapter.CreateReservation(selectedProduct, out request, out response, criteria.Token, ventrataPackagesOrNot);
                        var castedReservationResObject = new BookingReservationRes();
                        if (reservationResponseObject != null)
                        {
                            castedReservationResObject = ((BookingReservationRes)reservationResponseObject);
                            selectedProduct.BookingStatus = castedReservationResObject.status;
                            selectedProduct.IsPackage = Convert.ToBoolean(castedReservationResObject.isPackage);
                        }

                        var apiReservationStatus = selectedProduct.BookingStatus?.ToLower();
                        if (castedReservationResObject != null && apiReservationStatus == VentrataEntities.VentrataBookingStatus.OnHold.ToLower())
                        {
                            selectedProduct.Uuid = castedReservationResObject.uuid;
                            selectedProduct.SupplierReference = castedReservationResObject.supplierReference;
                            selectedProduct.TestMode = castedReservationResObject.testMode;
                            selectedProduct.IsCancellable = castedReservationResObject.cancellable;

                            var reservationDetails = new SupplierBookingReservationResponse()
                            {
                                ApiType = Convert.ToInt32(APIType.PrioHub),
                                ServiceOptionId = selectedOption?.ServiceOptionId ?? selectedOption?.BundleOptionID ?? 0,
                                AvailabilityReferenceId = product.AvailabilityReferenceId,
                                Status = apiReservationStatus == VentrataEntities.VentrataBookingStatus.OnHold.ToLower() ? Constant.StatusSuccess : Constant.StatusFailed,
                                BookedOptionId = criteria.Booking.BookingId,
                                BookingReferenceNo = criteria.Booking.ReferenceNumber,
                                OptionName = selectedOption?.Name ?? selectedOption.BundleOptionName,
                                ReservationResponse = SerializeDeSerializeHelper.Serialize(castedReservationResObject),
                                ReservationReferenceId = selectedProduct.SupplierReference,
                                Token = criteria.Token
                            };

                            _tableStorageOperation.InsertReservationDetails(reservationDetails);

                            //var reservationReq = item1ReservationReference;
                            //var reservationRes = item2BookingReservationReference + "serviceID : " + serviceId + ", travelDate : " + travelDate;

                            InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.Reservation, Constant.StatusSuccess);

                            try
                            {
                                _supplierBookingPersistence.UpdateReserveRequest(criteria.Token, product.AvailabilityReferenceId, criteria.Booking.ReferenceNumber);
                            }
                            catch (Exception ex)
                            {
                                //ignore
                            }

                            createOrderProducts.Add(product.AvailabilityReferenceId, castedReservationResObject);
                        }
                        else
                        {
                            InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.Reservation, Constant.StatusFailed);

                            //If the create order call is failed for any of the product then set all product booking status as failed
                            bookedProducts = SetFailedBookingStatus(selectedProducts, criteria);

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, product?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                     selectedProduct.SupplierReference, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                    Convert.ToInt32(APIType.Ventrata), product?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                    product?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, product?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();


                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            return bookedProducts;
                        }



                        //throw new NullReferenceException("for testing"); //for booking cancel testing
                    }

                    foreach (var product in createOrderProducts)
                    {
                        var ventrataSelectedProduct = selectedProducts.OfType<VentrataSelectedProduct>().FirstOrDefault(x => x.AvailabilityReferenceId == product.Key);
                        try
                        {
                            ventrataSelectedProduct.ProductOptions = UpdateOptionStatus(ventrataSelectedProduct.ProductOptions, ventrataSelectedProduct.SupplierReference);

                            var bookedProduct = criteria.Booking.IsangoBookingData?.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == ventrataSelectedProduct.AvailabilityReferenceId);

                            if (bookedProduct == null) continue;

                            bookedProduct = MapProductForVentrata(bookedProduct, ventrataSelectedProduct);
                            bookedProduct.OptionStatus = Constant.StatusSuccess;
                            bookedProducts.Add(bookedProduct);
                        }
                        catch (Exception ex)
                        {
                            ventrataSelectedProduct.ProductOptions = UpdateOptionStatus(ventrataSelectedProduct.ProductOptions, ventrataSelectedProduct.SupplierReference);
                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == ventrataSelectedProduct.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;

                            var option = ventrataSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                            bookedProduct.OptionStatus = GetBookingStatusNumber(ventrataSelectedProduct.SupplierReference, option.AvailabilityStatus);
                            if (!string.IsNullOrWhiteSpace(ventrataSelectedProduct.SupplierReference))
                            {
                                if (bookedProduct.APIExtraDetail == null)
                                    bookedProduct.APIExtraDetail = new ApiExtraDetail();
                                bookedProduct.APIExtraDetail.SupplieReferenceNumber = ventrataSelectedProduct.SupplierReference;
                            }

                            bookedProducts.Add(bookedProduct);
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "SupplierBookingService",
                                MethodName = "CreateVentrataReservation",
                                Token = criteria.Token,
                                Params = $"{ventrataSelectedProduct.Id}|{SerializeDeSerializeHelper.Serialize(ventrataSelectedProduct)}"
                            };

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, ventrataSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                               ventrataSelectedProduct.SupplierReference, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.Ventrata), ventrataSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                ventrataSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, ventrataSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (selectedProducts == null || !selectedProducts.Any()) return bookedProducts;
                selectedProducts.ForEach(product =>
                {
                    var bookedProduct = isangoBookedProducts?.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(product.AvailabilityReferenceId));
                    if (bookedProduct != null)
                    {
                        bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                        bookedProducts.Add(bookedProduct);
                    }
                });
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateVentrataReservation",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }

        public List<BookedProduct> CreateNewCitySightSeeingReservation(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var selectedProducts = criteria.Booking.SelectedProducts.
                Where(x => x.APIType.Equals(APIType.NewCitySightSeeing)).ToList();

            var request = string.Empty;
            var response = string.Empty;

            try
            {

                var logPurchaseCriteria = new LogPurchaseXmlCriteria
                {
                    BookingId = criteria.Booking.BookingId,
                    APIType = APIType.NewCitySightSeeing,
                    BookingReferenceNumber = criteria.Booking.ReferenceNumber
                };

                var createOrderProducts = new Dictionary<string, ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Reservation.ReservationResponse>();
                foreach (var product in selectedProducts)
                {
                    try
                    {
                        _supplierBookingPersistence.InsertReserveRequest(criteria.Token, product.AvailabilityReferenceId);
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                    var selectedOption = (ActivityOption)product.ProductOptions.FirstOrDefault(x => x.IsSelected);

                    // Reservation
                    var apiReservationResponse = _newCitySightSeeingAdapter.CreateReservation(product,
                        criteria?.Booking?.Language?.Code,
                        criteria?.Booking?.VoucherEmailAddress, criteria?.Booking?.VoucherPhoneNumber,
                        criteria?.Booking?.ReferenceNumber, criteria?.Booking?.User?.ZipCode,
                        criteria?.Booking?.User?.Address1, criteria?.Booking?.User?.City,
                        criteria.Token, out request, out response);

                    var castedReservationResObject = new ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Reservation.ReservationResponse();
                    if (apiReservationResponse != null)
                    {
                        if (!string.IsNullOrEmpty(apiReservationResponse.ReservationId))
                        {
                            ((NewCitySightSeeingSelectedProduct)product).NewCitySightSeeingReservationId = apiReservationResponse.ReservationId;
                        }
                        castedReservationResObject = apiReservationResponse;
                    }

                    var apiReservationId = ((NewCitySightSeeingSelectedProduct)product)?.NewCitySightSeeingReservationId;

                    if (!string.IsNullOrEmpty(apiReservationId))
                    {
                        logPurchaseCriteria.RequestXml = request;
                        logPurchaseCriteria.ResponseXml = response;
                        logPurchaseCriteria.Bookingtype = Constant.StatusReservation;
                        logPurchaseCriteria.Status = Constant.StatusSuccess;

                        var reservationDetails = new SupplierBookingReservationResponse()
                        {
                            ApiType = Convert.ToInt32(APIType.NewCitySightSeeing),
                            ServiceOptionId = selectedOption?.ServiceOptionId ?? selectedOption?.BundleOptionID ?? 0,
                            AvailabilityReferenceId = product.AvailabilityReferenceId,
                            Status = Constant.StatusSuccess,
                            BookedOptionId = criteria.Booking.BookingId,
                            BookingReferenceNo = criteria.Booking.ReferenceNumber,
                            OptionName = selectedOption?.Name ?? selectedOption.BundleOptionName,
                            ReservationResponse = SerializeDeSerializeHelper.Serialize(apiReservationResponse),
                            ReservationReferenceId = apiReservationId,
                            Token = criteria.Token
                        };

                        _tableStorageOperation.InsertReservationDetails(reservationDetails);

                        InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.Reservation, Constant.StatusSuccess);

                        try
                        {
                            _supplierBookingPersistence.UpdateReserveRequest(criteria.Token, product.AvailabilityReferenceId, criteria.Booking.ReferenceNumber);
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }

                        createOrderProducts.Add(product.AvailabilityReferenceId, apiReservationResponse);
                    }
                    else
                    {
                        logPurchaseCriteria.RequestXml = request;
                        logPurchaseCriteria.ResponseXml = response;
                        logPurchaseCriteria.Bookingtype = Constant.StatusReservation;
                        logPurchaseCriteria.Status = Constant.StatusFailed;

                        InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.CreateReservation, Constant.StatusFailed);

                        foreach (var newcitySightSeeingProduct in selectedProducts)
                        {
                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == newcitySightSeeingProduct.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;
                            bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                            bookedProducts.Add(bookedProduct);
                        }
                        //Api booking failed
                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);

                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, selectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.NewCitySightSeeing), selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }

                        return bookedProducts;
                    }


                }


                foreach (var productData in createOrderProducts)
                {

                    var newCitySightSelectedProduct = selectedProducts.OfType<NewCitySightSeeingSelectedProduct>().FirstOrDefault(x => x.AvailabilityReferenceId == productData.Key);
                    try
                    {
                        newCitySightSelectedProduct.ProductOptions = UpdateOptionStatus(newCitySightSelectedProduct.ProductOptions, newCitySightSelectedProduct.NewCitySightSeeingReservationId);

                        var bookedProduct = criteria.Booking.IsangoBookingData?.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == newCitySightSelectedProduct.AvailabilityReferenceId);

                        if (bookedProduct == null) continue;

                        bookedProduct = MapProductForNewCitySightSeeing(bookedProduct, newCitySightSelectedProduct);
                        bookedProduct.OptionStatus = Constant.StatusSuccess;
                        bookedProducts.Add(bookedProduct);
                    }
                    catch (Exception ex)
                    {
                        newCitySightSelectedProduct.ProductOptions = UpdateOptionStatus(newCitySightSelectedProduct.ProductOptions, newCitySightSelectedProduct.NewCitySightSeeingReservationId);
                        var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                            x.AvailabilityReferenceId == newCitySightSelectedProduct.AvailabilityReferenceId);
                        if (bookedProduct == null) continue;

                        var option = newCitySightSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                        bookedProduct.OptionStatus = GetBookingStatusNumber(newCitySightSelectedProduct.NewCitySightSeeingReservationId, option.AvailabilityStatus);
                        if (!string.IsNullOrWhiteSpace(newCitySightSelectedProduct.NewCitySightSeeingReservationId))
                        {
                            if (bookedProduct.APIExtraDetail == null)
                                bookedProduct.APIExtraDetail = new ApiExtraDetail();
                            bookedProduct.APIExtraDetail.SupplieReferenceNumber = newCitySightSelectedProduct.NewCitySightSeeingReservationId;
                        }

                        bookedProducts.Add(bookedProduct);
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "SupplierBookingService",
                            MethodName = "CreateNewCitySightSeeingReservation",
                            Token = criteria.Token,
                            Params = $"{newCitySightSelectedProduct.Id}|{SerializeDeSerializeHelper.Serialize(newCitySightSelectedProduct)}"
                        };

                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , $"Exception\n {ex.Message}\n{response}");
                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, newCitySightSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                           newCitySightSelectedProduct.NewCitySightSeeingReservationId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.NewCitySightSeeing), newCitySightSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            newCitySightSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, newCitySightSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }

                        _log.Error(isangoErrorEntity, ex);
                    }
                }

            }
            catch (Exception ex)
            {
                foreach (var selectedProduct in selectedProducts)
                {
                    var option = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);

                    var bookingReference = ((NewCitySightSeeingSelectedProduct)selectedProduct).NewCitySightSeeingReservationId;
                    if (bookedProduct == null) continue;
                    bookedProduct.OptionStatus = GetBookingStatusNumber(bookingReference, option?.AvailabilityStatus);
                    bookedProducts.Add(bookedProduct);
                }

                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateNewCitySightSeeingBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, selectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.NewCitySightSeeing), selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }





        public Dictionary<string, bool> CancelTiqetsBooking(List<TiqetsSelectedProduct> tiqetsSelectedProducts, string bookingReferenceNo, string token, string languageCode, string affiliateId)
        {
            var status = new Dictionary<string, bool>();
            tiqetsSelectedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });

            var isCancelled = false;
            foreach (var tiqetsSelectedProduct in tiqetsSelectedProducts)
            {
                try
                {
                    try
                    {
                        if (String.IsNullOrEmpty(tiqetsSelectedProduct.OrderReferenceId))
                        {
                            var rowKey = $"{bookingReferenceNo}-{tiqetsSelectedProduct.AvailabilityReferenceId}";
                            var ReservationDetails = _tableStorageOperation.RetrieveReservationData(rowKey);

                            tiqetsSelectedProduct.OrderReferenceId = ReservationDetails.ReservationReferenceId;
                        }
                    }
                    catch (Exception e)
                    {
                        //ignore
                    }

                    var orderInfoResponse = _tiqetsAdapter.GetOrderInformation(tiqetsSelectedProduct, bookingReferenceNo, token, languageCode, out var requestTxt, out var responseTxt, out var apiResponseStatusData, affiliateId);
                    _log.Info($"SupplierBookingService|CancelTiqetsBooking,{SerializeDeSerializeHelper.Serialize(orderInfoResponse)}");
                    var logCriteria = new LogPurchaseXmlCriteria
                    {
                        RequestXml = requestTxt,
                        ResponseXml = responseTxt,
                        Status = orderInfoResponse != null ? Constant.StatusSuccess : Constant.StatusFailed,
                        BookingId = 0,
                        APIType = APIType.Tiqets,
                        BookingReferenceNumber = bookingReferenceNo,
                        Bookingtype = "Get Order/Booking Info"
                    };
                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);

                    if (orderInfoResponse?.IsCancellable == true && orderInfoResponse?.OrderStatus.ToLowerInvariant() == "done")
                    {
                        _log.Info($"SupplierBookingService|CancelTiqetsBooking|TiqetsCancellationStart");
                        //Cancel the confirmed order as it is withing the cancellable window
                        var isOrderCancelled = _tiqetsAdapter.CancelOrder(tiqetsSelectedProduct, bookingReferenceNo, token, languageCode, out var request, out var response, out var apiResponseStatus, affiliateId);
                        _log.Info($"SupplierBookingService|CancelTiqetsBooking|TiqetsCancellationEnd" + isOrderCancelled);
                        isCancelled = isOrderCancelled;
                        status[tiqetsSelectedProduct.AvailabilityReferenceId] = isCancelled;

                        var logCriteriaCancel = new LogPurchaseXmlCriteria
                        {
                            RequestXml = request,
                            ResponseXml = response,
                            Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed,
                            BookingId = 0,
                            APIType = APIType.Tiqets,
                            BookingReferenceNumber = bookingReferenceNo,
                            Bookingtype = "Cancel Booking"
                        };
                        _supplierBookingPersistence.LogPurchaseXML(logCriteriaCancel);
                    }
                    //Product Already Cancelled
                    else if (orderInfoResponse?.IsCancellable == true && orderInfoResponse?.OrderStatus.ToLowerInvariant() == "cancelled")
                    {
                        _log.Info($"SupplierBookingService|CancelTiqetsBooking|TiqetsAlreadyCancelled");
                        status[tiqetsSelectedProduct.AvailabilityReferenceId] = true;
                    }
                    //Booking Not Successful
                    else if (orderInfoResponse?.OrderStatus.ToLowerInvariant() == "failed")
                    {
                        _log.Info($"SupplierBookingService|CancelTiqetsBooking|TiqetsBookingFailed");
                        status[tiqetsSelectedProduct.AvailabilityReferenceId] = true;
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = nameof(SupplierBookingService),
                        MethodName = nameof(CancelTiqetsBooking),
                        Token = token,
                        Params = $"{SerializeDeSerializeHelper.Serialize(tiqetsSelectedProduct)}{token}"
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            }
            _log.Info($"SupplierBookingService|CancelTiqetsBooking,{SerializeDeSerializeHelper.Serialize(status)}");
            return status;
        }

        /// <summary>
        /// Create Golden tour bookings
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<BookedProduct> CreateGoldenToursProductsBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();

            var request = string.Empty;
            var response = string.Empty;

            var selectedProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.Goldentours)).ToList();
            try
            {
                // Supplier Create Booking call initiated
                var bookedSelectedProducts = _goldenToursAdapter.CreateBooking(selectedProducts, criteria.Token, out request, out response);
                foreach (var selectedProduct in selectedProducts)
                {
                    var goldenToursSelectedProduct = (GoldenToursSelectedProduct)bookedSelectedProducts?.FirstOrDefault(x => x.Id == selectedProduct.Id) ?? (GoldenToursSelectedProduct)selectedProduct;

                    if (!string.IsNullOrWhiteSpace(goldenToursSelectedProduct?.TicketReferenceNumber))
                    {
                        //Api booking failed
                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);
                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, goldenToursSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                            goldenToursSelectedProduct?.TicketReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.Goldentours), goldenToursSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            goldenToursSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, goldenToursSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }
                    }
                    // Setting the booking status of all the product options by checking the supplier's TicketReferenceNumber
                    goldenToursSelectedProduct.ProductOptions = UpdateOptionStatus(
                        goldenToursSelectedProduct.ProductOptions,
                        goldenToursSelectedProduct.TicketReferenceNumber);

                    // Logging the supplier booking details in the DB
                    var logCriteria = new LogPurchaseXmlCriteria
                    {
                        RequestXml = request,
                        ResponseXml = response,
                        Status = !string.IsNullOrWhiteSpace(goldenToursSelectedProduct.TicketReferenceNumber)
                            ? Constant.StatusSuccess
                            : Constant.StatusFailed,
                        BookingId = criteria.Booking.BookingId,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber,
                        APIType = APIType.Goldentours,
                        ApiRefNumber = goldenToursSelectedProduct.TicketReferenceNumber
                    };
                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);

                    // Mapping the booked product of the IsangoBookingData as per the success/fail booking
                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                        x.AvailabilityReferenceId == goldenToursSelectedProduct.AvailabilityReferenceId);
                    if (bookedProduct == null || goldenToursSelectedProduct == null) continue;
                    bookedProduct = MapProductForGoldenTours(bookedProduct, goldenToursSelectedProduct);
                    bookedProducts.Add(bookedProduct);
                }
            }
            catch (Exception ex)
            {
                var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
                if (isangoBookedProducts == null) return bookedProducts;

                foreach (var selectedProduct in selectedProducts)
                {
                    var bookedProduct = isangoBookedProducts.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(selectedProduct.AvailabilityReferenceId));
                    if (bookedProduct == null) continue;
                    bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                    bookedProducts.Add(bookedProduct);
                }
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateGoldenToursProductsBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, selectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                    bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                    Convert.ToInt32(APIType.Goldentours), selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                    selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception eex)
                {
                    //ignore
                }
                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }

        ///<summary>
        ///Create Redeam bookings
        ///</summary>
        ///<param name="criteria"></param>
        ///<returns></returns>
        public List<BookedProduct> CreateRedeamBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var request = string.Empty;
            var response = string.Empty;
            var selectedProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.Redeam)).ToList();
            try
            {
                foreach (var selectedProduct in selectedProducts)
                {
                    var redeamSelectedProduct = selectedProduct;
                    ((RedeamSelectedProduct)redeamSelectedProduct).BookingReferenceNumber = criteria?.Booking?.ReferenceNumber ?? string.Empty;
                    var isHoldable = ((ActivityOption)selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected))?.Holdable ?? false;
                    if (isHoldable)
                    {

                        //Start- Get Reservation from Storage
                        var rowKey = $"{criteria.Booking.ReferenceNumber}-{selectedProduct.AvailabilityReferenceId}";
                        var ReservationDetails = _tableStorageOperation.RetrieveReservationData(rowKey);
                        var createOrderResponse = SerializeDeSerializeHelper.DeSerialize<CreateHoldResponse>(ReservationDetails?.ReservationResponse);
                        var result = new Tuple<string, string>("", "");
                        if (ReservationDetails != null)
                        {
                            result = RedeamReservationGet((RedeamSelectedProduct)redeamSelectedProduct, createOrderResponse, criteria.Booking, criteria.Token);
                        }
                        if (ReservationDetails == null && String.IsNullOrEmpty(((RedeamSelectedProduct)selectedProduct).HoldId))
                        {
                            // Create Hold before Booking
                            redeamSelectedProduct = _redeamAdapter.CreateHold(redeamSelectedProduct, criteria.Token).GetAwaiter().GetResult();
                        }

                        // Prepare BookedProduct with Failed Status
                        if (redeamSelectedProduct == null)
                        {
                            // Mapping the booked product of the IsangoBookingData as per the fail booking
                            var redeamBookedProduct = criteria?.Booking?.IsangoBookingData?.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);
                            if (redeamBookedProduct == null) continue;

                            redeamBookedProduct = MapProductForRedeam(redeamBookedProduct, selectedProduct as RedeamSelectedProduct);
                            bookedProducts.Add(redeamBookedProduct);
                            continue;
                        }
                    }
                    // Supplier Create Booking call initiated
                    redeamSelectedProduct = _redeamAdapter.CreateBooking(redeamSelectedProduct, criteria.Token, out request, out response);

                    //Prepare BookedProduct with Failed Status
                    if (redeamSelectedProduct == null)
                    {
                        //Api booking failed
                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);
                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, redeamSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                            string.Empty, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.Redeam), redeamSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            redeamSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, redeamSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }

                        // Mapping the booked product of the IsangoBookingData as per the fail booking
                        var redeamBookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                            x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);
                        if (redeamBookedProduct == null) continue;

                        redeamBookedProduct = MapProductForRedeam(redeamBookedProduct, selectedProduct as RedeamSelectedProduct);
                        bookedProducts.Add(redeamBookedProduct);
                        continue;
                    }
                    var redeamSelectedBookedProduct = (RedeamSelectedProduct)redeamSelectedProduct;

                    //Setting the booking status of all the product options by checking the supplier's TicketReferenceNumber
                    redeamSelectedProduct.ProductOptions = UpdateOptionStatus(redeamSelectedBookedProduct.ProductOptions, redeamSelectedBookedProduct.BookingReferenceNumber);

                    // Logging the supplier booking details in the DB
                    var logCriteria = new LogPurchaseXmlCriteria
                    {
                        RequestXml = request,
                        ResponseXml = response,
                        Status = !string.IsNullOrWhiteSpace(redeamSelectedBookedProduct.BookingReferenceNumber)
                            ? Constant.StatusSuccess
                            : Constant.StatusFailed,
                        BookingId = criteria.Booking.BookingId,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber,
                        APIType = APIType.Redeam,
                        ApiRefNumber = redeamSelectedBookedProduct.BookingReferenceNumber
                    };
                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);

                    // Mapping the booked product of the IsangoBookingData as per the success/fail booking
                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                        x.AvailabilityReferenceId == redeamSelectedProduct.AvailabilityReferenceId);
                    if (bookedProduct == null || redeamSelectedProduct == null) continue;
                    bookedProduct = MapProductForRedeam(bookedProduct, redeamSelectedBookedProduct);
                    bookedProducts.Add(bookedProduct);
                }
            }
            catch (Exception ex)
            {
                var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
                if (isangoBookedProducts == null) return bookedProducts;

                foreach (var selectedProduct in selectedProducts)
                {
                    var bookedProduct = isangoBookedProducts.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(selectedProduct.AvailabilityReferenceId));

                    if (bookedProduct == null) continue;
                    // Checking if already contains the booked product, to maintain the booking status
                    if (bookedProducts.Contains(bookedProduct)) continue;

                    bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                    bookedProducts.Add(bookedProduct);
                }

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                _log.Error($"SupplierBookingService|CreateRedeamProductsBooking|{SerializeDeSerializeHelper.Serialize(criteria)}", ex);

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, selectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber ?? string.Empty, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.Redeam), selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }
            }
            return bookedProducts;
        }

        public List<BookedProduct> CreateRedeamBookingReservation(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var selectedProducts = new List<SelectedProduct>();
            var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (criteria.Booking.SelectedProducts != null && criteria.Booking.SelectedProducts.Count > 0)
                {
                    selectedProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.Redeam)).ToList();
                    var logPurchaseCriteria = new LogPurchaseXmlCriteria
                    {
                        BookingId = criteria.Booking.BookingId,
                        APIType = APIType.Redeam,
                        BookingReferenceNumber = criteria.Booking.ReferenceNumber
                    };

                    var createOrderProducts = new Dictionary<string, ServiceAdapters.Redeam.Redeam.Entities.CreateHold.CreateHoldResponse>();
                    foreach (var product in selectedProducts)
                    {
                        var redeamSelectedProduct = product;
                        try
                        {
                            _supplierBookingPersistence.InsertReserveRequest(criteria.Token, product.AvailabilityReferenceId);
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }
                        var selectedOption = (ActivityOption)product.ProductOptions.FirstOrDefault(x => x.IsSelected);
                        var selectedProduct = product as RedeamSelectedProduct;
                        if (selectedProduct == null) continue;
                        selectedProduct.Supplier = new Supplier
                        {
                            AddressLine1 = criteria.Booking.User.Address1,
                            ZipCode = criteria.Booking.User.ZipCode,
                            City = criteria.Booking.User.City,
                            PhoneNumber = string.Empty
                        };

                        selectedProduct.ProductOptions[0].Customers[0].Email = criteria.Booking.VoucherEmailAddress;
                        ((RedeamSelectedProduct)redeamSelectedProduct).BookingReferenceNumber = criteria?.Booking?.ReferenceNumber ?? string.Empty;
                        var isHoldable = ((ActivityOption)selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected))?.Holdable ?? false;
                        //Reservation API Call if ticket class is two or three


                        //Set values in  selectedProduct as PrioReservationReference, PrioDistributorReference and PrioBookingStatus
                        if (isHoldable)
                        {
                            var redeamAPIProductResponse = _redeamAdapter.CreateHoldAPIOnly(redeamSelectedProduct, criteria.Token).GetAwaiter().GetResult();

                            if (redeamAPIProductResponse != null)
                            {
                                ((RedeamSelectedProduct)redeamSelectedProduct).HoldId = redeamAPIProductResponse.Hold.Id.ToString();
                                ((RedeamSelectedProduct)redeamSelectedProduct).HoldStatus = redeamAPIProductResponse.Hold.Status;

                                var reservationDetails = new SupplierBookingReservationResponse()
                                {
                                    ApiType = Convert.ToInt32(APIType.Redeam),
                                    ServiceOptionId = selectedOption?.ServiceOptionId ?? selectedOption?.BundleOptionID ?? 0,
                                    AvailabilityReferenceId = product.AvailabilityReferenceId,
                                    Status = redeamAPIProductResponse != null ? Constant.StatusSuccess : Constant.StatusFailed,
                                    BookedOptionId = criteria.Booking.BookingId,
                                    BookingReferenceNo = criteria.Booking.ReferenceNumber,
                                    OptionName = selectedOption?.Name ?? selectedOption.BundleOptionName,
                                    ReservationResponse = SerializeDeSerializeHelper.Serialize(redeamAPIProductResponse),
                                    ReservationReferenceId = redeamAPIProductResponse.Hold.Id.ToString(),
                                    Token = criteria.Token
                                };

                                _tableStorageOperation.InsertReservationDetails(reservationDetails);

                                InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.Reservation, Constant.StatusSuccess);

                                try
                                {
                                    _supplierBookingPersistence.UpdateReserveRequest(criteria.Token, product.AvailabilityReferenceId, criteria.Booking.ReferenceNumber);
                                }
                                catch (Exception ex)
                                {
                                    //ignore
                                }

                                createOrderProducts.Add(product.AvailabilityReferenceId, redeamAPIProductResponse);
                            }
                            else
                            {
                                InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.Reservation, Constant.StatusFailed);

                                //If the create order call is failed for any of the product then set all product booking status as failed
                                bookedProducts = SetFailedBookingStatus(selectedProducts, criteria);

                                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);

                                try
                                {
                                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, product?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                         ((RedeamSelectedProduct)redeamSelectedProduct).HoldId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                        Convert.ToInt32(APIType.Redeam), product?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                        product?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, product?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                    criteria?.Booking?.UpdateDBLogFlag();
                                }
                                catch (Exception e)
                                {
                                    //ignore
                                }

                                return bookedProducts;
                            }
                        }


                        //throw new NullReferenceException("for testing"); //for booking cancel testing
                    }

                    foreach (var product in createOrderProducts)
                    {
                        var redeamSelectedProduct = selectedProducts.OfType<RedeamSelectedProduct>().FirstOrDefault(x => x.AvailabilityReferenceId == product.Key);
                        try
                        {
                            redeamSelectedProduct.ProductOptions = UpdateOptionStatus(redeamSelectedProduct.ProductOptions, ((RedeamSelectedProduct)redeamSelectedProduct).HoldId);

                            var bookedProduct = criteria.Booking.IsangoBookingData?.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == redeamSelectedProduct.AvailabilityReferenceId);

                            if (bookedProduct == null) continue;

                            bookedProduct = MapProductForRedeam(bookedProduct, redeamSelectedProduct);
                            bookedProduct.OptionStatus = Constant.StatusSuccess;
                            bookedProducts.Add(bookedProduct);
                        }
                        catch (Exception ex)
                        {
                            redeamSelectedProduct.ProductOptions = UpdateOptionStatus(redeamSelectedProduct.ProductOptions, ((RedeamSelectedProduct)redeamSelectedProduct).HoldId);
                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == redeamSelectedProduct.AvailabilityReferenceId);
                            if (bookedProduct == null) continue;

                            var option = redeamSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                            bookedProduct.OptionStatus = GetBookingStatusNumber(((RedeamSelectedProduct)redeamSelectedProduct).HoldId, option.AvailabilityStatus);
                            if (!string.IsNullOrWhiteSpace(((RedeamSelectedProduct)redeamSelectedProduct).HoldId))
                            {
                                if (bookedProduct.APIExtraDetail == null)
                                    bookedProduct.APIExtraDetail = new ApiExtraDetail();
                                bookedProduct.APIExtraDetail.SupplieReferenceNumber = ((RedeamSelectedProduct)redeamSelectedProduct).HoldId;
                            }

                            bookedProducts.Add(bookedProduct);
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "SupplierBookingService",
                                MethodName = "CreateRedeamBookingReservation",
                                Token = criteria.Token,
                                Params = $"{redeamSelectedProduct.Id}|{SerializeDeSerializeHelper.Serialize(redeamSelectedProduct)}"
                            };

                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, redeamSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                               ((RedeamSelectedProduct)redeamSelectedProduct).HoldId, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.Redeam), redeamSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                redeamSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, redeamSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (selectedProducts == null || !selectedProducts.Any()) return bookedProducts;
                selectedProducts.ForEach(product =>
                {
                    var bookedProduct = isangoBookedProducts?.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(product.AvailabilityReferenceId));
                    if (bookedProduct != null)
                    {
                        bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                        bookedProducts.Add(bookedProduct);
                    }
                });
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateRedeamBookingReservation",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }



        /// <summary>
        /// Cancel booking call for Redeam
        /// </summary>
        /// <param name="selectedProducts"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Dictionary<string, bool> CancelRedeamBooking(List<SelectedProduct> selectedProducts, string token)
        {
            var cancellationStatus = new Dictionary<string, bool>();
            var request = string.Empty;
            var response = string.Empty;

            var redeamSelectedProducts = selectedProducts?.Cast<RedeamSelectedProduct>()?.ToList();
            var bookingReferenceNumbers = redeamSelectedProducts?.Select(x => x.BookingReferenceNumber)?.Distinct()?.ToList();
            redeamSelectedProducts?.ForEach(x =>
            {
                if (!string.IsNullOrWhiteSpace(x?.BookingReferenceNumber) && !cancellationStatus.Keys.Contains(x.BookingReferenceNumber))
                {
                    cancellationStatus.Add(x.BookingReferenceNumber, false);
                }
            });

            foreach (var bookingReferenceNumber in bookingReferenceNumbers)
            {
                if (!string.IsNullOrWhiteSpace(bookingReferenceNumber))
                {
                    try
                    {
                        var result = _redeamAdapter.CancelBooking(bookingReferenceNumber, token, out request, out response);
                        cancellationStatus[bookingReferenceNumber] = result;
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"SupplierBookingService|CancelRedeamBooking|{SerializeDeSerializeHelper.Serialize(selectedProducts)}", ex);
                    }
                }
            }

            foreach (var redeamSelectedProduct in redeamSelectedProducts)
            {
                try
                {
                    var isCancelled = cancellationStatus?.FirstOrDefault(x => x.Key == redeamSelectedProduct?.BookingReferenceNumber).Value ?? false;

                    var logCriteria = new LogPurchaseXmlCriteria
                    {
                        RequestXml = request,
                        ResponseXml = response,
                        Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed,
                        BookingId = 0,
                        APIType = APIType.Redeam,
                        BookingReferenceNumber = redeamSelectedProduct.BookingReferenceNumber,
                        Bookingtype = "Cancel Booking"
                    };

                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                }
                catch (Exception ex)
                {
                    _log.Error($"SupplierBookingService|CancelRedeamBooking|{SerializeDeSerializeHelper.Serialize(selectedProducts)}", ex);
                }
            }

            return cancellationStatus;
        }


        /// <summary>
        /// Cancel booking call for Redeam
        /// </summary>
        /// <param name="selectedProducts"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Dictionary<string, bool> DeleteHoldRedeamBooking(List<SelectedProduct> selectedProducts, string token)
        {
            var cancellationStatus = new Dictionary<string, bool>();
            var request = string.Empty;
            var response = string.Empty;

            var redeamSelectedProducts = selectedProducts?.Cast<RedeamSelectedProduct>()?.ToList();
            var bookingReferenceNumbers = redeamSelectedProducts?.Select(x => x.BookingReferenceNumber)?.Distinct()?.ToList();
            redeamSelectedProducts?.ForEach(x =>
            {
                if (!string.IsNullOrWhiteSpace(x?.BookingReferenceNumber) && !cancellationStatus.Keys.Contains(x.BookingReferenceNumber))
                {
                    cancellationStatus.Add(x.BookingReferenceNumber, false);
                }
            });

            foreach (var bookingReferenceNumber in bookingReferenceNumbers)
            {
                if (!string.IsNullOrWhiteSpace(bookingReferenceNumber))
                {
                    try
                    {
                        var holdIds = new List<string>
                        {
                         redeamSelectedProducts?.FirstOrDefault()?.HoldId
                        };
                        var result = _redeamAdapter.DeleteHold(holdIds, token)?.GetAwaiter().GetResult();
                        cancellationStatus[bookingReferenceNumber] = result?.FirstOrDefault().Value == "RELEASED" ? true : false;
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"SupplierBookingService|DeleteHoldRedeamBooking|{SerializeDeSerializeHelper.Serialize(selectedProducts)}", ex);
                    }
                }
            }

            foreach (var redeamSelectedProduct in redeamSelectedProducts)
            {
                try
                {
                    var isCancelled = cancellationStatus?.FirstOrDefault(x => x.Key == redeamSelectedProduct?.BookingReferenceNumber).Value ?? false;

                    var logCriteria = new LogPurchaseXmlCriteria
                    {
                        RequestXml = request,
                        ResponseXml = response,
                        Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed,
                        BookingId = 0,
                        APIType = APIType.Redeam,
                        BookingReferenceNumber = redeamSelectedProduct.BookingReferenceNumber,
                        Bookingtype = "Cancel Booking"
                    };

                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                }
                catch (Exception ex)
                {
                    _log.Error($"SupplierBookingService|DeleteHoldRedeamBooking|{SerializeDeSerializeHelper.Serialize(selectedProducts)}", ex);
                }
            }

            return cancellationStatus;
        }

        /// <summary>
        /// Create Rezdy Booking
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<BookedProduct> CreateRezdyBooking(ActivityBookingCriteria criteria)
        {
            var request = string.Empty;
            var response = string.Empty;
            var bookedProducts = new List<BookedProduct>();
            var labelDetails = _masterService.GetLabelDetailsAsync().Result;
            var labelDictionary = new Dictionary<string, string>();

            var rezdySelectedProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.Rezdy));
            try
            {
                foreach (var item in rezdySelectedProducts)
                {
                    var rezdyProduct = _rezdyAdapter.GetProductDetails(item.ProductOptions?.FirstOrDefault().SupplierOptionCode, criteria.Token);
                    ((RezdySelectedProduct)item).RezdyProduct = rezdyProduct;

                    item.CountryCode = criteria?.Booking?.User?.CountryCode;
                }
                var suppliersSelectedProducts = rezdySelectedProducts.GroupBy(x => ((RezdySelectedProduct)x).RezdyProduct?.SupplierId).ToList();

                foreach (var suppliersSelectedProduct in suppliersSelectedProducts)
                {
                    var selectedProducts = suppliersSelectedProduct.ToList();

                    selectedProducts.ForEach(x => x.Supplier = new Supplier
                    {
                        AddressLine1 = criteria.Booking.User.Address1,
                        ZipCode = criteria.Booking.User.ZipCode,
                        City = criteria.Booking.User.City,
                        PhoneNumber = criteria.Booking.User.PhoneNumber,
                        CountryName = criteria?.Booking?.User?.CountryCode,
                        EmailId = criteria.Booking.User.EmailAddress,
                    });
                    var result = _rezdyAdapter.CreateBooking(selectedProducts, criteria.Token, out request, out response);

                    //Prepare BookedProduct with Failed Status
                    if (result == null || result?.Count == 0)
                    {
                        // Mapping the booked product of the IsangoBookingData as per the fail booking
                        var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
                        if (isangoBookedProducts == null) return bookedProducts;
                        foreach (var selectedProduct in rezdySelectedProducts)
                        {
                            var rezdySelectedProduct = (RezdySelectedProduct)selectedProduct;
                            var bookedProduct = isangoBookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId.Equals(rezdySelectedProduct.AvailabilityReferenceId));
                            if (bookedProduct == null) continue;
                            bookedProduct.OptionStatus = string.IsNullOrEmpty(rezdySelectedProduct.OrderNumber) ? ((int)OptionBookingStatus.Failed).ToString() : ((int)OptionBookingStatus.Confirmed).ToString();
                            bookedProducts.Add(bookedProduct);
                        }
                        //Api booking failed
                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);
                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, selectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                            bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.Rezdy), selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }
                        continue;
                    }

                    if (result?.Count > 0)
                    {
                        foreach (var selectedProduct in selectedProducts)
                        {
                            var rezdySelectedProduct = (RezdySelectedProduct)result.FirstOrDefault(x => x.Id == selectedProduct.Id);
                            ((RezdySelectedProduct)selectedProduct).OrderNumber = rezdySelectedProduct?.OrderNumber;
                            selectedProduct.ProductOptions = UpdateOptionStatus(selectedProduct.ProductOptions, rezdySelectedProduct?.OrderNumber);
                            var logCriteria = new LogPurchaseXmlCriteria
                            {
                                RequestXml = request,
                                ResponseXml = response,
                                Status = !string.IsNullOrWhiteSpace(rezdySelectedProduct?.OrderNumber)
                                ? Constant.StatusSuccess : Constant.StatusFailed,
                                BookingId = criteria.Booking.BookingId,
                                BookingReferenceNumber = criteria.Booking.ReferenceNumber,
                                APIType = APIType.Rezdy,
                                ApiRefNumber = rezdySelectedProduct?.OrderNumber
                            };
                            _supplierBookingPersistence.LogPurchaseXML(logCriteria);

                            // Mapping the booked product of the IsangoBookingData as per the success/fail booking
                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);
                            if (bookedProduct == null || rezdySelectedProduct == null) continue;
                            bookedProduct = MapProductForRezdy(bookedProduct, rezdySelectedProduct);

                            bookedProducts.Add(bookedProduct);
                        }
                    }
                }
                return bookedProducts;
            }
            catch (Exception ex)
            {
                var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
                if (isangoBookedProducts == null) return bookedProducts;
                foreach (var selectedProduct in rezdySelectedProducts)
                {
                    var rezdySelectedProduct = (RezdySelectedProduct)selectedProduct;
                    var bookedProduct = isangoBookedProducts.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(rezdySelectedProduct.AvailabilityReferenceId));
                    if (bookedProduct == null) continue;
                    bookedProduct.OptionStatus = string.IsNullOrEmpty(rezdySelectedProduct.OrderNumber) ? ((int)OptionBookingStatus.Failed).ToString() : ((int)OptionBookingStatus.Confirmed).ToString();
                    bookedProducts.Add(bookedProduct);
                }
                _log.Error($"SupplierBookingService|CreateRezdyBooking|{SerializeDeSerializeHelper.Serialize(criteria)}", ex);

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                return bookedProducts;
            }
        }

        /// <summary>
        /// CreateGlobalTixBooking
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<BookedProduct> CreateGlobalTixBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var gtSelectedProducts = criteria?.Booking?.SelectedProducts?.Where(product => product.APIType.Equals(APIType.GlobalTix))?.ToList();
            var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                if (gtSelectedProducts != null && gtSelectedProducts.Count > 0)
                {
                    var bookingReferenceNumber = string.Empty;
                    var selectedProducts = criteria.SelectedProducts;

                    // TODO: Confirm that criteria.Booking.BookingId is Isango booking number
                    Booking gtBooking = _globalTixAdapter.CreateBooking(gtSelectedProducts, criteria.Booking.BookingId.ToString(), criteria.Token, out request, out response);

                    if (gtBooking != null && !string.IsNullOrWhiteSpace(gtBooking?.ReferenceNumber))
                    {
                        bookingReferenceNumber = gtBooking.ReferenceNumber;
                        selectedProducts = gtBooking.SelectedProducts;

                        foreach (var selectedProduct in selectedProducts)
                        {
                            selectedProduct.ProductOptions = UpdateOptionStatus(selectedProduct.ProductOptions, bookingReferenceNumber);

                            var logCriteria = new LogPurchaseXmlCriteria
                            {
                                RequestXml = request,
                                ResponseXml = response,
                                Status = !string.IsNullOrWhiteSpace(gtBooking?.ReferenceNumber)
                                ? Constant.StatusSuccess : Constant.StatusFailed,
                                BookingId = criteria.Booking.BookingId,
                                BookingReferenceNumber = criteria.Booking.ReferenceNumber,
                                APIType = APIType.GlobalTix,
                                ApiRefNumber = gtBooking?.ReferenceNumber
                            };
                            _supplierBookingPersistence.LogPurchaseXML(logCriteria);

                            BookedProduct bookedProduct = isangoBookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);
                            GlobalTixSelectedProduct gtSelProd = selectedProduct as GlobalTixSelectedProduct;
                            if (bookedProduct == null || gtSelProd == null)
                            {
                                continue;
                            }

                            bookedProduct.APIExtraDetail = gtSelProd.APIDetails;
                            bookedProduct.OptionStatus = ((int)selectedProduct.Status).ToString();
                            bookedProducts.Add(bookedProduct);
                        }
                    }
                    else
                    {
                        if (isangoBookedProducts == null) return bookedProducts;
                        foreach (var selectedProduct in gtSelectedProducts)
                        {
                            var gtSelectedProduct = (GlobalTixSelectedProduct)selectedProduct;
                            var bookedProduct = isangoBookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId.Equals(gtSelectedProduct.AvailabilityReferenceId));
                            if (bookedProduct == null) continue;
                            bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                            bookedProducts.Add(bookedProduct);
                        }
                        //Api booking failed
                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , response);
                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, selectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                            bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.GlobalTix), selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }
                    }

                    return bookedProducts;
                }
            }
            catch (Exception ex)
            {
                if (gtSelectedProducts == null || !gtSelectedProducts.Any()) return bookedProducts;

                gtSelectedProducts.ForEach(product =>
                {
                    var bookedProduct = isangoBookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId.Equals(product.AvailabilityReferenceId));
                    bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                    bookedProducts.Add(bookedProduct);
                });

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");
                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, gtSelectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                    bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                    Convert.ToInt32(APIType.GlobalTix), gtSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                    gtSelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, gtSelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error($"SupplierBookingService|CreateGlobalTixBooking|{SerializeDeSerializeHelper.Serialize(criteria)}", ex);
            }
            return bookedProducts;
        }



        /// <summary>
        /// GlobalTix Cancel entire Booking
        /// </summary>
        public Dictionary<string, bool> CancelGlobalTixBooking(List<SelectedProduct> selectedProducts, string token)
        {
            var cancellationStatus = new Dictionary<string, bool>();
            var request = string.Empty;
            var response = string.Empty;

            var globalTixSelectedProducts = selectedProducts?.Cast<GlobalTixSelectedProduct>()?.ToList();
            var bookingReferenceNumbers = globalTixSelectedProducts?.Select(x => x?.APIDetails?.SupplieReferenceNumber)?.Distinct()?.ToList();
            globalTixSelectedProducts?.ForEach(x =>
            {
                cancellationStatus.Add(x?.AvailabilityReferenceId, false);
                //if (!string.IsNullOrWhiteSpace(x?.APIDetails?.SupplieReferenceNumber) && !cancellationStatus.Keys.Contains(x?.APIDetails?.SupplieReferenceNumber))
                //{
                //    cancellationStatus.Add(x?.AvailabilityReferenceId, false);
                //}
                //else if(string.IsNullOrEmpty(x?.APIDetails?.SupplieReferenceNumber))
                //{
                //    cancellationStatus.Add(x?.AvailabilityReferenceId, false);
                //}
            });

            foreach (var bookingReferenceNumber in bookingReferenceNumbers)
            {
                if (!string.IsNullOrWhiteSpace(bookingReferenceNumber))
                {
                    try
                    {
                        var productSelected = globalTixSelectedProducts.Find(thisProduct => thisProduct.APIDetails.SupplieReferenceNumber.Equals(bookingReferenceNumber));
                        bool isNonThailandProduct = !productSelected.RegionId.ToLowerInvariant().Equals("6667");

                        var result = _globalTixAdapter.CancelByBooking(bookingReferenceNumber, token, out request, out response, isNonThailandProduct);
                        cancellationStatus[productSelected?.AvailabilityReferenceId] = result;
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"SupplierBookingService|CancelGlobalTixBooking|{SerializeDeSerializeHelper.Serialize(selectedProducts)}", ex);
                    }
                }
            }

            foreach (var globalTixSelectedProduct in globalTixSelectedProducts)
            {
                try
                {
                    var isCancelled = cancellationStatus?.FirstOrDefault(x => x.Key == globalTixSelectedProduct?.AvailabilityReferenceId).Value ?? false;

                    var logCriteria = new LogPurchaseXmlCriteria
                    {
                        RequestXml = request,
                        ResponseXml = response,
                        Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed,
                        BookingId = 0,
                        APIType = APIType.GlobalTix,
                        BookingReferenceNumber = globalTixSelectedProduct?.APIDetails?.SupplieReferenceNumber,
                        Bookingtype = "Cancel Booking"
                    };

                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                }
                catch (Exception ex)
                {
                    _log.Error($"SupplierBookingService|CancelGlobalTixBooking|{SerializeDeSerializeHelper.Serialize(selectedProducts)}", ex);
                }
            }
            return cancellationStatus;
        }

        /// <summary>
        /// Cancel Rezdy Booking
        /// </summary>
        /// <param name="selectedProducts"></param>
        /// <param name="bookingReferenceNumber"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Dictionary<string, bool> CancelRezdyBooking(List<SelectedProduct> selectedProducts, string bookingReferenceNumber, string token)
        {
            var isCancelled = false;
            var status = new Dictionary<string, bool>();
            var request = string.Empty;
            var response = string.Empty;
            selectedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });
            var logCriteria = new LogPurchaseXmlCriteria
            {
                BookingId = 0,
                APIType = APIType.Rezdy,
                BookingReferenceNumber = bookingReferenceNumber,
                Bookingtype = "Cancel Booking"
            };

            var distinctOrderNumber = selectedProducts.Cast<RezdySelectedProduct>().Select(x => x.OrderNumber).Distinct();

            foreach (var selectedProduct in selectedProducts)
            {
                try
                {
                    var rezdySelectedProduct = (RezdySelectedProduct)selectedProduct;
                    if (distinctOrderNumber.ToList().Contains(rezdySelectedProduct.OrderNumber))
                    {
                        var cancelBookingResponse = _rezdyAdapter.CancelBooking(rezdySelectedProduct.OrderNumber, token, out request, out response);
                        if (cancelBookingResponse == null) // check whether we get null value from adapter call
                        {
                            logCriteria.RequestXml = request;
                            logCriteria.ResponseXml = response;
                            logCriteria.Status = Constant.StatusFailed;
                            _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                            continue;
                        }
                        isCancelled = cancelBookingResponse.Booking.Status.Equals("CANCELLED", StringComparison.InvariantCultureIgnoreCase);
                        status[rezdySelectedProduct.AvailabilityReferenceId] = isCancelled;
                        logCriteria.Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed;
                        if (isCancelled)
                        {
                            distinctOrderNumber.ToList().Remove(rezdySelectedProduct.OrderNumber);
                        }
                        _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"SupplierBookingService|CancelRezdyBooking|{SerializeDeSerializeHelper.Serialize(selectedProduct)}{token}", ex);
                }
            }

            return status;
        }

        /// <summary>
        /// Cancel Rezdy Booking
        /// </summary>
        /// <param name="selectedProducts"></param>
        /// <param name="bookingReferenceNumber"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Dictionary<string, bool> CancelTourCMSBooking(List<SelectedProduct> selectedProducts,
            string bookingReferenceNumber, string token)
        {
            var isCancelled = false;
            var status = new Dictionary<string, bool>();
            var request = string.Empty;
            var response = string.Empty;
            selectedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });
            var logCriteria = new LogPurchaseXmlCriteria
            {
                BookingId = 0,
                APIType = APIType.TourCMS,
                BookingReferenceNumber = bookingReferenceNumber,
                Bookingtype = "Cancel Booking"
            };

            var distinctBookingId = selectedProducts.Cast<TourCMSSelectedProduct>().Select(x => x.BookingId).Distinct();

            foreach (var selectedProduct in selectedProducts)
            {
                try
                {
                    //find tourCMs channelId
                    var prefixServiceCode = selectedProduct?.ProductOptions?.FirstOrDefault()?.PrefixServiceCode;

                    var tourCMSSelectedProduct = (TourCMSSelectedProduct)selectedProduct;
                    if (distinctBookingId.ToList().Contains(tourCMSSelectedProduct.BookingId))
                    {
                        var cancelBookingResponse = _tourCMSAdapter.CancelBooking(tourCMSSelectedProduct.BookingId, prefixServiceCode, token, out request, out response);
                        if (cancelBookingResponse == null) // check whether we get null value from adapter call
                        {
                            logCriteria.RequestXml = request;
                            logCriteria.ResponseXml = response;
                            logCriteria.Status = Constant.StatusFailed;
                            _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                            continue;
                        }
                        isCancelled = cancelBookingResponse.Error.Equals("OK", StringComparison.InvariantCultureIgnoreCase);
                        if (isCancelled == false)
                        {
                            isCancelled = cancelBookingResponse.Error.Equals("PREVIOUSLY CANCELLED", StringComparison.InvariantCultureIgnoreCase);

                        }
                        status[tourCMSSelectedProduct.AvailabilityReferenceId] = isCancelled;
                        logCriteria.RequestXml = request;
                        logCriteria.ResponseXml = response;
                        logCriteria.Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed;
                        if (isCancelled)
                        {
                            distinctBookingId.ToList().Remove(tourCMSSelectedProduct.BookingId);
                        }
                        _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"SupplierBookingService|CancelTourCMSBooking|" +
                        $"{SerializeDeSerializeHelper.Serialize(selectedProduct)}{token}", ex);
                }
            }

            return status;
        }
        public Dictionary<string, bool> DeleteTourCMSBooking(List<SelectedProduct> selectedProducts,
           string bookingReferenceNumber, string token)
        {
            var isCancelled = false;
            var status = new Dictionary<string, bool>();
            var request = string.Empty;
            var response = string.Empty;
            selectedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });
            var logCriteria = new LogPurchaseXmlCriteria
            {
                BookingId = 0,
                APIType = APIType.TourCMS,
                BookingReferenceNumber = bookingReferenceNumber,
                Bookingtype = "Delete Booking"
            };

            var distinctBookingId = selectedProducts.Cast<TourCMSSelectedProduct>().Select(x => x.BookingId).Distinct();

            foreach (var selectedProduct in selectedProducts)
            {
                try
                {
                    //find tourCMs channelId
                    var prefixServiceCode = ((TourCMSSelectedProduct)selectedProduct)?.ShortReference;

                    var tourCMSSelectedProduct = (TourCMSSelectedProduct)selectedProduct;
                    if (distinctBookingId.ToList().Contains(tourCMSSelectedProduct.BookingId))
                    {
                        var deleteBookingResponse = _tourCMSAdapter.DeleteBooking(tourCMSSelectedProduct.BookingId, prefixServiceCode, token, out request, out response);
                        if (deleteBookingResponse == null) // check whether we get null value from adapter call
                        {
                            logCriteria.RequestXml = request;
                            logCriteria.ResponseXml = response;
                            logCriteria.Status = Constant.StatusFailed;
                            _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                            continue;
                        }
                        isCancelled = deleteBookingResponse.Error.Equals("OK", StringComparison.InvariantCultureIgnoreCase);
                        if (isCancelled == false)
                        {
                            isCancelled = deleteBookingResponse.Error.Equals("PREVIOUSLY CANCELLED", StringComparison.InvariantCultureIgnoreCase);

                        }
                        status[tourCMSSelectedProduct.AvailabilityReferenceId] = isCancelled;
                        logCriteria.RequestXml = request;
                        logCriteria.ResponseXml = response;
                        logCriteria.Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed;
                        if (isCancelled)
                        {
                            distinctBookingId.ToList().Remove(tourCMSSelectedProduct.BookingId);
                        }
                        _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"SupplierBookingService|DeleteTourCMSBooking|" +
                        $"{SerializeDeSerializeHelper.Serialize(selectedProduct)}{token}", ex);
                }
            }

            return status;
        }

        /// <summary>
        /// Cancel Rezdy Booking
        /// </summary>
        /// <param name="selectedProducts"></param>
        /// <param name="bookingReferenceNumber"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Dictionary<string, bool> CancelGoCityBooking(List<SelectedProduct>
            selectedProducts, string bookingReferenceNumber, string token,
            string customerEmail)
        {
            var isCancelled = false;
            var status = new Dictionary<string, bool>();
            var request = string.Empty;
            var response = string.Empty;
            selectedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });
            var logCriteria = new LogPurchaseXmlCriteria
            {
                BookingId = 0,
                APIType = APIType.GoCity,
                BookingReferenceNumber = bookingReferenceNumber,
                Bookingtype = "Cancel Booking"
            };

            var distinctBookingId = selectedProducts.Cast<GoCitySelectedProduct>().Select(x => x.OrderNumber).Distinct();

            foreach (var selectedProduct in selectedProducts)
            {
                try
                {

                    var orderNumber = selectedProduct?.ProductOptions?.FirstOrDefault()?.SupplierOptionCode;

                    var goCitySelectedProduct = (GoCitySelectedProduct)selectedProduct;
                    if (distinctBookingId.ToList().Contains(goCitySelectedProduct.OrderNumber))
                    {
                        var cancelBookingResponse = _goCityAdapter.CancelBooking(goCitySelectedProduct.OrderNumber, customerEmail, token, out request, out response);
                        if (cancelBookingResponse == null || cancelBookingResponse == false) // check whether we get null value from adapter call
                        {
                            logCriteria.RequestXml = request;
                            logCriteria.ResponseXml = response;
                            logCriteria.Status = Constant.StatusFailed;
                            _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                            continue;
                        }
                        isCancelled = cancelBookingResponse == true ? true : false;
                        status[goCitySelectedProduct.AvailabilityReferenceId] = isCancelled;
                        logCriteria.RequestXml = request;
                        logCriteria.ResponseXml = response;
                        logCriteria.Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed;
                        if (isCancelled)
                        {
                            distinctBookingId.ToList().Remove(goCitySelectedProduct.OrderNumber);
                        }
                        _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"SupplierBookingService|CancelGoCityBooking|" +
                        $"{SerializeDeSerializeHelper.Serialize(selectedProduct)}{token}", ex);
                }
            }

            return status;
        }

        public Dictionary<string, bool> CancelRaynaBooking(List<RaynaSelectedProduct> raynaSelectedProducts, string bookingReferenceNo, string token)
        {
            var status = new Dictionary<string, bool>();
            raynaSelectedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });

            var isCancelled = false;
            foreach (var raynaSelectedProduct in raynaSelectedProducts)
            {
                try
                {
                    var bookingId = raynaSelectedProduct.BookingId;
                    var referenceno = raynaSelectedProduct.OrderReferenceId;
                    var cancellationReason = "cancel";

                    _log.Info($"SupplierBookingService|CancelRaynaBooking|RaynaCancellationStart");
                    var cancelResponse = _raynaAdapter.CancelBooking(Convert.ToInt32(bookingId), referenceno, cancellationReason, token, out var request, out var response);
                    _log.Info($"SupplierBookingService|CancelRaynaBooking|RaynaCancellationEnd" + cancelResponse.ResultCancel.Status);
                    isCancelled = cancelResponse.ResultCancel.Status == 1 ? true : false;
                    status[raynaSelectedProduct.AvailabilityReferenceId] = isCancelled;

                    var logCriteriaCancel = new LogPurchaseXmlCriteria
                    {
                        RequestXml = request,
                        ResponseXml = response,
                        Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed,
                        BookingId = 0,
                        APIType = APIType.Rayna,
                        BookingReferenceNumber = bookingReferenceNo,
                        Bookingtype = "Cancel Booking"
                    };
                    _supplierBookingPersistence.LogPurchaseXML(logCriteriaCancel);
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = nameof(SupplierBookingService),
                        MethodName = nameof(CancelRaynaBooking),
                        Token = token,
                        Params = $"{SerializeDeSerializeHelper.Serialize(raynaSelectedProduct)}{token}"
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            }
            _log.Info($"SupplierBookingService|CancelRaynaBooking,{SerializeDeSerializeHelper.Serialize(status)}");
            return status;
        }
        #endregion Supplier API booking Methods

        private List<MappedLanguage> GetMappedLanguages()
        {
            return _memCache.GetMappedLanguage();
        }

        private List<TiqetsPackage> GetTiqetsPackages(string Product_ID)
        {
            var result = _masterPersistence.LoadTiqetsPackages(Product_ID);
            return result;
        }

        private List<TiqetsPaxMapping> GetPaxMappingsForAPI(APIType aPIType)
        {
            var allPaxMappings = _masterPersistence.LoadTiqetsPaxMappings();
            var result = allPaxMappings?.Where(x => x.APIType == aPIType)?.ToList();
            return result;
        }

        private List<VentrataPaxMapping> GetPaxMappingsForVentrataAPI(APIType aPIType)
        {
            var allPaxMappings = _masterPersistence.GetVentrataPaxMappings();
            var result = allPaxMappings?.Where(x => x.APIType == aPIType)?.ToList();
            return result;
        }

        private List<VentrataPackages> GetVentrataPackages(string productid, string optionid)
        {
            var result = _masterPersistence.LoadVentrataPackages(productid, optionid);
            return result;
        }

        private void InsertAsyncJob(string orderReferenceId
            , string languageCode
            , string token
            , ActivityBookingCriteria criteria
            , SelectedProduct tiqetsSelectedProduct
            , string affiliateID
            , int apiTypeId = 0
            , string apiTypeMethod = ""
            , string apiDistributerID = ""
            , int retryThreshold = 6
        )
        {
            var interval = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsAsyncCallInterval));

            var bookedoption = tiqetsSelectedProduct.ProductOptions.FirstOrDefault();

            var asyncBooking = new AsyncBooking
            {
                OrderReferenceId = orderReferenceId,
                LanguageCode = languageCode,
                RetryInterval = interval,
                RetryThreshold = retryThreshold,
                ApiType = apiTypeId,
                Token = token,
                NextProcessingTime = DateTime.UtcNow.AddMinutes(interval).ToString(CultureInfo.InvariantCulture),
                RetryCount = 0,
                CustomerEmail = criteria?.Booking?.User?.EmailAddress,
                BookingReferenceNo = criteria?.Booking?.ReferenceNumber,
                Status = AsyncBookingStatus.ToDo.ToString(),
                AvailabilityReferenceId = tiqetsSelectedProduct.AvailabilityReferenceId,
                OptionName = bookedoption?.Name,
                ServiceOptionId = Convert.ToInt32(bookedoption.ServiceOptionId),
                AffiliateId = affiliateID,
                ApiTypeMethod = apiTypeMethod,
                ApiDistributerId = apiDistributerID
            };

            _tableStorageOperation.InsertAsyncBookingDetails(asyncBooking);
        }

        /// <summary>
        /// MapProductForRezdy
        /// </summary>
        /// <param name="bookedProduct"></param>
        /// <param name="selectedProduct"></param>
        /// <returns></returns>
        private BookedProduct MapProductForRezdy(BookedProduct bookedProduct, RezdySelectedProduct selectedProduct)
        {
            var apiExtraDetail = bookedProduct.APIExtraDetail;
            if (apiExtraDetail == null) apiExtraDetail = new ApiExtraDetail();
            bookedProduct.PickUpLocation = selectedProduct.HotelPickUpLocation;
            var selectedOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
            //apiExtraDetail.QRCode = selectedOption.Qrc;
            var noOfPassengers = selectedOption?.TravelInfo?.NoOfPassengers?.Values.Sum(x => x);
            var noOfBarcodeCodes = selectedProduct?.Barcodes?.Count();
            //var isQRCodePerPax = noOfPassengers.Equals(noOfBarcodeCodes);
            var isQRCodePerPax = noOfBarcodeCodes > 1;
            apiExtraDetail.IsQRCodePerPax = isQRCodePerPax;
            //if (selectedOption?.TravelInfo?.NoOfPassengers.Keys.FirstOrDefault() == PassengerType.Family
            //    || selectedOption?.TravelInfo?.NoOfPassengers.Keys.FirstOrDefault() == PassengerType.Family2
            //   )
            //{
            //    apiExtraDetail.IsQRCodePerPax = true;
            //}

            apiExtraDetail.APIOptionName = selectedOption?.Name;
            apiExtraDetail.SupplierCancellationPolicy = selectedProduct.SupplierCancellationPolicy = selectedOption.ApiCancellationPolicy;
            apiExtraDetail.SupplieReferenceNumber = selectedProduct?.OrderNumber;
            var barCodeString = GetBarCodeStringForRezdy(selectedProduct.Barcodes);
            apiExtraDetail.QRCode = noOfBarcodeCodes == 1 ? $"{Isango.Mailer.Constants.Constant.QRCode}~{barCodeString}" : string.Empty;
            apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(Constant.String,APIType.Rezdy);

            apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
            if (noOfBarcodeCodes > 1)
            {
                foreach (var barcodeField in selectedProduct.Barcodes)
                {
                    var apiTicketDetail = new ApiTicketDetail
                    {
                        BarCode = barcodeField.BarCode,
                        SeatId = "1",
                        FirstName = barcodeField.FirstName,
                        LastName = barcodeField.LastName
                    };
                    apiExtraDetail.APITicketDetails.Add(apiTicketDetail);
                }
            }

            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct.OrderNumber, selectedOption?.AvailabilityStatus);
            return bookedProduct;
        }

        /// <summary>
        /// GetBarCodeStringForRezdy
        /// </summary>
        /// <param name="barCodes"></param>
        /// <returns></returns>
        private string GetBarCodeStringForRezdy(List<Barcode> barCodes)
        {
            var concatenatedCodes = string.Empty;
            if (barCodes != null && barCodes.Count() > 1)
            {
                foreach (var barcode in barCodes)
                {
                    concatenatedCodes = concatenatedCodes + "," + barcode.BarCode;
                }
            }
            else if (barCodes != null)
            {
                concatenatedCodes = barCodes?.FirstOrDefault()?.BarCode;
            }
            return concatenatedCodes;
        }

        public List<BookedProduct> CreateTourCMSActivityBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var selectedProducts = criteria.Booking.SelectedProducts.Where(x => x.APIType.Equals(APIType.TourCMS)).ToList();

            var request = string.Empty;
            var response = string.Empty;

            try
            {
                var logPurchaseCriteria = new LogPurchaseXmlCriteria
                {
                    BookingId = criteria.Booking.BookingId,
                    APIType = APIType.TourCMS,
                    BookingReferenceNumber = criteria.Booking.ReferenceNumber
                };


                if (selectedProducts != null && selectedProducts.Count > 0)
                {
                    foreach (var selectedProduct in selectedProducts)
                    {
                        var tourCMSSelectedProduct = (TourCMSSelectedProduct)selectedProduct;

                        var rowKey = $"{criteria.Booking.ReferenceNumber}-{selectedProduct.AvailabilityReferenceId}";
                        var ReservationDetails = _tableStorageOperation.RetrieveReservationData(rowKey);
                        var createOrderResponse = SerializeDeSerializeHelper.DeSerialize<NewBookingResponse>(ReservationDetails?.ReservationResponse);
                        var result = new Tuple<string, string>("", "");
                        if (ReservationDetails != null)
                        {
                            result = TourCMSReservationGet(tourCMSSelectedProduct, createOrderResponse, criteria.Booking, criteria.Token);
                        }
                        //End- Get Reservation from Storage
                        if (ReservationDetails == null && (String.IsNullOrEmpty(Convert.ToString(tourCMSSelectedProduct.BookingId)) || tourCMSSelectedProduct.BookingId == 0))
                        {
                            var getSelectedProducts = _tourCMSAdapter.CreateReservationSingle(
                         selectedProduct, criteria.Booking?.Language?.Code, criteria.Booking?.VoucherEmailAddress,
                         criteria.Booking?.VoucherPhoneNumber, criteria.Booking?.ReferenceNumber,
                         criteria.Token, out request, out response);
                        }

                        //Reservation Success
                        if (tourCMSSelectedProduct?.BookingId > 0)
                        {
                            if (!string.IsNullOrEmpty(request))
                            {
                                logPurchaseCriteria.RequestXml = request;
                                logPurchaseCriteria.ResponseXml = response;
                                logPurchaseCriteria.Bookingtype = Constant.StatusReservation;
                                logPurchaseCriteria.Status = Constant.StatusSuccess;

                                // log purchase criteria for CreateReservation call
                                InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.CreateReservation, Constant.StatusSuccess);
                            }
                            _tourCMSAdapter.CommitBookingSingle(selectedProduct, criteria.Token, out request, out response);
                        }
                        else
                        {
                            logPurchaseCriteria.RequestXml = request;
                            logPurchaseCriteria.ResponseXml = response;
                            logPurchaseCriteria.Status = Constant.StatusFailed;
                            logPurchaseCriteria.Bookingtype = Constant.StatusReservation;
                            InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.CreateReservation, Constant.StatusFailed);

                            foreach (var tourCMSProduct in selectedProducts)
                            {
                                var bookedProductData = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                    x.AvailabilityReferenceId == tourCMSProduct.AvailabilityReferenceId);
                                if (bookedProductData == null) continue;
                                bookedProductData.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                                bookedProducts.Add(bookedProductData);
                            }
                            //Api booking failed
                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                    , System.Net.HttpStatusCode.BadGateway
                                    , response);
                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, bookedProducts?.FirstOrDefault()?.ServiceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.TourCMS), bookedProducts?.FirstOrDefault()?.OptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                bookedProducts?.FirstOrDefault()?.OptionName ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, bookedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            return bookedProducts;
                        }



                        var bbSelectedProduct = (TourCMSSelectedProduct)selectedProduct;
                        var bookingStatus = selectedProduct == null ? TourCMSApiStatus.Cancelled.ToString() : bbSelectedProduct?.BookingStatus;

                        if (bookingStatus == (Convert.ToInt32(TourCMSApiStatus.Confirmed)).ToString())
                        {

                            logPurchaseCriteria.RequestXml = request;
                            logPurchaseCriteria.ResponseXml = response;
                            logPurchaseCriteria.Bookingtype = Constant.StatusBooking;
                            logPurchaseCriteria.Status = bookingStatus == (Convert.ToInt32(TourCMSApiStatus.Confirmed)).ToString() ?
                                Constant.StatusSuccess : Constant.StatusFailed;

                            // log purchase criteria for CreateBooking call
                            InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.CreateBooking, bookingStatus);
                            ///end Reservation
                            ///
                            var bookedSelectedProduct = (TourCMSSelectedProduct)(selectedProduct);
                            var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == bookedSelectedProduct.AvailabilityReferenceId);
                            bookedProduct = MapProductForTourCMS(bookedProduct, bookedSelectedProduct);
                            bookedProducts.Add(bookedProduct);
                        }
                        else
                        {

                            logPurchaseCriteria.RequestXml = request;
                            logPurchaseCriteria.ResponseXml = response;
                            logPurchaseCriteria.Bookingtype = Constant.StatusBooking;
                            logPurchaseCriteria.Status = Constant.StatusFailed;
                            _supplierBookingPersistence.LogPurchaseXML(logPurchaseCriteria);

                            foreach (var tourCMSProduct in selectedProducts)
                            {
                                var bookedProductData = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                    x.AvailabilityReferenceId == tourCMSProduct.AvailabilityReferenceId);
                                if (bookedProductData == null) continue;
                                bookedProductData.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                                bookedProducts.Add(bookedProductData);
                            }
                            //Api booking failed
                            criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                    , System.Net.HttpStatusCode.BadGateway
                                    , response);

                            try
                            {
                                LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, bookedProducts?.FirstOrDefault()?.ServiceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.TourCMS), bookedProducts?.FirstOrDefault()?.OptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                bookedProducts?.FirstOrDefault()?.OptionName ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, bookedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                criteria?.Booking?.UpdateDBLogFlag();
                            }
                            catch (Exception e)
                            {
                                //ignore
                            }

                            return bookedProducts;
                            //break;
                        }
                        selectedProduct.ProductOptions = UpdateOptionStatus(selectedProduct.ProductOptions, ((TourCMSSelectedProduct)selectedProduct).BookingReference);
                    }
                }
            }
            catch (Exception ex)
            {
                foreach (var selectedProduct in selectedProducts)
                {
                    var option = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);

                    var bookingReference = ((TourCMSSelectedProduct)selectedProduct).ReservationReference;
                    if (bookedProduct == null) continue;
                    bookedProduct.OptionStatus = GetBookingStatusNumber(bookingReference, option?.AvailabilityStatus);
                    bookedProducts.Add(bookedProduct);
                }

                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateTourCMSActivityBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, bookedProducts?.FirstOrDefault()?.ServiceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.TourCMS), bookedProducts?.FirstOrDefault()?.OptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        bookedProducts?.FirstOrDefault()?.OptionName ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, bookedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }





        private BookedProduct MapProductForNewCitySightSeeing(BookedProduct bookedProduct, NewCitySightSeeingSelectedProduct selectedProduct)
        {
            bookedProduct.APIExtraDetail = new ApiExtraDetail();
            var apiExtraDetail = bookedProduct?.APIExtraDetail;
            var selectedOption = selectedProduct?.ProductOptions?.FirstOrDefault(x => x.IsSelected);
            apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
            apiExtraDetail.SupplierLineNumber = selectedProduct?.ShortReference;
            apiExtraDetail.SupplieReferenceNumber = selectedProduct?.SupplierReferenceNumber;

            var noOfQRCodes = selectedProduct?.NewCitySightSeeingLines?.Count();
            var isQRCodePerPax = noOfQRCodes > 1;
            apiExtraDetail.IsQRCodePerPax = isQRCodePerPax;

            if (!isQRCodePerPax)
            {
                apiExtraDetail.QRCode = selectedProduct?.NewCitySightSeeingLines?.FirstOrDefault()?.QrCode;
                apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(Constant.String,APIType.NewCitySightSeeing);
            }

            if (selectedProduct.NewCitySightSeeingLines != null)
            {
                foreach (var ticket in selectedProduct?.NewCitySightSeeingLines)
                {
                    var apiTicketDetail = new ApiTicketDetail
                    {
                        BarCode = ticket?.QrCode,
                        PassengerType = ticket?.Rate,
                        SeatId = Convert.ToString(ticket?.Quantity),
                        APIOrderId = ticket?.OrderLineCode
                    };
                    apiExtraDetail.APITicketDetails.Add(apiTicketDetail);
                }
            }

            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct?.BookingReference, selectedOption?.AvailabilityStatus);

            return bookedProduct;
        }

        public Dictionary<string, bool> CancelNewCitySightSeeingBooking(
            List<SelectedProduct> selectedProducts,
            string bookingReferenceNumber, string token)
        {
            var isCancelled = false;
            var status = new Dictionary<string, bool>();
            var request = string.Empty;
            var response = string.Empty;
            selectedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });
            var logCriteria = new LogPurchaseXmlCriteria
            {
                BookingId = 0,
                APIType = APIType.NewCitySightSeeing,
                BookingReferenceNumber = bookingReferenceNumber,
                Bookingtype = "Cancel Booking"
            };

            var distinctBookingId = selectedProducts.Cast<NewCitySightSeeingSelectedProduct>().Select(x => x.NewCitySightSeeingReservationId).Distinct();

            foreach (var selectedProduct in selectedProducts)
            {
                try
                {
                    //find tourCMs channelId
                    var prefixServiceCode = selectedProduct?.ProductOptions?.FirstOrDefault()?.PrefixServiceCode;

                    var tourCMSSelectedProduct = (NewCitySightSeeingSelectedProduct)selectedProduct;
                    if (distinctBookingId.ToList().Contains(tourCMSSelectedProduct.NewCitySightSeeingReservationId))
                    {
                        var cancelBookingResponse = _newCitySightSeeingAdapter.CancelBooking(tourCMSSelectedProduct.NewCitySightSeeingReservationId, bookingReferenceNumber, token, out request, out response);
                        if (cancelBookingResponse == null) // check whether we get null value from adapter call
                        {
                            logCriteria.RequestXml = request;
                            logCriteria.ResponseXml = Constant.StatusSuccess;
                            logCriteria.Status = Constant.StatusSuccess;
                            isCancelled = true;

                            status[tourCMSSelectedProduct.AvailabilityReferenceId] = isCancelled;
                            logCriteria.Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed;
                            if (isCancelled)
                            {
                                distinctBookingId.ToList().Remove(tourCMSSelectedProduct.NewCitySightSeeingReservationId);
                            }
                            _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                        }
                        else
                        {
                            logCriteria.RequestXml = request;
                            logCriteria.ResponseXml = Constant.FailedStatus;
                            logCriteria.Status = Constant.FailedStatus;
                            _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"SupplierBookingService|CancelNewCitySightSeeingBooking|" +
                        $"{SerializeDeSerializeHelper.Serialize(selectedProduct)}{token}", ex);
                }
            }

            return status;
        }

        public List<BookedProduct> CreateGoCityBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var selectedProducts = criteria.Booking.SelectedProducts.
                Where(x => x.APIType.Equals(APIType.GoCity)).ToList();

            var request = string.Empty;
            var response = string.Empty;

            try
            {
                var logPurchaseCriteria = new LogPurchaseXmlCriteria
                {
                    BookingId = criteria.Booking.BookingId,
                    APIType = APIType.GoCity,
                    BookingReferenceNumber = criteria.Booking.ReferenceNumber
                };

                var bookedSelectedProducts = _goCityAdapter.CreateBooking
                    (criteria.Booking, criteria.Token, out request, out response);

                var bbSelectedProduct = (GoCitySelectedProduct)bookedSelectedProducts;
                var bookingStatus = bbSelectedProduct?.APIStatus?.ToUpperInvariant() == "COMPLETED" ? "Confirmed" : "CANCELLED";

                logPurchaseCriteria.RequestXml = request;
                logPurchaseCriteria.ResponseXml = response;
                logPurchaseCriteria.Bookingtype = Constant.StatusBooking;
                logPurchaseCriteria.Status = bookingStatus == "Confirmed" ?
                    Constant.StatusSuccess : Constant.StatusFailed;

                // log purchase criteria for CreateBooking call
                InsertLogPurchaseInDb(logPurchaseCriteria, request, response, Constant.CreateBooking, bookingStatus);

                foreach (var selectedProduct in selectedProducts)
                {
                    var bookedSelectedProduct = (GoCitySelectedProduct)bookedSelectedProducts;

                    bookedSelectedProduct.ProductOptions = UpdateOptionStatus(selectedProduct.ProductOptions, bookedSelectedProduct.OrderNumber);

                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == bookedSelectedProduct.AvailabilityReferenceId);

                    if (bookedProduct == null)
                    {
                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);

                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, selectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                                bookedSelectedProduct?.SupplierReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                                Convert.ToInt32(APIType.GoCity), selectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                selectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }

                        continue;
                    }
                    bookedProduct = MapProductForGoCity(bookedProduct, bookedSelectedProduct);
                    bookedProducts.Add(bookedProduct);
                }

            }
            catch (Exception ex)
            {
                foreach (var selectedProduct in selectedProducts)
                {
                    var option = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                    var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.
                        FirstOrDefault(x => x.AvailabilityReferenceId == selectedProduct.AvailabilityReferenceId);

                    var bookingReference = ((GoCitySelectedProduct)selectedProduct).OrderNumber;
                    if (bookedProduct == null) continue;
                    bookedProduct.OptionStatus = GetBookingStatusNumber(bookingReference, option?.AvailabilityStatus);
                    bookedProducts.Add(bookedProduct);
                }

                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingService",
                    MethodName = "CreateGoCityBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, selectedProducts?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        bookedProducts?.FirstOrDefault()?.APIExtraDetail?.SupplieReferenceNumber, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(APIType.GoCity), selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, selectedProducts?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ex);
            }
            return bookedProducts;
        }

        ///<summary>
        ///Create Redeam bookings
        ///</summary>
        ///<param name="criteria"></param>
        ///<returns></returns>
        public List<BookedProduct> CreateRaynaBooking(ActivityBookingCriteria criteria)
        {
            var bookedProducts = new List<BookedProduct>();
            var request = string.Empty;
            var response = string.Empty;
            var selectedProducts = criteria.Booking.SelectedProducts.Where(product => product.APIType.Equals(APIType.Rayna)).ToList();
            try
            {
                //foreach (var selectedProduct in selectedProducts)
                //{
                var raynaSelectedProductList = selectedProducts;
                var raynaSelectedProductSingle = selectedProducts.FirstOrDefault();
                foreach (var selectedProduct in selectedProducts)
                {
                    ((RaynaSelectedProduct)raynaSelectedProductSingle).BookingReferenceNumber = criteria?.Booking?.ReferenceNumber ?? string.Empty;
                }
                var bookingReference = criteria?.Booking?.ReferenceNumber;
                var voucherPhoneNumber = criteria?.Booking?.VoucherPhoneNumber;
                // Supplier Create Booking call initiated
                raynaSelectedProductList = _raynaAdapter.BookingConfirm(raynaSelectedProductList,
                    bookingReference, voucherPhoneNumber, criteria.Token, out request, out response);

                //Prepare BookedProduct with Failed Status
                if (raynaSelectedProductList == null || raynaSelectedProductList.Count == 0)
                {
                    foreach (var raynaSelectedProduct in raynaSelectedProductList)
                    {
                        //Api booking failed
                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                            , System.Net.HttpStatusCode.BadGateway
                            , response);
                        try
                        {
                            LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, raynaSelectedProduct?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                            string.Empty, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                            Convert.ToInt32(APIType.Rayna), raynaSelectedProduct?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            raynaSelectedProduct?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, raynaSelectedProduct?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }

                        // Mapping the booked product of the IsangoBookingData as per the fail booking
                        var raynaBookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                            x.AvailabilityReferenceId == raynaSelectedProduct.AvailabilityReferenceId);
                        if (raynaBookedProduct == null) continue;

                        raynaBookedProduct = MapProductForRayna(raynaBookedProduct, raynaSelectedProduct as RaynaSelectedProduct);
                        bookedProducts.Add(raynaBookedProduct);
                    }
                }
                if (raynaSelectedProductList != null || raynaSelectedProductList?.Count > 0)
                {
                    foreach (var raynaSelectedProduct in raynaSelectedProductList)
                    {
                        var raynaSelectedBookedProduct = (RaynaSelectedProduct)raynaSelectedProduct;
                        //Setting the booking status of all the product options by checking the supplier's TicketReferenceNumber
                        raynaSelectedProduct.ProductOptions = UpdateOptionStatus(raynaSelectedBookedProduct.ProductOptions, raynaSelectedBookedProduct.BookingReferenceNumber);

                        // Logging the supplier booking details in the DB
                        var logCriteria = new LogPurchaseXmlCriteria
                        {
                            RequestXml = request,
                            ResponseXml = response,
                            Status = !string.IsNullOrWhiteSpace(raynaSelectedBookedProduct.BookingReferenceNumber)
                                ? Constant.StatusSuccess
                                : Constant.StatusFailed,
                            BookingId = criteria.Booking.BookingId,
                            BookingReferenceNumber = criteria.Booking.ReferenceNumber,
                            APIType = APIType.Rayna,
                            ApiRefNumber = raynaSelectedBookedProduct.BookingReferenceNumber
                        };
                        _supplierBookingPersistence.LogPurchaseXML(logCriteria);

                        // Mapping the booked product of the IsangoBookingData as per the success/fail booking
                        var bookedProduct = criteria.Booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                            x.AvailabilityReferenceId == raynaSelectedProduct.AvailabilityReferenceId);
                        if (bookedProduct == null || raynaSelectedProduct == null) continue;
                        bookedProduct = MapProductForRayna(bookedProduct, raynaSelectedBookedProduct);
                        if (bookedProduct != null)
                        {
                            bookedProducts.Add(bookedProduct);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoBookedProducts = criteria?.Booking?.IsangoBookingData?.BookedProducts;
                if (isangoBookedProducts == null) return bookedProducts;

                foreach (var selectedProduct in selectedProducts)
                {
                    var bookedProduct = isangoBookedProducts.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(selectedProduct.AvailabilityReferenceId));

                    if (bookedProduct == null) continue;
                    // Checking if already contains the booked product, to maintain the booking status
                    if (bookedProducts.Contains(bookedProduct)) continue;

                    bookedProduct.OptionStatus = ((int)OptionBookingStatus.Failed).ToString();
                    if (bookedProduct != null)
                    {
                        bookedProducts.Add(bookedProduct);
                    }
                }

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , $"Exception\n {ex.Message}\n{response}");

                _log.Error($"SupplierBookingService|CreateRaynaProductsBooking|{SerializeDeSerializeHelper.Serialize(criteria)}", ex);
            }
            return bookedProducts;
        }
        private BookedProduct MapProductForGoCity(BookedProduct bookedProduct, GoCitySelectedProduct selectedProduct)
        {
            bookedProduct.APIExtraDetail = new ApiExtraDetail();
            var apiExtraDetail = bookedProduct.APIExtraDetail;
            var selectedOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
            apiExtraDetail.APITicketDetails = new List<ApiTicketDetail>();
            apiExtraDetail.SupplierLineNumber = selectedProduct.ShortReference;
            apiExtraDetail.SupplieReferenceNumber = selectedProduct.SupplierReferenceNumber;

            //var noOfQRCodes = selectedProduct.Passlist?.Count();
            //var isQRCodePerPax = noOfQRCodes > 1;
            //apiExtraDetail.IsQRCodePerPax = isQRCodePerPax;
            apiExtraDetail.QRCodeType = MapAPICodeFormatWithIsangoCode(Constant.TicketPdfType,APIType.GoCity);
            apiExtraDetail.QRCode = selectedProduct?.PrintPassesUrl + "&ticketOnly=true";
            bookedProduct.APIExtraDetail = apiExtraDetail;
            bookedProduct.OptionStatus = GetBookingStatusNumber(selectedProduct.BookingReference, selectedOption?.AvailabilityStatus);
            return bookedProduct;
            //if (!isQRCodePerPax)
            //{
            //    apiExtraDetail.QRCodeType = Constant.TicketPdfType;
            //    apiExtraDetail.QRCode = selectedProduct?.PrintPassesUrl+"&ticketOnly=true";
            //    //apiExtraDetail.QRCode = selectedProduct.Passlist?.FirstOrDefault()?.SkuCode;
            //    //apiExtraDetail.QRCodeType = Constant.String;
            //}

            //if (selectedProduct.Passlist != null)
            //{
            //    foreach (var ticket in selectedProduct.Passlist)
            //    {
            //        var apiTicketDetail = new ApiTicketDetail
            //        {
            //            CodeType = Constant.TicketPdfType,
            //            CodeValue = selectedProduct?.PrintPassesUrl+"&ticketOnly=true",
            //            BarCode = selectedProduct?.PrintPassesUrl+"&ticketOnly=true",
            //            APIOrderId = ticket?.ConfirmationCode
            //        };
            //        apiExtraDetail.APITicketDetails.Add(apiTicketDetail);
            //    }
            //}


        }
        public List<Entities.Ventrata.SupplierDetails> GetVentrataData()
        {
            try
            {
                var memCache = MemoryCache.Default;
                var key = "getventratasupplierdata";
                var res = memCache.Get(key);
                if (res != null)
                {
                    return (List<Entities.Ventrata.SupplierDetails>)res;
                }
                else
                {
                    var VentrataData = _masterPersistence.GetVentrataSupplierDetails();
                    memCache.Add(key, VentrataData, DateTimeOffset.UtcNow.AddMinutes(5));
                    return VentrataData;
                }
            }
            catch (Exception ex)
            {
                _log.Error("SupplierBookingService|GetVentrataData", ex);
                throw;
            }
        }

        public void LogBookingFailureInDB(Booking failedBooking, string bookingRefNo, int? serviceID, string tokenID, string apiRefID, string custEmail, string custContact, int? ApiType, int? optionID, string optionName, string avlbltyRefID, string ErrorLevel)
        {
            Task.Run(() => _supplierBookingPersistence.LogBookingFailureInDB(failedBooking, bookingRefNo, serviceID, tokenID, apiRefID, custEmail, custContact, ApiType, optionID, optionName, avlbltyRefID, ErrorLevel));
        }
        public Dictionary<string, bool> PrioHubCancelReservationAndBooking(
            List<PrioHubSelectedProduct> prioHubSelectedProducts,
            string bookingReferenceNumber, string token,
            string languageCode = "", string affiliateId = "", string email = "")
        {
            var status = new Dictionary<string, bool>();
            var request = String.Empty;
            var response = String.Empty;
            prioHubSelectedProducts.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });

            var isCancelled = false;

            var logCriteria = new LogPurchaseXmlCriteria
            {
                BookingId = 0,
                APIType = APIType.PrioHub,
                BookingReferenceNumber = bookingReferenceNumber
            };

            foreach (var prioHubSelectedProduct in prioHubSelectedProducts)
            {
                try
                {
                    //Case1: If Product is Reserved and not Booked.
                    //Case2: If Product is Reserved and Booked.
                    //Cancel Reservation Only in case of Ticket Class 2 and Ticket Class 3
                    var errorCode = prioHubSelectedProduct?.PrioHubApiConfirmedBooking?.ErrorCode;
                    var prioApiConfirmedBooking = prioHubSelectedProduct?.PrioHubApiConfirmedBooking?.BookingStatus;

                    //Cancel Reservation
                    //Case1: If Product is Reserved and not Booked.
                    if (string.Equals(prioHubSelectedProduct.PrioHubReservationStatus, ConstantPrioHub.BOOKINGRESERVED,
                            StringComparison.CurrentCultureIgnoreCase) &&
                        prioHubSelectedProduct.PrioHubApiConfirmedBooking == null)
                    {
                        var cancellationStatus =
                            _prioHubAdapter.CancelReservation(prioHubSelectedProduct,
                                token, out request, out response);
                        if (cancellationStatus.Item3 == ConstantPrioHub.BOOKINGRESERVATIONCANCELLED.ToString())
                        {
                            logCriteria.Bookingtype = ConstantPrioHub.CancelReservation;
                            isCancelled = true;
                        }
                    }
                    //Case2: If Product is Reserved and Booked.
                    else if (string.Equals(prioHubSelectedProduct.PrioHubReservationStatus, ConstantPrioHub.BOOKINGRESERVED,
                                 StringComparison.CurrentCultureIgnoreCase)
                                 && prioHubSelectedProduct.PrioHubApiConfirmedBooking != null
                                 && string.Equals(prioApiConfirmedBooking, ConstantPrioHub.BOOKINGCONFIRMED,
                                 StringComparison.CurrentCultureIgnoreCase)
                                 && string.IsNullOrEmpty(errorCode)
                            )
                    {
                        //Cancel Booking
                        var cancellationStatusGet = _prioHubAdapter.CancelBooking(prioHubSelectedProduct, token, out request, out response);
                        var cancellationStatus = cancellationStatusGet?.Item3;
                        if (cancellationStatus == ConstantPrioHub.ORDERCANCELLED)
                        {
                            logCriteria.Bookingtype = ConstantPrioHub.CancelBooking;
                            isCancelled = true;
                        }
                        else
                        {
                            var cancellationStatusError = cancellationStatusGet?.Item4;
                            var cancellationStatusErrorDesc = cancellationStatusGet?.Item5;
                            //"error_message": "This reservation can not be cancelled.
                            //Please check the cancellation policy on the voucher.",
                            if (cancellationStatusError?.ToUpper() == ConstantPrioHub.INVALIDCANCELLATON && !string.IsNullOrEmpty(cancellationStatusErrorDesc)
                                && cancellationStatusErrorDesc.Contains(ConstantPrioHub.CancellationPolicy))
                            {
                                //ignore this case.depend on cancelaltion policy, date is over
                            }
                            else
                            {
                                //data saved in Queue
                                try
                                {
                                    var apibookingRef = prioHubSelectedProduct?.PrioHubApiConfirmedBooking?.BookingReference ?? null;

                                    var criteria = new ActivityBookingCriteria
                                    {
                                        Booking = new Booking
                                        {
                                            User = new ISangoUser
                                            {
                                                EmailAddress = email
                                            },
                                            ReferenceNumber = bookingReferenceNumber
                                        }
                                    };

                                    if (cancellationStatus == ConstantPrioHub.BOOKINGPROCESSINGCANCELLATION)
                                    {
                                        InsertAsyncJob(apibookingRef + criteria.Booking.ReferenceNumber, languageCode, token,
                                         criteria, prioHubSelectedProduct, affiliateId, Convert.ToInt32(APIType.PrioHub), ConstantPrioHub.CANCELWEBHOOK.ToLower(), prioHubSelectedProduct?.PrioHubDistributerId, 3);
                                    }
                                    else
                                    {
                                        InsertAsyncJob(apibookingRef + criteria.Booking.ReferenceNumber, languageCode, token,
                                            criteria, prioHubSelectedProduct, affiliateId, Convert.ToInt32(APIType.PrioHub), ConstantPrioHub.CANCEL.ToLower(), prioHubSelectedProduct?.PrioHubDistributerId, 3);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var isangoErrorEntity = new IsangoErrorEntity
                                    {
                                        ClassName = "SupplierBookingService",
                                        MethodName = "PrioHubCancelReservationAndBooking",
                                        Token = token,
                                        Params = $"{prioHubSelectedProduct.Id}|{SerializeDeSerializeHelper.Serialize(prioHubSelectedProduct)}"
                                    };
                                    _log.Error(isangoErrorEntity, ex);
                                }
                            }

                        }
                    }


                    logCriteria.RequestXml = request;
                    logCriteria.ResponseXml = response;
                    logCriteria.Status = isCancelled ? Constant.StatusSuccess : Constant.StatusFailed;

                    _supplierBookingPersistence.LogPurchaseXML(logCriteria);
                    status[prioHubSelectedProduct.AvailabilityReferenceId] = isCancelled;
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "SupplierBookingService",
                        MethodName = "PrioHubCancelReservationAndBooking",
                        Token = token,
                        Params = $"{SerializeDeSerializeHelper.Serialize(prioHubSelectedProducts)}{bookingReferenceNumber}{token}"
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            }

            return status;
        }

        private string MapAPICodeFormatWithIsangoCode(string apiCodeFormat, APIType apiType)
        {
            var returnIsangoCodeFormat = apiCodeFormat;
            if (!string.IsNullOrEmpty(apiCodeFormat))
            {
                switch (apiType)
                {
                    case APIType.PrioHub:
                    case APIType.Prio:
                        {
                            if (apiCodeFormat.ToUpper().Contains(Constant.PrioHub_QRCODE.ToUpper()))
                            {
                                returnIsangoCodeFormat = Constant.IsangoQRCODE;
                            }
                            else if (apiCodeFormat.ToUpper().Contains(Constant.PrioHub_BARCODE.ToUpper()))
                            {
                                returnIsangoCodeFormat = Constant.IsangoBARCODE;
                            }
                            else if (apiCodeFormat.ToUpper().Contains(Constant.PrioHub_LINK.ToUpper()))
                            {
                                returnIsangoCodeFormat = Constant.IsangoLINK;
                            }
                            break;
                        }

                    case APIType.TourCMS:
                        if (apiCodeFormat.ToLower().Contains(Constant.TourCMS_BAR.ToLower()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoBARCODE;
                        }
                        else if (apiCodeFormat.ToUpper().Contains(Constant.String.ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoQRCODE;
                        }
                        break;

                    case APIType.Rayna:
                        if (apiCodeFormat.ToLower().Contains(Constant.API_Link.ToLower()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoLINK;
                        }
                        else if (apiCodeFormat.ToUpper().Contains(Constant.String.ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoQRCODE;
                        }
                        break;
                    case APIType.GoCity:
                    case APIType.Tiqets:
                        if (apiCodeFormat.ToLower().Contains(Constant.API_Link.ToLower()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoLINK;
                        }
                        break;

                    case APIType.NewCitySightSeeing:
                    case APIType.Citysightseeing:
                    case APIType.Bokun:
                    case APIType.Redeam:
                    case APIType.RedeamV12:
                    case APIType.Rezdy:
                    case APIType.BigBus:
                        {
                            if (apiCodeFormat.ToUpper().Contains(Constant.String.ToUpper()))
                            {
                                returnIsangoCodeFormat = Constant.IsangoQRCODE;
                            }
                            break;
                        }

                    case APIType.Ventrata:
                        if (apiCodeFormat.ToUpper().Contains(ConstantForVentrata.StringTypeForQRCode.ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoQRCODE;
                        }
                        else if (apiCodeFormat.ToUpper().Contains(ConstantForVentrata.LinkTypeForPDF.ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoLINK;
                        }
                        break;

                    case APIType.Goldentours:
                        if (apiCodeFormat.ToUpper().Contains(Constant.String.ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoQRCODE;
                        }
                        else if (apiCodeFormat.ToUpper().Contains(Constant.API_Link.ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoLINK;
                        }
                        break;


                    case APIType.Hotelbeds:
                        if (apiCodeFormat.ToUpper().Contains(Constant.API_Link.ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoLINK;
                        }
                        else if (apiCodeFormat.ToUpper().Contains(Constant.String.ToUpper()))
                        {
                            returnIsangoCodeFormat = Constant.IsangoQRCODE;
                        }
                        break;

                    default:
                        returnIsangoCodeFormat = apiCodeFormat;
                        break;

                }
            }
            return returnIsangoCodeFormat;
        }


    }
}