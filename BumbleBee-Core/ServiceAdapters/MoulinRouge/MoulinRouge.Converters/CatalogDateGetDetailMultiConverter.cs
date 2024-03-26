using Logger.Contract;
using ServiceAdapters.MoulinRouge.MoulinRouge.Converters.Contracts;
using CatalogDateGetDetailMulti = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CatalogDateGetDetailMulti;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Converters
{
    public class CatalogDateGetDetailMultiConverter : ConverterBase, ICatalogDateGetDetailMultiConverter
    {
        public CatalogDateGetDetailMultiConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// Retun Converted object by converting Response from API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectResult"></param>
        /// <returns></returns>
        public override object Convert<T>(T objectResult)
        {
            return DeSerializeXml<CatalogDateGetDetailMulti.Response>(objectResult.ToString());
        }
    }
}