namespace ServiceAdapters.BigBus.BigBus.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert(string objectResult);

        object Convert<T>(string response, T request);
    }
}