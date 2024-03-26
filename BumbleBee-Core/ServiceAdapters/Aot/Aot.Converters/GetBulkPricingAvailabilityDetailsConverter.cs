using Logger.Contract;
using ServiceAdapters.Aot.Aot.Converters.Contracts;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;

namespace ServiceAdapters.Aot.Aot.Converters
{
    public class GetBulkPricingAvailabilityDetailsConverter : ConverterBase, IGetBulkPricingAvailabilityDetailsConverter
    {
        public GetBulkPricingAvailabilityDetailsConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectresult">string response Option Avail Request call</param>
        /// <returns></returns>
        public override object Convert<T>(T objectresult, T inputRequest)
        {
            try
            {
                var result = DeSerializeXml<OptionAvailResponse>(objectresult as string);
                return result;
            }
            catch (System.InvalidOperationException)
            {
                return null;
            }
        }
    }
}