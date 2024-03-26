using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.TourCMS;
using Logger.Contract;
using ServiceAdapters.TourCMS.TourCMS.Converters.Contracts;
using ServiceAdapters.TourCMS.TourCMS.Entities.NewBooking;
using Util;

namespace ServiceAdapters.TourCMS.TourCMS.Converters
{
    public class NewBookingConverter : ConverterBase, INewBookingConverter
    {

        public NewBookingConverter(ILogger logger) : base(logger)
        {
        }

        public override object Convert<T>(string response, T request)
        {
            var result = SerializeDeSerializeHelper.DeSerializeXml<NewBookingResponse>(response);
            if (result == null) return null;

            return ConvertReserevationResult(result, request as SelectedProduct);
        }

        private SelectedProduct ConvertReserevationResult(NewBookingResponse reservationResponse, SelectedProduct selectedProduct)
        {
            if (reservationResponse.Error.ToLowerInvariant() == "ok")
            {
                ((TourCMSSelectedProduct)selectedProduct).BookingId =System.Convert.ToInt32(reservationResponse.Booking.BookingId);
            }
            return selectedProduct;
        }

        public override object Convert(object objectResponse, object criteria)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Convert API Result Entities to Isnago.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>

        public object Convert(object apiResponse, ServiceAdapters.TourCMS.TourCMS.Entities.MethodType methodType, object criteria = null)
        {
            throw new NotImplementedException();
        }

        List<Activity> INewBookingConverter.ConvertAvailablityResult(object optionsFromAPI, object criteria)
        {
            throw new NotImplementedException();
        }

        object IConverterBase.Convert(object objectResult, object criteria)
        {
            throw new NotImplementedException();
        }
    }
}