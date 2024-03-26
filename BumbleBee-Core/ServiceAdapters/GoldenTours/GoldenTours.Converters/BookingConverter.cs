using Isango.Entities;
using Isango.Entities.GoldenTours;
using Logger.Contract;
using ServiceAdapters.GoldenTours.GoldenTours.Converters.Contracts;
using ServiceAdapters.GoldenTours.GoldenTours.Entities.Booking;

namespace ServiceAdapters.GoldenTours.GoldenTours.Converters
{
    public class BookingConverter : ConverterBase, IBookingConverter
    {
        public BookingConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override object Convert<T>(T response, T request)
        {
            var result = DeSerializeXml<BookingResponse>(response as string);
            if (result == null) return null;

            var products = ConvertBookingResponse(result, request as List<SelectedProduct>);
            return products;
        }

        #region Private Method

        private List<SelectedProduct> ConvertBookingResponse(BookingResponse response, List<SelectedProduct> selectedProducts)
        {
            try
            {
                for (var i = 0; i < selectedProducts.Count; i++)
                {
                    if (response?.ProductInformation?.Products == null) continue;

                    var product = response.ProductInformation?.Products[i];
                    var goldenToursSelectedProduct = (GoldenToursSelectedProduct)selectedProducts[i];

                    goldenToursSelectedProduct.TicketReferenceNumber = product?.TicketRefNo;
                    goldenToursSelectedProduct.TicketUrl = product?.TicketUrl;
                    goldenToursSelectedProduct.QrCodes = product?.Ticketcodes?.Ticketcode?.Select(x => x.Ticketcodeinfo?.Code).ToList();
                }
                return selectedProducts;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GoldenTours.BookingConverter",
                    MethodName = "ConvertBookingResponse"
                };
                _logger.Error(isangoErrorEntity, ex);
                throw; //use throw as existing flow should not break bcoz of logging implementation.
            }
        }

        #endregion Private Method
    }
}