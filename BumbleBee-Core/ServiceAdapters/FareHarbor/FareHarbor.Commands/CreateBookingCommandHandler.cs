using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.FareHarbor;
using Logger.Contract;
using Newtonsoft.Json.Linq;
using ServiceAdapters.FareHarbor.Constants;
using ServiceAdapters.FareHarbor.FareHarbor.Commands.Contracts;
using ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels;
using System.Security.Authentication;
using System.Text;
using Util;

namespace ServiceAdapters.FareHarbor.FareHarbor.Commands
{
    public class CreateBookingCommandHandler : CommandHandlerBase, ICreateBookingCommandHandler
    {
        private string _supportEmail = "support@isango.com";
        private string _supportPhoneNumber = "+4402033551240";

        public CreateBookingCommandHandler(ILogger log) : base(log)
        {
            try
            {
                _supportPhoneNumber = ConfigurationManagerHelper.GetValuefromAppSettings("SupportPhoneNumer");
                _supportEmail = ConfigurationManagerHelper.GetValuefromAppSettings("mailfrom");
            }
            catch
            {
                //Ignored //Default values are give above
            }
        }

        protected override object FareHarborApiRequest<T>(T inputContext)
        {
            var selectedProduct = inputContext as CreateBooking;

            var createBooking = SerializeDeSerializeHelper.SerializeWithContractResolver(selectedProduct);
            var content = new StringContent(RemoverProperties(createBooking, true), Encoding.UTF8, Constant.ApplicationOrJson);
            var result = HttpClient.PostAsync(FormUrlCreateBooking(selectedProduct), content);
            result.Wait();
            return result.Result;
        }

        /// <summary>
        /// Remove Properties from the Booking object before it is passed to the api
        /// </summary>
        /// <param name="jSon">Request json string</param>
        /// /// <param name="isReBooking"></param>
        /// <returns>Json string without removed properties</returns>
        public override string RemoverProperties(string jSon, bool isReBooking)
        {
            var jsonJObject = JObject.Parse(jSon);
            try
            {
                jsonJObject.Descendants()
                        .OfType<JProperty>()
                        .Where(attr => attr.Name.StartsWith(Constant.ShortName) || attr.Name.StartsWith(Constant.AvailabilityId) || attr.Name.StartsWith(Constant.UuId) || (isReBooking && attr.Name.StartsWith(Constant.ReBooking)) || attr.Name.StartsWith(Constant.UserKey))
                        .ToList() // you should call ToList because you're about to changing the result, which is not possible if it is IEnumerable
                        .ForEach(attr => attr.Remove()); // removing unwanted attributes
                jSon = jsonJObject.ToString(); // backing result to json
            }
            catch (Exception ex)
            {
                //ignore
            }
            return jSon;
        }

        protected override async Task<object> FareHarborApiRequestAsync<T>(T inputContext)
        {
            var selectedProduct = inputContext as CreateBooking;

            var createBooking = SerializeDeSerializeHelper.SerializeWithContractResolver(selectedProduct);
            var content = new StringContent(RemoverProperties(createBooking, true), Encoding.UTF8, Constant.ApplicationOrJson);
            var result = await HttpClient.PostAsync(FormUrlCreateBooking(selectedProduct), content);
            return result;
        }

        private string FormUrlCreateBooking(CreateBooking createBooking)
        {
            return $"{Constant.CompanyUrlConstant}/{createBooking.ShortName}/{Constant.AvailabilitiesUrlConstant}/{createBooking.AvailabilityId}/{Constant.BookingsUrlConstant}/";
        }

        protected override object CreateInputRequest<T>(T product)
        {
            var selectedProduct = product as FareHarborSelectedProduct;
            var createBooking = new CreateBooking();

            if (selectedProduct != null)
            {
                createBooking.ShortName = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected)?.SupplierName;

                var firstOption = (ActivityOption)selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                if (firstOption != null)
                {
                    var customer = firstOption.Customers.FirstOrDefault(x => x.IsLeadCustomer);
                    createBooking.UserKey = firstOption.UserKey;
                    createBooking.AvailabilityId = firstOption.AvailToken;
                    createBooking.Contact = new Contact
                    {
                        Name = $"{customer?.FirstName} {customer?.LastName}",
                        Email = _supportEmail,
                        Phone = _supportPhoneNumber
                    };

                    try
                    {
                        if (!string.IsNullOrEmpty(selectedProduct.HotelPickUpLocation))
                        {
                            var LodgingId = Convert.ToInt32(selectedProduct.HotelPickUpLocation.Split('-')[0]);
                            createBooking.Lodging = LodgingId;
                        }
                    }
                    catch(Exception ex)
                    {

                    }

                    var customerList = new List<Customer>();
                    var FHPandA = firstOption.SellPrice.DatePriceAndAvailabilty.FirstOrDefault(x => x.Value.IsSelected).Value as FareHarborPriceAndAvailability;
                    var customerTypePriceIds = FHPandA?.CustomerTypePriceIds;

                    var numberOfPassengers = selectedProduct.ProductOptions.FirstOrDefault(selProduct => selProduct.IsSelected)?.TravelInfo.NoOfPassengers;

                    var isPerUnit = FHPandA?.PricingUnits?.FirstOrDefault()?.UnitType == UnitType.PerUnit;
                    if (numberOfPassengers != null)
                    {
                        foreach (var item in numberOfPassengers)
                        {
                            switch (item.Key)
                            {
                                case PassengerType.Adult:
                                    {
                                        GetCustomerList(PassengerType.Adult, item.Value, customerTypePriceIds,
                                           ref customerList, isPerUnit);
                                        break;
                                    }
                                case PassengerType.Child:
                                    {
                                        GetCustomerList(PassengerType.Child, item.Value, customerTypePriceIds,
                                           ref customerList, isPerUnit);
                                        break;
                                    }
                                case PassengerType.Infant:
                                    {
                                        GetCustomerList(PassengerType.Infant, item.Value, customerTypePriceIds,
                                           ref customerList, isPerUnit);
                                        break;
                                    }
                                case PassengerType.Youth:
                                    {
                                        GetCustomerList(PassengerType.Youth, item.Value, customerTypePriceIds,
                                           ref customerList, isPerUnit);
                                        break;
                                    }

                                default:
                                    break;
                            }
                        }
                    }

                    createBooking.Note = string.IsNullOrWhiteSpace(selectedProduct.SpecialRequest)
                        ? Constant.Na
                        : selectedProduct.SpecialRequest;
                    createBooking.VoucherNumber = selectedProduct.BookingReferenceNumber;
                    createBooking.Customers = customerList;
                    createBooking.CustomFieldValues = new List<CustomFieldValue>();
                }
            }
            return createBooking;
        }

        protected override void AddRequestHeadersAndAddressToApi<T>(T inputContext)
        {
            var userKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborUserKey);
            var createBooking = inputContext as CreateBooking;

            if(createBooking != null)
            {
                userKey = createBooking.UserKey;
            }
            else
            {
                var selectedProduct = inputContext as FareHarborSelectedProduct;
                var tempUserKey = ((ActivityOption)selectedProduct?.ProductOptions?.FirstOrDefault())?.UserKey;
                if (!string.IsNullOrEmpty(tempUserKey))
                {
                    userKey = tempUserKey;
                }
            }
            var httpClientHandler = new HttpClientHandler();

            // Set TLS versions for .NET Core 6.0
            httpClientHandler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            HttpClient = new HttpClient(httpClientHandler);
            // SET TLS version for Framework 4.5
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            
            if (HttpClient.BaseAddress == null)
            {
                HttpClient.Timeout = TimeSpan.FromMinutes(3);
                HttpClient.BaseAddress = new Uri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborUri));
            }
            else
            {
                HttpClient.DefaultRequestHeaders.Remove(Constant.XFareHarborApiApp);
                HttpClient.DefaultRequestHeaders.Remove(Constant.XFareHarborApiUser);
            }

            HttpClient.DefaultRequestHeaders.Add(Constant.XFareHarborApiApp, ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborAppKey));
            HttpClient.DefaultRequestHeaders.Add(Constant.XFareHarborApiUser, userKey);
        }

        private void GetCustomerList(PassengerType customerType, int count, Dictionary<PassengerType, Int64> customerTypePriceIds, ref List<Customer> customerList, bool isPerUnit = false)
        {
            var customerTypeRatepk = customerTypePriceIds?.FirstOrDefault(x => x.Key.Equals(customerType)).Value ?? 0;
            for (var i = 0; i < count; i++)
            {
                var isAlreadyExists = customerList?.Any(x => x.CustomerTypeRate == customerTypeRatepk) == true;
                if (isPerUnit)
                {
                    if (!isAlreadyExists)
                    {
                        var customerNew = new Customer
                        {
                            CustomerTypeRate = customerTypeRatepk
                        };
                        customerList.Add(customerNew);
                    }
                }
                else
                {
                    var customerNew = new Customer
                    {
                        CustomerTypeRate = customerTypeRatepk
                    };
                    customerList.Add(customerNew);
                }
            }
        }
    }
}