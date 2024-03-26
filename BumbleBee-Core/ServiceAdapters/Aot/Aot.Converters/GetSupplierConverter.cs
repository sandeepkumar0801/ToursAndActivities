using Logger.Contract;
using ServiceAdapters.Aot.Aot.Converters.Contracts;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;

namespace ServiceAdapters.Aot.Aot.Converters
{
    public class GetSupplierConverter : ConverterBase, IGetSupplierConverter
    {
        public GetSupplierConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectresult">string response Get Supplier Call</param>
        /// <returns></returns>
        public override object Convert<T>(T objectresult)
        {
            var result = DeSerializeXml<SupplierInfoResponse>(objectresult as string);
            return result;
        }
    }
}