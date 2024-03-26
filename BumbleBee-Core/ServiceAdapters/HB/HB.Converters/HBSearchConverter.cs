using ServiceAdapters.HB.HB.Converters.Contracts;
using ServiceAdapters.HB.HB.Entities;
using Search = ServiceAdapters.HB.HB.Entities.Search;

namespace ServiceAdapters.HB.HB.Converters
{
    public class HBSearchConverter : ConverterBase, IHbSearchConverter
    {
        /// <summary>
        /// Convert API Result Entities to Isnago.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>

        public object Convert(object apiResponse, MethodType methodType, object criteria = null)
        {
            var result = (Search.SearchRs)apiResponse;
            return result;
        }
    }
}