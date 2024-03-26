using Logger.Contract;
using ServiceAdapters.BigBus.BigBus.Commands.Contracts;
using ServiceAdapters.BigBus.BigBus.Entities;
using ServiceAdapters.BigBus.Constants;
using System.Text;
using Util;

namespace ServiceAdapters.BigBus.BigBus.Commands
{
    public class CreateReservationCommandHandler : CommandHandlerBase, ICreateReservationCommandHandler
    {
        public CreateReservationCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var inputData = new ReservationRequestObject
            {
                ReservationRequest = new ReservationRequest
                {
                    Products = new Products
                    {
                        Product = new List<ProductBooking>()
                    },
                }
            };

            foreach (var item in inputContext.Products)
            {
                var prod = new ProductBooking
                {
                    DateOfTravel = item.DateOfTravel,
                    ProductId = item.ProductId,
                    Items = new List<ItemBase>()
                };

                foreach (var passenger in item.NoOfPassengers)
                {
                    var perPassenger = new ItemBase
                    {
                        Category = passenger.Key,
                        Quantity = passenger.Value.ToString()
                    };

                    prod.Items.Add(perPassenger);
                }

                inputData.ReservationRequest.Products.Product.Add(prod);
            }

            return inputData;
        }

        protected override string BigBusApiRequest<T>(T inputContext)
        {
            var CreateReservation = SerializeDeSerializeHelper.Serialize(inputContext);
            var Content = new StringContent(CreateReservation, Encoding.UTF8, Constant.ApplicationJson);
            var Result = HttpClient.PostAsync(UriConstants.Reservation, Content);
            Result.Wait();
            return Result.Result.Content.ReadAsStringAsync().Result;
        }
    }
}