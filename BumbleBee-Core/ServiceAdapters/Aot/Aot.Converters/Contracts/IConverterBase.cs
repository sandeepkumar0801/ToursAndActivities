using ServiceAdapters.Aot.Aot.Entities;

namespace ServiceAdapters.Aot.Aot.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert<T>(T objectresult);

        T DeSerializeXml<T>(string responseXmlString);

        MethodType Converter { get; set; }

        object Convert<T>(T inputContext, T inputRequest);

        object Convert<T>(T objectresultdict, T inputRequest, T request);
    }
}