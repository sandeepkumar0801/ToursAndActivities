using ServiceAdapters.Rayna.Rayna.Converters.Contracts;
using ServiceAdapters.Rayna.Rayna.Entities;

namespace ServiceAdapters.Rayna.Rayna.Converters
{
    public class RaynaCancelConverter : ConverterBase, IRaynaCancelConverter
    {
      
        /// <summary>
        /// Convert API Result Entities to Isango.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>
        public object Convert(object apiResponse, MethodType methodType, object criteria = null)
        {
            var result = (CancelRES)apiResponse;
            return result != null ? ConvertAvailabilityResult(result) : null;
        }

        #region Private Methods

        private CancelRES ConvertAvailabilityResult(CancelRES result)
        {
            return result;
        }
        #endregion Private Methods
    }
}