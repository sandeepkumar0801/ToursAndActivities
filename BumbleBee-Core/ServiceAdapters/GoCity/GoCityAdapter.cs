using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.GoCity;
using Logger.Contract;
using ServiceAdapters.GoCity.GoCity.Commands.Contract;
using ServiceAdapters.GoCity.GoCity.Converters.Contracts;
using ServiceAdapters.GoCity.GoCity.Entities;
using ServiceAdapters.GoCity.GoCity.Entities.Booking;
using ServiceAdapters.GoCity.GoCity.Entities.Product;

namespace ServiceAdapters.GoCity
{
    public class GoCityAdapter : IGoCityAdapter, IAdapter
    {
        #region "Private Members"

        private readonly IProductsCommandHandler _productsCommandHandler;
        private readonly IBookingCommandHandler _bookingCommandHandler;
        private readonly IBookingConverter _bookingConverter;

        private readonly ICancellationCommandHandler _cancelCommandHandler;
        private readonly IGoCityCancellationConverter _cancelConverter;

        private readonly ILogger _log;

        #endregion "Private Members"

        public GoCityAdapter(
             IProductsCommandHandler productsCommandHandler,
             IBookingCommandHandler bookingCommandHandler,
             IBookingConverter bookingConverter,
             ICancellationCommandHandler goCityCancellationCommandHandler,
             IGoCityCancellationConverter goCityCancellationConverter,
             ILogger log
            )
        {
            _productsCommandHandler = productsCommandHandler;
            _bookingCommandHandler = bookingCommandHandler;
            _bookingConverter = bookingConverter;
            _cancelCommandHandler = goCityCancellationCommandHandler;
            _cancelConverter = goCityCancellationConverter;
            _log = log;
        }

        public ProductResponse ProductsAsync(
            GoCityCriteria goCityCriteria,
            string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var activityDetailRs = default(ProductResponse);
            var returnValue = _productsCommandHandler.Execute(goCityCriteria, token, MethodType.GetProducts, out request, out response);
            if (returnValue != null)
            {
                activityDetailRs = returnValue as ProductResponse;
            }

            return activityDetailRs;
        }

        public SelectedProduct CreateBooking(
           Booking booking, string token,
           out string request, out string response)
        {
            var _returnValue = new SelectedProduct();
            request = string.Empty;
            response = string.Empty;
            var selectedProducts = booking.SelectedProducts.Where(x => x.APIType == Isango.Entities.Enums.APIType.GoCity);
            foreach (var selectedProduct in selectedProducts)
            {
                var productOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                if (productOption == null || string.IsNullOrWhiteSpace(productOption?.SupplierOptionCode))
                {
                    throw new Exception($"productOption / SupplierOptionCode(GoCityProductId) not found for ActivityId {selectedProduct.Id}");
                }
                var _inputContext = new InputContext
                {
                    SelectedProducts = selectedProduct,
                    LanguageCode = booking?.Language?.Code,
                    VoucherEmailAddress = booking?.VoucherEmailAddress,
                    VoucherPhoneNumber = booking?.VoucherPhoneNumber,
                    TotalCustomers = productOption.TravelInfo.NoOfPassengers.Sum(x => x.Value),
                    BookingReference = booking?.ReferenceNumber
                };
                var returnAPIBookreservatonResponse = _bookingCommandHandler.Execute(_inputContext, token, MethodType.CreateBooking, out request, out response);
                if (returnAPIBookreservatonResponse == null) return null;

                var _responseValueIsango = _bookingConverter.Convert(returnAPIBookreservatonResponse, MethodType.CreateBooking, selectedProduct);
                var selectedProductIsango = _responseValueIsango as SelectedProduct;
                _returnValue = selectedProductIsango;
            }
            return _returnValue;
        }

        public bool? CancelBooking(string ordernum,
            string customerEmail, string token,
         out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            if (string.IsNullOrEmpty(ordernum))
            {
                return false;
            }
            var listOrdercancelrequest = new List<Ordercancelrequest>();
            var orderCancelRequests = new Ordercancelrequest
            {
                OrderNumber = ordernum
            };
            listOrdercancelrequest.Add(orderCancelRequests);
            var cancelBookingRq = new CancelBookingRequest
            {
                InternalUserEmail = customerEmail,
                OrderCancelRequests = listOrdercancelrequest,
                Reason = ""
            };
            var returnAPIBookcancelaltionResponse = _cancelCommandHandler.Execute(cancelBookingRq, token, MethodType.CancelBooking, out request, out response);
            if (returnAPIBookcancelaltionResponse == null) return null;

            var _responseValueIsango = _cancelConverter.Convert(returnAPIBookcancelaltionResponse, MethodType.CancelBooking, null);
            var returnStatus = _responseValueIsango as bool?;
            return returnStatus;
        }
    }
}