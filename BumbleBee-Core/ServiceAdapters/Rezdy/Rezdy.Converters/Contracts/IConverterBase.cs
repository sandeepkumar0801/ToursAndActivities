namespace ServiceAdapters.Rezdy.Rezdy.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert(string response);

        object Convert<T>(string response, T request);

        object Convert<T,A>(string response, T request,A apiResponse);
    }
}