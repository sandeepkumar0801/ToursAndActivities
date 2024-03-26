using Isango.Entities.Activities;
using Isango.Entities.GoCity;
using Logger.Contract;
using ServiceAdapters.GoCity.Constants;
using ServiceAdapters.GoCity.GoCity.Commands.Contract;
using ServiceAdapters.GoCity.GoCity.Entities.Booking;
using System.Text;
using Util;

namespace ServiceAdapters.GoCity.GoCity.Commands
{
    public class BookingCmdHandler : CommandHandlerBase, IBookingCommandHandler
    {
        public BookingCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T bookingContext)
        {
            var bookingRequest = new BookingRequest();
            try
            {
                var inputContext = bookingContext as ServiceAdapters.GoCity.GoCity.Entities.InputContext;
                var selectedProducts = inputContext.SelectedProducts;
                var languageCode = inputContext.LanguageCode;
                var voucherEmailAddress = inputContext.VoucherEmailAddress;
                var voucherPhoneNumber = inputContext.VoucherPhoneNumber;
                var TotalCustomers = inputContext.TotalCustomers;

                var goCitySelectdProducts = (GoCitySelectedProduct)selectedProducts;
                var IsangoRefNumber = $"{inputContext.BookingReference}-{goCitySelectdProducts?.ProductOptions?.FirstOrDefault()?.ServiceOptionId}";

                if (!(goCitySelectdProducts != null))
                {
                    return null;
                }
                var selectedProduct = goCitySelectdProducts;

                var selectedOptions = selectedProduct?.ProductOptions
                   ?.FindAll(f => f.IsSelected.Equals(true))
                   ?.Cast<ActivityOption>().ToList();

                var leadCustomer = selectedProduct?.ProductOptions?
                   .Find(f => f.IsSelected.Equals(true))
                   .Customers.Find(f => f.IsLeadCustomer.Equals(true));

                var customerName = leadCustomer?.FirstName.Contains(" ") == true ?
                   leadCustomer?.FirstName?.Split(' ')?.ToList() :
                   new List<string> { { leadCustomer.FirstName }, { leadCustomer.LastName } };

                var fName = customerName.FirstOrDefault() ?? "NA";
                var lName = customerName.LastOrDefault() ?? "NA";

                var customerData = new Customer
                {
                    FirstName = fName,
                    LastName = lName,
                    Phone = voucherPhoneNumber,
                    Email = voucherEmailAddress,
                };
                var travelInfoList = selectedOptions?.FirstOrDefault()?.TravelInfo;

                var details = new Details
                {
                    DeliveryMethod = "DIGITAL",
                    ExternalOrderNumber = IsangoRefNumber,
                    Locale = languageCode,
                    TravelDate = travelInfoList.StartDate.ToString("yyyy-MM-dd")
                };
                var cartList = new List<Cartitem>();
                var adultCount = travelInfoList?.NoOfPassengers?.Where(x => x.Key == Isango.Entities.Enums.PassengerType.Adult)?.Sum(x => x.Value);
                var childCount = travelInfoList?.NoOfPassengers?.Where(x => x.Key == Isango.Entities.Enums.PassengerType.Child)?.Sum(x => x.Value);
                var skuOptions = new Skuoptions
                {
                    Adult = Convert.ToInt32(adultCount),
                    Child = Convert.ToInt32(childCount)
                };
                var rateKey = selectedOptions.FirstOrDefault().SupplierOptionCode;
                var cartitem = new Cartitem
                {
                    ItemCode = rateKey,
                    SkuOptions = skuOptions
                };

                cartList.Add(cartitem);

                bookingRequest = new BookingRequest
                {
                    Customer = customerData,
                    Details = details,
                    CartItems = cartList
                };
                //test1
            }
            catch (Exception)
            {
                throw;
            }
            return bookingRequest;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(BookingResponse);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<BookingResponse>(responseText);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO Add logging here
                result = null;
            }
            return result;
        }

        protected override object GetResultsAsync(object input)
        {
            AddRequestHeadersAndAddressToApi();
            var url = Constant.Booking;
            var bookinfRQ = input as BookingRequest;
            if (bookinfRQ == null)
            {
                return null;
            }
            var createBooking = SerializeDeSerializeHelper.Serialize(bookinfRQ);
            var Content = new StringContent(createBooking, Encoding.UTF8, "application/json");
            var Result = _httpClient.PostAsync(url, Content);
            Result.Wait();
            return Result.Result.Content.ReadAsStringAsync().Result;
        }
    }
}