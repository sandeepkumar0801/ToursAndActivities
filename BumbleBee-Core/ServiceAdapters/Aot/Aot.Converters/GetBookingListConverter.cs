using Logger.Contract;
using ServiceAdapters.Aot.Aot.Converters.Contracts;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;

namespace ServiceAdapters.Aot.Aot.Converters
{
    public class GetBookingListConverter : ConverterBase, IGetBookingListConverter
    {
        public GetBookingListConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectresult">string response get booking list Request call</param>
        /// <returns></returns>
        public override object Convert<T>(T objectresult, T inputRequest)
        {
            var result = DeSerializeXml<ListBookingsResponse>(objectresult as string);
            return result;
        }
    }
}