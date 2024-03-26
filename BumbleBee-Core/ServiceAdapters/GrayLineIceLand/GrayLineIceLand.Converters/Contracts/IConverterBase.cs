using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Converters.Contracts
{
    public interface IConverterBase
    {
        MethodType Converter { get; set; }

        object Convert(object objectResult);

        object Convert(object objectResult, object input);
    }
}