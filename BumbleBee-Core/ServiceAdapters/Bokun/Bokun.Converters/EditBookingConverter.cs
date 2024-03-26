using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Converters.Contracts;
using ServiceAdapters.Bokun.Bokun.Entities.EditBooking;
using Util;

namespace ServiceAdapters.Bokun.Bokun.Converters
{
    public class EditBookingConverter : ConverterBase, IEditBookingConverter
    {
        public EditBookingConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectResult">Generic model for Edit Booking Call</param>
        /// <param name="criteria">Generic request model </param>
        /// <returns></returns>
        public object Convert<T>(T objectResult, T criteria)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<EditBookingRs>(objectResult.ToString());

            return result;
        }
    }
}