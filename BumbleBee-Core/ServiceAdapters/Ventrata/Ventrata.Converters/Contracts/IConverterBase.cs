using ServiceAdapters.Ventrata.Ventrata.Entities;

namespace ServiceAdapters.Ventrata.Ventrata.Converters.Contracts
{
    public interface IConverterBase
    {
        MethodType Converter { get; set; }

        object Convert(object objectResult, object criteria);
        object Convert(object objectResult);
    }
}
