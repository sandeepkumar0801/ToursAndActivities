using Logger.Contract;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Converters.Contracts;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;
using Util;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Converters
{
    public  class ReservationConverter : ConverterBase, IReservationConverter
    {
        public ReservationConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to isango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectResult">string response Companies Call</param>
        /// <param name="criteria"></param>
        /// <returns>Isango.Contracts.Entities.Supplier List Object</returns>
        public override object Convert<T>(T objectResult, object criteria)
        {
            try
            {

                var result = SerializeDeSerializeHelper.DeSerialize<ReservationResponse>(objectResult as string);

                if (result != null)
                {
                    //var bookingresult = ConvertProductsResult(result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, or take appropriate action
                Console.WriteLine($"An error occurred during conversion: {ex.Message}");
            }

            return null;
        }
    }
}
