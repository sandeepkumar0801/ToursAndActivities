using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.DataDumping;
using Isango.Entities.Enums;
using Isango.Entities.GoogleMaps.BookingServer;
using Isango.Entities.GoogleMaps.BookingServer.Enums;
using Isango.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using TableStorageOperations.Contracts;
using Util;
using WebAPI.Models.GoogleMapsBookingServer;
using WebAPI.Models.RequestModels;

namespace WebAPI.Helper
{
    /// <summary>
    /// Helper method for Google Maps controller
    /// </summary>
    public class GoogleMapsHelper
    {
        #region Properties

        private readonly ActivityHelper _activityHelper;
        private readonly ITableStorageOperation _TableStorageOperations;
        private readonly IActivityService _activityService;

        #endregion Properties

        #region ctr

        public GoogleMapsHelper(ActivityHelper activityHelper,
            ITableStorageOperation TableStorageOperations,
            IActivityService activityService)
        {
            _activityHelper = activityHelper;
            _TableStorageOperations = TableStorageOperations;
            _activityService = activityService;
        }

        #endregion ctr

        #region Public Methods

        /// <summary>
        /// To Check the Activity Availability
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CheckOrderFulfillabilityResponse CheckOrderFulfillability(CheckOrderFulfillabilityRequest request)
        {
            var partitionKey = DateTime.UtcNow.ToString("dd_MMM_yyyy");
            var tokenId = Guid.NewGuid().ToString();
            var lineItemFulfillabilities = new List<LineItemFulfillability>();
            foreach (var lineItem in request.LineItems)
            {
                var lineItemPrice = lineItem.Price.PriceMicros / Constants.Constant.MicroUnit;
                var serviceDetails = new List<StorageServiceDetail>();
                var paxDetails = new List<PaxDetail>();
                var violatedTicketConstraints = new List<ViolatedTicketConstraint>();
                GetPaxAndServiceDetails(lineItem, partitionKey, serviceDetails, paxDetails);
                var distinctOptions = serviceDetails.Select(s => s.ServiceOptionId).Distinct().ToList();
                if (serviceDetails.Any() && distinctOptions.Count == 1)
                {
                    var activity = _activityService.GetActivityById(Convert.ToInt32(lineItem.ServiceId), DateTime.Now,
                        "en")?.GetAwaiter().GetResult();
                    var isIndependablePaxPresent = IsIndependablePaxPresent(lineItem, serviceDetails, activity);
                    if (isIndependablePaxPresent || lineItemPrice == 0)
                    {
                        GetViolatedTicketConstraints(activity, serviceDetails, lineItem, violatedTicketConstraints);
                        if (!violatedTicketConstraints.Any())
                        {
                            activity = GetActivityAvailability(lineItem, paxDetails, tokenId);
                            var option = activity?.ProductOptions?.FirstOrDefault(w => w.ServiceOptionId == distinctOptions.First());
                            var checkinDate = DateTimeOffset.FromUnixTimeSeconds(lineItem.StartSec).DateTime;
                            if (option != null)
                            {
                                var priceAndAvailability = option.BasePrice.DatePriceAndAvailabilty.ContainsKey(checkinDate.Date) ? option.BasePrice.DatePriceAndAvailabilty[checkinDate.Date] : null;
                                if (!string.IsNullOrWhiteSpace(priceAndAvailability?.ReferenceId))
                                {
                                    var itemFulfillabilityResult =
                                        option.AvailabilityStatus == AvailabilityStatus.AVAILABLE
                                            ? ItemFulfillabilityResult.CAN_FULFILL
                                            : ItemFulfillabilityResult.SLOT_UNAVAILABLE;
                                    if (itemFulfillabilityResult == ItemFulfillabilityResult.CAN_FULFILL)
                                    {
                                        CheckPriceChange(option, lineItemPrice, lineItem);
                                    }
                                    var fulfillability = new LineItemFulfillability
                                    {
                                        AvailabilityReferenceId = priceAndAvailability.ReferenceId,
                                        LineItem = lineItem,
                                        FulfillabilityResult = itemFulfillabilityResult,
                                        UnfulfillableReason = string.Empty,
                                        ViolatedTicketConstraints = violatedTicketConstraints,
                                    };
                                    lineItemFulfillabilities.Add(fulfillability);
                                }
                                else
                                {
                                    var fulfillability = GetLineItemFulfillability(lineItem, ItemFulfillabilityResult.SLOT_UNAVAILABLE);
                                    lineItemFulfillabilities.Add(fulfillability);
                                }
                            }
                            else
                            {
                                var fulfillability = GetLineItemFulfillability(lineItem, ItemFulfillabilityResult.SLOT_UNAVAILABLE);
                                lineItemFulfillabilities.Add(fulfillability);
                            }
                        }
                        else
                        {
                            var fulfillability = GetLineItemFulfillability(lineItem, ItemFulfillabilityResult.TICKET_CONSTRAINT_VIOLATED);
                            fulfillability.ViolatedTicketConstraints = violatedTicketConstraints;
                            lineItemFulfillabilities.Add(fulfillability);
                        }
                    }
                    else
                    {
                        var fulfillability = GetLineItemFulfillability(lineItem, ItemFulfillabilityResult.CHILD_TICKETS_WITHOUT_ADULT);
                        lineItemFulfillabilities.Add(fulfillability);
                    }
                }
                else
                {
                    var fulfillability = GetLineItemFulfillability(lineItem, ItemFulfillabilityResult.SLOT_UNAVAILABLE);
                    lineItemFulfillabilities.Add(fulfillability);
                }
            }
            var fulfillabilityResponse = PrepareCheckOrderFulfillabilityResponse(request, lineItemFulfillabilities, tokenId);
            return fulfillabilityResponse;
        }

        /// <summary>
        /// Insert the Order Response data in the storage
        /// </summary>
        /// <param name="order"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public void InsertOrderResponse(Order order, string tokenId)
        {
            try
            {
                _TableStorageOperations.InsertOrderResponse(order, tokenId);
            }
            catch (Exception ex)
            {
                //log here
            }
        }

        /// <summary>
        /// Fetch the Order Response data for the given orderIds from storage
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        public List<Order> GetOrders(string userId, List<string> orderIds)
        {
            try
            {
                var orders = new List<Order>();
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    var order = _TableStorageOperations.GetOrders(Constants.Constant.UserId, userId);
                    if (order != null)
                        orders.Add(order);
                }
                else
                {
                    foreach (var orderId in orderIds)
                    {
                        var order = _TableStorageOperations.GetOrders(Constants.Constant.OrderId, orderId);
                        if (order != null)
                            orders.Add(order);
                    }
                }

                return orders;
            }
            catch (Exception ex)
            {
                //log here
                throw ex;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void CheckPriceChange(ProductOption option, long lineItemPrice, LineItem lineItem)
        {
            var price = option.BasePrice.Amount;
            if (price != lineItemPrice)
            {
                lineItem.Price.PriceMicros = Convert.ToInt64(price * Constants.Constant.MicroUnit);
                lineItem.WarningReason = price < lineItemPrice
                    ? WarningReason.PRICE_DECREASE
                    : WarningReason.PRICE_INCREASE;
            }
        }

        private void GetViolatedTicketConstraints(Activity activity, List<StorageServiceDetail> serviceDetails, LineItem lineItem, List<ViolatedTicketConstraint> violatedTicketConstraints)
        {
            foreach (var lineItemTicket in lineItem.Tickets)
            {
                var ticketServiceDetail = serviceDetails.FirstOrDefault(w => w.TicketTypeId == lineItemTicket.TicketId);
                if (ticketServiceDetail != null)
                {
                    var passengerInfo =
                        activity.PassengerInfo.FirstOrDefault(w => w.PassengerTypeId == ticketServiceDetail.PassengerTypeId);
                    var passengetCount = lineItemTicket.Count;
                    if (passengerInfo != null &&
                        !(passengerInfo.MinSize <= passengetCount && passengerInfo.MaxSize >= passengetCount))
                    {
                        var violatedTicketConstraint = new ViolatedTicketConstraint()
                        {
                            TicketId = lineItemTicket.TicketId,
                        };
                        if (passengerInfo.MinSize > passengetCount)
                        {
                            violatedTicketConstraint.MinTicketCount = passengerInfo.MinSize.ToString();
                        }
                        else
                        {
                            violatedTicketConstraint.MaxTicketCount = passengerInfo.MaxSize.ToString();
                        }
                        violatedTicketConstraints.Add(violatedTicketConstraint);
                    }
                }
            }
        }

        private LineItemFulfillability GetLineItemFulfillability(LineItem lineItem, ItemFulfillabilityResult fulfillabilityResult)
        {
            var fulfillability = new LineItemFulfillability
            {
                AvailabilityReferenceId = string.Empty,
                LineItem = lineItem,
                FulfillabilityResult = fulfillabilityResult,
                UnfulfillableReason = string.Empty,
                ViolatedTicketConstraints = new List<ViolatedTicketConstraint>(),
            };
            return fulfillability;
        }

        private CheckOrderFulfillabilityResponse PrepareCheckOrderFulfillabilityResponse(
            CheckOrderFulfillabilityRequest request, List<LineItemFulfillability> lineItemFulfillabilities, string tokenId)
        {
            var orderFulfillability = new OrderFulfillability()
            {
                OrderFulfillabilityResult =
                    request.LineItems.Count == lineItemFulfillabilities.Count(w =>
                        w.FulfillabilityResult == ItemFulfillabilityResult.CAN_FULFILL ||
                        w.FulfillabilityResult == ItemFulfillabilityResult.INCORRECT_PRICE)
                        ? OrderFulfillabilityResult.CAN_FULFILL
                        : OrderFulfillabilityResult.UNFULFILLABLE_LINE_ITEM,
                ItemFulfillability = lineItemFulfillabilities,
                UnfulfillableReason = string.Empty,
                TokenId = tokenId
            };
            var fulfillabilityResponse = new CheckOrderFulfillabilityResponse
            {
                CartExpirationSec = 36000,
                OrderFulfillability = orderFulfillability,
                Fees = new Fees(),
                //FeesAndTaxes = new
            };
            return fulfillabilityResponse;
        }

        private Activity GetActivityAvailability(LineItem lineItem, List<PaxDetail> paxDetails,
            string tokenId)
        {
            var affiliateId = ConfigurationManagerHelper.GetValuefromAppSettings("GoogleMapsAffiliatedId");
            var googleMapsCountryIp = ConfigurationManagerHelper.GetValuefromAppSettings("GoogleMapsCountryIp");
            var affiliate = _activityHelper.GetAffiliateInfo(affiliateId);
            var checkinDate = DateTimeOffset.FromUnixTimeSeconds(lineItem.StartSec).DateTime;
            var availabilityInput = new CheckAvailabilityRequest()
            {
                ActivityId = Convert.ToInt32(lineItem.ServiceId),
                LanguageCode = Constants.Constant.DefaultLanguage,
                AffiliateId = affiliateId,
                CheckinDate = checkinDate,
                CheckoutDate = checkinDate,
                CountryIp = googleMapsCountryIp,
                CurrencyIsoCode = lineItem.Price.CurrencyCode,
                PaxDetails = paxDetails,
                TokenId = tokenId,
            };
            var clientInfo = _activityHelper.PrepareClientInfoInput(availabilityInput);
            clientInfo.IsSupplementOffer = affiliate.AffiliateConfiguration.IsSupplementOffer;

            var criteria = new Criteria
            {
                // ReSharper disable once PossibleNullReferenceException
#pragma warning disable S2259 // Null pointers should not be dereferenced
                CheckinDate = availabilityInput.CheckinDate.Date,
#pragma warning restore S2259 // Null pointers should not be dereferenced
                CheckoutDate = availabilityInput.CheckoutDate.Date,
                NoOfPassengers = new Dictionary<PassengerType, int>()
            };
            var activity = _activityHelper.GetProductAvailability(availabilityInput, clientInfo, criteria);
            if (activity?.ProductOptions?.Count > 0)
            {
                activity.ProductOptions = _activityHelper.GetProductOptionsAfterPriceRuleEngine(activity.PriceTypeId,
                    activity.ProductOptions, clientInfo, criteria.CheckinDate);

                _TableStorageOperations.InsertData(activity, clientInfo.ApiToken);
            }

            return activity;
        }

        private void GetPaxAndServiceDetails(LineItem lineItem, string partitionKey, List<StorageServiceDetail> serviceDetails, List<PaxDetail> paxDetails)
        {
            foreach (var itemTicket in lineItem.Tickets)
            {
                var rowKey = $"{itemTicket.TicketId}-{lineItem.StartSec}";
                var serviceDetail =
                    _TableStorageOperations.RetrieveData<StorageServiceDetail>(partitionKey, rowKey,
                        Constants.Constant.PriceAndAvailabilityDumping);
                if (serviceDetail != null && serviceDetail.ActivityId == Convert.ToInt32(lineItem.ServiceId))
                {
                    serviceDetails.Add(serviceDetail);
                    paxDetails.Add(new PaxDetail()
                    {
                        Count = itemTicket.Count,
                        PassengerTypeId = (PassengerType)serviceDetail.PassengerTypeId
                    });
                }
            }
        }

        private bool IsIndependablePaxPresent(LineItem lineItem, List<StorageServiceDetail> serviceDetails, Activity activity)
        {
            foreach (var lineItemTicket in lineItem.Tickets)
            {
                var ticketServiceDetail =
                    serviceDetails.FirstOrDefault(w => w.TicketTypeId == lineItemTicket.TicketId);
                if (ticketServiceDetail != null)
                {
                    var passengerInfo =
                        activity.PassengerInfo.FirstOrDefault(w =>
                            w.PassengerTypeId == ticketServiceDetail.PassengerTypeId);
                    if (passengerInfo != null && passengerInfo.IndependablePax)
                    {
                        return passengerInfo.IndependablePax;
                    }
                }
            }
            return false;
        }

        #endregion Private Methods
    }
}