using Isango.Entities.Redeam;
using Logger.Contract;
using ServiceAdapters.Redeam.Constants;
using ServiceAdapters.Redeam.Redeam.Commands.Contracts;
using ServiceAdapters.Redeam.Redeam.Entities;
using ServiceAdapters.Redeam.Redeam.Entities.CreateBooking;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Util;

namespace ServiceAdapters.Redeam.Redeam.Commands
{
    internal class CreateBookingCommandHandler : CommandHandlerBase, ICreateBookingCommandHandler
    {
        private RedeamAPIConfig redeamAPIConfig;

        public CreateBookingCommandHandler(ILogger log) : base(log)
        {
            redeamAPIConfig = RedeamAPIConfig.GetInstance();
        }

        /// <summary>
        /// Call to Create Booking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object RedeamApiRequest<T>(T inputContext)
        {
            var input = inputContext as CreateBookingRequest;
            var methodPath = new Uri(GenerateMethodPath());

            var content = new StringContent(SerializeDeSerializeHelper.Serialize(input), Encoding.UTF8, Constant.ApplicationMediaType);

            var result = HttpClient.PostAsync(methodPath, content);
            result.Wait();
            return ValidateApiResponse(result.Result);

            #region ### Read Static file to avoid actual booking

            /*
            string dataFromFile = string.Empty;
            using (StreamReader r = new StreamReader(@"D:\Sandeep\API\API Redeam\Live Booking\19b4f1ad-2098-4fac-8a1b-c186143dc14a-RES.txt"))
            {
                dataFromFile = r.ReadToEnd();
            }
            content = new StringContent(dataFromFile);
            var retvlaue = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(dataFromFile, System.Text.Encoding.UTF8, "application/json") };
            return ValidateApiResponse(retvlaue);
            //*/

            #endregion ### Read Static file to avoid actual booking
        }

        /// <summary>
        /// Call to Create Booking asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> RedeamApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext as CreateBookingRequest;
            var methodPath = new Uri(GenerateMethodPath());

            var content = new StringContent(SerializeDeSerializeHelper.Serialize(input), Encoding.UTF8, Constant.ApplicationMediaType);

            var result = await HttpClient.PostAsync(methodPath, content);
            return ValidateApiResponse(result);
        }

        /// <summary>
        /// Create the supplier's input request from Isango entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest<T>(T inputContext)
        {
            var selectedProduct = inputContext as RedeamSelectedProduct;
            var productOption = selectedProduct?.ProductOptions.FirstOrDefault(x => x.IsSelected);
            var leadCustomer = productOption?.Customers?.FirstOrDefault(x => x.IsLeadCustomer);

            var createBookingRequest = new CreateBookingRequest
            {
                Booking = new Booking
                {
                    HoldId = selectedProduct?.HoldId,
                    Customer = new Entities.CreateBooking.Customer
                    {
                        FirstName = leadCustomer?.FirstName,
                        LastName = leadCustomer?.LastName,
                        Email = redeamAPIConfig.NotificationEmailAddressIsango,
                        Phone = redeamAPIConfig.SupportPhoneNumer
                    },
                    ResellerBookingRef = selectedProduct.BookingReferenceNumber,
                    Items = PrepareItems(selectedProduct)
                }
            };
            return createBookingRequest;
        }

        #region Private Methods

        /// <summary>
        /// Create the API endpoint url
        /// </summary>
        /// <returns></returns>
        private string GenerateMethodPath()
        {
            return $"{redeamAPIConfig.BaseUri}{UriConstants.CreateBooking}";
        }

        private List<Item> PrepareItems(RedeamSelectedProduct redeamSelectedProduct)
        {
            var items = new List<Item>();
            var productOption = redeamSelectedProduct?.ProductOptions.FirstOrDefault(x => x.IsSelected);

            if (productOption == null) return items;
            var customers = productOption.Customers;
            var numberOfPassengers = productOption.TravelInfo?.NoOfPassengers;
            if (customers == null || numberOfPassengers == null) return items;

            foreach (var customer in customers)
            {
                var priceId = redeamSelectedProduct.PriceId.FirstOrDefault(x => x.Key == customer.PassengerType.ToString().ToUpperInvariant()).Value;
                var item = new Item
                {
                    StartTime = productOption.TravelInfo.StartDate,
                    PriceId = priceId,
                    RateId = redeamSelectedProduct.RateId,
                    SupplierId = redeamSelectedProduct.SupplierId,
                    Quantity = 1,
                    Traveler = new Traveler
                    {
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        IsLead = customer.IsLeadCustomer,
                        Type = customer.PassengerType.ToString().ToUpperInvariant()
                    }
                };
                if (customer.PassengerType == Isango.Entities.Enums.PassengerType.Adult
                    || customer.PassengerType == Isango.Entities.Enums.PassengerType.Child)
                {
                    items.Add(item);
                }
            }
            return items;
        }

        #endregion Private Methods
    }
}