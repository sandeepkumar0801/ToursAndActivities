using Logger.Contract;
using ServiceAdapters.GoldenTours.GoldenTours.Converters.Contracts;
using ServiceAdapters.GoldenTours.GoldenTours.Entities.GetBookingDates;

namespace ServiceAdapters.GoldenTours.GoldenTours.Converters
{
    public class GetBookingDatesConverter : ConverterBase, IGetBookingDatesConverter
    {
        public GetBookingDatesConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override object Convert<T>(T response, T request)
        {
            var result = DeSerializeXml<GetBookingDatesResponse>(response as string);
            return result;
        }
    }
}