using Isango.Entities;
using Isango.Entities.Activities;
using Logger.Contract;
using ServiceAdapters.TourCMS.TourCMS.Converters.Contracts;
using ServiceAdapters.TourCMS.TourCMS.Entities;
using ServiceAdapters.TourCMS.TourCMS.Entities.CancelBookingResponse;
using Util;

namespace ServiceAdapters.TourCMS.TourCMS.Converters
{
    public class CancelBookingConverter : ConverterBase, ICancelBookingConverter
    {

        public CancelBookingConverter(ILogger logger) : base(logger)
        {
        }
       
        public override object Convert<T>(string response, T request)
        {
            var result = SerializeDeSerializeHelper.DeSerializeXml
                <CancelBookingResponse>(response);
            if (result == null) return null;

            return ConvertBookingResult(result);
        }

        private CancelBookingResponse ConvertBookingResult(CancelBookingResponse cancelBookingResponse)
        {
            try
            {
              return cancelBookingResponse;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TourCMS.CancelBookingConverter",
                    MethodName = "ConvertBookingResult"
                };
                _logger.Error(isangoErrorEntity, ex);
                throw; 
            }
        }

        public override object Convert(object objectResponse, object criteria)
        {
            throw new NotImplementedException();
        }

        List<Activity> ICancelBookingConverter.ConvertAvailablityResult(object optionsFromAPI, object criteria)
        {
            throw new NotImplementedException();
        }

        object IConverterBase.Convert(object apiResponse, MethodType methodType, object criteria)
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

        object IConverterBase.Convert(object objectResult, object criteria)
        {
            throw new NotImplementedException();
        }

    }
}