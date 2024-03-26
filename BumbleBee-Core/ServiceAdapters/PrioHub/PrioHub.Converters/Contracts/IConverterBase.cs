using ServiceAdapters.PrioHub.PrioHub.Entities;

namespace ServiceAdapters.PrioHub.PrioHub.Converters.Contracts
{
    public interface IConverterBase
    {
        MethodType Converter { get; set; }

        object Convert(object objectResult);
    }
}