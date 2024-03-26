using Isango.Entities;
using Isango.Entities.NewCitySightSeeing;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters.Contracts;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Reservation;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters
{
    public class NewCitySightSeeingReservationConverter : ConverterBase, INewCitySightSeeingReservationConverter
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
                var reservationRS = apiResponse as ReservationResponse;
                var selectedProduct = criteria as SelectedProduct;
                var selectedProducts = ConvertPurchaseResult(reservationRS, selectedProduct);
                return selectedProducts;
            }
            return null;
        }

        private SelectedProduct ConvertPurchaseResult(ReservationResponse reservationRS, SelectedProduct selectedProduct)
        {
            if (!string.IsNullOrEmpty(reservationRS.ReservationId))
            {
                ((NewCitySightSeeingSelectedProduct)selectedProduct).NewCitySightSeeingReservationId = reservationRS.ReservationId;
            }
            return selectedProduct;
        }
    }
}