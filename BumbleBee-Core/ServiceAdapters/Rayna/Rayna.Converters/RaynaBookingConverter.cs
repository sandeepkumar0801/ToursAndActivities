using Isango.Entities;
using Isango.Entities.Rayna;
using ServiceAdapters.Rayna.Rayna.Converters.Contracts;
using ServiceAdapters.Rayna.Rayna.Entities;

namespace ServiceAdapters.Rayna.Rayna.Converters
{
    public class RaynaBookingConverter : ConverterBase, IRaynaBookingConverter
    {
        /// <summary>
        /// Convert API Result Entities to Isango.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>

        public object Convert(object apiResponse, MethodType methodType, object criteria = null)
        {
            if (apiResponse != null)
            {
                var finalBookingRS = apiResponse as Tuple<BookingRES, List<TourTicketRES>,List<SelectedProduct>>;
                var finalProducts = ConvertPurchaseResult(finalBookingRS);
                return finalProducts;
            }
            return null;
        }

        private List<SelectedProduct> ConvertPurchaseResult(Tuple<BookingRES, List<TourTicketRES>, List<SelectedProduct>> finalBookingRS)
        {
            var finalBookingResult = finalBookingRS as Tuple<BookingRES, List<TourTicketRES>, List<SelectedProduct>>;

            var apibookingReponse = finalBookingResult?.Item1?.Result?.Details;
            var tourTicketResponseList = finalBookingResult?.Item2;
            var selectedProdList = finalBookingResult?.Item3;

            if (selectedProdList != null && selectedProdList.Count > 0)
            {
                foreach (var singleSelectedItem in selectedProdList)
                {
                    var selectedOptionId = System.Convert.ToString(singleSelectedItem?.ProductOptions?.FirstOrDefault()?.ServiceOptionId);
                    var apiBookingFilterResponse = apibookingReponse?.Where(x => x.ServiceUniqueId == selectedOptionId)?.FirstOrDefault();
                    var ticketbookingId = apiBookingFilterResponse?.BookingId;
                    var tourTicketFilterResponse = tourTicketResponseList?.Where(x => x.ResultTicket.BookingId == ticketbookingId)?.FirstOrDefault();

                    var raynaSelectedProduct = (RaynaSelectedProduct)singleSelectedItem;
                    //Case1
                    if (apiBookingFilterResponse?.Status.ToLower() == "success")
                    {
                        raynaSelectedProduct.DownloadRequired = apiBookingFilterResponse.DownloadRequired;
                        raynaSelectedProduct.OrderStatus = apiBookingFilterResponse?.Status;
                        raynaSelectedProduct.OrderReferenceId = finalBookingResult?.Item1?.Result?.ReferenceNo;
                        raynaSelectedProduct.BookingId = System.Convert.ToString(apiBookingFilterResponse?.BookingId);
                        raynaSelectedProduct.Success = true;
                    }
                    //Case 2:
                    var voucherData = tourTicketFilterResponse?.ResultTicket;
                    if (voucherData != null && !string.IsNullOrEmpty(voucherData.TicketURL))
                    {
                        raynaSelectedProduct.Success = true;
                        raynaSelectedProduct.TicketPdfUrl = voucherData.TicketURL;
                        raynaSelectedProduct.OrderReferenceId = voucherData.ReferenceNo;
                    }
                }
            }
            return selectedProdList;
         
        }
    }
}