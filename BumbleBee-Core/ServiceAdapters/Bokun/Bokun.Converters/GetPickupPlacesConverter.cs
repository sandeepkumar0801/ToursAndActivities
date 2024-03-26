using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Converters.Contracts;
using ServiceAdapters.Bokun.Bokun.Entities.GetPickupPlaces;
using Util;

namespace ServiceAdapters.Bokun.Bokun.Converters
{
    public class GetPickupPlacesConverter : ConverterBase, IGetPickupPlacesConverter
    {
        public GetPickupPlacesConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectResult">Generic model for Get pickup places Call</param>
        /// <param name="criteria">Generic request model</param>
        /// <returns></returns>
        public object Convert<T>(T objectResult, T criteria)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<GetPickupPlacesRS>(objectResult.ToString());
            return result;
        }
    }
}