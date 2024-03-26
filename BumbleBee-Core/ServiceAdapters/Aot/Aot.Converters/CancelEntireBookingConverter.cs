using Logger.Contract;
using ServiceAdapters.Aot.Aot.Converters.Contracts;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;

namespace ServiceAdapters.Aot.Aot.Converters
{
    public class CancelEntireBookingConverter : ConverterBase, ICancelEntireBookingConverter
    {
        public CancelEntireBookingConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectresult">string response Cancel Entire Booking Request call</param>
        /// <returns></returns>
        public override object Convert<T>(T objectresult, T inputRequest)
        {
            var result = DeSerializeXml<CancelServicesResponse>(objectresult as string);
            return result;
        }
    }
}