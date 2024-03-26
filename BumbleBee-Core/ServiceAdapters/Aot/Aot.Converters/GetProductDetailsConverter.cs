using Isango.Entities.ConsoleApplication.AgeGroup.AOT;
using Logger.Contract;
using ServiceAdapters.Aot.Aot.Converters.Contracts;

namespace ServiceAdapters.Aot.Aot.Converters
{
    public class GetProductDetailsConverter : ConverterBase, IGetProductDetailsConverter
    {
        public GetProductDetailsConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectresult">string response Get Product Details</param>
        /// <returns></returns>
        public override object Convert<T>(T objectresult)
        {
            var result = DeSerializeXml<OptionGeneralInfoResponse>(objectresult as string);
            return result;
        }
    }
}