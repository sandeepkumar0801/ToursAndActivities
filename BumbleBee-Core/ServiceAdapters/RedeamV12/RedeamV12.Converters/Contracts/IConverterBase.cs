namespace ServiceAdapters.RedeamV12.RedeamV12.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert<T>(T response);

        object Convert<T>(T response, T request);

        object Convert<T>(T response, T request, T extraRequest);

        object Convert<T>(T response, T request, T extraRequest, T pricing);
    }
}