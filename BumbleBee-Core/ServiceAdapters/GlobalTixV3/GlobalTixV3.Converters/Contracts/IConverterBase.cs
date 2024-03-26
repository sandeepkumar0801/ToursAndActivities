using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Converters.Contracts
{
    public interface IConverterBase
    {
        MethodType Converter { get; set; }
        object Convert(object objectResult);
        object Convert(object objectResult, object input);
        object Convert(object objectResult, object objectDetailResult,  object input);
    }
}
