using ServiceAdapters.MoulinRouge.MoulinRouge.Entities;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Converters.Contracts
{
    public interface IConverterBase
    {
        MethodType Converter { get; set; }

        object Convert<T>(T objectResult);

        T DeSerializeXml<T>(string responseXmlString);
    }
}