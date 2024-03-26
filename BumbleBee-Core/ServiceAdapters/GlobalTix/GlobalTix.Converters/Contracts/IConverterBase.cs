using ServiceAdapters.GlobalTix.GlobalTix.Entities;

namespace ServiceAdapters.GlobalTix.GlobalTix.Converters.Contracts
{
    public interface IConverterBase
    {
        MethodType Converter { get; set; }
        object Convert(object objectResult);
        object Convert(object objectResult, object input);
    }
}
