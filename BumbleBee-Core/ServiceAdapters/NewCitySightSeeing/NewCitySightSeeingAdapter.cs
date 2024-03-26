using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.NewCitySightSeeing;
using Logger.Contract;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands.Contract;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters.Contracts;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Availability;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Booking;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Product;

namespace ServiceAdapters.NewCitySightSeeing
{
    public class NewCitySightSeeingAdapter : INewCitySightSeeingAdapter, IAdapter
    {
        #region "Private Members"
        private readonly IProductsCommandHandler _productsCommandHandler;

        private readonly INewCitySightSeeingCalendarHandler _newCitySightSeeingCalendarHandler;
        private readonly INewCitySightSeeingCalendarConverter _newCitySightSeeingCalendarConverter;

        private readonly INewCitySightSeeingAvailabilityHandler _newCitySightSeeingAvailabilityHandler;
        private readonly INewCitySightSeeingAvailabilityConverter _newCitySightSeeingAvailabilityConverter;

        private readonly IReservationCommandHandler _newCitySightSeeingReservationCommandHandler;
        private readonly INewCitySightSeeingReservationConverter _newCitySightSeeingReservationConverter;

        private readonly IBookingCommandHandler _newCitySightSeeingBookingCommandHandler;
        private readonly INewCitySightSeeingBookingConverter _newCitySightSeeingBookingConverter;

        private readonly ICancellationCommandHandler _newCitySightSeeingCancellationCommandHandler;
        private readonly INewCitySightSeeingCancellationConverter _newCitySightSeeingCancellationConverter;

        private readonly ILogger _log;

        #endregion "Private Members"

        public NewCitySightSeeingAdapter(
             IProductsCommandHandler productsCommandHandler,

             INewCitySightSeeingCalendarHandler newCitySightSeeingCalendarHandler,
             INewCitySightSeeingCalendarConverter newCitySightSeeingCalendarConverter,

             INewCitySightSeeingAvailabilityHandler newCitySightSeeingAvailabilityHandler,
             INewCitySightSeeingAvailabilityConverter newCitySightSeeingAvailabilityConverter,

             IReservationCommandHandler newCitySightSeeingReservationCommandHandler,
             IBookingCommandHandler newCitySightSeeingBookingCommandHandler,

             INewCitySightSeeingReservationConverter newCitySightSeeingReservationConverter,
             INewCitySightSeeingBookingConverter newCitySightSeeingBookingConverter,

             ICancellationCommandHandler newCitySightSeeingCancellationCommandHandler,
             INewCitySightSeeingCancellationConverter newCitySightSeeingCancellationConverter,

             ILogger log
            )
        {
            _productsCommandHandler = productsCommandHandler;

            _newCitySightSeeingCalendarHandler = newCitySightSeeingCalendarHandler;
            _newCitySightSeeingCalendarConverter = newCitySightSeeingCalendarConverter;

            _newCitySightSeeingAvailabilityHandler = newCitySightSeeingAvailabilityHandler;
            _newCitySightSeeingAvailabilityConverter = newCitySightSeeingAvailabilityConverter;

            _newCitySightSeeingReservationCommandHandler = newCitySightSeeingReservationCommandHandler;
            _newCitySightSeeingBookingCommandHandler = newCitySightSeeingBookingCommandHandler;

            _newCitySightSeeingReservationConverter = newCitySightSeeingReservationConverter;
            _newCitySightSeeingBookingConverter = newCitySightSeeingBookingConverter;

            _newCitySightSeeingCancellationCommandHandler = newCitySightSeeingCancellationCommandHandler;
            _newCitySightSeeingCancellationConverter = newCitySightSeeingCancellationConverter;

            _log = log;
        }

        public List<Products> ProductsAsync(
            NewCitySightSeeingCriteria newCitySightSeeingCriteria,
            string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var activityDetailRs = default(List<Products>);
            var returnValue = _productsCommandHandler.Execute(newCitySightSeeingCriteria, token, MethodType.GetProducts, out request, out response);
            if (returnValue != null)
            {
                activityDetailRs = returnValue as List<Products>;
            }

            return activityDetailRs;
        }
        //Calendar API Data only
        public List<AvailabilityResponse> apiCalendarData(
            NewCitySightSeeingCriteria newCitySightSeeingCriteria,
            string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var apiAvailabilityResponse = default(List<AvailabilityResponse>);
            var returnValue =
                _newCitySightSeeingCalendarHandler.Execute(
                newCitySightSeeingCriteria,
                token, MethodType.GetAvailability,
                out request, out response);
            if (returnValue != null)
            {
                apiAvailabilityResponse = returnValue as List<AvailabilityResponse>;
            }

            return apiAvailabilityResponse;
        }
        //Calendar API data with converter
        public List<ProductOption> GetActivityInformation(
            NewCitySightSeeingCriteria newCitySightSeeingCriteria,
            string token,
            out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;

            if (newCitySightSeeingCriteria == null) return null;

            var resultActivity = default(List<ProductOption>);

            var returnValue =
                _newCitySightSeeingCalendarHandler.Execute(
                    newCitySightSeeingCriteria, token,
                    MethodType.GetAvailability,
                     out request, out response);

            if (returnValue != null)
            {
                var responseObject = returnValue as AvailabilityResponse;
                if (responseObject != null)
                {
                    var convertedActivity = _newCitySightSeeingCalendarConverter.Convert(returnValue, MethodType.GetAvailability, newCitySightSeeingCriteria);
                    resultActivity = convertedActivity as List<ProductOption>;
                }
            }
            return resultActivity;
        }

        public List<ProductOption> GetActivityAvailability(
            NewCitySightSeeingCriteria
            newCitySightSeeingCriteria, string token,
            out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;

            if (newCitySightSeeingCriteria == null) return null;
            var resultActivity = default(List<Isango.Entities.ProductOption>);

            var returnValue =
                _newCitySightSeeingAvailabilityHandler.Execute(
                newCitySightSeeingCriteria, token, MethodType.GetAvailability,
                 out request, out response);


            if (returnValue != null)
            {
                var responseObject = returnValue as AvailabilityResponse;
                if (responseObject != null)
                {
                    var convertedActivity = _newCitySightSeeingAvailabilityConverter.Convert(returnValue, MethodType.GetAvailability, newCitySightSeeingCriteria);
                    resultActivity = convertedActivity as List<Isango.Entities.ProductOption>;
                }
            }
            return resultActivity;
        }



        public List<SelectedProduct> CreateReservation(
            Booking booking, string token,
            out string request, out string response)
        {
            var _returnValue = new List<SelectedProduct>();
            request = string.Empty;
            response = string.Empty;
            var selectedProducts = booking.SelectedProducts.Where(x => x.APIType == Isango.Entities.Enums.APIType.NewCitySightSeeing);
            foreach (var selectedProduct in selectedProducts)
            {
                var productOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                if (productOption == null || string.IsNullOrWhiteSpace(productOption?.SupplierOptionCode))
                {
                    throw new Exception($"productOption / SupplierOptionCode(NewCitySightSeeingProductId) not found for ActivityId {selectedProduct.Id}");
                }
                var _inputContext = new InputContext
                {
                    SelectedProducts = selectedProduct,
                    LanguageCode = booking?.Language?.Code,
                    VoucherEmailAddress = booking?.VoucherEmailAddress,
                    VoucherPhoneNumber = booking?.VoucherPhoneNumber,
                    TotalCustomers = productOption.TravelInfo.NoOfPassengers.Sum(x => x.Value),
                    BookingReference = booking?.ReferenceNumber,
                    PostCode = booking?.User?.ZipCode,
                    Address = booking?.User?.Address1,
                    City = booking?.User?.City
                };

                var returnAPIReservatonResponse = _newCitySightSeeingReservationCommandHandler.Execute(_inputContext, token, MethodType.Reservation, out request, out response);
                if (returnAPIReservatonResponse == null) return null;

                var _responseValueIsango = _newCitySightSeeingReservationConverter.Convert(returnAPIReservatonResponse, MethodType.Reservation, selectedProduct);
                var selectedProductIsango = _responseValueIsango as SelectedProduct;
                _returnValue.Add(selectedProductIsango);
            }
            return _returnValue;
        }

        public SelectedProduct CreateReservationProduct(
          SelectedProduct selectedProduct, string language, string voucherEmailAddress,
           string voucherPhoneNumber, string referenceNumber,
           string zipCode, string address, string city, string token,
         out string request, out string response)
        {
            var _returnValue = new List<SelectedProduct>();
            request = string.Empty;
            response = string.Empty;
            var selectedProducts = selectedProduct;

            var productOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
            if (productOption == null || string.IsNullOrWhiteSpace(productOption?.SupplierOptionCode))
            {
                throw new Exception($"productOption / SupplierOptionCode(NewCitySightSeeingProductId) not found for ActivityId {selectedProduct.Id}");
            }
            var _inputContext = new InputContext
            {
                SelectedProducts = selectedProduct,
                LanguageCode = language,
                VoucherEmailAddress = voucherEmailAddress,
                VoucherPhoneNumber = voucherPhoneNumber,
                TotalCustomers = productOption.TravelInfo.NoOfPassengers.Sum(x => x.Value),
                BookingReference = referenceNumber,
                PostCode = zipCode,
                Address = address,
                City = city
            };

            var returnAPIReservatonResponse = _newCitySightSeeingReservationCommandHandler.Execute(_inputContext, token, MethodType.Reservation, out request, out response);
            if (returnAPIReservatonResponse == null) return null;

            var _responseValueIsango = _newCitySightSeeingReservationConverter.Convert(returnAPIReservatonResponse, MethodType.Reservation, selectedProduct);
            var selectedProductIsango = _responseValueIsango as SelectedProduct;
            return selectedProductIsango;
        }

        public NewCitySightSeeing.Entities.Reservation.ReservationResponse CreateReservation(
           SelectedProduct selectedProduct, string language, string voucherEmailAddress,
           string voucherPhoneNumber, string referenceNumber,
           string zipCode, string address, string city, string token,
           out string request, out string response)
        {
            var _returnValue = new List<SelectedProduct>();
            request = string.Empty;
            response = string.Empty;


            var productOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
            if (productOption == null || string.IsNullOrWhiteSpace(productOption?.SupplierOptionCode))
            {
                throw new Exception($"productOption / SupplierOptionCode(NewCitySightSeeingProductId) not found for ActivityId {selectedProduct.Id}");
            }
            var _inputContext = new InputContext
            {
                SelectedProducts = selectedProduct,
                LanguageCode = language,
                VoucherEmailAddress = voucherEmailAddress,
                VoucherPhoneNumber = voucherPhoneNumber,
                TotalCustomers = productOption.TravelInfo.NoOfPassengers.Sum(x => x.Value),
                BookingReference = referenceNumber,
                PostCode = zipCode,
                Address = address,
                City = city
            };

            var returnAPIReservatonResponse = _newCitySightSeeingReservationCommandHandler.Execute(_inputContext, token, MethodType.Reservation, out request, out response);
            if (returnAPIReservatonResponse == null)
            {
                return null;
            }
            return returnAPIReservatonResponse as NewCitySightSeeing.Entities.Reservation.ReservationResponse;
        }
        public List<SelectedProduct> CreateBooking(
           Booking booking, string token,
           out string request, out string response)
        {
            var _returnValue = new List<SelectedProduct>();
            request = string.Empty;
            response = string.Empty;
            var selectedProducts = booking.SelectedProducts.Where(x => x.APIType == Isango.Entities.Enums.APIType.NewCitySightSeeing);
            foreach (var selectedProduct in selectedProducts)
            {
                var productOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                if (productOption == null || string.IsNullOrWhiteSpace(productOption?.SupplierOptionCode))
                {
                    throw new Exception($"productOption / SupplierOptionCode(NewCitySightSeeingProductId) not found for ActivityId {selectedProduct.Id}");
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
                var returnAPIBookreservatonResponse = _newCitySightSeeingBookingCommandHandler.Execute(_inputContext, token, MethodType.CreateBooking, out request, out response);
                if (returnAPIBookreservatonResponse == null) return null;

                var _responseValueIsango = _newCitySightSeeingBookingConverter.Convert(returnAPIBookreservatonResponse, MethodType.CreateBooking, selectedProduct);
                var selectedProductIsango = _responseValueIsango as SelectedProduct;
                _returnValue.Add(selectedProductIsango);
            }
            return _returnValue;
        }

        public SelectedProduct CreateBookingSingle(
          SelectedProduct selectedProduct, string language, string voucherEmailAddress,
           string voucherPhoneNumber, string referenceNumber, string token,
          out string request, out string response)
        {
            
            request = string.Empty;
            response = string.Empty;
            
            var productOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
            if (productOption == null || string.IsNullOrWhiteSpace(productOption?.SupplierOptionCode))
            {
                throw new Exception($"productOption / SupplierOptionCode(NewCitySightSeeingProductId) not found for ActivityId {selectedProduct.Id}");
            }
            var _inputContext = new InputContext
            {
                SelectedProducts = selectedProduct,
                LanguageCode = language,
                VoucherEmailAddress = voucherEmailAddress,
                VoucherPhoneNumber = voucherPhoneNumber,
                TotalCustomers = productOption.TravelInfo.NoOfPassengers.Sum(x => x.Value),
                BookingReference = referenceNumber
            };
            var returnAPIBookreservatonResponse = _newCitySightSeeingBookingCommandHandler.Execute(_inputContext, token, MethodType.CreateBooking, out request, out response);
            if (returnAPIBookreservatonResponse == null) return null;

            var _responseValueIsango = _newCitySightSeeingBookingConverter.Convert(returnAPIBookreservatonResponse, MethodType.CreateBooking, selectedProduct);
            return _responseValueIsango as SelectedProduct;
       }

        public string CancelBooking(string reservationId,
            string isangoRef, string token,
         out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            if (string.IsNullOrEmpty(reservationId))
            {
                return null;
            }
            var cancelBookingRq = new CancelBookingRequest
            {
                ReservationId = reservationId,
                ExternalServiceRefCode = isangoRef
            };
            var returnAPIBookcancelaltionResponse = _newCitySightSeeingCancellationCommandHandler.Execute(cancelBookingRq, token, MethodType.CancelBooking, out request, out response);
            var cancellationResponse = returnAPIBookcancelaltionResponse as string;
            return cancellationResponse;
        }


        public AvailabilityResponse GetNullVariantData(
            NewCitySightSeeingCriteria newCitySightSeeingCriteria,
            string token,
            out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;

            if (newCitySightSeeingCriteria == null) return null;

            var returnValue =
                _newCitySightSeeingCalendarHandler.Execute(
                    newCitySightSeeingCriteria, token,
                    MethodType.GetAvailability,
                     out request, out response);
            return returnValue as AvailabilityResponse;
        }

    }
}