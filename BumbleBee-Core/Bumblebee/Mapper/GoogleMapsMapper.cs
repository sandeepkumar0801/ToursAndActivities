//using System;
using System.Collections.Generic;
using System.Linq;
using Isango.Entities;
using Isango.Entities.Booking.RequestModels;
using Isango.Entities.GoogleMaps.BookingServer;
using Isango.Entities.GoogleMaps.BookingServer.Enums;
using Isango.Service.Contract;
using TableStorageOperations.Contracts;
using TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities;
using Util;
using WebAPI.Models.GoogleMapsBookingServer;
using BookingStatus = Isango.Entities.Enums.BookingStatus;
using CustomerAddress = Isango.Entities.Booking.RequestModels.CustomerAddress;
using PassengerDetail = Isango.Entities.Booking.RequestModels.PassengerDetail;
using SelectedProduct = Isango.Entities.Booking.RequestModels.SelectedProduct;

namespace WebAPI.Mapper
{
    public class GoogleMapsMapper
    {
        private readonly IGoogleMapsDataDumpingService _googleMapsDataDumpingService;
        private readonly ITableStorageOperation _TableStorageOperations;

        public GoogleMapsMapper(IGoogleMapsDataDumpingService googleMapsDataDumpingService, ITableStorageOperation TableStorageOperations)
        {
            _googleMapsDataDumpingService = googleMapsDataDumpingService;
            _TableStorageOperations = TableStorageOperations;
        }

        public CreateBookingRequest PrepareCreateBookingRequest(CreateOrderRequest createOrderRequest, CheckOrderFulfillabilityResponse checkOrderFulfillabilityResponse)
        {
            var tokenId = checkOrderFulfillabilityResponse.OrderFulfillability.TokenId;

            var order = createOrderRequest.Order;
            var userInformation = order.UserInformation;
            var lineItems = order.Items;

            var currencyCode = order.Items.FirstOrDefault()?.Price.CurrencyCode ?? string.Empty;
            var affiliateId = ConfigurationManagerHelper.GetValuefromAppSettings("GoogleMapsAffiliatedId");

            var createBookingRequest = new CreateBookingRequest
            {
                LanguageCode = "en", //Hardcoded currently
                CurrencyIsoCode = currencyCode,
                AffiliateId = affiliateId,
                UserEmail = order.UserInformation.Email,
                UserPhoneNumber = order.UserInformation.Telephone,
                TokenId = tokenId,
                IsGuestUser = false, //TODO: Need to check
                CustomerAddress = new CustomerAddress
                {
                    Address = userInformation.Address.StreetAddress,
                    Town = userInformation.Address.Locality,
                    PostCode = userInformation.Address.PostalCode,
                    CountryName = userInformation.Address.Country,
                    CountryIsoCode = "" //TODO: Need to check
                },
                PaymentDetail = new PaymentDetail
                {
                    UserFullName = $"{userInformation.GivenName} {userInformation.FamilyName}",
                    //Currently hardcoded, will change when GPAY payment will be implemented
                    PaymentGateway = "0",
                    PaymentMethodType = "Prepaid"
                },
                SelectedProducts = PrepareSelectedProducts(lineItems, userInformation, checkOrderFulfillabilityResponse)
            };
            return createBookingRequest;
        }

        public CreateOrderResponse PrepareCreateOrderResponse(CheckOrderFulfillabilityResponse checkOrderFulfillabilityResponse, BookingResult bookingResult, Order order)
        {
            CreateOrderResponse createOrderResponse = null;

            if (bookingResult.BookingStatus == BookingStatus.Confirmed)
            {
                order.OrderId = bookingResult.BookingRefNo;
                order.Items.ForEach(item =>
                {
                    item.Status = Isango.Entities.GoogleMaps.BookingServer.Enums.BookingStatus.CONFIRMED;
                });

                createOrderResponse = new CreateOrderResponse
                {
                    Order = order
                };
            }
            else
            {
                // Updating the status in the 'CheckOrderFulfillabilityResponse' if the booking fails
                checkOrderFulfillabilityResponse.OrderFulfillability.OrderFulfillabilityResult =
                    OrderFulfillabilityResult.UNFULFILLABLE_LINE_ITEM;
                checkOrderFulfillabilityResponse.OrderFulfillability.ItemFulfillability.ForEach(itemFulfillability =>
                {
                    itemFulfillability.FulfillabilityResult = ItemFulfillabilityResult.SLOT_UNAVAILABLE;
                });

                createOrderResponse = new CreateOrderResponse
                {
                    OrderFailure = new OrderFailure
                    {
                        OrderFulfillability = checkOrderFulfillabilityResponse.OrderFulfillability,
                        Cause = Cause.ORDER_UNFULFILLABLE
                    }
                };
            }
            return createOrderResponse;
        }

        #region Private Methods

        private List<SelectedProduct> PrepareSelectedProducts(List<LineItem> items, UserInformation userInformation, CheckOrderFulfillabilityResponse checkOrderFulfillabilityResponse)
        {
            var selectedProducts = new List<SelectedProduct>();
            var tokenId = checkOrderFulfillabilityResponse.OrderFulfillability.TokenId;
            var lineItemFulfillabilities = checkOrderFulfillabilityResponse.OrderFulfillability.ItemFulfillability;
            //foreach (var item in items)
            foreach (var lineItemFulfillability in lineItemFulfillabilities)
            {
                var availabilityReferenceId = lineItemFulfillability.AvailabilityReferenceId;
                var item = lineItemFulfillability.LineItem;
                var availabilityData = _TableStorageOperations.RetrieveData<BaseAvailabilitiesEntity>(availabilityReferenceId, tokenId);

                var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(item.StartSec);
                var startDateTime = dateTimeOffset.UtcDateTime;

                var selectedProduct = new SelectedProduct
                {
                    AvailabilityReferenceId = availabilityData.RowKey,
                    CheckinDate = startDateTime.ToString(),
                    PickupLocation = "", //TODO: Will be picked from the IntakeFormAnswers
                    SpecialRequest = "", //TODO: Will be picked from the IntakeFormAnswers
                    //Currently, passing only the lead passenger details
                    PassengerDetails = new List<PassengerDetail>
                    {
                        new PassengerDetail
                        {
                            FirstName = userInformation.GivenName,
                            LastName = userInformation.FamilyName,
                            IsLeadPassenger = true,
                            PassengerTypeId = 0  // TODO: Need to retrieve it from the storage for the given TicketTypeId
                        }
                    },
                    Questions = PrepareQuestions(item.IntakeFormAnswers?.IntakeFormFieldAnswers)
                };
                selectedProducts.Add(selectedProduct);
            }
            return selectedProducts;
        }

        private List<Question> PrepareQuestions(List<IntakeFormFieldAnswer> intakeFormFieldAnswers)
        {
            if (intakeFormFieldAnswers == null) return null;

            var questions = new List<Question>();
            foreach (var intakeFormFieldAnswer in intakeFormFieldAnswers)
            {
                var question = new Question
                {
                    Id = intakeFormFieldAnswer.Id,
                };
                questions.Add(question);
            }
            return questions;
        }

        #endregion Private Methods
    }
}