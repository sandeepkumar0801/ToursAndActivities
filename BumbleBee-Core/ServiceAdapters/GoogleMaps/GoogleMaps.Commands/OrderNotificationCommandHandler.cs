using System.Collections.Generic;
using Logger.Contract;
using ServiceAdapters.GoogleMaps.GoogleMaps.Commands.Contracts;
using ServiceAdapters.GoogleMaps.Constants;
using System.Net.Http;
using System.Text;
using Isango.Entities.GoogleMaps.BookingServer;
using ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO;
using Util;
using PaymentInformation = Isango.Entities.GoogleMaps.BookingServer.PaymentInformation;

namespace ServiceAdapters.GoogleMaps.GoogleMaps.Commands
{
    internal class OrderNotificationCommandHandler : CommandHandlerBase, IOrderNotificationCommandHandler
    {
        public OrderNotificationCommandHandler(ILogger log) : base(log)
        {
            
        }

        //protected override object GoogleMapsApiRequest<T>(T inputContext)
        //{
        //    var input = inputContext as OrderNotificationDto;
        //    if (input == null) return null;

        //    var methodPath = GenerateMethodPath(input);
        //    var orderNotificationRequest = input.OrderNotificationRequest;

        //    var orderCreate = SerializeDeSerializeHelper.Serialize(orderNotificationRequest);
        //    var content = new StringContent(orderCreate, Encoding.UTF32, Constant.ApplicationMediaType);

        //    var httpClient = GetHttpClient();
        //    var result = httpClient.PostAsync(methodPath, content);
        //    result.Wait();

        //    return ValidateApiResponse(result.Result);
        //}

        protected override object GoogleMapsApiRequest<T>(T inputContext)
        {
            var input = inputContext as OrderNotificationDto;
            if (input == null) return null;

            var orderNotificationRequest = input.OrderNotificationRequest;
            var orderCreate = SerializeDeSerializeHelper.Serialize(orderNotificationRequest);
            var content = new StringContent(orderCreate, Encoding.UTF32, Constant.ApplicationMediaType);

            var methodPath = GenerateMethodPath(input);
            var authToken = GetAuthToken();
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), methodPath);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
            request.Content = content;

            var httpClient = GetHttpClient();
            var result = httpClient.SendAsync(request);
            result.Wait();

            return ValidateApiResponse(result.Result);
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            var order = inputContext as Order;
            if (order == null) return null;
            var partnerId = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.PartnerId);
            var updateMask = "status";
            var orderNotificationRequest = new OrderNotificationRequest
            {
                Name = $"partners/{partnerId}/bookings/{order.OrderId}",
                PaymentInformation = MapPaymentInformation(order.PaymentInformation),
                ClientInformation = MapClientInformation(order.UserInformation),
                MerchantId = order.MerchantId,
                Item = MapItems(order.Items)
            };
            var orderNotificationDto = new OrderNotificationDto
            {
                OrderId = order.OrderId,
                PartnerId = partnerId,
                OrderNotificationRequest = orderNotificationRequest,
                UpdateMask = updateMask
            };
            return orderNotificationDto;
        }

        #region Private Methods

        private List<Item> MapItems(List<LineItem> lineItems)
        {
            var items = new List<Item>();
            foreach (var lineItem in lineItems)
            {
                var item = new Item
                {
                    DurationSec = lineItem.DurationSec.ToString(),
                    ServiceId = lineItem.ServiceId,
                    StartSec = lineItem.StartSec.ToString(),
                    Status = Constant.FailedStatus, //lineItem.Status.ToString(),
                    Price = new OrderPrice
                    {
                        CurrencyCode = lineItem.Price.CurrencyCode,
                        PriceMicros = lineItem.Price.PriceMicros.ToString(),
                        PricingOptionTag = lineItem.Price.PricingOptionTag
                    },
                    Tickets = MapTickets(lineItem.Tickets)
                };
                items.Add(item);
            }
            return items;
        }

        private List<Ticket> MapTickets(List<OrderedTicket> orderedTickets)
        {
            var tickets = new List<Ticket>();
            foreach (var orderedTicket in orderedTickets)
            {
                var ticket = new Ticket
                {
                    Count = orderedTicket.Count.ToString(),
                    TicketId = orderedTicket.TicketId
                };
                tickets.Add(ticket);
            }
            return tickets;
        }

        private ClientInformation MapClientInformation(UserInformation userInformation)
        {
            var clientInformation = new ClientInformation
            {
                Address = new UserAddress
                {
                    StreetAddress = userInformation.Address.StreetAddress,
                    PostalCode = userInformation.Address.PostalCode,
                    AddressCountry = userInformation.Address.Country,
                    AddressLocality = userInformation.Address.Locality,
                    AddressRegion = userInformation.Address.Region
                },
                Email = userInformation.Email,
                FamilyName = userInformation.FamilyName,
                GivenName = userInformation.GivenName,
                Telephone = userInformation.Telephone
            };
            return clientInformation;
        }

        private Entities.DTO.PaymentInformation MapPaymentInformation(PaymentInformation paymentInformation)
        {
            var paymentInfo = new Entities.DTO.PaymentInformation
            {
                
            };
            return paymentInfo;
        }

        private string GenerateMethodPath(OrderNotificationDto input)
        {
            return string.Format(UriConstants.OrderNotification, input.PartnerId, input.OrderId, input.UpdateMask);
        }

        #endregion Private Methods
    }
}
