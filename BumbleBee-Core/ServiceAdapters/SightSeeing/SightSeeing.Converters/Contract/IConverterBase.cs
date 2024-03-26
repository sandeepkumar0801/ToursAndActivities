using ServiceAdapters.SightSeeing.SightSeeing.Entities;

namespace ServiceAdapters.SightSeeing.SightSeeing.Converters.Contract
{
    public interface IConverterBase
    {
        MethodType Converter { get; set; }

        object Convert(object objectResult);

        object Convert<T>(string response, T request);
    }
}