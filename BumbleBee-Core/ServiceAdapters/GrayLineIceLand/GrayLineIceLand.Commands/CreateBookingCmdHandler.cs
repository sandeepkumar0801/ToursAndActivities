using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.GrayLineIceLand.Constants;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands.Contracts;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;
using Product = ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels.Product;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands
{
    public class CreateBookingCmdHandler : CommandHandlerBase, ICreateBookingCmdHandler
    {
        public CreateBookingCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var bookingRq = new BookingRQ
            {
                AddExternalBookings = true
            };

            if (inputContext != null)
            {
                var leadCustomer = inputContext.SelectedProducts.FirstOrDefault()?.ProductOptions.FirstOrDefault(prod => prod.IsSelected)?.Customers.FirstOrDefault(x => x.IsLeadCustomer);

                bookingRq.OrderId = inputContext.SelectedProducts.FirstOrDefault()?.FactsheetId ?? 0;

                if (leadCustomer != null)
                {
                    bookingRq.LeadPassengerFirstName = leadCustomer.FirstName;
                    bookingRq.LeadPassengerLastName = leadCustomer.LastName;
                    bookingRq.ConfirmToEmail = leadCustomer.Email;
                    bookingRq.CustomerEmail = leadCustomer.Email;
                }

                bookingRq.AgentProfileId = inputContext.AgentProfileId;
                bookingRq.CurrencyCode = inputContext.CurrencyCode;
                bookingRq.CreatedBy = inputContext.AgentProfileId;
                bookingRq.Created = DateTime.Now.ToUniversalTime();
                bookingRq.AgentReference = inputContext.BookingReference;
                bookingRq.AutoCheckIn = true;

                bookingRq.Products = new Product[inputContext.SelectedProducts.Count];

                var i = 0;
                inputContext.SelectedProducts.ForEach(selectedProduct =>
                {
                    var selectedProductOption = (ActivityOption)selectedProduct.ProductOptions[0];
                    var travelInfo = selectedProduct.ProductOptions.FirstOrDefault(prod => prod.IsSelected)?.TravelInfo;

                    bookingRq.Products[i] = new Product
                    {
                        TourNumber = selectedProduct.Code,
                        ExternalReference = selectedProduct.BookingReferenceNumber
                    };

                    bookingRq.Products[i].TourNumber = selectedProduct.Code;
                    var keyDate = selectedProduct.ProductOptions.FirstOrDefault(prod => prod.IsSelected)?.TravelInfo.StartDate;
                    bookingRq.Products[i].TourDepartureId =
                        (selectedProductOption.SellPrice.DatePriceAndAvailabilty
                            .FirstOrDefault(x => x.Key.Equals(keyDate)).Value).TourDepartureId;

                    var pickupIdPartintDesc = selectedProduct?.HotelPickUpLocation?.Split('-')?.FirstOrDefault();
                    int? pickupId = null;
                    int.TryParse(pickupIdPartintDesc, out var tempId);
                    if (tempId > 0)
                    {
                        pickupId = tempId;
                    }
                    pickupId = pickupId > 0 ? pickupId : null;
                    bookingRq.Products[i].PickupLocationId = Convert.ToInt32(pickupId);

                    bookingRq.Products[i].Price = selectedProduct.Price;

                    var ageGroupIds = selectedProduct.PaxAgeGroupIds;

                    #region Passengers

                    var passengers = new List<Passenger>
                    {
                        GeneratePassanger(ageGroupIds, travelInfo, PassengerType.Adult)
                    };

                    var childCount = travelInfo?.NoOfPassengers?.Where(x => x.Key == PassengerType.Child)
                                         .Select(s => s.Value).FirstOrDefault() ?? 0;
                    if (childCount > 0)
                    {
                        passengers.Add(GeneratePassanger(ageGroupIds, travelInfo, PassengerType.Child));
                    }
                    var youthCount = travelInfo?.NoOfPassengers?.Where(x => x.Key == PassengerType.Youth)
                                         .Select(s => s.Value).FirstOrDefault() ?? 0;
                    if (youthCount > 0)
                    {
                        passengers.Add(GeneratePassanger(ageGroupIds, travelInfo, PassengerType.Youth));
                    }

                    bookingRq.Products[i].Passengers = passengers.ToArray();

                    #endregion Passengers

                    i++;
                });

                bookingRq.Payments = new Payment[1];
                bookingRq.Payments[0] = new Payment
                {
                    PaymentType = (int)PaymentType.Voucher
                };
            }

            return bookingRq;
        }

        protected override object GetResults(object input, string authString)
        {
            if (input != null)
            {
                var client = new AsyncClient
                {
                    ServiceURL = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineBaseUrl)}{Entities.Constants.BookingURL}"
                };
                var headers = new Dictionary<string, string>
                {
                    {Constant.Authorization, $"{Constant.Bearer}{authString}"},
                    {Constant.Accept, Constant.App_Json},
                    {Constant.Content_type, Constant.App_Json}
                };
                return client.PostJsonWithHeader((BookingRQ)input, headers);
            }

            return null;
        }

        protected override async Task<object> GetResultsAsync(object input, string authString)
        {
            if (input != null)
            {
                var client = new AsyncClient
                {
                    ServiceURL = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineBaseUrl)}{Entities.Constants.BookingURL}"
                };
                return await client.PostJsonAsync(authString, (BookingRQ)input);
            }
            return null;
        }

        public Passenger GeneratePassanger(Dictionary<PassengerType, int> ageGroupIds, TravelInfo travelInfo, PassengerType type)
        {
            var passenger = new Passenger()
            {
                AgeGroup = ageGroupIds.FirstOrDefault(x => x.Key.Equals(type)).Value,
                NumberOfPax = travelInfo.NoOfPassengers.FirstOrDefault(x => x.Key.Equals(type)).Value,
                Quantity = travelInfo.NoOfPassengers.FirstOrDefault(x => x.Key.Equals(type)).Value
            };

            return passenger;
        }

        protected override object GetResults(object input, string authString, out string requestJson, out string responseJson)
        {
            requestJson = string.Empty;
            responseJson = string.Empty;
            if (input != null)
            {
                var client = new AsyncClient
                {
                    ServiceURL = ConfigurationManagerHelper.GetValuefromAppSettings("GrayLineBaseUrl") + Constant.BookingUrl
                };
                var headers = new Dictionary<string, string>
                {
                    { "Authorization", "Bearer " + authString },
                    { "Accept", "application/json" },
                    { "Content-type", "application/json" }
                };
                return client.PostJsonWithHeader((BookingRQ)input, headers, out requestJson, out responseJson);
            }

            return null;
        }

        protected override async Task<string> GetStringResultsAsync(object input)
        {
            return null;
        }
    }
}