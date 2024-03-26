using ServiceAdapters.FareHarbor.FareHarbor.Entities;

namespace ServiceAdapters.FareHarbor.FareHarbor.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert<T>(T objectResult, object criteria);

        MethodType Converter { get; set; }
    }
}