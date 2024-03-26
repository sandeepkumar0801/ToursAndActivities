using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.GoldenTours;
using Logger.Contract;
using ServiceAdapters.GoldenTours.Constants;
using ServiceAdapters.GoldenTours.GoldenTours.Commands.Contracts;
using ServiceAdapters.GoldenTours.GoldenTours.Entities.Booking;
using System.Globalization;
using System.Reflection;
using System.Text;
using Util;
using Customer = ServiceAdapters.GoldenTours.GoldenTours.Entities.Booking.Customer;
using Product = ServiceAdapters.GoldenTours.GoldenTours.Entities.Booking.Product;

namespace ServiceAdapters.GoldenTours.GoldenTours.Commands
{
    public class BookingCommandHandler : CommandHandlerBase, IBookingCommandHandler
    {
        public BookingCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call to get the booking dates
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object GoldenToursApiRequest<T>(T inputContext)
        {
            var input = inputContext as BookingRequest;
            var methodPath = new Uri(GenerateMethodPath());
            var content = new StringContent(SerializeDeSerializeHelper.SerializeXml(input), Encoding.UTF8, Constant.ApplicationMediaType);

            var result = _httpClient.PostAsync(methodPath, content);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        /// <summary>
        /// Call to get the booking dates asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> GoldenToursApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext as BookingRequest;
            var methodPath = new Uri(GenerateMethodPath());
            var content = new StringContent(SerializeDeSerializeHelper.SerializeXml(input), Encoding.UTF8, Constant.ApplicationMediaType);

            var result = await _httpClient.PostAsync(methodPath, content);
            return ValidateApiResponse(result);
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            var selectedProducts = inputContext as List<SelectedProduct>;
            var selectedProduct = (GoldenToursSelectedProduct)selectedProducts?.FirstOrDefault();

            var bookingRequest = new BookingRequest
            {
                Key = _key,
                AgentId = _agentId,
                PaymentMode = Constant.DefaultPaymentMode,
                CurrencyCode = Constant.DefaultCurrencyCode,
                Customer = GetCustomer(selectedProduct),
                ProductInfo = new ProductInfo
                {
                    Product = GetProductInfo(selectedProducts)
                }
            };
            return bookingRequest;
        }

        #region Private Methods

        /// <summary>
        /// Create the API endpoint url
        /// </summary>
        /// <returns></returns>
        private string GenerateMethodPath()
        {
            return $"{_baseAddress}{UriConstants.Booking}";
        }

        private List<Product> GetProductInfo(List<SelectedProduct> selectedProducts)
        {
            var products = new List<Product>();
            foreach (var selectedProduct in selectedProducts)
            {
                var goldenToursSelectedProduct = (GoldenToursSelectedProduct)selectedProduct;
                var selectedOption = (ActivityOption)goldenToursSelectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);

                var pickupTimeId = string.Empty;

                var splitResult = selectedProduct?.HotelPickUpLocation?.Split('-');
                try
                {
                    if (splitResult.Length > 1)
                    {
                        Int32.TryParse(splitResult[0], out var pickupNumericId);
                        pickupTimeId = pickupNumericId > 0 ? pickupNumericId.ToString():string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    //Ignored as no pikupTimeid
                    //throw;
                }

                var product = new Product
                {
                    TravelDate = selectedOption?.TravelInfo.StartDate.ToString(Constant.DDMMYYYY, CultureInfo.InvariantCulture),
                    ProductId = selectedOption?.SupplierOptionCode,
                    ScheduleId = selectedOption?.ScheduleId,
                    PickuptimeId = pickupTimeId,
                    OtherRequirement = null,// goldenToursSelectedProduct.SpecialRequest,
                    PaxInfo = GetPaxInfo(goldenToursSelectedProduct.PaxInfo),
                    ReferenceNumber = goldenToursSelectedProduct.ReferenceNumber,
                };

                if (string.Equals(selectedOption?.ProductType, Constant.Transfers,
                    StringComparison.InvariantCultureIgnoreCase))
                    product.TransferInfo = GetTransferInfo(goldenToursSelectedProduct.ContractQuestions);

                products.Add(product);
            }
            return products;
        }

        private Customer GetCustomer(GoldenToursSelectedProduct selectedProduct)
        {
            var selectedOption = (ActivityOption)selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
            var leadPassenger = selectedOption?.Customers.FirstOrDefault(x => x.IsLeadCustomer);

            var customer = new Customer
            {
                FirstName = leadPassenger?.FirstName,
                LastName = leadPassenger?.LastName,
                Email = Constant.DefaultEmail,
                Title = string.IsNullOrWhiteSpace(leadPassenger?.Title) ? Constant.DefaultTitle : leadPassenger.Title,
                City = Constant.DefaultCity,
                CountryCode = Constant.DefaultCountryCode,
                Address1 = Constant.DefaultAddress,
                Phone = Constant.DefaultPhone,
                PostCode = Constant.DefaultPostCode,
                County = ""
            };
            return customer;
        }

        private PaxInfo GetPaxInfo(Dictionary<int, int> passengerInfo)
        {
            if (passengerInfo == null) return null;

            var units = new List<Unit>();
            foreach (var item in passengerInfo)
            {
                var unit = new List<Unit>
                {
                    new Unit
                    {
                        UnitId = Convert.ToString(item.Key),
                        PaxCount = Convert.ToString(item.Value)
                    }
                };
                units.AddRange(unit);
            }

            var paxInfo = new PaxInfo
            {
                Unit = units
            };

            return paxInfo;
        }

        private TransferInfo GetTransferInfo(List<ContractQuestion> contractQuestions)
        {
            var transferInfo = new TransferInfo();
            if (contractQuestions == null) return transferInfo;

            foreach (var contractQuestion in contractQuestions)
            {
                // Using reflection to get the propertyInfo for the given code, ignoring the case.
                var propertyInfo = transferInfo.GetType().GetProperty(contractQuestion.Code, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                // If propertyInfo is null then continue
                if (propertyInfo == null) continue;
                // Setting answer as the value of the property
                propertyInfo.SetValue(transferInfo, Convert.ChangeType(contractQuestion.Description, propertyInfo.PropertyType));
            }

            return transferInfo;
        }

        #endregion Private Methods
    }
}