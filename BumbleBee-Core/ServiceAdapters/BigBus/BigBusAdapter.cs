using Isango.Entities.BigBus;
using Isango.Entities.Enums;
using ServiceAdapters.BigBus.BigBus.Commands.Contracts;
using ServiceAdapters.BigBus.BigBus.Converters.Contracts;
using ServiceAdapters.BigBus.BigBus.Entities;
using SelectedProduct = Isango.Entities.SelectedProduct;

namespace ServiceAdapters.BigBus
{
    public class BigBusAdapter : IBigBusAdapter, IAdapter
    {
        #region "Private Members"

        private string _returnValue;
        private object _responseValue;
        private readonly InputContext _inputContext = new InputContext();

        private readonly ICreateBookingCommandHandler _createBookingCommandHandler;
        private readonly ICancelBookingCommandHandler _cancelBookingCommandHandler;
        private readonly ICreateReservationCommandHandler _createReservationCommandHandler;
        private readonly ICancelReservationCommandHandler _cancelReservationCommandHandler;

        private readonly ICreateBookingConverter _createBookingConverter;
        private readonly ICancelBookingConverter _cancelBookingConverter;
        private readonly ICreateReservationConverter _createReservationConverter;
        private readonly ICancelReservationConverter _cancelReservationConverter;

        #endregion "Private Members"

        #region Constructor

        public BigBusAdapter
            (
                ICreateBookingCommandHandler createBookingCommandHandler,
                ICancelBookingCommandHandler cancelBookingCommandHandler,
                ICreateReservationCommandHandler createReservationCommandHandler,
                ICancelReservationCommandHandler cancelReservationCommandHandler,

                ICreateBookingConverter createBookingConverter,
                ICancelBookingConverter cancelBookingConverter,
                ICreateReservationConverter createReservationConverter,
                ICancelReservationConverter cancelReservationConverter
            )
        {
            _createBookingCommandHandler = createBookingCommandHandler;
            _cancelBookingCommandHandler = cancelBookingCommandHandler;
            _createReservationCommandHandler = createReservationCommandHandler;
            _cancelReservationCommandHandler = cancelReservationCommandHandler;

            _createBookingConverter = createBookingConverter;
            _cancelBookingConverter = cancelBookingConverter;
            _createReservationConverter = createReservationConverter;
            _cancelReservationConverter = cancelReservationConverter;
        }

        #endregion Constructor

        /// <summary>
        /// This call is used to create supplier booking
        /// </summary>
        /// <param name="selectedProducts"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public List<SelectedProduct> CreateBooking(List<SelectedProduct> selectedProducts, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;

            var bigBusSelectedProduct = (BigBusSelectedProduct)selectedProducts.FirstOrDefault();
            _inputContext.TicketPerPassenger = bigBusSelectedProduct?.TicketPerPassenger ?? false;
            _inputContext.ReservationReference = bigBusSelectedProduct?.ReservationReference;
            _inputContext.Products = new List<Product>();

            foreach (var selectedProduct in selectedProducts)
            {
                var productOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                var travelInfo = productOption?.TravelInfo;
                var product = new Product
                {
                    ProductId = productOption?.SupplierOptionCode,//selectedProduct?.ProductOptions?.FirstOrDefault()?.SupplierOptionCode,
                    DateOfTravel = Convert.ToDateTime(travelInfo?.StartDate),
                    NoOfPassengers = new Dictionary<string, int>()
                };

                var adultCount = GetPaxCount(travelInfo?.NoOfPassengers, PassengerType.Adult);
                var childCount = GetPaxCount(travelInfo?.NoOfPassengers, PassengerType.Child);

                if (adultCount != 0)
                {
                    product.NoOfPassengers.Add(PassengerType.Adult.ToString(), adultCount);
                }

                if (childCount != 0)
                {
                    product.NoOfPassengers.Add(PassengerType.Child.ToString(), childCount);
                }

                _inputContext.Products.Add(product);
            }

            _returnValue = _createBookingCommandHandler.Execute(_inputContext, MethodType.CreateBooking, token, out request, out response);

            _responseValue = _createBookingConverter.Convert(_returnValue, selectedProducts);
            return _responseValue as List<SelectedProduct>;
        }

        /// <summary>
        /// This call is used to cancel supplier booking
        /// </summary>
        /// <param name="bookingReference"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public Dictionary<string, bool> CancelBooking(List<SelectedProduct> selectedProducts, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;

            _inputContext.BookingReference = ((BigBusSelectedProduct)selectedProducts.FirstOrDefault())?.BookingReference;

            _returnValue = _cancelBookingCommandHandler.Execute(_inputContext, MethodType.CancelBooking, token, out request, out response);

            _responseValue = _cancelBookingConverter.Convert(_returnValue, selectedProducts);
            return _responseValue as Dictionary<string, bool>;
        }

        /// <summary>
        /// This call is used to create reservation
        /// </summary>
        /// <param name="bigBusSelectedProducts"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public List<SelectedProduct> CreateReservation(List<SelectedProduct> selectedProducts, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;

            _inputContext.TicketPerPassenger = false;
            _inputContext.Products = new List<Product>();

            foreach (var selectedProduct in selectedProducts)
            {
                var productOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                if (productOption == null || string.IsNullOrWhiteSpace(productOption?.SupplierOptionCode))
                {
                    throw new Exception($"productOption / SupplierOptionCode(BigBusProductId) not found for ActivityId {selectedProduct.Id}");
                }
                var travelInfo = productOption?.TravelInfo;
                var product = new Product
                {
                    ProductId = productOption?.SupplierOptionCode,//selectedProduct?.ProductOptions?.FirstOrDefault()?.SupplierOptionCode,
                    DateOfTravel = Convert.ToDateTime(travelInfo?.StartDate),
                    NoOfPassengers = new Dictionary<string, int>()
                };

                var adultCount = GetPaxCount(travelInfo?.NoOfPassengers, PassengerType.Adult);
                var childCount = GetPaxCount(travelInfo?.NoOfPassengers, PassengerType.Child);

                if (adultCount != 0)
                {
                    product.NoOfPassengers.Add(PassengerType.Adult.ToString(), adultCount);
                }

                if (childCount != 0)
                {
                    product.NoOfPassengers.Add(PassengerType.Child.ToString(), childCount);
                }

                _inputContext.Products.Add(product);
            }

            if (_inputContext.Products.Count == 0)
            {
                return null;
            }
            _returnValue = _createReservationCommandHandler.Execute(_inputContext, MethodType.CreateReservation, token, out request, out response);

            _responseValue = _createReservationConverter.Convert(_returnValue, selectedProducts);
            return _responseValue as List<SelectedProduct>;
        }

        /// <summary>
        /// This call is used to cancel the reservation
        /// </summary>
        /// <param name="reservationReference"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public Dictionary<string, bool> CancelReservation(List<SelectedProduct> selectedProducts, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;

            _inputContext.ReservationReference = ((BigBusSelectedProduct)selectedProducts.FirstOrDefault())?.ReservationReference;

            _returnValue = _cancelReservationCommandHandler.Execute(_inputContext, MethodType.CancelReservation, token, out request, out response);

            _responseValue = _cancelReservationConverter.Convert(_returnValue, selectedProducts);
            return _responseValue as Dictionary<string, bool>;
        }

        #region Private Methods

        /// <summary>
        /// Get the selected passenger count for the given passenger type
        /// </summary>
        /// <param name="noOfPassengers"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private int GetPaxCount(Dictionary<PassengerType, int> noOfPassengers, PassengerType type)
        {
            return noOfPassengers?.Where(x => x.Key == type).Select(s => s.Value).FirstOrDefault() ?? 0;
        }

        #endregion Private Methods
    }
}