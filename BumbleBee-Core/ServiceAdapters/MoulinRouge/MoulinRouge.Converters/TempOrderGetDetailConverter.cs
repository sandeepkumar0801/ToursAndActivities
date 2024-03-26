using Logger.Contract;
using ServiceAdapters.MoulinRouge.MoulinRouge.Converters.Contracts;
using TempOrderGetDetail = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.TempOrderGetDetail;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Converters
{
    public class TempOrderGetDetailConverter : ConverterBase, ITempOrderGetDetailConverter
    {
        public TempOrderGetDetailConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectResult">Response From API</param>
        /// <returns></returns>
        public override object Convert<T>(T objectResult)
        {
            var response = objectResult as TempOrderGetDetail.Response;
            var convertedResult = ConvertToResult(response);
            return convertedResult;
        }

        private object ConvertToResult(TempOrderGetDetail.Response response)
        {
            return response;
        }
    }
}