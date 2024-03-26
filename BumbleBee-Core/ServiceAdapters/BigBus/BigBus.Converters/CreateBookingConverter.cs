using Isango.Entities;
using Isango.Entities.BigBus;
using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.BigBus.BigBus.Converters.Contracts;
using ServiceAdapters.BigBus.BigBus.Entities;
using ServiceAdapters.BigBus.Constants;
using Util;

namespace ServiceAdapters.BigBus.BigBus.Converters
{
    public class CreateBookingConverter : ConverterBase, ICreateBookingConverter
    {
        public CreateBookingConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override object Convert<T>(string response, T request)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<BookingResponseObject>(response);
            if (result == null) return null;

            return ConvertBookingResult(result, request as List<SelectedProduct>);
        }

        #region Private Methods

        private List<SelectedProduct> ConvertBookingResult(BookingResponseObject bookingResponse, List<SelectedProduct> selectedProducts)
        {
            try
            {
                var products = bookingResponse.BookingResult.Products;
                if (products == null) return null;

                foreach (var selectedProduct in selectedProducts)
                {
                    var bigBusSelectedProduct = (BigBusSelectedProduct)selectedProduct;
                    bigBusSelectedProduct.BookingStatus = bookingResponse.BookingResult.Status;
                    bigBusSelectedProduct.BookingReference = bookingResponse.BookingResult.BookingReference;
                    bigBusSelectedProduct.ShortReference = bookingResponse.BookingResult.ShortReference;
                    bigBusSelectedProduct.SupplierReferenceNumber = bookingResponse.BookingResult.BookingReference;

                    var bigBusProductTicket = products.Product.FirstOrDefault(x => x.ProductId == bigBusSelectedProduct.ProductOptions.FirstOrDefault()?.SupplierOptionCode);
                    bigBusSelectedProduct.BigBusTickets = new List<BigBusTicket>();
                    if (bigBusProductTicket == null) continue;

                    foreach (var item in bigBusProductTicket.Items)
                    {
                        var isTicketPerPassenger = System.Convert.ToBoolean(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TicketPerPassenger));

                        if (isTicketPerPassenger)
                        {
                            var passengerType = item.TicketBarCode.IndexOf(Constant.AD, 0, StringComparison.Ordinal) > 0 ? PassengerType.Adult.ToString() : PassengerType.Child.ToString();

                            var bigBusTicket = new BigBusTicket
                            {
                                TicketType = item.TicketType,
                                TicketBarCode = item.TicketBarCode,
                                PassengerType = passengerType,
                                Quantity = Constant.PassengerCount
                            };

                            bigBusSelectedProduct.BigBusTickets.Add(bigBusTicket);
                        }
                        else
                        {
                            var numberOfPassengers = selectedProducts.FirstOrDefault()?.ProductOptions?.FirstOrDefault()
                                ?.TravelInfo.NoOfPassengers;
                            if (numberOfPassengers == null) continue;
                            foreach (var passenger in numberOfPassengers)
                            {
                                var bigBusTicket = new BigBusTicket
                                {
                                    TicketType = item.TicketType,
                                    TicketBarCode = item.TicketBarCode,
                                    PassengerType = passenger.Key.ToString(),
                                    Quantity = passenger.Value.ToString()
                                };

                                bigBusSelectedProduct.BigBusTickets.Add(bigBusTicket);
                            }
                        }
                    }
                }

                return selectedProducts;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BigBus.CreateBookingConverter",
                    MethodName = "ConvertBookingResult"
                };
                _logger.Error(isangoErrorEntity, ex);
                throw; //use throw as existing flow should not break bcoz of logging implementation.
            }
        }

        #endregion Private Methods
    }
}