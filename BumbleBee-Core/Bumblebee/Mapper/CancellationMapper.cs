using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.BigBus;
using IsangoTiqetsEntities = Isango.Entities.Tiqets;
using Isango.Entities.Booking;
using Isango.Entities.Cancellation;
using Isango.Entities.CitySightseeing;
using Isango.Entities.Enums;
using Isango.Entities.FareHarbor;
using Isango.Entities.HotelBeds;
using Isango.Entities.Prio;
using Isango.Entities.Ventrata;
using Isango.Entities.Rezdy;
using ServiceAdapters.BigBus.BigBus.Entities;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using ServiceAdapters.Ventrata.Ventrata.Entities;
using TiqetsEntities = ServiceAdapters.Tiqets.Tiqets.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TableStorageOperations.Contracts;
using WebAPI.Models.RequestModels;
using WebAPI.Models.ResponseModels;
using CancellationParameters = Isango.Entities.Cancellation.CancellationParameters;
using CancellationPolicyDetail = Isango.Entities.Cancellation.CancellationPolicyDetail;
using CancellationStatus = WebAPI.Models.ResponseModels.CancellationStatus;
using SelectedProduct = Isango.Entities.SelectedProduct;
using Isango.Entities.TourCMS;
using Isango.Entities.NewCitySightSeeing;
using Isango.Entities.GoCity;
using Isango.Entities.PrioHub;
using Isango.Entities.Rayna;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using Isango.Entities.Canocalization;

namespace WebAPI.Mapper
{
    public class CancellationMapper
    {
        private ITableStorageOperation _apiStorageOperation;

        public CancellationMapper(ITableStorageOperation aPIStorageOperation)
        {
            _apiStorageOperation = aPIStorageOperation;
        }

        /// <summary>
        /// Map cancellation policy details
        /// </summary>
        /// <param name="cancellationPolicyData"></param>
        /// <param name="currencyIsoCode"></param>
        /// <returns></returns>
        public CancellationPolicyDetailResponse MapCancellationPolicyDetail(
            CancellationPolicyDetail cancellationPolicyData, string currencyIsoCode)
        {
            if (cancellationPolicyData?.UserCurrencyCode == null) return null;
            var cancellationPolicyDetailResponse =
                new CancellationPolicyDetailResponse
                {
                    UserCurrencyCode = currencyIsoCode ?? cancellationPolicyData.UserCurrencyCode,
                    UserRefundAmount = cancellationPolicyData.UserRefundAmount,
                    TotalAmount = cancellationPolicyData.UserRefundAmount +
                                  cancellationPolicyData.UserCancellationCharges,
                    SellingPrice = cancellationPolicyData.SellingPrice,
                    CancellationDescription = cancellationPolicyData.CancellationChargeDescription
                };

            return cancellationPolicyDetailResponse;
        }

        /// <summary>
        /// Map supplier data to create booked product for supplier booking cancellation
        /// </summary>
        /// <param name="supplierCancellationData"></param>
        /// <param name="bookingData"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public Booking MapSupplierDataForBookedProduct(SupplierCancellationData supplierCancellationData,
            ConfirmBookingDetail bookingData, Cancellation cancellation)
        {
            var availabilityReferenceId = bookingData?.BookedOptions
                ?.FirstOrDefault(x => x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                ?.AvailabilityReferenceId;
            var languageCode = bookingData?.LanguageCode;
            var booking = new Booking { Language = new Language { Code = languageCode } };
            var apiType = (APIType)supplierCancellationData.ApiType;

            switch (apiType)
            {
                case APIType.Citysightseeing:
                    var selectedProduct = new CitySightseeingSelectedProduct
                    {
                        APIType = apiType,
                        AvailabilityReferenceId = availabilityReferenceId,
                        Pnr = supplierCancellationData.SupplierBookingReferenceNumber,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                }
                            }
                        }
                    };
                    booking.SelectedProducts = new List<SelectedProduct> { selectedProduct };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    var bookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber }
                    };
                    booking.IsangoBookingData.BookedProducts.Add(bookedProduct);
                    break;

                case APIType.Tiqets:
                    var tiqetsProduct = new IsangoTiqetsEntities.TiqetsSelectedProduct
                    {
                        APIType = apiType,
                        AvailabilityReferenceId = availabilityReferenceId,
                        OrderReferenceId = supplierCancellationData.SupplierBookingReferenceNumber,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                }
                            }
                        }
                    };
                    booking.SelectedProducts = new List<SelectedProduct> { tiqetsProduct };
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    var bookedProductTiqets = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber }
                    };
                    booking.Affiliate = new Isango.Entities.Affiliate.Affiliate();
                    booking.Affiliate.Id = bookingData?.AffiliateId;
                    booking.IsangoBookingData.BookedProducts.Add(bookedProductTiqets);
                    break;

                case APIType.Aot:
                    var aotBookedProduct = new BookedProduct
                    {
                        AvailabilityReferenceId = availabilityReferenceId,
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        APIExtraDetail = new ApiExtraDetail
                        {
                            SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber,
                            SupplierLineNumber = supplierCancellationData.SupplierBookingLineNumber
                        },
                        CountryCode = supplierCancellationData.CountryName
                    };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    booking.IsangoBookingData.BookedProducts.Add(aotBookedProduct);
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;
                    var aotSelectedProduct = new SelectedProduct
                    {
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIType = apiType,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                }
                            }
                        }
                    };
                    booking.SelectedProducts = new List<SelectedProduct> { aotSelectedProduct };
                    break;

                case APIType.BigBus:
                    var bigBusProduct = new BigBusSelectedProduct
                    {
                        AvailabilityReferenceId = availabilityReferenceId,
                        BookingStatus = BigBusApiStatus.Booked,
                        BookingReference = supplierCancellationData.SupplierBookingReferenceNumber,
                        APIType = apiType,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                }
                            }
                        }
                    };
                    booking.SelectedProducts = new List<SelectedProduct> { bigBusProduct };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    var bigBusBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber }
                    };
                    booking.IsangoBookingData.BookedProducts.Add(bigBusBookedProduct);
                    break;

                case APIType.Bokun:
                    var bokunBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber }
                    };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    booking.IsangoBookingData.BookedProducts.Add(bokunBookedProduct);
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;
                    var bokunSelectedProduct = new SelectedProduct
                    {
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIType = apiType,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                }
                            }
                        }
                    };
                    booking.SelectedProducts = new List<SelectedProduct> { bokunSelectedProduct };
                    break;

                case APIType.Fareharbor:

                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;
                    var fareHarborProducts = new FareHarborSelectedProduct
                    {
                        UuId = supplierCancellationData.SupplierBookingReferenceNumber,
                        Code = supplierCancellationData.FHBSupplierShortName.Trim(),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(y =>
                                            y.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                }
                            }
                        },
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIType = apiType,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(y => y.ServiceId).FirstOrDefault())
                    };
                    var fareHarborBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber }
                    };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    booking.IsangoBookingData.BookedProducts.Add(fareHarborBookedProduct);
                    booking.SelectedProducts = new List<SelectedProduct> { fareHarborProducts };
                    break;

                case APIType.Graylineiceland:
                    var grayLineIceLandBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber }
                    };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    booking.IsangoBookingData.BookedProducts.Add(grayLineIceLandBookedProduct);
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;
                    var grayLineIceLandSelectedProduct = new SelectedProduct
                    {
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIType = apiType,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                }
                            }
                        }
                    };
                    booking.SelectedProducts = new List<SelectedProduct> { grayLineIceLandSelectedProduct };
                    break;

                case APIType.Hotelbeds:
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;

                    var hotelBeds = new HotelBedsSelectedProduct
                    {
                        FileNumber = supplierCancellationData.SupplierBookingReferenceNumber,

                        Language = languageCode,
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIType = apiType,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                },
                                Contract = new Isango.Entities.Contract
                                    {InComingOfficeCode = supplierCancellationData.OfficeCode},
                                IsSelected = true
                            }
                        }
                    };
                    if (booking.Language.Code != null)
                    {
                        booking.SelectedProducts = new List<SelectedProduct> { hotelBeds };
                    }

                    var hotelBedBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber },
                    };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    booking.IsangoBookingData.BookedProducts.Add(hotelBedBookedProduct);
                    break;

                case APIType.Prio:
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;

                    var prioSelectedProduct = new PrioSelectedProduct
                    {
                        PrioApiConfirmedBooking = new PrioApi
                        {
                            ErrorCode = supplierCancellationData.OfficeCode,
                            BookingStatus = PrioApiStatus.Confirmed,
                            DistributorReference = supplierCancellationData.BookingReferenceNumber,
                            BookingReference = supplierCancellationData.SupplierBookingReferenceNumber
                        },
                        PrioTicketClass = supplierCancellationData.BookedOptionId,
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIType = apiType,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                }
                            }
                        }
                    };
                    booking.SelectedProducts = new List<SelectedProduct> { prioSelectedProduct };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    var prioBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber }
                    };
                    booking.IsangoBookingData.BookedProducts.Add(prioBookedProduct);
                    break;

                case APIType.Rezdy:
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;
                    var rezdy = new RezdySelectedProduct
                    {
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIType = apiType,
                        OrderNumber = supplierCancellationData.SupplierBookingReferenceNumber,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                },
                                IsSelected = true
                            }
                        }
                    };
                    if (booking.Language.Code != null)
                    {
                        booking.SelectedProducts = new List<SelectedProduct> { rezdy };
                    }

                    var rezdyBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber },
                    };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    booking.IsangoBookingData.BookedProducts.Add(rezdyBookedProduct);
                    break;

                case APIType.Ventrata:
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;
                    var ventrataProduct = new VentrataSelectedProduct
                    {
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIType = apiType,
                        OrderNumber = supplierCancellationData.SupplierBookingReferenceNumber,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                },
                                IsSelected = true
                            }
                        },
                        ApiBookingDetails = new VentrataApiBookingDetails() { Status = VentrataBookingStatus.Confirmed },
                        BookingStatus = VentrataBookingStatus.OnHold,
                        Uuid = supplierCancellationData.SupplierBookingLineNumber,
                        ReasonForCancellation = cancellation.CancellationParameters.Reason,
                        IsCancellable = true,
                        ActivityCode = supplierCancellationData.FHBSupplierShortName
                    };
                    if (booking.Language.Code != null)
                    {
                        booking.SelectedProducts = new List<SelectedProduct> { ventrataProduct };
                    }

                    var ventrataBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber },
                    };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    booking.IsangoBookingData.BookedProducts.Add(ventrataBookedProduct);
                    break;

                case APIType.TourCMS:
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;
                    var tourCMS = new TourCMSSelectedProduct
                    {
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIType = apiType,
                        BookingId = Convert.ToInt32(supplierCancellationData.SupplierBookingReferenceNumber),
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                },
                                IsSelected = true,
                                PrefixServiceCode=bookingData?.BookedOptions?.Where(x=>x.BookedOptionId==supplierCancellationData.BookedOptionId)?.Select(x => x.SupplierCode)?.FirstOrDefault()
                            }
                        },

                    };
                    if (booking.Language.Code != null)
                    {
                        booking.SelectedProducts = new List<SelectedProduct> { tourCMS };
                    }

                    var tourCMSBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber },
                    };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    booking.IsangoBookingData.BookedProducts.Add(tourCMSBookedProduct);
                    break;
                case APIType.NewCitySightSeeing:
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;
                    var newCitySightSeeingData = new NewCitySightSeeingSelectedProduct
                    {
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIType = apiType,
                        NewCitySightSeeingReservationId = supplierCancellationData.SupplierBookingReferenceNumber,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                },
                                IsSelected = true

                            }
                        },

                    };
                    if (booking.Language.Code != null)
                    {
                        booking.SelectedProducts = new List<SelectedProduct> { newCitySightSeeingData };
                    }

                    var citySightSeeingBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber },
                    };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    booking.IsangoBookingData.BookedProducts.Add(citySightSeeingBookedProduct);
                    break;
                case APIType.GoCity:
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;
                    var GoCityselectedProduct = new GoCitySelectedProduct
                    {
                        APIType = apiType,
                        AvailabilityReferenceId = availabilityReferenceId,
                        OrderNumber = supplierCancellationData.SupplierBookingReferenceNumber,
                        CustomerEmail = bookingData?.VoucherEmail,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                }
                            }
                        }
                    };
                    booking.SelectedProducts = new List<SelectedProduct> { GoCityselectedProduct };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    var goCityBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber }
                    };
                    booking.IsangoBookingData.BookedProducts.Add(goCityBookedProduct);
                    break;

                case APIType.PrioHub:
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;

                    var prioHubSelectedProduct = new PrioHubSelectedProduct
                    {
                        PrioHubDistributerId = supplierCancellationData.OfficeCode,
                        PrioHubApiConfirmedBooking = new PrioHubAPITicket
                        {
                            ErrorCode = string.Empty,
                            BookingStatus = "BOOKING_CONFIRMED",
                            DistributorReference = supplierCancellationData.BookingReferenceNumber,
                            BookingReference = supplierCancellationData.SupplierBookingReferenceNumber,
                            DistributorId = supplierCancellationData.OfficeCode
                        },
                        PrioTicketClass = supplierCancellationData.BookedOptionId,
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIType = apiType,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        PrioHubReservationStatus = "BOOKING_RESERVED",
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                }
                            }
                        }
                    };
                    booking.SelectedProducts = new List<SelectedProduct> { prioHubSelectedProduct };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    var prioHubBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber }
                    };
                    booking.VoucherEmailAddress = bookingData?.VoucherEmail;
                    booking.IsangoBookingData.BookedProducts.Add(prioHubBookedProduct);
                    break;
                case APIType.Rayna:
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;
                    var raynaselectedProduct = new RaynaSelectedProduct
                    {
                        APIType = apiType,
                        AvailabilityReferenceId = availabilityReferenceId,
                        OrderReferenceId = supplierCancellationData.SupplierBookingReferenceNumber,
                        BookingId = supplierCancellationData?.SupplierBookingLineNumber,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                }
                            }
                        }
                    };
                    booking.SelectedProducts = new List<SelectedProduct> { raynaselectedProduct };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    var raynaBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber }
                    };
                    booking.IsangoBookingData.BookedProducts.Add(raynaBookedProduct);
                    break;
                case APIType.GlobalTixV3:
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;
                    var globalTixselectedProduct = new CanocalizationSelectedProduct
                    {
                        APIType = apiType,
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIDetails = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber },
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                }
                            }
                        }
                    };
                    booking.SelectedProducts = new List<SelectedProduct> { globalTixselectedProduct };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    var globalTixBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber }
                    };
                    booking.IsangoBookingData.BookedProducts.Add(globalTixBookedProduct);
                    break;
                case APIType.RedeamV12:
                    booking.ReferenceNumber = supplierCancellationData.BookingReferenceNumber;
                    var redeamV12Product = new CanocalizationSelectedProduct
                    {
                        BookingReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber,
                        APIType = apiType,
                        AvailabilityReferenceId = availabilityReferenceId,
                        //OrderReferenceId = supplierCancellationData.SupplierBookingReferenceNumber,
                        //BookingId = supplierCancellationData?.SupplierBookingLineNumber,
                        Id = Convert.ToInt32(bookingData?.BookedOptions?.Select(x => x.ServiceId).FirstOrDefault()),
                        ProductOptions = new List<ProductOption>
                        {
                            new ActivityOption
                            {
                                TravelInfo = new TravelInfo
                                {
                                    StartDate = Convert.ToDateTime(bookingData?.BookedOptions
                                        ?.FirstOrDefault(x =>
                                            x.BookedOptionId == cancellation.CancellationParameters.BookedOptionId)
                                        ?.TravelDate)
                                }
                            }
                        }
                    };
                    booking.SelectedProducts = new List<SelectedProduct> { redeamV12Product };
                    booking.IsangoBookingData = new IsangoBookingData { BookedProducts = new List<BookedProduct>() };
                    var redeamBookedProduct = new BookedProduct
                    {
                        OptionStatus = supplierCancellationData.BookedOptionStatusId.ToString(),
                        AvailabilityReferenceId = availabilityReferenceId,
                        APIExtraDetail = new ApiExtraDetail
                        { SupplieReferenceNumber = supplierCancellationData.SupplierBookingReferenceNumber }
                    };
                    booking.IsangoBookingData.BookedProducts.Add(redeamBookedProduct);
                    break;

            }
            return booking;
        }

        /// <summary>
        /// Map cancellation request to cancel booking in db
        /// </summary>
        /// <param name="cancellationRequest"></param>
        /// <param name="userData"></param>
        /// <param name="booking"></param>
        /// <returns></returns>
        public Cancellation MapCancellationRequest(CancellationRequest cancellationRequest,
            UserCancellationPermission userData, ConfirmBookingDetail booking)
        {
            var currencyCode = booking?.CurrencyIsoCode;
            var trackerStatusId = booking?.BookedOptions
                .FirstOrDefault(x => x.BookedOptionId == cancellationRequest.CancellationParameters.BookedOptionId)
                ?.BookedOptionStatusId;

            var cancellation = new Cancellation
            {
                BookingRefNo = cancellationRequest.BookingRefNo,
                TokenId = cancellationRequest.TokenId,
                CancelledByUser = 0,
                TrackerStatusId = trackerStatusId,
                CancelledByUserId = userData?.UserId,
                CancellationParameters = new CancellationParameters
                {
                    BookedOptionId = cancellationRequest.CancellationParameters.BookedOptionId,
                    AlternativeDates = cancellationRequest.CancellationParameters.AlternativeDates,
                    AlternativeTours = cancellationRequest.CancellationParameters.AlternativeTours,
                    CurrencyCode = currencyCode,
                    CustomerNotes = cancellationRequest.CancellationParameters.CustomerNotes,
                    Reason = cancellationRequest.CancellationParameters.Reason,
                    SupplierNotes = cancellationRequest.CancellationParameters.SupplierNotes,
                    //SupplierRefundAmount = cancellationRequest.CancellationParameters.SupplierRefundAmount,
                    UserRefundAmount = cancellationRequest.CancellationParameters.UserRefundAmount
                }
            };
            return cancellation;
        }

        /// <summary>
        /// Map cancel booking mail details for cancel booking mail
        /// </summary>
        /// <param name="bookingData"></param>
        /// <param name="supplierData"></param>
        /// <param name="optionAndServiceName"></param>
        /// <param name="tokenId"></param>
        /// <param name="bookedOption"></param>
        /// <returns></returns>
        public CancelBookingMailDetail MapCancelBookingMailDetail(ConfirmBookingDetail bookingData,
            SupplierCancellationData supplierData,
            Dictionary<string, string> optionAndServiceName, string tokenId,
            BookedOption bookedOption = null)
        {
            var cancelBookingMailDetails = new CancelBookingMailDetail
            {
                BookingReferenceNumber = bookingData.BookingReferenceNumber,
                TokenId = tokenId,
                ServiceId = bookedOption.ServiceId,
                TravelDate = bookedOption.TravelDate,
                CustomerEmailId = bookingData.VoucherEmail,
                ContactNumber = "N/A",
                ServiceName = optionAndServiceName["ServiceName"],
                OptionName = optionAndServiceName["OptionName"]
            };
            if (supplierData != null)
            {
                if (supplierData.ApiType == (Int32)APIType.PrioHub
                    && (supplierData.OfficeCode == "2425" || supplierData.OfficeCode == "1070569"))
                {
                    cancelBookingMailDetails.ApiTypeName = "PrioTicket";
                }
                else
                {
                    cancelBookingMailDetails.ApiTypeName = Convert.ToString((APIType)supplierData.ApiType);
                    cancelBookingMailDetails.APIBookingReferenceNumber = supplierData.SupplierBookingReferenceNumber;
                }
            }
            else
            {
                cancelBookingMailDetails.ApiTypeName = Convert.ToString(APIType.Undefined);
                cancelBookingMailDetails.APIBookingReferenceNumber = "N/A";
            }

            return cancelBookingMailDetails;
        }

        /// <summary>
        /// Map cancellation status from cancellation response
        /// </summary>
        /// <param name="cancellationResponse"></param>
        /// <param name="bookedOptionId"></param>
        /// <returns></returns>
        public Isango.Entities.Cancellation.CancellationStatus MapCancellationStatus(CancellationResponse cancellationResponse, int bookedOptionId)
        {
            Enum.TryParse(cancellationResponse.Status.AllCancelStatus.IsangoBookingCancel, true, out OptionCancelStatus isangoCancelStatus);
            Enum.TryParse(cancellationResponse.Status.AllCancelStatus.PaymentRefundStatus, true, out OptionCancelStatus paymentRefundStatus);
            Enum.TryParse(cancellationResponse.Status.AllCancelStatus.SupplierBookingCancel, true, out OptionCancelStatus supplierCancelStatus);

            var cancellationStatus = new Isango.Entities.Cancellation.CancellationStatus
            {
                BookedOptionId = bookedOptionId,
                IsangoCancelStatus = (int)isangoCancelStatus,
                PaymentRefundStatus = (int)paymentRefundStatus,
                SupplierCancelStatus = (int)supplierCancelStatus
            };
            return cancellationStatus;
        }

        /// <summary>
        /// Map cancellation status to cancellation response
        /// </summary>
        /// <param name="cancellationStatus"></param>
        /// <returns></returns>
        public CancellationResponse MapCancellationStatusResponse(Isango.Entities.Cancellation.CancellationStatus cancellationStatus)
        {
            var message = Constants.Constant.CancellationFailed;
            if (IsCancellationCompleted(cancellationStatus))
                message = Constants.Constant.CancellationSucceed;

            var cancellationResponse = new CancellationResponse
            {
                Status = new CancellationStatus
                {
                    AllCancelStatus = new AllCancelStatus
                    {
                        IsangoBookingCancel = ((OptionCancelStatus)cancellationStatus.IsangoCancelStatus).ToString(),
                        PaymentRefundStatus = ((OptionCancelStatus)cancellationStatus.PaymentRefundStatus).ToString(),
                        SupplierBookingCancel = ((OptionCancelStatus)cancellationStatus.SupplierCancelStatus).ToString()
                    },
                    Message = message
                }
            };
            return cancellationResponse;
        }

        /// <summary>
        /// Checking for product has already been cancelled
        /// </summary>
        /// <param name="cancellationStatus"></param>
        /// <returns></returns>
        public bool IsCancellationCompleted(Isango.Entities.Cancellation.CancellationStatus cancellationStatus)
        {
            return (cancellationStatus?.IsangoCancelStatus == (int)OptionCancelStatus.Success ||
                    cancellationStatus?.IsangoCancelStatus == (int)OptionCancelStatus.NotApplicable) &&
                   (cancellationStatus.PaymentRefundStatus == (int)OptionCancelStatus.Success ||
                    cancellationStatus.PaymentRefundStatus == (int)OptionCancelStatus.NotApplicable) &&
                   (cancellationStatus.SupplierCancelStatus == (int)OptionCancelStatus.Success ||
                    cancellationStatus.SupplierCancelStatus == (int)OptionCancelStatus.NotApplicable);
        }

        /// <summary>
        /// Check for isango db cancellation succeed
        /// </summary>
        /// <param name="cancellationStatus"></param>
        /// <returns></returns>
        public bool IsIsangoCancellationSucceed(Isango.Entities.Cancellation.CancellationStatus cancellationStatus)
        {
            return cancellationStatus.IsangoCancelStatus == (int)OptionCancelStatus.Success ||
                   cancellationStatus.IsangoCancelStatus == (int)OptionCancelStatus.NotApplicable;
        }

        /// <summary>
        /// Map cancellation status and payment refund amount in cancel booking mail
        /// </summary>
        /// <param name="cancellationRequest"></param>
        /// <param name="cancelBookingMailData"></param>
        /// <param name="cancellationResponse"></param>
        /// <param name="userData"></param>
        /// <param name="cancellationPolicyData"></param>
        /// <returns></returns>
        public CancelBookingMailDetail PrepareCancelBookingMailData(CancellationRequest cancellationRequest,
            CancelBookingMailDetail cancelBookingMailData, CancellationResponse cancellationResponse,
            UserCancellationPermission userData, CancellationPolicyDetail cancellationPolicyData)
        {
            cancelBookingMailData.IsangoBookingCancellationStatus =
                cancellationResponse.Status.AllCancelStatus.IsangoBookingCancel;
            cancelBookingMailData.PaymentRefundStatus = cancellationResponse.Status.AllCancelStatus.PaymentRefundStatus;
            cancelBookingMailData.APICancellationStatus =
                cancellationResponse.Status.AllCancelStatus.SupplierBookingCancel;
            cancelBookingMailData.PaymentRefundAmount = userData.IsPermitted == 1
                ? cancellationRequest.CancellationParameters?.UserRefundAmount.ToString(CultureInfo.InvariantCulture)
                : cancellationPolicyData?.UserRefundAmount.ToString(CultureInfo.InvariantCulture);
            return cancelBookingMailData;
        }

        /// <summary>
        /// Map cancellation response after cancellation steps
        /// </summary>
        /// <param name="cancellationResponse"></param>
        /// <param name="isangoDbCancelled"></param>
        /// <param name="supplierCancelStatus"></param>
        public CancellationResponse MapCancellationResponseAfterCancellationSteps(CancellationResponse cancellationResponse,
            Dictionary<string, string> isangoDbCancelled, OptionCancelStatus supplierCancelStatus)
        {
            //set cancellation status of three steps
            cancellationResponse.Status.AllCancelStatus.PaymentRefundStatus = isangoDbCancelled["isRefunded"];
            cancellationResponse.Status.AllCancelStatus.IsangoBookingCancel = isangoDbCancelled["isCancelled"];
            cancellationResponse.Status.AllCancelStatus.SupplierBookingCancel =
                supplierCancelStatus.ToString();
            return cancellationResponse;
        }
    }
}