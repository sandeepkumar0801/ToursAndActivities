using Logger.Contract;
using ServiceAdapters.GoldenTours.GoldenTours.Converters.Contracts;
using ServiceAdapters.GoldenTours.GoldenTours.Entities.Availability;

namespace ServiceAdapters.GoldenTours.GoldenTours.Converters
{
    public class AvailabilityConverter : ConverterBase, IAvailabilityConverter
    {
        public AvailabilityConverter(ILogger logger) : base(logger)
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
            var result = DeSerializeXml<AvailabilityResponse>(response as string);
            return result;
        }
    }
}