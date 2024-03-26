using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters.Contracts;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities;
using Util;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters
{
    public class NewCitySightSeeingCancellationConverter : ConverterBase, INewCitySightSeeingCancellationConverter
    {
        /// <summary>
        /// Convert API Result Entities to Isnago.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>

        public object Convert(object apiResponse, MethodType methodType, object criteria = null)
        {
            var result = SerializeDeSerializeHelper.DeSerialize
                 <string>(apiResponse.ToString());
            if (result == null) return null;

            return ConvertPurchaseResult(result);
        }

        private string ConvertPurchaseResult(string cancelRS)
        {
             return cancelRS;
        }
    }
}