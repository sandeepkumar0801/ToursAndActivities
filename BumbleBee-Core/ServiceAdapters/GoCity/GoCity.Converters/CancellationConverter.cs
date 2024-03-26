using ServiceAdapters.GoCity.GoCity.Converters.Contracts;
using ServiceAdapters.GoCity.GoCity.Entities;
using ServiceAdapters.GoCity.GoCity.Entities.CancelBooking;

namespace ServiceAdapters.GoCity.GoCity.Converters
{
    public class GoCityCancellationConverter : ConverterBase, IGoCityCancellationConverter
    {
        /// <summary>
        /// Convert API Result Entities to Isnago.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>

        public object Convert(object apiResponse, MethodType methodType, object criteria = null)
        {
            var result = apiResponse as CancelBookingResponse;
            if (result == null) return null;

            return ConvertPurchaseResult(result);
        }

        private bool ConvertPurchaseResult(CancelBookingResponse cancelRS)
        {
             return cancelRS.SuccessStatus;
        }
    }
}