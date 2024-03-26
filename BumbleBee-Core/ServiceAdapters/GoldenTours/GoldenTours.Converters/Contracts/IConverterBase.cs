namespace ServiceAdapters.GoldenTours.GoldenTours.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert<T>(T response);

        object Convert<T>(T response, T request);

        object Convert<T>(T response, T request, T extraRequest);

        object Convert<T>(T response, T request, T extraRequest, T extraResponse);

        T DeSerializeXml<T>(string responseXmlString);
    }
}