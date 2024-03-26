using Isango.Entities;
using Isango.Entities.GoCity;
using ServiceAdapters.GoCity.GoCity.Converters.Contracts;
using ServiceAdapters.GoCity.GoCity.Entities;
using ServiceAdapters.GoCity.GoCity.Entities.Booking;

namespace ServiceAdapters.GoCity.GoCity.Converters
{
    public class BookingConverter : ConverterBase, IBookingConverter
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

        private SelectedProduct ConvertPurchaseResult(BookingResponse bookingRS, SelectedProduct selectedProduct)
        {
            var goCitySelectedProduct = (GoCitySelectedProduct)selectedProduct;

            if (!string.IsNullOrEmpty(bookingRS.OrderDetails.OrderNumber))
            {
                goCitySelectedProduct.APIStatus = bookingRS.OrderDetails.Status;
                var apiOrderNumber = bookingRS?.OrderDetails?.OrderNumber;
                var passDetail = bookingRS?.PassDetails;
                goCitySelectedProduct.OrderNumber = apiOrderNumber;
                goCitySelectedProduct.BookingReference = apiOrderNumber;
                goCitySelectedProduct.ShortReference = apiOrderNumber;
                goCitySelectedProduct.SupplierReferenceNumber = apiOrderNumber;

                var passList = new List<Isango.Entities.GoCity.Passlist>();
                if (passDetail != null)
                {
                    foreach (var item in passDetail?.PassList)
                    {
                        var pass = new Isango.Entities.GoCity.Passlist
                        {
                            ConfirmationCode = item?.ConfirmationCode,
                            CreatedDate = item.CreatedDate,
                            ExpDate = item.ExpDate,
                            SkuCode = item?.SkuCode
                        };
                        passList.Add(pass);
                    }
                }
               goCitySelectedProduct.Passlist = passList;
               goCitySelectedProduct.GetPassUrl = passDetail?.GetPassUrl;
               goCitySelectedProduct.PrintPassesUrl = passDetail?.PrintPassesUrl;
               goCitySelectedProduct.MobilePassesUrl = passDetail?.MobilePassesUrl;
            }
            return selectedProduct;
        }
    }
}