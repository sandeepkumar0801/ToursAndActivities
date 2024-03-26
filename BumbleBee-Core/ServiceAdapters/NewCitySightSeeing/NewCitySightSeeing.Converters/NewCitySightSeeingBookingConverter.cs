using Isango.Entities;
using Isango.Entities.NewCitySightSeeing;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters.Contracts;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Booking;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters
{
    public class NewCitySightSeeingBookingConverter : ConverterBase, INewCitySightSeeingBookingConverter
    {
        /// <summary>
        /// Convert API Result Entities to Isnago.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>

        public object Convert(object apiResponse, MethodType methodType, object criteria = null)
        {
            if (apiResponse != null)
            {
                var bookingRS = apiResponse as BookingResponse;
                var selectedProduct = criteria as SelectedProduct;
                var selectedProducts = ConvertPurchaseResult(bookingRS, selectedProduct);
                return selectedProducts;
            }
            return null;
        }

        private SelectedProduct ConvertPurchaseResult(BookingResponse
            apiBookingRS, SelectedProduct selectedProduct)
        {
            var newCitySightSeeingSelectedProduct = (NewCitySightSeeingSelectedProduct)selectedProduct;

            newCitySightSeeingSelectedProduct.NewCitySightSeeingOrderCode = apiBookingRS?.OrderCode;
            newCitySightSeeingSelectedProduct.NewCitySightSeeingOrderDate = apiBookingRS.OrderDate;

            newCitySightSeeingSelectedProduct.BookingReference = System.Convert.ToString(apiBookingRS?.OrderCode);
            newCitySightSeeingSelectedProduct.ShortReference = System.Convert.ToString(apiBookingRS?.OrderCode);
            newCitySightSeeingSelectedProduct.SupplierReferenceNumber = System.Convert.ToString(apiBookingRS?.OrderCode);

            var listData = new List<LineList>();
            if (apiBookingRS != null)
            {
                foreach (var item in apiBookingRS?.Lines)
                {
                    var listSingle = new LineList
                    {
                        OrderLineCode = item?.OrderLineCode,
                        Quantity = item.Quantity,
                        Rate = item?.Rate,
                        QrCode=item.QrCode
                    };
                    listData.Add(listSingle);
                }
            }
            newCitySightSeeingSelectedProduct.NewCitySightSeeingLines = listData;
            return selectedProduct;
        }
    }
}