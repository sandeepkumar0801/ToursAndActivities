using Isango.Entities;
using Isango.Entities.Activities;
using Logger.Contract;
using ServiceAdapters.TourCMS.TourCMS.Converters.Contracts;
using ServiceAdapters.TourCMS.TourCMS.Entities;
using ServiceAdapters.TourCMS.TourCMS.Entities.DeleteBookingResponse;
using Util;

namespace ServiceAdapters.TourCMS.TourCMS.Converters
{
    public class DeleteBookingConverter : ConverterBase, IDeleteBookingConverter
    {

        public DeleteBookingConverter(ILogger logger) : base(logger)
        {
        }
       
        public override object Convert<T>(string response, T request)
        {
            var result = SerializeDeSerializeHelper.DeSerializeXml
                <DeleteBookingResponse>(response);
            if (result == null) return null;

            return ConvertBookingResult(result);
        }

        private DeleteBookingResponse ConvertBookingResult(DeleteBookingResponse cancelBookingResponse)
        {
            try
            {
              return cancelBookingResponse;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TourCMS.DeleteBookingConverter",
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

        List<Activity> IDeleteBookingConverter.ConvertAvailablityResult(object optionsFromAPI, object criteria)
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