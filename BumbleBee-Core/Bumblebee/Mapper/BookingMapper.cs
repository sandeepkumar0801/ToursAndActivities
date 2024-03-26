using DiscountRuleEngine.Contracts;
using DiscountRuleEngine.Model;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Aot;
using Isango.Entities.BigBus;
using Isango.Entities.Bokun;
using Isango.Entities.Booking;
using Isango.Entities.Booking.RequestModels;
using Isango.Entities.CitySightseeing;
using Isango.Entities.Enums;
using Isango.Entities.FareHarbor;
using Isango.Entities.GoCity;
using Isango.Entities.GoldenTours;
using Isango.Entities.GrayLineIceLand;
using Isango.Entities.HotelBeds;
using Isango.Entities.MoulinRouge;
using Isango.Entities.MyIsango;
using Isango.Entities.NewCitySightSeeing;
using Isango.Entities.Payment;
using Isango.Entities.Prio;
using Isango.Entities.Rayna;
using Isango.Entities.PrioHub;
using Isango.Entities.Redeam;
using Isango.Entities.Rezdy;
using Isango.Entities.Tiqets;
using Isango.Entities.TourCMS;
using Isango.Entities.Ventrata;
using Isango.Mailer.ServiceContracts;
using Isango.Service.Contract;
using Logger.Contract;
using Microsoft.WindowsAzure.Storage.Table;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using TableStorageOperations.Contracts;
using TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities;
using Util;
using WebAPI.Models.ResponseModels;
using BokunQuestion = Isango.Entities.Bokun.Question;
using BookingPassengerInfo = Isango.Entities.Booking.PassengerInfo;
using ClientDetail = Isango.Entities.ClientDetail;
using Constant = WebAPI.Constants.Constant;
using PassengerDetail = Isango.Entities.Booking.RequestModels.PassengerDetail;
using ProductDetail = Isango.Entities.Booking.ProductDetail;
using RequestSelectedProduct = Isango.Entities.Booking.RequestModels.SelectedProduct;
using SelectedProduct = Isango.Entities.SelectedProduct;
using System.Web;
using ServiceAdapters.Ventrata.Ventrata.Entities.Response;
using Isango.Entities.Canocalization;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using ILogger = Logger.Contract.ILogger;

namespace WebAPI.Mapper
{
    /// <summary>
    /// Map Booking Core Entities to Booking API Entities
    /// </summary>
    public class BookingMapper
    {
        private readonly ITableStorageOperation _tableStorageOperation;
        private readonly IActivityService _activityService;
        private readonly IMasterService _masterService;
        private readonly IProfileService _profileService;
        private readonly IAffiliateService _affiliateService;
        private readonly IDiscountEngine _discountEngine;
        private readonly IBookingService _bookingService;
        private readonly bool _isAvailabilityExpiryCheck;
        private readonly IMailAttachmentService _mailAttachmentService;
        private readonly ILogger _log;

        public BookingMapper()
        {
        }

        /// <summary>
        /// BookingMapper Constructor
        /// </summary>
        /// <param name="tableStorageOperation"></param>
        /// <param name="activityService"></param>
        /// <param name="masterService"></param>
        /// <param name="affiliateService"></param>
        /// <param name="discountEngine"></param>
        /// <param name="bookingService"></param>
        /// <param name="profileService"></param>
        /// <param name="mailAttachmentService"></param>
        /// <param name="log"></param>
        public BookingMapper(ITableStorageOperation tableStorageOperation, IActivityService activityService, IMasterService masterService,
            IProfileService profileService, IAffiliateService affiliateService,
            IDiscountEngine discountEngine, IBookingService bookingService,
            IMailAttachmentService mailAttachmentService, ILogger log = null)
        {
            _tableStorageOperation = tableStorageOperation;
            _activityService = activityService;
            _masterService = masterService;
            _affiliateService = affiliateService;
            _discountEngine = discountEngine;
            _bookingService = bookingService;
            _mailAttachmentService = mailAttachmentService;
            _profileService = profileService;
            _log = log;
            try
            {
                _isAvailabilityExpiryCheck = ConfigurationManagerHelper.GetValuefromAppSettings("IsAvailabilityExpiryCheck") == "1";
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingMapper",
                    MethodName = "BookingMapper",
                };
                _log.Error(isangoErrorEntity, ex);
                _isAvailabilityExpiryCheck = false;
            }
        }

        /// <summary>
        /// This operation is used to map service response to User email preference response model
        /// </summary>
        /// <param name="userEmailPreferencesList"></param>
        /// <returns></returns>
        public List<UserEmailPreferences> MapUserEmailPreferencesResponse(
            List<MyUserEmailPreferences> userEmailPreferencesList)
        {
            return userEmailPreferencesList?.Select(userpreference => new UserEmailPreferences
            {
                QuestionOrder = userpreference.QuestionOrder,
                QuestionText = userpreference.QuestionText,
                MyUserAnswers = userpreference.MyUserAnswers,
                UserPreferredAnswer = userpreference.UserPreferredAnswer
            }).ToList();
        }

        /// <summary>
        /// This operation is used to map service response to User email preference response model
        /// </summary>
        /// <param name="myBookingSummaries"></param>
        /// <returns></returns>
        public List<BookingSummaryResponse> MapUserBookingDetailsResponse(List<MyBookingSummary> myBookingSummaries)
        {
            return myBookingSummaries?.Select(bookingSummaries => new BookingSummaryResponse
            {
                BookedProducts = bookingSummaries.BookedProducts,
                BookingAmountCurrency = bookingSummaries.BookingAmountCurrency,
                BookingDate = bookingSummaries.BookingDate,
                BookingId = bookingSummaries.BookingId,
                BookingRefenceNumber = bookingSummaries.BookingRefenceNumber,
                PickUpTravelDate = bookingSummaries.GetBookingDate,
            }).ToList();
        }

        /// <summary>
        /// This method prepare the request model for DRE from the API request and availabilities storage
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private DiscountModel MapDiscountRequest(CreateBookingRequest request)
        {
            var totalCartPrice = 0M;
            var selectedProducts = new List<DiscountSelectedProduct>();

            var count = 0;
            var parentBundleId = 0;
            foreach (var selectedProduct in request.SelectedProducts)
            {
                var referenceId = selectedProduct.AvailabilityReferenceId;
                var availabilitiesData = _tableStorageOperation.RetrieveData<BaseAvailabilitiesEntity>(referenceId, request.TokenId);
                if (availabilitiesData == null) return null;

                var activity = _activityService
                    .GetActivityById(availabilitiesData.ActivityId, DateTime.Today, request.LanguageCode)?.GetAwaiter().GetResult();
                if (activity == null) return null;

                count = GetSequenceNumber(availabilitiesData.BundleOptionID, parentBundleId, count);
                parentBundleId = availabilitiesData.BundleOptionID;
                var xFactor = _masterService
                    .GetConversionFactorAsync(availabilitiesData.CurrencyCode, request.CurrencyIsoCode).GetAwaiter()
                    .GetResult();
                var product = new DiscountSelectedProduct
                {
                    Id = availabilitiesData.ActivityId,
                    AvailabilityReferenceId = referenceId,
                    SellPrice = availabilitiesData.BasePrice * xFactor,
                    IsSaleProduct = availabilitiesData.OnSale,
                    Margin = availabilitiesData.Margin,
                    CategoryIds = activity.CategoryIDs,
                    DestinationIds = activity.Regions?.Select(x => x.Id).ToList(),
                    ComponentOrderNumber = availabilitiesData.ComponentOrder,
                    ParentBundleId = availabilitiesData.BundleOptionID,
                    SequenceNumber = count,
                    CurrencyIsoCode = request.CurrencyIsoCode,
                    LineOfBusiness = activity.LineOfBusinessId,
                    GatePrice = Convert.ToDecimal(availabilitiesData.GateBasePrice) * xFactor
                };
                if (availabilitiesData.GateBasePrice > availabilitiesData.BasePrice)
                {
                    product.IsSaleProduct = product.GatePrice > product.SellPrice;
                }

                totalCartPrice += product.SellPrice;
                selectedProducts.Add(product);
            }

            // Preparing vouchers from the API request
            var vouchers = new List<VoucherInfo>();
            if (request.DiscountCoupons != null)
            {
                foreach (var discountCoupon in request.DiscountCoupons)
                {
                    var voucher = new VoucherInfo
                    {
                        VoucherCode = discountCoupon
                    };
                    vouchers.Add(voucher);
                }
            }

            //Preparing DiscountModel
            var discountModel = new DiscountModel
            {
                AffiliateId = request.AffiliateId,
                UTMParameter = request.UTMParameter,
                CustomerEmail = request.UserEmail,
                Cart = new DiscountCart
                {
                    TotalPrice = totalCartPrice,
                    SelectedProducts = selectedProducts,
                    CurrencyIsoCode = request.CurrencyIsoCode
                },
                Vouchers = vouchers,
            };

            return discountModel;
        }

        /// <summary>
        /// This method prepare the booking object used by the booking service methods
        /// </summary>
        /// <param name="request"></param>
        /// <param name="bookingReferenceNumber"></param>
        /// <returns></returns>
        public Booking PrepareBookingModel(CreateBookingRequest request, string bookingReferenceNumber = null)
        {
            try
            {
                var clientDetails = new ClientDetail();
                var baseAvailabilitiesEntity = default(BaseAvailabilitiesEntity);
                baseAvailabilitiesEntity = _tableStorageOperation.RetrieveData<BaseAvailabilitiesEntity>(request.SelectedProducts.FirstOrDefault().AvailabilityReferenceId, request.TokenId);

                if (string.IsNullOrEmpty(request.LanguageCode))
                {
                    request.LanguageCode = baseAvailabilitiesEntity?.LanguageCode ?? "en";
                }
                if (baseAvailabilitiesEntity?.ApiType == 9)
                {
                    if (string.IsNullOrEmpty(request.LanguageCode))
                    {
                        request.TiquetsLanguageCode = baseAvailabilitiesEntity?.LanguageCode ?? "en";
                    }
                    else
                    {
                        request.TiquetsLanguageCode = request?.TiquetsLanguageCode ?? request?.LanguageCode ?? "en";
                    }
                }
                if (String.IsNullOrEmpty(request.CurrencyIsoCode))
                {
                    request.CurrencyIsoCode = baseAvailabilitiesEntity?.CurrencyCode ?? "GBP";
                }
                if (request.ClientDetail != null)
                {
                    clientDetails.UserAgent = request.ClientDetail.UserAgent;
                    clientDetails.AcceptLanguage = request.ClientDetail.AcceptLanguage;
                    clientDetails.IsMobileDevice = request.ClientDetail.IsMobileDevice;
                }
                var userCreationDate = _profileService.GetUserCreationDate(request.UserEmail).GetAwaiter().GetResult();

                var user = new ISangoUser
                {
                    EmailAddress = request.UserEmail,
                    FirstName = request.SelectedProducts[0].PassengerDetails.FirstOrDefault(x => x.IsLeadPassenger)
                        ?.FirstName,
                    LastName = request.SelectedProducts[0].PassengerDetails.FirstOrDefault(x => x.IsLeadPassenger)
                        ?.LastName,
                    PhoneNumber = request.UserPhoneNumber,
                    Address1 = request.CustomerAddress?.Address,
                    State = request.CustomerAddress?.StateOrProvince,
                    City = request.CustomerAddress?.Town,
                    Country = request.CustomerAddress?.CountryName,
                    CountryCode = request.CustomerAddress?.CountryIsoCode?.ToUpperInvariant(),
                    ZipCode = request.CustomerAddress?.PostCode,
                    IsGuestLogin = request.IsGuestUser,
                    UserCreationDate = userCreationDate,
                    UserLoginSource = request.UserLoginSource
                };

                var affiliate = _affiliateService.GetAffiliateInformationAsync(request.AffiliateId).GetAwaiter()
                    .GetResult();
                if (affiliate == null)
                {
                    var message = Constant.NoAffiliateData;
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "BookingMapper",
                        MethodName = "PrepareBookingModel",
                        Params = $"{SerializeDeSerializeHelper.Serialize(request)}"
                    };
                    _log.Error(isangoErrorEntity, new Exception(message));
                    var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        ReasonPhrase = message
                    };
                    throw new HttpResponseException(data);
                }

                var discountRequest = MapDiscountRequest(request);
                DiscountCart discountCart = null;
                if (discountRequest != null)
                {
                    discountCart = _discountEngine.Process(discountRequest);
                }

                var totalMultiSaveDiscountedPrice = 0M;
                var totalDiscountedPrice = 0M;
                var selectedProducts = new List<SelectedProduct>();
                var multisaveSelectedProducts = new List<DiscountSelectedProduct>();
                foreach (var requestProduct in request.SelectedProducts)
                {
                    DiscountSelectedProduct discountSelectedProduct = null;

                    if (discountCart?.SelectedProducts != null)
                    {
                        discountSelectedProduct = discountCart.SelectedProducts.First(x =>
                            x.AvailabilityReferenceId.Equals(requestProduct.AvailabilityReferenceId));
                        multisaveSelectedProducts = discountCart.SelectedProducts.Where(x => x.IsMultiSaveApplied).ToList();
                    }

                    var result = _tableStorageOperation.RetrieveData<BaseAvailabilitiesEntity>(requestProduct.AvailabilityReferenceId, request.TokenId);
                    if (result == null)
                    {
                        var message = Constant.DataReferenceId + requestProduct.AvailabilityReferenceId + "Token" + request.TokenId + Constant.IsNotAvailable;
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "BookingMapper",
                            MethodName = "PrepareBookingModel",
                            Params = $"{SerializeDeSerializeHelper.Serialize(request)}"
                        };
                        _log.Error(isangoErrorEntity, new Exception(message));
                        var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            ReasonPhrase = message
                        };
                        throw new HttpResponseException(data);
                    }

                    var availabilitiesData = _tableStorageOperation.Retrieve(requestProduct.AvailabilityReferenceId,
                        request.TokenId, result.ApiType);

                    if (affiliate.AvailabilityReferenceExpiry > 0 && _isAvailabilityExpiryCheck)
                    {
                        var availabilityExpiresOn = availabilitiesData.Timestamp.DateTime.AddMinutes(affiliate.AvailabilityReferenceExpiry);

                        if (DateTime.UtcNow > availabilityExpiresOn)
                        {
                            var message = Constant.DataReferenceId + requestProduct.AvailabilityReferenceId + Constant.HasExpired;
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "BookingMapper",
                                MethodName = "PrepareBookingModel",
                                Params = $"{SerializeDeSerializeHelper.Serialize(request)}"
                            };
                            var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                            {
                                ReasonPhrase = message
                            };
                            throw new HttpResponseException(data);
                        }
                    }

                    var activity = _activityService
                        .GetActivityById(result.ActivityId, DateTime.Today, request.LanguageCode)?.GetAwaiter().GetResult();
                    if (activity == null)
                    {
                        var message = Constant.ActivityNoData;
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "BookingMapper",
                            MethodName = "PrepareBookingModel",
                            Params = $"{SerializeDeSerializeHelper.Serialize(request)}"
                        };
                        var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            ReasonPhrase = message
                        };
                        throw new HttpResponseException(data);
                    }

                    totalDiscountedPrice += discountSelectedProduct?.ProductDiscountedPrice ?? 0;
                    var selectedProduct = CreateSelectedProduct(discountSelectedProduct, availabilitiesData, request, (APIType)result.ApiType);
                    if (selectedProduct.APIType == APIType.TourCMS)
                    {
                        var data = activity.ProductOptions.Where(x => x.SupplierOptionCode == selectedProduct?.ProductOptions?.FirstOrDefault()?.SupplierOptionCode)?.FirstOrDefault();
                        selectedProduct.ProductOptions.FirstOrDefault().PrefixServiceCode = data.PrefixServiceCode;
                    }
                    try
                    {
                        selectedProduct.IsShowSupplierVoucher = activity?.ISSHOWSUPPLIERVOUCHER ?? false;
                        if (selectedProduct.APIType == APIType.Moulinrouge)
                        {
                            selectedProduct.IsShowSupplierVoucher = true;
                        }
                        selectedProduct.AdyenStringentAccount = activity.AdyenStringentAccount;
                    }
                    catch (Exception e)
                    {
                        //ignore but log
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "BookingMapper",
                            MethodName = "PrepareBookingModel",
                        };
                        _log.Error(isangoErrorEntity, e);
                    }
                    selectedProducts.Add(selectedProduct);
                }
                foreach (var multisaveSelectedProduct in multisaveSelectedProducts)
                {
                    totalMultiSaveDiscountedPrice += multisaveSelectedProduct?.MultiSaveDiscountedPrice ?? 0;
                }
                var totalBookingPrice = selectedProducts.Sum(x => x.Price);
                var amount = Convert.ToDecimal((totalBookingPrice * 100).ToString("0")) / 100;
                var totalDiscount = totalDiscountedPrice;
                Enum.TryParse(request.PaymentDetail.PaymentGateway, true, out PaymentGatewayType gatewayType);
                Enum.TryParse(request.PaymentDetail.PaymentOption, true, out PaymentOptionType paymentOptionType);
                Enum.TryParse(request.PaymentDetail.PaymentMethodType, true, out PaymentMethodType paymentMethodType);

                if (paymentMethodType == PaymentMethodType.Undefined)
                    paymentMethodType = affiliate.PaymentMethodType;

                var mapIsangoBookingData = MapIsangoBookingData(request);

                if (string.IsNullOrWhiteSpace(bookingReferenceNumber))
                {
                    //bookingReferenceNumber =
                    //    _bookingService.GenerateBookingRefNumber(request.AffiliateId, request.CurrencyIsoCode);

                    bookingReferenceNumber = _bookingService.GetReferenceNumberfromDB(request.AffiliateId, request.CurrencyIsoCode);
                }
                var booking = new Booking();
                booking.ReferenceNumber = bookingReferenceNumber;
                booking.BookingAgent = request.BookingAgent;
                booking.VoucherEmailAddress = user.EmailAddress;
                booking.VoucherPhoneNumber = user.PhoneNumber;
                booking.Currency = new Currency { IsoCode = request.CurrencyIsoCode };
                booking.Date = DateTime.Today;
                booking.BookingTime = request.BookingTime;
                booking.Language = new Language { Code = request.LanguageCode };
                booking.Amount = amount;
                booking.MultisaveAmountOnBooking = totalMultiSaveDiscountedPrice;
                booking.TotalDiscount = totalDiscount;
                booking.isRiskifiedEnabled = affiliate.isRiskifiedEnable;
                booking.PaymentOption = paymentOptionType;
                booking.PaymentGateway = gatewayType;
                booking.PaymentMethodType = paymentMethodType;
                booking.BookingUserAgent = new BookingUserAgent();
                booking.ActualIP = request.ActualIP;
                booking.IpAddress = request.IPAddress;
                booking.OriginCountry = request.OriginCountry;
                booking.OriginCity = request.OriginCity;
                booking.PostalCode = request.CustomerAddress.PostCode;
                booking.Town = request.CustomerAddress.Town;
                booking.Country = request.CustomerAddress.CountryName;
                booking.Street = request.CustomerAddress.Address;
                booking.SessionId = request.SessionId;
                booking.User = user;
                booking.Affiliate = affiliate;
                booking.SelectedProducts = selectedProducts;
                booking.IsangoBookingData = mapIsangoBookingData;
                booking.IsReservation = request.IsReservation ?? false;
                booking.TiquetsLanguageCode = request.TiquetsLanguageCode ?? "en";

                booking.ClientDetail = new ClientDetail()
                {
                    IsMobileDevice = clientDetails.IsMobileDevice,
                    AcceptLanguage = clientDetails.AcceptLanguage,
                    UserAgent = clientDetails.UserAgent
                };

                booking.BrowserInfo = new Isango.Entities.Booking.BrowserInfo
                {
                    AcceptHeader = request.BrowserInfo?.AcceptHeader,
                    ScreenHeight = request.BrowserInfo?.ScreenHeight,
                    ScreenWidth = request.BrowserInfo?.ScreenWidth,
                    ColorDepth = request.BrowserInfo?.ColorDepth,
                    JavaEnabled = request.BrowserInfo?.JavaEnabled ?? false,
                    Language = request.BrowserInfo?.Language,
                    TimeZoneOffset = request.BrowserInfo?.TimeZoneOffset,
                    UserAgent = request.BrowserInfo?.UserAgent
                };

                if (booking.Amount > 0
                    && (booking.PaymentGateway == PaymentGatewayType.WireCard
                        || booking.PaymentGateway == PaymentGatewayType.Apexx || booking.PaymentGateway == PaymentGatewayType.Adyen)
                    && booking.PaymentMethodType == PaymentMethodType.Transaction
                )
                {
                    if (request.PaymentDetail.CardDetails != null)
                    {
                        booking.Payment = new List<Payment>
                        {
                            new Payment {PaymentType = FillCreditCard(user, request.PaymentDetail)}
                        };
                    }
                    else
                    {
                        return null;
                    }
                }

                booking.IsangoBookingData.PaymentMethodType = ((int)booking.PaymentMethodType).ToString();

                booking.AdyenMerchantAccount = GetBookingAdyenMerchant(booking);

                return booking;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingMapper",
                    MethodName = "PrepareBookingModel",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }


        /// <summary>
        /// Get Merchant Type
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        private string GetBookingAdyenMerchant(Booking booking)
        {
            var adyenReleaseDate = ConfigurationManagerHelper.GetValuefromAppSettings("AdyenReleaseDate");
            string adyenMerchantType = ConfigurationManagerHelper.GetValuefromAppSettings(ServiceAdapters.Adyen.Constants.Constant.AdyenMerchantAccountNew);
            try
            {
                var isOldAdyen = booking.Date < Convert.ToDateTime(adyenReleaseDate);
                //check product level or affiliate level(adyen stringent account):
                //case1: if AdyenStringentAccount=true and AdyenStringentAccountRestrictProducts
                //case2: if AffiliateLevelStringent=true and AdyenStringentAccount=true
                var affiliatLevelStringent = booking?.Affiliate;

                var checkTwoStep = false;
                if (booking?.SelectedProducts != null)
                {
                    var productoptions = booking?.SelectedProducts.All(x => x.IsReceipt == true);
                    if (productoptions == true)
                    {
                        checkTwoStep = true;
                    }
                }
                if (checkTwoStep)
                {
                    adyenMerchantType = isOldAdyen == true ? ConfigurationManagerHelper.GetValuefromAppSettings(ServiceAdapters.Adyen.Constants.Constant.AdyenMerchantAccount) : ConfigurationManagerHelper.GetValuefromAppSettings(ServiceAdapters.Adyen.Constants.Constant.AdyenMerchantAccountNew);
                }

                else if (
                    (affiliatLevelStringent?.AdyenStringentAccount == true
                    && affiliatLevelStringent?.AdyenStringentAccountRestrictProducts == false)
                   )
                {
                    adyenMerchantType = ConfigurationManagerHelper.GetValuefromAppSettings(ServiceAdapters.Adyen.Constants.Constant.AdyenMerchantAccountStringent);
                }
                else if ((affiliatLevelStringent?.AdyenStringentAccount == true
                    && booking.SelectedProducts.Any(x => x.AdyenStringentAccount == true)))
                {
                    adyenMerchantType = ConfigurationManagerHelper.GetValuefromAppSettings(ServiceAdapters.Adyen.Constants.Constant.AdyenMerchantAccountStringent);
                }
                else
                {
                    adyenMerchantType = isOldAdyen == true ? ConfigurationManagerHelper.GetValuefromAppSettings(ServiceAdapters.Adyen.Constants.Constant.AdyenMerchantAccount) : ConfigurationManagerHelper.GetValuefromAppSettings(ServiceAdapters.Adyen.Constants.Constant.AdyenMerchantAccountNew);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingMapper",
                    MethodName = "GetBookingAdyenMerchant"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return adyenMerchantType;
        }


        public Booking PrepareCancelReservationModel(List<ReservationDBDetails> reservationDBDetails, string bookingReferenceNumber = null)
        {
            try
            {
                var selectedProducts = new List<SelectedProduct>();
                foreach (var requestProduct in reservationDBDetails)
                {
                    var result = _tableStorageOperation.RetrieveData<BaseAvailabilitiesEntity>(requestProduct.AvailabilityRefNo, requestProduct.Token);
                    if (result == null)
                    {
                        var message = Constant.DataReferenceId + requestProduct + "Token" + requestProduct.Token + Constant.IsNotAvailable;
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "BookingMapper",
                            MethodName = "PrepareCancelReservationModel",
                            Params = $"{SerializeDeSerializeHelper.Serialize(reservationDBDetails)}"
                        };
                        _log.Error(isangoErrorEntity, new Exception(message));
                        var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            ReasonPhrase = message
                        };
                        throw new HttpResponseException(data);
                    }
                    var availabilitiesData = _tableStorageOperation.Retrieve(requestProduct.AvailabilityRefNo,
                        requestProduct.Token, result.ApiType);

                    var selectedProduct = CreateReservationSelectedProduct(requestProduct.AvailabilityRefNo, availabilitiesData,
                        (APIType)result.ApiType, bookingReferenceNumber);

                    selectedProducts.Add(selectedProduct);
                }

                var booking = new Booking();
                booking.ReferenceNumber = bookingReferenceNumber;
                booking.Date = DateTime.Today;
                booking.Language = new Language { Code = "en" };
                booking.BookingUserAgent = new BookingUserAgent();
                booking.SelectedProducts = selectedProducts;

                return booking;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingMapper",
                    MethodName = "PrepareCancelReservationModel",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }


        private SelectedProduct CreateReservationSelectedProduct(
            string availabilityRefNo, TableEntity availabilitiesData,
            APIType apiType, string bookingReferenceNumber)
        {
            var selectedProduct = new SelectedProduct();

            if (apiType == APIType.PrioHub)
            {
                var prioAvailability = (PrioHubAvailabilities)availabilitiesData;

                var rowKey = $"{bookingReferenceNumber}-{availabilityRefNo}";
                var ReservationDetails = _tableStorageOperation.RetrieveReservationData(rowKey);
                var createOrderResponse = SerializeDeSerializeHelper.DeSerialize<ServiceAdapters.PrioHub.PrioHub.Entities.ReservationResponse.ReservationResponse>(ReservationDetails?.ReservationResponse);

                selectedProduct = new PrioHubSelectedProduct()
                {
                    AvailabilityReferenceId = availabilityRefNo,
                    APIType = apiType,
                    PrioHubDistributerId = prioAvailability.PrioHubDistributerId,
                    PrioReservationReference = createOrderResponse.Data.Reservation.ReservationReference
                };
                return selectedProduct;
            }
            else if (apiType == APIType.TourCMS)
            {
                var prioAvailability = (TourCMSAvailabilities)availabilitiesData;

                var rowKey = $"{bookingReferenceNumber}-{availabilityRefNo}";
                var ReservationDetails = _tableStorageOperation.RetrieveReservationData(rowKey);
                var createOrderResponse = SerializeDeSerializeHelper.DeSerialize<ServiceAdapters.TourCMS.TourCMS.Entities.NewBooking.NewBookingResponse>(ReservationDetails?.ReservationResponse);
                var channelidAccountId = createOrderResponse.Booking.ChannelId + "_" + createOrderResponse.Booking.AccountId;
                selectedProduct = new TourCMSSelectedProduct()
                {
                    AvailabilityReferenceId = availabilityRefNo,
                    APIType = apiType,
                    BookingId = Convert.ToInt32(createOrderResponse.Booking.BookingId),
                    ShortReference = channelidAccountId
                };

                selectedProduct.ProductOptions = new List<ProductOption>();
                var Option = new ProductOption();
                Option.PrefixServiceCode = channelidAccountId;
                selectedProduct.ProductOptions.Add(Option);
                return selectedProduct;
            }
            else if (apiType == APIType.Ventrata)
            {

                var ventrataAvailability = (VentrataAvailabilities)availabilitiesData;
                var rowKey = $"{bookingReferenceNumber}-{availabilityRefNo}";
                var ReservationDetails = _tableStorageOperation.RetrieveReservationData(rowKey);
                var createOrderResponse = SerializeDeSerializeHelper.DeSerialize<BookingReservationRes>(ReservationDetails?.ReservationResponse);

                selectedProduct = new VentrataSelectedProduct()
                {
                    AvailabilityReferenceId = availabilityRefNo,
                    APIType = apiType,
                    Uuid = createOrderResponse.uuid,
                    ActivityCode = ((VentrataAvailabilities)availabilitiesData).VentrataSupplierId,
                    VentrataBaseURL = ((VentrataAvailabilities)availabilitiesData).VentrataBaseURL,
                    BookingStatus = "ON_HOLD",
                    IsCancellable = true,
                    ApiBookingDetails = new VentrataApiBookingDetails()
                };
                return selectedProduct;
            }
            else if (apiType == APIType.Tiqets)
            {
                selectedProduct = new SelectedProduct()
                {
                    AvailabilityReferenceId = availabilityRefNo,
                    APIType = apiType
                };
                return selectedProduct;
            }
            else if (apiType == APIType.Redeam)
            //Redeam Code will update and check with Redeam new version
            {
                var redeamAvailability = (RedeamAvailabilities)availabilitiesData;
                var rowKey = $"{bookingReferenceNumber}-{availabilityRefNo}";
                var ReservationDetails = _tableStorageOperation.RetrieveReservationData(rowKey);
                var createOrderResponse = SerializeDeSerializeHelper.DeSerialize<ServiceAdapters.Redeam.Redeam.Entities.CreateHold.CreateHoldResponse>(ReservationDetails?.ReservationResponse);

                selectedProduct = new RedeamSelectedProduct()
                {
                    AvailabilityReferenceId = availabilityRefNo,
                    APIType = apiType,
                    HoldId = Convert.ToString(createOrderResponse?.Hold?.Id),
                    HoldStatus = createOrderResponse?.Hold?.Status
                };
                return selectedProduct;
            }

            return null;
        }

        /// <summary>
        /// This method prepares the booking API response
        /// </summary>
        /// <param name="bookingResult"></param>
        /// <param name="bookingReferenceNumber"></param>
        /// <param name="paymentGateway"></param>
        /// <returns></returns>
        public BookingResponse PrepareBookingResponse(BookingResult bookingResult, string bookingReferenceNumber,
        string paymentGateway)
        {
            Enum.TryParse(paymentGateway, true, out PaymentGatewayType gatewayType);
            var bookingResponse = new BookingResponse
            {
                Message = bookingResult?.StatusMessage,
                Status = Convert.ToString(bookingResult?.BookingStatus),
                ReferenceId = string.IsNullOrEmpty(bookingResult?.BookingRefNo)
                    ? bookingReferenceNumber
                    : bookingResult?.BookingRefNo,
                GatewayDetail = new GatewayDetail
                {
                    Html = bookingResult?.RequestHtml,
                    Type = Convert.ToString(gatewayType)?.ToUpperInvariant(),
                    Url = bookingResult?.Url,
                    FallbackFingerPrint = bookingResult?.FallbackFingerPrint ?? string.Empty
                },
                IsWebhookReceived = bookingResult.IsWebhookReceived
            };
            return bookingResponse;
        }

        /// <summary>
        /// This method prepares the BookingDetailResponse
        /// </summary>
        /// <param name="confirmBookingDetail"></param>
        /// <returns></returns>
        public BookingDetailResponse PrepareBookingDetailResponse(ConfirmBookingDetail confirmBookingDetail)
        {
            var bookingDetailResponse = new BookingDetailResponse
            {
                AffiliateId = confirmBookingDetail.AffiliateId,
                Language = confirmBookingDetail.LanguageCode,
                CurrencyIsoCode = confirmBookingDetail.CurrencyIsoCode,
                BookingReferenceNumber = confirmBookingDetail.BookingReferenceNumber,
                CustomerEmail = confirmBookingDetail.VoucherEmail,
                BookingDate = confirmBookingDetail.BookingDate,
                ProductDetails = new List<ProductDetail>()
            };
            foreach (var bookedOption in confirmBookingDetail.BookedOptions)
            {
                var productDetail = new ProductDetail
                {
                    Status = bookedOption.ServiceStatus.Replace("from Allocation", "").Trim(),
                    TravelDate = bookedOption.TravelDate,
                    DiscountAmount = bookedOption.DiscountAmount,
                    MultiSaveAmount = bookedOption.MutliSaveDiscount,
                    SellAmount = bookedOption.SellAmount,
                    LeadTravellerName = bookedOption.LeadPaxName,
                    ProductName = bookedOption.ServiceName,
                    IsReceipt = bookedOption.IsReceipt,
                    LinkType = bookedOption.IsQRCodePerPax ? null : bookedOption.LinkType,
                    LinkValue = bookedOption.IsQRCodePerPax ? null : bookedOption.LinkValue.Replace("QR_CODE~", ""),
                    Passengers = new List<Passenger>(),
                    AvailabilityReferenceId = bookedOption.AvailabilityReferenceId,
                    IsShowSupplierVoucher = bookedOption.IsShowSupplierVoucher,
                    BookedOptionId = bookedOption.BookedOptionId,
                    BookedOptionName = bookedOption.BookedOptionName,
                    IsQRCodePerPax = bookedOption.IsQRCodePerPax,
                    ApiType = bookedOption.ApiType,
                    isangoVoucherLink = GetVoucherLink(confirmBookingDetail.BookingReferenceNumber, bookedOption.BookedOptionId)
                };

                if (bookedOption.IsQRCodePerPax)
                {
                    foreach (var passengerDetail in bookedOption.BookedPassengerDetails)
                    {
                        if (passengerDetail.QRCodeDetail?.Count > 0)
                        {
                            foreach (var qrCode in passengerDetail.QRCodeDetail)
                            {
                                var fileName = _mailAttachmentService.FilterIllegalCharacterFromPath(qrCode.BarCode);
                                var generatedQRCodeImageFile = $"{confirmBookingDetail.BookingReferenceNumber}_{qrCode.BookedOptionID}_{fileName}";
                                _mailAttachmentService.GenerateQrCode(qrCode.BarCode, generatedQRCodeImageFile);
                                var passenger = new Passenger
                                {
                                    AgeGroupDescription = passengerDetail.AgeGroupDescription,
                                    PaxCount = 1,
                                    LinkType = string.IsNullOrWhiteSpace(bookedOption?.LinkType) ? "QRCODE" : (bookedOption?.LinkType ?? string.Empty),
                                    //QRCodeValue = qrCode.BarCode,
                                    LinkValue = qrCode.BarCode,//$"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}/QRCodes/{generatedQRCodeImageFile}.png",
                                    PassengerTypeId = passengerDetail.PassengerTypeId
                                };
                                productDetail.Passengers.Add(passenger);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var passengerDetail in bookedOption.BookedPassengerDetails)
                    {
                        var passenger = new Passenger
                        {
                            AgeGroupDescription = passengerDetail.AgeGroupDescription,
                            PaxCount = passengerDetail.PaxCount,
                            LinkType = null,
                            LinkValue = null,
                            //QRCodeValue = null,
                            PassengerTypeId = passengerDetail.PassengerTypeId
                        };
                        productDetail.Passengers.Add(passenger);
                    }
                }
                bookingDetailResponse.ProductDetails.Add(productDetail);
            }

            return bookingDetailResponse;
        }
        public string GetVoucherLink(string bookingReferenceNumber, int? bookedoptionid)
        {
            // Inject IHttpContextAccessor in your controller or service to get the current HttpContext
            var httpContextAccessor = new HttpContextAccessor();
            var context = httpContextAccessor.HttpContext;

            if (context == null)
            {
                // You might want to handle the case where there is no HttpContext (e.g., in a background task).
                throw new InvalidOperationException("HttpContext is not available.");
            }

            var url = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}";
            var voucherlink = $"{url}/api/voucher/v1/book/{bookingReferenceNumber}/3/{bookedoptionid}";
            return voucherlink;
        }

        /// <summary>
        /// Prepare Booking Details Response By Booking Ref No. and UserId
        /// </summary>
        /// <param name="confirmBookingData"></param>
        /// <param name="bookingOptionData"></param>
        /// <returns></returns>
        public BookingOptionsDetailsResponse PrepareBookingDetailResponse(ConfirmBookingDetail confirmBookingData,
            List<BookingDetail> bookingOptionData)
        {
            var bookingOptionDetailList = new List<BookingOptionDetail>();

            if (bookingOptionData != null && confirmBookingData?.BookedOptions?.Count > 0)
            {
                foreach (var option in bookingOptionData)
                {
                    var optionAndServiceName = _bookingService
                        .GetOptionAndServiceNameAsync(confirmBookingData.BookingReferenceNumber, false,
                            option?.BookingDetailId.ToString()).GetAwaiter().GetResult();
                    var bookedOption = new BookingOptionDetail();
                    if (option != null)
                    {
                        bookedOption.OptionId = option.BookingDetailId;
                        bookedOption.OptionName = optionAndServiceName["OptionName"];
                        switch (option.TrakerStatusId)
                        {
                            case 1:
                                bookedOption.OptionStatus = "OnRequest";
                                break;

                            case 3:
                                bookedOption.OptionStatus = "Cancelled";
                                break;

                            case 2:
                            case 4:
                                bookedOption.OptionStatus = "Confirmed";
                                break;

                            default:
                                bookedOption.OptionStatus = "Failed";
                                break;
                        }
                        bookedOption.ServiceId = option.ServiceId;
                        bookedOption.ServiceName = optionAndServiceName["ServiceName"];
                        bookedOption.TravelDate = confirmBookingData.BookedOptions
                            .Where(x => x.BookedOptionId == option.BookingDetailId).Select(x => x.TravelDate)
                            .FirstOrDefault();
                        bookedOption.SellingPrice =
                            confirmBookingData.BookedOptions.Where(x => x.BookedOptionId == option.BookingDetailId)
                                .Select(x => x.SellAmount).FirstOrDefault() + " " + confirmBookingData.CurrencyIsoCode;
                    }

                    bookingOptionDetailList.Add(bookedOption);
                }
            }

            var bookingOptionDetails = new BookingOptionsDetailsResponse
            {
                BookingOptionsDetails = bookingOptionDetailList
            };
            return bookingOptionDetails;
        }

        /// <summary>
        /// This method prepares the ReceiveDetailResponse
        /// </summary>
        /// <param name="receiveDetail"></param>
        /// <returns></returns>
        public ReceiveDetailResponse PrepareReceiveDetailResponse(ReceiveDetail receiveDetail)
        {
            var receiveDetailResponse = new ReceiveDetailResponse
            {
                Status = "Success",
                AffiliateId = receiveDetail?.AffiliateId,
                AffiliateName = receiveDetail?.AffiliateName,
                BookedOptionId = receiveDetail.BookedOptionId,
                BookingReferenceNumber = receiveDetail?.BookingReferenceNumber,
                ChargeAmount = receiveDetail.ChargeAmount,
                CompanyWebsite = receiveDetail?.CompanyWebsite,
                CountryID = receiveDetail?.CountryID,
                CurrencySymbol = receiveDetail?.CurrencySymbol,
                FinancialBookingTransactionId = receiveDetail.FinancialBookingTransactionId,
                LanguageCode = receiveDetail?.LanguageCode,
                LeadPaxName = receiveDetail?.LeadPaxName,
                PassengerFirstName = receiveDetail?.PassengerFirstName,
                PassengerLastName = receiveDetail?.PassengerLastName,
                PhoneNumber = receiveDetail?.PhoneNumber,
                Remarks = receiveDetail?.Remarks,
                SellCurrency = receiveDetail?.SellCurrency,
                ServiceName = receiveDetail?.ServiceName,
                ServiceOptionName = receiveDetail?.ServiceOptionName,
                UserId = receiveDetail.UserId,
                VoucherEmail = receiveDetail?.VoucherEmail,
                PaymentGatwayType = receiveDetail?.PaymentGatwayType,
                AdyenMerchantAccout = receiveDetail?.AdyenMerchantAccout
            };

            return receiveDetailResponse;
        }

        #region Private Methods

        private CreditCard FillCreditCard(ISangoUser user, PaymentDetail paymentDetail)
        {
            var card = paymentDetail?.CardDetails;
            var cardHolderName = paymentDetail.UserFullName;
            var creditCard = new CreditCard
            {
                CardNumber = card.Number,
                CardType = card.Type,
                CardHoldersName = !string.IsNullOrEmpty(cardHolderName) ? cardHolderName : $"{user.FirstName} {user.LastName}",
                ExpiryMonth = card.ExpiryMonth,
                ExpiryYear = card.ExpiryYear,
                SecurityCode = card.SecurityCode,
                CardHoldersAddress1 = user.Address1,
                CardHoldersAddress2 = user.Address2,
                CardHoldersCity = user.City,
                CardHoldersState = string.Empty,
                CardHoldersCountryName = user.CountryCode?.ToUpperInvariant(),
                CardHoldersZipCode = user.ZipCode,
                BillingAddressCountry = user.Country,
                BillingAddressState = string.Empty,
                CardHoldersPhoneNumber = user.PhoneNumber
                // CardHoldersEmail = user.EmailAddress.Trim()
            };

            return creditCard;
        }

        private int GetSequenceNumber(int bundleOptionId, int parentBundleId, int count)
        {
            if (bundleOptionId > 0)
            {
                if (parentBundleId != bundleOptionId)
                {
                    count++;
                }
            }
            else
            {
                count++;
            }

            return count;
        }

        private SelectedProduct CreateSelectedProduct(DiscountSelectedProduct discountSelectedProduct,
            TableEntity availabilityData, CreateBookingRequest request, APIType apiType)
        {
            var defaultAvailability = (BaseAvailabilitiesEntity)availabilityData;
            var requestProduct = request.SelectedProducts.FirstOrDefault(x => x.AvailabilityReferenceId.Equals
                (availabilityData.RowKey));

            var selectedProduct = CreateDefaultSelectedProduct(discountSelectedProduct, defaultAvailability, request);

            switch (apiType)
            {
                case APIType.Citysightseeing:
                    {
                        var citysightseeingSelectedProduct = new CitySightseeingSelectedProduct();
                        citysightseeingSelectedProduct =
                            (CitySightseeingSelectedProduct)FillSpecializedSelectedProduct(selectedProduct,
                                citysightseeingSelectedProduct);
                        return citysightseeingSelectedProduct;
                    }
                case APIType.Graylineiceland:
                    {
                        var gliAvailability = (GraylineIcelandAvailabilities)availabilityData;
                        var gliSelectedProduct = new GrayLineIceLandSelectedProduct();
                        gliSelectedProduct =
                            (GrayLineIceLandSelectedProduct)FillSpecializedSelectedProduct(selectedProduct,
                                gliSelectedProduct);
                        gliSelectedProduct.Code = gliAvailability.TourNumber;
                        gliSelectedProduct.PaxAgeGroupIds = GetGLIPaxAgeGroupIdsAsync(defaultAvailability.ActivityId, apiType);
                        gliSelectedProduct.HotelPickUpLocation = selectedProduct.HotelPickUpLocation;
                        var gliOption =
                            gliSelectedProduct.ProductOptions.FirstOrDefault(x => x.Id == gliAvailability?.OptionId);
                        if (gliOption != null)
                        {
                            var gliPriceAndAvailability = gliOption?.SellPrice?.DatePriceAndAvailabilty?.FirstOrDefault().Value;
                            if (gliPriceAndAvailability == null)
                            {
                                gliPriceAndAvailability = new DefaultPriceAndAvailability();
                            }
                            gliPriceAndAvailability.IsSelected = true;
                            gliPriceAndAvailability.TourDepartureId = gliAvailability.TourDepartureId;
                            var prioDatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>
                            {
                                {gliOption.TravelInfo.StartDate, gliPriceAndAvailability}
                            };
                            gliOption.SellPrice.DatePriceAndAvailabilty = prioDatePriceAndAvailabilty;
                            gliOption.BasePrice.DatePriceAndAvailabilty = prioDatePriceAndAvailabilty;
                        }

                        return gliSelectedProduct;
                    }
                case APIType.Hotelbeds:
                    {
                        var hbAvailability = (TicketsAvailabilities)availabilityData;
                        var hbSelectedProduct = new HotelBedsSelectedProduct();
                        hbSelectedProduct =
                            (HotelBedsSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, hbSelectedProduct);

                        hbSelectedProduct.ProductOptions.ForEach(x =>
                        {
                            var ao = ((ActivityOption)x);
                            ao.RateKey = hbAvailability.RateKey;
                            ao.AvailToken = hbAvailability.RowKey;
                            ao.Code = hbAvailability.ModalityCode;

                            //((ActivityOption)x).AvailToken = hbAvailability.AvailToken;
                            ao.Contract =
                                    SerializeDeSerializeHelper.DeSerialize<Isango.Entities.Contract>(hbAvailability
                                        .ContractDetails);
                        });

                        hbSelectedProduct.Code = hbAvailability.TicketCode;

                        hbSelectedProduct.Destination = hbAvailability.Destination;
                        hbSelectedProduct.ContractQuestions = PrepareContractQuestions(requestProduct);

                        var availabilityDataCQ = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ContractQuestion>>(hbAvailability?.ContractQuestions);

                        if (hbSelectedProduct.ContractQuestions?.Count == 0 && availabilityDataCQ?.Count > 0)
                        {
                            hbSelectedProduct.ContractQuestions = availabilityDataCQ;
                        }

                        return hbSelectedProduct;
                    }
                case APIType.Moulinrouge:
                    {
                        var mrAvailability = (MoulinRougeAvailabilities)availabilityData;
                        var mrSelectedProduct = new MoulinRougeSelectedProduct();
                        mrSelectedProduct =
                            (MoulinRougeSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, mrSelectedProduct);

                        mrSelectedProduct.CatalogDateId =
                            mrAvailability.CatalogDateId ?? 0;
                        mrSelectedProduct.RateId = mrAvailability.RateId ?? 0;
                        mrSelectedProduct.CategoryId = mrAvailability.CategoryId ?? 0;
                        mrSelectedProduct.BlocId = mrAvailability.BlocId ?? 0;
                        mrSelectedProduct.FloorId = mrAvailability.FloorId ?? 0;
                        mrSelectedProduct.ContingentId = mrAvailability.ContingentId ?? 0;

                        var mrOption =
                            mrSelectedProduct.ProductOptions.FirstOrDefault(x => x.Id == mrAvailability?.OptionId);
                        if (mrOption != null)
                        {
                            var defaultPriceAndAvailability = mrOption?.SellPrice?.DatePriceAndAvailabilty?.FirstOrDefault().Value;
                            var mrPriceAndAvailability = new MoulinRougePriceAndAvailability
                            {
                                IsSelected = true,
                                MoulinRouge = new APIContextMoulinRouge
                                {
                                    CatalogDateId = mrAvailability.CatalogDateId ?? 0,
                                    RateId = mrAvailability.RateId ?? 0,
                                    CategoryId = mrAvailability.CategoryId ?? 0,
                                    BlocId = mrAvailability.BlocId ?? 0,
                                    FloorId = mrAvailability.FloorId ?? 0,
                                    ContingentId = mrAvailability.ContingentId ?? 0
                                },
                                CatalogDateID = mrAvailability.CatalogDateId ?? 0,
                                PricingUnits = defaultPriceAndAvailability?.PricingUnits,
                                TotalPrice = defaultPriceAndAvailability?.TotalPrice ?? 0,
                                AvailabilityStatus = defaultPriceAndAvailability.AvailabilityStatus,
                                ReferenceId = defaultPriceAndAvailability.ReferenceId,
                                Capacity = defaultPriceAndAvailability.Capacity
                            };
                            var mrDatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>
                            {
                                {mrOption.TravelInfo.StartDate, mrPriceAndAvailability}
                            };
                            mrOption.SellPrice.DatePriceAndAvailabilty = mrDatePriceAndAvailabilty;
                            mrOption.BasePrice.DatePriceAndAvailabilty = mrDatePriceAndAvailabilty;
                        }

                        return mrSelectedProduct;
                    }
                case APIType.Prio:
                    {
                        var prioAvailability = (PrioAvailabilities)availabilityData;
                        var prioSelectedProduct = new PrioSelectedProduct();
                        prioSelectedProduct =
                            (PrioSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, prioSelectedProduct);

                        prioSelectedProduct.PickupPoints = prioAvailability.PickupPoints;
                        prioSelectedProduct.PrioTicketClass = prioAvailability.PrioTicketClass;

                        var prioOption =
                            prioSelectedProduct.ProductOptions.FirstOrDefault(x => x.Id == prioAvailability?.ServiceOptionId);
                        var defaultPriceAndAvailability = prioOption?.SellPrice?.DatePriceAndAvailabilty?.FirstOrDefault().Value;
                        var prioPriceAndAvailability = new PrioPriceAndAvailability
                        {
                            FromDateTime = prioAvailability.FromDate,
                            ToDateTime = prioAvailability.ToDate,
                            Vacancies = prioAvailability.Vacancies,
                            IsSelected = true,
                            PricingUnits = defaultPriceAndAvailability?.PricingUnits,
                            TotalPrice = defaultPriceAndAvailability?.TotalPrice ?? 0,
                            AvailabilityStatus = defaultPriceAndAvailability.AvailabilityStatus,
                            ReferenceId = defaultPriceAndAvailability.ReferenceId,
                            Capacity = defaultPriceAndAvailability.Capacity
                        };
                        var datePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        {prioOption.TravelInfo.StartDate, prioPriceAndAvailability}
                    };
                        prioOption.SellPrice.DatePriceAndAvailabilty = datePriceAndAvailabilty;
                        prioOption.BasePrice.DatePriceAndAvailabilty = datePriceAndAvailabilty;
                        return prioSelectedProduct;
                    }
                case APIType.Fareharbor:
                    {
                        var fhbAvailability = (FareharborAvailabilities)availabilityData;
                        var fhbSelectedProduct = new FareHarborSelectedProduct();
                        fhbSelectedProduct =
                            (FareHarborSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, fhbSelectedProduct);

                        var fhbOption = (ActivityOption)fhbSelectedProduct.ProductOptions.FirstOrDefault(x => x.Id == fhbAvailability?.ServiceOptionId);
                        var defaultPriceAndAvailability = fhbOption?.SellPrice?.DatePriceAndAvailabilty?.FirstOrDefault().Value;
                        if (fhbOption != null)
                        {
                            var fareHarborPriceAndAvailability = new FareHarborPriceAndAvailability
                            {
                                IsSelected = true,
                                CustomerTypePriceIds =
                                    SerializeDeSerializeHelper.DeSerialize<Dictionary<PassengerType, Int64>>
                                        (fhbAvailability.CustomerTypePriceIds),
                                PricingUnits = defaultPriceAndAvailability?.PricingUnits,
                                TotalPrice = defaultPriceAndAvailability?.TotalPrice ?? 0,
                                AvailabilityStatus = defaultPriceAndAvailability.AvailabilityStatus,
                                ReferenceId = defaultPriceAndAvailability.ReferenceId,
                                Capacity = defaultPriceAndAvailability.Capacity
                            };
                            var fhbDatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>
                            {
                                {fhbOption.TravelInfo.StartDate, fareHarborPriceAndAvailability}
                            };
                            fhbOption.SellPrice.DatePriceAndAvailabilty = fhbDatePriceAndAvailabilty;
                            fhbOption.BasePrice.DatePriceAndAvailabilty = fhbDatePriceAndAvailabilty;
                            fhbOption.UserKey = fhbAvailability.UserKey;
                            fhbOption.AvailToken = fhbAvailability.AvailToken;
                            fhbOption.SupplierName = fhbAvailability.SupplierName;
                        }

                        return fhbSelectedProduct;
                    }
                case APIType.Bokun:
                    {
                        var bokunAvailability = (BokunAvailabilities)availabilityData;
                        var bokunSelectedProduct = new BokunSelectedProduct();
                        bokunSelectedProduct =
                            (BokunSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, bokunSelectedProduct);

                        bokunSelectedProduct.PricingCategoryIds =
                            SerializeDeSerializeHelper.DeSerialize<List<int>>(bokunAvailability.PricingCategoryId);
                        bokunSelectedProduct.StartTimeId = bokunAvailability.StartTimeId;

                        int.TryParse(bokunAvailability.RateId, out var tempInt);
                        bokunSelectedProduct.RateId = tempInt;
                        bokunSelectedProduct.Questions = PrepareBokunQuestions(requestProduct, request.UserEmail);

                        return bokunSelectedProduct;
                    }
                case APIType.Aot:
                    {
                        var aotAvailability = (AotAvailabilities)availabilityData;
                        var aotSelectedProduct = new AotSelectedProduct();
                        aotSelectedProduct =
                            (AotSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, aotSelectedProduct);

                        aotSelectedProduct.RoomType = aotAvailability.RoomType;
                        aotSelectedProduct.AotOptionType = aotAvailability.OptionType;
                        aotSelectedProduct.OptCode = aotAvailability.SupplierOptionCode;
                        aotSelectedProduct.SupplierCancellationPolicy = aotAvailability.SupplierCancellationPolicy;

                        return aotSelectedProduct;
                    }

                case APIType.Tiqets:
                    {
                        var tiqetsAvailability = (TiqetsAvailabilities)availabilityData;
                        var tiqetsSelectedProduct = new TiqetsSelectedProduct();
                        tiqetsSelectedProduct =
                            (TiqetsSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, tiqetsSelectedProduct);

                        tiqetsSelectedProduct.FactSheetId = tiqetsAvailability.FactSheetId;
                        tiqetsSelectedProduct.TimeSlot = tiqetsAvailability.TimeSlot?.Trim();
                        var requireData = SerializeDeSerializeHelper.DeSerialize<List<string>>(tiqetsAvailability.RequiresVisitorsDetails);
                        tiqetsSelectedProduct.RequiresVisitorsDetails = requireData;
                        //var requireDataVariant = SerializeDeSerializeHelper.DeSerialize<List<ProductVariantIdName>>(tiqetsAvailability.RequiresVisitorsDetailsWithVariant);
                        //tiqetsSelectedProduct.RequiresVisitorsDetailsWithVariant = requireDataVariant;
                        tiqetsSelectedProduct.ContractQuestions = PrepareContractQuestions(requestProduct);
                        return tiqetsSelectedProduct;
                    }
                case APIType.Goldentours:
                    {
                        var passengerMappings =
                            _masterService.GetPassengerMapping(APIType.Goldentours).GetAwaiter().GetResult();
                        var goldenToursAvailability = (GoldenToursAvailabilities)availabilityData;

                        #region Validate Pickup location against pickup options in storage table

                        var ra = request.SelectedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == goldenToursAvailability.RowKey);

                        var po = selectedProduct.ProductOptions.FirstOrDefault(
                                        x => x.ServiceOptionId == goldenToursAvailability.ServiceOptionId
                                        && x.SupplierOptionCode == goldenToursAvailability.SupplierOptionCode
                                    ) as ActivityOption;

                        if (!string.IsNullOrWhiteSpace(goldenToursAvailability?.PickupLocations)
                            && !string.IsNullOrWhiteSpace(selectedProduct.HotelPickUpLocation)
                        )
                        {
                            var pickiploactions = SerializeDeSerializeHelper.DeSerialize<Dictionary<Int32, string>>(goldenToursAvailability?.PickupLocations);
                            var selectedPickupLocationId = selectedProduct.HotelPickUpLocation?.Split('-')?.FirstOrDefault()?.ToInt() ?? 0;
                            if (pickiploactions?.ContainsKey(selectedPickupLocationId) != true)
                            {
                                selectedProduct.HotelPickUpLocation = null;

                                if (ra != null)
                                {
                                    ra.PickupLocation = null;
                                }
                                if (po != null)
                                {
                                    po.HotelPickUpLocation = null;
                                }
                            }
                        }
                        else if (string.IsNullOrWhiteSpace(goldenToursAvailability?.PickupLocations) && !string.IsNullOrWhiteSpace(selectedProduct.HotelPickUpLocation))
                        {
                            selectedProduct.HotelPickUpLocation = null;
                            if (ra != null)
                            {
                                ra.PickupLocation = null;
                            }
                            if (po != null)
                            {
                                po.HotelPickUpLocation = null;
                            }
                        }

                        #endregion Validate Pickup location against pickup options in storage table

                        var goldenToursSelectedProduct = new GoldenToursSelectedProduct();
                        goldenToursSelectedProduct =
                            (GoldenToursSelectedProduct)FillSpecializedSelectedProduct(selectedProduct,
                                goldenToursSelectedProduct);

                        var goldenToursOption = (ActivityOption)goldenToursSelectedProduct.ProductOptions.FirstOrDefault(x => x.Id == goldenToursAvailability?.ServiceOptionId)
                            ??
                            (ActivityOption)goldenToursSelectedProduct.ProductOptions.FirstOrDefault(x => x.ServiceOptionId == goldenToursAvailability?.ServiceOptionId)
                            ;
                        if (goldenToursOption != null)
                        {
                            goldenToursOption.ScheduleId = goldenToursAvailability.ScheduleId;
                            goldenToursOption.ProductType = goldenToursAvailability.ProductType;
                            goldenToursOption.RefNo = goldenToursAvailability.RefNo;
                        }

                        goldenToursSelectedProduct.ContractQuestions = PrepareContractQuestions(requestProduct);

                        // Deserializing it in the PerPersonPricingUnits as GT only supports PerPerson units, need to change this if the PerUnit comes
                        var pricingUnits =
                            SerializeDeSerializeHelper.DeSerialize<List<PerPersonPricingUnit>>(goldenToursAvailability
                                .BasePricingUnits);

                        // Getting Isango supported pricing units from the list of pricing units
                        var paxSupportedByIsangoOnly = pricingUnits.Where(e => e.SupportedByIsangoOnly)
                            .Select(e => e.PassengerType).ToList();

                        var numberOfPassengers = goldenToursOption?.TravelInfo?.NoOfPassengers;
                        if (numberOfPassengers != null)
                        {
                            var paxInfo = new Dictionary<int, int>();
                            foreach (var passengerInfo in numberOfPassengers)
                            {
                                // If PassengerType is only Isango supported then not mapping it in the paxInfo, so that it can not be passed in the supplier booking request
                                if (paxSupportedByIsangoOnly.Contains(passengerInfo.Key)) continue;

                                var unitId = passengerMappings
                                                 .FirstOrDefault(x => x.PassengerTypeId == (int)passengerInfo.Key)
                                                 ?.SupplierPassengerTypeId ?? 0;
                                if (!paxInfo.Keys.Contains(unitId) || unitId == 0)
                                    paxInfo.Add(unitId, passengerInfo.Value);
                            }

                            goldenToursSelectedProduct.PaxInfo = paxInfo;
                        }

                        return goldenToursSelectedProduct;
                    }
                case APIType.BigBus:
                    {
                        var bigBusSelectedProduct = new BigBusSelectedProduct();
                        bigBusSelectedProduct =
                            (BigBusSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, bigBusSelectedProduct);
                        return bigBusSelectedProduct;
                    }
                case APIType.Redeam:
                    {
                        var redeamAvailability = (RedeamAvailabilities)availabilityData;
                        var redeamSelectedProduct = new RedeamSelectedProduct();
                        redeamSelectedProduct =
                            (RedeamSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, redeamSelectedProduct);

                        var redeamOption =
                            (ActivityOption)redeamSelectedProduct.ProductOptions.FirstOrDefault(x =>
                               x.Id == redeamAvailability?.ServiceOptionId);
                        if (redeamOption != null)
                        {
                            redeamOption.Cancellable = redeamAvailability.Cancellable;
                            redeamOption.Holdable = redeamAvailability.Holdable;
                            redeamOption.HoldablePeriod = redeamAvailability.HoldablePeriod;
                            redeamOption.Type = redeamAvailability.Type;
                            redeamOption.Time = redeamAvailability.Time;
                            redeamOption.Refundable = redeamAvailability.Refundable;
                        }

                        redeamSelectedProduct.SupplierId = redeamAvailability.SupplierId;
                        redeamSelectedProduct.RateId = redeamAvailability.RateId;
                        redeamSelectedProduct.PriceId =
                            SerializeDeSerializeHelper.DeSerialize<Dictionary<string, string>>(redeamAvailability.PriceId);

                        return redeamSelectedProduct;
                    }
                case APIType.Rezdy:
                    {
                        var rezdyAvailability = (RezdyAvailabilities)availabilityData;
                        var rezdySelectedProduct = new RezdySelectedProduct();
                        rezdySelectedProduct = (RezdySelectedProduct)FillSpecializedSelectedProduct(selectedProduct, rezdySelectedProduct);

                        rezdySelectedProduct.ProductCode = rezdyAvailability.SupplierOptionCode;
                        rezdySelectedProduct.Seats = rezdyAvailability.Seats;
                        rezdySelectedProduct.SeatsAvailable = rezdyAvailability.SeatsAvailable;
                        rezdySelectedProduct.StartTimeLocal = rezdyAvailability.StartTimeLocal;
                        rezdySelectedProduct.EndTimeLocal = rezdyAvailability.EndTimeLocal;
                        rezdySelectedProduct.PickUpId = rezdyAvailability.PickUpId;
                        rezdySelectedProduct.BookingQuestions = PrepareRezdyQuestions(requestProduct);
                        if (!string.IsNullOrEmpty(requestProduct?.PickupLocation) && rezdyAvailability.PickUpId > 0)
                        {
                            rezdySelectedProduct.RezdyPickUpLocation = _tableStorageOperation.RetrievePickUpLocation(rezdyAvailability.PickUpId, requestProduct.PickupLocation.Split('-')?.FirstOrDefault());
                        }
                        return rezdySelectedProduct;
                    }
                case APIType.GlobalTix:
                    {
                        var gtAvailability = (GlobalTixAvailabilities)availabilityData;
                        var gtSelectedProduct = new GlobalTixSelectedProduct();
                        gtSelectedProduct = (GlobalTixSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, gtSelectedProduct);

                        // Set RateKey from availabilityData in selected product option
                        ActivityOption actOpt = gtSelectedProduct.ProductOptions.FirstOrDefault(x => x.Id == gtAvailability.ServiceOptionId) as ActivityOption;
                        actOpt.RateKey = gtAvailability.RateKey;
                        actOpt.ContractQuestions = SerializeDeSerializeHelper.DeSerialize<List<Isango.Entities.ContractQuestion>>(gtAvailability.ContractQuestions);
                        gtSelectedProduct.ContractQuestions = PrepareContractQuestions(requestProduct);
                        return gtSelectedProduct;
                    }
                case APIType.Ventrata:
                    {
                        var ventrataAvailability = (VentrataAvailabilities)availabilityData;
                        var ventrataSelectedProduct = new VentrataSelectedProduct();
                        ventrataSelectedProduct = (VentrataSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, ventrataSelectedProduct);

                        // Set RateKey from availabilityData in selected product option
                        //TODO - Tocheck Ventrata optionId and product option id
                        ActivityOption actOpt = ventrataSelectedProduct.ProductOptions.FirstOrDefault(x => x.Id == ventrataAvailability.OptionId) as ActivityOption;
                        //Rate key is used to store Availability id but it comes at P&A level. So what to store in it
                        //Set availability id in Rate key. Fetch it from Base price and availability in availability data.
                        actOpt.RateKey = ventrataAvailability.AvailabilityId;
                        actOpt.VentrataProductId = ventrataAvailability.VentrataProductId;
                        actOpt.SupplierOptionCode = ventrataAvailability.SupplierOptionCode;

                        //Set pick up point details
                        var pickupPointForVentrata = SerializeDeSerializeHelper.DeSerialize<List<PickupPointsDetailsForVentrata>>(ventrataAvailability.PickupPointsDetailsForVentrata);
                        if (pickupPointForVentrata != null && pickupPointForVentrata.Count > 0 && !string.IsNullOrEmpty(actOpt.HotelPickUpLocation))
                        {
                            actOpt.pickupPointId = pickupPointForVentrata.Find(thisPointDetails => thisPointDetails.RandomIntegerId.Equals(Convert.ToInt32(actOpt.HotelPickUpLocation.Split('-')[0]))).Id;
                        }

                        //Set Base URL
                        var getVentrataData = _masterService.GetVentrataData();
                        var getVentrataBaseURLData = getVentrataData?.Where(x => x.SupplierBearerToken == ventrataSelectedProduct.ActivityCode)?.FirstOrDefault()?.BaseURL;
                        if (!string.IsNullOrEmpty(getVentrataBaseURLData))
                        {
                            ventrataSelectedProduct.VentrataBaseURL = getVentrataBaseURLData;
                        }
                        var getVentrataPerPax = getVentrataData?.Where(x => x.SupplierBearerToken == ventrataSelectedProduct.ActivityCode)?.FirstOrDefault()?.IsPerPaxQRCode;
                        if (!string.IsNullOrEmpty(getVentrataPerPax))
                        {
                            ventrataSelectedProduct.VentrataIsPerPaxQRCode = getVentrataPerPax;
                        }
                        return ventrataSelectedProduct;
                    }
                case APIType.TourCMS:
                    {
                        var tourAvailability = (TourCMSAvailabilities)availabilityData;
                        var tourSelectedProduct = new TourCMSSelectedProduct();
                        tourSelectedProduct =
                            (TourCMSSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, tourSelectedProduct);

                        tourSelectedProduct.ProductOptions.ForEach(x =>
                        {
                            var ao = ((ActivityOption)x);
                            ao.RateKey = tourAvailability.RateKey;
                            ao.AvailToken = tourAvailability.RowKey;
                        });
                        tourSelectedProduct.ContractQuestions = PrepareContractQuestions(requestProduct);
                        var pickupPointDetails = tourAvailability.PickupPointsDetails;
                        if (!string.IsNullOrEmpty(pickupPointDetails))
                        {
                            tourSelectedProduct.PickupPointsForTourCMS = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PickupPointsForTourCMS>>(pickupPointDetails);
                        }
                        var availabilityDataCQ = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ContractQuestion>>(tourAvailability?.ContractQuestions);

                        if (tourSelectedProduct.ContractQuestions?.Count == 0 && availabilityDataCQ?.Count > 0)
                        {
                            tourSelectedProduct.ContractQuestions = availabilityDataCQ;
                        }

                        return tourSelectedProduct;
                    }
                case APIType.NewCitySightSeeing:
                    {
                        var newCitySightAvailability = (NewCitySightSeeingAvailabilities)availabilityData;
                        var newCitySelectedProduct = new NewCitySightSeeingSelectedProduct();
                        newCitySelectedProduct =
                            (NewCitySightSeeingSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, newCitySelectedProduct);

                        newCitySelectedProduct.ProductOptions.ForEach(x =>
                        {
                            var ao = ((ActivityOption)x);
                            ao.RateKey = newCitySightAvailability?.RateKey;
                            ao.RateCode = newCitySightAvailability?.RateCode;
                            ao.VariantCondition = newCitySightAvailability.VariantCondition;
                        });
                        return newCitySelectedProduct;
                    }
                case APIType.GoCity:
                    {
                        var goCitySelectedProduct = new GoCitySelectedProduct();
                        goCitySelectedProduct =
                            (GoCitySelectedProduct)FillSpecializedSelectedProduct(selectedProduct,
                                goCitySelectedProduct);
                        return goCitySelectedProduct;
                    }
                case APIType.PrioHub:
                    {
                        var prioAvailability = (PrioHubAvailabilities)availabilityData;
                        var prioHubSelectedProduct = new PrioHubSelectedProduct();
                        prioHubSelectedProduct =
                            (PrioHubSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, prioHubSelectedProduct);

                        prioHubSelectedProduct.PickupPoints = prioAvailability?.PickupPoints;
                        prioHubSelectedProduct.PrioTicketClass = prioAvailability.PrioTicketClass;
                        //Assignment
                        prioHubSelectedProduct.PrioHubProductPaxMapping = SerializeDeSerializeHelper.DeSerialize<List<PrioHubProductPaxMapping>>(prioAvailability?.PrioHubProductPaxMapping);
                        prioHubSelectedProduct.PrioHubAvailabilityId = prioAvailability?.PrioHubAvailabilityId;
                        prioHubSelectedProduct.PrioHubProductGroupCode = prioAvailability?.PrioHubProductGroupCode;
                        prioHubSelectedProduct.PickUpPointForPrioHub = SerializeDeSerializeHelper.DeSerialize<List<PickUpPointForPrioHub>>(prioAvailability?.PickupPointsDetails);
                        prioHubSelectedProduct.PrioHubProductTypeStatus = prioAvailability.PrioHubProductTypeStatus;
                        //Combi Products
                        prioHubSelectedProduct.ProductCombiDetails = SerializeDeSerializeHelper.DeSerialize<List<ProductCombiDetails>>(prioAvailability?.ProductCombiDetails);
                        prioHubSelectedProduct.PrioHubComboSubProduct = SerializeDeSerializeHelper.DeSerialize<List<PrioHubComboSubProduct>>(prioAvailability?.PrioHubComboSubProduct);
                        //Cluster Product
                        prioHubSelectedProduct.Cluster = SerializeDeSerializeHelper.DeSerialize<ProductCluster>(prioAvailability?.PrioHubClusterProduct);

                        //DistributerId
                        prioHubSelectedProduct.PrioHubDistributerId = prioAvailability?.PrioHubDistributerId;

                        var prioOption =
                            prioHubSelectedProduct.ProductOptions.FirstOrDefault(x => x.Id == prioAvailability?.ServiceOptionId);
                        var defaultPriceAndAvailability = prioOption?.SellPrice?.DatePriceAndAvailabilty?.FirstOrDefault().Value;
                        var prioHubPriceAndAvailability = new PrioHubPriceAndAvailability
                        {
                            FromDateTime = prioAvailability.FromDate,
                            ToDateTime = prioAvailability.ToDate,
                            Vacancies = prioAvailability.Vacancies,
                            IsSelected = true,
                            PricingUnits = defaultPriceAndAvailability?.PricingUnits,
                            TotalPrice = defaultPriceAndAvailability?.TotalPrice ?? 0,
                            AvailabilityStatus = defaultPriceAndAvailability.AvailabilityStatus,
                            ReferenceId = defaultPriceAndAvailability.ReferenceId,
                            Capacity = defaultPriceAndAvailability.Capacity
                        };
                        var datePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        {prioOption.TravelInfo.StartDate, prioHubPriceAndAvailability}
                    };
                        prioOption.SellPrice.DatePriceAndAvailabilty = datePriceAndAvailabilty;
                        prioOption.BasePrice.DatePriceAndAvailabilty = datePriceAndAvailabilty;
                        return prioHubSelectedProduct;
                    }
                case APIType.Rayna:
                    {

                        var raynaAvailability = (RaynaAvailabilities)availabilityData;
                        var raynaSelectedProduct = new RaynaSelectedProduct();
                        raynaSelectedProduct =
                            (RaynaSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, raynaSelectedProduct);

                        var raynaOption =
                            (ActivityOption)raynaSelectedProduct.ProductOptions.FirstOrDefault(x =>
                               x.Id == raynaAvailability?.ServiceOptionId);
                        if (raynaOption != null)
                        {
                            raynaOption.TourId = raynaAvailability.TourId;
                            raynaOption.TourOptionId = raynaAvailability.TourOptionId;
                            raynaOption.TransferId = raynaAvailability.TransferId;
                            raynaOption.TimeSlotId = raynaAvailability.TimeSlotId;
                            raynaOption.TourStartTime = raynaAvailability.TourStartTime;
                        }
                        return raynaSelectedProduct;
                    }
                case APIType.RedeamV12:
                    {
                        var redeamAvailability = (RedeamAvailabilities)availabilityData;
                        var redeamSelectedProduct = new CanocalizationSelectedProduct();
                        redeamSelectedProduct =
                            (CanocalizationSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, redeamSelectedProduct);

                        var redeamOption =
                            (ActivityOption)redeamSelectedProduct.ProductOptions.FirstOrDefault(x =>
                               x.Id == redeamAvailability?.ServiceOptionId);
                        if (redeamOption != null)
                        {
                            redeamOption.Cancellable = redeamAvailability.Cancellable;
                            redeamOption.Holdable = redeamAvailability.Holdable;
                            redeamOption.HoldablePeriod = redeamAvailability.HoldablePeriod;
                            redeamOption.Type = redeamAvailability.Type;
                            redeamOption.Time = redeamAvailability.Time;
                            redeamOption.Refundable = redeamAvailability.Refundable;
                        }

                        redeamSelectedProduct.SupplierId = redeamAvailability.SupplierId;
                        redeamSelectedProduct.RateId = redeamAvailability.RateId;
                        redeamSelectedProduct.PriceId =
                            SerializeDeSerializeHelper.DeSerialize<Dictionary<string, string>>(redeamAvailability.PriceId);
                        redeamSelectedProduct.RedeamAvailabilityId = redeamAvailability.RedeamAvailabilityId;
                        redeamSelectedProduct.RedeamAvailabilityStart = redeamAvailability.RedeamAvailabilityStart;
                        return redeamSelectedProduct;
                    }
                case APIType.GlobalTixV3:
                    {
                        var gtAvailability = (GlobalTixV3Availabilities)availabilityData;
                        var gtSelectedProduct = new CanocalizationSelectedProduct();
                        gtSelectedProduct = (CanocalizationSelectedProduct)FillSpecializedSelectedProduct(selectedProduct, gtSelectedProduct);

                        // Set RateKey from availabilityData in selected product option
                        ActivityOption actOpt = gtSelectedProduct.ProductOptions.FirstOrDefault(x => x.Id == gtAvailability.ServiceOptionId) as ActivityOption;
                        actOpt.RateKey = gtAvailability.RateKey;
                        actOpt.ContractQuestions = SerializeDeSerializeHelper.DeSerialize<List<Isango.Entities.ContractQuestion>>(gtAvailability.ContractQuestions);
                        gtSelectedProduct.ContractQuestions = PrepareContractQuestions(requestProduct);
                        return gtSelectedProduct;
                    }
                default:
                    return selectedProduct;
            }
        }

        private SelectedProduct CreateDefaultSelectedProduct(DiscountSelectedProduct discountSelectedProduct,
            BaseAvailabilitiesEntity availabilitiesData, CreateBookingRequest request)
        {
            var xFactor = _masterService
                .GetConversionFactorAsync(availabilitiesData.CurrencyCode, request.CurrencyIsoCode).GetAwaiter()
                .GetResult();
            var requestProduct =
                request.SelectedProducts.FirstOrDefault(
                    x => x.AvailabilityReferenceId.Equals(availabilitiesData.RowKey));

            var selectedProduct = new SelectedProduct();
            var activity = _activityService.GetActivityById(availabilitiesData.ActivityId, DateTime.Now, request.LanguageCode.ToUpper())?.GetAwaiter().GetResult();

            if (activity?.ProductOptions?.Any() != true)
            {
                activity = _activityService.GetActivityById(availabilitiesData.ActivityId, DateTime.Now, Constant.EN)?.GetAwaiter().GetResult();
            }
            var selectedOption = CreateActivityOption(activity, availabilitiesData, request, xFactor);

            selectedProduct.Id = activity.ActivityType == ActivityType.Bundle
                ? selectedOption.ComponentServiceID
                : activity.ID;
            selectedProduct.BundleOptionId = selectedOption.BundleOptionID;
            selectedProduct.Variant = selectedOption.Variant;
            selectedProduct.AvailabilityReferenceId = requestProduct?.AvailabilityReferenceId;
            selectedProduct.Name = activity.Name;
            selectedProduct.ProductType = activity.ProductType;
            selectedProduct.ParentBundleId = availabilitiesData.BundleOptionID;
            selectedProduct.ParentBundleName = availabilitiesData.BundleOptionID > 0 ? activity.Name : "";
            selectedProduct.ProductOptions = new List<ProductOption> { selectedOption };
            selectedProduct.SpecialRequest = requestProduct?.SpecialRequest;
            selectedProduct.HotelPickUpLocation = requestProduct?.PickupLocation;
            selectedProduct.HotelDropoffLocation = requestProduct?.DropOffLocation;
            selectedProduct.UnitType = availabilitiesData.UnitType;
            selectedProduct.RegionId = activity.Regions.Find(city => city.Type.Equals(RegionType.City)).Id.ToString();
            selectedProduct.Regions = activity.Regions;

            decimal totalPrice;
            //TODO: Need to refactor if-else
            if (discountSelectedProduct != null)
            {
                selectedProduct.AppliedDiscountCoupons =
                    GetDiscountCoupons(discountSelectedProduct.AppliedDiscountCoupons);
                totalPrice = CalculateTotalPrice(discountSelectedProduct.SellPrice,
                    discountSelectedProduct.ProductDiscountedPrice, discountSelectedProduct.MultiSaveDiscountedPrice);
                selectedOption.SellPrice.Amount = totalPrice;
                if (discountSelectedProduct.IsSaleProduct == true && discountSelectedProduct.GatePrice == discountSelectedProduct.SellPrice)
                {
                    selectedOption.BasePrice.Amount = discountSelectedProduct.GatePrice;
                    requestProduct.IsSameGateBase = true;
                }
                //selectedOption.SellPrice = new Price
                //{
                //    Amount = totalPrice,
                //    Currency = new Currency { IsoCode = request.CurrencyIsoCode },
                //    DatePriceAndAvailabilty = selectedOption.SellPrice.DatePriceAndAvailabilty

                //};
            }
            else
            {
                selectedProduct.AppliedDiscountCoupons = new List<AppliedDiscountCoupon>();
                totalPrice = availabilitiesData.BasePrice * xFactor;
                selectedOption.SellPrice.Amount = totalPrice;
                //selectedOption.SellPrice = new Price
                //{
                //    Amount = totalPrice,
                //    Currency = new Currency { IsoCode = request.CurrencyIsoCode }
                //};
            }

            selectedProduct.DiscountedPrice = discountSelectedProduct?.ProductDiscountedPrice ?? 0;
            selectedProduct.MultisaveDiscountedPrice = discountSelectedProduct?.MultiSaveDiscountedPrice ?? 0;

            if (selectedOption.PickUpOption.Equals(PickUpDropOffOptionType.Fixed))
                selectedProduct.IsPickupFilled = true;
            else if (selectedOption.PickUpOption.Equals(PickUpDropOffOptionType.RequestedByCustomer))
            {
                if (!string.IsNullOrEmpty(requestProduct?.PickupLocation))
                {
                    selectedProduct.IsPickupFilled = true;
                }
            }
            else if (!string.IsNullOrEmpty(requestProduct?.PickupLocation))
            {
                selectedProduct.IsPickupFilled = true;
                selectedOption.PickUpOption = PickUpDropOffOptionType.RequestedByCustomer;
                selectedOption.HotelPickUpLocation = requestProduct.PickupLocation;
            }

            selectedProduct.City = activity.Regions.Find(city => city.Type.Equals(RegionType.City)).Name?.Trim();
            var country = activity.Regions.Find(city => city.Type.Equals(RegionType.Country));
            selectedProduct.Country = country.Name?.Trim();
            selectedProduct.CountryCode = country.IsoCode?.Trim();

            if (!string.IsNullOrEmpty(activity?.CoOrdinates))
            {
                var productCoordinates = activity.CoOrdinates.Split(',');
                if (productCoordinates.Length > 1)
                {
                    selectedProduct.Latitude = productCoordinates[0];
                    selectedProduct.Longitude = productCoordinates[1];
                }
            }

            var categoryId = activity?.PriorityWiseCategory?.OrderBy(x => x.Value)?.FirstOrDefault().Key ?? 0;
            if (categoryId != 0)
            {
                var categoryName = _activityService.LoadRegionCategoryMapping()?
                    .FirstOrDefault(x => x.CategoryId == categoryId)?
                    .CategoryName ?? string.Empty;
                if (!string.IsNullOrEmpty(categoryName))
                {
                    selectedProduct.Category = categoryName;
                }
            }

            if (activity.IsServiceLevelPickUp)
            {
                selectedProduct.IsServiceLevelPickUp = true;
                selectedProduct.PickUpOption = selectedOption.PickUpOption;
                selectedProduct.DropOffOption = selectedOption.PickUpOption;
                selectedProduct.ScheduleReturnDetails = activity.ScheduleReturnDetails;
            }
            else
            {
                selectedProduct.IsServiceLevelPickUp = false;
            }

            selectedProduct.ScheduleLocation = activity.ScheduleLocation;
            selectedProduct.Duration = activity.Duration;
            selectedProduct.DurationString = activity.DurationString;
            selectedProduct.ActivityType = activity.ActivityType;
            selectedProduct.Time = activity.Time;
            selectedProduct.TsProductCode = activity.ShortName;

            try
            {
                if (selectedOption.StartTime != null && selectedOption.StartTime != default(TimeSpan))
                {
                    selectedProduct.StartTime = selectedOption.StartTime.ToString();
                }
                else
                {
                    selectedProduct.StartTime = activity.Schedule;
                }
            }
            catch (Exception ex)
            {
                selectedProduct.StartTime = activity.Schedule;
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingMapper",
                    MethodName = "CreateDefaultSelectedProduct",
                };
                _log.Error(isangoErrorEntity, ex);
            }

            selectedProduct.IsPaxDetailRequired = activity.IsPaxDetailRequired;
            selectedProduct.IsReceipt = activity.IsReceipt;
            selectedProduct.OnSale = availabilitiesData.OnSale;

            var ImportantNotes = new StringBuilder();
            ImportantNotes.Append("<b> " + "When The Activity Operates" + " </b>");
            ImportantNotes.Append("<br />");
            ImportantNotes.Append(activity.ScheduleOperates);
            if (activity.ScheduleUnavailableDates != string.Empty)
            {
                ImportantNotes.Append("<br />");
                ImportantNotes.Append(" <b>" + "Except" + " </b>");
                ImportantNotes.Append(activity.ScheduleUnavailableDates);
            }

            selectedProduct.AvailabilityInformation = ImportantNotes.ToString();
            selectedProduct.Itineraries = activity.Itineraries;
            selectedProduct.CancellationPolicy = activity.CancellationPolicy;
            selectedProduct.APIType = activity.ApiType;

            selectedProduct.Price = totalPrice;
            selectedProduct.ActivityCode = activity.Code;

            return selectedProduct;
        }

        private ActivityOption CreateActivityOption(Activity activity, BaseAvailabilitiesEntity availabilityData, CreateBookingRequest request, decimal exchangeRateValue)
        {
            ProductOption productOption;
            //HB Activity
            if (activity.ApiType == APIType.Hotelbeds && !string.IsNullOrEmpty(availabilityData.SupplierOptionCode))
            {
                productOption = activity.ProductOptions.FirstOrDefault(x =>
                    x.PrefixServiceCode.Equals(availabilityData.SupplierOptionCode));
                productOption.Id = availabilityData.OptionId; //availabilityData.ServiceOptionId;
                productOption.IsSelected = true;
                //TODO : Set option name from table storage.
                //poption.Name = availabilityData.
            }
            else if (activity.ApiType == APIType.Bokun
                     || activity.ApiType == APIType.Fareharbor
                     || activity.ApiType == APIType.Tiqets
                     || activity.ApiType == APIType.Goldentours
                     || activity.ApiType == APIType.Redeam
                     || activity.ApiType == APIType.Undefined
            )
            {
                productOption = activity.ProductOptions.FirstOrDefault(x => x.Id.Equals(availabilityData.ServiceOptionId));
                if (productOption == null)
                {
                    productOption = activity.ProductOptions.FirstOrDefault(x => x.Id.Equals(availabilityData.OptionId));
                }
            }
            else
            {
                productOption = activity.ProductOptions.FirstOrDefault(x => x.Id.Equals(availabilityData.OptionId));
                if (productOption == null)
                {
                    productOption = activity.ProductOptions.FirstOrDefault(x => x.Id.Equals(availabilityData.ServiceOptionId));
                }
            }

            Enum.TryParse(availabilityData.AvailabilityStatus, true, out AvailabilityStatus availabilityStatus);
            var selectedProduct =
                request.SelectedProducts.FirstOrDefault(x => x.AvailabilityReferenceId.Equals(availabilityData.RowKey));
            var travelInfo = SerializeDeSerializeHelper.DeSerialize<TravelInfo>(availabilityData.TravelInfo);

            TimeSpan.TryParse(availabilityData.TimeSlot, out var startTimeSlot);
            var perPersonPricingUnit = new List<PerPersonPricingUnit>();
            if (activity.ApiType == APIType.Rayna)
            {
                perPersonPricingUnit = SerializeDeSerializeHelper.DeSerialize<List<PerPersonPricingUnit>>(availabilityData.BasePricingUnits);
            }
            var basePricingUnits = SerializeDeSerializeHelper.DeSerialize<List<PricingUnit>>(availabilityData.BasePricingUnits);

            var adultBasePricingUnits = SerializeDeSerializeHelper.DeSerialize<List<PerPersonPricingUnit>>(availabilityData.BasePricingUnits);

            var baseDatePriceAndAvailability = new Dictionary<DateTime, PriceAndAvailability>()
                {
                    {
                        travelInfo.StartDate,
                        new DefaultPriceAndAvailability()
                            {
                                PricingUnits = basePricingUnits,
                                TotalPrice = availabilityData.BasePrice,
                                AvailabilityStatus = availabilityStatus,
                                Capacity = basePricingUnits.Capacity,
                                ReferenceId = availabilityData.RowKey,
                                PerPersonPricingUnit=perPersonPricingUnit
                            }
                    }
                };

            var baseGateDatePriceAndAvailability = new Dictionary<DateTime, PriceAndAvailability>();
            if (activity.ApiType == APIType.Rezdy)
            {
                var gateBasePricingUnits = SerializeDeSerializeHelper.DeSerialize<List<PricingUnit>>(availabilityData.GateBasePricingUnits);

                baseGateDatePriceAndAvailability = new Dictionary<DateTime, PriceAndAvailability>()
                {
                    {
                        travelInfo.StartDate,
                        new DefaultPriceAndAvailability()
                            {
                                PricingUnits = gateBasePricingUnits,
                                TotalPrice = availabilityData.GateBasePrice,
                                AvailabilityStatus = availabilityStatus,
                                Capacity = basePricingUnits.Capacity,
                                ReferenceId = availabilityData.RowKey,
                                PerPersonPricingUnit=perPersonPricingUnit
                            }
                    }
                };
            }

            var sellPricingUnits = SerializeDeSerializeHelper.DeSerialize<List<PricingUnit>>(availabilityData.BasePricingUnits);
            sellPricingUnits?.ForEach(x => x.Price = x.Price * exchangeRateValue);
            var sellPrice = new Price
            {
                Amount = availabilityData.BasePrice * exchangeRateValue,
                Currency = new Currency { IsoCode = request.CurrencyIsoCode },
                DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
                {
                    {
                        travelInfo.StartDate,
                        new DefaultPriceAndAvailability()
                            {
                                PricingUnits = sellPricingUnits,
                                TotalPrice = availabilityData.BasePrice * exchangeRateValue,
                                AvailabilityStatus = availabilityStatus,
                                Capacity = basePricingUnits.Capacity,
                                ReferenceId = availabilityData.RowKey,
                                PerPersonPricingUnit=perPersonPricingUnit
                            }
                    }
                }
            };
            if (activity.ApiType == APIType.GlobalTix)
            {
                baseDatePriceAndAvailability[travelInfo.StartDate].TourDepartureId = ((GlobalTixAvailabilities)availabilityData).TourDepartureId;
                sellPrice.DatePriceAndAvailabilty[travelInfo.StartDate].TourDepartureId = ((GlobalTixAvailabilities)availabilityData).TourDepartureId;
            }
            else if (activity.ApiType == APIType.GlobalTixV3)
            {
                baseDatePriceAndAvailability[travelInfo.StartDate].TourDepartureId = ((GlobalTixV3Availabilities)availabilityData).TourDepartureId;
                sellPrice.DatePriceAndAvailabilty[travelInfo.StartDate].TourDepartureId = ((GlobalTixV3Availabilities)availabilityData).TourDepartureId;
            }
            var activityOption = new ActivityOption
            {
                // ReSharper disable once PossibleNullReferenceException
                Id = productOption.Id,
                ServiceOptionId = availabilityData.ServiceOptionId,
                Name = availabilityData.OptionName,
                SupplierName = productOption.SupplierName,
                Description = productOption.Description,
                SellPrice = productOption?.SellPrice?.DatePriceAndAvailabilty?.Count > 0 ? productOption.SellPrice : sellPrice,
                IsSelected = true,
                ComponentOrder = availabilityData.ComponentOrder,
                ComponentServiceID = availabilityData.ComponentServiceID,
                PriceTypeID = availabilityData.PriceTypeID,
                BundleOptionID = availabilityData.BundleOptionID,
                BundleOptionName = availabilityData.BundleOptionName,
                IsSameDayBookable = availabilityData.IsSameDayBookable,
                SupplierOptionCode = availabilityData.SupplierOptionCode,
                Margin = new Margin { Value = availabilityData.Margin, IsPercentage = true },
                BasePrice = new Price
                {
                    Amount = availabilityData.BasePrice,
                    Currency = new Currency { IsoCode = availabilityData.CurrencyCode },
                    DatePriceAndAvailabilty = baseDatePriceAndAvailability
                },
                CostPrice = new Price
                {
                    Amount = availabilityData.CostPrice,
                    Currency = new Currency { IsoCode = availabilityData.CurrencyCode }
                },
                GateBasePrice = new Price
                {
                    Amount = availabilityData.GateBasePrice,
                    Currency = new Currency { IsoCode = availabilityData.CurrencyCode }
                },
                AvailabilityStatus = availabilityStatus,
                Customers = GetCustomers(selectedProduct?.PassengerDetails, travelInfo, request.UserEmail,
                    activity.IsPaxDetailRequired, activity.PassengerInfo),
                TravelInfo = travelInfo,
                StartTime = startTimeSlot,
                Variant = availabilityData.Variant,
                CancellationPrices = SerializeDeSerializeHelper.DeSerialize<List<CancellationPrice>>(availabilityData.CancellationPrices),
                CancellationText = availabilityData.CancellationText,
                ApiCancellationPolicy = availabilityData.ApiCancellationPolicy
            };
            if (activity.ApiType == APIType.Rezdy)
            {
                activityOption.GateBasePrice.DatePriceAndAvailabilty = baseGateDatePriceAndAvailability;
            }
            if (activity.ApiType == APIType.Rayna) //need to set cost availability 
            {
                var perPersonPricingUnitCost = new List<PerPersonPricingUnit>();
                perPersonPricingUnitCost = SerializeDeSerializeHelper.DeSerialize<List<PerPersonPricingUnit>>(availabilityData.CostPricingUnits);
                var costPricingUnits = SerializeDeSerializeHelper.DeSerialize<List<PricingUnit>>(availabilityData.CostPricingUnits);
                var costDatePriceAndAvailability = new Dictionary<DateTime, PriceAndAvailability>()
                {
                    {
                        travelInfo.StartDate,
                        new DefaultPriceAndAvailability()
                            {
                                PricingUnits = costPricingUnits,
                                TotalPrice = availabilityData.CostPrice,
                                AvailabilityStatus = availabilityStatus,
                                Capacity = basePricingUnits.Capacity,
                                ReferenceId = availabilityData.RowKey,
                                PerPersonPricingUnit=perPersonPricingUnit
                            }
                    }
                };
                activityOption.CostPrice.DatePriceAndAvailabilty = costDatePriceAndAvailability;
            }
            if (activity.ApiType == APIType.PrioHub && !string.IsNullOrWhiteSpace(availabilityData?.TimeSlot) && availabilityData.TimeSlot != "00:00:00")
            {
                TimeSpan timeSpan;
                string displayTime = string.Empty;
                if (TimeSpan.TryParse(availabilityData?.TimeSlot, out timeSpan))
                {
                    DateTime time = DateTime.Today.Add(timeSpan);
                    displayTime = time.ToString("hh:mm tt"); // It will give "03:00 AM"
                }
                activityOption.Name = activityOption.Name + " @ " + displayTime;
            }

            else if (!string.IsNullOrWhiteSpace(availabilityData?.TimeSlot) && availabilityData.TimeSlot != "00:00:00")
            {
                activityOption.Name = activityOption.Name + " @ " + availabilityData.TimeSlot;
            }

            try
            {
                foreach (var pu in adultBasePricingUnits)
                {
                    var customers = activityOption?.Customers?.Where(x => x.PassengerType == pu.PassengerType).ToList();
                    if (customers?.Count() == pu?.Ages?.Count)
                    {
                        for (var i = 0; i < pu.Ages.Count; i++)
                        {
                            customers[i].Age = Convert.ToInt32(pu?.Ages[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return activityOption;
        }

        private List<AppliedDiscountCoupon> GetDiscountCoupons(
            List<DiscountRuleEngine.Model.DiscountCoupon> discountCoupons)
        {
            var appliedDiscountCoupons = new List<AppliedDiscountCoupon>();
            if (discountCoupons == null) return appliedDiscountCoupons;

            foreach (var discountCoupon in discountCoupons)
            {
                var appliedDiscountCoupon = new AppliedDiscountCoupon
                {
                    Code = discountCoupon.DiscountCouponCode,
                    Type = discountCoupon.DiscountType,
                    Price = discountCoupon.DiscountedPrice
                };
                appliedDiscountCoupons.Add(appliedDiscountCoupon);
            }

            return appliedDiscountCoupons;
        }

        private SelectedProduct FillSpecializedSelectedProduct(SelectedProduct selectedProduct,
            SelectedProduct specializedSelectedProduct)
        {
            specializedSelectedProduct.Id = selectedProduct.Id;
            specializedSelectedProduct.AvailabilityReferenceId = selectedProduct.AvailabilityReferenceId;
            specializedSelectedProduct.Name = selectedProduct.Name;
            specializedSelectedProduct.HotelPickUpLocation = selectedProduct.HotelPickUpLocation;
            specializedSelectedProduct.HotelDropoffLocation = selectedProduct?.HotelDropoffLocation;
            specializedSelectedProduct.OnSale = selectedProduct.OnSale;
            specializedSelectedProduct.ProductType = selectedProduct.ProductType;
            specializedSelectedProduct.ProductOptions = selectedProduct.ProductOptions;
            specializedSelectedProduct.Duration = selectedProduct.Duration;
            specializedSelectedProduct.Time = selectedProduct.Time;
            specializedSelectedProduct.ScheduleReturnDetails = selectedProduct.ScheduleReturnDetails;
            specializedSelectedProduct.Supplier = selectedProduct.Supplier;
            specializedSelectedProduct.TsProductName = selectedProduct.TsProductName;
            specializedSelectedProduct.TsProductCode = selectedProduct.TsProductCode;
            specializedSelectedProduct.SupplierPrice = selectedProduct.SupplierPrice;
            specializedSelectedProduct.SupplierCurrency = selectedProduct.SupplierCurrency;
            specializedSelectedProduct.SellPrice = selectedProduct.SellPrice;
            specializedSelectedProduct.MultisaveDiscountedPrice = selectedProduct.MultisaveDiscountedPrice;
            specializedSelectedProduct.StartTime = selectedProduct.StartTime;
            specializedSelectedProduct.IsServiceLevelPickUp = selectedProduct.IsServiceLevelPickUp;
            specializedSelectedProduct.IsPickupFilled = selectedProduct.IsPickupFilled;
            specializedSelectedProduct.CancellationPolicy = selectedProduct.CancellationPolicy;
            specializedSelectedProduct.AvailabilityInformation = selectedProduct.AvailabilityInformation;
            specializedSelectedProduct.ThumbNailImage = selectedProduct.ThumbNailImage;
            specializedSelectedProduct.ActivityCode = selectedProduct.ActivityCode;
            specializedSelectedProduct.ProductName = selectedProduct.ProductName;
            specializedSelectedProduct.ProductId = selectedProduct.ProductId;
            specializedSelectedProduct.Price = selectedProduct.Price;
            specializedSelectedProduct.ActivityType = selectedProduct.ActivityType;
            specializedSelectedProduct.IsPaxDetailRequired = selectedProduct.IsPaxDetailRequired;
            specializedSelectedProduct.DiscountedPrice = selectedProduct.DiscountedPrice;
            specializedSelectedProduct.SpecialRequest = selectedProduct.SpecialRequest;
            specializedSelectedProduct.RegionId = selectedProduct.RegionId;
            specializedSelectedProduct.ProductSeoUrl = selectedProduct.ProductSeoUrl;
            specializedSelectedProduct.AttractionIds = selectedProduct.AttractionIds;
            specializedSelectedProduct.ImagePath = selectedProduct.ImagePath;
            specializedSelectedProduct.ImageId = selectedProduct.ImageId;
            specializedSelectedProduct.City = selectedProduct.City;
            specializedSelectedProduct.Country = selectedProduct.Country;
            specializedSelectedProduct.IsPackage = selectedProduct.IsPackage;
            specializedSelectedProduct.PriceWithSymbol = selectedProduct.PriceWithSymbol;
            specializedSelectedProduct.IsSmartPhoneVoucher = selectedProduct.IsSmartPhoneVoucher;
            specializedSelectedProduct.NodeAttractionId = selectedProduct.NodeAttractionId;
            specializedSelectedProduct.NodeAttractionName = selectedProduct.NodeAttractionName;
            specializedSelectedProduct.CategoryTypes = selectedProduct.CategoryTypes;
            specializedSelectedProduct.APIType = selectedProduct.APIType;
            specializedSelectedProduct.ParentBundleId = selectedProduct.ParentBundleId;
            specializedSelectedProduct.ParentBundleName = selectedProduct.ParentBundleName;
            specializedSelectedProduct.ActivityOperator = selectedProduct.ActivityOperator;
            specializedSelectedProduct.ScheduleLocation = selectedProduct.ScheduleLocation;
            specializedSelectedProduct.PickUpOption = selectedProduct.PickUpOption;
            specializedSelectedProduct.DropOffOption = selectedProduct.DropOffOption;
            specializedSelectedProduct.IsReceipt = selectedProduct.IsReceipt;
            specializedSelectedProduct.Itineraries = selectedProduct.Itineraries;
            specializedSelectedProduct.AppliedDiscountCoupons = selectedProduct.AppliedDiscountCoupons;
            specializedSelectedProduct.SupplierConfirmationData = selectedProduct.SupplierConfirmationData;
            specializedSelectedProduct.CartReferenceId = selectedProduct.CartReferenceId;
            specializedSelectedProduct.Expiry = selectedProduct.Expiry;
            specializedSelectedProduct.UnitType = selectedProduct.UnitType;
            specializedSelectedProduct.BundleOptionId = selectedProduct.BundleOptionId;
            specializedSelectedProduct.Regions = selectedProduct.Regions;
            specializedSelectedProduct.CountryCode = selectedProduct.CountryCode;
            specializedSelectedProduct.Latitude = selectedProduct.Latitude;
            specializedSelectedProduct.Longitude = selectedProduct.Longitude;

            return specializedSelectedProduct;
        }

        private decimal CalculateTotalPrice(decimal price, decimal totalDiscountedPrice,
            decimal totalMultiSaveDiscountedPrice)
        {
            var totalPrice = price - (totalDiscountedPrice + totalMultiSaveDiscountedPrice);
            return totalPrice <= 0 ? 0 : totalPrice;
        }

        private List<BokunQuestion> PrepareBokunQuestions(RequestSelectedProduct selectedProduct, string userEmail)
        {
            bool _isNotificationOn = false;
            bool _isSendNotificationToCustomer = false;
            bool _isSendNotificationToIsango = false;
            var _notificationEmailAddressIsango = string.Empty;
            var _supportPhoneNumber = string.Empty;
            var questions = new List<BokunQuestion>();
            try
            {
                _supportPhoneNumber = ConfigurationManagerHelper.GetValuefromAppSettings("SupportPhoneNumer");
                _isNotificationOn = ConfigurationManagerHelper.GetValuefromAppSettings("BokunIsNotificationOn") == "1";
                if (_isNotificationOn)
                {
                    _isSendNotificationToCustomer =
                        ConfigurationManagerHelper.GetValuefromAppSettings("BokunIsSendNotificationToCustomer") == "1";

                    _notificationEmailAddressIsango =
                        ConfigurationManagerHelper.GetValuefromAppSettings("BokunNotificationEmailAddressIsango");

                    if (_isSendNotificationToCustomer)
                    {
                        _isSendNotificationToIsango = false;
                        _notificationEmailAddressIsango = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                _isNotificationOn = false;
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingMapper",
                    MethodName = "PrepareBokunQuestions",
                };
                _log.Error(isangoErrorEntity, ex);
            }

            if (selectedProduct.PassengerDetails == null) return new List<BokunQuestion>();
            var leadPassenger = selectedProduct.PassengerDetails.FirstOrDefault(x => x.IsLeadPassenger);

            var emailAnswer = userEmail;
            if (_isNotificationOn)
            {
                if (_isSendNotificationToCustomer)
                {
                    emailAnswer = userEmail;
                }
                else if (_isSendNotificationToIsango
                         && !string.IsNullOrWhiteSpace(_notificationEmailAddressIsango)
                )
                {
                    emailAnswer = _notificationEmailAddressIsango;
                }
            }

            var firstName = new BokunQuestion
            {
                QuestionId = Constant.FirstName,
                Answers = new List<string> { leadPassenger.FirstName },
                QuestionType = Constant.QuestionType,
            };
            var lastName = new BokunQuestion
            {
                QuestionId = Constant.LastName,
                Answers = new List<string> { leadPassenger.LastName },
                QuestionType = Constant.QuestionType,
            };

            var email = new BokunQuestion
            {
                QuestionId = Constant.Email,
                Answers = new List<string> { emailAnswer },
                QuestionType = Constant.QuestionType,
            };
            questions = new List<BokunQuestion> { firstName, lastName, email };

            if (selectedProduct?.Questions?.Count > 0)
            {
                foreach (var question in selectedProduct?.Questions)
                {
                    if (question?.Id != Constant.FirstName
                        || question?.Id != Constant.LastName
                        || question?.Id != Constant.Email
                    )
                    {
                        var bokunQuestion = new BokunQuestion
                        {
                            QuestionId = question?.Id,
                            Answers = new List<string> { question?.Answer },
                            Label = question?.Label,
                            QuestionType = question.QuestionType ?? Constant.QuestionType,
                        };
                        switch (question?.Id)
                        {
                            case Constant.Phonenumber:
                                {
                                    bokunQuestion.Answers = new List<string> { _supportPhoneNumber };
                                    break;
                                }
                        }

                        questions.Add(bokunQuestion);
                    }
                }
            }

            return questions;
        }

        private List<ContractQuestion> PrepareContractQuestions(RequestSelectedProduct selectedProduct)
        {
            var questions = new List<ContractQuestion>();
            if (selectedProduct.Questions == null) return questions;

            foreach (var productQuestion in selectedProduct.Questions)
            {
                var question = new ContractQuestion
                {
                    Code = productQuestion.Id,
                    Name = productQuestion.Label,
                    Description = productQuestion.Answer,
                    IsRequired = productQuestion.IsRequired,
                    Answer = productQuestion.Answer //Introduced while HB Apitude
                };
                questions.Add(question);
            }

            return questions;
        }

        private List<Customer> GetCustomers(List<PassengerDetail> passengerDetails, TravelInfo travelInfo,
            string userEmail, bool isPaxDetailRequired, List<BookingPassengerInfo> passengerInformations)
        {
            var customers = new List<Customer>();
            if (passengerDetails == null) return customers;

            if (!isPaxDetailRequired)
            {
                customers = GetDummyCustomers(travelInfo, passengerInformations);
                var passengerDetail = passengerDetails.FirstOrDefault(x => x.IsLeadPassenger);
                var customer = new Customer
                {
                    FirstName = passengerDetail?.FirstName,
                    LastName = passengerDetail?.LastName,
                    Email = userEmail,
                    IsLeadCustomer = passengerDetail?.IsLeadPassenger ?? false,
                    PassengerType = PassengerType.Adult,
                    Age = Constant.AdultAge,
                    PassportNationality = passengerDetail?.PassportNationality,
                    PassportNumber = passengerDetail?.PassportNumber,
                    AgeSupplier = passengerDetail?.AgeSupplier
                };
                customers.Add(customer);
            }
            else
            {
                foreach (var passengerDetail in passengerDetails)
                {
                    var passengerInfo
                        = //passengerInformations.FirstOrDefault(x => x.AgeGroupId == passengerDetail.AgeGroupId)
                          //??
                        passengerInformations.FirstOrDefault(x => x.PassengerTypeId == passengerDetail.PassengerTypeId);

                    var childAge = passengerInfo == null ? 0 : passengerInfo?.FromAge ?? 0;
                    var customer = new Customer
                    {
                        FirstName = passengerDetail.FirstName,
                        LastName = passengerDetail.LastName,
                        Email = userEmail,
                        IsLeadCustomer = passengerDetail.IsLeadPassenger,
                        PassengerType = passengerInfo == null ? 0 : (PassengerType)passengerInfo.PassengerTypeId,
                        PassportNumber = passengerDetail.PassportNumber,
                        PassportNationality = passengerDetail.PassportNationality,
                        AgeSupplier = passengerDetail.AgeSupplier
                    };
                    customer.Age = customer.PassengerType.Equals(PassengerType.Adult) ? Constant.AdultAge : childAge;
                    customers.Add(customer);
                }
            }

            return customers;
        }

        private List<Customer> GetDummyCustomers(TravelInfo travelInfo,
            List<BookingPassengerInfo> passengerInformations)
        {
            var customers = new List<Customer>();
            foreach (var key in travelInfo.NoOfPassengers.Keys)
            {
                var noOfPassengers = key == PassengerType.Adult
                    ? travelInfo.NoOfPassengers[key] - 1
                    : travelInfo.NoOfPassengers[key];

                customers.AddRange(GetDummyCustomersByPassengerType(noOfPassengers, passengerInformations, key));
            }

            return customers;
        }

        private List<Customer> GetDummyCustomersByPassengerType(int noOfPassengers,
            List<BookingPassengerInfo> passengerInformations, PassengerType key)
        {
            var customers = new List<Customer>();
            int? age;
            if (key == PassengerType.Adult)
            {
                age = Constant.AdultAge;
            }
            else
            {
                var ages = passengerInformations.Where(x => ((PassengerType)x.PassengerTypeId == key));
                age = ages.FirstOrDefault(x => x.ToAge == ages.Max(y => y.ToAge))?.ToAge;
            }

            for (int i = 0; i < noOfPassengers; i++)
            {
                var customer = new Customer
                {
                    FirstName = $"{key}FirstName{i + 1}",
                    LastName = $"{key}LastName{i + 1}",
                    PassengerType = key,
                    Age = age ?? 0,
                    IsLeadCustomer = false
                };
                customers.Add(customer);
            }

            return customers;
        }

        private IsangoBookingData MapIsangoBookingData(CreateBookingRequest request)
        {
            try
            {
                var registeredCustomerDetail = GetRegisteredCustomerDetails(request);
                var customerLocation = GetCustomerLocation(request);
                var bookedProducts = GetBookedProduct(request);
                var isangoBookingData = new IsangoBookingData
                {
                    AffiliateId = request.AffiliateId,
                    TokenId = request.TokenId,
                    CurrencyIsoCode = request.CurrencyIsoCode,
                    LanguageCode = request.LanguageCode,
                    CustomerEmailId = request.UserEmail,
                    AgentEmail = request.AgentEmailID,
                    AgentID = request.AgentID,
                    PaymentMethodType = request?.PaymentDetail?.PaymentMethodType,
                    UTMParameter = request.UTMParameter,
                    BookingAgent = request.BookingAgent,
                    IsGuestUser = request.IsGuestUser,
                    RegisteredCustomerDetail = registeredCustomerDetail,
                    CustomerLocation = customerLocation,
                    BookedProducts = bookedProducts,
                    ExternalReferenceNumber = request?.ExternalReferenceNumber,
                    VistaraMemberNumber = request?.VistaraMemberNumber,
                    CVPoints = request?.CVPoints
                };

                return isangoBookingData;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingMapper",
                    MethodName = "MapIsangoBookingData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> GetBookedProduct(CreateBookingRequest request)
        {
            try
            {
                var bookedProducts = new List<BookedProduct>();

                foreach (var selectedProduct in request.SelectedProducts)
                {
                    var availabilityData = _tableStorageOperation.RetrieveData<BaseAvailabilitiesEntity>(selectedProduct.AvailabilityReferenceId,
                            request.TokenId);
                    if (availabilityData == null) continue;

                    var activity = _activityService.GetActivityById(availabilityData.ActivityId, DateTime.Today, request.LanguageCode.ToUpperInvariant())?.GetAwaiter().GetResult();

                    if (activity?.ProductOptions?.Any() != true)
                    {
                        activity = _activityService.GetActivityById(availabilityData.ActivityId, DateTime.Today, Constant.EN)?.GetAwaiter().GetResult();
                    }
                    if (activity == null) continue;

                    var isPaxDetailRequired = activity.IsPaxDetailRequired;
                    if (activity.ActivityType.Equals(ActivityType.Bundle))
                    {
                        activity = _activityService.GetActivityById(availabilityData.ComponentServiceID, DateTime.Today, Constant.EN)?.GetAwaiter().GetResult();
                    }

                    var xFactor = _masterService
                        .GetConversionFactorAsync(availabilityData.CurrencyCode, request.CurrencyIsoCode).GetAwaiter()
                        .GetResult();
                    var basePricingUnits = GetPricingUnits(availabilityData.BasePricingUnits);

                    var travelInfo = GetTravelInfo(availabilityData.TravelInfo);
                    var personChangedForProductBooked = travelInfo?.NoOfPassengers?.Where(x => x.Key.ToString() != PassengerType.Infant.ToString())?.Sum(x => x.Value) ?? 0;
                    var startTime = default(TimeSpan);
                    var isStartTime = false;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(availabilityData?.TimeSlot) && availabilityData?.TimeSlot != "00:00:00")
                        {
                            isStartTime = TimeSpan.TryParse(availabilityData.TimeSlot, out startTime);
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "BookingMapper",
                            MethodName = "GetBookedProduct",
                        };
                        _log.Error(isangoErrorEntity, ex);
                    }

                    var bookedProduct = new BookedProduct
                    {
                        ServiceId = Convert.ToInt32(activity.Id),
                        IsPaxDetailRequired = isPaxDetailRequired,
                        AvailabilityReferenceId = selectedProduct.AvailabilityReferenceId,
                        OptionPrice = new OptionPrice
                        {
                            BasePrice = selectedProduct.IsSameGateBase == true ? availabilityData.GateBasePrice : availabilityData.BasePrice,
                            CostPrice = availabilityData.CostPrice,
                            GatePrice = availabilityData.GateBasePrice,
                            CostCurrency = availabilityData.CurrencyCode ?? (activity.ApiType == APIType.PrioHub ? request.CurrencyIsoCode : availabilityData.CurrencyCode),
                            Quantity = basePricingUnits.FirstOrDefault()?.Quantity ?? 0,
                            CostROE = xFactor,
                            SellROE = xFactor,
                            SellPrice = selectedProduct.IsSameGateBase == true ? availabilityData.GateBasePrice * xFactor : availabilityData.BasePrice * xFactor,
                            PersonCharged = personChangedForProductBooked
                        },
                        OptionName = isStartTime ? availabilityData.OptionName + " @ " + availabilityData.TimeSlot
                                                : availabilityData.OptionName,

                        AppliedSales = GetAppliedSales(availabilityData.RowKey, availabilityData.PriceOfferReferenceId),
                        PassengerDetails = GetPassengers(availabilityData, selectedProduct, xFactor,
                            activity.PassengerInfo, isPaxDetailRequired, selectedProduct.IsSameGateBase),
                        Time = isStartTime ? availabilityData.TimeSlot : null,
                        CheckinDate = travelInfo.StartDate,
                        OptionId = availabilityData.ServiceOptionId,
                        OptionStatus = availabilityData.AvailabilityStatus,
                        IsShowSupplierVoucher = activity?.ISSHOWSUPPLIERVOUCHER ?? false
                    };

                    var adultBasePricingUnits = SerializeDeSerializeHelper.DeSerialize<List<PerPersonPricingUnit>>(availabilityData.BasePricingUnits);

                    try
                    {
                        foreach (var punit in adultBasePricingUnits)
                        {
                            var customers = bookedProduct?.PassengerDetails?.Where(x => x.PassengerTypeId == (int)punit.PassengerType).ToList();
                            if (customers?.Count() == punit?.Ages?.Count)
                            {
                                for (var i = 0; i < punit.Ages.Count; i++)
                                {
                                    customers[i].Age = Convert.ToInt32(punit?.Ages[i]);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    bookedProducts.Add(bookedProduct);
                }

                return bookedProducts;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingMapper",
                    MethodName = "GetBookedProduct",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<Isango.Entities.PassengerDetail> GetPassengers(BaseAvailabilitiesEntity availabilityData,
            RequestSelectedProduct selectedProduct, decimal xFactor, List<BookingPassengerInfo> passengerInfo,
            bool isPaxDetailRequired, bool isSameGateBase = false)
        {
            var basePricingUnits = GetPricingUnits(availabilityData.BasePricingUnits);
            if (isSameGateBase == true)
            {
                basePricingUnits = GetPricingUnits(availabilityData.GateBasePricingUnits);
            }
            var costPricingUnits = GetPricingUnits(availabilityData.CostPricingUnits);
            var gatePricingUnits = GetPricingUnits(availabilityData.GateBasePricingUnits);

            var adultBasePricingUnits = SerializeDeSerializeHelper.DeSerialize<List<AdultPricingUnit>>(availabilityData.BasePricingUnits);

            var passengerDetails = new List<Isango.Entities.PassengerDetail>();
            var travelInfo = SerializeDeSerializeHelper.DeSerialize<TravelInfo>(availabilityData.TravelInfo);

            var personChangedForProductBooked = travelInfo?.NoOfPassengers?.Sum(x => x.Value) ?? 0;

            var leadPaxTypeId = PassengerType.Undefined;
            if ((!isPaxDetailRequired) || (personChangedForProductBooked != selectedProduct?.PassengerDetails?.Count))
            {
                var leadPassenger = selectedProduct.PassengerDetails.FirstOrDefault(x => x.IsLeadPassenger);

                if (leadPassenger == null)
                {
                    leadPassenger = selectedProduct.PassengerDetails.FirstOrDefault();
                }
                //#TODO Remove Age group and use passenger type
                var leadPassengerInfo = //passengerInfo?.FirstOrDefault(x => x.AgeGroupId == leadPassenger?.AgeGroupId)
                                        //??
                    passengerInfo?.FirstOrDefault(x => x.PassengerTypeId == leadPassenger.PassengerTypeId)
                    ?? passengerInfo?.FirstOrDefault(x => x.IndependablePax && x.PassengerTypeId == 1)
                    ?? passengerInfo?.FirstOrDefault(x => x.IndependablePax);

                if (leadPassengerInfo != null && (PassengerType)leadPassengerInfo.PassengerTypeId > 0)
                {
                    leadPaxTypeId = (PassengerType)leadPassengerInfo.PassengerTypeId;
                }
                else
                {
                    var message = "Age group id could not be resolved for lead passenger.";
                    var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        ReasonPhrase = message
                    };
                    throw new HttpResponseException(data);
                }

                var pxd = GetDummyPassengers(travelInfo, availabilityData, passengerInfo, xFactor, leadPaxTypeId);
                if (pxd?.Count > 0)
                {
                    passengerDetails.AddRange(pxd);
                }

                var passengerDetail = new Isango.Entities.PassengerDetail
                {
                    FirstName = leadPassenger?.FirstName,
                    LastName = leadPassenger?.LastName,
                    IsLeadPassenger = true,
                    //AgeGroupId = leadPassengerInfo?.AgeGroupId ?? 0,
                    PassengerTypeId = leadPassengerInfo?.PassengerTypeId ?? 1,
                    IndependablePax = leadPassengerInfo?.IndependablePax ?? false,

                    PaxPrice = new PerPaxPrice
                    {
                        BasePrice = GetPerPaxPrice(basePricingUnits, leadPassengerInfo.PassengerTypeId, leadPaxTypeId),
                        CostPrice = GetPerPaxPrice(costPricingUnits, leadPassengerInfo.PassengerTypeId, leadPaxTypeId),
                        GatePrice = GetPerPaxPrice(gatePricingUnits, leadPassengerInfo.PassengerTypeId, leadPaxTypeId),
                        SellPrice = GetPerPaxPrice(basePricingUnits, leadPassengerInfo.PassengerTypeId, leadPaxTypeId) *
                                    xFactor
                    }
                };
                passengerDetails.Add(passengerDetail);
            }
            else
            {
                if (availabilityData?.BundleOptionID > 0) // condition applied for bundles where products have different child passengers based on ages.....issue - 
                {
                    var checkForDefaultFunctionality = false;
                    foreach (var paxType in travelInfo.NoOfPassengers)
                    {
                        try
                        {
                            for (var i = 0; i < paxType.Value; i++)
                            {
                                try
                                {
                                    var productPassengerDetail = selectedProduct.PassengerDetails?.FirstOrDefault(x => ((PassengerType)x?.PassengerTypeId) == paxType.Key && !passengerDetails.Any(y => y.FirstName == x.FirstName && y.LastName == x.LastName));
                                    if (productPassengerDetail != null)
                                    {
                                        var productPassengerInfo =
                                        // passengerInfo?.FirstOrDefault(x => x.AgeGroupId == productPassengerDetail.AgeGroupId) ??
                                        passengerInfo?.FirstOrDefault(x => x.PassengerTypeId == productPassengerDetail.PassengerTypeId);

                                        var passengerType = (PassengerType)productPassengerInfo?.PassengerTypeId;

                                        var passengerDetail = new Isango.Entities.PassengerDetail
                                        {
                                            FirstName = productPassengerDetail.FirstName,
                                            LastName = productPassengerDetail.LastName,
                                            IsLeadPassenger = productPassengerDetail.IsLeadPassenger,
                                            //AgeGroupId = productPassengerDetail.AgeGroupId,
                                            PassengerTypeId = productPassengerInfo.PassengerTypeId,
                                            IndependablePax = productPassengerInfo?.IndependablePax ?? false,
                                            PaxPrice = new PerPaxPrice
                                            {
                                                BasePrice = GetPerPaxPrice(basePricingUnits, productPassengerDetail.AgeGroupId,
                                                    passengerType),
                                                CostPrice = GetPerPaxPrice(costPricingUnits, productPassengerDetail.AgeGroupId,
                                                    passengerType),
                                                GatePrice = GetPerPaxPrice(gatePricingUnits, productPassengerDetail.AgeGroupId,
                                                    passengerType),
                                                SellPrice = GetPerPaxPrice(basePricingUnits, productPassengerDetail.AgeGroupId,
                                                                passengerType) * xFactor
                                            }
                                        };
                                        passengerDetails.Add(passengerDetail);
                                    }
                                    else
                                    {
                                        productPassengerDetail = selectedProduct.PassengerDetails?.FirstOrDefault(x => !passengerDetails.Any(y => y.FirstName == x.FirstName && y.LastName == x.LastName));
                                        if (productPassengerDetail != null)
                                        {
                                            var productPassengerInfo =
                                            // passengerInfo?.FirstOrDefault(x => x.AgeGroupId == productPassengerDetail.AgeGroupId) ??
                                            passengerInfo?.FirstOrDefault(x => x.PassengerTypeId == productPassengerDetail.PassengerTypeId);

                                            var passengerType = (PassengerType)productPassengerInfo?.PassengerTypeId;

                                            var passengerDetail = new Isango.Entities.PassengerDetail
                                            {
                                                FirstName = productPassengerDetail.FirstName,
                                                LastName = productPassengerDetail.LastName,
                                                IsLeadPassenger = productPassengerDetail.IsLeadPassenger,
                                                //AgeGroupId = productPassengerDetail.AgeGroupId,
                                                PassengerTypeId = productPassengerInfo.PassengerTypeId,
                                                IndependablePax = productPassengerInfo?.IndependablePax ?? false,
                                                PaxPrice = new PerPaxPrice
                                                {
                                                    BasePrice = GetPerPaxPrice(basePricingUnits, productPassengerDetail.AgeGroupId,
                                                        passengerType),
                                                    CostPrice = GetPerPaxPrice(costPricingUnits, productPassengerDetail.AgeGroupId,
                                                        passengerType),
                                                    GatePrice = GetPerPaxPrice(gatePricingUnits, productPassengerDetail.AgeGroupId,
                                                        passengerType),
                                                    SellPrice = GetPerPaxPrice(basePricingUnits, productPassengerDetail.AgeGroupId,
                                                                    passengerType) * xFactor
                                                }
                                            };
                                            passengerDetails.Add(passengerDetail);
                                        }
                                        else
                                        {
                                            checkForDefaultFunctionality = true;
                                            break;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    checkForDefaultFunctionality = true;
                                    break;
                                }
                            }

                            if (checkForDefaultFunctionality)
                            {
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            checkForDefaultFunctionality = true;
                            break;
                        }
                    }

                    if (checkForDefaultFunctionality)
                    {
                        passengerDetails = new List<Isango.Entities.PassengerDetail>();
                        foreach (var productPassengerDetail in selectedProduct.PassengerDetails)
                        {
                            var productPassengerInfo =
                                // passengerInfo?.FirstOrDefault(x => x.AgeGroupId == productPassengerDetail.AgeGroupId) ??
                                passengerInfo?.FirstOrDefault(x => x.PassengerTypeId == productPassengerDetail.PassengerTypeId);

                            var passengerType = (PassengerType)productPassengerInfo?.PassengerTypeId;

                            var passengerDetail = new Isango.Entities.PassengerDetail
                            {
                                FirstName = productPassengerDetail.FirstName,
                                LastName = productPassengerDetail.LastName,
                                IsLeadPassenger = productPassengerDetail.IsLeadPassenger,
                                //AgeGroupId = productPassengerDetail.AgeGroupId,
                                PassengerTypeId = productPassengerInfo.PassengerTypeId,
                                IndependablePax = productPassengerInfo?.IndependablePax ?? false,
                                PaxPrice = new PerPaxPrice
                                {
                                    BasePrice = GetPerPaxPrice(basePricingUnits, productPassengerDetail.AgeGroupId,
                                        passengerType),
                                    CostPrice = GetPerPaxPrice(costPricingUnits, productPassengerDetail.AgeGroupId,
                                        passengerType),
                                    GatePrice = GetPerPaxPrice(gatePricingUnits, productPassengerDetail.AgeGroupId,
                                        passengerType),
                                    SellPrice = GetPerPaxPrice(basePricingUnits, productPassengerDetail.AgeGroupId,
                                                    passengerType) * xFactor
                                }
                            };
                            passengerDetails.Add(passengerDetail);
                        }
                    }
                }
                else
                {
                    foreach (var productPassengerDetail in selectedProduct.PassengerDetails)
                    {
                        var productPassengerInfo =
                            // passengerInfo?.FirstOrDefault(x => x.AgeGroupId == productPassengerDetail.AgeGroupId) ??
                            passengerInfo?.FirstOrDefault(x => x.PassengerTypeId == productPassengerDetail.PassengerTypeId);

                        var passengerType = (PassengerType)productPassengerInfo?.PassengerTypeId;

                        var passengerDetail = new Isango.Entities.PassengerDetail
                        {
                            FirstName = productPassengerDetail.FirstName,
                            LastName = productPassengerDetail.LastName,
                            IsLeadPassenger = productPassengerDetail.IsLeadPassenger,
                            //AgeGroupId = productPassengerDetail.AgeGroupId,
                            PassengerTypeId = productPassengerInfo.PassengerTypeId,
                            IndependablePax = productPassengerInfo?.IndependablePax ?? false,
                            PaxPrice = new PerPaxPrice
                            {
                                BasePrice = GetPerPaxPrice(basePricingUnits, productPassengerDetail.AgeGroupId,
                                    passengerType),
                                CostPrice = GetPerPaxPrice(costPricingUnits, productPassengerDetail.AgeGroupId,
                                    passengerType),
                                GatePrice = GetPerPaxPrice(gatePricingUnits, productPassengerDetail.AgeGroupId,
                                    passengerType),
                                SellPrice = GetPerPaxPrice(basePricingUnits, productPassengerDetail.AgeGroupId,
                                                passengerType) * xFactor
                            }
                        };
                        passengerDetails.Add(passengerDetail);
                    }
                }
            }

            return passengerDetails;
        }

        private List<PerPersonPricingUnit> GetPricingUnits(string pricingUnits)
        {
            return string.IsNullOrWhiteSpace(pricingUnits)
                ? new List<PerPersonPricingUnit>()
                : SerializeDeSerializeHelper.DeSerialize<List<PerPersonPricingUnit>>(pricingUnits);
        }

        private TravelInfo GetTravelInfo(string travelInfo)
        {
            return string.IsNullOrWhiteSpace(travelInfo) ? new TravelInfo() : SerializeDeSerializeHelper.DeSerialize<TravelInfo>(travelInfo);
        }

        private decimal GetPerPaxPrice(List<PerPersonPricingUnit> pricingUnits, int ageGroupId, PassengerType key = PassengerType.Undefined)
        {
            var pricingUnit = pricingUnits?.FirstOrDefault(x => x.PassengerType == key);

            return pricingUnit?.Price ?? 0;
        }

        private List<Sale> GetAppliedSales(string referenceId, string priceOfferReferenceId)
        {
            var appliedSales = new List<Sale>();
            if (string.IsNullOrWhiteSpace(referenceId) || string.IsNullOrWhiteSpace(priceOfferReferenceId))
                return appliedSales;

            var priceOffers = _tableStorageOperation.RetrievePriceOfferData(referenceId, priceOfferReferenceId);

            foreach (var priceOffer in priceOffers)
            {
                var appliedSale = new Sale
                {
                    AppliedRuleId = priceOffer.Id,
                    SaleAmount = priceOffer.SaleAmount,
                    SaleType = priceOffer.ModuleName,
                    CostAmount = priceOffer.CostAmount
                };
                appliedSales.Add(appliedSale);
            }

            return appliedSales;
        }

        private List<Isango.Entities.PassengerDetail> GetDummyPassengers(TravelInfo travelInfo,
            BaseAvailabilitiesEntity availabilityData, List<BookingPassengerInfo> passengerInformations,
            decimal xFactor, PassengerType leadPassengerType)
        {
            var passengerDetails = new List<Isango.Entities.PassengerDetail>();
            foreach (var key in travelInfo.NoOfPassengers.Keys)
            {
                var noOfPassengers = key == leadPassengerType
                    ? travelInfo.NoOfPassengers[key] - 1
                    : travelInfo.NoOfPassengers[key];

                var dummyPassengers = GetDummyPassengersByPassengerType(noOfPassengers,
                    passengerInformations, xFactor, availabilityData, key);
                if (dummyPassengers?.Count > 0)
                {
                    passengerDetails.AddRange(dummyPassengers);
                }
            }

            return passengerDetails;
        }

        private List<Isango.Entities.PassengerDetail> GetDummyPassengersByPassengerType(int noOfPassengers,
            List<BookingPassengerInfo> passengerInformations, decimal xFactor,
            BaseAvailabilitiesEntity availabilityData, PassengerType key)
        {
            var basePricingUnits = GetPricingUnits(availabilityData.BasePricingUnits);
            var costPricingUnits = GetPricingUnits(availabilityData.CostPricingUnits);
            var gatePricingUnits = GetPricingUnits(availabilityData.GateBasePricingUnits);

            var passengerDetails = new List<Isango.Entities.PassengerDetail>();
            for (int i = 0; i < noOfPassengers; i++)
            {
                var passengerInfo =
                    passengerInformations.FirstOrDefault(x => ((PassengerType)x.PassengerTypeId == key));
                //var ageGroupId = passengerInfo?.AgeGroupId ?? 0;
                var passengerDetail = new Isango.Entities.PassengerDetail
                {
                    FirstName = $"{key}FirstName{i + 1}",
                    LastName = $"{key}LastName{i + 1}",
                    IsLeadPassenger = false,
                    //AgeGroupId = ageGroupId,
                    PassengerTypeId = passengerInfo?.PassengerTypeId ?? (int)key,
                    IndependablePax = passengerInfo?.IndependablePax ?? false,
                    PaxPrice = new PerPaxPrice
                    {
                        BasePrice = GetPerPaxPrice(basePricingUnits, 0, key),
                        CostPrice = GetPerPaxPrice(costPricingUnits, 0, key),
                        GatePrice = GetPerPaxPrice(gatePricingUnits, 0, key),
                        SellPrice = GetPerPaxPrice(basePricingUnits, 0, key) * xFactor
                    }
                };
                passengerDetails.Add(passengerDetail);
            }

            return passengerDetails;
        }

        private CustomerLocation GetCustomerLocation(CreateBookingRequest request)
        {
            return new CustomerLocation
            {
                IPAddress = request?.IPAddress,
                CustomerOriginDestination = request?.OriginCity,
                CustomerOriginCountry = request?.OriginCountry
            };
        }

        private Isango.Entities.CustomerAddress GetRegisteredCustomerDetails(CreateBookingRequest request)
        {
            return new Isango.Entities.CustomerAddress
            {
                CustomerPhoneNumber = request.UserPhoneNumber,
                IsGuestUser = request.IsGuestUser,
                Address = request.CustomerAddress?.Address,
                City = request.CustomerAddress?.Town,
                Country = request.CustomerAddress?.CountryName,
                CountryIsoCode = request.CustomerAddress?.CountryIsoCode,
                ZipCode = request.CustomerAddress?.PostCode,
                State = request.CustomerAddress?.StateOrProvince
            };
        }

        private Isango.Entities.CustomerAddress GetRegisteredReceiveCustomerDetails(CreateReceiveBookingRequest request)
        {
            return new Isango.Entities.CustomerAddress
            {
                CustomerPhoneNumber = request.UserPhoneNumber,
                //IsGuestUser = request.IsGuestUser,
                Address = request.CustomerAddress?.Address,
                City = request.CustomerAddress?.Town,
                Country = request.CustomerAddress?.CountryName,
                CountryIsoCode = request.CustomerAddress?.CountryIsoCode,
                ZipCode = request.CustomerAddress?.PostCode,
                State = request.CustomerAddress?.StateOrProvince
            };
        }

        #endregion Private Methods

        /// <summary>
        /// if ip address length is > 15 than it will pick last 4 Octet from ip. Example "38.4.32.0.21.0.85.222.49.152.203.135.123.74.68.250" will be converted to "123.74.68.250"
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <returns></returns>
        public string ValidateAndGetIp(string ipaddress)
        {
            var result = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(ipaddress))
                {
                    var lengthCharacters = ipaddress.Length;
                    if (lengthCharacters > 15 && ipaddress.Contains('.'))
                    {
                        var splitedOctet = ipaddress.Split('.');
                        for (int i = splitedOctet.Length - 4; i < splitedOctet.Length; i++)
                        {
                            result += splitedOctet[i] + ".";
                        }

                        result = result.Substring(0, result.Length - 1);
                    }

                    var ipAddressPattern =
                        "^([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.([01] ?\\d\\d ?| 2[0 - 4]\\d | 25[0 - 5])\\.([01] ?\\d\\d ?| 2[0 - 4]\\d | 25[0 - 5])$";

                    var regexMatch = Regex.Match(result, ipAddressPattern);
                    if (regexMatch.Success)
                    {
                        //Valid ip address pattern
                    }
                    else
                    {
                        result = "13.107.246.10";
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingMapper",
                    MethodName = "ValidateAndGetIp",
                };
                _log.Error(isangoErrorEntity, ex);
                result = ipaddress;
            }

            return result;
        }

        /// <summary>
        /// This method prepare the booking object used by the booking service methods
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Booking PrepareReceiveBookingModel(CreateReceiveBookingRequest request)
        {
            try
            {
                var user = new ISangoUser
                {
                    UserId = request.UserId,
                    EmailAddress = request.UserEmail,
                    FirstName = request.PaymentDetail?.UserFullName,
                    PhoneNumber = request.UserPhoneNumber,
                    Address1 = request.CustomerAddress.Address,
                    City = request.CustomerAddress.Town,
                    State = request.CustomerAddress?.StateOrProvince,
                    Country = request.CustomerAddress.CountryIsoCode.ToUpperInvariant(),
                    CountryCode = request.CustomerAddress.CountryIsoCode.ToUpperInvariant(),
                    ZipCode = request.CustomerAddress.PostCode,
                };

                var affiliate = _affiliateService.GetAffiliateInformationAsync(request.AffiliateId).GetAwaiter().GetResult();
                if (affiliate == null) return null;

                Enum.TryParse(request.PaymentDetail.PaymentGateway, true, out PaymentGatewayType gatewayType);
                Enum.TryParse(request.PaymentDetail.PaymentOption, true, out PaymentOptionType paymentOptionType);
                Enum.TryParse(request.PaymentDetail.PaymentMethodType, true, out PaymentMethodType paymentMethodType);

                if (paymentMethodType == PaymentMethodType.Undefined)
                    paymentMethodType = affiliate.PaymentMethodType;

                var mapIsangoBookingData = MapIsangoReceiveBookingData(request);
                var booking = new Booking
                {
                    ReferenceNumber = request.BookingReferenceNumber,
                    VoucherEmailAddress = user.EmailAddress,
                    VoucherPhoneNumber = user.PhoneNumber,
                    Currency = new Currency { IsoCode = request.CurrencyIsoCode },
                    Date = DateTime.Today,
                    Amount = Convert.ToDecimal((request.ChargeAmount * 100).ToString("0")) / 100,
                    PaymentOption = paymentOptionType,
                    PaymentGateway = gatewayType,
                    PaymentMethodType = paymentMethodType,
                    BookingUserAgent = new BookingUserAgent(),
                    IpAddress = request.IPAddress,
                    User = user,
                    IsangoBookingData = mapIsangoBookingData,
                    BookingId = request.AmendmentId,
                    Affiliate = affiliate,
                    ActualIP = request.ActualIP,
                    OriginCountry = request.OriginCountry,
                    OriginCity = request.OriginCity,
                    PostalCode = request.CustomerAddress.PostCode,
                    Town = request.CustomerAddress.Town,
                    Country = request.CustomerAddress.CountryName,
                    Street = request.CustomerAddress.Address,
                    State = request.CustomerAddress.StateOrProvince,
                    BrowserInfo = new Isango.Entities.Booking.BrowserInfo()
                    {
                        AcceptHeader = request.BrowserInfo?.AcceptHeader,
                        ScreenHeight = request.BrowserInfo?.ScreenHeight,
                        ScreenWidth = request.BrowserInfo?.ScreenWidth,
                        ColorDepth = request.BrowserInfo?.ColorDepth,
                        JavaEnabled = request.BrowserInfo?.JavaEnabled ?? false,
                        Language = request.BrowserInfo?.Language,
                        TimeZoneOffset = request.BrowserInfo?.TimeZoneOffset,
                        UserAgent = request.BrowserInfo?.UserAgent
                    },
                    AdyenMerchantAccountCancelRefund = request.AdyenMerchantAccout
                };

                if (booking.Amount > 0
                    && (booking.PaymentGateway == PaymentGatewayType.WireCard
                        || booking.PaymentGateway == PaymentGatewayType.Apexx || booking.PaymentGateway == PaymentGatewayType.Adyen)
                    && booking.PaymentMethodType == PaymentMethodType.Transaction
                )
                {
                    if (request.PaymentDetail.CardDetails != null)
                    {
                        booking.Payment = new List<Payment>
                        {
                            new Payment {PaymentType = FillCreditCard(user, request.PaymentDetail)}
                        };
                    }
                    else
                    {
                        return null;
                    }
                }

                booking.IsangoBookingData.PaymentMethodType = ((int)booking.PaymentMethodType).ToString();
                return booking;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingMapper",
                    MethodName = "PrepareReceiveBookingModel",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private IsangoBookingData MapIsangoReceiveBookingData(CreateReceiveBookingRequest request)
        {
            try
            {
                var registeredCustomerDetail = GetRegisteredReceiveCustomerDetails(request);
                var isangoBookingData = new IsangoBookingData
                {
                    CurrencyIsoCode = request.CurrencyIsoCode,
                    CustomerEmailId = request.UserEmail,
                    PaymentMethodType = request.PaymentDetail.PaymentMethodType,
                    RegisteredCustomerDetail = registeredCustomerDetail,
                };

                return isangoBookingData;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingMapper",
                    MethodName = "MapIsangoReceiveBookingData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// PrepareRezdyQuestions
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <returns></returns>
        private List<BookingQuestions> PrepareRezdyQuestions(RequestSelectedProduct selectedProduct)
        {
            var questions = new List<BookingQuestions>();

            if (selectedProduct?.Questions?.Count > 0)
            {
                foreach (var question in selectedProduct?.Questions)
                {
                    var rezdyQuestion = new BookingQuestions
                    {
                        Answers = new List<string> { question?.Answer, },
                        Question = question?.Label,
                        Required = question.IsRequired
                    };
                    questions.Add(rezdyQuestion);
                }
            }

            return questions;
        }

        /// <summary>
        /// Get GLI Pax AgeGroup Ids Async
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="apiType"></param>
        /// <returns></returns>
        private Dictionary<PassengerType, int> GetGLIPaxAgeGroupIdsAsync(int activityId, APIType apiType)
        {
            var paxAgeGroupIds = new Dictionary<PassengerType, int>();
            var ageGroups = _masterService.GetGLIAgeGroupAsync(activityId, apiType)?.GetAwaiter().GetResult();

            var adultAgeGroup = ageGroups?.FirstOrDefault(x => x.PassengerType == PassengerType.Adult);

            if (adultAgeGroup != null)
            {
                paxAgeGroupIds.Add(PassengerType.Adult, adultAgeGroup.AgeGroupId);
            }

            var childAgeGroup = ageGroups?.Where(x => x.PassengerType == PassengerType.Child).FirstOrDefault();
            if (childAgeGroup != null)
            {
                paxAgeGroupIds.Add(PassengerType.Child, childAgeGroup.AgeGroupId);
            }

            var youthAgeGroup = ageGroups?.Where(x => x.PassengerType == PassengerType.Youth).FirstOrDefault();
            if (youthAgeGroup != null)
            {
                paxAgeGroupIds.Add(PassengerType.Youth, youthAgeGroup.AgeGroupId);
            }

            return paxAgeGroupIds;
        }
    }
}