using Isango.Entities.FareHarbor;
using Logger.Contract;
using ServiceAdapters.FareHarbor.Constants;
using ServiceAdapters.FareHarbor.FareHarbor.Commands.Contracts;
using ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels;
using System.Text;
using Util;

namespace ServiceAdapters.FareHarbor.FareHarbor.Commands
{
    public class RebookingCommandHandler : CommandHandlerBase, IRebookingCommandHandler
    {
        public RebookingCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object FareHarborApiRequest<T>(T inputContext)
        {
            var selectedProduct = inputContext as CreateBooking;

            var createBooking = SerializeDeSerializeHelper.SerializeWithContractResolver(selectedProduct);
            var content = new StringContent(RemoverProperties(createBooking, false), Encoding.UTF8, Constant.ApplicationOrJson);
            var result = HttpClient.PostAsync(FormUrlCreateBooking(selectedProduct), content);
            result.Wait();
            return result.Result;
        }

        protected override async Task<object> FareHarborApiRequestAsync<T>(T inputContext)
        {
            var selectedProduct = inputContext as CreateBooking;

            var createBooking = SerializeDeSerializeHelper.SerializeWithContractResolver(selectedProduct);
            var content = new StringContent(RemoverProperties(createBooking, false), Encoding.UTF8, Constant.ApplicationOrJson);
            var result = await HttpClient.PostAsync(FormUrlCreateBooking(selectedProduct), content);
            return result;
        }

        private string FormUrlCreateBooking(CreateBooking createBooking)
        {
            return $"{Constant.CompanyUrlConstant}/{createBooking.ShortName}/{Constant.AvailabilitiesUrlConstant}/{createBooking.AvailabilityId}/{Constant.BookingsUrlConstant}/";
        }

        protected override object CreateInputRequest<T>(T selectedProduct)
        {
            var selectedProductReq = selectedProduct as FareHarborSelectedProduct;
            var createBooking = new CreateBooking();
            if (selectedProductReq != null)
            {
                createBooking.ShortName = selectedProductReq.ProductOptions.FirstOrDefault(x => x.IsSelected)?.SupplierName;
                createBooking.Rebooking = selectedProductReq.UuId;
                createBooking.AvailabilityId = selectedProductReq.ProductOptions.FirstOrDefault(x => x.IsSelected)?.Id.ToString();

                var customer = selectedProductReq.ProductOptions.FirstOrDefault(x => x.IsSelected)?.Customers.FirstOrDefault(x => x.IsLeadCustomer);
                createBooking.Contact = new Contact
                {
                    Name = $"{customer?.FirstName} {customer?.LastName}",
                    Email = Constant.TestEmail,
                    Phone = Constant.TestPhoneNumber
                };

                var customerList = new List<Customer>();

                var numberOfPassengers = selectedProductReq.ProductOptions.FirstOrDefault(selProduct => selProduct.IsSelected)?.TravelInfo.NoOfPassengers;
                var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");

                if (numberOfPassengers != null)
                {
                    Parallel.ForEach(numberOfPassengers, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, item =>
                    {
                        if (item.Key == Isango.Entities.Enums.PassengerType.Adult)
                        {
                            for (var count = 0; count < item.Value; count++)
                            {
                                customerList.Add(new Customer
                                {
                                    CustomerTypeRate = selectedProductReq.AdultPriceId
                                });
                            }
                        }

                        if (item.Key == Isango.Entities.Enums.PassengerType.Child)
                        {
                            for (var count = 0; count < item.Value; count++)
                            {
                                customerList.Add(new Customer
                                {
                                    CustomerTypeRate = selectedProductReq.ChildPriceId
                                });
                            }
                        }
                    });
                }

                createBooking.Note = Constant.TestNote;
                createBooking.Customers = customerList;
            }

            return createBooking;
        }
    }
}